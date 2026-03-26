using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Task_Managment_Data.Core;

namespace Task_Managment_Data.Features
{
    [Table("TaskStatuses")]
    public class TaskStatus : AuditableEntity<int>
    {
        [Required(ErrorMessage = "Status name is required"), MaxLength(50)]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<WorkTask> Tasks { get; set; } = new List<WorkTask>();
    }
}