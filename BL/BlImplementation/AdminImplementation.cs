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

        public void AdvanceSystemClock(BO.TimeUnit timeUnit)
        {
            DateTime newClock = timeUnit switch
            {
                BO.TimeUnit.Minute => ClockManager.Now.AddMinutes(1),
                BO.TimeUnit.Hour => ClockManager.Now.AddHours(1),
                BO.TimeUnit.Day => ClockManager.Now.AddDays(1),
                BO.TimeUnit.Month => ClockManager.Now.AddMonths(1),
                BO.TimeUnit.Year => ClockManager.Now.AddYears(1),
                _ => throw new ArgumentOutOfRangeException(nameof(timeUnit), "Invalid time unit")
            };

            ClockManager.UpdateClock(newClock);
        }

        public TimeSpan GetRiskTimeRange()
        {
            return _dal.Config.RiskRange;
        }

        public DateTime GetSystemClock()
        {
            return ClockManager.Now;
        }

        public void InitializeDatabase()
        {
            ResetDatabase();
            //_dal.Initialization();
        }

        public void ResetDatabase()
        {
            //_dal.Config.ResetToDefaults();
            _dal.ResetDB();
        }

        public void SetRiskTimeRange(TimeSpan timeRange)
        {
            _dal.Config.RiskRange = timeRange;
        }
    }
}
