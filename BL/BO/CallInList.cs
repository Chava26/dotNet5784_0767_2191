namespace BO;
using System;

using Helpers;



/// <summary>
/// Logical Data Entity "Call in List" - CallInList
/// This entity represents a call that is listed in the general call list, containing the relevant information for display.
/// It is a read-only entity, and it will be displayed as part of the list of all general calls.
/// </summary>
public class CallInList
{
    /// <summary>
    /// The unique ID of the assignment entity for the current call. 
    /// If no assignment has been made, it will be null.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// The unique ID of the call.
    /// </summary>
    public int CallId { get; set; }

    /// <summary>
    /// The type of the call, using an Enum to represent the different types of calls.
    /// </summary>
    public CallType CallType { get; set; }

    /// <summary>
    /// The time when the call was opened.
    /// </summary>
    public DateTime OpenTime { get; set; }

    /// <summary>
    /// The remaining time until the call must be completed (calculated based on the call's max end time).
    /// </summary>
    public TimeSpan? TimeRemaining { get; set; }

    /// <summary>
    /// The name of the last volunteer who worked on this call.
    /// </summary>
    public string? LastVolunteerName { get; set; }

    /// <summary>
    /// The time spent completing the call's treatment, relevant only for calls that have been handled.
    /// </summary>
    public TimeSpan? TreatmentCompletionTime { get; set; }

    /// <summary>
    /// The status of the call, determined by various factors like assignment status, max end time, and current time.
    /// </summary>
    public CallStatus Status { get; set; }

    /// <summary>
    /// The total number of assignments made for the current call (including completed, canceled, etc.).
    /// </summary>
    public int AssignmentsCount { get; set; }
    /// <summary>
    /// Overrides ToString for debugging purposes using reflection.
    /// </summary>
     public override string ToString() => this.ToStringProperty();


    /// <summary>
    /// Constructor to initialize the call entity with the necessary properties.
    /// </summary>
    /// <param name="callId">The unique ID of the call.</param>
    /// <param name="callType">The type of the call (using an Enum).</param>
    /// <param name="openTime">The time when the call was opened.</param>
    /// <param name="timeRemaining">Remaining time until the call must be completed.</param>
    /// <param name="lastVolunteerName">The name of the last volunteer who worked on the call.</param>
    /// <param name="treatmentCompletionTime">The time spent completing the call (only for handled calls).</param>
    /// <param name="status">The current status of the call.</param>
    /// <param name="assignmentsCount">The number of assignments made for this call.</param>
    //public CallInList(int callId, CallType callType, DateTime openTime, TimeSpan timeRemaining, string lastVolunteerName, TimeSpan treatmentCompletionTime, CallStatus status, int assignmentsCount)
    //{
    //    CallId = callId;
    //    CallType = callType;
    //    OpenTime = openTime;
    //    TimeRemaining = timeRemaining;
    //    LastVolunteerName = lastVolunteerName;
    //    TreatmentCompletionTime = treatmentCompletionTime;
    //    Status = status;
    //    AssignmentsCount = assignmentsCount;
    //}
}
//public override string ToString() => this.ToStringProperty();

/// <summary>
/// Override of ToString method for debugging purposes, using Reflection to display the property values.
/// </summary>
//public override string ToString()
//{
//    var properties = this.GetType().GetProperties();
//    var result = "";

//    foreach (var property in properties)
//    {
//        result += $"{property.Name}: {property.GetValue(this)}\n";
//    }

//    return result;
//}




