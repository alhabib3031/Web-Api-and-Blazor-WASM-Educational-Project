using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.DTOs;
using TaskFlow.Entity;
using TaskFlow.Services.Interfaces;

namespace TaskFlow.Services;

public class GoogleCalendarService : IGoogleCalendarService
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _configuration;

    public GoogleCalendarService(ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    private async Task<UserCredential?> GetUserCredentialAsync(int userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user is null || string.IsNullOrEmpty(user.GoogleAccessToken))
            return null;

        var clientId = _configuration["Authentication:Google:ClientId"];
        var clientSecret = _configuration["Authentication:Google:ClientSecret"];

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            return null;

        var initializer = new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets { ClientId = clientId, ClientSecret = clientSecret },
            Scopes = ["https://www.googleapis.com/auth/calendar.events"],
        };

        var flow = new GoogleAuthorizationCodeFlow(initializer);

        var token = new TokenResponse
        {
            AccessToken = user.GoogleAccessToken,
            RefreshToken = user.GoogleRefreshToken,
        };

        var credential = new UserCredential(flow, "me", token);

        if (!string.IsNullOrEmpty(user.GoogleRefreshToken))
        {
            try
            {
                if (await credential.RefreshTokenAsync(CancellationToken.None))
                {
                    user.GoogleAccessToken = credential.Token.AccessToken;
                    if (credential.Token.RefreshToken is not null)
                        user.GoogleRefreshToken = credential.Token.RefreshToken;
                    await _db.SaveChangesAsync(CancellationToken.None);
                }
            }
            catch
            {
                // Token refresh failed, will use existing token (may fail later)
            }
        }

        return credential;
    }

    public async Task<List<CalendarEventDto>> GetUpcomingEventsAsync(int userId)
    {
        try
        {
            var credential = await GetUserCredentialAsync(userId);
            if (credential is null)
                return [];

            var calendarService = new CalendarService(
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "TaskFlow",
                }
            );

            var request = calendarService.Events.List("primary");
            request.TimeMinDateTimeOffset = DateTimeOffset.UtcNow;
            request.MaxResults = 10;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            var events = await request.ExecuteAsync();

            return
            [
                .. events.Items.Select(e => new CalendarEventDto
                {
                    Title = e.Summary ?? "(Without Title)",
                    StartTime =
                        e.Start.DateTimeDateTimeOffset
                        ?? (
                            e.Start.Date is not null
                                ? DateTimeOffset.Parse(e.Start.Date)
                                : DateTimeOffset.UtcNow
                        ),
                }),
            ];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GoogleCalendarService.GetUpcomingEventsAsync] {ex.Message}");
            return [];
        }
    }

    public async Task<(bool Success, string Message)> AddEventAsync(
        int userId,
        string title,
        string description,
        DateTimeOffset start,
        DateTimeOffset end
    )
    {
        try
        {
            var credential = await GetUserCredentialAsync(userId);
            if (credential is null)
                return (false, "You Have To Loging Using Google Account First!");

            var calendarService = new CalendarService(
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "TaskFlow",
                }
            );

            var googleEvent = new Event
            {
                Summary = title,
                Description = description,
                Start = new EventDateTime { DateTimeDateTimeOffset = start.ToUniversalTime() },
                End = new EventDateTime { DateTimeDateTimeOffset = end.ToUniversalTime() },
                Reminders = new Event.RemindersData
                {
                    UseDefault = false,
                    Overrides = [new EventReminder { Method = "email", Minutes = 0 }],
                },
            };

            await calendarService.Events.Insert(googleEvent, "primary").ExecuteAsync();
            return (true, "Synced to Google Calendar successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GoogleCalendarService.AddEventAsync] {ex.Message}");
            return (false, $"Failed to sync to Google Calendar: {ex.Message}");
        }
    }
}
