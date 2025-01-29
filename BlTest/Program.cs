using System;
using BlApi;
using BO;
using DO;
/// <summary>
/// Main class for managing the application menus and execution.
/// </summary>
class Program
{
    static readonly IBl s_bl = Factory.Get();


    /// <summary>
    /// Main entry point of the application. Displays the main menu and routes user choices.
    /// </summary>
    static void Main()
        {
        try
        {
            while (true)
            {
                Console.WriteLine("Main Menu:");
                Console.WriteLine("1. Volunteer Management");
                Console.WriteLine("2. Call Management");
                Console.WriteLine("3. Admin Management");
                Console.WriteLine("0. Exit");
                Console.Write("Choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                    throw new InvalidFormatException("The sub menu choice is not valid.");


                switch (choice)
                {
                    case 1:
                        VolunteerMenu();
                        break;
                    case 2:
                        CallMenu();
                        break;
                    case 3:
                        AdminMenu();
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            throw new BO.GenralInitializationExcption("A custom error occurred in Initialization.", ex);
        }
    }
    /// <summary>
    /// Displays and manages the admin menu.
    /// </summary>
    static void AdminMenu()
        {
            while (true)
            {
                Console.WriteLine("\nAdmin Management:");
                Console.WriteLine("1. Get System Clock");
                Console.WriteLine("2. Advance System Clock");
                Console.WriteLine("3. Get Risk Time Range");
                Console.WriteLine("4. Set Risk Time Range");
                Console.WriteLine("5. Reset Database");
                Console.WriteLine("6. Initialize Database");
                Console.WriteLine("0. Back");
                Console.Write("Choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                try
                {
                    switch (choice)
                    {
                        case 1:
                            Console.WriteLine($"Current System Clock: {s_bl.Admin.GetSystemClock()}");
                            break;
                        case 2:
                            Console.Write("Enter time unit (Minute, Hour, Day, Month, Year): ");
                            if (Enum.TryParse(Console.ReadLine(), out BO.TimeUnit timeUnit))
                            {
                                s_bl.Admin.AdvanceSystemClock(timeUnit);
                                Console.WriteLine("System clock advanced.");
                            }
                            else
                            {
                                Console.WriteLine("Invalid time unit.");
                            }
                            break;
                        case 3:
                            Console.WriteLine($"Current Risk Time Range: {s_bl.Admin.GetRiskTimeRange()}");
                            break;
                        case 4:
                            Console.Write("Enter new risk time range (hh:mm:ss): ");
                            if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan timeRange))
                            {
                                s_bl.Admin.SetRiskTimeRange(timeRange);
                                Console.WriteLine("Risk time range updated.");
                            }
                            else
                            {
                                Console.WriteLine("Invalid time format.");
                            }
                            break;
                        case 5:
                            s_bl.Admin.ResetDatabase();
                            Console.WriteLine("Database reset.");
                            break;
                        case 6:
                            s_bl.Admin.InitializeDatabase();
                            Console.WriteLine("Database initialized.");
                            break;
                        case 0:
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                throw new BO.GenralInitializationExcption("A custom error occurred in AdminMenu.", ex);
            }
        }

        }

        static void VolunteerMenu()
        {
        try
        {
            while (true)
            {
                Console.WriteLine("\nVolunteer Management:");
                Console.WriteLine("1. Get Volunteer Details");
                Console.WriteLine("2. List Volunteers");
                Console.WriteLine("3. Add Volunteer");
                Console.WriteLine("4. Remove Volunteer");
                Console.WriteLine("0. Back");
                Console.Write("Choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                    throw new InvalidFormatException("The voluntee menu choice is not valid.");



                switch (choice)
                    {
                        case 1:
                            Console.Write("Enter Volunteer ID: ");
                            if (int.TryParse(Console.ReadLine(), out int volunteerId))
                            {
                                var volunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);
                                Console.WriteLine(volunteer);
                            }
                            else
                            {
                                Console.WriteLine("Invalid ID.");
                            }
                            break;
                        case 2:
                            foreach (var volunteer in s_bl.Volunteer.GetVolunteersList())
                                Console.WriteLine(volunteer);
                            break;
                        case 3:
                            Console.Write("Enter Full Name: ");
                            string fullName = Console.ReadLine()!;
                            Console.Write("Enter Phone Number: ");
                            string phoneNumber = Console.ReadLine()!;
                            Console.Write("Enter Email: ");
                            string email = Console.ReadLine()!;
                            Console.Write("Enter Role (Manager, Volunteer): ");
                            if (Enum.TryParse(Console.ReadLine(), out BO.Role role))
                            {
                                var volunteer = new BO.Volunteer
                                {
                                    FullName = fullName,
                                    PhoneNumber = phoneNumber,
                                    Email = email,
                                    role = role,
                                    IsActive = true
                                };
                                s_bl.Volunteer.AddVolunteer(volunteer);
                                Console.WriteLine("Volunteer added.");
                            }
                            break;
                        case 4:
                            Console.Write("Enter Volunteer ID: ");
                            if (int.TryParse(Console.ReadLine(), out int vId))
                            {
                                s_bl.Volunteer.DeleteVolunteer(vId);
                                Console.WriteLine("Volunteer removed.");
                            }
                            else
                            {
                                Console.WriteLine("Invalid ID.");
                            }
                            break;
                        case 0:
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Try again.");
                            break;
                    }
               
            }
             }
                catch (Exception ex)
        {
                throw new BO.GenralInitializationExcption("An error occurred in VolunteerMenu.", ex);
        }
        }
    /// <summary>
    /// Displays and manages the call menu.
    /// </summary>
    static void CallMenu()
    {
        try
        {

            while (true)
            {
                Console.WriteLine("\nCall Management:");
                Console.WriteLine("1. Get Call Details");
                Console.WriteLine("2. List Calls");
                Console.WriteLine("3. Add Call");
                Console.WriteLine("4. Remove Call");
                Console.WriteLine("0. Back");
                Console.Write("Choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                    throw new InvalidFormatException("The Call menu choice is not valid.");


                switch (choice)
                    {
                        case 1:
                            Console.Write("Enter Call ID: ");
                            if (int.TryParse(Console.ReadLine(), out int callId))
                            {
                                var call = s_bl.Call.GetCallDetails(callId);
                                Console.WriteLine(call);
                            }
                            else
                            {
                                Console.WriteLine("Invalid ID.");
                            }
                            break;
                        case 2:
                            foreach (var call in s_bl.Call.GetCalls(null, null, null))
                                Console.WriteLine(call);
                            break;
                        case 3:
                            Console.Write("Enter Call Type (Emergency, Assistance, etc.): ");
                            if (Enum.TryParse(Console.ReadLine(), out BO.CallType callType))
                            {
                                Console.Write("Enter Description: ");
                                string description = Console.ReadLine()!;
                                Console.Write("Enter Address: ");
                                string address = Console.ReadLine()!;
                                Console.Write("Enter Latitude: ");
                                if (double.TryParse(Console.ReadLine(), out double latitude))
                                {
                                    Console.Write("Enter Longitude: ");
                                    if (double.TryParse(Console.ReadLine(), out double longitude))
                                    {
                                        var call = new BO.Call
                                        {
                                            CallType = callType,
                                            Description = description,
                                            FullAddress = address,
                                            Latitude = latitude,
                                            Longitude = longitude,
                                            OpenTime = DateTime.Now,
                                            Status = BO.CallStatus.Open
                                        };
                                        s_bl.Call.AddCall(call);
                                        Console.WriteLine("Call added.");
                                    }
                                }
                            }
                            break;
                        case 4:
                            Console.Write("Enter Call ID: ");
                            if (int.TryParse(Console.ReadLine(), out int cId))
                            {
                                s_bl.Call.DeleteCall(cId);
                                Console.WriteLine("Call removed.");
                            }
                            else
                            {
                                Console.WriteLine("Invalid ID.");
                            }
                            break;
                        case 0:
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Try again.");
                            break;
                    }
                }
               
            }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
    }
