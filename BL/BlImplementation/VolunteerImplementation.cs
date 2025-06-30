
//namespace BlImplementation;
//using BlApi;
//using BO;
//using Helpers;
//using System;
//using System.Numerics;
//using System.Xml.Linq;

//internal class VolunteerImplementation : IVolunteer
//{
//    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

//    public void AddVolunteer(BO.Volunteer boVolunteer)
//    {
//        try
//        {
//            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

//            // Validate input format and basic structure
//            Helpers.VolunteerManager.ValidateInputFormat(boVolunteer);
//            // Validate logical rules for the volunteer
//            var (latitude, longitude) = VolunteerManager.logicalChecking(boVolunteer);
//            if (latitude != null && longitude != null)
//            {
//                // Update the properties of the BOVolunteer instance
//                boVolunteer.Latitude = latitude;
//                boVolunteer.Longitude = longitude;
//            }
//            // Prepare DO.Volunteer object 
//            VolunteerManager.EncryptPassword(boVolunteer.Password);
//            DO.Volunteer doVolunteer = VolunteerManager.CreateDoVolunteer(boVolunteer);
//            _dal.Volunteer.Create(doVolunteer);
//            VolunteerManager.Observers.NotifyListUpdated(); //stage 5

//        }
//        catch (BLTemporaryNotAvailableException)
//        {
//            throw;
//        }
//        catch (DO.DalAlreadyExistsException ex)
//        {
//            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
//        }
//        catch (BO.BlInvalidFormatException )
//        {
//            throw ;
//        }

//        catch (Exception ex)
//        {
//            // Handle all other unexpected exceptions
//            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while adding the volunteer.", ex);
//        }

//    }


//    /// <summary>
//    /// Deletes a volunteer by their ID.
//    /// Validates that the volunteer can be deleted before attempting the deletion.
//    /// </summary>
//    /// <param name="volunteerId">The ID of the volunteer to delete.</param>
//    /// <exception cref="ArgumentException">Thrown when the input ID is invalid.</exception>
//    /// <exception cref="InvalidOperationException">Thrown when the volunteer cannot be deleted.</exception>
//    /// <exception cref="BlDeletionException">Thrown when an error occurs in the data layer.</exception>
//    public void DeleteVolunteer(int volunteerId)
//    {
//        try
//        {
//            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

//            // Check if the volunteer can be deleted
//            IEnumerable<DO.Assignment> assignmentsWithVolunteer = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);
//            if (assignmentsWithVolunteer.Any())
//                throw new BO.InvalidOperationException("Volunteer cannot be deleted because they are or have been assigned to tasks.");
//            _dal.Volunteer.Delete(volunteerId);
//            VolunteerManager.Observers.NotifyListUpdated(); //stage 5

//        }

//        catch (BLTemporaryNotAvailableException)
//        {
//            throw;
//        }
//        catch (BO.InvalidOperationException ex)
//        {
//            // Handle logical business errors, such as tasks assigned to the volunteer
//            throw new BO.BlDeletionException($"Unable to delete volunteer with ID {volunteerId} because they are assigned to tasks.", ex);
//        }
//        catch (DO.DalDoesNotExistException ex)
//        {
//            // Handle the case where the volunteer does not exist in the database
//            throw new BO.BlDeletionException($"Error deleting volunteer with ID {volunteerId}. Volunteer not found.", ex);
//        }
//        catch (Exception ex)
//        {
//            // Handle all other unexpected exceptions
//            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while deleting the volunteer.", ex);
//        }
//    }


