namespace BO;

/// <summary>
/// Represents a Volunteer with relevant attributes and behaviors.
/// </summary>
public class Volunteer
{
    /// <summary>
    /// Unique identifier for the Volunteer.
    /// Once set during creation, it cannot be updated.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Full name (first and last) of the Volunteer.
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Phone number of the Volunteer.
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Email address of the Volunteer.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Password for the Volunteer.
    /// Initially set by an administrator, and can be updated by the Volunteer.
    /// The password must be strong and securely encrypted.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Current full address of the Volunteer.
    /// Used to determine availability for handling tasks within a certain distance.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Latitude of the Volunteer's address.
    /// Calculated when the address is updated.
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// Longitude of the Volunteer's address.
    /// Calculated when the address is updated.
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    /// Role of the Volunteer, either "Manager" or "Volunteer".
    /// Can only be set or updated by a Manager.
    /// </summary>
    public Role Role { get; set; }

    /// <summary>
    /// Indicates whether the Volunteer is currently active.
    /// Inactive Volunteers retain their history but cannot handle new tasks.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Maximum distance (in kilometers) for receiving tasks.
    /// If null, there is no distance limitation.
    /// </summary>
    public double? MaxDistanceForTask { get; set; }

    /// <summary>
    /// Type of distance calculation (e.g., Air Distance, Walking Distance, Driving Distance).
    /// </summary>
    public DistanceType DistanceType { get; set; }

    /// <summary>
    /// Total number of tasks the Volunteer has completed.
    /// Read-only property.
    /// </summary>
    public int TotalCompletedTasks { get; private set; }

    /// <summary>
    /// Total number of tasks the Volunteer has canceled.
    /// Read-only property.
    /// </summary>
    public int TotalCanceledTasks { get; private set; }

    /// <summary>
    /// Total number of tasks the Volunteer accepted but expired.
    /// Read-only property.
    /// </summary>
    public int TotalExpiredTasks { get; private set; }

    ///// <summary>
    ///// The current task in progress for the Volunteer.
    ///// FALSE if no task is in progress.
    ///// </summary>
    // public bool IsActive  { get; set; }

    /// <summary>
    /// The current task in progress for the Volunteer.
    /// Null if no task is in progress.
    /// </summary>
    public CallInAction? CurrentTask { get; set; }
    public override string ToString()
    {
        return $"Id: {Id}, FullName: {FullName}, Email: {Email}, Role: {Role}";
    }
    public Volunteer(int id, string fullName, string phoneNumber, string email, Role role, DistanceType distanceType)
    {
        Id = id;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Email = email;
        Role = role;
        DistanceType = distanceType;
        IsActive = true; 
    }
}




