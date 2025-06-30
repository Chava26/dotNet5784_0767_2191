namespace Dal;

using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

internal class CallImplementation : ICall
{
    /// <summary>
    ///  Adds a call
    /// </summary>
    /// <param name="call"></param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call call)
    {
        Call newCall = call with { Id = Config.NextCallId };
        DataSource.Calls.Add(newCall);
    }



    /// <summary>
    /// The function deletes the Call whose ID is equal to the received ID
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="DalDoesNotExistException"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        Call matchingObject = Read(id) ?? throw new DalDoesNotExistException($"Call with ID={id}is not exists");
        DataSource.Calls.Remove(matchingObject);

    }
    /// <summary>
    /// The faction deletes the entire list of Calls
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }
    /// <summary>
    ///    The function searches the list for a Call with the same ID if it finds it returns the Call and if not returns NULL
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(int id)
    {
        return DataSource.Calls.FirstOrDefault(Call => Call.Id == id);
    }
    /// <summary>Reads the first Call that matches a filter.</summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);
    }

    /// <summary>
    /// The function returns to the entire IEnumerable of Calls
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null) //stage 2
         => filter == null
          ? DataSource.Calls.Select(item => item)
                : DataSource.Calls.Where(filter);

    /// <summary>
    /// The function updates the Call's details by deleting him from the list of Calls and adding him back with the updated details
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="DalDoesNotExistException"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Call call)
    {
        Call matchingObject = Read(call.Id) ?? throw new DalDoesNotExistException($"call with ID={call.Id}is not exists");
        Delete(matchingObject.Id);
        Create(call);
    }

    

    
}
