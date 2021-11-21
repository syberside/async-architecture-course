using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aTES.Analytics.DataAccess
{
    public class DbTask
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("fullname")]
        [Required]
        public string FullName { get; set; }

        [Column("cost")]
        [Required]
        public long Cost { get; set; }
    }
}
