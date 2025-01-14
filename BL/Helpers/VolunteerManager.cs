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
               (DO.Role)boVolunteer.role,
                boVolunteer.IsActive,
                boVolunteer.MaxDistanceForTask,
               EncryptPassword(boVolunteer.Password),
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

            if (!System.Text.RegularExpressions.Regex.IsMatch(boVolunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("Invalid email format.");

            if (boVolunteer.Id<0)
                throw new ArgumentException("Invalid ID format.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(boVolunteer.PhoneNumber, @"^\d{10}$"))
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
        public static (double? Latitude, double? Longitude) logicalChecking( BO.Volunteer boVolunteer)
        {
            IsPasswordStrong(boVolunteer.Password);
            return Tools.GetCoordinatesFromAddress(boVolunteer.Address);


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
            bool isAdmin = boVolunteer.role == Role.Manager;

            if (!isAdmin && !isSelf)
                throw new UnauthorizedAccessException("Only an admin or the volunteer themselves can perform this update.");

            if (!isAdmin && boVolunteer.role != Role.Volunteer)
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
        public static bool VerifyPassword(string plainPassword, string encryptedPassword)
        {
            var encryptedAttempt = EncryptPassword(plainPassword);
            return encryptedAttempt == encryptedPassword;
        }






    }
}

