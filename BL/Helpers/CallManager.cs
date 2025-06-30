//using BlApi;
//using BO;
//using DalApi;
//using DO;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.NetworkInformation;
//using System.Text;
//using System.Threading.Tasks;

//namespace Helpers;
//internal class CallManager
//{
//    private static IDal s_dal = DalApi.Factory.Get;//stage 4




//    internal static ObserverManager Observers = new(); //stage 5

//    /// <summary>
//    /// Calculates the status of a call based on its properties and the latest assignment
//    /// </summary>
//    /// <param name="call">The call entity from the data layer.</param>
//    /// <param name="latestAssignment">The latest assignment for the call, if any.</param>
//    /// <param name="riskThreshold">The time span considered as a risk threshold.</param>
//    /// <returns>The calculated <see cref="CallStatus"/>.</returns>
//    public static CallStatus CalculateCallStatus(DO.Call call, DO.Assignment? latestAssignment = null)
//    {
//        if (latestAssignment is null) latestAssignment = s_dal.Assignment.ReadAll().FirstOrDefault(a => a.CallId == call.Id);

//        TimeSpan riskThreshold = TimeSpan.FromMinutes(30); // Risk threshold configuration

//        // If no assignment exists, determine if the call is open or expired
//        if (latestAssignment == null)
//        {
//            if (call.MaxFinishTime.HasValue && DateTime.Now > call.MaxFinishTime.Value)
//            {
//                return CallStatus.Expired;
//            }

//            TimeSpan? timeToMax = call.MaxFinishTime.HasValue
//                ? call.MaxFinishTime.Value - DateTime.Now
//                : null;

//            if (timeToMax.HasValue && timeToMax <= riskThreshold)
//            {
//                return CallStatus.OpenRisk;
//            }

//            return CallStatus.Open;
//        }

//        //If an assignment exists but has an exit time, the call is closed
//        if (latestAssignment.exitTime.HasValue)
//        {
//            return CallStatus.Closed;
//        }

//        // Calculate time remaining for in-progress calls
//        TimeSpan? timeRemaining = call.MaxFinishTime.HasValue
//            ? call.MaxFinishTime.Value - DateTime.Now
//            : null;

//        // Determine risk status for in-progress calls
//        bool isAtRisk = timeRemaining.HasValue && timeRemaining <= riskThreshold;

//        return isAtRisk ? CallStatus.InProgressRisk : CallStatus.InProgress;
//    }

//    /// <summary>
//    /// Validates the details of the updated call.
//    /// Performs format and logical checks on the provided Call object.
//    /// </summary>
//    /// <param name="call">The updated Call object to be validated.</param>
//    /// <exception cref="ArgumentException">Thrown if any of the validation checks fail.</exception>
//    internal static void ValidateCallDetails(BO.Call call)
//    {


//        // Validate that the address is a valid address with latitude and longitude
//        if (string.IsNullOrWhiteSpace(call.FullAddress))
//        {
//            throw new BlInvalidFormatException("The address must be valid");
//        }

//        // Validate the call type is valid
//        if (!Enum.IsDefined(typeof(BO.CallType), call.CallType))
//        {
//            throw new BlInvalidFormatException("Invalid call type.");
//        }

//        // Validate the description length
//        if (!string.IsNullOrEmpty(call.Description) && call.Description.Length > 500)
//        {
//            throw new BlInvalidFormatException("Description is too long (maximum 500 characters).");
//        }

//        // Validate that there are no assignments in the past
//        if (call.CallAssignments != null && call.CallAssignments.Any(a => a.TreatmentStartTime < call.OpenTime))
//        {
//            throw new BlInvalidFormatException("Assignments cannot start before the call's open time.");
//        }
//        //Validate that the open time is not in the future
//        if (call.OpenTime > DateTime.Now)
//        {
//            throw new BlInvalidFormatException("The open time cannot be in the future.");
//        }
//        //Validate that the endTime is past the openTime
//        if (call.MaxEndTime.HasValue && call.MaxEndTime.Value <= call.OpenTime)
//        {
//            throw new BO.BlInvalidOperationException("The MaxEndTime must be greater than the OpenTime.");
//        }
//    }

//    internal static void logicalChecking(BO.Call boCall)
//    {
//        if (boCall.MaxEndTime.HasValue && boCall.MaxEndTime.Value <= boCall.OpenTime)
//        {
//            throw new BO.BlInvalidOperationException("The MaxEndTime must be greater than the OpenTime.");
//        }
//        // Validate that the open time is not in the future
//        if (boCall.OpenTime > DateTime.Now)
//        {
//            throw new ArgumentException("The open time cannot be in the future.");
//        }



//    }

