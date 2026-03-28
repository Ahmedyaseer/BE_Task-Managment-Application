using System.ComponentModel.DataAnnotations;

namespace AKG_Task_Managment_System.DTOs.Task
{
    public class CreateTaskDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public int AssignedToId { get; set; }
        [Required]
        public int DepartmentId { get; set; }
    }

    public class UpdateTaskDto
    {
        [MaxLength(200)]
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? AssignedToId { get; set; }
        public int? TaskStatusId { get; set; }
        public int? DepartmentId { get; set; }
    }

    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int AssignedToId { get; set; }
        public string AssignedToName { get; set; } = string.Empty;
        public int TaskStatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
