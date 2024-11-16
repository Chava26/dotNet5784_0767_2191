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
        Read(item.id)
        //DataSource.Volunteers.Any(volunteer => volunteer.Id == item.Id)
        //    throw new InvalidOperationException("A volunteer with the same ID already exists.");

        //DataSource.Volunteers.Add(item);
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteAll()
    {
        throw new NotImplementedException();
    }

    public Volunteer? Read(int id)
    {
        throw new NotImplementedException();
    }

    public List<Volunteer> ReadAll()
    {
        throw new NotImplementedException();
    }

    public void Update(Volunteer item)
    {
        throw new NotImplementedException();
    }
}
