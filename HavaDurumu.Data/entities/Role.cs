using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavaDurumu.Data.Entities
{
    [Table("Roles")]
    public class Role
    {
        [Key]
        public int RoleID { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        // İlişki: Bir rolün birden fazla kullanıcısı olabilir
        public virtual ICollection<User> Users { get; set; }
    }
}
