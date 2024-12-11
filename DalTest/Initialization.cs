using DalApi; // Using the API for accessing the data layer (DAL)
using DO; // Using the Data Objects representing the information
using System.Data; // Provides support for database-related structures

namespace DalTest;

// Static class for initializing data in the system
public static class Initialization
{
    private static IDal? s_dal; // Variable to hold the reference to the DAL interface, can be null
    private static readonly Random s_rand = new(); // Object for generating random data

    // Function to create volunteer data
    private static void createVolunteers()
    {
        // Predefined arrays of volunteer data
        string[] names = {
    "Sari", "Pnina", "Shira", "Chaya", "Yosi",
    "Beni", "Tamar", "Eli", "Moshe", "Chana",
    "Ari", "Chaim", "Shani", "Yonatan", "David"
};

        string[] emails = {
    "Sari@gmail.com", "Pnina@gmail.com", "Shira@gmail.com", "Chaya@gmail.com", "Yosi@gmail.com",
    "Beni@gmail.com", "Tamar@gmail.com", "Eli@gmail.com", "Moshe@gmail.com", "Chana@gmail.com",
    "Ari@gmail.com", "Chaim@gmail.com", "Shani@gmail.com", "Yonatan@gmail.com", "David@gmail.com"
};

        string[] phones = {
    "050-321-4571", "052-897-6524", "054-123-8745", "053-549-1235", "055-789-6521",
    "050-987-1234", "054-112-3345", "053-785-9513", "052-312-6578", "055-654-9872",
    "053-789-2134", "050-125-6789", "052-456-7890", "054-654-3210", "055-123-7894"
};
        //I used GPT to create this array
        string[] addresses = {
    "Jerusalem, King George St.", "Tel Aviv, Rothschild Blvd.", "Haifa, Carmel Beach", "Beer Sheva, Ben Gurion Blvd.",
    "Ashdod, HaShalom St.", "Netanya, Herzl St.", "Rishon LeZion, Neot Afeka St.", "Petah Tikva, HaSharon St.", "Holon, Jabotinsky St.",
    "Bnei Brak, Rabbi Akiva St.", "Rehovot, Herzl St.", "Bat Yam, Ben Gurion Blvd.","Herzliya, Sokolov St.","Hadera, HaSharon Blvd.","Eilat, HaTmarim St."
};
        //I used GPT to create this array
        string[] passwords = {
    "Sari45G92h","Pnina87Z56j", "Shira32X19b", "Chaya54L72k", "Yosi18M91q", "Beni39P65a",
    "Tamar75V21n", "Eli26W83f", "Moshe90R47z", "Chana63S29m", "Ari42D78c", "Chaim56H14p", "Shani81J67y",
   "Yonatan97K50x", "David34Q85v"
};
        //I used GPT to create this array
     (double Latitude, double Longitude)[] coordinates = {
         (31.7683, 35.2137),(32.0853, 34.7818), (32.7940, 34.9896), (31.2518, 34.7913), (31.8044, 34.6553),
         (32.3215, 34.8532), (31.9706, 34.7925), (32.0840, 34.8878), (32.0158, 34.7874),  (32.0809, 34.8333),
          (31.8948, 34.8093), (32.0236, 34.7502), (32.1663, 34.8436), (32.4340, 34.9196),(29.5581, 34.9482)
     };
        for (int i = 0; i < 15; i++) // Loop to create 15 volunteers
        {
            int id;
            do
                id = s_rand.Next(100000000, 999999999); // Generate a random unique ID
            while (s_dal!.Volunteer.Read(id) != null); // Ensure the ID is unique

            // Define volunteer data
            string name = names[i];
            string email = emails[i];
            string phone = phones[i];
            string password = passwords[i];
            double Latitude = coordinates[i].Latitude;
            double Longitude = coordinates[i].Longitude;
            double MaximumDistance = s_rand.Next(5, 50); // Random maximum distance

            // Add the volunteer to the database
            s_dal!.Volunteer.Create(new Volunteer(id, name, email, phone, Role.volunteer, true, MaximumDistance, password, addresses[i], Longitude, Latitude));
        }
    }

