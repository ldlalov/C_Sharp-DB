using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Data.Models
{
    public class Student
    {
        public Student()
        {
            StudentsCourses = new HashSet<StudentCourse>();
            Homeworks = new HashSet<Homework>();
        }
        public int StudentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [StringLength(10)]
        [Column(TypeName = "char(10)")]
        public string? PhoneNumber { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime RegisteredOn { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? Birthday { get; set; }
        public ICollection<StudentCourse> StudentsCourses { get; set; }
        public ICollection<Homework> Homeworks { get; set; }

    }
}
