namespace Dal;

using System.Collections.Generic;
using DalApi;
using DO;
using System;

public class VolunteerImplementation : IVolunteer
{
    /// <summary>
    ///  Adds a volunteer only if one with the same ID is not found If one already exists throws an error
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Create(ref Volunteer item)
    {
        if(Read(item.Id)!=null)
            throw new InvalidOperationException("A volunteer with the same ID already exists.");


        DataSource.Volunteers.Add(item);
    }
    /// <summary>
    /// The function deletes the Volunteer whose ID is equal to the received ID
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Delete(int id)
    {
        Volunteer matchingObject = Read(id)?? throw new NotImplementedException("Object of type Volunteer with such ID does not exist."); ;
           
        DataSource.Volunteers.Remove(matchingObject);

    }
    /// <summary>
    /// The faction deletes the entire list of volunteers
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }
    /// <summary>
    ///    The function searches the list for a volunteer with the same ID if it finds it returns the volunteer and if not returns NULL
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Volunteer? Read(int id)
    {
          return DataSource.Volunteers.FirstOrDefault(volunteer => volunteer.Id == id);
    }
    /// <summary>
    /// The function returns to the entire list of volunteers
    /// </summary>
    /// <returns></returns>

    public List<Volunteer> ReadAll()
    {

        return new List<Volunteer>(DataSource.Volunteers);

    }
    /// <summary>
    /// The function updates the volunteer's details by deleting him from the list of volunteers and adding him back with the updated details
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Update(Volunteer item)
    {
        Volunteer matchingObject = Read(item.Id) ?? throw new InvalidOperationException("An object of type Volunteer with such ID does not exist.");
        Delete(matchingObject.Id);
        Create(ref item);
    }
}
