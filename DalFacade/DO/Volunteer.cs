

namespace DO;
using System;
using static global::DO.Enums;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

/// <summary>
/// 
/// </summary>
/// <param name="Id">Personal unique ID of the Volunteer (as in national id card)</param>
/// <param name="Name">Private Name of the Volunteer</param>
/// <param name="Phone">Phone number of the Volunteer</param>
/// <param name="Email">Email number of the Volunteer</param>
/// <param name="Password">Password of the Volunteer</param>
/// <param name="Adress">A complete and true address in the correct format of the volunteer.</param>
/// <param name="Latitude"></param>
/// <param name="Longitude"></param>
/// <param name="Role">Manager or volunteer</param>
/// <param name="IsActive">Is the volunteer active or inactive?</param>
/// <param name="MaximumDistance">The maximum distance to receive a assigment.</param>
/// <param name="DistanceType">Distance type: Air distance, walking distance, driving distance</param>
public record Volunteer
(
    int Id,
    string Name,
    string Email,
    string Phone,
    Role Role,
    bool IsActive,
    double? MaximumDistance = null,
    string? Password = null,
    string? Adress = null,
    double? Longitude = null,

    double? Latitude = null,
    DistanceType DistanceType= DistanceType.airDistance
)
{
    public Volunteer() : this(0, "", "", "", Role.volunteer, false) { }
}
//    public record Volunteer
//    {
//        public int Id { get; init; }
//        public string FullName { get; init; }
//        public string Cellphone { get; set; }
//        public string Email { get; set; }
//        public string? Password { get; set; }
//        public string? FullAddress { get; set; }
//        public double? Longitude { get; init; }
//        public double? Latitude { get; init; }
//        public double? MaximumDistance { get; init; }

//        //public Volunteer() : this(0, "", "", "") { } // empty ctor for stage 3 


//}