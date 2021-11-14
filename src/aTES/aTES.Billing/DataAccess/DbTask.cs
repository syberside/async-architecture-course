using aTES.Billing.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aTES.Billing.DataAccess
{
    [Table("tasks")]
    public class DbTask : ITask
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("public-id")]

        public string PublicId { get; set; }


        [Column("description")]
        [Required]
        public string Description { get; set; }


        [Column("assigned_user_id")]
        public Guid? AssignedUserId { get; set; }

        public virtual DbUser AssignedUser { get; set; }


        [Required]
        public string JiraId { get; set; }

        public int BirdInCageCost { get; set; }

        public int MilletInABowlCost { get; set; }

        public string FullName => $"[{JiraId}] {Description}";

        public string AssigneeId => AssignedUser.PublicId;
    }
}
