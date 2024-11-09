using System.ComponentModel;

using Microsoft.Graph.Models;
using Microsoft.SemanticKernel;

namespace Assistant.Desktop.Plugins;

public class DateTimePlugin
{
    [KernelFunction(name: "get_current_datetime")]
    [Description("Get current datetime in utc")]
    [return: Description("Current datetime in utc")]
    public DateTime GetCurrentDateTime()
    {
        return DateTime.UtcNow;
    }
    
    [KernelFunction(name: "get_current_timezone")]
    [Description("Get current timezone")]
    [return: Description("Current timezone")]
    public string GetCurrentTimezone()
    {
        return TimeZoneInfo.Local.DisplayName;
    }
    
    [KernelFunction(name: "get_duration_in_minutes")]
    [Description("Get duration between start and end datetime in minutes")]
    [return: Description("Duration between start and end datetime in minutes")]
    public double GetDurationInMinutes(DateTimeTimeZone start, DateTimeTimeZone end)
    {
        if (start.DateTime == null || end.DateTime == null)
        {
            return 0;
        }
        
        DateTime startDateTime = DateTime.Parse(start.DateTime);
        DateTime endDateTime = DateTime.Parse(end.DateTime);
        TimeSpan duration = endDateTime - startDateTime;
        return duration.TotalMinutes;
    }
}