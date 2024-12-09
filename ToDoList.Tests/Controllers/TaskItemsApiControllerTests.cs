using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.Controllers;
using ToDoList.Data.DatabaseContext;
using ToDoList.Models.Entities;
using Xunit;

namespace ToDoList.Tests.Controllers
{
    /// <summary>
    /// Тестовый класс для контроллера TaskItemsApiController.
    /// </summary>
    public class TaskItemsApiControllerTests
    {
        /// <summary>
        /// Создаёт контекст базы данных с использованием In-Memory провайдера.
        /// </summary>
        private ToDoListDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ToDoListDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Уникальное имя базы
                .Options;
            return new ToDoListDbContext(options);
        }

        [Fact]
        public async Task GetTaskItems_ReturnsAllTasks()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.TaskItems.AddRange(
                new TaskItem
                {
                    Title = "Task 1",
                    Description = "Description 1",
                    EndDate = DateTime.Now.AddDays(1),
                    IsCompleted = false
                },
                new TaskItem
                {
                    Title = "Task 2",
                    Description = "Description 2",
                    EndDate = DateTime.Now.AddDays(2),
                    IsCompleted = true
                }
            );
            await context.SaveChangesAsync();
            var controller = new TaskItemsApiController(context);

            // Act
            var result = await controller.GetTaskItems() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var tasks = result.Value as IEnumerable<TaskItem>;
            Assert.NotNull(tasks);
            Assert.Equal(2, tasks.Count());
        }

        [Fact]
        public async Task GetTaskItem_ValidId_ReturnsTask()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var task = new TaskItem
            {
                Title = "Task 1",
                Description = "Description 1",
                EndDate = DateTime.Now.AddDays(1),
                IsCompleted = false
            };
            context.TaskItems.Add(task);
            await context.SaveChangesAsync();
            var controller = new TaskItemsApiController(context);

            // Act
            var result = await controller.GetTaskItem(task.Id) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var retrievedTask = result.Value as TaskItem;
            Assert.NotNull(retrievedTask);
            Assert.Equal(task.Title, retrievedTask.Title);
        }

        [Fact]
        public async Task CreateTaskItem_ValidTask_ReturnsCreatedTask()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new TaskItemsApiController(context);
            var newTask = new TaskItem
            {
                Title = "New Task",
                Description = "Task Description",
                EndDate = DateTime.Now.AddDays(5),
                IsCompleted = false
            };

            // Act
            var result = await controller.CreateTaskItem(newTask) as CreatedAtActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("GetTaskItem", result.ActionName);
            var createdTask = result.Value as TaskItem;
            Assert.NotNull(createdTask);
            Assert.Equal(newTask.Title, createdTask.Title);
        }

        [Fact]
        public async Task UpdateTaskItem_ValidTask_ReturnsNoContent()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var task = new TaskItem
            {
                Title = "Task 1",
                Description = "Description 1",
                EndDate = DateTime.Now.AddDays(1),
                IsCompleted = false
            };
            context.TaskItems.Add(task);
            await context.SaveChangesAsync();

            // Извлекаем задачу из базы данных
            var controller = new TaskItemsApiController(context);
            var taskToUpdate = await context.TaskItems.FirstAsync();

            // Изменяем свойства
            taskToUpdate.Title = "Updated Task";
            taskToUpdate.Description = "Updated Description";
            taskToUpdate.EndDate = DateTime.Now.AddDays(3);
            taskToUpdate.IsCompleted = true;

            // Act
            var result = await controller.UpdateTaskItem(taskToUpdate.Id, taskToUpdate);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var dbTask = await context.TaskItems.FindAsync(taskToUpdate.Id);
            Assert.Equal("Updated Task", dbTask.Title);
            Assert.True(dbTask.IsCompleted);
        }

        [Fact]
        public async Task DeleteTaskItem_ValidId_RemovesTask()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var task = new TaskItem
            {
                Title = "Task 1",
                Description = "Description 1",
                EndDate = DateTime.Now.AddDays(1),
                IsCompleted = false
            };
            context.TaskItems.Add(task);
            await context.SaveChangesAsync();
            var controller = new TaskItemsApiController(context);

            // Act
            var result = await controller.DeleteTaskItem(task.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await context.TaskItems.FindAsync(task.Id));
        }
    }
}
