using IssueLog.API.Models;
using IssueLog.ModelsSage;
using Microsoft.EntityFrameworkCore;

namespace IssueLog.API.Data {
    public class TempContext : DbContext {
        public TempContext (DbContextOptions<TempContext> options) : base (options) { }
        public virtual DbSet<IssueFile> IssueFile { get; set; }
    }
}