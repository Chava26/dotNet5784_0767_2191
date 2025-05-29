
namespace BlImplementation;
using BlApi;
using Helpers;
using System;
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
            var (latitude, longitude) = VolunteerManager.logicalChecking(boVolunteer);
            if(latitude != null && longitude != null)
            {
                // Update the properties of the BOVolunteer instance
                boVolunteer.Latitude = latitude;
                boVolunteer.Longitude = longitude;
            }
            // Prepare DO.Volunteer object 
            DO.Volunteer doVolunteer = VolunteerManager.CreateDoVolunteer(boVolunteer);
            _dal.Volunteer.Create(doVolunteer);
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5

        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }
        catch (Exception ex)
        {
            // Handle all other unexpected exceptions
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while adding the volunteer.", ex);
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

            // Check if the volunteer can be deleted
            IEnumerable<DO.Assignment> assignmentsWithVolunteer = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);
            if (assignmentsWithVolunteer is not null)
                throw new BO.BlInvalidFormatException("Volunteer cannot be deleted because they are or have been assigned to tasks.");
            _dal.Volunteer.Delete(volunteerId);
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5

        }

        catch (InvalidOperationException ex)
        {
            // Handle logical business errors, such as tasks assigned to the volunteer
            throw new BO.VolunteerDeletionException($"Unable to delete volunteer with ID {volunteerId} because they are assigned to tasks.", ex);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // Handle the case where the volunteer does not exist in the database
            throw new BO.VolunteerDeletionException($"Error deleting volunteer with ID {volunteerId}. Volunteer not found.", ex);
        }
        catch (Exception ex)
        {
            // Handle all other unexpected exceptions
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while deleting the volunteer.", ex);
        }
    }


    /// <summary>
    /// Retrieves and filters a list of volunteers based on their activity status and sorts them based on a specified field.
    /// </summary>
    /// <param name="isActive">A nullable boolean to filter volunteers by their activity status. If null, no filtering is applied.</param>
    /// <param name="sortBy">A nullable enumeration for sorting the list by a specific volunteer field.</param>
    /// <returns>An ordered and filtered list of volunteer entities in the business logic layer.</returns>
  public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerSortField? sortBy = null, BO.CallType? filterField = null)
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
        }).Where(vol => !filterField.HasValue || vol.CurrentCallType == filterField.Value);

        // Sort the volunteer list based on the provided sorting criterion
        return sortBy switch
        {
            BO.VolunteerSortField.Name => volunteerList.OrderBy(v => v.FullName),

            BO.VolunteerSortField.TotalHandledCalls => volunteerList.OrderByDescending(v => v.TotalHandledCalls),
            BO.VolunteerSortField.TotalCanceledCalls => volunteerList.OrderByDescending(v => v.TotalCanceledCalls),
            BO.VolunteerSortField.TotalExpiredCalls => volunteerList.OrderByDescending(v => v.TotalExpiredCalls),
            //BO.VolunteerSortField.SumOfCalls => volunteerList.OrderBy(v => v.TotalHandledCalls),
            BO.VolunteerSortField.SumOfCancellation => volunteerList.OrderBy(v => v.TotalCanceledCalls),
            BO.VolunteerSortField.SumOfExpiredCalls => volunteerList.OrderBy(v => v.TotalExpiredCalls),
            _ => volunteerList.OrderBy(v => v.Id) // Default: Sort by volunteer ID
        };
    }
    catch (DO.DalDoesNotExistException ex)
    {
        // Catch data access exceptions and rethrow as business logic exceptions
        throw new BO.BlGeneralDatabaseException("Error accessing data.", ex);
    }
        catch (Exception ex)
        {
            // Handle all other unexpected exceptions
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while geting Volunteers.", ex);
        }
    }


    /// <summary>
    /// Retrieves the details of a volunteer, including any active call assignment they are handling.
    /// </summary>
    /// <param name="volunteerId">The unique ID of the volunteer.</param>
    /// <returns>
    /// A <see cref="BO.Volunteer"/> object containing the volunteer's details.
    /// If the volunteer has an active assignment, it will include a <see cref="BO.CallInProgress"/> object.
    /// </returns>
    /// <exception cref="BO.BlDoesNotExistException">
    /// Thrown if the volunteer does not exist in the data layer or there is an issue accessing their data.
    /// </exception>
    /// <remarks>
    /// This method queries the data layer to fetch volunteer details and their related call assignments.
    /// If there is no active call assignment, the <see cref="BO.Volunteer.CallInProgress"/> property will be null.
    /// </remarks>
    public BO.Volunteer GetVolunteerDetails(int volunteerId)
    {
        try
        {
            var doVolunteer = _dal.Volunteer.Read(volunteerId) ??
               throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist");
            var assigments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);
            var activeAssignment = assigments.FirstOrDefault(a => a.exitTime == null);
            BO.CallInProgress? boCallInProgress = null;
            if (activeAssignment is not null)
            {
                    var callDetails = _dal.Call.Read(activeAssignment.CallId);
                    if (callDetails is not null)
                    {
                        boCallInProgress = new BO.CallInProgress
                        {
                            Id = activeAssignment.Id,
                            CallId = activeAssignment.CallId,
                            Type = (BO.CallType)callDetails.MyCallType,
                            Address = callDetails.Address,
                            OpenTime = callDetails.OpenTime,
                            EntryTime = activeAssignment.EntryTime,
                            DistanceFromVolunteer = Tools.CalculateDistance(doVolunteer.Latitude??0, doVolunteer.Longitude??0, callDetails.Latitude, callDetails.Longitude),
                            Status = Tools.CalculateStatus(activeAssignment, callDetails, 30)
                        };
                    }
            }
            return VolunteerManager.CreateBoVolunteer(doVolunteer, boCallInProgress);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException("Volunteer not found in data layer.", ex);
        }
        catch (Exception ex)
        {
            // Handle all other unexpected exceptions
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while geting Volunteer details.", ex);
        }
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
            IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll(v => v.Name == username);

            DO.Volunteer? matchingVolunteer = volunteers.FirstOrDefault(v => VolunteerManager.VerifyPassword(password, v.Password!)) ?? throw new BO.BlDoesNotExistException("Incorrect username or password."); 

            return (BO.Role)matchingVolunteer.role;
        }
        catch (BO.BlDoesNotExistException ex)
        {
            throw ;
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while login.", ex);
        }
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
    public void UpdateVolunteer(int requesterId, BO.Volunteer VolunteerForUpdate)
    {

        try
        {
            DO.Volunteer requester = _dal.Volunteer.Read(requesterId) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={requesterId} does not  exists and can not update other Volunteer");
            // Ensure permissions are correct
            VolunteerManager.ValidatePermissions(requester, VolunteerForUpdate);

            // Validate input format and basic structure
            VolunteerManager.ValidateInputFormat(VolunteerForUpdate);

            // Validate logical rules for the volunteer
            (VolunteerForUpdate.Latitude, VolunteerForUpdate.Longitude) = VolunteerManager.logicalChecking(VolunteerForUpdate);

            DO.Volunteer originalVolunteer = _dal.Volunteer.Read(VolunteerForUpdate.Id)!;
            var changedFields = VolunteerManager.GetChangedFields(originalVolunteer, VolunteerForUpdate);
            VolunteerManager.CanUpdateFields(requester.role, changedFields, VolunteerForUpdate);

            // Prepare DO.Volunteer object for data layer update
            DO.Volunteer doVolunteer = VolunteerManager.CreateDoVolunteer(VolunteerForUpdate);

            _dal.Volunteer.Update(doVolunteer); // Attempt to update the data layer
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5
            VolunteerManager.Observers.NotifyItemUpdated(doVolunteer.Id); //stage 5

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException($"Volunteer with ID={VolunteerForUpdate.Id} does not  exists", ex);

        }

        catch (Exception ex)
        {
            // Handle all other unexpected exceptions
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while update.", ex);
        }
    }
    #region Stage 5
    public void AddObserver(Action listObserver) =>
    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5


}
