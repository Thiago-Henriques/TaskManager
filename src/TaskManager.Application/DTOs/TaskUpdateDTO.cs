using System;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs
{
    public class TaskUpdateDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public TaskItemStatus Status { get; set; }
        public Guid UserId { get; set; }
    }
}
