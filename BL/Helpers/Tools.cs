
using System.Collections;
using System.Reflection;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using BO;
using Newtonsoft.Json.Linq;
using System.Text.Json;
namespace Helpers;
using System.Net;
using System.Net.Mail;




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
    /// <summary>
    /// Calculates the great-circle distance between two geographic coordinates 
    /// using the Haversine formula.
    /// </summary>
    /// <param name="lat1">Latitude of the first point in decimal degrees.</param>
    /// <param name="lon1">Longitude of the first point in decimal degrees.</param>
    /// <param name="lat2">Latitude of the second point in decimal degrees.</param>
    /// <param name="lon2">Longitude of the second point in decimal degrees.</param>
    /// <returns>The distance between the two points in kilometers.</returns>
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
            string apiKey = "PK.83B935C225DF7E2F9B1ee90A6B46AD86";
            using var client = new HttpClient();
            string url = $"https://us1.locationiq.com/v1/search.php?key={apiKey}&q={Uri.EscapeDataString(address)}&format=json";

            var response = client.GetAsync(url).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
                throw new Exception("Invalid address or API error.");

            var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.ValueKind != JsonValueKind.Array || doc.RootElement.GetArrayLength() == 0)
                throw new Exception("Address not found.");

            var root = doc.RootElement[0];

            double latitude = double.Parse(root.GetProperty("lat").GetString());
            double longitude = double.Parse(root.GetProperty("lon").GetString());

            return (latitude, longitude);
        }


            
        /// <summary>
        /// Sends an email using an SMTP server.
        /// </summary>
        /// <param name="toEmail">The recipient's email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body of the email.</param>
        /// <exception cref="Exception">Thrown when the email cannot be sent.</exception>
        public static void SendEmail(string toEmail, string subject, string body)
        {
            var fromAddress = new MailAddress("yedidimorganization@gmail.com", "Yedidim");
            var toAddress = new MailAddress(toEmail);

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("yedidimorganization@gmail.com", "wrhm bdep vaqb nisb"),
                EnableSsl = true,
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
            })
            {
                smtpClient.Send(message);
            }
        }

     }
    
   


    
