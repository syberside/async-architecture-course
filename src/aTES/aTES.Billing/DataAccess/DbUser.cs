using aTES.Billing.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aTES.Billing.DataAccess
{
    [Table("users")]
    public class DbUser
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("public-id")]
        public string PublicId { get; set; }

        [Required]
        [Column("username")]
        public string Username { get; set; }

        [Required]
        [Column("role")]
        public Roles Role { get; set; }

        [Required]
        [Column("amount")]
        public long Balance { get; set; }
    }
}