//    /// <summary>
//    /// Retrieves and filters a list of volunteers based on their activity status and sorts them based on a specified field.
//    /// </summary>
//    /// <param name="isActive">A nullable boolean to filter volunteers by their activity status. If null, no filtering is applied.</param>
//    /// <param name="sortBy">A nullable enumeration for sorting the list by a specific volunteer field.</param>
//    /// <returns>An ordered and filtered list of volunteer entities in the business logic layer.</returns>
//    public IEnumerable<BO.VolunteerInList> GetVolunteersList(
//            bool? isActive = null,
//            BO.VolunteerSortField? sortBy = null,
//            BO.CallType? filterField = null)
//    {
//        try
//        {
//            //  טעינה ראשונית
//            var volunteers = _dal.Volunteer
//                .ReadAll(v => !isActive.HasValue || v.IsActive == isActive.Value)
//                .ToList();

//            var assignmentsByVolunteer = _dal.Assignment.ReadAll()
//                .GroupBy(a => a.VolunteerId)
//                .ToDictionary(g => g.Key, g => g.ToList());

//            var callsById = _dal.Call.ReadAll().ToDictionary(c => c.Id);

//            // השלכת הנתונים לאובייקט העסקי
//            var list = volunteers.Select(v =>
//            {
//                var aList = assignmentsByVolunteer.TryGetValue(v.Id, out var l) ? l : new List<DO.Assignment>();

//                // סיכומים
//                var handled = aList.Count(a => a.TypeOfEndTime == DO.EndOfTreatment.treated);
//                var canceled = aList.Count(a => a.TypeOfEndTime == DO.EndOfTreatment.administratorCancel);
//                var expired = aList.Count(a => a.TypeOfEndTime == DO.EndOfTreatment.expired);

//                // הקצאה פעילה
//                var current = aList.FirstOrDefault(a => a.exitTime == null);
//                var callType = current?.CallId is int cid && callsById.TryGetValue(cid, out var c)
//                               ? (BO.CallType)c.MyCallType
//                               : BO.CallType.None;

//                return new BO.VolunteerInList
//                {
//                    Id = v.Id,
//                    FullName = v.Name,
//                    IsActive = v.IsActive,
//                    TotalHandledCalls = handled,
//                    TotalCanceledCalls = canceled,
//                    TotalExpiredCalls = expired,
//                    CurrentCallId = current?.CallId,
//                    CurrentCallType = callType
//                };
//            })
//            .Where(vol => !filterField.HasValue || vol.CurrentCallType == filterField.Value)
//            .ToList();          // חומריזציה

//            // מיון
//            return sortBy switch
//            {
//                BO.VolunteerSortField.Name => list.OrderBy(v => v.FullName),
//                BO.VolunteerSortField.TotalHandledCalls => list.OrderByDescending(v => v.TotalHandledCalls),
//                BO.VolunteerSortField.TotalCanceledCalls => list.OrderByDescending(v => v.TotalCanceledCalls),
//                BO.VolunteerSortField.TotalExpiredCalls => list.OrderByDescending(v => v.TotalExpiredCalls),
//                BO.VolunteerSortField.SumOfCancellation => list.OrderByDescending(v => v.TotalCanceledCalls),
//                BO.VolunteerSortField.SumOfExpiredCalls => list.OrderByDescending(v => v.TotalExpiredCalls),
//                _ => list.OrderBy(v => v.Id)
//            };
//        }
//        catch (DO.DalDoesNotExistException ex)
//        {
//            throw new BO.BlGeneralDatabaseException("Error accessing data.", ex);
//        }
//        catch (Exception ex)
//        {
//            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while getting volunteers.", ex);
//        }
//    }



