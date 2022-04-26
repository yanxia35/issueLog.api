using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.API.Models {
  [Table ("failure_modes", Schema = "issue_log")]
  public class FailureMode {
    [Column ("id")]
    public int Id { get; set; }

    [Column ("failure_mode_desc")]
    public string FailureModeDesc { get; set; }

    [Column ("failure_type")]
    public string FailureType { get; set; }

    [Column ("name")]
    public string Name { get; set; }
  }
}