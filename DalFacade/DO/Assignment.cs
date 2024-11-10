

using System.Threading.Channels;

namespace DO;


public record class Assignment 
{
    public int Id { get; set; }
    public int CallId { get; set; }
    public int VolunteerId { get; set; }
    public DateTime StartDepartment { get; set; }
    public DateTime? FinishDepartment { get; set; }
  public Assignment() : this() { }
}
