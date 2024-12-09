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
    /// Использует In-Memory базу данных для тестирования.
    /// </summary>
    public class TaskItemsApiControllerTests
    {
        /// <summary>
        /// Создаёт контекст базы данных с использованием In-Memory провайдера.
        /// Это позволяет тестировать контроллер без использования реальной базы данных.
        /// </summary>
        private ToDoListDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ToDoListDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            return new ToDoListDbContext(options);
        }

        /// <summary>
        /// Тест для метода GetTaskItems.
        /// Проверяет, что метод возвращает все задачи, находящиеся в базе данных.
        /// </summary>
        [Fact]
        public async Task GetTaskItems_ReturnsAllTasks()
        {
            // Arrange: Создаём базу данных и добавляем тестовые данные.
            var context = GetInMemoryDbContext();
            context.TaskItems.AddRange(
                new TaskItem
                {
                    Id = 1,
                    Title = "Task 1",
                    Description = "Description 1",
                    EndDate = DateTime.Now.AddDays(1),
                    IsCompleted = false
                },
                new TaskItem
                {
                    Id = 2,
                    Title = "Task 2",
                    Description = "Description 2",
                    EndDate = DateTime.Now.AddDays(2),
                    IsCompleted = true
                }
            );
            await context.SaveChangesAsync();
            var controller = new TaskItemsApiController(context);

            // Act: Вызываем метод контроллера.
            var result = await controller.GetTaskItems() as OkObjectResult;

            // Assert: Проверяем результат.
            Assert.NotNull(result); // Проверяем, что результат не null.
            var tasks = result.Value as IEnumerable<TaskItem>;
            Assert.NotNull(tasks); // Проверяем, что возвращённый список задач не null.
            Assert.Equal(2, tasks.Count()); // Проверяем, что в списке две задачи.
        }

        /// <summary>
        /// Тест для метода GetTaskItem.
        /// Проверяет, что метод возвращает задачу по указанному ID.
        /// </summary>
        [Fact]
        public async Task GetTaskItem_ValidId_ReturnsTask()
        {
            // Arrange: Создаём базу данных и добавляем задачу.
            var context = GetInMemoryDbContext();
            var task = new TaskItem
            {
                Id = 1,
                Title = "Task 1",
                Description = "Description 1",
                EndDate = DateTime.Now.AddDays(1),
                IsCompleted = false
            };
            context.TaskItems.Add(task);
            await context.SaveChangesAsync();
            var controller = new TaskItemsApiController(context);

            // Act: Вызываем метод контроллера с валидным ID.
            var result = await controller.GetTaskItem(1) as OkObjectResult;

            // Assert: Проверяем результат.
            Assert.NotNull(result); // Проверяем, что результат не null.
            var retrievedTask = result.Value as TaskItem;
            Assert.NotNull(retrievedTask); // Проверяем, что задача найдена.
            Assert.Equal(task.Title, retrievedTask.Title); // Проверяем соответствие заголовка.
            Assert.Equal(task.Description, retrievedTask.Description); // Проверяем описание.
            Assert.Equal(task.EndDate, retrievedTask.EndDate); // Проверяем дату окончания.
            Assert.Equal(task.IsCompleted, retrievedTask.IsCompleted); // Проверяем статус выполнения.
        }

        /// <summary>
        /// Тест для метода CreateTaskItem.
        /// Проверяет, что метод создаёт новую задачу и возвращает корректный результат.
        /// </summary>
        [Fact]
        public async Task CreateTaskItem_ValidTask_ReturnsCreatedTask()
        {
            // Arrange: Создаём базу данных и контроллер.
            var context = GetInMemoryDbContext();
            var controller = new TaskItemsApiController(context);

            // Создаём новую задачу для добавления.
            var newTask = new TaskItem
            {
                Title = "New Task",
                Description = "Task Description",
                EndDate = DateTime.Now.AddDays(5),
                IsCompleted = false
            };

            // Act: Вызываем метод CreateTaskItem.
            var result = await controller.CreateTaskItem(newTask) as CreatedAtActionResult;

            // Assert: Проверяем результат.
            Assert.NotNull(result); // Проверяем, что результат не null.
            Assert.Equal("GetTaskItem", result.ActionName); // Проверяем имя метода для CreatedAtAction.
            Assert.NotNull(result.Value); // Проверяем, что задача возвращается в ответе.
            var createdTask = result.Value as TaskItem;
            Assert.Equal(newTask.Title, createdTask.Title); // Проверяем соответствие заголовка.
            Assert.Equal(newTask.Description, createdTask.Description); // Проверяем описание.
            Assert.Equal(newTask.EndDate, createdTask.EndDate); // Проверяем дату окончания.
            Assert.Equal(newTask.IsCompleted, createdTask.IsCompleted); // Проверяем статус выполнения.
        }

        /// <summary>
        /// Тест для метода UpdateTaskItem.
        /// Проверяет, что метод обновляет существующую задачу.
        /// </summary>
        [Fact]
        public async Task UpdateTaskItem_ValidTask_ReturnsNoContent()
        {
            // Arrange: Создаём базу данных и добавляем задачу.
            var context = GetInMemoryDbContext();
            var task = new TaskItem
            {
                Id = 1,
                Title = "Task 1",
                Description = "Description 1",
                EndDate = DateTime.Now.AddDays(1),
                IsCompleted = false
            };
            context.TaskItems.Add(task);
            await context.SaveChangesAsync();

            // Обновляемые данные задачи.
            var updatedTask = new TaskItem
            {
                Id = 1,
                Title = "Updated Task",
                Description = "Updated Description",
                EndDate = DateTime.Now.AddDays(3),
                IsCompleted = true
            };

            var controller = new TaskItemsApiController(context);

            // Act: Вызываем метод UpdateTaskItem.
            var result = await controller.UpdateTaskItem(1, updatedTask);

            // Assert: Проверяем результат.
            Assert.IsType<NoContentResult>(result); // Метод должен возвращать NoContent.
            var dbTask = await context.TaskItems.FindAsync(1); // Проверяем обновление в базе данных.
            Assert.Equal(updatedTask.Title, dbTask.Title);
            Assert.Equal(updatedTask.Description, dbTask.Description);
            Assert.Equal(updatedTask.EndDate, dbTask.EndDate);
            Assert.Equal(updatedTask.IsCompleted, dbTask.IsCompleted);
        }

        /// <summary>
        /// Тест для метода DeleteTaskItem.
        /// Проверяет, что метод удаляет задачу по указанному ID.
        /// </summary>
        [Fact]
        public async Task DeleteTaskItem_ValidId_RemovesTask()
        {
            // Arrange: Создаём базу данных и добавляем задачу.
            var context = GetInMemoryDbContext();
            var task = new TaskItem
            {
                Id = 1,
                Title = "Task 1",
                Description = "Description 1",
                EndDate = DateTime.Now.AddDays(1),
                IsCompleted = false
            };
            context.TaskItems.Add(task);
            await context.SaveChangesAsync();
            var controller = new TaskItemsApiController(context);

            // Act: Вызываем метод DeleteTaskItem.
            var result = await controller.DeleteTaskItem(1);

            // Assert: Проверяем результат.
            Assert.IsType<NoContentResult>(result); // Метод должен возвращать NoContent.
            Assert.Null(await context.TaskItems.FindAsync(1)); // Проверяем, что задача удалена из базы данных.
        }
    }
}