//    /// <summary>
//    /// Sends an email notification to all volunteers within the specified distance from a new call.
//    /// </summary>
//    /// <param name="call">The call object that was opened.</param>
//    internal static void SendEmailWhenCalOpened(BO.Call call)
//    {
//       var volunteer = s_dal.Volunteer.ReadAll();
//        if(volunteer is not null){
//        foreach (var item in volunteer)
//        {

//            if (item.MaximumDistance >= Tools.CalculateDistance((double)item.Latitude!, (double)item.Longitude!, (double)call.Latitude!, (double)call.Longitude!))
//            {
//                string subject = "Openning call";
//                string body = $@"
//      Hello {item.Name},

//     A new call has been opened in your area.
//      Call Details:
//      - Call ID: {call.Id}
//      - Call Type: {call.CallType}
//      - Call Address: {call.FullAddress}
//      - Opening Time: {call.OpenTime}
//      - Description: {call.Description}
//      - Entry Time for Treatment: {call.MaxEndTime}
//      -call Status:{call.Status}

//      If you wish to handle this call, please log into the system.

//      Best regards,  
//     Call Management System";

//                Tools.SendEmail(item.Email, subject, body);
//            }
//        }
//        }
//    }

//    /// <summary>
//    /// Sends an email notification to the volunteer when their assignment is canceled.
//    /// </summary>
//    /// <param name="volunteer">The volunteer to notify.</param>
//    /// <param name="assignment">The assignment that was canceled.</param>
//    internal static void SendEmailToVolunteer(DO.Volunteer volunteer, DO.Assignment assignment)
//    {
//        DO.Call call = s_dal.Call.Read(assignment.CallId)!;

//        string subject = "Assignment Canceled";
//        string body = $@"
//      Hello {volunteer.Name},

//      Your assignment for handling call {assignment.Id} has been canceled by the administrator.

//      Call Details:
//      - Call ID: {assignment.CallId}
//      - Call Type: {call.MyCallType}
//      - Call Address: {call.Address}
//      - Opening Time: {call.OpenTime}
//      - Description: {call.Description}
//      - Entry Time for Treatment: {assignment.EntryTime}

//      Best regards,  
//      Call Management System";

//        Tools.SendEmail(volunteer.Email, subject, body);
//    }
//    internal static DO.Call CreateDoCall(BO.Call call)
//    {

//        return new DO.Call
//        {
//            MyCallType = (DO.CallType)call.CallType,
//            Description = call.Description,
//            Address = call.FullAddress,
//            Latitude = (double)call.Latitude!,
//            Longitude = (double)call.Longitude!,
//            OpenTime = call.OpenTime,
//            MaxFinishTime = call.MaxEndTime
//        };
//    }
//   internal static BO.Call CreateBoCall(DO.Call call, List<CallAssignInList> callAssignments)
//    {
//    return   new BO.Call{
//        Id = call.Id,
//            CallType = (BO.CallType) call.MyCallType,
//            Description = call.Description,
//            FullAddress = call.Address,
//            Latitude = call.Latitude,
//            Longitude = call.Longitude,
//            OpenTime = call.OpenTime,
//            MaxEndTime = call.MaxFinishTime,
//             Status = CalculateCallStatus(call),
//           CallAssignments = callAssignments
//        };
//    }
//    /// <summary>
//    /// Periodically updates the calls based on the current clock and checks for expired assignments.
//    /// </summary>
//    /// <param name="oldClock">The previous clock time.</param>
//    /// <param name="newClock">The new clock time.</param>

//    internal static void PeriodicCallsUpdates(DateTime oldClock, DateTime newClock)
//    {


//        List<DO.Call> expiredCalls = s_dal.Call.ReadAll(c => c.MaxFinishTime < newClock).ToList();
//        expiredCalls.ForEach(call =>
//        {
//            List<DO.Assignment> assignments = s_dal.Assignment.ReadAll(a => a.CallId == call.Id).ToList();

//            if (!assignments.Any())
//            {
//                s_dal.Assignment.Create(new DO.Assignment(
//                    CallId: call.Id,
//                    VolunteerId: 0,
//                    EntryTime: ClockManager.Now,
//                    exitTime: ClockManager.Now,
//                    TypeOfEndTime: (DO.EndOfTreatment)BO.EndOfTreatment.expired
//                ));
//                CallManager.Observers.NotifyListUpdated(); // Stage 5

//            }

//            List<DO.Assignment> assignmentsWithNull = s_dal.Assignment.ReadAll(a => a.CallId == call.Id && a.TypeOfEndTime is null).ToList();