    // Function to create emergency call data
    private static void createCalls()
    {
        // Predefined arrays of emergency call data
        string[] issues = {
    "Flat tire", "Engine failure", "Battery is dead", "Ran out of fuel", "Locked keys inside the car", "Brake system malfunction",
    "Overheated engine", "Warning light is on","Strange noise from the engine", "Headlights are not working", "Power steering failure","Transmission problem",
    "Exhaust system damage","Car won’t start","Tire tread is worn out","Leaking oil", "Windshield wipers are broken","Clutch is slipping",
    "Car stuck in gear","AC is not working","Heater is not functioning","Radiator is leaking", "Alternator failure","Starter motor failure",
    "Car is vibrating while driving","Brake pads are worn out","Broken timing belt","Suspension problem", "Check engine light is blinking","Fuel pump failure",
    "Spark plugs need replacement","Catalytic converter issue","Car pulling to one side while driving","Broken fan belt", "Electric window not working",
    "Central locking system failure","Flat spare tire","Key fob is not functioning","Damaged side mirror","Tail lights are not working","Hood won’t close",
    "Trunk won’t open","Excessive smoke from exhaust","Battery terminals are corroded","Parking brake is stuck","Fuel gauge is not accurate",
    "Airbag warning light is on","Steering wheel is stiff","Horn is not working","Dashboard lights flickering"
     };
        //I used GPT to create this array
        double[] longitudes = {
    34.8697, 34.8951, 35.2105, 34.8048, 34.8708,
    34.9482, 35.4732, 34.7746, 34.9640, 34.7915,
    35.3035, 35.5312, 34.6499, 34.5743, 34.8043,
    34.8500, 35.0738, 34.8447, 34.8111, 35.2137,
    34.8925, 34.9537, 34.8703, 34.7640, 35.5683,
    35.0063, 35.4960, 35.5773, 35.0345, 35.1047,
    34.7443, 34.8248, 34.9065, 34.8878, 35.2854,
    34.9197, 34.8019, 34.8383, 35.3047, 35.7496,
    35.5833, 35.2940, 35.5683, 35.0944, 34.7383,
    35.4500, 34.7872, 35.0247,34.3233,35.4545,32.0835,32.0722
};
        //I used GPT to create this array
        double[] latitudes = {
    32.3488, 31.9575, 31.7781, 32.1250, 32.0004,
    29.5580, 31.5590, 32.0628, 32.7980, 31.2525,
    32.6996, 32.7922, 31.8014, 31.6693, 31.9648,
    32.3329, 32.9234, 32.1663, 31.8941, 31.2586,
    32.5113, 32.5726, 32.1847, 31.6096, 33.2082,
    31.8996, 32.9646, 33.2790, 31.0707, 32.6672,
    32.0171, 32.0684, 32.1782, 32.0840, 32.6082,
    32.4340, 30.6100, 32.0853, 32.9170, 33.0622,
    32.8333, 31.7768, 33.2082, 33.0044, 31.8775,
    31.8667, 32.0158, 32.4746,31.2334,32.4554,34.8547,34.7934
};
        //I used GPT to create this array
        string[] addresses = {
    "Highway 2, Central Israel","Highway 6, Central Israel", "Begin Boulevard, Jerusalem","Glilot Interchange, Tel Aviv",
    "HaTmarim, Eilat", "Ein Gedi, Dead Sea","Rothschild Blvd, Tel Aviv","Carmel Beach, Haifa","Soroka Hospital, Beer Sheva",
    "El-Rama, Nazareth", "Kinneret, Tiberias","Port Road, Ashdod","Herzl, Ashkelon","Jabotinsky, Rishon Lezion","Ben Gurion, Netanya",
    "Shderot HaMeginim, Acre (Akko)","Sokolov, Herzliya","Weizmann Institute, Rehovot","Haatzmaut, Arad","Jabotinsky, Caesarea",
    "HaMeyasdim, Zikhron Ya'akov", "Ahuza, Raanana", "Jabotinsky, Kiryat Gat","Golan, Kiryat Shmona","Ze'ev Jabotinsky, Modiin", "HaRabbanim, Safed (Tzfat)",
    "HaGalil, Metula", "HaTomer, Dimona", "HaGilboa, Yokneam", "Ben Gurion, Bat Yam", "Jabotinsky, Ramat Gan",  "Herzl, Kfar Saba",
    "Rothschild Blvd, Petah Tikva", "Shderot HaMeginim, Afula",  "HaBanim, Hadera","Eilat, Mitzpe Ramon","Rabbi Akiva, Bnei Brak","HaMeyasdim, Karmiel",
    "Givat Yoav, Golan Heights","Ginosar, Sea of Galilee","HaNegev, Maale Adumim", "HaTamar, Qiryat Shemona","Sderot HaGalil, Nahariya", "Hachalutzim, Yavne",
    "Ein Esariya, Jericho Area","Sokolov, Holon","Harish Boulevard, Harish", "Bar Ilan, Bnei Brak","Eilat, Machtesh Rimon","Geha Highway","Ayalon Highway",
};

        for (int i = 0; i < 50; i++) // Loop to create 50 emergency calls
        {
            CallType _callType = (CallType)s_rand.Next(0, Enum.GetValues(typeof(CallType)).Length); // Random call type
            DateTime TimeOfOpen = new DateTime(s_dal!.Config.Clock.Hour - 2, 1, 1); // Call open time
            DateTime MaxTimeToFinish = TimeOfOpen.AddDays(s_rand.Next((s_dal!.Config.Clock - TimeOfOpen).Days)); // Maximum finish time

            // Define call data
            double Longitude = longitudes[i];
            double Latitude = latitudes[i];
            string? Address = addresses[i];
            string? Description = issues[i];

            // Add the call to the database
            s_dal!.Call.Create(new Call(_callType, Address, Longitude, Latitude, TimeOfOpen, MaxTimeToFinish, Description));
        }
    }

