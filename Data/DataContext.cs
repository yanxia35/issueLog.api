using IssueLog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace IssueLog.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<IssueAction> IssueActions { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Process> Processes { get; set; }
        public DbSet<FailureMode> FailureModes { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<PartIssue> PartIssues { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<EcPart> EcParts { get; set; }
        public DbSet<MbomPart> MbomParts { get; set; }
        public DbSet<TEcPart> TEcParts { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public DbSet<IssueFile> IssueFiles { get; set; }
        public virtual DbSet<ReportProjectIssueCount> ReportProjectIssueCounts { get; set; }
        public DbSet<SubProject> SubProjects { get; set; }
        public DbSet<SubProjectStatus> subProjectStatuses { get; set; }
        public DbSet<ActivePO> ActivePOs { get; set; }
        public DbSet<IssueSchedule> IssueSchedule {get; set;}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}