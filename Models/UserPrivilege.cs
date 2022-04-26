using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.API.Models {
    [Table ("user_privilege")]
    public class UserPrivilege {
        [Key]
        [Column ("id")]
        public int Id { get; set; }

        [Column ("user_id")]
        public string UserId { get; set; }

        [Column ("can_edit_projects"), Required]
        [DefaultValue ("false")]

        public bool CanEditProjects { get; set; }

        [Column ("can_edit_part_catalog"), Required]
        [DefaultValue ("false")]
        public bool CanEditPartCatalog { get; set; }

    }
}