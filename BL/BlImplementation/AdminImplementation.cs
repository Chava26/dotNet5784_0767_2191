using BlApi;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlImplementation
{
    internal class AdminImplementation : IAdmin
    {
        private readonly DalApi.IDal _dal = DalApi.Factory.Get;




        #region Stage 5
        public void AddClockObserver(Action clockObserver) =>
        AdminManager.ClockUpdatedObservers += clockObserver;
        public void RemoveClockObserver(Action clockObserver) =>
        AdminManager.ClockUpdatedObservers -= clockObserver;
        public void AddConfigObserver(Action configObserver) =>
        AdminManager.ConfigUpdatedObservers += configObserver;
        public void RemoveConfigObserver(Action configObserver) =>
        AdminManager.ConfigUpdatedObservers -= configObserver;
        #endregion Stage 5

        public void AdvanceSystemClock(BO.TimeUnit timeUnit)
        {
            DateTime newClock = timeUnit switch
            {
                BO.TimeUnit.Minute => AdminManager.Now.AddMinutes(1),
                BO.TimeUnit.Hour => AdminManager.Now.AddHours(1),
                BO.TimeUnit.Day => AdminManager.Now.AddDays(1),
                BO.TimeUnit.Month => AdminManager.Now.AddMonths(1),
                BO.TimeUnit.Year => AdminManager.Now.AddYears(1),
                _ => throw new ArgumentOutOfRangeException(nameof(timeUnit), "Invalid time unit")
            };

            AdminManager.UpdateClock(newClock);
        }

        public TimeSpan GetRiskTimeRange()
        {
            return AdminManager.MaxRange;
        }
        public DateTime GetSystemClock()
        {
            return AdminManager.Now;
        }
        public void InitializeDatabase()
        {
            AdminManager.InitializeDB();
        }
        public void ResetDatabase()
        {
            AdminManager.ResetDB();
        }

        
        public void SetRiskTimeRange(TimeSpan timeRange)
        {
            AdminManager.MaxRange = timeRange;
        }
    }
}
