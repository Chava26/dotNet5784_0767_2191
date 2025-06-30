using BlApi;
using BO;
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


        public void StartSimulator(int interval)  //stage 7
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
            AdminManager.Start(interval); //stage 7
        }

        public void StopSimulator()
    => AdminManager.Stop(); //stage 7

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
            try
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
            catch (BLTemporaryNotAvailableException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BO.BlGeneralDatabaseException("An error occurred while advancing the system clock.", ex);
            }
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
            try
            {
                AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
                AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
                AdminManager.InitializeDB();
            }
            catch (BLTemporaryNotAvailableException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BO.BlGeneralDatabaseException("An error occurred while initializing the database.", ex);
            }
          
        }
        public void ResetDatabase()
        {
            try
            {
                AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
                AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
                AdminManager.ResetDB();
            }

            catch (BLTemporaryNotAvailableException) { throw; }
            catch (Exception ex) { throw new BO.BlGeneralDatabaseException("An error occurred while resetting the database.", ex); }
        }   

        
        public void SetRiskTimeRange(TimeSpan timeRange)
        {
            try
            {
                AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
                AdminManager.MaxRange = timeRange;

            }
            catch (BLTemporaryNotAvailableException) { throw; }
            catch (Exception ex) { throw new BO.BlGeneralDatabaseException("An error occurred while setting the risk time range.", ex); }
        }
    }
}
