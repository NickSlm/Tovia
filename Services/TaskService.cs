using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

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

        public async Task<string> PostEventAsync(string title, string? description, DateTime? dateTime, string priority)
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
                    dateTime = dateTime,
                    timeZone = "Israel Standard Time"
                },
                end = new
                {
                    dateTime = dateTime,
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
        public async Task<string> CreateTaskAsync(string title, string? description, DateTime? dateTime, string priority)
        {
            string url = $"https://graph.microsoft.com/v1.0/me/todo/lists/{_authService.AccountTaskListId}/tasks";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.AccessToken);

            var taskData = new
            {
                Title = title,
                Importance = priority,
                dueDateTime = dateTime.HasValue ? new
                {
                    dateTime = dateTime.Value.ToString("o"),
                    timeZone = TimeZoneInfo.Local.Id
                } : null,
                body = new
                {
                    ContentType = "text",
                    Content = description
                }

            };


            var json = JsonSerializer.Serialize(taskData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);


            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseBody);
                string taskId = doc.RootElement.GetProperty("id").GetString();
                return taskId;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return $"Failed to create Task: {response.StatusCode} - {error}";
            }
        }
        public async Task<string> DeleteTaskAsync(string taskId)
        {
            string url = $"https://graph.microsoft.com/v1.0/me/todo/lists/{_authService.AccountTaskListId}/tasks/{taskId}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.AccessToken);

            var response = await _httpClient.DeleteAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return "Task deleted successfully";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return $"Failed to delete task: {response.StatusCode} - {error}";
            }

        }
        public async Task<List<ToDoItem>> GetTasksAsync()
        {

            var taskList = new List<ToDoItem>();

            string url = $"https://graph.microsoft.com/v1.0/me/todo/lists/{_authService.AccountTaskListId}/tasks";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.AccessToken);

            var json = await _httpClient.GetStringAsync(url);
            using var jsonDoc = JsonDocument.Parse(json);

            var rootElement = jsonDoc.RootElement;
            var value = rootElement.GetProperty("value");

            foreach (var jsonElement in value.EnumerateArray())
            {
                var titleJsonElement = jsonElement.GetProperty("title");
                var title = titleJsonElement.GetString();

                var importanceJsonElement = jsonElement.GetProperty("importance");
                var importance = importanceJsonElement.GetString();

                DateTime? dueDateTime = null;
                if (jsonElement.TryGetProperty("dueDateTime", out var dueDateTimeJsonElement))
                {
                    var dateTimeJsonElement = dueDateTimeJsonElement.GetProperty("dateTime");
                    var dateTimeString = dateTimeJsonElement.GetString();
                    dueDateTime = DateTime.TryParse(dateTimeString, out var parsedDateTime) ? parsedDateTime : (DateTime?)null;
                }

                var task = new ToDoItem
                {
                    Title = title,
                    Importance = importance,
                    DueDate = dueDateTime
                };

                taskList.Add(task);

            }

            return taskList;
        }
    }
}
