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
using System;
using BlApi;
using BO;
using DO;
///לבדוק מה עם כל הזריקות שיש פה
namespace BlTest
{
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
                Console.WriteLine("Please log in.");
                Console.Write("Username: ");
                string username = Console.ReadLine()!;

                Console.Write("Enter Password (must be at least 8 characters, contain upper and lower case letters, a digit, and a special character): ");
                string password = Console.ReadLine()!;

                BO.Role userRole = s_bl.Volunteer.Login(username, password);
                Console.WriteLine($"Login successful! Your role is: {userRole}");

                //If Manager do
                if (userRole == BO.Role.Manager)
                {
                    ShowMenu();
                }
                else
                {
                    Console.WriteLine("UpDate Volunteer");
                    UpDateVolunteer();
                }
            }
            catch (BO.BlDoesNotExistException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (BO.BlInvalidFormatException ex)
            {
                Console.WriteLine("The sub menu choice is not valid.", ex);
            }
            catch (BO.BlGeneralDatabaseException ex)
            {
                Console.WriteLine($"System Error: {ex.Message}");
            }
            catch(Exception ex) {
                Console.WriteLine($"Unknow Error: {ex.Message}");

            }

            }
        /// <summary>
        ///  Displays the menu for a manager.
        /// </summary>
        static void ShowMenu()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("\n--- BL Test System ---");
                    Console.WriteLine("1. Administration");
                    Console.WriteLine("2. Volunteers");
                    Console.WriteLine("3. Calls");
                    Console.WriteLine("0. Exit");
                    Console.Write("Choose an option: ");

                    if (int.TryParse(Console.ReadLine(), out int choice))
                    {
                        switch (choice)
                        {
                            case 1:
                                AdminMenu();
                                break;
                            case 2:
                                VolunteerMenu();
                                break;
                            case 3:
                                CallMenu();
                                break;
                            case 0:
                                return;
                            default:
                                Console.WriteLine("Invalid choice. Try again.");
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while displaying the menu: " + ex.Message);
            }
        }

        /// <summary>
        /// Displays the administration menu and handles user interactions.
        /// </summary>
        static void AdminMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- Administration ---");
                Console.WriteLine("1. Reset Database");
                Console.WriteLine("2. Initialize Database");
                Console.WriteLine("3. Advance Clock");
                Console.WriteLine("4. Show Clock");
                Console.WriteLine("5. Get Risk Time Range");
                Console.WriteLine("6. Set Risk Time Range");
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
                            s_bl.Admin.ResetDB();
                            Console.WriteLine("Database reset successfully");
                            break;
                        case 2:
                            s_bl.Admin.InitializeDB();
                            Console.WriteLine("Database initialized successfully");
                            break;
                        case 3:
                            Console.Write("Enter time unit (Minute, Hour, Day, Month, Year): ");
                            if (Enum.TryParse(Console.ReadLine(), true, out BO.TimeUnit timeUnit))
                            {
                                s_bl.Admin.AdvanceClock(timeUnit);
                                Console.WriteLine("System clock advanced.");
                            }
                            else
                            {
                                throw new FormatException("Invalid time unit. Please enter: Minute, Hour, Day, Month, Year.");
                            }
                            break;
                        case 4:
                            Console.WriteLine($"Current System Clock: {s_bl.Admin.GetClock()}");
                            break;
                        case 5:
                            Console.WriteLine($"Current Risk Time Range: {s_bl.Admin.GetMaxRange()}");
                            break;
                        case 6:
                            Console.Write("Enter new risk time range (hh:mm:ss): ");
                            if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan timeRange))
                            {
                                s_bl.Admin.SetMaxRange(timeRange);
                                Console.WriteLine("Risk time range updated.");
                            }
                            else
                            {
                                throw new FormatException("Invalid time format. Please use hh:mm:ss.");
                            }
                            break;
                        case 0:
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Try again.");
                            break;
                    }
                }
                catch (BO.BlInvalidFormatException)
                {
                    Console.WriteLine("Invalid time format.");
                }
                catch (BO.BlGeneralDatabaseException ex)
                {
                    Console.WriteLine($"A database error occurred: {ex.Message}");
                }
            }
        }
        /// <summary>
        /// Displays the volunteer management menu and handles user interactions.
        /// The menu allows listing, filtering, reading, adding, removing, and updating volunteers.
        /// </summary>
        /// <remarks>
        /// The function runs in a loop until the user chooses to exit.  
        /// It validates user input and handles exceptions that may occur during operations.
        /// </remarks>
        /// <exception cref="FormatException">
        /// Thrown when the user enters an invalid input that cannot be parsed correctly.
        /// </exception>
        static void VolunteerMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- Volunteer Management ---");
                Console.WriteLine("1. List Volunteers");
                Console.WriteLine("2. Get Filter/Sort volunteer");
                Console.WriteLine("3. Read Volunteer by ID");
                Console.WriteLine("4. Add Volunteer");
                Console.WriteLine("5. Remove Volunteer");
                Console.WriteLine("6. Update Volunteer");
                Console.WriteLine("0. Back");
                Console.Write("Choose an option: ");

                // Try parsing user input; if invalid, throw an exception
                if (!int.TryParse(Console.ReadLine(), out int choice))
                    throw new FormatException("The volunteer menu choice is not valid.");

                switch (choice)
                {
                    case 1:
                        try
                        {
                            // Retrieves and displays all volunteers
                            foreach (var volunteer in s_bl.Volunteer.GetVolunteersList())
                                Console.WriteLine(volunteer);
                        }
                        catch (BO.BlDoesNotExistException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        catch (BO.BlGeneralDatabaseException ex)
                        {
                            Console.WriteLine($"System Error: {ex.Message}");
                        }
                        break;

                    case 2:
                        try
                        {
                            // Filtering and sorting volunteers
                            bool? isActive;
                            BO.VolunteerSortField? sortBy;

                            // Calls a function to get filtering and sorting criteria
                            GetVolunteerFilterAndSortCriteria(out isActive, out sortBy);

                            var volunteersList = s_bl.Volunteer.GetVolunteersList(isActive, sortBy);
                            if (volunteersList != null)
                                foreach (var volunteer in volunteersList)
                                    Console.WriteLine(volunteer);
                            else
                                Console.WriteLine("No volunteers found matching the criteria.");
                        }
                        catch (BO.BlDoesNotExistException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        catch (BO.BlGeneralDatabaseException ex)
                        {
                            Console.WriteLine($"System Error: {ex.Message}");
                        }
                        break;

                    case 3:
                        try
                        {
                            Console.Write("Enter Volunteer ID: ");
                            if (int.TryParse(Console.ReadLine(), out int volunteerId))
                            {
                                // Fetch and display details of a specific volunteer
                                var volunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);
                                Console.WriteLine(volunteer);
                            }
                            else
                                throw new FormatException("Invalid input. Volunteer ID must be a number.");
                        }
                        catch (BO.BlDoesNotExistException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;

                    case 4:
                        try
                        {
                            Console.WriteLine("Enter Volunteer details:");
                            Console.Write("ID: ");
                            if (int.TryParse(Console.ReadLine(), out int id))
                            {
                                // Creates a new volunteer object and adds it to the system
                                BO.Volunteer volunteer = CreateVolunteer(id);
                                s_bl.Volunteer.AddVolunteer(volunteer);
                                Console.WriteLine("Volunteer created successfully!");
                            }
                            else
                                throw new FormatException("Invalid input. Volunteer ID must be a number.");
                        }
                        catch (BO.BlAlreadyExistsException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        catch (BO.BlInvalidFormatException ex)
                        {
                            Console.WriteLine($"Input Error: {ex.Message}");
                        }
                        catch (BO.BlApiRequestException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        catch (BO.BlGeolocationNotFoundException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        catch (BO.BlGeneralDatabaseException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;

                    case 5:
                        try
                        {
                            Console.Write("Enter Volunteer ID: ");
                            if (int.TryParse(Console.ReadLine(), out int vId))
                            {
                                // Deletes a volunteer by ID
                                s_bl.Volunteer.DeleteVolunteer(vId);
                                Console.WriteLine("Volunteer removed.");
                            }
                            else
                                throw new FormatException("Invalid input. Volunteer ID must be a number.");
                        }
                        catch (BO.BlDoesNotExistException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        catch (BO.BlGeneralDatabaseException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;

                    case 6:
                        // Calls a function to update volunteer information
                        UpDateVolunteer();
                        break;

                    case 0:
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }
        /// <summary>
        /// Gets filtering and sorting criteria for volunteers from user input.
        /// </summary>
        /// <param name="isActive">Filter by active status (true/false/null).</param>
        /// <param name="sortBy">Sorting field, defaults to ID if input is invalid.</param>
        /// <exception cref="FormatException">Thrown if sorting input is invalid.</exception>
        public static void GetVolunteerFilterAndSortCriteria(out bool? isActive, out BO.VolunteerSortField? sortBy)
        {
            isActive = null;
            sortBy = null;

            try
            {

                Console.WriteLine("Is the volunteer active? (yes/no or leave blank for null): ");
                string activeInput = Console.ReadLine();

                if (!string.IsNullOrEmpty(activeInput))
                {
                    if (activeInput.Equals("yes", StringComparison.OrdinalIgnoreCase))
                        isActive = true;
                    else if (activeInput.Equals("no", StringComparison.OrdinalIgnoreCase))
                        isActive = false;
                    else
                        Console.WriteLine("Invalid input for active status. Defaulting to null.");
                }

                Console.WriteLine("Choose how to sort the volunteers by: ");
                Console.WriteLine("1. ID");
                Console.WriteLine("2. Name");
                Console.WriteLine("3. Total Responses Handled");
                Console.WriteLine("4. Total Responses Cancelled");
                Console.WriteLine("5. Total Expired Responses");
                Console.WriteLine("6. Sum of Calls");
                Console.WriteLine("7. Sum of Cancellations");
                Console.WriteLine("8. Sum of Expired Calls");
                Console.WriteLine("Select sorting option by number: ");
                string sortInput = Console.ReadLine();

                if (int.TryParse(sortInput, out int sortOption))
                {
                    switch (sortOption)
                    {
                        case 1:
                            sortBy = BO.VolunteerSortField.Id;
                            break;
                        case 2:
                            sortBy = BO.VolunteerSortField.Name;
                            break;
                        case 3:
                            sortBy = BO.VolunteerSortField.TotalResponsesHandled;
                            break;
                        case 4:
                            sortBy = BO.VolunteerSortField.TotalResponsesCancelled;
                            break;
                        case 5:
                            sortBy = BO.VolunteerSortField.TotalExpiredResponses;
                            break;
                        case 6:
                            sortBy = BO.VolunteerSortField.SumOfCalls;
                            break;
                        case 7:
                            sortBy = BO.VolunteerSortField.SumOfCancellation;
                            break;
                        case 8:
                            sortBy = BO.VolunteerSortField.SumOfExpiredCalls;
                            break;
                        default:
                            Console.WriteLine("Invalid selection. Defaulting to sorting by ID.");
                            break;
                    }
                }
                else
                {
                    throw new FormatException("Invalid input for sorting option. Defaulting to sorting by ID.");
                }
            }
            catch (BO.BlGeneralDatabaseException ex)
            {
                Console.WriteLine($"Exception: {ex.GetType().Name}");
                Console.WriteLine($"Message: {ex.Message}");
            }
        }
/// <summary>
/// Creates a new volunteer with user-provided details.
/// </summary>
/// <param name="requesterId">The unique ID of the volunteer.</param>
/// <returns>A new <see cref="BO.Volunteer"/> object.</returns>
/// <exception cref="FormatException">
/// Thrown if any input is invalid (e.g., incorrect format for boolean, numeric, or enum values).
/// </exception>        
       static BO.Volunteer CreateVolunteer(int requesterId)
        {

            Console.Write("Name: ");
            string? name = Console.ReadLine();

            Console.Write("Phone Number: ");
            string? phoneNumber = Console.ReadLine();

            Console.Write("Email: ");
            string? email = Console.ReadLine();

            Console.Write("IsActive? (true/false): ");
            if (!bool.TryParse(Console.ReadLine(), out bool active))
                throw new FormatException("Invalid input for IsActive.");

            Console.WriteLine("Please enter Role: 'Manager' or 'Volunteer'.");
            if (!Enum.TryParse(Console.ReadLine(), out BO.Role role))
                throw new FormatException("Invalid role.");

            Console.Write("Password: ");
            string? password = Console.ReadLine();

            Console.Write("Address: ");
            string? address = Console.ReadLine();

            Console.WriteLine("Enter location details:");
            Console.Write("Latitude: ");
            if (!double.TryParse(Console.ReadLine(), out double latitude))
                throw new FormatException("Invalid latitude format.");

            Console.Write("Longitude: ");
            if (!double.TryParse(Console.ReadLine(), out double longitude))
                throw new FormatException("Invalid longitude format.");

            Console.Write("Largest Distance: ");
            if (!double.TryParse(Console.ReadLine(), out double largestDistance))
                throw new FormatException("Invalid largest distance format.");

            Console.Write("Distance Type (Air, Drive or Walk): ");
            if (!Enum.TryParse(Console.ReadLine(), true, out BO.DistanceType myDistanceType))
                throw new FormatException("Invalid distance type.");

            return new BO.Volunteer
            {
                Id = requesterId,
                Name = name,
                Phone = phoneNumber,
                Email = email,
                Active = active,
                MyRole = role,
                Password = password,
                Address = address,
                Latitude = latitude,
                Longitude = longitude,
                LargestDistance = largestDistance,
                MyDistanceType = myDistanceType,
                TotalCallsHandled = 0,
                TotalCallsCancelled = 0,
                TotalExpiredCallsChosen = 0,
                CurrentCallInProgress = null
            };

        }
        /// <summary>
        /// Updates an existing volunteer with new details provided by the user.
        /// </summary>
        /// <exception cref="FormatException">Thrown if the volunteer ID is not a valid number.</exception>
        /// <exception cref="BO.BlDoesNotExistException">Thrown if the volunteer does not exist.</exception>
        /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the user lacks permissions.</exception>
        /// <exception cref="BO.BlInvalidFormatException">Thrown if the provided data is in an invalid format.</exception>
        /// <exception cref="BO.BlGeneralDatabaseException">Thrown if a database error occurs.</exception>
        /// <exception cref="Exception">Handles unexpected errors.</exception>
        static void UpDateVolunteer()
        {

            //מה עושים עם כל אלה בעדכון?
            //TotalCallsHandled = 0,
            //     TotalCallsCancelled = 0,
            //     TotalExpiredCallsChosen = 0,
            //צריך פשוט לא לעדכן אותם
            try
            {
                Console.Write("Enter requester ID: ");
                if (int.TryParse(Console.ReadLine(), out int requesterId))
                {
                    BO.Volunteer boVolunteer = CreateVolunteer(requesterId);
                    s_bl.Volunteer.UpdateVolunteer(requesterId, boVolunteer);
                    Console.WriteLine("Volunteer updated successfully.");
                }
                else
                    throw new FormatException("Invalid input. Volunteer ID must be a number.");

            }
            catch (BO.BlDoesNotExistException ex)
            {
                Console.WriteLine(ex);
            }
            catch (BO.BlUnauthorizedAccessException ex)
            {
                Console.WriteLine(ex);
            }
            catch (BO.BlInvalidFormatException ex)
            {
                Console.WriteLine(ex);
            }
            catch (BO.BlGeneralDatabaseException ex)
            {
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred: " + ex.Message);
            }
        }


        static void CallMenu()
        {
            try
            {

                while (true)
                {
                    Console.WriteLine("\n--- Call Management ---");
                    Console.WriteLine("1. Get call quantities by status");
                    Console.WriteLine("2. Get Closed Calls Handled By Volunteer");
                    Console.WriteLine("3. Show All Callsl");
                    Console.WriteLine("4. Read Call by ID");
                    Console.WriteLine("5. Add Call");
                    Console.WriteLine("6. Remove Call");
                    Console.WriteLine("7. Update Call");
                    Console.WriteLine("8. Get Open Calls For Volunteer");
                    Console.WriteLine("9. Mark Call As Canceled");
                    Console.WriteLine("10. Mark Call As Completed");
                    Console.WriteLine("11. Select Call For Treatment");
                    Console.WriteLine("0. Back");
                    Console.Write("Choose an option: ");

                    if (!int.TryParse(Console.ReadLine(), out int choice))
                        throw new FormatException("The call menu choice is not valid.");

                    switch (choice)
                    {
                        case 1:
                            try
                            {
                                int[] callQuantities = s_bl.Call.GetCallQuantitiesByStatus();
                                Console.WriteLine("Call quantities by status:");

                                foreach (BO.Status status in Enum.GetValues(typeof(BO.Status)))
                                {
                                    Console.WriteLine($"{status}: {callQuantities[(int)status]}");
                                }
                            }
                            catch (BO.BlGeneralDatabaseException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                                if (ex.InnerException != null)
                                {
                                    Console.WriteLine($"Internal error: {ex.InnerException.Message}");
                                }
                            }
                            break;
                        case 2:
                            try
                            {
                                Console.Write("Enter Volunteer ID: ");
                                if (int.TryParse(Console.ReadLine(), out int volunteerId))
                                {
                                    Console.WriteLine("Enter Call Type filter (or press Enter to skip):");
                                    string? callTypeInput = Console.ReadLine();
                                    BO.CallType? callTypeFilter = Enum.TryParse(callTypeInput, out BO.CallType parsedCallType) ? parsedCallType : null;

                                    Console.WriteLine("Enter Sort Field (or press Enter to skip):");
                                    string? sortFieldInput = Console.ReadLine();
                                    BO.ClosedCallInListFields? sortField = Enum.TryParse(sortFieldInput, out BO.ClosedCallInListFields parsedSortField) ? parsedSortField : null;

                                    var closedCalls = s_bl.Call.GetClosedCallsByVolunteer(volunteerId, callTypeFilter, sortField);

                                    Console.WriteLine("\nClosed Calls Handled By Volunteer:");
                                    foreach (var call in closedCalls)
                                    {
                                        Console.WriteLine(call);
                                    }
                                }
                                else
                                {
                                    throw new BO.BlInvalidFormatException("Invalid input. Volunteer ID must be a number.");
                                }
                            }
                            catch (BO.BlGeneralDatabaseException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                                if (ex.InnerException != null)
                                {
                                    Console.WriteLine($"Internal error: {ex.InnerException.Message}");
                                }
                            }
                            break;
                        case 3:
                            try
                            {
                                Console.WriteLine("Enter sort field (CallId, CallType, OpenTime, TimeRemainingToCall, LastVolunteer, CompletionTime, MyStatus, TotalAllocations) or press Enter to skip:");
                                string? filterFieldInput = Console.ReadLine();
                                BO.CallInListFields? filterField = Enum.TryParse(filterFieldInput, out BO.CallInListFields parsedFilterField) ? parsedFilterField : null;

                                object? filterValue = null;
                                if (filterField.HasValue)
                                {
                                    Console.WriteLine("Enter filter value:");
                                    filterValue = Console.ReadLine();
                                }

                                Console.WriteLine("Enter sort field (CallId, CallType, OpenTime, TimeRemainingToCall, LastVolunteer, CompletionTime, MyStatus, TotalAllocations) or press Enter to skip:");
                                string? sortFieldInput = Console.ReadLine();
                                BO.CallInListFields? sortField = Enum.TryParse(sortFieldInput, out BO.CallInListFields parsedSortField) ? parsedSortField : null;

                                var callList = s_bl.Call.GetCallList(filterField, filterValue, sortField);

                                foreach (var call in callList)
                                    Console.WriteLine(call);
                            }
                            catch (BO.BlGeneralDatabaseException ex)
                            {
                                Console.WriteLine($"Exception: {ex.GetType().Name} - {ex.Message}");
                                if (ex.InnerException != null)
                                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                            }
                            break;
                        case 4:
                            try
                            {
                                Console.Write("Enter Call ID: ");
                                if (int.TryParse(Console.ReadLine(), out int callId))
                                {
                                    var call = s_bl.Call.GetCallDetails(callId);
                                    Console.WriteLine(call);
                                }
                                else
                                {
                                    throw new FormatException("Invalid input. Volunteer ID must be a number.");
                                }
                            }
                            catch (BO.BlDoesNotExistException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            catch (BO.BlGeneralDatabaseException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            break;
                        case 5:
                            try
                            {
                                Console.WriteLine("Enter Call details:");
                                Console.Write("ID: ");
                                if (int.TryParse(Console.ReadLine(), out int id))
                                {
                                    BO.Call call = CreateCall(id);
                                    s_bl.Call.AddCall(call);
                                    Console.WriteLine("Call created successfully!");
                                }
                                else
                                    throw new FormatException("Invalid input. Cll ID must be a number.");
                            }
                            catch (BO.BlDoesNotExistException ex)
                            {
                                Console.WriteLine($"Exception: {ex.GetType().Name}, Message: {ex.Message}");
                            }
                            catch (BO.BlInvalidOperationException ex)
                            {
                                Console.WriteLine($"Exception: {ex.GetType().Name}, Message: {ex.Message}");
                            }
                            catch (BO.BlInvalidFormatException ex)
                            {
                                Console.WriteLine($"Exception: {ex.GetType().Name}, Message: {ex.Message}");
                            }
                            catch (BO.BlAlreadyExistsException ex)
                            {
                                Console.WriteLine($"Exception: {ex.GetType().Name}, Message: {ex.Message}");
                            }
                            catch (BO.BlGeneralDatabaseException ex)
                            {
                                Console.WriteLine($"Exception: {ex.GetType().Name}, Message: {ex.Message}");
                                if (ex.InnerException != null)
                                {
                                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                            }
                            ;
                            break;
                        case 6:
                            try
                            {
                                Console.Write("Enter Call ID: ");
                                if (int.TryParse(Console.ReadLine(), out int cId))
                                {
                                    s_bl.Call.DeleteCall(cId);
                                    Console.WriteLine("Call removed.");
                                }
                                else
                                {
                                    throw new FormatException("Invalid input. Volunteer ID must be a number.");
                                }
                            }
                            catch (BlDoesNotExistException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            catch (BlDeletionException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            catch (BlGeneralDatabaseException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            break;
                        case 7:
                            UpDateCall();
                            break;
                        case 8:
                            try
                            {
                                Console.Write("Enter Volunteer ID: ");
                                if (int.TryParse(Console.ReadLine(), out int volunteerId))
                                {
                                    Console.WriteLine("Enter Call Type filter (or press Enter to skip):");
                                    string? callTypeInput = Console.ReadLine();
                                    BO.CallType? callTypeFilter = Enum.TryParse(callTypeInput, out BO.CallType parsedCallType) ? parsedCallType : null;

                                    Console.WriteLine("Enter Sort Field (or press Enter to skip):");
                                    string? sortFieldInput = Console.ReadLine();
                                    BO.OpenCallInListFields? sortField = Enum.TryParse(sortFieldInput, out BO.OpenCallInListFields parsedSortField) ? parsedSortField : null;

                                    var openCalls = s_bl.Call.GetOpenCallsForVolunteer(volunteerId, callTypeFilter, sortField);

                                    Console.WriteLine("\nOpen Calls Available for Volunteer:");
                                    foreach (var call in openCalls)
                                    {
                                        Console.WriteLine(call);
                                    }
                                }
                                else
                                {
                                    throw new BlInvalidFormatException("Invalid input. Volunteer ID must be a number.");
                                }
                            }
                            catch (BO.BlDoesNotExistException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            catch (BO.BlGeneralDatabaseException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                                if (ex.InnerException != null)
                                {
                                    Console.WriteLine($"Internal error: {ex.InnerException.Message}");
                                }
                            }
                            break;
                        case 9:
                            try
                            {
                                Console.Write("Enter Volunteer ID: ");
                                if (!int.TryParse(Console.ReadLine(), out int volunteerId))
                                    throw new BlInvalidFormatException("Invalid input. Volunteer ID must be a number.");

                                Console.Write("Enter call ID: ");
                                if (!int.TryParse(Console.ReadLine(), out int assignmentId))
                                    throw new BlInvalidFormatException("Invalid input. call ID must be a number.");

                                s_bl.Call.UpdateCallCancellation(volunteerId, assignmentId);
                                Console.WriteLine("The call was successfully canceled.");
                            }
                            catch (BO.BlUnauthorizedAccessException ex)
                            {
                                Console.WriteLine($"Authorization Error: {ex.Message}");
                            }
                            catch (BO.BlInvalidOperationException ex)
                            {
                                Console.WriteLine($"Operation Error: {ex.Message}");
                            }
                            catch (BO.BlGeneralDatabaseException ex)
                            {
                                Console.WriteLine($"Database Error: {ex.Message}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Unexpected error: {ex.Message}");
                            }
                            break;
                        case 10:
                            try
                            {
                                Console.Write("Enter Volunteer ID: ");
                                string? volunteerInput = Console.ReadLine();
                                if (!int.TryParse(volunteerInput, out int volunteerId))
                                {
                                    throw new FormatException("Invalid input. Volunteer ID must be a number.");
                                }

                                Console.Write("Enter Assignment ID: ");
                                string? assignmentInput = Console.ReadLine();
                                if (!int.TryParse(assignmentInput, out int assignmentId))
                                {
                                    throw new FormatException("Invalid input. Assignment ID must be a number.");
                                }

                                s_bl.Call.UpdateCallCompletion(volunteerId, assignmentId);

                                Console.WriteLine("Call completion updated successfully!");
                            }
                            catch (BO.BlDoesNotExistException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            catch (BO.BlUnauthorizedAccessException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            catch (BO.BlInvalidOperationException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            catch (BO.BlGeneralDatabaseException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            break;
                        case 11:
                            try
                            {
                                Console.Write("Enter Volunteer ID: ");
                                if (!int.TryParse(Console.ReadLine(), out int volunteerId))
                                    throw new FormatException("Invalid input. Volunteer ID must be a number.");

                                Console.Write("Enter Call ID: ");
                                if (!int.TryParse(Console.ReadLine(), out int callId))
                                    throw new FormatException("Invalid input. Call ID must be a number.");

                                s_bl.Call.SelectCallForTreatment(volunteerId, callId);
                                Console.WriteLine("The call has been successfully assigned to the volunteer.");
                            }
                            catch (FormatException ex)
                            {
                                Console.WriteLine($"Input Error: {ex.Message}");
                            }
                            catch (BO.BlInvalidOperationException ex)
                            {
                                Console.WriteLine($"Operation Error: {ex.Message}");
                            }
                            catch (BO.BlGeneralDatabaseException ex)
                            {
                                Console.WriteLine($"Database Error: {ex.Message}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Unexpected error: {ex.Message}");
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
        static BO.Call CreateCall(int id)
        {
            Console.WriteLine("Enter the call type (0 for None, 1 for MusicPerformance, 2 for MusicTherapy, 3 for SingingAndEmotionalSupport, 4 for GroupActivities, 5 for PersonalizedMusicCare):");
            if (!Enum.TryParse(Console.ReadLine(), out BO.CallType callType))
            {
                throw new FormatException("Invalid call type.");
            }

            Console.WriteLine("Enter the verbal description:");
            string verbalDescription = Console.ReadLine();

            Console.WriteLine("Enter the address:");
            string address = Console.ReadLine();

            //Console.WriteLine("Enter the latitude:");
            //if (!double.TryParse(Console.ReadLine(), out double latitude))
            //{
            //    throw new FormatException("Invalid latitude value.");
            //}

            //Console.WriteLine("Enter the longitude:");
            //if (!double.TryParse(Console.ReadLine(), out double longitude))
            //{
            //    throw new FormatException("Invalid longitude value.");
            //}

            Console.WriteLine("Enter the max finish time (yyyy-mm-dd) or leave empty:");
            string maxFinishTimeInput = Console.ReadLine();
            DateTime? maxFinishTime = string.IsNullOrEmpty(maxFinishTimeInput) ? null : DateTime.Parse(maxFinishTimeInput);

            Console.WriteLine("Enter the status (0 for InProgress, 1 for AtRisk, 2 for InProgressAtRisk, 3 for Opened, 4 for Closed, 5 for Expired):");
            if (!Enum.TryParse(Console.ReadLine(), out Status status))
            {
                throw new FormatException("Invalid status.");
            }

            return new BO.Call
            {
                Id = id,
                MyCallType = callType,
                VerbalDescription = verbalDescription,
                Address = address,
                Latitude = 0,
                Longitude = 0,
                //האם זה הזמן הנוכחי?
                OpenTime = DateTime.Now,
            };


        }
        //אני באמצע הפונ
        //למה היא לא מקבלת ID?
        static void UpDateCall()
        {
            Console.Write("Enter Call ID: ");
            int.TryParse(Console.ReadLine(), out int callId);
            Console.Write("Enter New Description (optional) : ");
            string description = Console.ReadLine();
            Console.Write("Enter New Full Address (optional) : ");
            string address = Console.ReadLine();
            Console.Write("Enter Call Type (optional) : ");
            BO.CallType? callType = Enum.TryParse(Console.ReadLine(), out BO.CallType parsedType) ? parsedType : (BO.CallType?)null;
            Console.Write("Enter Max Finish Time (hh:mm , (optional)): ");
            TimeSpan? maxFinishTime = TimeSpan.TryParse(Console.ReadLine(), out TimeSpan parsedTime) ? parsedTime : (TimeSpan?)null;
            try
            {
                var callToUpdate = s_bl.Call.GetCallDetails(callId);
                if (callToUpdate == null)
                    throw new BO.BlDoesNotExistException($"Call with ID{callId} does not exist!");
                var newUpdatedCall = new BO.Call
                {
                    Id = callId,
                    VerbalDescription = !string.IsNullOrWhiteSpace(description) ? description : callToUpdate.VerbalDescription,
                    Address = !string.IsNullOrWhiteSpace(address) ? address : /*callToUpdate. FullAddress*/"No Address",
                    OpenTime = callToUpdate.OpenTime,
                    MaxFinishTime = (maxFinishTime.HasValue ? DateTime.Now.Date + maxFinishTime.Value : callToUpdate.MaxFinishTime),
                    MyCallType = callType ?? callToUpdate.MyCallType
                };
                s_bl.Call.UpdateCallDetails(newUpdatedCall);
                Console.WriteLine("Call updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType().Name}, Message: {ex.Message}");
            }
            //try
            //{
            //    Console.Write("Enter requester ID: ");
            //    if (int.TryParse(Console.ReadLine(), out int requesterId))
            //    {
            //        //BO.Call boCall = CreateCall(requesterId);
            //        //s_bl.Call.UpdateCallDetails(requesterId, boCall);
            //        Console.WriteLine("Volunteer updated successfully.");
            //    }
            //    else
            //        throw new FormatException("Invalid input. Volunteer ID must be a number.");
            //}
            //catch { }
            //}
        }
    }
}

