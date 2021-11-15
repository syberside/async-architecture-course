using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aTES.Billing.DataAccess
{
    [Table("transaction_log")]
    public class DbTransactionLogRecord
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("owner_id")]
        public Guid OwnerId { get; set; }

        public virtual DbUser Owner { get; set; }

        [Column("credit")]
        public long Credit { get; set; }

        [Column("debit")]
        public long Debit { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("details")]
        [Required]
        public string Details { get; set; }
    }
}
