namespace Dal;
using DalApi;
using DO;


internal class CallImplementation : ICall
{
    public void Create(Call call)
    {
        Call newCall = call with { Id = Config.NextCallId };
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        Calls.Add(newCall);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }

    public Call? Read(int id)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return Calls.FirstOrDefault(Call => Call.Id == id);
    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        return XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
    }

    public void Update(Call Call)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Calls.RemoveAll(it => it.Id == Call.Id) == 0)
            throw new DalDoesNotExistException($"Course with ID={Call.Id} does Not exist");
        Calls.Add(Call);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }
    public void Delete(int id)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Calls.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Course with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml);
    }

}



