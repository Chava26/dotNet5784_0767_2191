
using System.Collections;
using System.Reflection;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using BO;
using Newtonsoft.Json.Linq;
using System;


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
        public static string ToStringProperty<T>(this T t)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t), "The object instance cannot be null.");
            }

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var propertyValues = properties
                .Select(property =>
                {
                    var value = property.GetValue(t, null);

                    if (value is IEnumerable enumerable && !(value is string))
                    {
                        var elements = enumerable.Cast<object>().Select(e => e?.ToString() ?? "null");
                        return $"{property.Name}: [{string.Join(", ", elements)}]";
                    }

                    return $"{property.Name}: {(value != null ? value.ToString() : "null")}";
                });

            return string.Join(", ", propertyValues);
        }

        //public static bool IsValidChecksum(string id)
        //{
        //    int sum = 0;
        //    for (int i = 0; i < id.Length; i++)
        //    {
        //        int digit = int.Parse(id[i].ToString());
        //        if (i % 2 == 1) digit *= 2;
        //        if (digit > 9) digit -= 9;
        //        sum += digit;
        //    }
        //    return sum % 10 == 0;
        //}
       
        /// <summary>
        /// Calculates the status of a call based on its current progress and remaining time until the maximum finish time.
        /// </summary>
        /// <param name="assignment">The assignment related to the volunteer.</param>
        /// <param name="call">The call being handled by the volunteer.</param>
        /// <param name="riskThreshold">The time threshold (in minutes) for determining a "risk" status.</param>
        /// <returns>The calculated <see cref="CallStatus"/> of the call.</returns>
        public static CallStatus CalculateStatus(DO.Assignment assignment, DO.Call call, int riskThreshold = 30)
        {
            // Check if the call is currently in progress (not completed)
            if (assignment.exitTime == null)
            {
                // Verify that the call has a defined maximum finish time
                if (call.MaxFinishTime.HasValue)
                {
                    // Calculate the remaining time for the call
                    var remainingTime = call.MaxFinishTime.Value - DateTime.Now;

                    // Determine if the call is approaching the risk threshold
                    if (remainingTime.TotalMinutes <= riskThreshold)
                    {
                        return CallStatus.InProgressRisk; // Call is in progress and nearing its maximum finish time
                    }
                    else
                    {
                        return CallStatus.InProgress; // Call is in progress
                    }
                }
                else
                {
                    // If MaxFinishTime is not defined, assume the call is simply in progress
                    return CallStatus.InProgress;
                }
            }

            return CallStatus.Closed;
        }

        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var r = 6371; 
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return r * c;
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
        public static (double? Latitude, double? Longitude) GetCoordinatesFromAddress(string? address = null)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new BlInvalidFormatException(address);
            }
            try
            {

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
                        throw new BlApiRequestException($"API request failed with status code: {response.StatusCode}"); 
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BlApiRequestException($"Error occurred while fetching coordinates for the address. {ex.Message}");

            }

        }
    }
    //public static string ToStringProperty<T>(this T t)
    //{
    //    if (t == null)
    //        return "null";

    //    var result = new StringBuilder();

    //    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

    //    foreach (var property in properties)
    //    {
    //        var value = property.GetValue(t);

    //        if (value is IEnumerable enumerable && !(value is string))
    //        {
    //            result.Append($"{property.Name}: [");

    //            foreach (var item in enumerable)
    //            {
    //                result.Append($"{item}, ");
    //            }

    //            result.Append("], ");
    //        }
    //        else
    //        {
    //            result.Append($"{property.Name}: {value}, ");
    //        }
    //    }

    //    return result.ToString().TrimEnd(',', ' ');
    //}
}
    
