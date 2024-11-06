using System.ComponentModel;

using Assistant.Desktop.Models;
using Assistant.Desktop.Services;

using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.SemanticKernel;

namespace Assistant.Desktop.Plugins;

public class GraphPlugin
{
    private readonly IGraphService _graphService;

    public GraphPlugin(IGraphService graphService)
    {
        _graphService = graphService;
    }
    
    [KernelFunction(name:"get_me")]
    [Description("Get information about the current user")]
    [return:Description("Information about the current user")]
    public async Task<UserModel?> GetMeAsync()
    {
        GraphServiceClient graphClient =  _graphService.GetGraphClient();
        User? user = await graphClient.Me.GetAsync();
        if(user != null)
        {
            return new UserModel()
            {
                DisplayName = user.DisplayName,
                Department = user.Department,
                AboutMe = user.AboutMe,
                JobTitle = user.JobTitle,
            };
        }
        return null;
    }
    
    [KernelFunction(name:"get_calendar_event_count")]
    [Description("Get information about the current user's calendar")]
    [return:Description("Information about the current user's calendar")]
    public async Task<int> GetMyCalendar(DateTime startDateTime, DateTime endDateTime)
    {
        GraphServiceClient graphClient =  _graphService.GetGraphClient();

        EventCollectionResponse? events = await graphClient.Me
            .CalendarView
            .GetAsync(options =>
            {
                options.QueryParameters.StartDateTime = startDateTime.ToString("o");
                options.QueryParameters.EndDateTime = endDateTime.ToString("o");
            });
        
        return events?.Value?.Count ?? 0;
    }
}