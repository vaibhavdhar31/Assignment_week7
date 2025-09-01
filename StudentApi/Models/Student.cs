using System.ComponentModel.DataAnnotations;

namespace StudentApi.Models;

public class Student
{
    [Key]
    public int Rn { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Batch is required")]
    [StringLength(50, ErrorMessage = "Batch cannot exceed 50 characters")]
    public string Batch { get; set; } = string.Empty;
    
    [Range(0, 100, ErrorMessage = "Marks must be between 0 and 100")]
    public int Marks { get; set; }
}
