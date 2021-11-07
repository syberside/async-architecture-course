using aTES.TaskTracker.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aTES.TaskTracker.DataLayer
{
    [Table("tasks")]
    public class DbTask : ITask
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("is_completed")]
        [Required]
        public bool IsCompeleted { get; set; }

        [Column("description")]
        [Required]
        public string Description { get; set; }

        [Column("assigned_user_id")]
        [Required]
        public Guid AssignedUserId { get; set; }

        [Required]
        public virtual DbUser AssignedUser { get; set; }

        [NotMapped]
        public string PublicId => Id.ToString();

        [NotMapped]
        public string AssigneePublicId => AssignedUser.PublicId;

        public string AssigneeName => AssignedUser.Username;
    }
}
