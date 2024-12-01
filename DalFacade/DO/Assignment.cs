
using static DO.Enums;

namespace DO;
/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="CallId"></param>
/// <param name="VolunteerId"></param>
/// <param name="EntryTime"></param>
/// <param name="EndTime"></param>
/// <param name="TypeOfEndTime"></param>

public record Assignment
(
    
    int CallId,
    int VolunteerId,
    EndOfTreatment? TypeOfEndTime,
    DateTime EntryTime,
    DateTime? EndTime = null,
    int Id=0
)

{
    /// <summary>
    ///  Default constructor
    /// </summary>
    public Assignment() : this(0,0, EndOfTreatment.treated, DateTime.Now) { }
}

//namespace DO;
//    public record class Assignment 
//    {

//        public int Id { get; init; }
//        public int CallId { get; init; }
//        public int VolunteerId { get; init; }
//        public DateTime StartDepartment { get; init; }
//        public DateTime? FinishDepartment { get; init; }
//      public Assignment() {
//        Id = DalList.Config.NextAssignmentId;
//        StartDepartment = DalList.Config.Clock;

//      }
//    }
