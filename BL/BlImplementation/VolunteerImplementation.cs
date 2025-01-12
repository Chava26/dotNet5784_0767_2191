
namespace BlImplementation;
using BlApi;
using BO;
using DO;
using Helpers;
using System.Numerics;
using System.Xml.Linq;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddVolunteer(BO.Volunteer boVolunteer)
    {
        

        try
        {
            // Validate input format and basic structure
            Helpers.VolunteerManager.ValidateInputFormat(boVolunteer);

            // Validate logical rules for the volunteer
            var (latitude, longitude) = VolunteerManager.GetCoordinatesFromAddress(boVolunteer.Address);

            // Update the properties of the BOVolunteer instance
            boVolunteer.Latitude = latitude;
            boVolunteer.Longitude = longitude;
            // Prepare DO.Volunteer object 
            DO.Volunteer doVolunteer = VolunteerManager.CreateDoVolunteer(boVolunteer);
            _dal.Volunteer.Create(doVolunteer);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }
        catch (Exception ex)
        {
            // Handle all other unexpected exceptions
            throw new BO.GeneralDatabaseException("An unexpected error occurred while adding the volunteer.", ex);
        }

    }


    /// <summary>
    /// Deletes a volunteer by their ID.
    /// Validates that the volunteer can be deleted before attempting the deletion.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer to delete.</param>
    /// <exception cref="ArgumentException">Thrown when the input ID is invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the volunteer cannot be deleted.</exception>
    /// <exception cref="ApplicationException">Thrown when an error occurs in the data layer.</exception>
    public void DeleteVolunteer(int volunteerId)
    {
        try
        {
            // Validate input ID format
            //if (!Helpers.Tools.IsValidId(id))
            //    throw new ArgumentException("Invalid volunteer ID format.");

            // Check if the volunteer can be deleted
            DO.Assignment volunteerTasks = _dal.Assignment.Read(volunteerId);
            if (volunteerTasks is not null)
                throw new InvalidOperationException("Volunteer cannot be deleted because they are or have been assigned to tasks.");
            _dal.Assignment.Delete(volunteerId);
        }
        catch (ArgumentException ex)
        {
            // Handle invalid ID format
            throw new BO.VolunteerDeletionException($"Invalid ID format for volunteer ID: {id}.", ex);
        }
        catch (InvalidOperationException ex)
        {
            // Handle logical business errors, such as tasks assigned to the volunteer
            throw new BO.VolunteerDeletionException($"Unable to delete volunteer with ID {id} because they are assigned to tasks.", ex);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // Handle the case where the volunteer does not exist in the database
            throw new BO.VolunteerDeletionException($"Error deleting volunteer with ID {id}. Volunteer not found.", ex);
        }
        catch (Exception ex)
        {
            // Handle all other unexpected exceptions
            throw new BO.GeneralDatabaseException("An unexpected error occurred while deleting the volunteer.", ex);
        }
    }


    /// <summary>
    /// Retrieves and filters a list of volunteers based on their activity status and sorts them based on a specified field.
    /// </summary>
    /// <param name="isActive">A nullable boolean to filter volunteers by their activity status. If null, no filtering is applied.</param>
    /// <param name="sortBy">A nullable enumeration for sorting the list by a specific volunteer field.</param>
    /// <returns>An ordered and filtered list of volunteer entities in the business logic layer.</returns>
  public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerSortField? sortBy = null)
{
    try
    {
        // Fetch all volunteers, applying an optional filter for activity status
        IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll(v =>
            !isActive.HasValue || v.IsActive == isActive.Value);

        // Map the data to the business object VolunteerInList
        var volunteerList = volunteers.Select(v =>
        {
            // Retrieve all assignments for the volunteer
            var volunteerAssignments = _dal.Assignment.ReadAll(a => a.VolunteerId == v.Id);

            // Count different types of assignments (Handled, Canceled, Expired)
            var totalHandled = volunteerAssignments.Count(a => a.TypeOfEndTime == DO.EndOfTreatment.treated);
            var totalCanceled = volunteerAssignments.Count(a => a.TypeOfEndTime == DO.EndOfTreatment.administratorCancel);
            var totalExpired = volunteerAssignments.Count(a => a.TypeOfEndTime == DO.EndOfTreatment.expired);

            // Get the current assignment if available
            var currentAssignment = volunteerAssignments.FirstOrDefault(a => a.exitTime == null);
            var currentCallId = currentAssignment?.CallId;

            return new BO.VolunteerInList
            {
                Id = v.Id,
                FullName = v.Name,
                IsActive = v.IsActive,
                TotalHandledCalls = totalHandled,
                TotalCanceledCalls = totalCanceled,
                TotalExpiredCalls = totalExpired,
                CurrentCallId = currentCallId,
                CurrentCallType = currentCallId.HasValue
                    ? (BO.CallType)(_dal.Call.Read(currentCallId.Value)?.MyCallType ?? DO.CallType.None)
                    : BO.CallType.None
            };
        });

        // Sort the volunteer list based on the provided sorting criterion
        return sortBy switch
        {
            BO.VolunteerSortField.SumOfCalls => volunteerList.OrderBy(v => v.TotalHandledCalls),
            BO.VolunteerSortField.SumOfCancellation => volunteerList.OrderBy(v => v.TotalCanceledCalls),
            BO.VolunteerSortField.SumOfExpiredCalls => volunteerList.OrderBy(v => v.TotalExpiredCalls),
            _ => volunteerList.OrderBy(v => v.Id) // Default: Sort by volunteer ID
        };
    }
    catch (DO.DalDoesNotExistException ex)
    {
        // Catch data access exceptions and rethrow as business logic exceptions
        throw new BO.GeneralDatabaseException("Error accessing data.", ex);
    }
}



    

    public BO.Volunteer GetVolunteerDetails(int volunteerId)
    {
        var doVolunteer = _dal.Volunteer.Read(volunteerId) ??
        throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does Not exist");
        var doAssignment = _dal.Assignment.Read(volunteerId);
        if (doAssignment is not null)
        {
            var doCall = _dal.Call.Read(volunteerId);

            BO.CallInProgress boCallInProgress = new BO.CallInProgress() { 
                Id = doAssignment.Id,
            CallId = doAssignment.CallId,
            Type = doCall.MyCallType,
            Address = doCall.Address,
            OpenTime = doCall.OpenTime,
            EntryTime = doAssignment.EntryTime,
            DistanceFromVolunteer = distanceFromVolunteer,
            Status = status
            };
        }
        return new()
        {
            Id = volunteerId,
            Email= doVolunteer.Email,
            PhoneNumber= doVolunteer.Phone,
            Role= doVolunteer.role,
            IsActive= doVolunteer.IsActive,
            MaxDistanceForTask= doVolunteer.MaximumDistance,
            Password= doVolunteer.Password,
            Address= doVolunteer.Adress,
            Longitude= doVolunteer.Longitude,
            Latitude = doVolunteer.Latitude,
            DistanceType= doVolunteer.DistanceType,
            callInProgress= boCallInProgress
        };
    }


    /// <summary>
    /// Authenticates a user based on username and password.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="password">The plain text password provided by the user.</param>
    /// <returns>The role of the user if authentication is successful.</returns>
    /// <exception cref="BO.AuthenticationException">Thrown if the username or password is incorrect.</exception>
    /// <exception cref="BO.DataAccessException">Thrown if there is an issue accessing data.</exception>
    public BO.Role Login(string username, string password)
    {
        try
        {
            // Fetch the user from the data layer
            var user = _dal.Volunteer.Read(v => v.Username == username).FirstOrDefault();

            // Check if the user exists
            if (user == null)
            {
                throw new BO.AuthenticationException("User does not exist.");
            }

            // Verify the password (decrypting the stored password and comparing)
            if (!Helpers.VolunteerManager.VerifyPassword(password, user.EncryptedPassword))
            {
                throw new BO.AuthenticationException("Incorrect password.");
            }

            // Return the user's role
            return (BO.Role)user.Role;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException("Error accessing data.", ex);
        }
    }


    public void UpdateVolunteer(int requesterId, BO.Volunteer volunteer)
    {
        throw new NotImplementedException();
    }
  
    /// <summary>
    /// Updates the details of a volunteer.
    /// Validates input, permissions, and logical constraints before updating the data layer.
    /// </summary>
    /// <param name="requesterId">The ID of the user requesting the update.</param>
    /// <param name="boVolunteer">The BO.Volunteer object containing updated details.</param>
    /// <exception cref="ArgumentException">Thrown when input data is invalid.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the requester lacks permissions.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the volunteer does not exist.</exception>
    /// <exception cref="ApplicationException">Thrown when an error occurs in the data layer.</exception>
    public void UpdateVolunteer(int requesterId, BO.Volunteer boVolunteer)
    {
       

       
        try
        {
            //if (string.IsNullOrWhiteSpace(requesterId))
            //    throw new ArgumentException("Requester ID cannot be null or empty.");
            // Validate input format and basic structure
            Helpers.VolunteerManager.ValidateInputFormat( boVolunteer);
            //Helpers.VolunteerManager.logicalChecking( requesterId, boVolunteer)
          // Validate logical rules for the volunteer
          var (latitude, longitude) = VolunteerManager.GetCoordinatesFromAddress(boVolunteer.Address);

            // Update the properties of the BOVolunteer instance
            boVolunteer.Latitude = latitude;
            boVolunteer.Longitude = longitude;

            // Ensure permissions are correct
            Helpers.VolunteerManager.ValidatePermissions(requesterId, boVolunteer);

            // Prepare DO.Volunteer object for data layer update
            DO.Volunteer doVolunteer = Helpers.VolunteerManager.CreateDoVolunteer(boVolunteer);
            _dal.Volunteer.Update(doVolunteer); // Attempt to update the data layer
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException($"Volunteer with ID={boVolunteer.Id} already exists", ex);

        }

        catch (Exception ex)
        {
            // Handle all other unexpected exceptions
            throw new BO.GeneralDatabaseException("An unexpected error occurred while update.", ex);
        }
    }

}
