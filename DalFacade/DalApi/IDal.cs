

namespace DalApi;
/// <summary>
/// This interface defines methods for accessing the various data components 
/// in the Data Access Layer (DAL). It provides access to entities like 
/// assignments, calls, volunteers, and configuration, along with a method 
/// to reset the database.
/// </summary>

public interface IDal
{
    IAssignment Assignment { get; }
    ICall Call { get; }
    IVolunteer Volunteer {  get; }
    IConfig Config {  get; }
    void ResetDB();

}