    // Function to create assignments linking volunteers to calls
    private static void createAssignments()
    {
        IEnumerable<Volunteer>? volunteers = s_dal!.Volunteer.ReadAll(); // Retrieve all volunteers
        IEnumerable<Call>? calls = s_dal!.Call.ReadAll(); // Retrieve all calls

        for (int i = 0; i < 50; i++) // Loop to create 50 assignments
        {
            DateTime minTime = calls.ElementAt(i).OpenTime; // Call open time
            DateTime maxTime = (DateTime)calls.ElementAt(i).MaxFinishTime!; // Maximum finish time
            TimeSpan difference = maxTime - minTime - TimeSpan.FromHours(2); // Time range

            // Generate a random time within the range
            int validDifference = (int)Math.Max(difference.TotalMinutes, 0);
            DateTime randomTime = minTime.AddMinutes(s_rand.Next(validDifference));

            // Create the assignment
            s_dal!.Assignment.Create(new Assignment(calls.ElementAt(s_rand.Next(calls.Count() - 15)).Id, volunteers.ElementAt(s_rand.Next(volunteers.Count())).Id, (EndOfTreatment)
                s_rand.Next(Enum.GetValues(typeof(EndOfTreatment)).Length - 1), randomTime.AddHours(2), randomTime));
        }
    }

    // Main function to start the initialization process
    public static void Do(IDal dal)
    {
        // Check if the DAL object is null
        s_dal = dal ?? throw new NullReferenceException("DAL object cannot be null!");

        Console.WriteLine("Reset Configuration values and List values...");
        s_dal.ResetDB(); // Reset the database

        // Call functions to create the entities
        createVolunteers();
        createCalls();
        createAssignments();
    }
}
