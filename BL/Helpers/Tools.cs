using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    internal static class Tools
    {
        /// <summary>
        /// Extension method to generate a string representation of all the properties of an object of type T using Reflection.
        /// If a property is a collection, its elements will be included in the string.
        /// </summary>
        /// <typeparam name="T">The type of the object to reflect upon.</typeparam>
        /// <param name="t">The object instance to analyze.</param>
        /// <returns>A string containing the names and values of all properties of the object, including elements of collections.</returns>
        //public static string ToStringProperty<T>(this T t)
        //{
        //    if (t == null)
        //    {
        //        throw new ArgumentNullException(nameof(t), "The object instance cannot be null.");
        //    }

        //    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        //    var propertyValues = properties
        //        .Select(property =>
        //        {
        //            var value = property.GetValue(t, null);

        //            if (value is IEnumerable enumerable && !(value is string))
        //            {
        //                var elements = enumerable.Cast<object>().Select(e => e?.ToString() ?? "null");
        //                return $"{property.Name}: [{string.Join(", ", elements)}]";
        //            }

        //            return $"{property.Name}: {(value != null ? value.ToString() : "null")}";
        //        });

        //    return string.Join(", ", propertyValues);
        //}
        //public (double Longitude, double Latitude)? GetCoordinatesFromAddress(string address)
        //{
        //    // Mock function to simulate address resolution
        //    return new Random().Next(0, 2) == 0 ? null : (Longitude: 34.7818, Latitude: 32.0853);
        //}
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\d{10}$");
        }
        public static bool IsValidId(string id)
        {
            if (!long.TryParse(id, out _)) return false;
            return id.Length == 9 && IsValidChecksum(id);
        }

        public static bool IsValidChecksum(string id)
        {
            int sum = 0;
            for (int i = 0; i < id.Length; i++)
            {
                int digit = int.Parse(id[i].ToString());
                if (i % 2 == 1) digit *= 2;
                if (digit > 9) digit -= 9;
                sum += digit;
            }
            return sum % 10 == 0;
        }
        private static bool IsValidEmail(string email)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

    }
}
