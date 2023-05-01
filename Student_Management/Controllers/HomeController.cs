using ExcelDataReader;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Protocol;
using NuGet.Protocol.Plugins;
using Student_Management.DAL;
using Student_Management.Models;
using Student_Management.ViewModel;
using System.Diagnostics;
using System.Net;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Student_Management.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AddStudent(int? id)
        {
            Student student = new Student();
            if (id == null || id == 0)
            {
                return View(student);
            }
            else
            {
                var students = _context.students.Find(id);
                return View(students);
            }
        }

        [HttpGet]
        public IActionResult AddStudentCourse(int? id)
        {
            var student = _context.students.Find(id);
            ViewBag.StudentId = student.StudentId;


            CourseVM course = new()
            {
                CourseList = _context.courses.Select(u => new SelectListItem
                {
                    Text = u.CourseName,
                    Value = u.CourseId.ToString()
                }),
                course = new Course()
            };
            return View(course);
        }

        [HttpPost]
        public IActionResult AddStudentCourse(CourseVM model, int studentId)
        {
            if (model.course == null)
            {
                TempData["error"] = "Please Select Course Before Submiting";
                return RedirectToAction("StudentData");

            }
            var Scourse = _context.studentCourse.Where(x => x.StudentId == studentId && x.CourseId == model.course.CourseId).FirstOrDefault();
            if (Scourse != null)
            {
                TempData["error"] = "This course is already taken by particular student.." + studentId;
                return RedirectToAction("Studentdata");
            }
            var Studentcourse = _context.students.Find(studentId);
            var course = _context.courses.Where(x => x.CourseId == model.course.CourseId).FirstOrDefault();
            StudentCourse courses = new StudentCourse()
            {
                StudentId = studentId,
                CourseId = model.course.CourseId,
                CourseName = course.CourseName
            };

            var Course = _context.courses.Where(x => x.CourseId == model.course.CourseId).First();
            var coursePrice = Course.CoursePrice;
            var student = _context.students.Find(studentId);



            if (student.TotalAmt == null)
            {
                student.TotalAmt = 0;
            }
            var TotalCourseAmt = student.TotalAmt += coursePrice;



            Student particularstudent = new Student()
            {
                TotalAmt = TotalCourseAmt,
            };

            _context.studentCourse.Add(courses);
            _context.SaveChanges();
            TempData["success"] = "Course Added Successfully ";
            return RedirectToAction("StudentData");

        }

        [HttpPost]
        public IActionResult AddStudent(Student model)
        {
            if (ModelState.IsValid)
            {
                if (model.StudentId == 0)
                {
                    _context.Add(model);
                    TempData["success"] = "Student Added Successfully";

                }
                else
                {
                    _context.Update(model);
                    TempData["success"] = "Student Updated Successfully";
                }
                _context.SaveChanges();
                return RedirectToAction("StudentData");
            }
            else
            {
                return View(model);
            }



        }

        [HttpPost]
        public IActionResult Login(Admin model)
        {
      
                var user = _context.admins.Where(x => x.AdminEmail == model.AdminEmail).FirstOrDefault();
                if (user == null)
                {
                    TempData["error"] = "User Not Found!";
                    return RedirectToAction("Login");
                }
                if (user.AdminPassword != model.AdminPassword)
                {
                    TempData["error"] = "Invalid Password!";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["success"] = "Admin login Successfully";
                    return RedirectToAction("StudentData");
                }
          
        
        }

        [HttpGet]
        public IActionResult StudentData(string? term)
        {
            if (term == null)
            {
                IEnumerable<Student> students = _context.students.ToList();
                return View(students);
            }
            else
            {
                ViewBag.SearchStr = term;
                IEnumerable<Student> student = _context.students.Where(x => x.Name.ToLower().Contains(term.ToLower())).ToList();
                return View(student);
            }

        }

        public async Task<IActionResult> DeleteStudent(int? id)
        {
            var student = _context.students.Find(id);
            var studentcourse = _context.studentCourse.Where(x => x.StudentId == id).FirstOrDefault();
            if (studentcourse == null)
            {
                _context.Remove(student);
                _context.SaveChanges();
                TempData["success"] = "Student Deleted Successfully";
                return RedirectToAction("StudentData");
            }
            else
            {
                TempData["error"] = "You cannot delete a student with course!";
                return RedirectToAction("StudentData");
            }
        }

        public async Task<IActionResult> Deletecourse(int? id)
        {

            var assigncourse = _context.studentCourse.Where(x => x.CourseId == id).FirstOrDefault();

            if (assigncourse == null)
            {
                var course = _context.courses.Find(id);
                _context.courses.Remove(course);
                _context.SaveChanges();
                TempData["success"] = "Course Deleted Successfully";
                return RedirectToAction("AllCourse");
            }
            else
            {
                var course = _context.courses.Where(x => x.CourseId == id).First();
                TempData["error"] = course.CourseName + " course is assigned to some students";
                return RedirectToAction("AllCourse");
            }

        }

        public async Task<IActionResult> DeleteParticularCourse(int id, string StudentId)
        {

            var student = _context.students.Where(x => x.StudentId.ToString() == StudentId).FirstOrDefault();
            var course = _context.courses.Where(x => x.CourseId == id).FirstOrDefault();
            var Particularcourse = _context.studentCourse.Where(p => p.CourseId == id && p.StudentId.ToString() == StudentId).ToList();
            _context.studentCourse.RemoveRange(Particularcourse);

            var totalprice = student.TotalAmt -= course.CoursePrice;
            Student students = new Student()
            {
                TotalAmt = totalprice,
            };

            _context.SaveChanges();
            TempData["success"] = "Course Deleted Successfully";
            return RedirectToAction("StudentData");
        }

        public IActionResult AllCourse(string? term)
        {
            if(term == null)
            {
                IEnumerable<Course> course = _context.courses.ToList();
                return View(course);
            }
            else
            {
                ViewBag.SearchStr = term;
                IEnumerable<Course> course = _context.courses.Where(x => x.CourseName.ToLower().Contains(term.ToLower())).ToList();
                return View(course);
            }
         
        }
        [HttpGet]
        public IActionResult AddCourse(int? id)
        {
            Course course = new Course();
            if (id == null || id == 0)
            {
                return View(course);
            }
            else
            {
                var courses = _context.courses.Find(id);
                return View(courses);
            }
        }
        [HttpPost]
        public IActionResult AddCourse(Course course, int id)
        {
            if (ModelState.IsValid)
            {
                var student = _context.students.Find(id);
                course = new Course
                {
                    CourseId = id,
                    CourseName = course.CourseName,
                    CoursePrice = course.CoursePrice
                };

                if (course.CourseId == 0)
                {
                    _context.Add(course);
                    TempData["success"] = "Course Added Successfully";

                }
                else
                {
                    var studentcourse = _context.studentCourse.Where(x => x.CourseId == id).FirstOrDefault();
                    if (studentcourse != null)
                    {
                        TempData["error"] = studentcourse.CourseName + " Course can't be edit it is assigned to student";
                        return RedirectToAction("AllCourses");
                    }
                    _context.Update(course);
                    TempData["success"] = "Course Updated Successfully";
                }
                _context.SaveChanges();
                return RedirectToAction("AllCourse");
            }
            else
            {
                return View(course);
            }
        }
        

        public IActionResult ParticularCourse(int? id)
        {
            if (id == 0)
            {
                return View("StudentData");
            }
            else
            {
                var student = _context.students.Find(id);
                ViewBag.StudentId = student.StudentId;
                IEnumerable<StudentCourse> studentcourse = _context.studentCourse.Where(x => x.StudentId == id).ToList();
                return View(studentcourse);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ExportExcel(IFormFileCollection form)
        {
            List<Student> students = new List<Student>();
            //var filepath = "C:\\Users\\sumant.kumar\\Desktop\\inevtory task.xlsx";


            // Change filepath Accondingly
            var filepath = "C:\\Users\\sumant.kumar\\Desktop\\StudentManagement.xlsx";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(filepath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {

                    while (reader.Read()) //Each row of the file
                    {
                        students.Add(new Student
                        {
                            Name = reader.GetValue(0).ToString(),
                            RollNo = int.Parse(reader.GetValue(1).ToString()),
                            Email = reader.GetValue(2).ToString(),
                            ContactNo = reader.GetValue(3).ToString(),
                            Address = reader.GetString(4),
                            State = reader.GetString(5),
                            City = reader.GetString(6),
                            ZipCode = int.Parse(reader.GetValue(7).ToString()),
                            TotalAmt = double.Parse(reader.GetValue(8).ToString())
                        });

                    }
                }


            }
            _context.students.AddRange(students);
            _context.SaveChanges();
            TempData["success"] = "Student Data File is Imported";
            return RedirectToAction("StudentData");

        }
    }
}