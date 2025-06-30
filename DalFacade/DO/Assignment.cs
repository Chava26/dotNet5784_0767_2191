

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
    DateTime EntryTime,
    EndOfTreatment? TypeOfEndTime = null,
    DateTime? exitTime = null,
    int Id=0
)

{
    /// <summary>
    ///  Default constructor
    /// </summary>
    public Assignment() : this(0,0, DateTime.Now, EndOfTreatment.treated) { }
}


