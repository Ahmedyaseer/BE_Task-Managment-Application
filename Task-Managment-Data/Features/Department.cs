using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Task_Managment_Data.Core;

namespace Task_Managment_Data.Features
{
    [Table("Departments")]
    public class Department : AuditableEntity<int>
    {
        [Required(ErrorMessage = "Department name is required"), MaxLength(100)]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}   