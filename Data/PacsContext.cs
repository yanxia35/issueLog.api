using IssueLog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace IssueLog.API.Data
{
    public class PacsContext : DbContext
    {
        public PacsContext(DbContextOptions<PacsContext> options) : base(options) { }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<TbProject> TbProjects { get; set; }
        public virtual DbSet<UserPrivilege> UserPrivileges { get; set; }
        public virtual DbSet<ProjectReportedHour> ProjectReportedHours { get; set; }
        public virtual DbSet<ProjectReportedHoursByYears> ProjectReportedHoursByYears { get; set; }
        public DbSet<ProjectSchedule> ProjectSchedules { get; set; }
        public DbSet<Deliverable> Deliverables { get; set; }
        public DbSet<SubProject> SubProjects { get; set; }
        public DbSet<MbomPart> MbomParts { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<ActionOwner> ActionOwners { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectReportedHour>(e =>
            {
                e.HasNoKey();
            });
            modelBuilder.Entity<ProjectReportedHoursByYears>(e =>
            {
                e.HasNoKey();
            });
        }
    }
}