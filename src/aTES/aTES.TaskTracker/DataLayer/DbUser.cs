using aTES.TaskTracker.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aTES.TaskTracker.DataLayer
{
    [Table("users")]
    public class DbUser
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        public string PublicId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public Roles Role { get; set; }
    }
}
