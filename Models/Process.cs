using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.API.Models
{
  [Table("processes",Schema =("issue_log"))]
  public class Process
  {
    [Column("id")]
    public int Id { get; set; }

    [Column("issue_process")]
    public string IssueProcess { get; set; }

    [Column("department")]
    public string Department { get; set; }

    [Column("name")]
    public string Name { get; set; }
  }
}