//    /// <summary>
//    /// Retrieves the details of a volunteer, including any active call assignment they are handling.
//    /// </summary>
//    /// <param name="volunteerId">The unique ID of the volunteer.</param>
//    /// <returns>
//    /// A <see cref="BO.Volunteer"/> object containing the volunteer's details.
//    /// If the volunteer has an active assignment, it will include a <see cref="BO.CallInProgress"/> object.
//    /// </returns>
//    /// <exception cref="BO.BlDoesNotExistException">
//    /// Thrown if the volunteer does not exist in the data layer or there is an issue accessing their data.
//    /// </exception>
//    /// <remarks>
//    /// This method queries the data layer to fetch volunteer details and their related call assignments.
//    /// If there is no active call assignment, the <see cref="BO.Volunteer.CallInProgress"/> property will be null.
//    /// </remarks>
//    public BO.Volunteer GetVolunteerDetails(int volunteerId)
//    {
//        try
//        {
//            var doVolunteer = _dal.Volunteer.Read(volunteerId) ??
//               throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist");
//            var assigments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);
//            var activeAssignment = assigments.FirstOrDefault(a => a.exitTime == null);
//            BO.CallInProgress? boCallInProgress = null;
//            if (activeAssignment is not null)
//            {
//                    var callDetails = _dal.Call.Read(activeAssignment.CallId);
//                    if (callDetails is not null)
//                    {
//                        boCallInProgress = new BO.CallInProgress
//                        {
//                            Id = activeAssignment.Id,
//                            CallId = activeAssignment.CallId,
//                            Type = (BO.CallType)callDetails.MyCallType,
//                            Address = callDetails.Address,
//                            OpenTime = callDetails.OpenTime,
//                            EntryTime = activeAssignment.EntryTime,
//                            DistanceFromVolunteer = Tools.CalculateDistance(doVolunteer.Latitude??0, doVolunteer.Longitude??0, callDetails.Latitude, callDetails.Longitude),
//                            Status = Tools.CalculateStatus(activeAssignment, callDetails, 30)
//                        };
//                    }
//            }
//            return VolunteerManager.CreateBoVolunteer(doVolunteer, boCallInProgress);
//        }
//        catch (DO.DalDoesNotExistException ex)
//        {
//            throw new BO.BLDoesNotExistException("Volunteer not found in data layer.", ex);
//        }
//        catch (Exception ex)
//        {
//            // Handle all other unexpected exceptions
//            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while geting Volunteer details.", ex);
//        }
//    }




//    /// <summary>
//    /// Authenticates a user based on username and password.
//    /// </summary>
//    /// <param name="username">The username of the user.</param>
//    /// <param name="password">The plain text password provided by the user.</param>
//    /// <returns>The role of the user if authentication is successful.</returns>
//    /// <exception cref="BO.AuthenticationException">Thrown if the username or password is incorrect.</exception>
//    /// <exception cref="BO.DataAccessException">Thrown if there is an issue accessing data.</exception>
//    public BO.Role Login(string username, string password)
//    {
//        try
//        {
//            // Fetch the user from the data layer
//            IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll(v => v.Name == username);

//            DO.Volunteer? matchingVolunteer = volunteers.FirstOrDefault(v => VolunteerManager.VerifyPassword(password, v.Password!)) ?? throw new BO.BlDoesNotExistException("Incorrect username or password.");
//            BO.Role role = (BO.Role)matchingVolunteer.role;
//            return (BO.Role)matchingVolunteer.role;
//        }
//        catch (BO.BlDoesNotExistException)
//        {
//            throw ;
//        }
//        catch (Exception ex)
//        {
//            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while login.", ex);
//        }
//    }



//    /// <summary>
//    /// Updates the details of a volunteer.
//    /// Validates input, permissions, and logical constraints before updating the data layer.
//    /// </summary>
//    /// <param name="requesterId">The ID of the user requesting the update.</param>
//    /// <param name="boVolunteer">The BO.Volunteer object containing updated details.</param>
//    /// <exception cref="ArgumentException">Thrown when input data is invalid.</exception>
//    /// <exception cref="UnauthorizedAccessException">Thrown when the requester lacks permissions.</exception>
//    /// <exception cref="KeyNotFoundException">Thrown when the volunteer does not exist.</exception>
//    /// <exception cref="ApplicationException">Thrown when an error occurs in the data layer.</exception>
//    public void UpdateVolunteer(int requesterId, BO.Volunteer VolunteerForUpdate)
//    {

