
namespace BO;
using Helpers;
using System;


/// <summary>
/// Represents a closed call in a list view.
/// Used for displaying closed calls in a volunteer's history.
/// </summary>
public class ClosedCallInList
    {
        /// <summary>
        /// Unique identifier for the closed call.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Type of the call.
        /// </summary>
        public CallType CallType { get; set; }

        /// <summary>
        /// Full address of the call.
        /// </summary>
        public string FullAddress { get; set; }

        /// <summary>
        /// Opening time of the call.
        /// </summary>
        public DateTime OpenTime { get; set; }

        /// <summary>
        /// Time when the call was assigned for handling.
        /// </summary>
        public DateTime TreatmentStartTime { get; set; }

        /// <summary>
        /// Actual completion time of the call, if applicable.
        /// </summary>
        public DateTime? CompletionTime { get; set; }

        /// <summary>
        /// Type of call completion, if applicable.
        /// </summary>
        public EndOfTreatment? CompletionType { get; set; }

    /// <summary>
    /// Overrides ToString for debugging purposes using reflection.
    /// </summary>
    public override string ToString() => this.ToStringProperty();

    ///// <summary>
    ///// Initializes a new instance of the <see cref="ClosedCallInList"/> class.
    ///// </summary>
    ///// <param name="id">Unique identifier for the closed call.</param>
    ///// <param name="callType">Type of the call.</param>
    ///// <param name="fullAddress">Full address of the call.</param>
    ///// <param name="openTime">Opening time of the call.</param>
    ///// <param name="assignmentTime">Assignment time of the call.</param>
    ///// <param name="completionTime">Actual completion time of the call, if applicable.</param>
    ///// <param name="completionType">Type of call completion, if applicable.</param>
    //public ClosedCallInList(
    //    int id,
    //    CallType callType,
    //    string fullAddress,
    //    DateTime openTime,
    //    DateTime assignmentTime,
    //    DateTime? completionTime,
    //    EndOfTreatment? completionType)
    //{
    //    Id = id;
    //    CallType = callType;
    //    FullAddress = fullAddress;
    //    OpenTime = openTime;
    //    AssignmentTime = assignmentTime;
    //    CompletionTime = completionTime;
    //    CompletionType = completionType;
    //}
}

