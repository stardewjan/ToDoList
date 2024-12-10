using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data.DatabaseContext;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ToDoListDbContext _context;

        public HomeController(ILogger<HomeController> logger, ToDoListDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalTasks = await _context.TaskItems.CountAsync(); // Общее количество задач
            var completedTasks = await _context.TaskItems.CountAsync(t => t.IsCompleted); // Количество выполненных задач
            var notCompletedTasks = totalTasks - completedTasks; // Количество невыполненных задач

            var statistics = new
            {
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                NotCompletedTasks = notCompletedTasks
            };

            return View(statistics); // Возвращаем статистику в представление
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
