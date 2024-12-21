using DalApi;

namespace Dal;

/// <summary>
/// This class implements the IConfig interface, providing access to configuration properties 
/// like Clock and RiskRange, and also includes a method to reset the configuration to default values.
/// </summary>
internal class ConfigImplementation : IConfig

     /// <summary>
     /// Gets or sets the current clock (DateTime) from the configuration.
     /// </summary>
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }
    /// <summary>
    /// Gets or sets the risk range (TimeSpan) from the configuration.
    /// </summary>
    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }
    /// <summary>
    /// Resets the configuration values to their default settings.
    /// This includes resetting the Clock and RiskRange properties.
    /// </summary>
    public void Reset()
    {
        Config.Reset();
    }


}
