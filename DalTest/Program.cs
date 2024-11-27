using Dal;
using DalApi;
using DO;
using Microsoft.VisualBasic;
using System.ComponentModel.Design;
using static DO.Enums;


namespace DalTest
{

    internal class Program
    {
        public static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); //stage 1
        public static ICall? s_dalCall = new CallImplementation(); //stage 1
        public static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
        public static IConfig? s_dalConfig = new ConfigImplementation();
        public enum MainMenu
        {
            ExitMainMenu,
            AssignmentSubmenu,
            VolunteerSubmenu,
            CallSubmenu,
            InitializeData,
            DisplayAllData,
            ConfigSubmenu,
            ResetDatabase
        }
        public enum SubMenu
        {
            Exit,
            Create,
            Read,
            ReadAll,
            UpDate,
            Delete,
            DeleteAll
        }
        private enum ConfigSubmenu
        {
            Exit,
            AdvanceClockByMinute,
            AdvanceClockByHour,
            AdvanceClockByDay,
            AdvanceClockByMonth,
            AdvanceClockByYear,
            DisplayClock,
            ChangeClockOrRiskRange,
            DisplayConfigVar,
            Reset
        }
        private static Volunteer CreateVolunteer(int id)
        {
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
        private static Call CreateCall()
        {

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

        private static Assignment CreateAssignment(int id)
        {
            Console.Write("Enter Call ID: ");
            if (!int.TryParse(Console.ReadLine(), out int CallId)) throw new FormatException("your Id is invalid!");
            Console.Write("Enter Volunteer ID: ");
            int volunteerId = int.Parse(Console.ReadLine()!);
            Console.Write("Enter Type Of End Time : 1 for treated, 2 for Self Cancellation,3 for CancelingAnAdministrator,4 for CancellationHasExpired ");
            if (!Enum.TryParse(Console.ReadLine(), out EndOfTreatment typeOfEndTime)) throw new FormatException("type Of End Time is invalid!");
            Console.Write("Enter Ending Time of Treatment ( YYYY-MM-DD HH:MM): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime EndTime)) throw new FormatException("EndTime is invalid!");
            return new Assignment(id, CallId, volunteerId, typeOfEndTime, EndTime);
        }

        private static void Create(string choice)
        {
            Console.WriteLine("Enter your details");
            Console.Write("Enter ID: ");
            if (!int.TryParse(Console.ReadLine(), out int yourId)) throw new FormatException("your Id is invalid!");

            switch (choice)
            {
                case "VolunteerSubmenu":
                    Volunteer Vol = CreateVolunteer(yourId);
                    s_dalVolunteer!.Create(Vol);
                    break;
                case "CallSubmenu":
                    Call Call = CreateCall();
                    s_dalCall!.Create(Call);
                    break;
                case "AssignmentSubmenu":
                    Assignment Ass = CreateAssignment(yourId);
                    s_dalAssignment!.Create(Ass);
                    break;

            }
        }
        private static void Update(string choice)
        {
            Console.WriteLine("Enter your details");
            Console.Write("Enter ID: ");
            if (!int.TryParse(Console.ReadLine(), out int yourId)) throw new FormatException("your Id is invalid!");

            switch (choice)
            {
                case "VolunteerSubmenu":
                    Volunteer Vol = CreateVolunteer(yourId);
                    s_dalVolunteer!.Update(Vol);
                    break;
                case "CallSubmenu":
                    Call Call = CreateCall();
                    s_dalCall!.Update(Call);
                    break;
                case "AssignmentSubmenu":
                    Assignment Ass = CreateAssignment(yourId);
                    s_dalAssignment!.Update(Ass);
                    break;
            }
        }
        private static void Read(string choice)
        {
            Console.WriteLine("Enter ID: ");
            if (!int.TryParse(Console.ReadLine(), out int yourId)) throw new FormatException("your Id is invalid!");
            switch (choice)
            {
                case "VolunteerSubmenu":
                    s_dalVolunteer!.Read(yourId);
                    break;
                case "CallSubmenu":
                    s_dalCall!.Delete(yourId);
                    break;
                case "AssignmentSubmenu":
                    s_dalAssignment!.Delete(yourId);
                    break;
            }
        }
        private static void ReadAll(string choice)
        {

            switch (choice)
            {
                case "VolunteerSubmenu":
                    foreach (var item in s_dalVolunteer!.ReadAll())
                        Console.WriteLine(item);
                    break;
                case "CallSubmenu":
                    foreach (var item in s_dalCall!.ReadAll())
                        Console.WriteLine(item);
                    break;
                case "AssignmentSubmenu":
                    foreach (var item in s_dalAssignment!.ReadAll())
                        Console.WriteLine(item);
                    break;
            }
        }
        private static void Delete(string choice)
        {
            Console.WriteLine("Enter ID: ");
            if (!int.TryParse(Console.ReadLine(), out int yourId)) throw new FormatException("your Id is invalid!");
            switch (choice)
            {
                case "VolunteerSubmenu":
                    s_dalVolunteer!.Delete(yourId);
                    break;
                case "CallSubmenu":
                    s_dalCall!.Delete(yourId);
                    break;
                case "AssignmentSubmenu":
                    s_dalAssignment!.Delete(yourId);
                    break;
            }
        }

        private static void DeleteAll(string choice)
        {
            switch (choice)
            {
                case "VolunteerSubmenu":
                    s_dalVolunteer!.DeleteAll();
                    break;
                case "CallSubmenu":
                    s_dalCall!.DeleteAll();
                    break;
                case "AssignmentSubmenu":
                    s_dalAssignment!.DeleteAll();
                    break;
            }
        }
        private static void EntityMenu(string choice)
        {
            Console.WriteLine("Menu for entity");
            foreach (SubMenu option in Enum.GetValues(typeof(SubMenu)))
            {
                Console.WriteLine($"{(int)option}. {option}");
            }
            Console.WriteLine("Enter a number");
            bool isValid = Enum.TryParse(Console.ReadLine(), out SubMenu subChoice);
            while (!isValid)
            {
                Console.WriteLine("Your choise number is not valid, please enter again");
                isValid = Enum.TryParse(Console.ReadLine(), out subChoice);
            }
            //if (!Enum.TryParse(Console.ReadLine(), out SubMenu subChoice)) throw new FormatException("Invalid choice");
            while (subChoice != 0)
            {
                switch (subChoice)
                {
                    case SubMenu.Create:
                        Create(choice);
                        break;
                    case SubMenu.Read:
                        Console.WriteLine("Enter Your ID");

                        Read(choice);
                        break;
                    case SubMenu.ReadAll:
                        ReadAll(choice);
                        break;
                    case SubMenu.Delete:
                        Delete(choice);
                        break;
                    case SubMenu.DeleteAll:
                        DeleteAll(choice);
                        break;
                    case SubMenu.UpDate:
                        Update(choice);
                        break;
                    case SubMenu.Exit:
                        return;
                    default:
                        Console.WriteLine("Your choise is not valid, please enter again");
                        break;
                }
                Console.WriteLine("Enter a number");
                bool _isValid = Enum.TryParse(Console.ReadLine(), out subChoice);
                while (!_isValid)
                {
                    Console.WriteLine("Your choise number is not valid, please enter again");
                    _isValid = Enum.TryParse(Console.ReadLine(), out subChoice);

                }
            }
        }
        private static void ConfigSubmenuu()
        {
            Console.WriteLine("Config Menu:");
            foreach (ConfigSubmenu option in Enum.GetValues(typeof(ConfigSubmenu)))
            {
                Console.WriteLine($"{(int)option}. {option}");
            }
            Console.Write("Select an option: ");
            bool isVaild = Enum.TryParse(Console.ReadLine(), out ConfigSubmenu userInput);
            while (!isVaild){
                Console.WriteLine("Your choise  is not valid, please enter again");
                isVaild = Enum.TryParse(Console.ReadLine(), out userInput);
            }
            while (userInput is not ConfigSubmenu.Exit)
                {
                    switch (userInput)
                    {
                        case ConfigSubmenu.AdvanceClockByMinute:

                            s_dalConfig!.Clock = s_dalConfig.Clock.AddMinutes(1);
                            break;
                        case ConfigSubmenu.AdvanceClockByHour:
                            s_dalConfig!.Clock = s_dalConfig.Clock.AddHours(1);
                            break;
                        case ConfigSubmenu.AdvanceClockByDay:
                            s_dalConfig!.Clock = s_dalConfig.Clock.AddDays(1);
                            break;
                        case ConfigSubmenu.AdvanceClockByMonth:
                            s_dalConfig!.Clock = s_dalConfig.Clock.AddMonths(1);
                            break;
                        case ConfigSubmenu.AdvanceClockByYear:
                            s_dalConfig!.Clock = s_dalConfig.Clock.AddYears(1);
                            break;
                        case ConfigSubmenu.DisplayClock:
                            Console.WriteLine(s_dalConfig!.Clock);
                            break;
                        case ConfigSubmenu.ChangeClockOrRiskRange:
                            Console.WriteLine($"RiskRange : {s_dalConfig!.RiskRange}");
                            break;
                        case ConfigSubmenu.DisplayConfigVar:
                            Console.Write("write new format for RiskRange (with seconds minutes  and hours): ");
                            string riskRangeInput = Console.ReadLine()!;
                            if (TimeSpan.TryParse(riskRangeInput, out TimeSpan newRiskRange)) 
                            {
                                s_dalConfig!.RiskRange = newRiskRange;
                                Console.WriteLine($"RiskRange update to: {s_dalConfig.RiskRange}");
                            }
                           else{
                            Console.WriteLine($"your format not good");
                            }
                            break;

                        case ConfigSubmenu.Reset:
                            s_dalConfig!.Reset();
                            break;


                    }

                    bool isValid = Enum.TryParse(Console.ReadLine(), out userInput);
                    while (!isValid)
                    {
                        Console.WriteLine("Your choise number is not valid, please enter again");
                        isValid = Enum.TryParse(Console.ReadLine(), out userInput);

                    }
               
            } 
        }
        private static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Main Menu:");
                foreach (MainMenu option in Enum.GetValues(typeof(MainMenu)))
                {
                    Console.WriteLine($"{(int)option}. {option}");
                }
                Console.Write("Select an option: ");
                bool isVaild = Enum.TryParse(Console.ReadLine(), out MainMenu userInput);
                while (!isVaild)
                {
                    Console.WriteLine("Your choise  is not valid, please enter again");
                    isVaild = Enum.TryParse(Console.ReadLine(), out  userInput);
                }
                while (userInput is not MainMenu.ExitMainMenu)
                {
                    switch (userInput)
                    {
                        case MainMenu.AssignmentSubmenu:
                        case MainMenu.VolunteerSubmenu:
                        case MainMenu.CallSubmenu:
                            string sChoice = userInput.ToString();
                            EntityMenu(sChoice);
                            break;
                        case MainMenu.InitializeData:
                            Initialization.Do(s_dalVolunteer, s_dalCall, s_dalAssignment, s_dalConfig);
                            break;
                        case MainMenu.DisplayAllData:
                            {
                                ReadAll("VolunteerSubmenu");
                                ReadAll("CallSubmenu");
                                ReadAll("AssignmentSubmenu");
                            }
                            break;
                        case MainMenu.ConfigSubmenu:

                            ConfigSubmenuu();

                            break;
                        case MainMenu.ResetDatabase:

                            s_dalConfig!.Reset(); //stage 1
                            s_dalVolunteer!.DeleteAll(); //stage 1
                            s_dalCall!.DeleteAll(); //stage 1
                            s_dalAssignment!.DeleteAll(); //stage 1

                            break;
                    }
                    Console.WriteLine("Main Menu:");
                    foreach (MainMenu option in Enum.GetValues(typeof(MainMenu)))
                    {
                        Console.WriteLine($"{(int)option}. {option}");
                    }
                    Console.Write("Select an option: ");
                    bool isValid = Enum.TryParse(Console.ReadLine(), out userInput);
                    while (!isValid)
                    {
                        Console.WriteLine("Your choise number is not valid, please enter again");
                        isValid = Enum.TryParse(Console.ReadLine(), out userInput);

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}



