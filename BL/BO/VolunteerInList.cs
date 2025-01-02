using System;
namespace BO;
/// <summary>
/// Represents a Volunteer displayed in a list with essential details.
/// </summary>
public class VolunteerInList
{
    /// <summary>
    /// Unique identifier for the Volunteer (ID number).
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Full name (first and last) of the Volunteer.
    /// </summary>
    public string FullName { get; init; }

    /// <summary>
    /// Indicates whether the Volunteer is currently active.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Total number of calls handled successfully by the Volunteer.
    /// </summary>
    public int TotalHandledCalls { get; init; }

    /// <summary>
    /// Total number of calls canceled by the Volunteer.
    /// </summary>
    public int TotalCanceledCalls { get; init; }

    /// <summary>
    /// Total number of calls that expired under the Volunteer's responsibility.
    /// </summary>
    public int TotalExpiredCalls { get; init; }

    /// <summary>
    /// Identifier of the current call being handled by the Volunteer, if any.
    /// Null if no call is currently in progress.
    /// </summary>
    public int? CurrentCallId { get; init; }

    /// <summary>
    /// Type of the current call being handled by the Volunteer.
    /// If no call is in progress, this property is set to <see cref="CallType.None"/>.
    /// </summary>
    public CallType CurrentCallType { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VolunteerInList"/> class.
    /// </summary>
   
    public VolunteerInList( int id, string fullName, bool isActive, int totalHandledCalls, int totalCanceledCalls, int totalExpiredCalls, int? currentCallId,CallType currentCallType)
    {
        Id = id;
        FullName = fullName;
        IsActive = isActive;
        TotalHandledCalls = totalHandledCalls;
        TotalCanceledCalls = totalCanceledCalls;
        TotalExpiredCalls = totalExpiredCalls;
        CurrentCallId = currentCallId;
        CurrentCallType = currentCallType;
    }
}


