using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using ToDoList.Data.DatabaseContext;
using ToDoList.Models.Entities;

namespace ToDoList.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Контроллер для управления задачами")]
    public class TaskItemsApiController : ControllerBase
    {
        private readonly ToDoListDbContext _db;

        public TaskItemsApiController(ToDoListDbContext context)
        {
            _db = context;
        }

        /// <summary>
        /// Получить список всех задач.
        /// </summary>
        /// <returns>Список задач.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Получить список задач", Description = "Возвращает все задачи.")]
        [SwaggerResponse(200, "Список задач успешно получен.", typeof(IEnumerable<TaskItem>))]
        public async Task<IActionResult> GetTaskItems()
        {
            var taskItems = await _db.TaskItems.ToListAsync();
            return Ok(taskItems);
        }

        /// <summary>
        /// Получить задачу по ID.
        /// </summary>
        /// <param name="id">Идентификатор задачи.</param>
        /// <returns>Детали задачи.</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Получить задачу", Description = "Возвращает задачу по ID.")]
        [SwaggerResponse(200, "Задача найдена.", typeof(TaskItem))]
        [SwaggerResponse(404, "Задача не найдена.")]
        public async Task<IActionResult> GetTaskItem(int id)
        {
            var taskItem = await _db.TaskItems.FindAsync(id);
            if (taskItem == null) return NotFound();

            return Ok(taskItem);
        }

        /// <summary>
        /// Создать новую задачу.
        /// </summary>
        /// <param name="taskItem">Данные задачи.</param>
        /// <returns>Созданная задача.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Создать задачу", Description = "Добавляет новую задачу.")]
        [SwaggerResponse(201, "Задача успешно создана.", typeof(TaskItem))]
        [SwaggerResponse(400, "Неверный запрос.")]
        public async Task<IActionResult> CreateTaskItem([FromBody] TaskItem taskItem)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _db.TaskItems.Add(taskItem);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaskItem), new { id = taskItem.Id }, taskItem);
        }

        /// <summary>
        /// Обновить задачу.
        /// </summary>
        /// <param name="id">Идентификатор задачи.</param>
        /// <param name="taskItem">Обновленные данные задачи.</param>
        /// <returns>Результат обновления.</returns>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Обновить задачу", Description = "Изменяет данные существующей задачи.")]
        [SwaggerResponse(204, "Задача успешно обновлена.")]
        [SwaggerResponse(400, "Неверный запрос.")]
        [SwaggerResponse(404, "Задача не найдена.")]
        public async Task<IActionResult> UpdateTaskItem(int id, [FromBody] TaskItem taskItem)
        {
            if (id != taskItem.Id) return BadRequest();

            _db.Entry(taskItem).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskItemExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Удалить задачу.
        /// </summary>
        /// <param name="id">Идентификатор задачи.</param>
        /// <returns>Результат удаления.</returns>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Удалить задачу", Description = "Удаляет задачу по ID.")]
        [SwaggerResponse(204, "Задача успешно удалена.")]
        [SwaggerResponse(404, "Задача не найдена.")]
        public async Task<IActionResult> DeleteTaskItem(int id)
        {
            var taskItem = await _db.TaskItems.FindAsync(id);
            if (taskItem == null) return NotFound();

            _db.TaskItems.Remove(taskItem);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskItemExists(int id)
        {
            return _db.TaskItems.Any(e => e.Id == id);
        }

        /// <summary>
        /// Завершить задачу.
        /// </summary>
        /// <param name="id">Идентификатор задачи.</param>
        /// <returns>Обновлённая задача.</returns>
        [HttpPost("{id}/complete")]
        [SwaggerOperation(Summary = "Завершить задачу", Description = "Обновляет задачу, устанавливая статус 'Выполнено'.")]
        [SwaggerResponse(200, "Задача успешно завершена.", typeof(TaskItem))]
        [SwaggerResponse(404, "Задача не найдена.")]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var taskItem = await _db.TaskItems.FindAsync(id);
            if (taskItem == null)
            {
                return NotFound();
            }

            taskItem.IsCompleted = true; // Обновляем статус на "Выполнено"
            await _db.SaveChangesAsync();

            return Ok(taskItem); // Возвращаем обновленную задачу
        }

    }
}
