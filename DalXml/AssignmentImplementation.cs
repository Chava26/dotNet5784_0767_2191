namespace Dal;
using DalApi;
using DO;


internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment assignment)
    {
        Assignment newAssignment = assignment with { Id = Config.NextAssignmentId };
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        Assignments.Add(newAssignment);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }

    public Assignment? Read(int id)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
            return Assignments.FirstOrDefault(Assignment => Assignment.Id == id);
    }

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        return XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
    }

    public void Update(Assignment assignment)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (Assignments.RemoveAll(it => it.Id == assignment.Id) == 0)
            throw new DalDoesNotExistException($"Course with ID={assignment.Id} does Not exist");
        Assignments.Add(assignment);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }
    public void Delete(int id)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (Assignments.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Course with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }
    public void DeleteAll()
    {
         XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignments_xml);
    }

}