//            if (assignmentsWithNull.Any())
//            {
//                assignments.ForEach(assignment => {
//                    s_dal.Assignment.Update(assignment with
//                    {
//                        exitTime = ClockManager.Now,
//                        TypeOfEndTime = (DO.EndOfTreatment)BO.EndOfTreatment.expired
//                    });
//                    Observers.NotifyItemUpdated(assignment.Id); //stage 5 
//                    DO.Volunteer volunteer = s_dal.Volunteer.Read(a => a.Id == assignment.VolunteerId)!;
//                    SendEmailToVolunteer(volunteer, assignment);

//                }
//                    );
//            }
//        });
//    }

//    // פונקציות עזר לסינון ומיון
//    internal static IEnumerable<BO.CallInList> ApplyFilter(IEnumerable<BO.CallInList> callList, BO.CallField field, object value)
//    {
//        return field switch
//        {
//            BO.CallField.CallType => callList.Where(c => c.CallType.Equals(value)),
//            BO.CallField.Status => callList.Where(c => c.Status.Equals(value)),
//            BO.CallField.OpenTime => callList.Where(c => c.OpenTime.Equals(value)),
//            BO.CallField.AssignmentsCount => callList.Where(c => c.AssignmentsCount.Equals(value)),
//            BO.CallField.CallId => callList.Where(c => c.CallId.Equals(value)),
//            _ => callList
//        };
//    }

