
using System.ComponentModel.DataAnnotations;

namespace Student_Management.Models
{
    public class StudentCourse
    {
        [Key]
        public int StudentCourseId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
    }
}
