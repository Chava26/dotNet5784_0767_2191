
using System;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Xml.Linq;
using BlApi;
using BO;
using DalApi;
using DO;
using static System.Net.Mime.MediaTypeNames;

namespace BlTest
{
    /// <summary>
    /// Main class for managing the application menus and execution.
    /// </summary>
    class Program
    {
        static readonly IBl s_bl = BlApi.Factory.Get();
        /// <summary>
        /// Main entry point of the application. Displays the main menu and routes user choices.
        /// </summary>
        static void Main()
        {
            try
            {

                while (true)
                {
                    Console.WriteLine("\n--- BL Test System ---");
                    Console.WriteLine("0. Exit");
                    Console.WriteLine("1. Volunteers");
                    Console.WriteLine("2. Calls");
                    Console.WriteLine("3. Administration");
                    Console.Write("Choose an option: ");

                    if (int.TryParse(Console.ReadLine(), out int choice))
                    {
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
                            s_bl.Admin.ResetDatabase();
                            Console.WriteLine("Database reset successfully");
                            break;
                        case 2:
                            s_bl.Admin.InitializeDatabase();
                            Console.WriteLine("Database initialized successfully");
                            break;
                        case 3:
                            Console.Write("Enter time unit (Minute, Hour, Day, Month, Year): ");
                            if (Enum.TryParse(Console.ReadLine(), true, out BO.TimeUnit timeUnit))
                            {
                                s_bl.Admin.AdvanceSystemClock(timeUnit);
                                Console.WriteLine("System clock advanced.");
                            }
                            else
                            {
                                throw new FormatException("Invalid time unit. Please enter: Minute, Hour, Day, Month, Year.");
                            }
                            break;
                        case 4:
                            Console.WriteLine($"Current System Clock: {s_bl.Admin.GetSystemClock()}");
                            break;
                        case 5:
                            Console.WriteLine($"Current Risk Time Range: {s_bl.Admin.GetRiskTimeRange()}");
                            break;
                        case 6:
                            Console.Write("Enter new risk time range (hh:mm:ss): ");
                            if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan timeRange))
                            {
                                s_bl.Admin.SetRiskTimeRange(timeRange);
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
                Console.WriteLine("1. Login");

                Console.WriteLine("2. List Volunteers");
                Console.WriteLine("3. Get Filter/Sort volunteer");
                Console.WriteLine("4. Read Volunteer by ID");
                Console.WriteLine("5. Add Volunteer");
                Console.WriteLine("6. Remove Volunteer");
                Console.WriteLine("7. Update Volunteer");
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
                            Console.WriteLine("Please log in.");
                            Console.Write("Username: ");
                            string username = Console.ReadLine()!;

                            Console.Write("Enter Password (must be at least 8 characters, contain upper and lower case letters, a digit, and a special character): ");
                            string password = Console.ReadLine()!;

                            BO.Role userRole = s_bl.Volunteer.Login(username, password);
                            Console.WriteLine($"Login successful! Your role is: {userRole}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");

                        }
                        break;
                    case 2:
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

                    case 3:
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

                    case 4:
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

                    case 5:
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
                        catch (BO.GeolocationNotFoundException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        catch (BO.BlGeneralDatabaseException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;

                    case 6:
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

                    case 7:
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
                string activeInput = Console.ReadLine()!;

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
                string sortInput = Console.ReadLine()!;

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
                            sortBy = BO.VolunteerSortField.TotalHandledCalls;
                            break;
                        case 4:
                            sortBy = BO.VolunteerSortField.TotalCanceledCalls;
                            break;
                        case 5:
                            sortBy = BO.VolunteerSortField.TotalExpiredCalls;
                            break;
                        case 6:
                            //sortBy = BO.VolunteerSortField.SumOfCalls;
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
            string? name = Console.ReadLine()!;

            Console.Write("Phone Number: ");
            string? phoneNumber = Console.ReadLine()!;

            Console.Write("Email: ");
            string? email = Console.ReadLine()!;

            Console.Write("IsActive? (true/false): ");
            if (!bool.TryParse(Console.ReadLine(), out bool active))
                throw new FormatException("Invalid input for IsActive.");

            Console.WriteLine("Please enter Role: 'Manager' or 'Volunteer'.");
            if (!Enum.TryParse(Console.ReadLine(), out BO.Role role))
                throw new FormatException("Invalid role.");

            Console.Write("Password: ");
            string? password = Console.ReadLine()!;

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
            if (!double.TryParse(Console.ReadLine(), out double maximumDistance))
                throw new FormatException("Invalid largest distance format.");

            Console.Write("Distance Type (Air, Drive or Walk): ");
            if (!Enum.TryParse(Console.ReadLine(), true, out BO.DistanceType myDistanceType))
                throw new FormatException("Invalid distance type.");

            return new BO.Volunteer
            {

                Id = requesterId,
                FullName= name,
                Email = email,
                PhoneNumber = phoneNumber,
                role = role,
                IsActive = active,
                MaxDistanceForTask = maximumDistance,
                Password = password,
                Address = address,
                Longitude = longitude,
                Latitude = latitude,
                DistanceType = myDistanceType,
                callInProgress = null,
                TotalHandledCalls= 0,
                TotalCanceledCalls =0,
                TotalExpiredCalls =0
            };

        }
        /// <summary>
        /// Updates an existing volunteer with new details provided by the user.
        /// </summary>
        /// <exception cref="FormatException">Thrown if the volunteer ID is not a valid number.</exception>
        /// <exception cref="BO.BlDoesNotExistException">Thrown if the volunteer does not exist.</exception>
        /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the user lacks permissions.</exception>
        /// <exception cref="BO.BlInvalidFormatException">Thrown if the provided data is in an invalid format.</exception>
        /// <exception cref="BO.BlBlGeneralDatabaseException">Thrown if a database error occurs.</exception>
        /// <exception cref="Exception">Handles unexpected errors.</exception>
        static void UpDateVolunteer()
        {
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
                                int[] callQuantities = (int[])s_bl.Call.GetCallQuantitiesByStatus();
                                Console.WriteLine("Call quantities by status:");

                                foreach (BO.CallStatus status in Enum.GetValues(typeof(BO.CallStatus)))
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
                                    BO.CallField? sortField = Enum.TryParse(sortFieldInput, out BO.CallField parsedSortField) ? parsedSortField : null;

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
                                BO.CallField? filterField = Enum.TryParse(filterFieldInput, out BO.CallField parsedFilterField) ? parsedFilterField : null;

                                object? filterValue = null;
                                if (filterField.HasValue)
                                {
                                    Console.WriteLine("Enter filter value:");
                                    filterValue = Console.ReadLine();
                                }

                                Console.WriteLine("Enter sort field (CallId, CallType, OpenTime, TimeRemainingToCall, LastVolunteer, CompletionTime, MyStatus, TotalAllocations) or press Enter to skip:");
                                string? sortFieldInput = Console.ReadLine();
                                BO.CallField? sortField = Enum.TryParse(sortFieldInput, out BO.CallField parsedSortField) ? parsedSortField : null;

                                var callList = s_bl.Call.GetCalls(filterField, filterValue, sortField);

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
                                    CallField? sortField = Enum.TryParse(sortFieldInput, out CallField parsedSortField) ? parsedSortField : null;

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

                                s_bl.Call.CancelAssignment(volunteerId, assignmentId);
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

                                s_bl.Call.CompleteCall(volunteerId, assignmentId);

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
            Console.WriteLine("Enter the car issue type:");
            Console.WriteLine("0 - None");
            Console.WriteLine("1 - Flat Tire");
            Console.WriteLine("2 - Dead Battery");
            Console.WriteLine("3 - Engine Failure");
            Console.WriteLine("4 - Overheating");
            Console.WriteLine("5 - Brake Failure");
            Console.WriteLine("6 - Transmission Issue");
            Console.WriteLine("7 - Alternator Failure");
            Console.WriteLine("8 - Starter Motor Failure");
            Console.WriteLine("9 - Oil Leak");
            Console.WriteLine("10 - Coolant Leak");
            Console.WriteLine("11 - Fuel Pump Failure");
            Console.WriteLine("12 - Clogged Fuel Filter");
            Console.WriteLine("13 - Exhaust Leak");
            Console.WriteLine("14 - Suspension Problem");
            Console.WriteLine("15 - Power Steering Failure");
            Console.WriteLine("16 - Timing Belt Failure");
            Console.WriteLine("17 - Battery Corrosion");
            Console.WriteLine("18 - Check Engine Light");
            Console.WriteLine("19 - Air Conditioner Failure");
            Console.WriteLine("20 - Worn Brake Pads");
            Console.WriteLine("21 - Spark Plug Issue");
            Console.WriteLine("22 - Blown Fuse");
            Console.WriteLine("23 - Headlight Failure");
            Console.WriteLine("24 - Sensor Malfunction");
            if (!Enum.TryParse(Console.ReadLine(), out BO.CallType callType))
            {
                throw new FormatException("Invalid call type.");
            }

            Console.WriteLine("Enter the verbal description:");
            string verbalDescription = Console.ReadLine()!;

            Console.WriteLine("Enter the address:");
            string address = Console.ReadLine()!;

            Console.WriteLine("Enter the max finish time (yyyy-mm-dd) or leave empty:");
            string maxFinishTimeInput = Console.ReadLine()!;
            DateTime? maxFinishTime = string.IsNullOrEmpty(maxFinishTimeInput) ? null : DateTime.Parse(maxFinishTimeInput);

            Console.WriteLine("Enter the status (0 for InProgress, 1 for AtRisk, 2 for InProgressAtRisk, 3 for Opened, 4 for Closed, 5 for Expired):");
            if (!Enum.TryParse(Console.ReadLine(), out CallStatus status))
            {
                throw new FormatException("Invalid status.");
            }

            return new BO.Call
            {
                Id = id,
                CallType = callType,
                Description = verbalDescription,
                FullAddress = address,
                Latitude = 0,
                Longitude = 0,
                OpenTime = DateTime.Now,
                Status=status,
                MaxEndTime= maxFinishTime,
            };


        }
        static void UpDateCall()
        {
            Console.Write("Enter Call ID: ");
            int.TryParse(Console.ReadLine(), out int callId);
            Console.Write("Enter New Description (optional) : ");
            string description = Console.ReadLine()!;
            Console.Write("Enter New Full Address (optional) : ");
            string address = Console.ReadLine()!;
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
                    Description = !string.IsNullOrWhiteSpace(description) ? description : callToUpdate.Description,
                    FullAddress = !string.IsNullOrWhiteSpace(address) ? address : /*callToUpdate. FullAddress*/"No Address",
                    OpenTime = callToUpdate.OpenTime,
                    MaxEndTime = (maxFinishTime.HasValue ? DateTime.Now.Date + maxFinishTime.Value : callToUpdate.MaxEndTime),
                    CallType = callType ?? callToUpdate.CallType
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