//    internal static IEnumerable<BO.CallInList> ApplySort(IEnumerable<BO.CallInList> callList, BO.CallField field)
//    {
//        return field switch
//        {
//            BO.CallField.CallType => callList.OrderBy(c => c.CallType),
//            BO.CallField.Status => callList.OrderBy(c => c.Status),
//            BO.CallField.OpenTime => callList.OrderBy(c => c.OpenTime),
//            BO.CallField.AssignmentsCount => callList.OrderBy(c => c.AssignmentsCount),
//            BO.CallField.CallId => callList.OrderBy(c => c.CallId),
//            _ => callList.OrderBy(c => c.CallId)
//        };
//    }
//}

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




    internal static ObserverManager Observers = new(); //stage 5

    /// <summary>
    /// Calculates the status of a call based on its properties and the latest assignment
    /// </summary>
    /// <param name="call">The call entity from the data layer.</param>
    /// <param name="latestAssignment">The latest assignment for the call, if any.</param>
    /// <param name="riskThreshold">The time span considered as a risk threshold.</param>
    /// <returns>The calculated <see cref="CallStatus"/>.</returns>
    public static CallStatus CalculateCallStatus(DO.Call call, DO.Assignment? latestAssignment = null)
    {
        if (latestAssignment is null)
        {
            lock (AdminManager.BlMutex) //stage 7
                latestAssignment = s_dal.Assignment.ReadAll().FirstOrDefault(a => a.CallId == call.Id);
        }

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
        if (string.IsNullOrWhiteSpace(call.FullAddress))
        {
            throw new BlInvalidFormatException("The address must be valid");
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
            throw new BO.BlInvalidOperationException("The MaxEndTime must be greater than the OpenTime.");
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
        IEnumerable<DO.Volunteer> volunteer;

        lock (AdminManager.BlMutex) //stage 7
            volunteer = s_dal.Volunteer.ReadAll();

        if (volunteer is not null)
        {
            foreach (var item in volunteer)
            {

                if (item.MaximumDistance >= Tools.CalculateDistance((double)item.Latitude!, (double)item.Longitude!, (double)call.Latitude!, (double)call.Longitude!))
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
        DO.Call call;

        lock (AdminManager.BlMutex) //stage 7
            call = s_dal.Call.Read(assignment.CallId)!;

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
            Latitude = (double)call.Latitude!,
            Longitude = (double)call.Longitude!,
            OpenTime = call.OpenTime,
            MaxFinishTime = call.MaxEndTime
        };
    }
    internal static BO.Call CreateBoCall(DO.Call call, List<CallAssignInList> callAssignments)
    {
        return new BO.Call
        {
            Id = call.Id,
            CallType = (BO.CallType)call.MyCallType,
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
    /// <summary>
    /// Periodically updates the calls based on the current clock and checks for expired assignments.
    /// </summary>
    /// <param name="oldClock">The previous clock time.</param>
    /// <param name="newClock">The new clock time.</param>

    internal static void PeriodicCallsUpdates(DateTime oldClock, DateTime newClock)
    {
        List<DO.Call> expiredCalls;
        List<DO.Assignment> allAssignments;
        List<DO.Volunteer> allVolunteers;

        // קבלת כל הנתונים בבת אחת וביצוע ToList() מיד
        lock (AdminManager.BlMutex) //stage 7
        {
            expiredCalls = s_dal.Call.ReadAll(c => c.MaxFinishTime < newClock).ToList();
            allAssignments = s_dal.Assignment.ReadAll().ToList();
            allVolunteers = s_dal.Volunteer.ReadAll().ToList();
        }

        // רשימות לצבירת הודעות Observer
        bool shouldNotifyListUpdated = false;
        List<int> updatedAssignmentIds = new List<int>();

        expiredCalls.ForEach(call =>
        {
            var callAssignments = allAssignments.Where(a => a.CallId == call.Id).ToList();

            if (!callAssignments.Any())
            {
                lock (AdminManager.BlMutex) //stage 7
                    s_dal.Assignment.Create(new DO.Assignment(
                        CallId: call.Id,
                        VolunteerId: 0,
                        EntryTime: ClockManager.Now,
                        exitTime: ClockManager.Now,
                        TypeOfEndTime: (DO.EndOfTreatment)BO.EndOfTreatment.expired
                    ));

                shouldNotifyListUpdated = true;
            }

            var assignmentsWithNull = callAssignments.Where(a => a.TypeOfEndTime is null).ToList();

            if (assignmentsWithNull.Any())
            {
                assignmentsWithNull.ForEach(assignment => {
                    lock (AdminManager.BlMutex) //stage 7
                    {
                        s_dal.Assignment.Update(assignment with
                        {
                            exitTime = ClockManager.Now,
                            TypeOfEndTime = (DO.EndOfTreatment)BO.EndOfTreatment.expired
                        });
                    }

                    updatedAssignmentIds.Add(assignment.Id);

                    var volunteer = allVolunteers.FirstOrDefault(v => v.Id == assignment.VolunteerId);
                    if (volunteer != null)
                        SendEmailToVolunteer(volunteer, assignment);
                });
            }
        });

        // ביצוע כל הודעות Observer מחוץ ל-lock
        if (shouldNotifyListUpdated)
            CallManager.Observers.NotifyListUpdated();

        foreach (int assignmentId in updatedAssignmentIds)
            Observers.NotifyItemUpdated(assignmentId);
    }

    // פונקציות עזר לסינון ומיון
    internal static IEnumerable<BO.CallInList> ApplyFilter(IEnumerable<BO.CallInList> callList, BO.CallField field, object value)
    {
        return field switch
        {
            BO.CallField.CallType => callList.Where(c => c.CallType.Equals(value)),
            BO.CallField.Status => callList.Where(c => c.Status.Equals(value)),
            BO.CallField.OpenTime => callList.Where(c => c.OpenTime.Equals(value)),
            BO.CallField.AssignmentsCount => callList.Where(c => c.AssignmentsCount.Equals(value)),
            BO.CallField.CallId => callList.Where(c => c.CallId.Equals(value)),
            _ => callList
        };
    }
   static public IEnumerable<BO.OpenCallInList> GetOpenCalls(int volunteerId, BO.CallType? callType, BO.CallField? sortField)
    {
        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex)
            volunteer = s_dal.Volunteer.Read(volunteerId) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist.");
        var (volunteerLat, volunteerLon) = (
            volunteer.Latitude ?? throw new BO.BlInvalidFormatException($"Cannot calculate distance: Volunteer with ID {volunteerId} is missing latitude."),
            volunteer.Longitude ?? throw new BO.BlInvalidFormatException($"Cannot calculate distance: Volunteer with ID {volunteerId} is missing longitude.")
        );
        lock (AdminManager.BlMutex)
        {
            var openCalls = s_dal.Call.ReadAll()
            .Where(c =>
            (CallManager.CalculateCallStatus(c) == BO.CallStatus.Open || CallManager.CalculateCallStatus(c) == BO.CallStatus.OpenRisk))
            .Select(c => new BO.OpenCallInList
            {
                Id = c.Id,
                CallType = (BO.CallType)c.MyCallType,
                Description = c.Description,
                FullAddress = c.Address,
                OpenTime = c.OpenTime,
                MaxEndTime = c.MaxFinishTime,
                DistanceFromVolunteer = Tools.CalculateDistance(volunteerLat, volunteerLon, c.Latitude, c.Longitude)
            });

            return sortField.HasValue
            ? openCalls.OrderBy(c => c.GetType().GetProperty(sortField.ToString()!)?.GetValue(c))
            : openCalls.OrderBy(c => c.Id);
        }
    }

    internal static IEnumerable<BO.CallInList> ApplySort(IEnumerable<BO.CallInList> callList, BO.CallField field)
    {
        return field switch
        {
            BO.CallField.CallType => callList.OrderBy(c => c.CallType),
            BO.CallField.Status => callList.OrderBy(c => c.Status),
            BO.CallField.OpenTime => callList.OrderBy(c => c.OpenTime),
            BO.CallField.AssignmentsCount => callList.OrderBy(c => c.AssignmentsCount),
            BO.CallField.CallId => callList.OrderBy(c => c.CallId),
            _ => callList.OrderBy(c => c.CallId)
        };
    }
}