

namespace DO;
/// <summary>
/// record class for enums 
/// </summary>


public enum Role { volunteer, Manager };
public enum DistanceType { airDistance, walkingDistance, drivingDistance };
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
};
public enum EndOfTreatment
{
    Completed,
    treated,
    selfCancel,
    administratorCancel,
    expired
};


/// <summary>
/// Main menu options for user interaction
/// </summary>
public enum MainMenu
{
    ExitMainMenu, AssignmentSubmenu, VolunteerSubmenu, CallSubmenu, InitializeData, DisplayAllData, ConfigSubmenu, ResetDatabase
}
/// <summary>
/// Submenu options for CRUD operations
/// </summary>
public enum SubMenu
{
    Exit, Create, Read, ReadAll, UpDate, Delete, DeleteAll
}

/// <summary>
/// Options specific to the Config submenu
/// </summary>
public enum ConfigSubmenu
{
    Exit, AdvanceClockByMinute, AdvanceClockByHour, AdvanceClockByDay, AdvanceClockByMonth, AdvanceClockByYear, DisplayClock, ChangeClockOrRiskRange, DisplayConfigVar, ChangeClock, Reset
}
