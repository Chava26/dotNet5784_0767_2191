
namespace BlImplementation;

using BO;
using DO;
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
           
            // Validate call details
            Helpers.CallManager.ValidateCallDetails(call);
            // Calculate latitude and longitude based on the address
            var coordinates = Helpers.Tools.GetCoordinatesFromAddress(call.FullAddress);
            Helpers.CallManager.logicalChecking(call);
            if (!coordinates.Latitude.HasValue || !coordinates.Longitude.HasValue)
            {
                throw new BO.BlInvalidFormatException("The address must be valid and resolvable to latitude and longitude.");
            }

            // Assign calculated latitude and longitude to the call
            call.Latitude = coordinates.Latitude.Value;
            call.Longitude = coordinates.Longitude.Value;

            // Map BO.Call to DO.Call
            var dataCall = new DO.Call
            {
                MyCallType = (DO.CallType)call.CallType,
                Description = call.Description,
                Address = call.FullAddress,
                Latitude = call.Latitude,
                Longitude = call.Longitude,
                OpenTime = call.OpenTime,
                MaxFinishTime = call.MaxEndTime
            };

            // Attempt to add the call to the data layer

            _dal.Call.Create(dataCall);
        }
        //catch (BO.BlInvalidFormatException)
        //{
        //    throw new Exception();
        //}
        catch (DO.DalAlreadyExistsException ex)
        {
            // Handle specific exceptions from the data layer and rethrow if necessary
            throw new BO.BlAlreadyExistsException("Failed to add the call to the system.", ex);
        }
        catch (Exception ex)
        {
            // Catch the data layer exception and rethrow a custom exception to the UI layer
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while add.", ex);
        }

    }

    public void SelectCallForTreatment(int volunteerId, int callId)
    {
        try
        {
            var call = _dal.Call.Read(callId) ?? throw new BO.BlInvalidOperationException($"Call with ID {callId} not found.");
            var status = CallManager.CalculateCallStatus(call);

            if (status == CallStatus.Expired || status == CallStatus.Closed || (status == CallStatus.InProgress && _dal.Assignment.Read(callId) != null))
            {
                throw new BO.BlInvalidOperationException($"Cannot select this call for treatment, since the call's status is: {status}");
            }

            var newAssignment = new DO.Assignment(
                Id: 0,
                CallId: callId,
                VolunteerId: volunteerId,
                EntryTime: ClockManager.Now,
                exitTime: null,
                TypeOfEndTime: null
            );
            _dal.Assignment.Create(newAssignment);
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
            var assignment = _dal.Assignment.Read(assignmentId) ?? throw new KeyNotFoundException($"Assignment with ID {assignmentId} not found.");

            var volunteer = _dal.Volunteer.Read(requesterId) ?? throw new KeyNotFoundException($"Volunteer with ID {requesterId} not found.");
            if (volunteer.role != DO.Role.Manager || assignment.VolunteerId != requesterId)
            {
                throw new BO.BlUnauthorizedAccessException("You do not have permission to cancel this assignment.");
            }
            var call = _dal.Call.Read(assignment.CallId) ?? throw new KeyNotFoundException($"Call with ID {assignment.CallId} not found.");
            CallStatus status = CallManager.CalculateCallStatus(call,assignment);

            if (status == CallStatus.Expired || status == CallStatus.Closed)
            {
                throw new BO.BlInvalidOperationException($"Cannot cancel an assignment that is {status}.");
            }

            assignment = assignment with
            {
                exitTime = ClockManager.Now, 
                TypeOfEndTime = (assignment.VolunteerId == requesterId) ? DO.EndOfTreatment.selfCancel: DO.EndOfTreatment.administratorCancel
            };

            _dal.Assignment.Update(assignment);
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
            var assignment = _dal.Assignment.Read(assignmentId) ?? throw new BO.BlDoesNotExistException($"Assignment with ID {assignmentId} not found.");

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
                exitTime = ClockManager.Now
            };

            _dal.Assignment.Update(updatedAssignment);
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
        // Fetch the call details

        var call = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"The call with the  ID={callId}does not exist.");


        // Step 2: Fetch the latest assignment for the call
        var latestAssignment = _dal.Assignment.ReadAll() // Get all assignments
            .Where(a => a.CallId == callId) // Filter by CallId
            .OrderByDescending(a => a.EntryTime) // Get the latest by EntryTime
            .FirstOrDefault(); // May return null if no assignments exist

        // Step 3: Calculate the status using the helper method
        CallStatus status = CallManager.CalculateCallStatus(call);
        // Step 4: Check if the call can be deleted
        if (status != CallStatus.Open)
        {
            throw new BO.BlDeletionException("The call cannot be deleted because it is not in an open state.");
        }

        if (latestAssignment is not null)
        {
            throw new BO.BlDeletionException("The call cannot be deleted because it has been assigned to a volunteer.");
        }

        // Step 5: Attempt to delete the call
        _dal.Call.Delete(callId); // Call the data layer's Delete method
        
    }



    /// <summary>
    /// Retrieves the details of a specific call by its ID, including its assignments (if any).
    /// </summary>
    /// <param name="callId">The unique identifier of the call to fetch details for.</param>
    /// <returns>An object containing the call details and its assignments (if applicable).</returns>
    /// <exception cref="Exception">Thrown if no call with the given ID exists in the data layer.</exception>
    public BO.Call GetCallDetails(int callId)
    {
        try {
        // Fetch the call by its ID from the data layer
        DO.Call? call = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID {callId} was not found.");


        // Fetch the list of assignments for the call (if any)
        var callAssignments = _dal.Assignment.ReadAll(a => a.CallId == callId)
                                              .Select(a => new BO.CallAssignInList
                                              {
                                                  VolunteerId = a.VolunteerId,
                                                  VolunteerName = _dal.Volunteer.Read(a.VolunteerId)?.Name, // Assuming Volunteer entity is already loaded
                                                  TreatmentStartTime = a.EntryTime,
                                                  TreatmentEndTime = a.exitTime,
                                                  TypeOfEndTreatment = (BO.EndOfTreatment?)a.TypeOfEndTime
                                              })
                                              .ToList();

        // Create the BO.Call object with the necessary details
       return  new BO.Call
        {
            Id = call.Id,
            CallType = (BO.CallType)call.MyCallType,
            Description = call.Description,
            FullAddress = call.Address,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpenTime = call.OpenTime,
            MaxEndTime = call.MaxFinishTime,
              Status = CallManager.CalculateCallStatus(call),
           CallAssignments = callAssignments
        };
        }catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException("Volunteer not found in data layer.", ex);
        }
        catch (Exception ex)
        {
            // Handle all other unexpected exceptions
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while geting call details.", ex);
        }
    }
    /// <summary>
    /// Retrieves a filtered and sorted list of calls based on the provided criteria.
    /// </summary>
    /// <param name="filterField">The enum field of <see cref="BO.CallInList"/> to filter by. Nullable.</param>
    /// <param name="filterValue">The value to filter by. Nullable.</param>
    /// <param name="sortField">The enum field of <see cref="BO.CallInList"/> to sort by. Nullable.</param>
    /// <returns>A filtered and sorted collection of <see cref="BO.CallInList"/>.</returns>

    public IEnumerable<BO.CallInList> GetCalls(BO.CallField? filterField, object? filterValue, BO.CallField? sortField)
    {
       try {
        // Explicit type for the calls and assignments
        IEnumerable<DO.Call> allCalls = _dal.Call.ReadAll().ToList();
        // Map calls to BO.CallInList objects
        IEnumerable<BO.CallInList> callList = allCalls.Select(call =>
        {
            var assignments = _dal.Assignment.ReadAll(a => a.CallId == call.Id);
            var latestAssignment = assignments.OrderByDescending(a => a.EntryTime).FirstOrDefault();

            CallStatus status = CallManager.CalculateCallStatus(call, latestAssignment);

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
        }catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("Failed to retrieve calls list", ex);
        }
    }

    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? callType, BO.CallField? sortField)
    {
        try
        {
            var assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.exitTime != null)
                .Where(a => callType == null || (BO.CallType)_dal.Call.Read(a.CallId)!.MyCallType == callType)
                .Select(a =>
                {
                    var call = _dal.Call.Read(a.CallId)?? throw new BO.BlDoesNotExistException($"Call with ID={a.CallId} does not exist.and there is problem in Assignment with id= {a.Id}"); ;
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
                });

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
            var volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist.");
            var (volunteerLat, volunteerLon) = (
                volunteer.Latitude ?? throw new BO.BlInvalidFormatException($"Cannot calculate distance: Volunteer with ID {volunteerId} is missing latitude."),
                volunteer.Longitude ?? throw new BO.BlInvalidFormatException($"Cannot calculate distance: Volunteer with ID {volunteerId} is missing longitude.")
            ); var openCalls = _dal.Call.ReadAll()
                .Where(c =>
                (CallManager.CalculateCallStatus(c) == BO.CallStatus.Open || CallManager.CalculateCallStatus(c) == BO.CallStatus.OpenRisk)) // הפשטת הבדיקה
                .Select(c => new BO.OpenCallInList
                {
                    Id = c.Id, 
                    CallType = (BO.CallType)c.MyCallType, 
                    Description = c.Description,
                    FullAddress = c.Address, 
                    OpenTime = c.OpenTime,
                    MaxEndTime = c.MaxFinishTime,  
                    DistanceFromVolunteer = Tools.CalculateDistance(volunteerLat, volunteerLon, c.Latitude, c.Longitude)
                });

            return sortField.HasValue
            ? openCalls.OrderBy(c => c.GetType().GetProperty(sortField.ToString()!)?.GetValue(c))
            : openCalls.OrderBy(c => c.Id);

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
        var counts = new int[Enum.GetValues(typeof(BO.CallStatus)).Length];
        _dal.Call.ReadAll()
                    .GroupBy(call => CallManager.CalculateCallStatus(call))
                    .ToList()
                    .ForEach(g => counts[(int)g.Key] = g.Count());
        return counts;
      
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
            // Validate the input data (check for format and logical consistency)
            Helpers.CallManager.ValidateCallDetails(updatedCall);
           Helpers.CallManager.logicalChecking(updatedCall);
            var (latitude, longitude) = Tools.GetCoordinatesFromAddress(updatedCall.FullAddress);
            // Check if either latitude or longitude is null
            if (latitude is null || longitude is null)
            {
                throw new ArgumentException("The address must be valid and resolvable to latitude and longitude.");
            }

            // Update the properties of the updatedCall instance
            updatedCall.Latitude = latitude.Value;
            updatedCall.Longitude = longitude.Value;


            // Convert BO.Call to DO.Call for data layer update
            DO.Call callToUpdate = new DO.Call
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
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException($"Call with ID={updatedCall.Id} does not exists", ex);
        }
        catch (Exception ex)
        {
            // Catch the data layer exception and rethrow a custom exception to the UI layer
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while update.", ex);
        }
     }

}

