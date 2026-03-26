using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Task_Managment_Data.Core;

namespace Task_Managment_Data.Features
{
    [Table("Employees")]
    public class Employee : AuditableEntity<int>
    {
        [Required(ErrorMessage = "First name is required"), MaxLength(50)]
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required"), MaxLength(50)]
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("jobTitle")]
        public string? JobTitle { get; set; }

        public int DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; } = null!;

        public virtual ICollection<WorkTask> AssignedTasks { get; set; } = new List<WorkTask>();
    }
}