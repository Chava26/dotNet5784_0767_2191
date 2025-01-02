
using System;


namespace BO;
/// <summary>
/// Logical Data Entity "Call Assign in List" - CallAssignInList
/// This entity represents an assignment for a call, containing details for that specific assignment.
/// It is a read-only entity, and it will be displayed as part of the list of assignments for a call.
/// </summary>
public class CallAssignInList
    {
        /// <summary>
        /// The volunteer's unique ID assigned to this specific call.
        /// </summary>
        public int? VolunteerId { get; set; }

        /// <summary>
        /// The volunteer's name.
        /// </summary>
        public string? VolunteerName { get; set; }

        /// <summary>
        /// The time when the volunteer started handling the call.
        /// </summary>
        public DateTime TreatmentStartTime { get; set; }

        /// <summary>
        /// The actual time when the treatment of the call was completed.
        /// </summary>
        public DateTime? TreatmentEndTime { get; set; }

        /// <summary>
        /// The completion status of the treatment.
        /// </summary>
        public EndOfTreatment? TypeOfEndTreatment { get; set; }

        /// <summary>
        /// Constructor to initialize the assignment with the required properties.
        /// </summary>
        /// <param name="volunteerId">The unique identifier of the volunteer assigned to the call.</param>
        /// <param name="volunteerName">The name of the volunteer.</param>
        /// <param name="treatmentStartTime">The time when the volunteer started handling the call.</param>
        /// <param name="treatmentEndTime">The time when the volunteer completed the treatment of the call.</param>
        /// <param name="treatmentStatus">The completion status of the treatment.</param>
        public CallAssignInList(int volunteerId, string volunteerName, DateTime treatmentStartTime, DateTime treatmentEndTime, EndOfTreatment treatmentType)
        {
            VolunteerId = volunteerId;
            VolunteerName = volunteerName;
            TreatmentStartTime = treatmentStartTime;
            TreatmentEndTime = treatmentEndTime;
             TypeOfEndTreatment = treatmentType;
        }

        /// <summary>
        /// Override of ToString method for debugging purposes, using Reflection to display the property values.
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


