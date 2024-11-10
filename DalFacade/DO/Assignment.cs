

using System.Threading.Channels;

namespace DO;
    public record class Assignment 
    {
  
        public int Id { get; init; }
        public int CallId { get; init; }
        public int VolunteerId { get; init; }
        public DateTime StartDepartment { get; init; }
        public DateTime? FinishDepartment { get; init; }
      public Assignment() {
        Id = DalList.Config.NextAssignmentId;
        StartDepartment = DalList.Config.Clock;

      }
    }

