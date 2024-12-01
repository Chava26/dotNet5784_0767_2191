namespace Dal;

using System.Collections.Generic;
using DalApi;
using DO;
using System;
using DalList;

public class AssignmentImplementation : IAssignment
{
    /// <summary>
    ///  Adds a call
    /// </summary>
    /// <param name="call"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Create(Assignment assignment)
    {
        Assignment newAssignment = assignment with { Id = Config.NextAssignmentId };
        DataSource.Assignments.Add(newAssignment);
    }

    /// <summary>
    /// The function deletes the Assignment whose ID is equal to the received ID
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Delete(int id)
    {
        Assignment matchingObject = Read(id) ?? throw new NotImplementedException("Object of type Assignment with such ID does not exist."); ;

        DataSource.Assignments.Remove(matchingObject);

    }
    /// <summary>
    /// The faction deletes the entire list of Assignments
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }
    /// <summary>
    ///    The function searches the list for a Assignment with the same ID if it finds it returns the v and if not returns NULL
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Assignment? Read(int id)
    {
        return DataSource.Assignments.FirstOrDefault(Assignment => Assignment.Id == id);
    }
    /// <summary>
    /// The function returns to the entire list of Assignments
    /// </summary>
    /// <returns></returns>

    public List<Assignment> ReadAll()
    {

        return new List<Assignment>(DataSource.Assignments);

    }
    /// <summary>
    /// The function updates the Assignment's details by deleting him from the list of Assignments and adding him back with the updated details
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Update(Assignment item)
    {
        Assignment matchingObject   = Read(item.Id) ?? throw new InvalidOperationException("An object of type Assignment with such ID does not exist.");
        Delete(matchingObject.Id);
        Create(item);
    }
}
