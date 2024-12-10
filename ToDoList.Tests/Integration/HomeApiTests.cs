using System.Net;
using System.Net.Http.Json;
using ToDoList.Models;
using ToDoList.Models.DataTransferObject;
using ToDoList.Models.Entities;
using Xunit;

namespace ToDoList.Tests
{
    public class HomeApiTests
    {
        private readonly HttpClient _client;

        // Конструктор для инициализации HttpClient с базовым адресом API
        public HomeApiTests()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:8080")
            };
        }

        // Тестирование получения статистики по задачам
        [Fact]
        public async Task GetStatistics_ShouldReturnSuccessStatusCode()
        {
            // Act: Отправляем GET-запрос для получения статистики по задачам
            var response = await _client.GetAsync("/api/HomeApi/statistics");
            var statistics = await response.Content.ReadFromJsonAsync<Statistics>();

            // Assert: Проверяем, что ответ успешный
            Assert.Equal(HttpStatusCode.OK, response.StatusCode); // Статус должен быть OK
            Assert.NotNull(statistics); // Статистика не должна быть null
            Assert.True(statistics.TotalTasks >= 0); // Общее количество задач не может быть отрицательным
            Assert.True(statistics.CompletedTasks >= 0); // Количество выполненных задач не может быть отрицательным
            Assert.True(statistics.NotCompletedTasks >= 0); // Количество невыполненных задач не может быть отрицательным
        }
    }
}
