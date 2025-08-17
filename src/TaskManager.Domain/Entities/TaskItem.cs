using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities
{
    public class TaskItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public TaskItemStatus Status { get; set; }
        public Guid UserId { get; set; }
    }
}
