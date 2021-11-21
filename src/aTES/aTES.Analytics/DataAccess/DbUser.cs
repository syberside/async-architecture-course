using aTES.Analytics.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aTES.Analytics.DataAccess
{
    [Table("users")]
    public class DbUser
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("username")]
        [Required]
        public string Username { get; set; }

        [Column("role")]
        [Required]
        public Roles Role { get; set; }

        [Required]
        [Column("balance")]
        public long Balance { get; set; }

        [Required]
        [Column("public_id")]
        public string PublicId { get; set; }
    }
}
