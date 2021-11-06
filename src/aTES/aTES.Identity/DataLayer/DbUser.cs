using aTES.Identity.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aTES.Identity.DataLayer
{
    [Table("users")]
    public class DbUser : IUser
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("username")]
        [Required]
        public string Username { get; set; }

        [Column("pwd_hash")]
        [Required]
        public string PasswordHash { get; set; }

        [Column("role")]
        [Required]

        public Roles Role { get; set; }


        [NotMapped]
        public string SubjectId => Id.ToString();

        [NotMapped]
        public bool IsActive => true;
    }
}
