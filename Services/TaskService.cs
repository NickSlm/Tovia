using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ToDoListPlus.Services
{
    public class TaskService
    {
        private readonly AuthService _authService;
        private readonly HttpClient _httpClient;

        public TaskService(AuthService authService)
        {
            _authService = authService;
            _httpClient = new HttpClient();
        }

        public async Task<string> PostEventAsync(string title, string? description, DateTime? date)
        {
            string url = "https://graph.microsoft.com/v1.0/me/events";

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.AccessToken);

            var eventData = new
            {
                subject = title,
                isReminderOn = true,
                reminderMinutesBeforeStart = 720,
                body = new
                {
                    contentType = "HTML",
                    content = description
                },
                start = new
                {
                    dateTime = date,
                    timeZone = "Israel Standard Time"
                },
                end = new
                {
                    dateTime = date,
                    timeZone = "Israel Standard time"
                }

            };

            var json = JsonSerializer.Serialize(eventData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseBody);
                string eventId = doc.RootElement.GetProperty("id").GetString();
                return eventId;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return $"Failed to create event: {response.StatusCode} - {error}";
            }

        }
        public async Task<string> DeleteEventAsync(string eventId)
        {
            string url = $"https://graph.microsoft.com/v1.0/me/events/{eventId}";

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService. AccessToken);

            var response = await _httpClient.DeleteAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return "Event deleted successfully";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return $"Failed to delete event: {response.StatusCode} - {error}";
            }
        }
    }
}
