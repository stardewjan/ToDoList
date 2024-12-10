using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsCompleted { get; set; }
    }
}
