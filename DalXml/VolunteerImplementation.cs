namespace Dal;
using DalApi;
using DO;
using System.Xml.Linq;
/// <summary>
/// Provides services for handling Volunteer data, including CRUD operations
/// and conversion between XML elements and Volunteer objects.
/// </summary>
internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Converts an XElement representing a Volunteer into a Volunteer object.
    /// </summary>
    /// <param name="s">The XElement containing Volunteer data.</param>
    /// <returns>A Volunteer object populated with data from the XElement.</returns>
    /// <exception cref="DalXMLFileLoadCreateException">Thrown if any required fields cannot be converted.</exception>
    static Volunteer getVolunteer(XElement s)
    {
        return new DO.Volunteer()
        {
           
            Id = s.ToIntNullable("Id") ?? throw new DalXMLFileLoadCreateException("can't convert id"),
            Name = (string?)s.Element("Name") ?? throw new DalXMLFileLoadCreateException("can't convert name"),
            Email = (string?)s.Element("Email") ?? throw new DalXMLFileLoadCreateException("can't convert email"),
            Phone = (string?)s.Element("Phone") ?? throw new DalXMLFileLoadCreateException("can't convert phone"),
            IsActive = (bool?)s.Element("IsActive") ?? false,
            role = s.ToEnumNullable<Role>("Role") ?? Role.volunteer,
            MaximumDistance = s.ToDoubleNullable("MaximumDistance"),
            Password = (string?)s.Element("Password"),
            Adress = (string?)s.Element("Adress"),
            Longitude = s.ToDoubleNullable("Longitude"),
            Latitude = s.ToDoubleNullable("Latitude"),
            DistanceType = s.ToEnumNullable<DistanceType>("DistanceType") ?? DistanceType.airDistance
        };
    }
    /// <summary>
    /// Creates an XElement representation of a Volunteer object for XML storage.
    /// </summary>
    /// <param name="volunteer">The Volunteer object to convert.</param>
    /// <returns>An XElement containing the Volunteer data.</returns>
    static XElement CreateVolunteerElement(Volunteer volunteer)
    {
        return new XElement("Volunteer",
            new XElement("Id", volunteer.Id),
            new XElement("Name", volunteer.Name),
            new XElement("Email", volunteer.Email),
            new XElement("Phone", volunteer.Phone),
            new XElement("IsActive", volunteer.IsActive),
            new XElement("Role", volunteer.role),
            volunteer.MaximumDistance.HasValue ? new XElement("MaximumDistance", volunteer.MaximumDistance.Value) : null,
            !string.IsNullOrEmpty(volunteer.Password) ? new XElement("Password", volunteer.Password) : null,
            !string.IsNullOrEmpty(volunteer.Adress) ? new XElement("Adress", volunteer.Adress) : null,
            volunteer.Longitude.HasValue ? new XElement("Longitude", volunteer.Longitude.Value) : null,
            volunteer.Latitude.HasValue ? new XElement("Latitude", volunteer.Latitude.Value) : null,
            new XElement("DistanceType", volunteer.DistanceType)
        );
    }
    /// <summary>
    /// Reads a specific Volunteer from the XML file by ID.
    /// </summary>
    /// <param name="id">The ID of the Volunteer to read.</param>
    /// <returns>The Volunteer object if found; otherwise, null.</returns>
    public Volunteer? Read(int id)
    {
        XElement? VolunteerElem =
        XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().FirstOrDefault(st =>
        (int?)st.Element("Id") == id);
        return VolunteerElem is null ? null : getVolunteer(VolunteerElem);
    }
    /// <summary>
    /// Reads a specific Volunteer from the XML file using a filter function.
    /// </summary>
    /// <param name="filter">The filter function to find the Volunteer.</param>
    /// <returns>The Volunteer object if found; otherwise, null.</returns>
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().Select(s =>
        getVolunteer(s)).FirstOrDefault(filter);
    }
    /// <summary>
    /// Updates an existing Volunteer in the XML file.
    /// </summary>
    /// <param name="item">The Volunteer object with updated data.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if the Volunteer does not exist.</exception>
    public void Update(Volunteer item)
    {
        XElement VolunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        (VolunteersRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == item.Id)
        ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={item.Id} does Not exist"))
        .Remove();
        VolunteersRootElem.Add(new XElement("Volunteer", createVolunteerElement(item)));
        XMLTools.SaveListToXMLElement(VolunteersRootElem, Config.s_volunteers_xml);
    }
    /// <summary>
    /// Creates a new Volunteer in the XML file.
    /// </summary>
    /// <param name="item">The Volunteer object to add.</param>
    /// <exception cref="DalAlreadyExistsException">Thrown if the Volunteer already exists.</exception>


    public void Create(Volunteer item)
    {
         XElement VolunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
            if (VolunteersRootElem.Elements().Any(st => (int?)st.Element("Id") == item.Id))
                throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists");

            VolunteersRootElem.Add(CreateVolunteerElement(item));
            XMLTools.SaveListToXMLElement(VolunteersRootElem, Config.s_volunteers_xml);
    }


    /// <summary>
    /// Reads all volunteers from the XML file, optionally filtering them using the provided predicate.
    /// </summary>
    /// <param name="filter">An optional filter function to apply to the volunteers.</param>
    /// <returns>An IEnumerable of Volunteer objects.</returns>
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        IEnumerable<XElement> volunteerElements = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements("Volunteer");
        IEnumerable<Volunteer> volunteers = volunteerElements.Select(e => GetVolunteer(e));

        return filter != null ? volunteers.Where(filter) : volunteers;

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="DO.DalDoesNotExistException"></exception>
    public void Delete(int id)
    {
        XElement VolunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        (VolunteersRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == item.Id)
        ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={id} does Not exist"))
        .Remove();
    }
    /// <summary>
    /// Deletes all volunteers from the XML file.
    /// </summary>
    public void DeleteAll()
    {
        XElement volunteersRoot = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        volunteersRoot.Elements("Volunteer").Remove();
        XMLTools.SaveListToXMLElement(volunteersRoot, Config.s_volunteers_xml);
    }

    
}



