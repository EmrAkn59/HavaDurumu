using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavaDurumu.Data.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserID { get; set; }

        // Foreign Key
        public int RoleID { get; set; }
        [ForeignKey("RoleID")]
        public virtual Role Role { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(256)]
        public string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
