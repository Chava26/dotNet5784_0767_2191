namespace Dal;
using DalApi;
using DO;
/// <summary>
/// This class implements the ICall interface, providing CRUD operations (Create, Read, Update, Delete) for the 'Call' entity.
/// It interacts with an XML data source for storing call-related data.
/// </summary>

internal class CallImplementation : ICall
{
    /// <summary>
    /// Creates a new call and adds it to the XML data source.
    /// The new call gets the next available Call ID from the Config class.
    /// </summary>
    /// <param name="call">The call object to be created.</param>
    public void Create(Call call)
    {
        Call newCall = call with { Id = Config.NextCallId };
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        Calls.Add(newCall);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }
    /// <summary>
    /// Reads a call by its ID from the XML data source.
    /// </summary>
    /// <param name="id">The ID of the call to be read.</param>
    /// <returns>The call with the specified ID, or null if not found.</returns>
    public Call? Read(int id)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return Calls.FirstOrDefault(Call => Call.Id == id);
    }
    /// Reads a Call by a filter from the XML data source.
    public Call? Read(Func<Call, bool> filter)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return calls.FirstOrDefault(filter);
    }
    /// <summary>
    /// Reads all calls from the XML data source, with an optional filter for selection.
    /// </summary>
    /// <param name="filter">An optional filter to apply to the list of calls.</param>
    /// <returns>An IEnumerable collection of calls matching the filter criteria.</returns>
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        return XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
    }
    /// <summary>
    /// Updates an existing call in the XML data source.
    /// If the call does not exist, throws a DalDoesNotExistException.
    /// </summary>
    /// <param name="Call">The updated call object.</param>
    public void Update(Call Call)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Calls.RemoveAll(it => it.Id == Call.Id) == 0)
            throw new DalDoesNotExistException($"Course with ID={Call.Id} does Not exist");
        Calls.Add(Call);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }
    /// <summary>
    /// Deletes a call by its ID from the XML data source.
    /// If the call does not exist, throws a DalDoesNotExistException.
    /// </summary>
    /// <param name="id">The ID of the call to be deleted.</param>
    public void Delete(int id)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Calls.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Course with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }
    /// <summary>
    /// Deletes all calls from the XML data source.
    /// </summary>
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml);
    }

}