//        try
//        {
//            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

//            DO.Volunteer requester = _dal.Volunteer.Read(requesterId) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={requesterId} does not  exists and can not update other Volunteer");
//            DO.Volunteer originalVolunteer = _dal.Volunteer.Read(VolunteerForUpdate.Id)!;
//            if (VolunteerForUpdate.Password == "")
//            {
//                VolunteerForUpdate.Password = originalVolunteer.Password!;
//            }

//            // Ensure permissions are correct
//            VolunteerManager.ValidatePermissions(requester, VolunteerForUpdate);

//            // Validate input format and basic structure
//            VolunteerManager.ValidateInputFormat(VolunteerForUpdate);

//            // Validate logical rules for the volunteer
//            (VolunteerForUpdate.Latitude, VolunteerForUpdate.Longitude) = VolunteerManager.logicalChecking(VolunteerForUpdate);

//            var changedFields = VolunteerManager.GetChangedFields(originalVolunteer, VolunteerForUpdate);
//            VolunteerManager.CanUpdateFields(requester.role, changedFields, VolunteerForUpdate);

//            // Prepare DO.Volunteer object for data layer update
//            DO.Volunteer doVolunteer = VolunteerManager.CreateDoVolunteer(VolunteerForUpdate);

//            _dal.Volunteer.Update(doVolunteer); // Attempt to update the data layer
//            VolunteerManager.Observers.NotifyListUpdated(); //stage 5
//            VolunteerManager.Observers.NotifyItemUpdated(doVolunteer.Id); //stage 5

//        }

//        catch (BLTemporaryNotAvailableException)
//        {
//            throw;
//        }
//        catch (BO.BlUpdatingException )
//        {
//            throw ;

//        }
//        catch (DO.DalDoesNotExistException ex)
//        {
//            throw new BO.BLDoesNotExistException($"Volunteer with ID={VolunteerForUpdate.Id} does not  exists", ex);

//        }

//        catch (Exception ex)
//        {
//            // Handle all other unexpected exceptions
//            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while update.", ex);
//        }
//    }
//    #region Stage 5
//    public void AddObserver(Action listObserver) =>
//    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
//    public void AddObserver(int id, Action observer) =>
//    VolunteerManager.Observers.AddObserver(id, observer); //stage 5
//    public void RemoveObserver(Action listObserver) =>
//    VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
//    public void RemoveObserver(int id, Action observer) =>
//    VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
//    #endregion Stage 5


