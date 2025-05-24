
namespace BlApi
{
    /// <summary>
    /// Interface for administrative functionalities in the system.
    /// </summary>
    public interface IAdmin
    {
        #region Stage 5
        void AddConfigObserver(Action configObserver);
        void RemoveConfigObserver(Action configObserver);
        void AddClockObserver(Action clockObserver);
        void RemoveClockObserver(Action clockObserver);
        #endregion Stage 5

        /// <summary>
        /// Retrieves the current system clock value.
        /// </summary>
        /// <returns>The current system time as a DateTime object.</returns>
        DateTime GetSystemClock();

        /// <summary>
        /// Advances the system clock by a specified time unit.
        /// </summary>
        /// <param name="timeUnit">The time unit to advance by (minute, hour, day, month, year).</param>
        void AdvanceSystemClock(BO.TimeUnit timeUnit);

        /// <summary>
        /// Retrieves the current risk time range configuration.
        /// </summary>
        /// <returns>The risk time range as a TimeSpan object.</returns>
        TimeSpan GetRiskTimeRange();

        /// <summary>
        /// Sets the risk time range configuration.
        /// </summary>
        /// <param name="timeRange">The new risk time range to set.</param>
        void SetRiskTimeRange(TimeSpan timeRange);

        /// <summary>
        /// Resets the database to its initial state.
        /// </summary>
        void ResetDatabase();

        /// <summary>
        /// Initializes the database with default data.
        /// </summary>
        void InitializeDatabase();
    }

}
