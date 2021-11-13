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


        [Required]
        [Column("sequence_value")]
        public int SequenceValue { get; set; }


        [NotMapped]
        public string FullName => "[UBERPOP-" + SequenceValue + "] " + Description;


        [NotMapped]
        public string AssigneeName => AssignedUser.Username;

        [NotMapped]
        public string PublicId => Id.ToString();

        [NotMapped]
        public string AssigneePublicId => AssignedUser.PublicId;
    }
}
