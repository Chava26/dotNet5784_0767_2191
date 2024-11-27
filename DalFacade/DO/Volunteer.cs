

namespace DO;
using System;
using static global::DO.Enums;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;


/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="Phone"></param>
/// <param name="Email"></param>
/// <param name="Password"></param>
/// <param name="Adrass"></param>
/// <param name="Latitude"></param>
/// <param name="Longitude"></param>
/// <param name="Job"></param>
/// <param name="IsActive"></param>
/// <param name="MaximumDistance"></param>
/// <param name="DistanceType"></param>
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