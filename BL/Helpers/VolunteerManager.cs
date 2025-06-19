using BO;
using DalApi;
using DO;
using Newtonsoft.Json.Linq;
namespace Helpers
{
    internal class VolunteerManager
    {
        private static IDal s_dal = Factory.Get; //stage 4

        internal static ObserverManager Observers = new(); //stage 5

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
        /// Creates a BO.Volunteer object from a DO.Volunteer object.
        /// </summary>
        /// <param name="boVolunteer">The DO.Volunteer object.</param>
        /// <returns>A BO.Volunteer object.</returns>
        internal static BO.Volunteer CreateBoVolunteer(DO.Volunteer doVolunteer, BO.CallInProgress? boCallInProgress)
        {
            var assigments = s_dal.Assignment.ReadAll(a => a.VolunteerId == doVolunteer.Id);

            return new BO.Volunteer
            {
                Id = doVolunteer.Id,
                FullName = doVolunteer.Name,
                Email = doVolunteer.Email,
                PhoneNumber = doVolunteer.Phone,
                role = (BO.Role)doVolunteer.role,
                IsActive = doVolunteer.IsActive,
                MaxDistanceForTask = doVolunteer.MaximumDistance,
                Password = doVolunteer.Password!,
                Address = doVolunteer.Adress,
                Longitude = doVolunteer.Longitude,
                Latitude = doVolunteer.Latitude,
                DistanceType = (BO.DistanceType)doVolunteer.DistanceType,
                callInProgress = boCallInProgress,
                TotalHandledCalls = assigments.Count(a => a.TypeOfEndTime == DO.EndOfTreatment.treated),
                TotalCanceledCalls = assigments.Count(a => a.TypeOfEndTime == DO.EndOfTreatment.administratorCancel || a.TypeOfEndTime == DO.EndOfTreatment.selfCancel),
                TotalExpiredCalls = assigments.Count(a => a.TypeOfEndTime == DO.EndOfTreatment.expired),
            };
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
                throw new BO.BlDoesNotExistException("Volunteer object cannot be null.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(boVolunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new BO.BlInvalidFormatException("Invalid email format.");

            if (boVolunteer.Id<0 )
                throw new BO.BlInvalidFormatException("Invalid ID format.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(boVolunteer.PhoneNumber, @"^\d{10}$"))
                throw new BO.BlInvalidFormatException("Invalid phone number format. Phone number must have 10 digits.");

            if (boVolunteer.FullName.Length < 2)
                throw new BO.BlInvalidFormatException("Volunteer name is too short. Name must have at least 2 characters.");

            if (boVolunteer.Password.Length < 6)
                throw new BO.BlInvalidFormatException("Password is too short");
            
        }
        internal static bool IsValidId(int id)
        {
            string idStr = id.ToString().PadLeft(9, '0');
            if (idStr.Length != 9 || !int.TryParse(idStr, out _))
                return false;

            int sum = 0;

            for (int i = 0; i < 9; i++)
            {
                int digit = idStr[i] - '0';
                digit *= (i % 2 == 0) ? 1 : 2;

                if (digit > 9)
                    digit -= 9;

                sum += digit;
            }

            return sum % 10 == 0;
        }
        ///// <summary>
        ///// Validates logical constraints of the volunteer object.
        ///// </summary>
        ///// <param name="boVolunteer">The BO.Volunteer object to validate.</param>
        ///// <exception cref="ArgumentException">Thrown when any logical constraint is violated.</exception>
        
        public static (double? Latitude, double? Longitude) logicalChecking( BO.Volunteer boVolunteer)
        {
            if (!IsValidId(boVolunteer.Id))
                throw new BO.BlInvalidFormatException("Invalid ID format. ID must be a valid number with a correct checksum.");
            if (!IsPasswordStrong(boVolunteer.Password))
                throw new BO.BlInvalidFormatException("Password is too weak. It must have at least 6 characters, including uppercase, lowercase, and numbers.");
            return Tools.GetCoordinatesFromAddress(boVolunteer.Address);


        }
        /// <summary>
        /// Validates the requester's permissions to update the volunteer.
        /// </summary>
        /// <param name="requesterId">The ID of the requester.</param>
        /// <param name="boVolunteer">The BO.Volunteer object being updated.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown when the requester lacks permissions.</exception>
        public static void ValidatePermissions(DO.Volunteer requester, BO.Volunteer VolunteerForUpdate)
        {
            bool isSelf = requester.Id == VolunteerForUpdate.Id;

            bool isAdmin = requester.role == DO.Role.Manager;

            if (!isAdmin && !isSelf)
                throw new UnauthorizedAccessException("Only an admin or the volunteer themselves can perform this update.");
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
        internal static List<string> GetChangedFields(DO.Volunteer original, BO.Volunteer updated)
        {
            var changedFields = new List<string>();

            if (original.Name != updated.FullName) changedFields.Add("Name");
            if (original.Email != updated.Email) changedFields.Add("Email");
            if (original.Phone != updated.PhoneNumber) changedFields.Add("Phone");
            if (original.role != (DO.Role)updated.role) changedFields.Add("Role");
            if (original.Adress != updated.Address) changedFields.Add("Address");

            return changedFields;
        }

        internal static void CanUpdateFields(DO.Role requesterRole, List<string> changedFields, BO.Volunteer boVolunteer)
        {
            if (changedFields.Contains("Role"))
            {
                //bool isAdmin = boVolunteer.role == BO.Role.Manager;

                if (requesterRole != DO.Role.Manager)
                    throw new BO.BlUnauthorizedAccessException("You do not have permission to update the Role field.");

            }

        }
    }
}

