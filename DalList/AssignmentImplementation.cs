namespace Dal;

using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    ///  Adds a call
    /// </summary>
    /// <param name="assignment"></param>
    /// <exception cref="InvalidOperationException"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Assignment assignment)
    {
        Assignment newAssignment = assignment with { Id = Config.NextAssignmentId };
        DataSource.Assignments.Add(newAssignment);
    }

    /// <summary>
    /// The function deletes the Assignment whose ID is equal to the received ID
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="DalDeletionImpossible"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)] 
    public void Delete(int id)
    {
        Assignment matchingObject = Read(id) ?? throw new DalDoesNotExistException($"Assignment with ID={id}is not exists ");

        DataSource.Assignments.Remove(matchingObject);

    }
    /// <summary>
    /// The faction deletes the entire list of Assignments
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }
    /// <summary>
    ///    The function searches the list for a Assignment with the same ID if it finds it returns the v and if not returns NULL
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(int Id)
    {
        return DataSource.Assignments.FirstOrDefault(Assignment => Assignment.Id == Id);
    }
    /// <summary>
    ///    The function searches the list for a Assignment  if it finds it returns the v and if not returns NULL
    /// </summary>
    /// <param name="filter">filter what need to search</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return DataSource.Assignments.FirstOrDefault(filter);
    }

    /// <summary>
    /// The function read all the Assignment in IEnumerable type
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null) //stage 2
        => filter == null
        ? DataSource.Assignments.Select(item => item)
           : DataSource.Assignments.Where(filter);

    /// <summary>
    /// The function updates the Assignment's details by deleting him from the list of Assignments and adding him back with the updated details
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="DalDoesNotExistException"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Assignment item)
    {
        Assignment matchingObject   = Read(item.Id) ?? throw new DalDoesNotExistException($"Assignment with ID={item.Id}is not exists");
        Delete(matchingObject.Id);
        Create(item);
    }
}
