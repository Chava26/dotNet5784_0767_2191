

using System.Net.NetworkInformation;

namespace BO;
/// <summary>
/// Represents the role of a Volunteer.
/// </summary>
public enum Role
{
    Volunteer,
    Manager

}

/// <summary>
/// Represents the type of distance calculation.
/// </summary>
public enum DistanceType
{
    AirDistance,
    WalkingDistance,
    DrivingDistance
}
/// <summary>
/// Enum representing the type of the call.
/// </summary>
public enum CallType
{
    None,
    General,             // General calls.Follow-up calls
    FlatTire,
    DeadBattery,
    EngineFailure,
    Overheating,
    BrakeFailure,
    TransmissionIssue,
    AlternatorFailure,
    StarterMotorFailure,
    OilLeak,
    CoolantLeak,
    FuelPumpFailure,
    CloggedFuelFilter,
    ExhaustLeak,
    SuspensionProblem,
    PowerSteeringFailure,
    TimingBeltFailure,
    BatteryCorrosion,
    CheckEngineLight,
    AirConditionerFailure,
    WornBrakePads,
    SparkPlugIssue,
    BlownFuse,
    HeadlightFailure,
    SensorMalfunction
}

/// <summary>
/// Enum representing time units for advancing the system clock.
/// </summary>
public enum TimeUnit
{
    Minute,
    Hour,
    Day,
    Month,
    Year
}
/// <summary>
/// Enum representing the status of the call.
/// </summary>

public enum CallStatus
{
    Open,              // The call is not being handled by any volunteer.
    InProgress,        // The call is currently being handled by a volunteer.
    Closed,            // The call has been handled and completed by a volunteer.
    Expired,           // The call was not selected for handling or was not completed in time.
    OpenRisk,          // The open call is approaching the required closing time.
    InProgressRisk     // The call in progress is approaching the required closing time.
}

/// <summary>
/// Represents a call currently in progress for a Volunteer.
/// </summary>
public class CallInAction
{
    // Define properties related to the call in progress.
}
/// <summary>
/// Represents the type of completion for a call.
/// </summary>
public enum CompletionType
{
    Resolved,
    Canceled,
    Expired
}
// Enum for Treatment Completion Status(End treatment status)
public enum EndOfTreatment {

    Completed,
    treated, 
    selfCancel,
    administratorCancel, 
    expired 
};
// Enum for volunteer sort
public enum VolunteerSortField
{
    None,
    Id,
    Name,
       IsActive,
    //SumOfCalls,
        SumOfExpiredCalls,
    SumOfCancellation,
    TotalHandledCalls,
    TotalCanceledCalls,
    TotalExpiredCalls

}
                     

/// <summary>
/// Enum representing the fields by which calls can be filtered or sorted.
/// </summary>
public enum CallField
{
    None,
    CallId,
    CallType,
    OpenTime,
    TimeRemaining,
    LastVolunteerName,
    TreatmentCompletionTime,
    Status,
    AssignmentsCount
}


///// <summary>
///// Enum representing time units for advancing the system clock.
///// </summary>
//public enum TimeUnit
//{
//    Minute,
//    Hour,
//    Day,
//    Month,
//    Year
//}

//public enum 
//{
//    Completed,          // Treatment has been completed successfully.
//    Canceled,           // Treatment was canceled before completion.
//    Expired             // Treatment was not completed in time.
//}

