using Microsoft.EntityFrameworkCore;
using ToDoList.Models.Entities;

namespace ToDoList.Data.DatabaseContext
{
    public class ToDoListDbContext : DbContext
    {
        public ToDoListDbContext(DbContextOptions<ToDoListDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<TaskItem> TaskItems { get; set; }

    }
}
