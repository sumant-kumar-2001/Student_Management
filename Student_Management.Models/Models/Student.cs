using System.ComponentModel.DataAnnotations;

namespace Student_Management.Models
{
    public class Student
    {
        [Key]
        public int StudentId  { get; set; }
        [Required]
        public string Name    { get; set; }
        public int RollNo   { get; set; }
        [Required]
        [EmailAddress]
        public string Email  { get; set; }
        public string Address   { get; set; }
        public string State  { get; set; }
        public string City   { get; set; }
        public int ZipCode { get; set; }
        public string ContactNo    { get; set; }
        public double? TotalAmt { get; set; }

    }
}
