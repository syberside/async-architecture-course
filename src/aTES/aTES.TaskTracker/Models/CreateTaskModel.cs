using System.ComponentModel.DataAnnotations;

namespace aTES.TaskTracker.Models
{
    public class CreateTaskModel
    {
        [Required]
        public string Description { get; set; }
    }
}
