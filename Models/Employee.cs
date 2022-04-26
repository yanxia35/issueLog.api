using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.API.Models
{
    [Table("employees")]
    public class Employee
    {
        [Column("id")]
        public string Id { get; set; }
        [Column("short_name")]
        public string ShortName { get; set; }
        [Column("first_name")]
        public string FirstName { get; set; }
        [Column("last_name")]
        public string LastName { get; set; }
        [Column("is_former")]
        public bool IsFormer { get; set; }
        [Column("group_id")]
        public string GroupId { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("phone")]
        public string Phone { get; set; } 
    }
}