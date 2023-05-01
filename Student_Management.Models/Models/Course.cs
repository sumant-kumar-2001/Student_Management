using System.ComponentModel.DataAnnotations;

namespace Student_Management.Models
{
    public class Course
    {
        public int CourseId   { get; set; }
        [Required]
        public string CourseName { get; set; }
        [Required]
        public double CoursePrice  { get; set; }
    }
}
