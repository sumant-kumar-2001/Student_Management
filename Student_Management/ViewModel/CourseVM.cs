using Microsoft.AspNetCore.Mvc.Rendering;
using Student_Management.Models;

namespace Student_Management.ViewModel
{
    public class CourseVM
    {
        public Course course { get; set; }
        public IEnumerable<SelectListItem> CourseList { get; set; }

    }
}
