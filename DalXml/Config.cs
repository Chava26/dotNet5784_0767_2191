using System.Runtime.CompilerServices;
using System.Xml.Linq;
namespace Dal;
/// <summary>
/// This class handles configuration settings related to calls, assignments, risk range, and the clock. 
/// It provides access to configuration data stored in XML files and allows modifications.
/// </summary>
internal static class Config
{ /// <summary>
  /// Constants for the data  XML files.
  /// </summary
    internal const string s_data_config_xml = "data-config.xml";
    internal const string s_volunteers_xml = "volunteers.xml";
    internal const string s_assignments_xml = "assignments.xml";
    internal const string s_calls_xml = "calls.xml";


    /// <summary>
    /// Gets and sets the next available call ID from the configuration.
    /// </summary>
    internal static int NextCallId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
        [MethodImpl(MethodImplOptions.Synchronized)]
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCalld", value);
    }
    /// <summary>
    /// Gets and sets the current clock (DateTime) from the configuration.
    /// </summary>
    internal static DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }
    /// <summary>
    /// Gets and sets the next available assignment ID from the configuration.
    /// </summary>
    internal static int NextAssignmentId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
        [MethodImpl(MethodImplOptions.Synchronized)]
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
    }
    /// <summary>
    /// Gets and sets the risk range (TimeSpan) from the configuration.
    /// </summary>
    internal static TimeSpan RiskRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "RiskRange");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "RiskRange", value);

    }
    /// <summary>
    /// Resets the configuration values to their default settings.
    /// Resets NextAssignmentId, NextCallId, Clock, and RiskRange.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
    {
        RiskRange = TimeSpan.FromHours(1);
        NextAssignmentId = 2000;
        NextCallId = 1000;
         Clock = DateTime.Now;
        XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", 2000);
        XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", 1000);
        XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", DateTime.Now);

    }
}