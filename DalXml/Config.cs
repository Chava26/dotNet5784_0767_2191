namespace Dal;
internal static class Config
{
    internal const string s_data_config_xml = "data-config.xml";
    internal const string s_volunteers_xml = "volunteers.xml";
    internal const string s_assignments_xml = "assignments.xml";
    internal const string s_calls_xml = "calls.xml";

    //...
    internal static int NextCallId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCourseId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCourseId", value);
    }
    //...
    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }
    internal static int NextAssignmentId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
       private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
    }
    //public static TimeSpan RiskRange
    //{
    //    get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "RiskRange");
    //    internal set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "NextAssignmentId", value);
    //}
    public static TimeSpan RiskRange { get; internal set; } = TimeSpan.FromHours(1);

    internal static void Reset()
    {
        RiskRange = TimeSpan.FromHours(1);
        NextAssignmentId = 2000;
        NextCallId = 1000;
         Clock = DateTime.Now;
    }
}