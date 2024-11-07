

namespace DO;

public record Volunteer
{
    int Id
    string fullName;
    string cellphone;
    string email;
    pasword  Nullable(string);
    fullAdress  Nullable(string);
    Longitude Nullable(double);
    Latitude  Nullable(double);
    enum job {volunteer,manager};
    maximumDistance  Nullable(double);
    enum distanceType {airDistance, walking distance, drivingDistance };




}