using Xunit;
using Moq;
using Moq.Protected;
using Tovia.States;
using Tovia.Models;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Tovia.interfaces;


namespace Tovia.Tests
{
    public class TaskManagerTests
    {

        public readonly Mock<IMicrosoftGraphService> _mockGraphService;
        public readonly TaskManager _taskManager;

        public TaskManagerTests()
        {
            _mockGraphService = new Mock<IMicrosoftGraphService>();
            _taskManager = new TaskManager(_mockGraphService.Object);
        }

        public static IEnumerable<object[]> ToDoItemData => new List<object[]>
            {
            new object[] {new ToDoItem {
                Title = "Task1",
                Description = "Task1 Description",
                DueDate = DateTime.Now,
                Importance = "normal",
                IsComplete = true},
                true
            },
            new object[] {new ToDoItem {
                Title = "Task2",
                Description = "Task2 Description",
                DueDate = DateTime.Now,
                Importance = "low",
                IsComplete = false},
                false
            },
            new object[] {new ToDoItem {
                Title = "Task3",
                Description = "Task3 Description",
                DueDate = DateTime.Now,
                Importance = "high",
                IsComplete = true},
                true
            },
            };


        [Fact]
        public async Task LoadTasks()
        {
            var returnedItemList = new List<ToDoItem>()
            {
                new ToDoItem
                {
                    Title = "Task 1",
                    Description = "Task 1 Description",
                    DueDate = DateTime.Now,
                    Importance = "high",
                    TaskId = "Task 1 ID",
                    EventId = "Task 1 EventID",
                    IsComplete = true
                },
                new ToDoItem
                {
                    Title = "Task 2",
                    Description = "Task 2 Description",
                    DueDate = DateTime.Now,
                    Importance = "high",
                    TaskId = "Task 2 ID",
                    EventId = "Task 2 EventID",
                    IsComplete = false
                },
                new ToDoItem
                {
                    Title = "Task 3",
                    Description = "Task 3 Description",
                    DueDate = DateTime.Now,
                    Importance = "high",
                    TaskId = "Task 3 ID",
                    EventId = "Task 3 EventID",
                    IsComplete = true
                }
            };

            _mockGraphService.Setup(s => s.GetTasksAsync())
                .ReturnsAsync(returnedItemList);

            foreach (var task in returnedItemList)
            {
                task.OnCompletionChanged?.Invoke(task, EventArgs.Empty);
            }

            _mockGraphService.Verify(s => s.UpdateTaskAsync(It.IsAny<string>(), It.IsAny<bool>()), Times.Exactly(returnedItemList.Count));

        }

        [Theory]
        [MemberData(nameof(ToDoItemData))]
        public async Task SaveTask_WithValidInput(ToDoItem item, bool createEvent)
        {
            var returnedTask = item;
            returnedTask.TaskId = "taskID";
            if (createEvent)
            {
                returnedTask.EventId = "eventID";
            }
            _mockGraphService.Setup(s => s.CreateTaskAsync(item, createEvent)).ReturnsAsync(returnedTask);

            await _taskManager.SaveTask(item, createEvent);

            Assert.Single(_taskManager.ToDoList);
        }

        [Fact]
        public async Task DeleteTask_WithEventId_DeletesEventAndTask()
        {
            ToDoItem item = new ToDoItem
            {
                Title = "Task 1",
                Description = "Task 1 Description",
                DueDate = DateTime.Now,
                Importance = "normal",
                IsComplete = true,
                EventId = "a",
                TaskId = "a"
            };

            _taskManager.ToDoListInternal.Add(item);

            await _taskManager.RemoveTask(item);

            _mockGraphService.Verify(s => s.DeleteTaskAsync("a"), Times.Once);
            _mockGraphService.Verify(s => s.DeleteEventAsync("a"), Times.Once);

            Assert.DoesNotContain(item, _taskManager.ToDoListInternal);

        }

        [Fact]
        public async Task DeleteTask_WithoutEvent_DeleteTask()
        {
            ToDoItem item = new ToDoItem
            {
                Title = "Task 1",
                Description = "Task 1 Description",
                DueDate = DateTime.Now,
                Importance = "normal",
                IsComplete = true,
                TaskId = "a"
            };

            _taskManager.ToDoListInternal.Add(item);

            await _taskManager.RemoveTask(item);

            _mockGraphService.Verify(s => s.DeleteTaskAsync("a"), Times.Once);
            _mockGraphService.Verify(s => s.DeleteEventAsync("a"), Times.Never);

            Assert.DoesNotContain(item, _taskManager.ToDoListInternal);

        }
    }
}
