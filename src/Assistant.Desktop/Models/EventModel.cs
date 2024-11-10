using Microsoft.Graph.Models;

namespace Assistant.Desktop.Models;

public class EventModel
{
    public string? Subject { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
}