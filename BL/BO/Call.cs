using System;
using System.Collections.Generic;

namespace BO;

   
   

    /// <summary>
    /// Logical Data Entity "Call" - Call
    /// This entity represents a call, which can be viewed, updated, added, or deleted.
    /// It includes a list of allocations for the call.
    /// </summary>
    public class Call
    {
        /// <summary>
        /// The unique identifier of the call.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// The type of the call (Enum).
        /// </summary>
        public CallType CallType { get; init; }

        /// <summary>
        /// A textual description of the call.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The full address of the call.
        /// This value may be updated, and if the address is incorrect or the volunteer chooses to leave it as null,
        /// it can be set to null.
        /// </summary>
        public string? FullAddress { get; set; }

        /// <summary>
        /// The latitude of the call, which is used to calculate distances between addresses.
        /// The value is updated when the address is updated.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// The longitude of the call, which is used to calculate distances between addresses.
        /// The value is updated when the address is updated.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// The time when the call was opened.
        /// This value is set by the system and cannot be updated.
        /// </summary>
        public DateTime OpenTime { get; init; }

        /// <summary>
        /// The latest possible time the call should be completed (can be null).
        /// </summary>
        public DateTime? MaxEndTime { get; set; }

        /// <summary>
        /// The status of the call, determined by the treatment type, the maximum end time, and the system's current time.
        /// </summary>
        public CallStatus Status { get; set; }

        /// <summary>
        /// A list of allocations for the current call (past and present).
        /// </summary>
        public List<CallAssignInList>? CallAssignments { get; set; } = new List<CallAssignInList>();

        /// <summary>
        /// Constructor to initialize the call entity with the required values.
        /// </summary>
        /// <param name="id">The unique identifier of the call.</param>
        /// <param name="callType">The type of the call.</param>
        /// <param name="description">A description of the call.</param>
        /// <param name="fullAddress">The full address of the call.</param>
        /// <param name="latitude">The latitude of the call location.</param>
        /// <param name="longitude">The longitude of the call location.</param>
        /// <param name="openTime">The time the call was opened.</param>
        public Call(int id, CallType callType, DateTime openTime, CallStatus status)
        {
            Id = id;
            CallType = callType;
            OpenTime = openTime;
            Status= status;
        }

    /// <summary>
    /// Override of ToString method for debugging purposes, using Reflection to display the property values.
    /// </summary>
    //public override string ToString()
    //{
    //    var properties = this.GetType().GetProperties();
    //    var result = "";

    //    foreach (var property in properties)
    //    {
    //        result += $"{property.Name}: {property.GetValue(this)}\n";
    //    }

    //    return result;
    //}
    //public override string ToString() => this.ToStringProperty();

}

/// <summary>
/// Logical Data Entity "CallAssignInList" - Allocation of the Call.
/// This represents the assignment details for the current call.
/// </summary>
//public class CallAssignInList
//{
//    // Define properties related to the Call assignment here (e.g., volunteer, allocation time, etc.)
//}


