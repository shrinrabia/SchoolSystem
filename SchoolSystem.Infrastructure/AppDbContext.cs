using Microsoft.EntityFrameworkCore;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<CourseInstance> CourseInstances { get; set; } = null!;
        public DbSet<Participant> Participants { get; set; } = null!;
        public DbSet<CourseRegistration> CourseRegistrations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Teacher>().HasData(new Teacher { Id = 1, Name = "Hans" });
            modelBuilder.Entity<Course>().HasData(new Course { Id = 1, Name = "C# Programming", TeacherId = 1 });
        }
    }
}
    