using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Windows;
using Tovia.interfaces;
using Tovia.Models;

namespace Tovia.Services
{
    public class MicrosoftGraphService: ITaskProvider
    {
        private readonly AppStateService _appStateService;
        private readonly HttpClient _httpClient = new HttpClient()
        {
            BaseAddress = new Uri("https://graph.microsoft.com")

        };

        public event PropertyChangedEventHandler? PropertyChanged;

        public MicrosoftGraphService(AppStateService appStateService)
        {
            _appStateService = appStateService; 
        }
        public ObservableCollection<ToDoItem> ToDoList { get; set; } = new();

        public async Task<ToDoItem> CreateTaskAsync(ToDoItem item, bool createEvent)
        {
            var TaskListId = _appStateService.User.TaskListId;
            var AccessToken = _appStateService.AccessToken;

            var request = new HttpRequestMessage(HttpMethod.Post, $"/v1.0/me/todo/lists/{TaskListId}/tasks");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

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

            request.Content = content;
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"Failed creating task");
                return null;
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseBody);
            string taskId = doc.RootElement.GetProperty("id").GetString();

            item.TaskId = taskId;

            if (createEvent)
            {
                string eventResponse = await PostEventAsync(item.Title, item.Description, item.DueDate, item.Importance);
                using var eventDoc = JsonDocument.Parse(eventResponse);

                var eventId = eventDoc.RootElement.GetProperty("id").GetString();
                string eventWebLink = eventDoc.RootElement.GetProperty("webLink").GetString();

                await CreateLinkedResourcesAsync(item.Title, eventId, taskId, eventWebLink);

                item.EventId = eventId;
            }
            return item;
        }
        private async Task CreateLinkedResourcesAsync(string title, string eventId, string taskId, string eventWebLink)
        {
            var TaskListId = _appStateService.User.TaskListId;
            var AccessToken = _appStateService.AccessToken;

            var request = new HttpRequestMessage(HttpMethod.Post, $"https://graph.microsoft.com/v1.0/me/todo/lists/{TaskListId}/tasks/{taskId}/linkedResources");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            var data = new
            {
                webUrl = eventWebLink,
                applicationName = "Outlook Calendar",
                displayName = title,
                externalId = eventId
            };

            var json = JsonSerializer.Serialize(data);
            var content= new StringContent(json, Encoding.UTF8, "application/json");

            request.Content = content;
            var response = await _httpClient.SendAsync(request);

        }
        public async Task DeleteTaskAsync(string taskId)
        {
            var TaskListId = _appStateService.User.TaskListId;
            var AccessToken = _appStateService.AccessToken;

            var request = new HttpRequestMessage(HttpMethod.Delete, $"v1.0/me/todo/lists/{TaskListId}/tasks/{taskId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to delete Task: {response.StatusCode} - {error}");
            }
        }
        public async Task UpdateTaskAsync(string taskId, bool IsComplete)
        {
            var TaskListId = _appStateService.User.TaskListId;
            var AccessToken = _appStateService.AccessToken;


            var request = new HttpRequestMessage(HttpMethod.Patch, $"v1.0/me/todo/lists/{TaskListId}/tasks/{taskId}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            var taskData = new
            {
                status = IsComplete ? "completed" : "notStarted"
            };

            var json = JsonSerializer.Serialize(taskData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            request.Content = content;
            await _httpClient.SendAsync(request);

        }
        public async Task<List<ToDoItem>> GetTasksAsync()
        {
            MessageBox.Show("micro get");
            var TaskListId = _appStateService.User.TaskListId;
            var AccessToken = _appStateService.AccessToken;

            var taskList = new List<ToDoItem>();

            var request = new HttpRequestMessage(HttpMethod.Get, $"v1.0/me/todo/lists/{TaskListId}/tasks?$expand=linkedResources");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
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
        private async Task<string> PostEventAsync(string title, string? description, DateTime? dateTime, string priority)
        {
            var AccessToken = _appStateService.AccessToken;


            var request = new HttpRequestMessage(HttpMethod.Post, $"v1.0/me/events");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

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
            request.Content = content;
            var response = await _httpClient.SendAsync(request);

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
            var AccessToken = _appStateService.AccessToken;

            var request = new HttpRequestMessage(HttpMethod.Delete, $"v1.0/me/events/{eventId}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to delete event: {response.StatusCode} - {error}");
            }
        }
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
