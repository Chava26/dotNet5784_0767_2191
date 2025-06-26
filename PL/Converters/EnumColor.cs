using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using BO;

namespace PL.Converters
{
    /// <summary>
    /// Converter for automotive CallType enum to background color
    /// </summary>
    public class CallTypeToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CallType callType)
            {
                return callType switch
                {
                    // General & System
                    CallType.None => new SolidColorBrush(Colors.White),
                    CallType.General => new SolidColorBrush(Color.FromRgb(240, 248, 255)), // AliceBlue - General
                    CallType.CheckEngineLight => new SolidColorBrush(Color.FromRgb(255, 215, 0)), // Gold - Check Engine
                    CallType.SensorMalfunction => new SolidColorBrush(Color.FromRgb(255, 228, 181)), // Moccasin - Sensors
                    CallType.BlownFuse => new SolidColorBrush(Color.FromRgb(255, 222, 173)), // NavajoWhite - Fuses

                    // Engine & Power - Critical (Red tones)
                    CallType.EngineFailure => new SolidColorBrush(Color.FromRgb(255, 99, 71)), // Tomato - Critical Engine
                    CallType.Overheating => new SolidColorBrush(Color.FromRgb(255, 69, 0)), // RedOrange - Overheating
                    CallType.OilLeak => new SolidColorBrush(Color.FromRgb(139, 69, 19)), // SaddleBrown - Oil Leak
                    CallType.CoolantLeak => new SolidColorBrush(Color.FromRgb(70, 130, 180)), // SteelBlue - Coolant
                    CallType.TimingBeltFailure => new SolidColorBrush(Color.FromRgb(205, 92, 92)), // IndianRed - Timing Belt
                    CallType.SparkPlugIssue => new SolidColorBrush(Color.FromRgb(255, 165, 0)), // Orange - Spark Plugs

                    // Battery & Electrical - Yellow/Orange tones
                    CallType.DeadBattery => new SolidColorBrush(Color.FromRgb(255, 255, 0)), // Yellow - Dead Battery
                    CallType.AlternatorFailure => new SolidColorBrush(Color.FromRgb(255, 140, 0)), // DarkOrange - Alternator
                    CallType.StarterMotorFailure => new SolidColorBrush(Color.FromRgb(255, 165, 0)), // Orange - Starter
                    CallType.BatteryCorrosion => new SolidColorBrush(Color.FromRgb(255, 228, 196)), // Bisque - Corrosion
                    CallType.HeadlightFailure => new SolidColorBrush(Color.FromRgb(173, 216, 230)), // LightBlue - Headlights

                    // Fuel System - Pink tones
                    CallType.FuelPumpFailure => new SolidColorBrush(Color.FromRgb(255, 20, 147)), // DeepPink - Fuel Pump
                    CallType.CloggedFuelFilter => new SolidColorBrush(Color.FromRgb(255, 182, 193)), // LightPink - Fuel Filter
                    CallType.ExhaustLeak => new SolidColorBrush(Color.FromRgb(128, 128, 128)), // Gray - Exhaust

                    // Brakes & Safety - Red tones (Critical)
                    CallType.BrakeFailure => new SolidColorBrush(Color.FromRgb(220, 20, 60)), // Crimson - Critical Brakes
                    CallType.WornBrakePads => new SolidColorBrush(Color.FromRgb(255, 192, 203)), // Pink - Worn Pads

                    // Transmission & Drivetrain - Gold tones
                    CallType.TransmissionIssue => new SolidColorBrush(Color.FromRgb(218, 165, 32)), // GoldenRod - Transmission

                    // Steering & Suspension - Purple tones
                    CallType.PowerSteeringFailure => new SolidColorBrush(Color.FromRgb(147, 112, 219)), // MediumPurple - Power Steering
                    CallType.SuspensionProblem => new SolidColorBrush(Color.FromRgb(221, 160, 221)), // Plum - Suspension

                    // Simple Roadside - Green tones
                    CallType.FlatTire => new SolidColorBrush(Color.FromRgb(144, 238, 144)), // LightGreen - Flat Tire
                    CallType.AirConditionerFailure => new SolidColorBrush(Color.FromRgb(175, 238, 238)), // PaleTurquoise - AC

                    _ => new SolidColorBrush(Colors.White) // Default
                };
            }
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter for CallStatus enum to background color
    /// </summary>
    public class CallStatusToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CallStatus status)
            {
                return status switch
                {
                    CallStatus.Open => new SolidColorBrush(Color.FromRgb(144, 238, 144)), // LightGreen - Open
                    CallStatus.OpenRisk => new SolidColorBrush(Color.FromRgb(255, 215, 0)), // Gold - Open Risk
                    CallStatus.InProgress => new SolidColorBrush(Color.FromRgb(135, 206, 235)), // SkyBlue - In Progress
                    CallStatus.InProgressRisk => new SolidColorBrush(Color.FromRgb(255, 140, 0)), // DarkOrange - In Progress Risk
                    CallStatus.Closed => new SolidColorBrush(Color.FromRgb(60, 179, 113)), // MediumSeaGreen - Closed
                    CallStatus.Expired => new SolidColorBrush(Color.FromRgb(220, 20, 60)), // Crimson - Expired
                    _ => new SolidColorBrush(Colors.White) // Default
                };
            }
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter for CallType enum to text color for better readability
    /// </summary>
    public class CallTypeToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CallType callType)
            {
                return callType switch
                {
                    // White text on dark backgrounds for critical issues
                    CallType.EngineFailure => new SolidColorBrush(Colors.White),
                    CallType.Overheating => new SolidColorBrush(Colors.White),
                    CallType.BrakeFailure => new SolidColorBrush(Colors.White),
                    CallType.OilLeak => new SolidColorBrush(Colors.White),
                    CallType.FuelPumpFailure => new SolidColorBrush(Colors.White),
                    CallType.ExhaustLeak => new SolidColorBrush(Colors.White),
                    _ => new SolidColorBrush(Colors.Black) // Black text for light backgrounds
                };
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter for CallStatus enum to text color for better readability
    /// </summary>
    public class CallStatusToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CallStatus status)
            {
                return status switch
                {
                    CallStatus.InProgressRisk => new SolidColorBrush(Colors.White), // White on orange
                    CallStatus.Expired => new SolidColorBrush(Colors.White), // White on crimson
                    _ => new SolidColorBrush(Colors.Black) // Black text for light backgrounds
                };
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    
/// <summary>
/// Converter for CallType severity - returns text indicator based on severity
/// </summary>
public class CallTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CallType callType)
            {
                return callType switch
                {
                    // Critical - Red Alert
                    CallType.EngineFailure => "CRIT",
                    CallType.BrakeFailure => "CRIT",
                    CallType.Overheating => "CRIT",

                    // High Priority - Orange
                    CallType.TransmissionIssue => "HIGH",
                    CallType.PowerSteeringFailure => "HIGH",
                    CallType.FuelPumpFailure => "HIGH",

                    // Medium Priority - Yellow
                    CallType.DeadBattery => "MED",
                    CallType.AlternatorFailure => "MED",
                    CallType.CheckEngineLight => "MED",
                    CallType.StarterMotorFailure => "MED",
                    CallType.SensorMalfunction => "MED",
                    CallType.BlownFuse => "MED",

                    // Low Priority - Green
                    CallType.FlatTire => "LOW",
                    CallType.AirConditionerFailure => "LOW",
                    CallType.HeadlightFailure => "LOW",
                    CallType.WornBrakePads => "LOW",

                    // Maintenance
                    CallType.OilLeak => "MAINT",
                    CallType.CoolantLeak => "MAINT",
                    CallType.ExhaustLeak => "MAINT",
                    CallType.SuspensionProblem => "MAINT",
                    CallType.TimingBeltFailure => "MAINT",
                    CallType.SparkPlugIssue => "MAINT",
                    CallType.BatteryCorrosion => "MAINT",
                    CallType.CloggedFuelFilter => "MAINT",

                    CallType.General => "GEN",
                    _ => "STD"
                };
            }
            return "STD";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}