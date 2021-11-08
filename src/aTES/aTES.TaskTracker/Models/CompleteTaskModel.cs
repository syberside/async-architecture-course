using System;
using System.ComponentModel.DataAnnotations;

namespace aTES.TaskTracker.Models
{
    public class CompleteTaskModel
    {
        [Required]
        public Guid TaskId { get; set; }
    }
}
