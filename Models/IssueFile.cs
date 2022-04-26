using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.API.Models
{
    [Table("issue_files", Schema = "issue_log")]
    public class IssueFile
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("file_url")]
        public string FileUrl { get; set; }
        [Column("description")]
        public string Description { get; set; }
        public Issue Issue { get; set; }

        [Column("issue_id")]
        public int IssueId { get; set; }

    }
}