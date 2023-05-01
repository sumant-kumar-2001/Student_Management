using Microsoft.EntityFrameworkCore;
using Student_Management.Models;

namespace Student_Management.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Student> students { get; set; }
        public DbSet<Course> courses { get; set; }
        public DbSet<Admin> admins { get; set; }
        public DbSet<StudentCourse> studentCourse { get; set; }
    }
}
