using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HavaDurumu.Data.Entities.ViewModels
{
    [Table("vw_UserProfiles")]
    public class UserProfilesView
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

