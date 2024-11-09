﻿using System.ComponentModel;

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
    public async Task<List<EventModel>> GetMyCalendar(DateTime startDateTime, DateTime endDateTime)
    {
        GraphServiceClient graphClient =  _graphService.GetGraphClient();

        EventCollectionResponse? events = await graphClient.Me
            .CalendarView
            .GetAsync(options =>
            {
                options.QueryParameters.StartDateTime = startDateTime.ToString("o");
                options.QueryParameters.EndDateTime = endDateTime.ToString("o");
            });

        List<EventModel> eventModels = [];
        if (events?.Value?.Count > 0)
        {
            foreach (var @event in events.Value)
            {
                eventModels.Add(new EventModel()
                {
                    Subject = @event.Subject,
                    Start = @event.Start,
                    End = @event.End
                });
            }
        }

        return eventModels;
    }
    
    [KernelFunction(name:"set_reminder")]
    [Description("Set a reminder for an event")]
    [return:Description("True if the reminder was set successfully, false otherwise")]
    public async Task<bool> SetReminderAsync(string subject, DateTime startDateTime, int durationInMinutes = 15, int reminderMinutesBeforeStart = 15)
    {
        GraphServiceClient graphClient = _graphService.GetGraphClient();

        var @event = new Event
        {
            Subject = subject,
            Start = new DateTimeTimeZone
            {
                DateTime = startDateTime.ToString("o"),
                TimeZone = "UTC"
            },
            End = new DateTimeTimeZone
            {
                DateTime = startDateTime.AddMinutes(durationInMinutes).ToString("o"),
                TimeZone = "UTC"
            },
            ReminderMinutesBeforeStart = reminderMinutesBeforeStart
        };

        try
        {
            await graphClient.Me.Events.PostAsync(@event);
            return true;
        }
        catch (Exception ex)
        {
            // Handle exception
            Console.WriteLine($"Error setting reminder: {ex.Message}");
            return false;
        }
    }
}

public class EventModel
{
    public string? Subject { get; set; }
    public DateTimeTimeZone? Start { get; set; }
    public DateTimeTimeZone? End { get; set; }
}