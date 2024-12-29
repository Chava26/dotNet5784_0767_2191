
namespace Dal;
using DalApi;
using DO;
/// <summary>
/// A thread-safe singleton implementation of the DalList class.
/// This class provides lazy initialization and ensures only one instance is created during the application's lifetime.
/// </summary>

sealed internal class DalList : IDal
{
    /// <summary>
   /// Singleton instance of the DalList class, initialized lazily and thread-safe.
    /// </summary>
    private static readonly Lazy<IDal> lazyInstance = new Lazy<IDal>(() => new DalList());

    public static IDal Instance => lazyInstance.Value;

    /// <summary>
    /// Private constructor to prevent external instantiation.
    /// </summary>
    private DalList() { }

        /// <summary>
        /// Implementation of the entries-related operations.
        /// </summary>
    
    public IAssignment Assignment { get; } = new AssignmentImplementation();


    public ICall Call { get; } = new CallImplementation();


    public IVolunteer Volunteer { get; } = new VolunteerImplementation();


    public IConfig Config { get; } = new ConfigImplementation();

    /// <summary>
    /// The function reset all the data base (Assignments,Calls,Volunteers,Config)
    /// </summary>
    public void ResetDB()
    {
        Config.Reset();

        Volunteer.DeleteAll();

        Call.DeleteAll();

        Assignment.DeleteAll();

    }
}

