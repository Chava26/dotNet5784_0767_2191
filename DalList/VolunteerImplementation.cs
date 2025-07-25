namespace Dal;

using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    ///  Adds a volunteer only if one with the same ID is not found If one already exists throws an error
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="InvalidOperationException"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Volunteer item)
    {
        if(Read(item.Id)!=null)
            throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists");
        DataSource.Volunteers.Add(item);
    }
    /// <summary>
    /// The function deletes the Volunteer whose ID is equal to the received ID
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="DalDoesNotExistException"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        Volunteer matchingObject = Read(id) ?? throw new DalDoesNotExistException($"Volunteer with ID={id}is not exists ");

        DataSource.Volunteers.Remove(matchingObject);

    }
    /// <summary>
    /// The faction deletes the entire list of volunteers
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }
    /// <summary>
    ///    The function searches the list for a volunteer with the same ID if it finds it returns the volunteer and if not returns NULL
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(int id)
    {
          return DataSource.Volunteers.FirstOrDefault(volunteer => volunteer.Id == id);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return DataSource.Volunteers.FirstOrDefault(filter);
    }

    /// <summary>
    /// The function returns to the entire IEnumerable of Volunteers
    /// </summary>
    /// <returns></returns>

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null) //stage 2
         => filter == null
          ? DataSource.Volunteers.Select(item => item)
                : DataSource.Volunteers.Where(filter);


    /// <summary>
    /// The function updates the volunteer's details by deleting him from the list of volunteers and adding him back with the updated details
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="InvalidOperationException"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Volunteer item)
    {
        Volunteer matchingObject = Read(item.Id) ?? throw new DalDoesNotExistException($"Volunteer with ID={item.Id}is not exists");
        Delete(matchingObject.Id);
        Create(item);
    }
}
