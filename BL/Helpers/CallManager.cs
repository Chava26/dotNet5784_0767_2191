using BlApi;
using BO;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Helpers;
internal class CallManager
{
    private static IDal s_dal = DalApi.Factory.Get;//stage 4
    
    /// <summary>
    /// Calculates the status of a call based on its properties and the latest assignment
    /// </summary>
    /// <param name="call">The call entity from the data layer.</param>
    /// <param name="latestAssignment">The latest assignment for the call, if any.</param>
    /// <param name="riskThreshold">The time span considered as a risk threshold.</param>
    /// <returns>The calculated <see cref="CallStatus"/>.</returns>
    public static CallStatus CalculateCallStatus(DO.Call call, DO.Assignment? latestAssignment = null)
    {
        if (latestAssignment is null) latestAssignment = s_dal.Assignment.ReadAll().FirstOrDefault(a => a.CallId == call.Id);

        TimeSpan riskThreshold = TimeSpan.FromMinutes(30); // Risk threshold configuration

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

        //If an assignment exists but has an exit time, the call is closed
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
            throw new BlInvalidFormatException("The address must be valid with latitude and longitude.");
        }

        // Validate the call type is valid
        if (!Enum.IsDefined(typeof(BO.CallType), call.CallType))
        {
            throw new BlInvalidFormatException("Invalid call type.");
        }

        // Validate the description length
        if (!string.IsNullOrEmpty(call.Description) && call.Description.Length > 500)
        {
            throw new BlInvalidFormatException("Description is too long (maximum 500 characters).");
        }

        // Validate that there are no assignments in the past
        if (call.CallAssignments != null && call.CallAssignments.Any(a => a.TreatmentStartTime < call.OpenTime))
        {
            throw new BlInvalidFormatException("Assignments cannot start before the call's open time.");
        }
        //Validate that the open time is not in the future
        if (call.OpenTime > DateTime.Now)
        {
            throw new BlInvalidFormatException("The open time cannot be in the future.");
        }
        //Validate that the endTime is past the openTime
        if (call.MaxEndTime.HasValue && call.MaxEndTime.Value <= call.OpenTime)
        {
            throw new BO.BlInvalidOperationException("The MaxEndTime must be greater than the OpenTime.");
        }
    }

    internal static void logicalChecking(BO.Call boCall)
    {
        if (boCall.MaxEndTime.HasValue && boCall.MaxEndTime.Value <= boCall.OpenTime)
        {
            throw new InvalidOperationException("The MaxEndTime must be greater than the OpenTime.");
        }
        // Validate that the open time is not in the future
        if (boCall.OpenTime > DateTime.Now)
        {
            throw new ArgumentException("The open time cannot be in the future.");
        }
       


    }

    /// <summary>
    /// Sends an email notification to all volunteers within the specified distance from a new call.
    /// </summary>
    /// <param name="call">The call object that was opened.</param>
    internal static void SendEmailWhenCalOpened(BO.Call call)
    {
       var volunteer = s_dal.Volunteer.ReadAll();
        if(volunteer is not null){
        foreach (var item in volunteer)
        {

            if (item.MaximumDistance >= Tools.CalculateDistance((double)item.Latitude!, (double)item.Longitude!, call.Latitude, call.Longitude))
            {
                string subject = "Openning call";
                string body = $@"
      Hello {item.Name},

     A new call has been opened in your area.
      Call Details:
      - Call ID: {call.Id}
      - Call Type: {call.CallType}
      - Call Address: {call.FullAddress}
      - Opening Time: {call.OpenTime}
      - Description: {call.Description}
      - Entry Time for Treatment: {call.MaxEndTime}
      -call Status:{call.Status}

      If you wish to handle this call, please log into the system.

      Best regards,  
     Call Management System";

                Tools.SendEmail(item.Email, subject, body);
            }
        }
        }
    }

    /// <summary>
    /// Sends an email notification to the volunteer when their assignment is canceled.
    /// </summary>
    /// <param name="volunteer">The volunteer to notify.</param>
    /// <param name="assignment">The assignment that was canceled.</param>
    internal static void SendEmailToVolunteer(DO.Volunteer volunteer, DO.Assignment assignment)
    {
        DO.Call call = s_dal.Call.Read(assignment.CallId)!;

        string subject = "Assignment Canceled";
        string body = $@"
      Hello {volunteer.Name},

      Your assignment for handling call {assignment.Id} has been canceled by the administrator.

      Call Details:
      - Call ID: {assignment.CallId}
      - Call Type: {call.MyCallType}
      - Call Address: {call.Address}
      - Opening Time: {call.OpenTime}
      - Description: {call.Description}
      - Entry Time for Treatment: {assignment.EntryTime}

      Best regards,  
      Call Management System";

        Tools.SendEmail(volunteer.Email, subject, body);
    }
    internal static DO.Call CreateDoCall(BO.Call call)
    {
        return new DO.Call
        {
            MyCallType = (DO.CallType)call.CallType,
            Description = call.Description,
            Address = call.FullAddress,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpenTime = call.OpenTime,
            MaxFinishTime = call.MaxEndTime
        };
    }
   internal static BO.Call CreateBoCall(DO.Call call, List<CallAssignInList> callAssignments)
    {
    return   new BO.Call{
        Id = call.Id,
            CallType = (BO.CallType) call.MyCallType,
            Description = call.Description,
            FullAddress = call.Address,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpenTime = call.OpenTime,
            MaxEndTime = call.MaxFinishTime,
             Status = CalculateCallStatus(call),
           CallAssignments = callAssignments
        };
}
}


