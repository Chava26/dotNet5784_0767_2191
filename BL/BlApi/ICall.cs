using System;
using System.Collections.Generic;
using BO;
namespace BlApi;


/// <summary>
/// Interface for managing calls in the system.
/// </summary>
public interface ICall
{
    /// <summary>
    /// Retrieves quantities of calls grouped by their status.
    /// </summary>
    /// <returns>An array where each index represents the quantity of calls with a corresponding status.</returns>
    IEnumerable<int> GetCallQuantitiesByStatus();

    /// <summary>
    /// Retrieves a filtered and sorted list of calls.
    /// </summary>
    /// <param name="filterField">The field to filter by. Nullable; if null, no filtering is applied.</param>
    /// <param name="filterValue">The value to filter by. Nullable; if null, no filtering is applied.</param>
    /// <param name="sortField">The field to sort by. Nullable; if null, sorts by call number.</param>
    /// <returns>A filtered and sorted collection of calls.</returns>
    IEnumerable<BO.CallInList> GetCalls(BO.CallField? filterField, object? filterValue, BO.CallField? sortField);

    /// <summary>
    /// Retrieves details of a specific call by its identifier.
    /// </summary>
    /// <param name="callId">The identifier of the call.</param>
    /// <returns>A Call object with details of the specified call.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if no call with the specified ID exists.</exception>
    BO.Call GetCallDetails(int callId);

    /// <summary>
    /// Updates the details of a specific call.
    /// </summary>
    /// <param name="updatedCall">A Call object containing updated details.</param>
    /// <exception cref="ArgumentException">Thrown if the provided details are invalid.</exception>
    /// <exception cref="KeyNotFoundException">Thrown if no call with the specified ID exists.</exception>
    void UpdateCallDetails(BO.Call updatedCall);

    /// <summary>
    /// Deletes a specific call by its identifier.
    /// </summary>
    /// <param name="callId">The identifier of the call to delete.</param>
    /// <exception cref="InvalidOperationException">Thrown if the call cannot be deleted (e.g., it's not open or has been assigned).</exception>
    /// <exception cref="KeyNotFoundException">Thrown if no call with the specified ID exists.</exception>
    void DeleteCall(int callId);

    /// <summary>
    /// Adds a new call.
    /// </summary>
    /// <param name="call">A Call object containing details of the new call.</param>
    /// <exception cref="ArgumentException">Thrown if the provided details are invalid.</exception>
    void AddCall(BO.Call call);

    /// <summary>
    /// Retrieves a list of closed calls handled by a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The identifier of the volunteer.</param>
    /// <param name="callType">The type of calls to filter by. Nullable; if null, no filtering is applied.</param>
    /// <param name="sortField">The field to sort by. Nullable; if null, sorts by call number.</param>
    /// <returns>A filtered and sorted collection of closed calls.</returns>
    IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? callType, BO.CallField? sortField);

    /// <summary>
    /// Retrieves a list of open calls available for selection by a volunteer.
    /// </summary>
    /// <param name="volunteerId">The identifier of the volunteer.</param>
    /// <param name="callType">The type of calls to filter by. Nullable; if null, no filtering is applied.</param>
    /// <param name="sortField">The field to sort by. Nullable; if null, sorts by call number.</param>
    /// <returns>A filtered and sorted collection of open calls including the distance from the volunteer.</returns>
    IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? callType, BO.CallField? sortField);

    /// <summary>
    /// Updates a call to indicate it has been completed.
    /// </summary>
    /// <param name="volunteerId">The identifier of the volunteer completing the call.</param>
    /// <param name="assignmentId">The identifier of the assignment associated with the call.</param>
    /// <exception cref="InvalidOperationException">Thrown if the call cannot be marked as completed.</exception>
    void CompleteCall(int volunteerId, int assignmentId);

    /// <summary>
    /// Cancels an assignment associated with a call.
    /// </summary>
    /// <param name="requesterId">The identifier of the person requesting the cancellation.</param>
    /// <param name="assignmentId">The identifier of the assignment to cancel.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown if the requester is not authorized to cancel the assignment.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the assignment cannot be canceled.</exception>
    void CancelAssignment(int requesterId, int assignmentId);

    /// <summary>
    /// Assigns a call to a volunteer.
    /// </summary>
    /// <param name="volunteerId">The identifier of the volunteer.</param>
    /// <param name="callId">The identifier of the call to assign.</param>
    /// <exception cref="InvalidOperationException">Thrown if the call cannot be assigned.</exception>
    void SelectCallForTreatment(int volunteerId, int callId);
}
