using System;
using System.Collections.Generic;
namespace BO;

   
    /// <summary>
    /// Logical Data Entity "Open Call in List" - OpenCallInList
    /// This entity represents a call that is only for viewing, with no additional logic.
    /// </summary>
    public class OpenCallInList
    {
        /// <summary>
        /// The unique identifier of the open call.
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
        /// </summary>
        public string FullAddress { get; init; }

        /// <summary>
        /// The date and time when the call was opened.
        /// </summary>
        public DateTime OpenTime { get; init; }

        /// <summary>
        /// The latest possible time the call should be completed (can be null).
        /// </summary>
        public DateTime? MaxEndTime { get; set; }

        /// <summary>
        /// The distance of the call from the volunteer.
        /// This value will not be calculated in the logical layer, but is used in the display of open calls.
        /// </summary>
        public double DistanceFromVolunteer { get; set; }

        /// <summary>
        /// Constructor to initialize the properties of the OpenCallInList entity.
        /// </summary>
        
        public OpenCallInList(int id, CallType callType, string fullAddress, DateTime openTime)
        {
            Id = id;
            CallType = callType;
            FullAddress = fullAddress;
            OpenTime = openTime;
        }
        /// <summary>
        /// Override of ToString method for debugging purposes using Reflection to display the property values.
        /// </summary>
        public override string ToString()
        {
            var properties = this.GetType().GetProperties();
            var result = "";

            foreach (var property in properties)
            {
                result += $"{property.Name}: {property.GetValue(this)}\n";
            }

            return result;
        }
    }
