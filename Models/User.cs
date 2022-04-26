using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.API.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id"),Required]
        public string UserId { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Column("password_hash")]
        public byte[] PasswordHash { get; set; }
        [Column("password_salt")]
        public byte[] PasswordSalt { get; set; }
    }

}