namespace Dal;
using DalApi;
using DO;
using System.Runtime.CompilerServices;

/// <summary>
/// This class implements the IAssignment interface, providing CRUD operations (Create, Read, Update, Delete) for the 'Call' entity.
/// It interacts with an XML data source for storing Assignment-related data.
/// </summary>
internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Creates a new Assignment and adds it to the XML data source.
    /// The new Assignment gets the next available Assignment ID from the Config class.
    /// </summary>
    /// <param name="Assignment">The Assignment object to be created.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Assignment assignment)
    {
        Assignment newAssignment = assignment with { Id = Config.NextAssignmentId };
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        Assignments.Add(newAssignment);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }
    /// <summary>
    /// Reads a assignment by its ID from the XML data source.
    /// </summary>
    /// <param name="id">The ID of the call to be read.</param>
    /// <returns>The assignment with the specified ID, or null if not found.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(int id)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
            return Assignments.FirstOrDefault(Assignment => Assignment.Id == id);
    }
    /// Reads an Assignment by a filter from the XML data source.
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        return assignments.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all assignments from the XML data source, with an optional filter for selection.
    /// </summary>
    /// <param name="filter">An optional filter to apply to the list of assignments.</param>
    /// <returns>An IEnumerable collection of calls matching the filter criteria.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        return filter == null ? assignments : assignments.Where(filter); ;
    }


    /// <summary>
    /// Updates an existing assignment in the XML data source.
    /// If the assignment does not exist, throws a DalDoesNotExistException.
    /// </summary>
    /// <param name="assignment">The updated assignment object.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Assignment assignment)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (Assignments.RemoveAll(it => it.Id == assignment.Id) == 0)
            throw new DalDoesNotExistException($"Course with ID={assignment.Id} does Not exist");
        Assignments.Add(assignment);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }
    /// <summary>
    /// Deletes a assignment by its ID from the XML data source.
    /// If the assignment does not exist, throws a DalDoesNotExistException.
    /// </summary>
    /// <param name="id">The ID of the assignment to be deleted.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (Assignments.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Course with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }
    /// <summary>
    /// Deletes all assignments from the XML data source.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
         XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignments_xml);
    }

}

