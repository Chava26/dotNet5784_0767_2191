
namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System.Collections.Generic;
using System.Data;

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
            var (latitude, longitude) = Helpers.CallManager.logicalChecking(call);
            if (latitude is null || longitude is null)
            {
                throw new ArgumentException("The address must be valid and resolvable to latitude and longitude.");
            }

            // Assign calculated latitude and longitude to the call
            call.Latitude = latitude.Value;
            call.Longitude = longitude.Value;

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
        catch (DO.DalAlreadyExistsException ex)
        {
            // Handle specific exceptions from the data layer and rethrow if necessary
            throw new BO.BlAlreadyExistsException("Failed to add the call to the system.", ex);
        }
        catch (Exception ex)
        {
            // Catch the data layer exception and rethrow a custom exception to the UI layer
            throw new BO.GeneralDatabaseException("An unexpected error occurred while add.", ex);
        }

    }

    public void AssignCallToVolunteer(int volunteerId, int callId)
    {
        throw new NotImplementedException();
    }

    public void CancelAssignment(int requesterId, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void CompleteCall(int volunteerId, int assignmentId)
    {
        throw new NotImplementedException();
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
        // Step 1: Fetch the call details from the data layer
        var call = _dal.Call.Read(callId); // Fetch the call details

        if (call == null)
        {
            throw new ArgumentException("The call with the specified ID does not exist.");
        }

        // Step 2: Fetch the latest assignment for the call
        var latestAssignment = _dal.Assignment.ReadAll() // Get all assignments
            .Where(a => a.CallId == callId) // Filter by CallId
            .OrderByDescending(a => a.EntryTime) // Get the latest by EntryTime
            .FirstOrDefault(); // May return null if no assignments exist

        // Step 3: Calculate the status using the helper method
        var status = Helpers.CallManager.CalculateCallStatus(call, latestAssignment);

        // Step 4: Check if the call can be deleted
        if (status != CallStatus.Open)
        {
            throw new InvalidOperationException("The call cannot be deleted because it is not in an open state.");
        }

        if (latestAssignment != null)
        {
            throw new InvalidOperationException("The call cannot be deleted because it has been assigned to a volunteer.");
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
        // Fetch the call by its ID from the data layer
        DO.Call? call = _dal.Call.Read(callId);

        // If no call was found, throw an exception
        if (call == null)
        {
            throw new Exception($"Call with ID {callId} was not found.");
        }

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
        BO.Call callInList = new BO.Call
        {
            Id = call.Id,
            CallType = (BO.CallType)call.MyCallType,
            Description = call.Description,
            FullAddress = call.Address,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpenTime = call.OpenTime,
            MaxEndTime = call.MaxFinishTime,
            Status = Helpers.CallManager.CalculateCallStatus(call, _dal.Assignment.ReadAll(a => a.CallId == callId).FirstOrDefault()), // Assuming CalculateCallStatus is a method you have
            CallAssignments = callAssignments
        };

        return callInList;
    }


    /// <summary>
    /// Retrieves the quantities of calls grouped by their status.
    /// </summary>
    /// <returns>An array where each index corresponds to the count of calls with a specific status.</returns>
    public IEnumerable<int> GetCallQuantitiesByStatus()
    {
        // Risk threshold configuration
        TimeSpan riskThreshold = TimeSpan.FromMinutes(30);

        // Get all calls and assignments from the DAL
        IEnumerable<DO.Call> allCalls = _dal.Call.ReadAll().ToList();
        IEnumerable<DO.Assignment> allAssignments = _dal.Assignment.ReadAll().ToList();

        // Group calls by their calculated status
        var groupedCalls = allCalls.GroupBy(call =>
        {
            var latestAssignment = allAssignments
                .Where(a => a.CallId == call.Id)
                .OrderByDescending(a => a.EntryTime)
                .FirstOrDefault();

            return Helpers.CallManager.CalculateCallStatus(call, latestAssignment, riskThreshold);
        });

        // Return the count of calls for each status
        return Enum.GetValues(typeof(CallStatus))
            .Cast<CallStatus>()
            .Select(status => groupedCalls.FirstOrDefault(g => g.Key == status)?.Count() ?? 0);
    }

    /// <summary>
    /// Retrieves a filtered and sorted list of calls based on the provided criteria.
    /// </summary>
    /// <param name="filterField">The enum field of <see cref="BO.CallInList"/> to filter by. Nullable.</param>
    /// <param name="filterValue">The value to filter by. Nullable.</param>
    /// <param name="sortField">The enum field of <see cref="BO.CallInList"/> to sort by. Nullable.</param>
    /// <returns>A filtered and sorted collection of <see cref="BO.CallInList"/>.</returns>
    public IEnumerable<BO.CallInList> GetCalls(Enum? filterField, object? filterValue, Enum? sortField)
    {
        TimeSpan riskThreshold = TimeSpan.FromMinutes(30); // Risk threshold configuration

        // Explicit type for the calls and assignments
        IEnumerable<DO.Call> allCalls = _dal.Call.ReadAll().ToList();
        IEnumerable<DO.Assignment> allAssignments = _dal.Assignment.ReadAll().ToList();

        // Map calls to BO.CallInList objects
        IEnumerable<BO.CallInList> callList = allCalls.Select(call =>
        {
            var latestAssignment = allAssignments
                .Where(a => a.CallId == call.Id)
                .OrderByDescending(a => a.EntryTime)
                .FirstOrDefault();

            CallStatus status = Helpers.CallManager.CalculateCallStatus(call, latestAssignment, riskThreshold);

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
                AssignmentsCount = allAssignments.Count(a => a.CallId == call.Id)
            };
        });

        // Apply filtering
        if (filterField != null && filterValue != null)
        {
            callList = callList.Where(c => c.GetType().GetProperty(filterField.ToString())?.GetValue(c)?.Equals(filterValue) == true);
        }

        // Apply sorting
        if (sortField != null)
        {
            callList = callList.OrderBy(c => c.GetType().GetProperty(sortField.ToString())?.GetValue(c));
        }
        else
        {
            callList = callList.OrderBy(c => c.CallId); // Default sort by CallId
        }

        return callList;
    }

    public IEnumerable<CallInList> GetCalls(CallField? filterField, object? filterValue, CallField? sortField)
    {
        throw new NotImplementedException();
    }


    public IEnumerable<ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? callType, CallField? sortField)
    {
        throw new NotImplementedException();
    }


    public IEnumerable<OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? callType, CallField? sortField)
    {
        throw new NotImplementedException();
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
            var (latitude, longitude) = Helpers.CallManager.logicalChecking(updatedCall);

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
            throw new BO.GeneralDatabaseException("An unexpected error occurred while update.", ex);
        }
    }

}

