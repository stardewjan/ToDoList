﻿using System.Net;
using System.Net.Http.Json;
using ToDoList.Models.Entities;
using Xunit;

namespace ToDoList.Tests
{
    public class TaskItemsApiTests
    {
        private readonly HttpClient _client;

        // Конструктор для инициализации HttpClient с базовым адресом API
        public TaskItemsApiTests()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:8080")
            };
        }

        // Тестирование получения всех задач
        [Fact]
        public async Task GetTaskItems_ShouldReturnSuccessStatusCode()
        {
            // Act: Отправляем GET-запрос для получения всех задач
            var response = await _client.GetAsync("/api/TaskItemsApi");
            var tasks = await response.Content.ReadFromJsonAsync<List<TaskItem>>();

            // Assert: Проверяем, что ответ успешный и что задачи не пусты
            Assert.Equal(HttpStatusCode.OK, response.StatusCode); // Статус должен быть OK
            Assert.NotNull(tasks); // Список задач не должен быть null
            Assert.NotEmpty(tasks); // Список задач не должен быть пустым
        }

        // Тестирование получения задачи по ID
        [Fact]
        public async Task GetTaskItem_WithValidId_ShouldReturnTask()
        {
            // Arrange: Создаем тестовую задачу для проверки
            var newTask = await CreateTestTask();

            // Act: Отправляем GET-запрос для получения задачи по ID
            var response = await _client.GetAsync($"/api/TaskItemsApi/{newTask.Id}");
            var task = await response.Content.ReadFromJsonAsync<TaskItem>();

            // Assert: Проверяем, что задача успешно возвращена
            Assert.Equal(HttpStatusCode.OK, response.StatusCode); // Статус должен быть OK
            Assert.NotNull(task); // Задача не должна быть null
            Assert.Equal(newTask.Title, task.Title); // Заголовок задачи должен совпасть с созданным
        }

        // Тестирование создания задачи с правильными данными
        [Fact]
        public async Task CreateTaskItem_WithValidData_ShouldReturnCreated()
        {
            // Arrange: Создаем новый объект задачи с данными
            var newTask = new TaskItem
            {
                Title = "Test Task",
                Description = "Test Description",
                //EndDate = DateTime.Now.AddDays(1),
                IsCompleted = false
            };

            // Act: Отправляем POST-запрос для создания задачи
            var response = await _client.PostAsJsonAsync("/api/TaskItemsApi", newTask);
            var createdTask = await response.Content.ReadFromJsonAsync<TaskItem>();

            // Assert: Проверяем, что задача была успешно создана
            Assert.Equal(HttpStatusCode.Created, response.StatusCode); // Статус должен быть Created
            Assert.NotNull(createdTask); // Задача не должна быть null
            Assert.Equal(newTask.Title, createdTask.Title); // Заголовок задачи должен совпасть
            Assert.True(createdTask.Id > 0); // У задачи должен быть задан ID
        }

        // Тестирование обновления задачи с правильными данными
        [Fact]
        public async Task UpdateTaskItem_WithValidData_ShouldReturnNoContent()
        {
            // Arrange: Создаем тестовую задачу и изменяем её данные
            var task = await CreateTestTask();
            task.Title = "Updated Title";
            task.Description = "Updated Description";

            // Act: Отправляем PUT-запрос для обновления задачи
            var response = await _client.PutAsJsonAsync($"/api/TaskItemsApi/{task.Id}", task);

            // Assert: Проверяем, что задача была успешно обновлена
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode); // Статус должен быть NoContent

            // Дополнительная проверка, что изменения сохранились
            var updatedResponse = await _client.GetAsync($"/api/TaskItemsApi/{task.Id}");
            var updatedTask = await updatedResponse.Content.ReadFromJsonAsync<TaskItem>();
            Assert.Equal("Updated Title", updatedTask.Title); // Проверяем обновленный заголовок
        }

        // Тестирование удаления задачи по ID
        [Fact]
        public async Task DeleteTaskItem_WithValidId_ShouldReturnNoContent()
        {
            // Arrange: Создаем тестовую задачу для удаления
            var task = await CreateTestTask();

            // Act: Отправляем DELETE-запрос для удаления задачи
            var response = await _client.DeleteAsync($"/api/TaskItemsApi/{task.Id}");

            // Assert: Проверяем, что задача была успешно удалена
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode); // Статус должен быть NoContent

            // Дополнительная проверка, что задача действительно удалена
            var getResponse = await _client.GetAsync($"/api/TaskItemsApi/{task.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode); // Статус должен быть NotFound
        }

        // Вспомогательный метод для создания тестовой задачи
        private async Task<TaskItem> CreateTestTask()
        {
            var newTask = new TaskItem
            {
                Title = "Test Task",
                Description = "Test Description",
                //EndDate = DateTime.Now.AddDays(1),
                IsCompleted = false
            };

            var response = await _client.PostAsJsonAsync("/api/TaskItemsApi", newTask);
            return await response.Content.ReadFromJsonAsync<TaskItem>();
        }

        // Тестирование завершения задачи
        [Fact]
        public async Task CompleteTask_WithValidId_ShouldReturnUpdatedTask()
        {
            // Arrange: Создаем тестовую задачу для завершения
            var newTask = await CreateTestTask();

            // Act: Отправляем POST-запрос для завершения задачи
            var response = await _client.PostAsync($"/api/TaskItemsApi/{newTask.Id}/complete", null);
            var updatedTask = await response.Content.ReadFromJsonAsync<TaskItem>();

            // Assert: Проверяем, что задача была успешно завершена
            Assert.Equal(HttpStatusCode.OK, response.StatusCode); // Статус должен быть OK
            Assert.NotNull(updatedTask); // Задача не должна быть null
            Assert.True(updatedTask.IsCompleted); // Статус задачи должен быть "Выполнено"
            Assert.NotNull(updatedTask.EndDate); // Дата завершения должна быть установлена
        }

    }
}
