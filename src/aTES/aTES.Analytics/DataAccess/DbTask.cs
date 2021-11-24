using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aTES.Analytics.DataAccess
{
    public class DbTask
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("cost")]
        [Required]
        public long Cost { get; set; }

        [Column("closed_at")]
        [Required]
        public DateTime ClosedAt { get; set; }

        [Required]
        [Column("public_id")]
        public string PublicId { get; set; }
    }
}
