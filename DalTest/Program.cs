using Dal; // Namespace for the DAL implementation
using DalApi; // Namespace for the DAL API interface
using DO; // Namespace for Data Objects
using static DO.Enums; // Allows direct access to Enums without specifying the class name


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
            Console.WriteLine("Enter your details");
            Console.Write("Enter ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) throw new FormatException("your Id is invalid!");
            Console.Write("Enter your name");
            string name = Console.ReadLine()!;
            Console.Write("Enter your email");
            string email = Console.ReadLine()!;
            Console.Write("Enter your phone");
            string phone = Console.ReadLine()!;
            Console.Write("Enter your password");
            string password = Console.ReadLine()!;
            Console.Write("Enter  (1 for Manager, 2 for Volunteer, etc.): ");
            if (!Enum.TryParse(Console.ReadLine(), out Role role)) throw new FormatException("role is invalid!");
            Console.Write("Is Active? (true/false): ");
            bool Active = bool.Parse(Console.ReadLine()!);
            Console.Write("Enter your address");
            string address = Console.ReadLine()!;
            Console.Write("Enter your Maximum Distance");
            double MaximumDistance = double.Parse(Console.ReadLine()!);
            Console.Write("Enter your Latitude");
            double Latitude = double.Parse(Console.ReadLine()!);
            Console.Write("Enter your Longitude");
            double Longitude = double.Parse(Console.ReadLine()!);
            Console.Write("Enter Distance Type  (1 for Aerial Distance, 2 for walking Distance,3 for driving Distance, etc.): ");
            if (!Enum.TryParse(Console.ReadLine(), out DistanceType distanceType)) throw new FormatException("distanceType is invalid!");
            return new Volunteer(id, name, email, phone, role, Active, MaximumDistance, password, address, Longitude, Latitude, distanceType);
        }
        /// <summary>
        /// Function to create a new call
        /// </summary>
        /// <returns>A new call object</returns>
        private static Call CreateCall()
        {
            Console.WriteLine("Enter your details");
            Console.Write("Enter Call Type (1 for Type1, 2 for Type2, etc.): ");
            if (!Enum.TryParse(Console.ReadLine(), out Enums.CallType _callType)) throw new FormatException("callType is invalid!");
            Console.Write("Enter Description of the problem");
            string description = Console.ReadLine()!;
            Console.Write("Enter your address");
            string address = Console.ReadLine()!;
            Console.Write("Enter your Latitude");
            double Latitude = double.Parse(Console.ReadLine()!);
            Console.Write("Enter your Longitude");
            double Longitude = double.Parse(Console.ReadLine()!);
            Console.Write("Enter Opening Time (YYYY-MM-DD HH:MM): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime TimeOfOpen)) throw new FormatException("TimeOfOpen is invalid!");
            Console.Write("Enter Max Time Finish Calling (YYYY-MM-DD HH:MM): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime MaxTimeToFinish)) throw new FormatException("MaxTimeToFinish is invalid!");
            return new Call(_callType, address, Longitude, Latitude, TimeOfOpen, MaxTimeToFinish, description);

        }
        /// <summary>
        /// Function to create a new assignment
        /// </summary>
        /// <returns>A new assignment object</returns>
        private static Assignment CreateAssignment()
        {
            Console.WriteLine("Enter your details");
            Console.Write("Enter Call ID: ");
            if (!int.TryParse(Console.ReadLine(), out int CallId)) throw new FormatException("your Id is invalid!");
            Console.Write("Enter Volunteer ID: ");
            if (!int.TryParse(Console.ReadLine(), out int volunteerId)) throw new FormatException("volunteer Id is invalid!");
            Console.Write("Enter Type Of End Time : 1 for treated, 2 for Self Cancellation,3 for CancelingAnAdministrator,4 for CancellationHasExpired ");
            if (!Enum.TryParse(Console.ReadLine(), out EndOfTreatment typeOfEndTime)) throw new FormatException("type Of End Time is invalid!");
            Console.Write("Enter Ending Time of Treatment ( YYYY-MM-DD HH:MM): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime EndTime)) throw new FormatException("EndTime is invalid!");
            return new Assignment(CallId, volunteerId, typeOfEndTime, EndTime);
        }
        /// <summary>
        /// Function to create a new entity and add it to the relevant list
        /// </summary>
        /// <param name="choice">Entity type (Volunteer, Call, Assignment)</param>
        private static void CreateEntity(string choice, dynamic entityType)
        {
            try
            {


                switch (choice)
                {
                    case "Volunteer":
                        Volunteer Vol = CreateVolunteer();
                        entityType.Create(Vol);
                        break;
                    case "Call":
                        Call Call = CreateCall();
                        entityType.Create(Call);
                        break;
                    case "Assignment":
                        Assignment Ass = CreateAssignment();
                        entityType.Create(Ass);
                        break;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Create : {ex.Message}");

            }

        }
        /// <summary>
        /// The function creates or updates an entity in its respective array.
        /// </summary>
        /// <param name="choice">The submenu option indicating which entity to handle.</param>
        /// <exception cref="FormatException">Thrown if the user provides an invalid ID.</exception>
        private static void UpdateEntity(string choice, dynamic entityType)
        {
            try
            {
            Console.WriteLine("Enter your details");
            Console.Write("Enter ID: ");

           

            // Handle updates based on the submenu choice.
            switch (choice)
            {
                case "Volunteer":
                    // Create and update a volunteer entity.
                    Volunteer Vol = CreateVolunteer();
                     entityType.Update(Vol);
                    break;
                case "Call":
                    // Create and update a call entity.
                    Call Call = CreateCall();
                      entityType.Update(Call);
                    break;
                case "Assignment":
                    // Create and update an assignment entity.
                    Assignment Ass = CreateAssignment();
                    entityType.Update(Ass);
                    break;
            }
            }catch(Exception ex) {
                Console.WriteLine($"Error in Update : {ex.Message}");

            }

        }

        /// <summary>
        /// Reads a specific entity from the database based on its ID.
        /// </summary>
        /// <param name="choice">The submenu option indicating which entity to read.</param>
        /// <exception cref="FormatException">Thrown if the user provides an invalid ID.</exception>
        private static void ReadEntity(dynamic entityType)
        {
            try
            {
                Console.WriteLine("Enter Your ID");
                // Parse the ID; throw an exception for invalid input.
                if (!int.TryParse(Console.ReadLine(), out int yourId)) throw new InvalidFormatException("Your ID is invalid!");
                entityType.Read(yourId);

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
        private static void ReadAllEntity( dynamic entityType)
        {
            try
            {
                foreach (var item in entityType.ReadAll())
                    Console.WriteLine(item);
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
        /// <exception cref="FormatException">Thrown if the user provides an invalid ID.</exception>
        private static void DeleteEntity(dynamic entityType)
        {
            try
            {
                Console.WriteLine("Enter ID: ");

                // Parse the ID; throw an exception for invalid input.
                if (!int.TryParse(Console.ReadLine(), out int yourId)) throw new InvalidFormatException("Your ID is invalid!");

                // Delete the specified entity based on the submenu choice.
                entityType.Delete(yourId);
                Console.WriteLine("Entity successfully deleted.");

            }
            catch (Exception ex) {
                Console.WriteLine($"Error in delete : {ex.Message}");

            }


        }

  

        /// <summary>
        /// Displays a menu for a specific entity and handles user actions.
        /// </summary>
        /// <param name="choice">The submenu option indicating which entity to manage.</param>
        /// <exception cref="FormatException">Thrown if the user provides an invalid choice.</exception>
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
                        CreateEntity(choice, entityType);
                            break;
                    case SubMenu.Read:
                            ReadEntity(entityType);
                            break;
                    case SubMenu.ReadAll:
                            ReadAllEntity(entityType);
                            break;
                    case SubMenu.Delete:
                            DeleteEntity(entityType);
                            break;
                    case SubMenu.DeleteAll:
                            entityType.DeleteAll();
                            Console.WriteLine("Entity successfully deleted all.");

                            break;
                    case SubMenu.UpDate:
                            UpdateEntity(choice, entityType);
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
                            ReadAllEntity(s_dal.Volunteer);
                            ReadAllEntity(s_dal.Call);
                            ReadAllEntity(s_dal.Assignment);
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
