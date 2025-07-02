
namespace BlImplementation;

using BlApi;
using BO;
using DalApi;
using DO;
//using global::BO;
using Helpers;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;

internal class CallImplementation : BlApi.ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Adds a new call to the system.
    /// Validates the call details, calculates required fields, and stores the call in the data layer.
    /// </summary>
    /// <param name="call">The business object representing the call to be added.</param>
    /// <exception cref="ArgumentException">Thrown if the call details are invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the call cannot be added due to logical issues.</exception>
    public void AddCall(BO.Call call)
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

            // Validate call details
            Helpers.CallManager.ValidateCallDetails(call);
            // Calculate latitude and longitude based on the address
            //var coordinates = Helpers.Tools.GetCoordinatesFromAddress(call.FullAddress);
            //Helpers.CallManager.logicalChecking(call);
            //if (!coordinates.Latitude.HasValue || !coordinates.Longitude.HasValue)
            //{
            //    throw new BO.BlInvalidFormatException("The address must be valid and resolvable to latitude and longitude.");
            //}

            // Assign calculated latitude and longitude to the call
            //call.Latitude = coordinates.Latitude.Value;
            //call.Longitude = coordinates.Longitude.Value;
            DO.Call doCall = CallManager.CreateDoCall(call); // Map BO.Call to DO.Call
            lock (AdminManager.BlMutex) //stage 7
                _dal.Call.Create(doCall);
            // Compute the coordinates asynchronously without waiting for the results
            // Attempt to add the call to the data layer
            DO.Call call1 = _dal.Call.Read(c => (doCall.Address == c.Address && doCall.Description == c.Description && doCall.MaxFinishTime == c.MaxFinishTime && doCall.OpenTime == c.OpenTime && doCall.Latitude == c.Latitude && doCall.Longitude == c.Longitude && doCall.MyCallType == c.MyCallType))!;
            CallManager.Observers.NotifyListUpdated(); //stage 5

            _ = UpdateCoordinatesForCallAsync(call1, call);

        }

        catch (BLTemporaryNotAvailableException)
        {
            throw;
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            // Handle specific exceptions from the data layer and rethrow if necessary
            throw new BO.BlAlreadyExistsException("Failed to add the call to the system.", ex);
        }
        catch (BlInvalidFormatException ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while add.", ex);
        }
        catch (BlInvalidOperationException ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while add.", ex);
        }
        catch (Exception ex)
        {
            // Catch the data layer exception and rethrow a custom exception to the UI layer
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while add.", ex);
        }

    }
    private async Task UpdateCoordinatesForCallAsync(DO.Call doCall,BO.Call? boCall=null)
    {
       
            if (!string.IsNullOrEmpty(doCall.Address))
            {
                var (lat, lon) = await Tools.GetCoordinatesFromAddressAsync(doCall.Address);
            if (lat is not null && lon is not null)
            {
                doCall = doCall with { Latitude = lat.Value, Longitude = lon.Value };
                lock (AdminManager.BlMutex)
                    _dal.Call.Update(doCall);

            }
            // äôòìú Observers îçåõ ìÎlock
            _ = Task.Run(() =>
            {
                if (boCall is not null)
                    CallManager.SendEmailWhenCalOpened(boCall);

                CallManager.Observers.NotifyListUpdated();
                CallManager.Observers.NotifyItemUpdated(doCall.Id);
            });
            //if (boCall is not null)
            //        CallManager.SendEmailWhenCalOpened(boCall);
            //    CallManager.Observers.NotifyListUpdated();
            //    CallManager.Observers.NotifyItemUpdated(doCall.Id);

            }
      

        }

    public void SelectCallForTreatment(int volunteerId, int callId)
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

            // Fix for CS1023: Embedded statement cannot be a declaration or labeled statement
            DO.Call call;
            lock (AdminManager.BlMutex) //stage 7
                call = _dal.Call.Read(callId) ?? throw new BO.BlInvalidOperationException($"Call with ID {callId} not found.");
            

            var status = Tools.CalculateCallStatus(call);

            if (status == CallStatus.Expired || status == CallStatus.Closed || (status == CallStatus.InProgress && _dal.Assignment.Read(callId) != null))
            {
                throw new BO.BlInvalidOperationException($"Cannot select this call for treatment, since the call's status is: {status}");
            }

            var newAssignment = new DO.Assignment(
                Id: 0,
                CallId: callId,
                VolunteerId: volunteerId,
                EntryTime: AdminManager.Now,
                TypeOfEndTime: null,
                exitTime: null
            );
            lock (AdminManager.BlMutex) //stage 7
                _dal.Assignment.Create(newAssignment);

            VolunteerManager.Observers.NotifyItemUpdated(volunteerId);  //stage 5
            VolunteerManager.Observers.NotifyListUpdated();  //stage 5
            CallManager.Observers.NotifyListUpdated(); //stage 5  
        }

        catch (BLTemporaryNotAvailableException)
        {
            throw;
        }
        catch (BO.BlInvalidOperationException ex)
        {
            throw new BO.BlInvalidOperationException($"Invalid operation: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlInvalidOperationException("An error occurred while selecting the call for treatment.", ex);
        }
    }


    /// <summary>
    /// Cancels an assignment for a volunteer, updating its status and end time.
    /// </summary>
    /// <param name="requesterId">The ID of the requester (usually the volunteer or manager).</param>
    /// <param name="assignmentId">The ID of the assignment to cancel.</param>
    /// <exception cref="ArgumentException">Thrown if the assignment or volunteer is not found.</exception>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the requester does not have permission to cancel the assignment.</exception>
    /// <exception cref="BO.BlInvalidOperationException">Thrown if the assignment cannot be canceled due to its status (e.g., Expired or Closed).</exception>
    /// <exception cref="BO.BlInvalidOperationException">Thrown for other errors during the cancellation process.</exception>
    public void CancelAssignment(int requesterId, int assignmentId)
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
            DO.Assignment assignment;
            DO.Volunteer volunteer;
            DO.Call call;

            lock (AdminManager.BlMutex){ //stage 7
                assignment = _dal.Assignment.Read(assignmentId) ?? throw new KeyNotFoundException($"Assignment with ID {assignmentId} not found.");

               volunteer = _dal.Volunteer.Read(requesterId) ?? throw new KeyNotFoundException($"Volunteer with ID {requesterId} not found.");
               call = _dal.Call.Read(assignment.CallId) ?? throw new KeyNotFoundException($"Call with ID {assignment.CallId} not found.");
            }
            if (volunteer.role != DO.Role.Manager && assignment.VolunteerId != requesterId)
            {
                throw new BO.BlUnauthorizedAccessException("You do not have permission to cancel this assignment.");
            }
            CallStatus status = Tools.CalculateCallStatus(call, assignment);

            if (status == CallStatus.Expired || status == CallStatus.Closed)
            {
                throw new BO.BlInvalidOperationException($"Cannot cancel an assignment that is {status}.");
            }

            assignment = assignment with
            {
                exitTime = AdminManager.Now,
                TypeOfEndTime = (assignment.VolunteerId == requesterId) ? DO.EndOfTreatment.selfCancel : DO.EndOfTreatment.administratorCancel
            };
            lock (AdminManager.BlMutex) //stage 7
                _dal.Assignment.Update(assignment);

            CallManager.SendEmailToVolunteer(volunteer!, assignment);

            CallManager.Observers.NotifyItemUpdated(assignment.CallId);  //stage 5
            VolunteerManager.Observers.NotifyItemUpdated(assignment.VolunteerId);  //stage 5
            VolunteerManager.Observers.NotifyListUpdated();
            CallManager.Observers.NotifyListUpdated(); 
        }

        catch (BLTemporaryNotAvailableException)
        {
            throw;
        }
        catch (KeyNotFoundException ex)
        {
            throw new ArgumentException(ex.Message, ex);
        }
        catch (BO.BlUnauthorizedAccessException ex)
        {
            throw new BO.BlInvalidOperationException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlInvalidOperationException("An error occurred while updating the call cancellation.", ex);
        }

    }
    /// <summary>
    /// Marks an assignment as completed by updating its finish status and exit time.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer completing the assignment.</param>
    /// <param name="assignmentId">The ID of the assignment to complete.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the assignment with the given ID is not found.</exception>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the volunteer is not authorized to complete the assignment.</exception>
    /// <exception cref="BO.BlInvalidOperationException">Thrown if the assignment has already been completed or canceled.</exception>
    /// <exception cref="BO.BlGeneralDatabaseException">Thrown if a database error occurs while updating the assignment.</exception>
    public void CompleteCall(int volunteerId, int assignmentId)
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
            DO.Assignment assignment;
            lock (AdminManager.BlMutex) //stage 7
                assignment = _dal.Assignment.Read(assignmentId) ?? throw new BO.BlDoesNotExistException($"Assignment with ID {assignmentId} not found.");

            if (assignment.VolunteerId != volunteerId)
            {
                throw new BO.BlUnauthorizedAccessException($"Volunteer with ID {volunteerId} is not authorized to complete this assignment.");
            }

            if (assignment.TypeOfEndTime.HasValue)
            {
                throw new BO.BlInvalidOperationException("The assignment has already been completed or canceled.");
            }

            var updatedAssignment = assignment with
            {
                TypeOfEndTime = DO.EndOfTreatment.treated,
                exitTime = AdminManager.Now
            };
            lock (AdminManager.BlMutex) //stage 7
                _dal.Assignment.Update(updatedAssignment);

            VolunteerManager.Observers.NotifyItemUpdated(updatedAssignment.VolunteerId);  //stage 5
            VolunteerManager.Observers.NotifyListUpdated();  //stage 5
            CallManager.Observers.NotifyItemUpdated(updatedAssignment.CallId);  //stage 5
            CallManager.Observers.NotifyListUpdated(); //stage 5  
        }

        catch (BLTemporaryNotAvailableException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An error occurred while updating the assignment completion.", ex);
        }
    }
    /// <summary>
    /// Deletes a call by its unique ID, provided it meets the conditions for deletion.
    /// A call can only be deleted if it is in the "Open" status and has never been assigned to any volunteer.
    /// </summary>
    /// <param name="callId">The unique identifier of the call to delete.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if the call with the specified ID does not exist in the data layer.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the call is not in an "Open" status or if it has been assigned to a volunteer.
    /// </exception>
    public void DeleteCall(int callId)
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

            DO.Call call;
            DO.Assignment? latestAssignment; // Declare latestAssignment in the correct scope

            // Fetch the call details
            lock (AdminManager.BlMutex)
            { //stage 7
                call = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"The call with the ID={callId} does not exist.");
                latestAssignment = _dal.Assignment.ReadAll() // Get all assignments
                    .Where(a => a.CallId == callId) // Filter by CallId
                    .OrderByDescending(a => a.EntryTime) // Get the latest by EntryTime
                    .FirstOrDefault(); // May return null if no assignments exist
            }

            // Step 3: Calculate the status using the helper method
            CallStatus status = Tools.CalculateCallStatus(call);

            // Step 4: Check if the call can be deleted
            if (status != CallStatus.Open)
            {
                throw new BO.InvalidOperationException("The call cannot be deleted because it is not in an open state.");
            }

            if (latestAssignment is not null)
            {
                throw new BO.InvalidOperationException("The call cannot be deleted because it has been assigned to a volunteer.");
            }

            // Step 5: Attempt to delete the call
            _dal.Call.Delete(callId); // Call the data layer's Delete method
            CallManager.Observers.NotifyListUpdated(); //stage 5
        }
        catch (BLTemporaryNotAvailableException)
        {
            throw;
        }
        catch (InvalidOperationException ex)
        {
            // Handle logical business errors, such as tasks assigned to the volunteer
            throw new BO.BlDeletionException($"Unable to delete Call with ID {callId} :", ex);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // Handle the case where the volunteer does not exist in the database
            throw new BO.BlDeletionException($"Error deleting call with ID {callId}. call not found.", ex);
        }
        catch (Exception ex)
        {
            // Handle all other unexpected exceptions
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while deleting the volunteer.", ex);
        }
    }



    /// <summary>
    /// Retrieves the details of a specific call by its ID, including its assignments (if any).
    /// </summary>
    /// <param name="callId">The unique identifier of the call to fetch details for.</param>
    /// <returns>An object containing the call details and its assignments (if applicable).</returns>
    /// <exception cref="Exception">Thrown if no call with the given ID exists in the data layer.</exception>
    public BO.Call GetCallDetails(int callId)
    {
        try
        {
            DO.Call? call;
            List<BO.CallAssignInList> callAssignments; // Declare callAssignments in the correct scope

            lock (AdminManager.BlMutex) //stage 7
            {
                // Fetch the call by its ID from the data layer
                call = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID {callId} was not found.");

                // Fetch the list of assignments for the call (if any)
                callAssignments = _dal.Assignment.ReadAll(a => a.CallId == callId)
                                                  .Select(a => new BO.CallAssignInList
                                                  {
                                                      VolunteerId = a.VolunteerId,
                                                      VolunteerName = _dal.Volunteer.Read(a.VolunteerId)?.Name, // Assuming Volunteer entity is already loaded
                                                      TreatmentStartTime = a.EntryTime,
                                                      TreatmentEndTime = a.exitTime,
                                                      TypeOfEndTreatment = (BO.EndOfTreatment?)a.TypeOfEndTime
                                                  })
                                                  .ToList();
            }

            return CallManager.CreateBoCall(call, callAssignments); // Create the BO.Call object with the necessary details
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException("Volunteer not found in data layer.", ex);
        }
        catch (Exception ex)
        {
            // Handle all other unexpected exceptions
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while getting call details.", ex);
        }
    }
  
    public IEnumerable<BO.CallInList> GetCalls(BO.CallField? filterField, object? filterValue, BO.CallField? sortField)
    {
        try
        {
            IEnumerable<DO.Call> allCalls;
            lock (AdminManager.BlMutex) //stage 7
                // Explicit type for the calls and assignments
               allCalls = _dal.Call.ReadAll().ToList();
            // Map calls to BO.CallInList objects
            //stage 7

                IEnumerable<BO.CallInList> callList = allCalls.Select(call =>
                {
                    lock (AdminManager.BlMutex)
                    {
                        var assignments = _dal.Assignment.ReadAll(a => a.CallId == call.Id);
                        var latestAssignment = assignments.OrderByDescending(a => a.EntryTime).FirstOrDefault();
                        CallStatus status = Tools.CalculateCallStatus(call, latestAssignment);
                        if (status == CallStatus.Open)
                        {
                            int i=1;
                            i++;
                        }
                        return new BO.CallInList
                        {
                            Id = latestAssignment?.Id,
                            CallId = call.Id,
                            CallType = (BO.CallType)call.MyCallType,
                            OpenTime = call.OpenTime,
                            TimeRemaining = call.MaxFinishTime.HasValue ? call.MaxFinishTime.Value - DateTime.Now : null,
                            LastVolunteerName = latestAssignment != null
                                ? _dal.Volunteer.Read(latestAssignment.VolunteerId)?.Name : null,
                            TreatmentCompletionTime = latestAssignment?.exitTime.HasValue == true
                                ? latestAssignment.exitTime.Value - call.OpenTime
                                : null,
                            Status = status,
                            AssignmentsCount = assignments.Count(a => a.CallId == call.Id)
                        };
                    }
            });
            
                // Apply filtering
            if (filterField != null && filterValue != null)
            {
                callList = callList.Where(c => c.GetType().GetProperty(filterField.ToString()!)?.GetValue(c)?.Equals(filterValue) == true);
            }
            // Apply sorting
            return sortField.HasValue
                ? callList.OrderBy(c => typeof(BO.CallInList).GetProperty(sortField.ToString())?.GetValue(c))
                : callList.OrderBy(c => c.CallId);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("Failed to retrieve calls list", ex);
        }
    }



    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? callType, BO.CallField? sortField)
    {
        try
        {
            IEnumerable<BO.ClosedCallInList> assignments; // Declare assignments in the correct scope

            lock (AdminManager.BlMutex)
            {
                assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.exitTime != null)
                    .Where(a => callType == null || (BO.CallType)_dal.Call.Read(a.CallId)!.MyCallType == callType)
                    .Select(a =>
                    {
                        var call = _dal.Call.Read(a.CallId) ?? throw new BO.BlDoesNotExistException($"Call with ID={a.CallId} does not exist.and there is problem in Assignment with id= {a.Id}");
                        return new BO.ClosedCallInList
                        {
                            Id = call.Id,
                            CallType = (BO.CallType)call.MyCallType,
                            FullAddress = call.Address,
                            OpenTime = call.OpenTime,
                            TreatmentStartTime = a.EntryTime,
                            CompletionTime = a.exitTime,
                            CompletionType = (BO.EndOfTreatment)a.TypeOfEndTime!
                        };
                    }).ToList(); // Ensure assignments is properly initialized
            }

            return sortField.HasValue
                ? assignments.OrderBy(a => a.GetType().GetProperty(sortField.ToString()!)?.GetValue(a))
                : assignments.OrderBy(a => a.Id);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An error occurred while retrieving the closed calls list.", ex);
        }
    }

    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? callType, BO.CallField? sortField)
    {

        try
        {

           return  CallManager.GetOpenCalls(volunteerId, callType, sortField);
        }
        catch (BO.BlDoesNotExistException )
        {
            throw ;
        }
        catch (BO.BlInvalidFormatException )
        {
            throw ;
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An error occurred while retrieving the open calls list.", ex);
        }
    }
    /// <summary>
    /// Retrieves the number of calls grouped by their status.
    /// Ensures that all possible statuses are included in the output, even if their count is zero.
    /// </summary>
    /// <returns>An array where each index corresponds to a call status and contains the count of calls with that status.</returns>
    public IEnumerable<int> GetCallQuantitiesByStatus()
    {
        //IEnumerable<DO.Call> calls;
        //lock (AdminManager.BlMutex)
        //    calls = _dal.Call.ReadAll();
        //var counts = new int[Enum.GetValues(typeof(BO.CallStatus)).Length];



        //_dal.Call.ReadAll()
        //            .GroupBy(call => CallManager.CalculateCallStatus(call))
        //            .ToList()
        //            .ForEach(g => counts[(int)g.Key] = g.Count());
        //return counts;
 {
            // Retrieve all calls
            IEnumerable<DO.Call> calls;
            lock (AdminManager.BlMutex)
                calls = _dal.Call.ReadAll();

            // Group calls by status and count them
            var groupedCalls = from call in calls
                               group call by Tools.CalculateCallStatus(call) into grouped
                               select new { Status = grouped.Key, Count = grouped.Count() };

            // For each status in the enum, retrieve the c
            var result = from status in Enum.GetValues(typeof(BO.CallStatus)).Cast<BO.CallStatus>()
                         let count = groupedCalls.FirstOrDefault(g => g.Status == status)?.Count ?? 0
                         select count;

            return result;
        }
    }


    /// <summary>
    /// Updates the details of an existing call based on the provided Call object.
    /// It performs validation on the input data (format and logical checks),
    /// and then attempts to update the corresponding call in the data layer.
    /// </summary>
    /// <param name="updatedCall">The updated Call object containing all the new details.</param>
    /// <exception cref="Exception">Thrown if the call with the given ID does not exist or if validation fails.</exception>
    public void UpdateCallDetails(BO.Call updatedCall)
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

            // Validate the input data (check for format and logical consistency)
            Helpers.CallManager.ValidateCallDetails(updatedCall);
            Helpers.CallManager.logicalChecking(updatedCall);
            bool addreesUpdated = false;
            DO.Call? realyCall;
            lock (AdminManager.BlMutex)
                realyCall = _dal.Call.Read(updatedCall.Id);
            
            if (realyCall!.Address != updatedCall.FullAddress)
            {
                addreesUpdated = true;
            }
            //var (latitude, longitude) = Tools.GetCoordinatesFromAddress(updatedCall.FullAddress);
            //// Check if either latitude or longitude is null
            //if (latitude is null || longitude is null)
            //{
            //    throw new ArgumentException("The address must be valid and resolvable to latitude and longitude.");
            //}

            ////Update the properties of the updatedCall instance
            //updatedCall.Latitude = latitude.Value;
            //updatedCall.Longitude = longitude.Value;
            DO.Call callToUpdate;
            lock (AdminManager.BlMutex)
            {
                // Convert BO.Call to DO.Call for data layer update
                callToUpdate = new DO.Call
                {
                    Id = updatedCall.Id,
                    MyCallType = (DO.CallType)updatedCall.CallType,
                    Description = updatedCall.Description,
                    Address = updatedCall.FullAddress,
                    Latitude = updatedCall.Latitude,
                    Longitude = updatedCall.Longitude,
                    OpenTime = updatedCall.OpenTime,
                    MaxFinishTime = updatedCall.MaxEndTime
                };

                // Attempt to update the call in the data layer

                _dal.Call.Update(callToUpdate);
            }
            CallManager.Observers.NotifyListUpdated(); //stage 5
            CallManager.Observers.NotifyItemUpdated(callToUpdate.Id); //stage 5
            if (addreesUpdated)
            {
                // If the address was updated, recalculate latitude and longitude
                _ = UpdateCoordinatesForCallAsync(callToUpdate);
            }

        }
        catch (BLTemporaryNotAvailableException)
        {
            throw;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException($"Call with ID={updatedCall.Id} does not exists", ex);
        }
        catch (Exception ex)
        {
            // Catch the data layer exception and rethrow a custom exception to the UI layer
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while updating.", ex);
        }
    }


    #region Stage 5
    public void AddObserver(Action listObserver) =>
    CallManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    CallManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    CallManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    CallManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5


}

