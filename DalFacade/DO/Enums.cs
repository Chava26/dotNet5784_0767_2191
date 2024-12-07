
using DalApi;
using static DO.Enums;
using System.Diagnostics;

namespace DO;
/// <summary>
/// record class for enums 
/// </summary>
    public record class Enums
    {

        public enum Role { volunteer, manager };
        public enum DistanceType { airDistance, walkingDistance, drivingDistance };
        public enum CallType { necessary , noNecessary};
        public enum EndOfTreatment { treated, selfCancel, administratorCancel,expired };
}


