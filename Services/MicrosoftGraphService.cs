using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Windows;
using ToDoListPlus.Models;

namespace ToDoListPlus.Services
{
    public class MicrosoftGraphService: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly AuthService _authService;
        private readonly HttpClient _httpClient;

        public ObservableCollection<ToDoItem> ToDoList { get; set; } = new();

        public MicrosoftGraphService(AuthService authService)
        {
            _authService = authService;
            _httpClient = new HttpClient();
        }

        public async Task<ToDoItem> CreateTaskAsync(ToDoItem item, bool createEvent)
        {
            string AccessToken = await _authService.GetAccessToken();

            string url = $"https://graph.microsoft.com/v1.0/me/todo/lists/{_authService.AccountTaskListId}/tasks";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            var taskData = new
            {
                Title = item.Title,
                Importance = item.Importance,
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


            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseBody);
                string taskId = doc.RootElement.GetProperty("id").GetString();
                string eventId = string.Empty;

                if (createEvent)
                {
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
                }

                item.EventId = eventId;
                item.TaskId = taskId;

                return item;
            }
            else
            {
                return null;
            }
        }
        public async Task<string> DeleteTaskAsync(string taskId)
        {
            string AccessToken = await _authService.GetAccessToken();

            string url = $"https://graph.microsoft.com/v1.0/me/todo/lists/{_authService.AccountTaskListId}/tasks/{taskId}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

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
        public async Task<string> UpdateTaskAsync(string taskId, bool IsComplete)
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
            var response = await _httpClient.PatchAsync(url, content);
            return "Update Completed";

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
                    dueDateTime = DateTime.TryParse(dateTimeString, out var parsedDateTime) ? parsedDateTime : (DateTime?)null;
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
                return responseBody;
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

            string AccessToken = await _authService.GetAccessToken();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

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

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
