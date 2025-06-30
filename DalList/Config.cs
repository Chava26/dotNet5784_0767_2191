
using System.Runtime.CompilerServices;

namespace Dal;
   /// <summary>
/// Configuration Entity
/// </summary>
/// <param name="nextCallId">an ID number for the next new call</param>
/// <param name="nextAssignmentId">a unique id for each new assignment</param>
/// <param name="Clock">a system clock that is maintained seperatly from the pc</param>
/// <param name="RiskRange">a time range from which there and on it is considered at risk</param>
    internal static class Config
    {
        internal const int startCallId = 1000;
        private static int nextCallId = startCallId;
        internal static int NextCallId { [MethodImpl(MethodImplOptions.Synchronized)] get => nextCallId++; }

        internal const int startAssignmentId = 2000;
        private static int nextAssignmentId = startAssignmentId;
        internal static int NextAssignmentId { [MethodImpl(MethodImplOptions.Synchronized)] get => nextAssignmentId++; }

    internal static DateTime Clock { [MethodImpl(MethodImplOptions.Synchronized)] get; [MethodImpl(MethodImplOptions.Synchronized)] set; } = DateTime.Now;

    public static TimeSpan RiskRange { [MethodImpl(MethodImplOptions.Synchronized)] get; [MethodImpl(MethodImplOptions.Synchronized)] internal set; }= TimeSpan.FromHours(1);
    /// <summary>
    /// Resets the configuration values to their default settings.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
        {
            nextCallId = startCallId;
            nextAssignmentId = startAssignmentId;
            Clock = DateTime.Now;
            RiskRange = TimeSpan.FromHours(1);

        }
    }






