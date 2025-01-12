using BO;
using DalApi;
using Newtonsoft.Json.Linq;
namespace Helpers
{
    internal class VolunteerManager
    {
        private static IDal s_dal = Factory.Get; //stage 4

        /// <summary>
        /// Creates a DO.Volunteer object from a BO.Volunteer object.
        /// </summary>
        /// <param name="boVolunteer">The BO.Volunteer object.</param>
        /// <returns>A DO.Volunteer object.</returns>
        internal static DO.Volunteer CreateDoVolunteer(BO.Volunteer boVolunteer)
        {
            return new DO.Volunteer(
                boVolunteer.Id,
                boVolunteer.FullName,
                boVolunteer.Email,
                boVolunteer.PhoneNumber,
                boVolunteer.Role,
                boVolunteer.IsActive,
                boVolunteer.MaxDistanceForTask,
                boVolunteer.Password,
                boVolunteer.Address,
                boVolunteer.Longitude,
                boVolunteer.Latitude
            );
        }


        /// <summary>
        /// Validates the basic format of the input values.
        /// </summary>
        /// <param name="requesterId">The ID of the requester.</param>
        /// <param name="boVolunteer">The BO.Volunteer object to validate.</param>
        /// <exception cref="ArgumentException">Thrown when any input format is invalid.</exception>
        internal static void ValidateInputFormat(BO.Volunteer boVolunteer)
        {
           

            if (boVolunteer == null)
                throw new ArgumentException("Volunteer object cannot be null.");

            if (!Tools.IsValidEmail(boVolunteer.Email))
                throw new ArgumentException("Invalid email format.");

            if (!Tools.IsValidId(boVolunteer.Id))
                throw new ArgumentException("Invalid ID format.");

            if (!Tools.IsValidPhoneNumber(boVolunteer.PhoneNumber))
                throw new ArgumentException("Invalid phone number format.");

            if (boVolunteer.FullName.Length < 2)
                throw new ArgumentException("Volunteer name is too short.");

            if (boVolunteer.Password.Length < 6)
                throw new ArgumentException("Password is too short.");
        }
        ///// <summary>
        ///// Validates logical constraints of the volunteer object.
        ///// </summary>
        ///// <param name="boVolunteer">The BO.Volunteer object to validate.</param>
        ///// <exception cref="ArgumentException">Thrown when any logical constraint is violated.</exception>
        //public void ValidateLogicalConstraints(BO.Volunteer boVolunteer)
        //{
        //    var coordinates = GetCoordinatesFromAddress(boVolunteer.Address);
        //    if (coordinates == null)
        //        throw new ArgumentException("Address is invalid or cannot be resolved to coordinates.");

        //    boVolunteer.Longitude = coordinates.Longitude;
        //    boVolunteer.Latitude = coordinates.Latitude;
        //}
        public static logicalChecking(int requesterId, BO.Volunteer boVolunteer)
        {
            bool isSelf = requesterId == boVolunteer.Id;
            bool isAdmin = boVolunteer.Role == Role.Manager;

            if (!isAdmin && !isSelf)
                throw new UnauthorizedAccessException("Only an admin or the volunteer themselves can perform this update.");

            if (!isAdmin && boVolunteer.Role != Role.Volunteer)
                throw new UnauthorizedAccessException("Only an admin can update the volunteer's role.");
            IsPasswordStrong(boVolunteer.Password);

        }
        /// <summary>
        /// Validates the requester's permissions to update the volunteer.
        /// </summary>
        /// <param name="requesterId">The ID of the requester.</param>
        /// <param name="boVolunteer">The BO.Volunteer object being updated.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown when the requester lacks permissions.</exception>
        public static void ValidatePermissions(int requesterId, BO.Volunteer boVolunteer)
        {
            bool isSelf = requesterId == boVolunteer.Id;
            bool isAdmin = boVolunteer.Role == Role.Manager;

            if (!isAdmin && !isSelf)
                throw new UnauthorizedAccessException("Only an admin or the volunteer themselves can perform this update.");

            if (!isAdmin && boVolunteer.Role != Role.Volunteer)
                throw new UnauthorizedAccessException("Only an admin can update the volunteer's role.");
        }
        /// <summary>
        /// Validates the strength of a password.
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <returns>True if the password is strong, otherwise false.</returns>
        public static bool IsPasswordStrong(string password)
        {
            // Ensure the password has a minimum length, uppercase letters, lowercase letters, numbers, and symbols
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit) &&
                   password.Any(ch => !char.IsLetterOrDigit(ch));
        }
        /// <summary>
        /// Encrypts a plain text password using a hashing algorithm.
        /// </summary>
        /// <param name="password">The plain text password.</param>
        /// <returns>The hashed password.</returns>
        private static string EncryptPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// Verifies if a plain text password matches the encrypted password.
        /// </summary>
        /// <param name="plainPassword">The plain text password to check.</param>
        /// <param name="encryptedPassword">The hashed password stored in the system.</param>
        /// <returns>True if the passwords match, otherwise false.</returns>
        public bool VerifyPassword(string plainPassword, string encryptedPassword)
        {
            var encryptedAttempt = EncryptPassword(plainPassword);
            return encryptedAttempt == encryptedPassword;
        }


        

            private static readonly string apiKey = "PK.83B935C225DF7E2F9B1ee90A6B46AD86";  
            private static readonly string apiUrl = "https://us1.locationiq.com/v1/search.php?key={0}&q={1}&format=json";

            /// <summary>
            /// Retrieves coordinates (latitude and longitude) for a given address.
            /// If the address is invalid or the API request fails, an appropriate exception is thrown.
            /// </summary>
            /// <param name="address">The address for which coordinates are requested.</param>
            /// <returns>A tuple containing latitude and longitude of the address.</returns>
            /// <exception cref="InvalidAddressException">Thrown when the address is invalid or cannot be processed.</exception>
            /// <exception cref="ApiRequestException">Thrown when the API request fails.</exception>
            /// <exception cref="GeolocationNotFoundException">Thrown when no geolocation is found for the address.</exception>
            public static (double? Latitude, double? Longitude) GetCoordinatesFromAddress(string? address=null)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(address))
                    {
                    return (null, null);

                }

                // Create URL for the API request
                string url = string.Format(apiUrl, apiKey, Uri.EscapeDataString(address));

                    using (HttpClient client = new HttpClient())
                    {
                        // Make the synchronous API request
                        HttpResponseMessage response = client.GetAsync(url).Result;

                        // Check if the API request was successful
                        if (response.IsSuccessStatusCode)
                        {
                            string jsonResponse = response.Content.ReadAsStringAsync().Result;

                        // Parse the JSON response
                        JArray jsonArray = JArray.Parse(jsonResponse);

                            // If there are results, return the coordinates
                            if (jsonArray.Count > 0)
                            {
                                var firstResult = jsonArray[0];
                                double latitude = (double)firstResult["lat"];
                                double longitude = (double)firstResult["lon"];
                                return (latitude, longitude);
                            }
                            else
                            {
                                throw new GeolocationNotFoundException(address);
                            }
                        }
                        else
                        {
                            throw new Exception(response.StatusCode.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error occurred while fetching coordinates for the address. ", ex);
                }
            }
        }
    }

