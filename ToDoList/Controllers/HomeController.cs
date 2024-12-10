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
            var totalTasks = await _context.TaskItems.CountAsync(); // ����� ���������� �����
            var completedTasks = await _context.TaskItems.CountAsync(t => t.IsCompleted); // ���������� ����������� �����
            var notCompletedTasks = totalTasks - completedTasks; // ���������� ������������� �����

            var statistics = new
            {
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                NotCompletedTasks = notCompletedTasks
            };

            return View(statistics); // ���������� ���������� � �������������
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
