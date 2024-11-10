

namespace DO;

/// <>
/// 
/// </summary>

public record Volunteer
{


    public int Id { get; init; }
    public string FullName { get; init; }
    public string Cellphone { get; init; }
    public string Email { get; init; }
    public string? Password { get; init; }
    public string? FullAddress { get; init; }
    public double? Longitude { get; init; }
    public double? Latitude { get; init; }
    public double? MaximumDistance { get; init; }

    public Volunteer(): this(0, "") { }

   
}