using Microsoft.Graph.Models;

namespace Assistant.Desktop.Models;

public class EventModel
{
    public string? Subject { get; set; }
    public DateTimeTimeZone? Start { get; set; }
    public DateTimeTimeZone? End { get; set; }
}