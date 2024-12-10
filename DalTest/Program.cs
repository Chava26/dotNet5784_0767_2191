using Dal; // Namespace for the DAL implementation
using DalApi; // Namespace for the DAL API interface
using DO; // Namespace for Data Objects
//using static DO; // Allows direct access to Enums without specifying the class name


namespace DalTest
{

    internal class Program
    {

        // Static initialization of the DAL instance (Stage 2 implementation)
        static readonly IDal s_dal = new DalList();

        /// <summary>
        /// Main menu options for user interaction
        /// </summary>
        public enum MainMenu
        {  ExitMainMenu,  AssignmentSubmenu,  VolunteerSubmenu,  CallSubmenu,  InitializeData,  DisplayAllData,  ConfigSubmenu,  ResetDatabase
        }
        /// <summary>
        /// Submenu options for CRUD operations
        /// </summary>
        public enum SubMenu
        {
            Exit, Create, Read, ReadAll, UpDate, Delete, DeleteAll
        }

        /// <summary>
        /// Options specific to the Config submenu
        /// </summary>
        private enum ConfigSubmenu
        {
            Exit,AdvanceClockByMinute,AdvanceClockByHour,AdvanceClockByDay,AdvanceClockByMonth,AdvanceClockByYear,DisplayClock,ChangeClockOrRiskRange,DisplayConfigVar,   Reset
        }
        /// <summary>
        /// Function to create a new volunteer
        /// </summary>
        /// <param name="id">Unique ID for the volunteer</param>
        /// <returns>A new volunteer object</returns>
        private static Volunteer CreateVolunteer()
        {
            Console.Write("Enter ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) throw new FormatException("your Id is invalid!");
            Console.Write("Enter your name: ");
            string name = Console.ReadLine()!;
            Console.Write("Enter your email: ");
            string email = Console.ReadLine()!;
            Console.Write("Enter your phone: ");
            string phone = Console.ReadLine()!;
            Console.Write("Enter your password: ");
            string password = Console.ReadLine()!;
            Console.Write("Enter  (1 for Manager, 2 for Volunteer, etc.): ");
            if (!Enum.TryParse(Console.ReadLine(), out Role role)) throw new FormatException("role is invalid!");
            Console.Write("Is Active? (true/false): ");
            bool Active = bool.Parse(Console.ReadLine()!);
            Console.Write("Enter your address: ");
            string address = Console.ReadLine()!;
            Console.Write("Enter your Maximum Distance: ");
            double MaximumDistance = double.Parse(Console.ReadLine()!);
            Console.Write("Enter your Latitude: ");
            double Latitude = double.Parse(Console.ReadLine()!);
            Console.Write("Enter your Longitude: ");
            double Longitude = double.Parse(Console.ReadLine()!);
            Console.Write("Enter Distance Type  (1 for Aerial Distance, 2 for walking Distance,3 for driving Distance, etc.): ");
            if (!Enum.TryParse(Console.ReadLine(), out DistanceType distanceType)) throw new FormatException("distanceType is invalid!");
            return new Volunteer(id, name, email, phone, role, Active, MaximumDistance, password, address, Longitude, Latitude, distanceType);
        }
        /// <summary>
        /// Function to create a new call
        /// </summary>
        /// <returns>A new call object</returns>
        private static Call CreateCall(bool ForUpdate = false)
        {

            Console.Write("Enter Call Type (1 for Type1, 2 for Type2, etc.): ");
            if (!Enum.TryParse(Console.ReadLine(), out Enums.CallType _callType)) throw new FormatException("callType is invalid!");
            Console.Write("Enter Description of the problem:  ");
            string description = Console.ReadLine()!;
            Console.Write("Enter your address: ");
            string address = Console.ReadLine()!;
            Console.Write("Enter your Latitude: ");
            double Latitude = double.Parse(Console.ReadLine()!);
            Console.Write("Enter your Longitude");
            double Longitude = double.Parse(Console.ReadLine()!);
            Console.Write("Enter Opening Time (YYYY-MM-DD HH:MM): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime TimeOfOpen)) throw new FormatException("TimeOfOpen is invalid!");
            Console.Write("Enter Max Time Finish Calling (YYYY-MM-DD HH:MM): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime MaxTimeToFinish)) throw new FormatException("MaxTimeToFinish is invalid!");
            if (ForUpdate)
            {
                Console.Write("Enter Assignment ID for update: ");
                if (!int.TryParse(Console.ReadLine(), out int CallId)) throw new InvalidFormatException("Calll Id is invalid!");
                return new Call(_callType, address, Longitude, Latitude, TimeOfOpen, MaxTimeToFinish, description, CallId);
            }
            else
            {
                return new Call(_callType, address, Longitude, Latitude, TimeOfOpen, MaxTimeToFinish, description);
            }
        }
        /// <summary>
        /// Function to create a new assignment
        /// </summary>
        /// <exception cref="InvalidFormatException">Thrown if the user provides an invalid ID.</exception>

        /// <returns>A new assignment object</returns>
        private static Assignment CreateAssignment(bool ForUpdate=false)
        {
            Console.WriteLine("Enter your details for ");
            Console.Write("Enter Call ID: ");
            if (!int.TryParse(Console.ReadLine(), out int CallId)) throw new InvalidFormatException("call Id is invalid!");
            Console.Write("Enter Volunteer ID: ");
            if (!int.TryParse(Console.ReadLine(), out int volunteerId)) throw new InvalidFormatException("volunteer Id is invalid!");
            Console.Write("Enter Type Of End Time : 1 for treated, 2 for Self Cancellation,3 for CancelingAnAdministrator,4 for CancellationHasExpired: ");
            if (!Enum.TryParse(Console.ReadLine(), out EndOfTreatment typeOfEndTime)) throw new InvalidFormatException("type Of End Time is invalid!");
            Console.Write("Enter entry Time of Treatment ( YYYY-MM-DD HH:MM): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime EntryTime)) throw new InvalidFormatException("EndTime is invalid!");
            Console.Write("Enter exit Time of Treatment ( YYYY-MM-DD HH:MM): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime exitTime)) throw new InvalidFormatException("EndTime is invalid!");

            if (ForUpdate)
            {
                Console.Write("Enter Assignment ID for update: ");
                if (!int.TryParse(Console.ReadLine(), out int AssignmentId)) throw new InvalidFormatException("Assignment Id is invalid!");
                return new Assignment(CallId, volunteerId, typeOfEndTime, EntryTime, exitTime,AssignmentId);
            }
            else
            {
                return new Assignment(CallId, volunteerId, typeOfEndTime, EntryTime, exitTime);

            }

        }
        /// <summary>
        /// Function to create a new entity and add it to the relevant list
        /// </summary>
        /// <param name="choice">Entity type (Volunteer, Call, Assignment)</param>
        private static void CreateEntity(string choice)
        {
            try
            {
                Console.WriteLine("Enter your details for create");

                switch (choice)
                {
                    case "Volunteer":
                        Volunteer Vol = CreateVolunteer();
                        s_dal!.Volunteer.Create(Vol);
                        break;
                    case "Call":
                        Call Call = CreateCall();
                        s_dal!.Call.Create(Call);
                        break;
                    case "Assignment":
                        Assignment Ass = CreateAssignment();
                        s_dal.Assignment.Create(Ass);
                        break;
                }
                Console.WriteLine($"Create {choice} is sucsses");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Create {ex.Message}");

            }

        }
        /// <summary>
        /// The function creates or updates an entity in its respective array.
        /// </summary>
        /// <param name="choice">The submenu option indicating which entity to handle.</param>
        /// <exception cref="InvalidFormatException">Thrown if the user provides an invalid ID.</exception>
        private static void UpdateEntity(string choice)
        {
            try
            {
             Console.Write("Enter a new detial with same id for update: ");
            // Handle updates based on the submenu choice.
            switch (choice)
            {
                    case "Volunteer":
                        // Create and update a volunteer entity.
                        Volunteer Vol = CreateVolunteer();
                        s_dal!.Volunteer.Update(Vol);
                        break;
                    case "Call":
                        // Create and update a call entity.
                        Call Call = CreateCall(true);
                        s_dal.Call.Update(Call);
                        break;
                    case "Assignment":
                        // Create and update an assignment entity.
                        Assignment Ass = CreateAssignment(true);
                        s_dal.Assignment.Update(Ass);
                        break;
                }
                Console.WriteLine($"update {choice} is sucsses");

            }
            catch (Exception ex) {
                Console.WriteLine($"Error in Update : {ex.Message}");
            }

        }
        /// <summary>
        /// Reads a specific entity from the database based on its ID.
        /// </summary>
        /// <param name="choice">The submenu option indicating which entity to read.</param>
        /// <exception cref="InvalidFormatException">Thrown if the user provides an invalid ID.</exception>
        private static void ReadEntity(string choice)
        {
            try {
                Console.Write($"Enter {choice} ID to read: ");

                // Parse the ID; throw an exception for invalid input.
                if (!int.TryParse(Console.ReadLine(), out int yourId)) throw new InvalidFormatException("Your ID is invalid!");

            // Read the specified entity based on the submenu choice.
            switch (choice)
            {
                case "Volunteer":
                        Console.WriteLine(s_dal!.Volunteer.Read(yourId));
                    break;
                case "Call":
                        Console.WriteLine(s_dal!.Call.Read(yourId)); 
                    break;
                case "Assignment":
                        Console.WriteLine(s_dal!.Assignment.Read(yourId)); 
                    break;

            }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Read : {ex.Message}");
            }

        }

        /// <summary>
        /// Reads all entities from a specific array and displays them.
        /// </summary>
        /// <param name="choice">The submenu option indicating which entities to read.</param>
        private static void ReadAllEntity(string choice)
        {
            try
            {
                switch (choice)
                {
                    case "Volunteer":
                        foreach (var item in s_dal!.Volunteer.ReadAll())
                            Console.WriteLine(item);
                        break;
                    case "Call":
                        foreach (var item in s_dal!.Call.ReadAll())
                            Console.WriteLine(item);
                        break;
                    case "Assignment":
                        foreach (var item in s_dal.Assignment.ReadAll())
                            Console.WriteLine(item);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ReadAll : {ex.Message}");
            }

        }

        /// <summary>
        /// Deletes a specific entity from its array based on its ID.
        /// </summary>   
        /// <param name="choice">The submenu option indicating which entity to delete.</param>
        /// <exception cref="InvalidFormatException">Thrown if the user provides an invalid ID.</exception>
        private static void DeleteEntity(string choice)
        {
            try
            {
                Console.WriteLine("Enter ID: ");
                // Parse the ID; throw an exception for invalid input.
                if (!int.TryParse(Console.ReadLine(), out int yourId)) throw new InvalidFormatException("Your ID is invalid!");
                // Delete the specified entity based on the submenu choice.

                switch (choice)
                {
                    case "VolunteerSubmenu":
                        s_dal!.Volunteer.Delete(yourId);
                        break;
                    case "CallSubmenu":
                        s_dal.Call.Delete(yourId);
                        break;
                    case "AssignmentSubmenu":
                        s_dal.Assignment.Delete(yourId);
                        break;
                }
                Console.WriteLine("Enter ID: ");

            }
            catch (Exception ex) {
                Console.WriteLine($"Error in delete : {ex.Message}");

            }


        }


        /// <summary>
        /// Deletes all entities in a specific array.
        /// </summary>
        /// <param name="choice">The submenu option indicating which entities to delete.</param>
        private static void DeleteAllEntity(string choice)
        {
            try { 
            // Delete all entities of the chosen type.
            switch (choice)
            {
                case "VolunteerSubmenu":
                    s_dal.Volunteer.DeleteAll();
                    break;
                case "CallSubmenu":
                    s_dal.Call.DeleteAll();
                    break;
                case "AssignmentSubmenu":
                    s_dal.Assignment.DeleteAll();
                    break;
            }
            Console.WriteLine("Entity successfully deleted all.");
            }
            catch (Exception ex) {
                Console.WriteLine($"Error in delete all: {ex.Message}");
            }
        }
        /// <summary>
        /// Displays a menu for a specific entity and handles user actions.
        /// </summary>
        /// <param name="choice">The submenu option indicating which entity to manage.</param>
        /// <exception cref="InvalidFormatException">Thrown if the user provides an invalid choice.</exception>
        private static void EntityMenu(string choice, dynamic entityType)
        {
          try { 
            Console.WriteLine("Menu for entity");

            // Display all submenu options.
            foreach (SubMenu option in Enum.GetValues(typeof(SubMenu)))
            {
                Console.WriteLine($"{(int)option}. {option}");
            }

            Console.WriteLine("Enter a number");

            // Parse user input; throw an exception for invalid input.
            if (!Enum.TryParse(Console.ReadLine(), out SubMenu subChoice))
                throw new InvalidFormatException("The sub menu choice is not valid.");

            // Handle actions until the user exits.
            while (subChoice != SubMenu.Exit)
            {
               
                switch (subChoice)
                {
                    case SubMenu.Create:
                        CreateEntity(choice);
                            break;
                    case SubMenu.Read:
                            ReadEntity(choice);
                            break;
                    case SubMenu.ReadAll:
                            ReadAllEntity(choice);
                            break;
                    case SubMenu.Delete:
                            DeleteEntity(choice);
                            break;
                    case SubMenu.DeleteAll:
                            DeleteAllEntity(choice);

                            break;
                    case SubMenu.UpDate:
                            UpdateEntity(choice);
                        break;
                    default:
                        Console.WriteLine("Your choice is not valid, please enter again");
                        break;
                }

                Console.WriteLine("Enter a number");
                if (!Enum.TryParse(Console.ReadLine(), out subChoice))
                    throw new InvalidFormatException("The sub menu choice is not valid.");
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in choice action : {ex.Message}");
            }
        }

        /// <summary>
        /// Displays the configuration submenu and handles configuration options.
        /// </summary>
        private static void ConfigSubmenuu()
        {
            try {
            Console.WriteLine("Config Menu:");

            // Display all configuration options.
            foreach (ConfigSubmenu option in Enum.GetValues(typeof(ConfigSubmenu)))
            {
                Console.WriteLine($"{(int)option}. {option}");
            }

            Console.Write("Select an option: ");

            // Parse user input; throw an exception for invalid input.
            if (!Enum.TryParse(Console.ReadLine(), out ConfigSubmenu userInput))
                throw new InvalidFormatException("The Config menu choice is not valid.");
                // Handle actions until the user exits.
                while (userInput is not ConfigSubmenu.Exit)
                {
                    switch (userInput)
                    {
                        case ConfigSubmenu.AdvanceClockByMinute:
                            s_dal!.Config.Clock = s_dal!.Config.Clock.AddMinutes(1);
                            break;
                        case ConfigSubmenu.AdvanceClockByHour:
                            s_dal!.Config.Clock = s_dal!.Config.Clock.AddHours(1);
                            break;
                        case ConfigSubmenu.AdvanceClockByDay:
                            s_dal!.Config.Clock = s_dal!.Config.Clock.AddDays(1);
                            break;
                        case ConfigSubmenu.AdvanceClockByMonth:
                            s_dal!.Config.Clock = s_dal!.Config.Clock.AddMonths(1);
                            break;
                        case ConfigSubmenu.AdvanceClockByYear:
                            s_dal!.Config.Clock = s_dal!.Config.Clock.AddYears(1);
                            break;
                        case ConfigSubmenu.DisplayClock:
                            Console.WriteLine(s_dal!.Config.Clock);
                            break;
                        case ConfigSubmenu.ChangeClockOrRiskRange:
                            Console.WriteLine($"RiskRange: {s_dal.Config!.RiskRange}");
                            break;
                        case ConfigSubmenu.DisplayConfigVar:
                            Console.Write("Write new format for RiskRange (e.g., seconds, minutes, hours): ");
                            string riskRangeInput = Console.ReadLine()!;
                            if (TimeSpan.TryParse(riskRangeInput, out TimeSpan newRiskRange))
                            {
                                s_dal!.Config.RiskRange = newRiskRange;
                                Console.WriteLine($"RiskRange updated to: {s_dal.Config.RiskRange}");
                            }
                            else
                            {
                                Console.WriteLine("Invalid format.");
                            }
                            break;
                        case ConfigSubmenu.Reset:
                            s_dal!.ResetDB();
                            break;
                    }

                    Console.Write("Select an option: ");
                    if (!Enum.TryParse(Console.ReadLine(), out userInput))
                        throw new InvalidFormatException("The Config menu choice is not valid.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in config menu : {ex.Message}");

            }
        }
        /// <summary>
        /// Main function that displays the main menu and handles user actions.
        /// </summary>
        /// <param name="args">Command-line arguments (not used in this implementation).</param>
        /// <exception cref="FormatException">Thrown if the user provides an invalid choice.</exception>
        private static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Main Menu:");

                // Display all main menu options.
                foreach (MainMenu option in Enum.GetValues(typeof(MainMenu)))
                {
                    Console.WriteLine($"{(int)option}. {option}");
                }

                Console.Write("Select an option: ");

                // Parse user input; throw an exception for invalid input.
                if (!Enum.TryParse(Console.ReadLine(), out MainMenu userInput))
                    throw new InvalidFormatException("The menu choice is not valid.");

                // Handle actions until the user exits the main menu.
                while (userInput is not MainMenu.ExitMainMenu)
                {
                    switch (userInput)
                    {
                        case MainMenu.AssignmentSubmenu:
                            EntityMenu("Assignment", s_dal.Assignment);
                            break;
                        case MainMenu.VolunteerSubmenu:
                            EntityMenu("Volunteer", s_dal.Volunteer);
                            break;
                        case MainMenu.CallSubmenu:
                            EntityMenu("Call", s_dal.Call);
                            break;
                        case MainMenu.InitializeData:
                            Initialization.Do(s_dal); // Initialize data.
                            break;
                        case MainMenu.DisplayAllData:
                            ReadAllEntity("VolunteerSubmenu");
                            ReadAllEntity("CallSubmenu");
                            ReadAllEntity("AssignmentSubmenu");
                            break;
                        case MainMenu.ConfigSubmenu:
                            ConfigSubmenuu();
                            break;
                        case MainMenu.ResetDatabase:
                            s_dal!.ResetDB(); // Reset the database.
                            break;
                    }

                    // Display the main menu again after an action.
                    Console.WriteLine("Main Menu:");
                    foreach (MainMenu option in Enum.GetValues(typeof(MainMenu)))
                    {
                        Console.WriteLine($"{(int)option}. {option}");
                    }

                    Console.Write("Select an option: ");
                    if (!Enum.TryParse(Console.ReadLine(), out userInput))
                        throw new InvalidFormatException("The menu choice is not valid.");
                }
            }
            catch (Exception ex)
            {
                // Catch and display any exceptions.
                Console.WriteLine($"Error in Main function: {ex.Message}");
            }

        }
    }

}
