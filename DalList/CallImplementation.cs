namespace Dal;

using System.Collections.Generic;
using DalApi;
using DO;
using System;

public class CallImplementation : ICall
{
    /// <summary>
    ///  Adds a call
    /// </summary>
    /// <param name="call"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public int Create(Call call)
    {
        Call newCall = call;
        int newId= DalList.Config.NextCallId;
        DataSource.Calls.Add(newCall);
        return newId;

    }



    /// <summary>
    /// The function deletes the Call whose ID is equal to the received ID
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Delete(int id)
    {
        Call matchingObject = Read(id) ?? throw new NotImplementedException("Object of type Volunteer with such ID does not exist.");
        DataSource.Calls.Remove(matchingObject);

    }
    /// <summary>
    /// The faction deletes the entire list of Calls
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }
    /// <summary>
    ///    The function searches the list for a Call with the same ID if it finds it returns the Call and if not returns NULL
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Call? Read(int id)
    {
        return DataSource.Calls.FirstOrDefault(Call => Call.Id == id);
    }
    /// <summary>
    /// The function returns to the entire list of Calls
    /// </summary>
    /// <returns></returns>

    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Calls);
    }
    /// <summary>
    /// The function updates the Call's details by deleting him from the list of Calls and adding him back with the updated details
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Update(Call call)
    {
        Call matchingObject = Read(call.Id) ?? throw new InvalidOperationException("An object of type call with such ID does not exist.");
        Delete(matchingObject.Id);
        Create(call);
    }

    

    
}