//}
namespace BlImplementation;
using BlApi;
using BO;
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
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

            // Validate input format and basic structure
            Helpers.VolunteerManager.ValidateInputFormat(boVolunteer);
            // Validate logical rules for the volunteer
            var (latitude, longitude) = VolunteerManager.logicalChecking(boVolunteer);
            if (latitude != null && longitude != null)
            {
                // Update the properties of the BOVolunteer instance
                boVolunteer.Latitude = latitude;
                boVolunteer.Longitude = longitude;
            }
            // Prepare DO.Volunteer object 
            VolunteerManager.EncryptPassword(boVolunteer.Password);
            DO.Volunteer doVolunteer = VolunteerManager.CreateDoVolunteer(boVolunteer);

            lock (AdminManager.BlMutex) //stage 7
                _dal.Volunteer.Create(doVolunteer);

            VolunteerManager.Observers.NotifyListUpdated(); //stage 5

        }
        catch (BLTemporaryNotAvailableException)
        {
            throw;
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }
        catch (BO.BlInvalidFormatException)
        {
            throw;
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
    /// <exception cref="BlDeletionException">Thrown when an error occurs in the data layer.</exception>
    public void DeleteVolunteer(int volunteerId)
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

            lock (AdminManager.BlMutex) //stage 7
            {
                // Check if the volunteer can be deleted
                IEnumerable<DO.Assignment> assignmentsWithVolunteer = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);
                if (assignmentsWithVolunteer.Any())
                    throw new BO.InvalidOperationException("Volunteer cannot be deleted because they are or have been assigned to tasks.");
                _dal.Volunteer.Delete(volunteerId);
            }

            VolunteerManager.Observers.NotifyListUpdated(); //stage 5

        }

        catch (BLTemporaryNotAvailableException)
        {
            throw;
        }
        catch (BO.InvalidOperationException ex)
        {
            // Handle logical business errors, such as tasks assigned to the volunteer
            throw new BO.BlDeletionException($"Unable to delete volunteer with ID {volunteerId} because they are assigned to tasks.", ex);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // Handle the case where the volunteer does not exist in the database
            throw new BO.BlDeletionException($"Error deleting volunteer with ID {volunteerId}. Volunteer not found.", ex);
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
    public IEnumerable<BO.VolunteerInList> GetVolunteersList(
            bool? isActive = null,
            BO.VolunteerSortField? sortBy = null,
            BO.CallType? filterField = null)
    {
        try
        {
            List<DO.Volunteer> volunteers;
            Dictionary<int, List<DO.Assignment>> assignmentsByVolunteer;
            Dictionary<int, DO.Call> callsById;

            lock (AdminManager.BlMutex) //stage 7
            {
                //  טעינה ראשונית
                volunteers = _dal.Volunteer
                    .ReadAll(v => !isActive.HasValue || v.IsActive == isActive.Value)
                    .ToList();

                assignmentsByVolunteer = _dal.Assignment.ReadAll()
                    .GroupBy(a => a.VolunteerId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                callsById = _dal.Call.ReadAll().ToDictionary(c => c.Id);
            }

            // השלכת הנתונים לאובייקט העסקי
            var list = volunteers.Select(v =>
            {
                var aList = assignmentsByVolunteer.TryGetValue(v.Id, out var l) ? l : new List<DO.Assignment>();

                // סיכומים
                var handled = aList.Count(a => a.TypeOfEndTime == DO.EndOfTreatment.treated);
                var canceled = aList.Count(a => a.TypeOfEndTime == DO.EndOfTreatment.administratorCancel);
                var expired = aList.Count(a => a.TypeOfEndTime == DO.EndOfTreatment.expired);

                // הקצאה פעילה
                var current = aList.FirstOrDefault(a => a.exitTime == null);
                var callType = current?.CallId is int cid && callsById.TryGetValue(cid, out var c)
                               ? (BO.CallType)c.MyCallType
                               : BO.CallType.None;

                return new BO.VolunteerInList
                {
                    Id = v.Id,
                    FullName = v.Name,
                    IsActive = v.IsActive,
                    TotalHandledCalls = handled,
                    TotalCanceledCalls = canceled,
                    TotalExpiredCalls = expired,
                    CurrentCallId = current?.CallId,
                    CurrentCallType = callType
                };
            })
            .Where(vol => !filterField.HasValue || vol.CurrentCallType == filterField.Value)
            .ToList();          // חומריזציה

            // מיון
            return sortBy switch
            {
                BO.VolunteerSortField.Name => list.OrderBy(v => v.FullName),
                BO.VolunteerSortField.TotalHandledCalls => list.OrderByDescending(v => v.TotalHandledCalls),
                BO.VolunteerSortField.TotalCanceledCalls => list.OrderByDescending(v => v.TotalCanceledCalls),
                BO.VolunteerSortField.TotalExpiredCalls => list.OrderByDescending(v => v.TotalExpiredCalls),
                BO.VolunteerSortField.SumOfCancellation => list.OrderByDescending(v => v.TotalCanceledCalls),
                BO.VolunteerSortField.SumOfExpiredCalls => list.OrderByDescending(v => v.TotalExpiredCalls),
                _ => list.OrderBy(v => v.Id)
            };
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlGeneralDatabaseException("Error accessing data.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while getting volunteers.", ex);
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
            DO.Volunteer doVolunteer;
            IEnumerable<DO.Assignment> assigments;
            DO.Call? callDetails = null;

            lock (AdminManager.BlMutex) //stage 7
            {
                doVolunteer = _dal.Volunteer.Read(volunteerId) ??
                   throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist");
                assigments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);
                var activeAssignment1 = assigments.FirstOrDefault(a => a.exitTime == null);

                if (activeAssignment1 is not null)
                {
                    callDetails = _dal.Call.Read(activeAssignment1.CallId);
                }
            }

            var activeAssignment = assigments.FirstOrDefault(a => a.exitTime == null);
            BO.CallInProgress? boCallInProgress = null;
            if (activeAssignment is not null && callDetails is not null)
            {
                boCallInProgress = new BO.CallInProgress
                {
                    Id = activeAssignment.Id,
                    CallId = activeAssignment.CallId,
                    Type = (BO.CallType)callDetails.MyCallType,
                    Address = callDetails.Address,
                    OpenTime = callDetails.OpenTime,
                    EntryTime = activeAssignment.EntryTime,
                    DistanceFromVolunteer = Tools.CalculateDistance(doVolunteer.Latitude ?? 0, doVolunteer.Longitude ?? 0, callDetails.Latitude, callDetails.Longitude),
                    Status = Tools.CalculateStatus(activeAssignment, callDetails, 30)
                };
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
            IEnumerable<DO.Volunteer> volunteers;

            lock (AdminManager.BlMutex) //stage 7
                volunteers = _dal.Volunteer.ReadAll(v => v.Name == username);

            DO.Volunteer? matchingVolunteer = volunteers.FirstOrDefault(v => VolunteerManager.VerifyPassword(password, v.Password!)) ?? throw new BO.BlDoesNotExistException("Incorrect username or password.");
            BO.Role role = (BO.Role)matchingVolunteer.role;
            return (BO.Role)matchingVolunteer.role;
        }
        catch (BO.BlDoesNotExistException)
        {
            throw;
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
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

            DO.Volunteer requester;
            DO.Volunteer originalVolunteer;

            lock (AdminManager.BlMutex) //stage 7
            {
                requester = _dal.Volunteer.Read(requesterId) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={requesterId} does not  exists and can not update other Volunteer");
                originalVolunteer = _dal.Volunteer.Read(VolunteerForUpdate.Id)!;
            }

            if (VolunteerForUpdate.Password == "")
            {
                VolunteerForUpdate.Password = originalVolunteer.Password!;
            }

            // Ensure permissions are correct
            VolunteerManager.ValidatePermissions(requester, VolunteerForUpdate);

            // Validate input format and basic structure
            VolunteerManager.ValidateInputFormat(VolunteerForUpdate);

            // Validate logical rules for the volunteer
            (VolunteerForUpdate.Latitude, VolunteerForUpdate.Longitude) = VolunteerManager.logicalChecking(VolunteerForUpdate);

            var changedFields = VolunteerManager.GetChangedFields(originalVolunteer, VolunteerForUpdate);
            VolunteerManager.CanUpdateFields(requester.role, changedFields, VolunteerForUpdate);

            // Prepare DO.Volunteer object for data layer update
            DO.Volunteer doVolunteer = VolunteerManager.CreateDoVolunteer(VolunteerForUpdate);

            lock (AdminManager.BlMutex) //stage 7
                _dal.Volunteer.Update(doVolunteer); // Attempt to update the data layer

            VolunteerManager.Observers.NotifyListUpdated(); //stage 5
            VolunteerManager.Observers.NotifyItemUpdated(doVolunteer.Id); //stage 5

        }

        catch (BLTemporaryNotAvailableException)
        {
            throw;
        }
        catch (BO.BlUpdatingException)
        {
            throw;

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