
namespace BlImplementation;
using BlApi;



/// <summary>
/// Implementation of the entries-related operations.
/// </summary>
internal class Bl : IBl
{ 
    /// <summary>
    /// Implementation of the entries-related operations.
     /// </summary>

    public ICall Call { get; } = new CallImplementation();


    public IVolunteer Volunteer { get; } = new VolunteerImplementation();


    public IAdmin Admin { get; } = new AdminImplementation();

}


