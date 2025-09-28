using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Windows;
using Tovia.Models;

namespace Tovia.Services
{
    public class MicrosoftGraphService: IMicrosoftGraphService
    {
        private readonly AuthService _authService;
        private readonly HttpClient _httpClient;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MicrosoftGraphService(AuthService authService)
        {
            _authService = authService;
            _httpClient = new HttpClient();
        }

        public ObservableCollection<ToDoItem> ToDoList { get; set; } = new();

        public async Task<ToDoItem> CreateTaskAsync(ToDoItem item, bool createEvent)
        {

            string AccessToken = await _authService.GetAccessToken();

            string url = $"https://graph.microsoft.com/v1.0/me/todo/lists/{_authService.AccountTaskListId}/tasks";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            var taskData = new
            {
                item.Title,
                item.Importance,
                status = "notStarted",
                dueDateTime = item.DueDate.HasValue ? new
                {
                    dateTime = item.DueDate.Value.ToString("o"),
                    timeZone = TimeZoneInfo.Local.Id
                } : null,
                body = new
                {
                    ContentType = "text",
                    Content = item.Description
                }
            };

            var json = JsonSerializer.Serialize(taskData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"Failed creating task");
                return null;
            }

            //Get TaskID
            string responseBody = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseBody);
            string taskId = doc.RootElement.GetProperty("id").GetString();

            item.TaskId = taskId;

            if (createEvent)
            {
                //If we want to create a Task Event in Microsoft outlook

                string eventId = string.Empty;

                string eventResponse = await PostEventAsync(item.Title, item.Description, item.DueDate, item.Importance);
                string linkUrl = $"https://graph.microsoft.com/v1.0/me/todo/lists/{_authService.AccountTaskListId}/tasks/{taskId}/linkedResources";
                using var eventDoc = JsonDocument.Parse(eventResponse);

                eventId = eventDoc.RootElement.GetProperty("id").GetString();
                string eventWebLink = eventDoc.RootElement.GetProperty("webLink").GetString();

                var linkData = new
                {
                    webUrl = eventWebLink,
                    applicationName = "Outlook Calendar",
                    displayName = item.Title,
                    externalId = eventId
                };

                var linkJson = JsonSerializer.Serialize(linkData);
                var linkContent = new StringContent(linkJson, Encoding.UTF8, "application/json");
                var linkResponse = await _httpClient.PostAsync(linkUrl, linkContent);

                item.EventId = eventId;
            }
            return item;
        }
        public async Task DeleteTaskAsync(string taskId)
        {
            string AccessToken = await _authService.GetAccessToken();

            string url = $"https://graph.microsoft.com/v1.0/me/todo/lists/{_authService.AccountTaskListId}/tasks/{taskId}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            var response = await _httpClient.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to delete Task: {response.StatusCode} - {error}");
            }
        }
        public async Task UpdateTaskAsync(string taskId, bool IsComplete)
        {
            string AccessToken = await _authService.GetAccessToken();

            string url = $"https://graph.microsoft.com/v1.0/me/todo/lists/{_authService.AccountTaskListId}/tasks/{taskId}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            var taskData = new
            {
                status = IsComplete ? "completed" : "notStarted"
            };

            var json = JsonSerializer.Serialize(taskData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _httpClient.PatchAsync(url, content);

        }
        public async Task<List<ToDoItem>> GetTasksAsync()
        {
            string AccessToken = await _authService.GetAccessToken();

            var taskList = new List<ToDoItem>();

            string url = $"https://graph.microsoft.com/v1.0/me/todo/lists/{_authService.AccountTaskListId}/tasks?$expand=linkedResources";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            var json = await _httpClient.GetStringAsync(url);
            using var jsonDoc = JsonDocument.Parse(json);

            var rootElement = jsonDoc.RootElement;
            var value = rootElement.GetProperty("value");

            //Iterating over tasks 
            foreach (var jsonElement in value.EnumerateArray())
            {
                var titleJsonElement = jsonElement.GetProperty("title");
                var title = titleJsonElement.GetString();

                var idJsonElement = jsonElement.GetProperty("id");
                var id = idJsonElement.GetString();

                var statusJsonElement = jsonElement.GetProperty("status");
                var status = statusJsonElement.GetString();

                var importanceJsonElement = jsonElement.GetProperty("importance");
                var importance = importanceJsonElement.GetString();

                var descriptionString = string.Empty;

                if (jsonElement.TryGetProperty("body", out var descriptionJsonElement))
                {
                    var description = descriptionJsonElement.GetProperty("content");
                    descriptionString = description.GetString();
                }

                DateTime? dueDateTime = null;
                if (jsonElement.TryGetProperty("dueDateTime", out var dueDateTimeJsonElement))
                {
                    var dateTimeJsonElement = dueDateTimeJsonElement.GetProperty("dateTime");
                    var dateTimeString = dateTimeJsonElement.GetString();
                    dueDateTime = DateTime.TryParse(dateTimeString, out var parsedDateTime) ? parsedDateTime : null;
                }

                //Get externalId from the linkedResource
                string? externalId = string.Empty;
                if (jsonElement.TryGetProperty("linkedResources", out var linkedResourcesElement) 
                    && linkedResourcesElement.ValueKind == JsonValueKind.Array && linkedResourcesElement.GetArrayLength() > 0)
                {
                    externalId = linkedResourcesElement[0].GetProperty("externalId").GetString();
                }

                //Create a new ToDoItem object
                var task = new ToDoItem
                {
                    Title = title,
                    Importance = importance,
                    Description = descriptionString,
                    DueDate = dueDateTime,
                    TaskId = id,
                    EventId = externalId,
                    IsComplete = status == "completed" ? true : false,
                };
                taskList.Add(task);
            }
            return taskList;
        }
        public async Task<string> PostEventAsync(string title, string? description, DateTime? dateTime, string priority)
        {
            string url = "https://graph.microsoft.com/v1.0/me/events";
            string AccessToken = await _authService.GetAccessToken();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

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
                    dateTime,
                    timeZone = "Israel Standard Time"
                },
                end = new
                {
                    dateTime,
                    timeZone = "Israel Standard time"
                }
            };

            var json = JsonSerializer.Serialize(eventData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to create Event: {response.StatusCode} - {error}");
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;

        }
        public async Task DeleteEventAsync(string eventId)
        {
            string url = $"https://graph.microsoft.com/v1.0/me/events/{eventId}";

            string AccessToken = await _authService.GetAccessToken();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            var response = await _httpClient.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to delete event: {response.StatusCode} - {error}");
            }
        }
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
