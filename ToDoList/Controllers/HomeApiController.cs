using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using ToDoList.Data.DatabaseContext;
using ToDoList.Models;
using ToDoList.Models.DataTransferObject;

namespace ToDoList.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Контроллер для статистики задач")]
    public class HomeApiController : ControllerBase
    {
        private readonly ToDoListDbContext _db;

        public HomeApiController(ToDoListDbContext context)
        {
            _db = context;
        }

        /// <summary>
        /// Получить статистику по задачам.
        /// </summary>
        /// <returns>Общую статистику по задачам.</returns>
        [HttpGet("statistics")]
        [SwaggerOperation(Summary = "Получить статистику по задачам", Description = "Возвращает статистику по задачам, включая общее количество задач, выполненные и невыполненные задачи.")]
        [SwaggerResponse(200, "Статистика успешно получена.", typeof(Statistics))]
        public async Task<IActionResult> GetStatistics()
        {
            var totalTasks = await _db.TaskItems.CountAsync(); // Общее количество задач
            var completedTasks = await _db.TaskItems.CountAsync(t => t.IsCompleted); // Количество выполненных задач
            var notCompletedTasks = totalTasks - completedTasks; // Количество невыполненных задач

            var statistics = new Statistics
            {
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                NotCompletedTasks = notCompletedTasks
            };

            return Ok(statistics);
        }
    }
}
