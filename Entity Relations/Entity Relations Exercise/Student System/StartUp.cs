using P01_StudentSystem.Data;

public class StartUp
{
    private static void Main(string[] args)
    {
        var db = new StudentSystemContext();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        //db.Students.Add(new Student {
        //Name = "Lyubomir",
        //RegisteredOn = DateTime.Now,
        //});
        //db.SaveChanges();

        //db.Courses.Add(new Course { 
        //Name = "C#",
        //StartDate = DateTime.Parse("2023/01/01"),
        //EndDate = DateTime.Parse("2023/12/31"),
        //Price = 450
        //});
        //db.SaveChanges();

        //db.Resources.Add(new Resource
        //{
        //    Name = "Resource1",
        //    Url = "www.test.com",
        //    ResourceType = ResourceType.Video,
        //    CourseId = 1,
        //});
        //db.SaveChanges();
    }
}