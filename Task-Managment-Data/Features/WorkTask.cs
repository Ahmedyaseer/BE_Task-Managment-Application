using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Task_Managment_Data.Core;

namespace Task_Managment_Data.Features
{
    /// <summary>
    /// Renamed from Task to WorkTask to avoid conflict with System.Threading.Tasks.Task.
    /// </summary>
    [Table("Tasks")]
    public class WorkTask : AuditableEntity<int>
    {
        [Required(ErrorMessage = "Title is required"), MaxLength(200)]
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        public int AssignedToId { get; set; }
        public int TaskStatusId { get; set; } = (int)TaskStatusEnum.Pending;

        [ForeignKey("AssignedToId")]
        public virtual Employee AssignedTo { get; set; } = null!;

        [ForeignKey("TaskStatusId")]
        public virtual TaskStatus Status { get; set; } = null!;
        [NotMapped]
        public TaskStatusEnum StatusEnum
        {
            get => (TaskStatusEnum)TaskStatusId;
            set => TaskStatusId = (int)value;
        }
    }
}