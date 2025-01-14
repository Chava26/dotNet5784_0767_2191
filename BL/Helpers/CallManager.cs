using BlApi;
using BO;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers;
internal class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    /// <summary>
    /// Calculates the status of a call based on its properties and the latest assignment.
    /// </summary>
    /// <param name="call">The call entity from the data layer.</param>
    /// <param name="latestAssignment">The latest assignment for the call, if any.</param>
    /// <param name="riskThreshold">The time span considered as a risk threshold.</param>
    /// <returns>The calculated <see cref="CallStatus"/>.</returns>
    public static CallStatus CalculateCallStatus(DO.Call call, DO.Assignment? latestAssignment, TimeSpan? riskThreshold=null)
    {
        // If no assignment exists, determine if the call is open or expired
        if (latestAssignment == null)
        {
            if (call.MaxFinishTime.HasValue && DateTime.Now > call.MaxFinishTime.Value)
            {
                return CallStatus.Expired;
            }

            TimeSpan? timeToMax = call.MaxFinishTime.HasValue
                ? call.MaxFinishTime.Value - DateTime.Now
                : null;

            if (timeToMax.HasValue && timeToMax <= riskThreshold)
            {
                return CallStatus.OpenRisk;
            }

            return CallStatus.Open;
        }

        // If an assignment exists but has an exit time, the call is closed
        if (latestAssignment.exitTime.HasValue)
        {
            return CallStatus.Closed;
        }

        // Calculate time remaining for in-progress calls
        TimeSpan? timeRemaining = call.MaxFinishTime.HasValue
            ? call.MaxFinishTime.Value - DateTime.Now
            : null;

        // Determine risk status for in-progress calls
        bool isAtRisk = timeRemaining.HasValue && timeRemaining <= riskThreshold;

        return isAtRisk ? CallStatus.InProgressRisk : CallStatus.InProgress;
    }

    /// <summary>
    /// Validates the details of the updated call.
    /// Performs format and logical checks on the provided Call object.
    /// </summary>
    /// <param name="call">The updated Call object to be validated.</param>
    /// <exception cref="ArgumentException">Thrown if any of the validation checks fail.</exception>
    internal static void ValidateCallDetails(BO.Call call)
    {

        
        // Validate that the address is a valid address with latitude and longitude
        if (string.IsNullOrWhiteSpace(call.FullAddress) ||
            !(call.Latitude >= -90 && call.Latitude <= 90 &&
              call.Longitude >= -180 && call.Longitude <= 180))
        {
            throw new ArgumentException("The address must be valid with latitude and longitude.");
        }

        // Validate the call type is valid
        if (!Enum.IsDefined(typeof(BO.CallType), call.CallType))
        {
            throw new ArgumentException("Invalid call type.");
        }

        // Validate the description length
        if (!string.IsNullOrEmpty(call.Description) && call.Description.Length > 500)
        {
            throw new ArgumentException("Description is too long (maximum 500 characters).");
        }

        // Validate that there are no assignments in the past
        if (call.CallAssignments != null && call.CallAssignments.Any(a => a.TreatmentStartTime < call.OpenTime))
        {
            throw new ArgumentException("Assignments cannot start before the call's open time.");
        }
    }

    internal static (double? Latitude, double? Longitude) logicalChecking(BO.Call boCall)
    {
        if (boCall.MaxEndTime.HasValue && boCall.MaxEndTime.Value <= boCall.OpenTime) { 
            throw new InvalidOperationException("The MaxEndTime must be greater than the OpenTime.");
        }
        // Validate that the open time is not in the future
        if (boCall.OpenTime > DateTime.Now)
        {
            throw new ArgumentException("The open time cannot be in the future.");
        }

        return Tools.GetCoordinatesFromAddress(boCall.FullAddress);


    }

}


