using System;

namespace BO;

    /// <summary>
    /// Logical data entity representing a call currently being handled by a volunteer.
    /// This entity is read-only and designed for display purposes.
    /// </summary>
    public class CallInProgress
    {
        /// <summary>
        /// Unique identifier for the assignment entity.
        /// Retrieved from Assignment.DO based on volunteer ID and appropriate call status.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Unique identifier for the call entity.
        /// Retrieved from Assignment.DO.
        /// </summary>
        public int CallId { get; init; }

        /// <summary>
        /// The type of the call.
        /// Enum type defined in BO layer.
        /// </summary>
        public CallType Type { get; init; }

        /// <summary>
        /// Descriptive text about the call.
        /// Retrieved from Call.DO.
        /// Nullable.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Full address of the call.
        /// Retrieved from Call.DO.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The time the call was opened.
        /// Retrieved from Call.DO
        /// </summary>
        public DateTime OpenTime { get; set; }

        /// <summary>
        /// The maximum time allowed to complete the call.
        /// Nullable.
        /// </summary>
        public DateTime? MaxCompletionTime { get; set; }

        /// <summary>
        /// The time the call entered into handling by the current volunteer.
        /// </summary>
        public DateTime EntryTime { get; set; }

        /// <summary>
        /// The distance between the call and the current location of the handling volunteer.
        /// Calculated in the logic layer.
        /// </summary>
        public double DistanceFromVolunteer { get; set; }

        /// <summary>
        /// The status of the call.
        /// Enum type defined in BO layer.
        /// </summary>
        public CallStatus Status { get; set; }

    /// <summary>
    /// Overrides ToString for debugging purposes using reflection.
    /// </summary>
    //public override string ToString()
    //{
    //    return this.ToStringProperties();
    //}

    //public override string ToString() => this.ToStringProperty();

    /// <summary>
    /// Initializes a new instance of the CallInProgress class.
    /// </summary>
    //public CallInProgress(int id, int callId, CallType callType, string address, DateTime openingTime, DateTime entryTime, double distanceFromVolunteer, CallStatus status)
    //{
    //    Id = id;
    //    CallId = callId;
    //    Type = callType;
    //    Address = address;
    //    OpenTime = openingTime;
    //    EntryTime = entryTime;
    //    DistanceFromVolunteer = distanceFromVolunteer;
    //    Status = status;
    //}
}




///// <summary>
///// Extension method for generating a string representation of the properties for debugging.
///// </summary>
//public static class EntityExtensions
//{
//    public static string ToStringProperties(this object obj)
//    {
//        if (obj == null) return string.Empty;
//        var properties = obj.GetType().GetProperties();
//        return string.Join(", ", properties.Select(prop => $"{prop.Name}: {prop.GetValue(obj)}"));
//    }
//}


