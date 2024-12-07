
namespace Dal;
using DalApi;
using DO;
/// <summary>
/// The class organiztion all the data base (Assignments,Calls,Volunteers,Config)
/// </summary>
sealed public class DalList : IDal
{
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

