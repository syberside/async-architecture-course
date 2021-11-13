using aTES.TaskTracker.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace aTES.TaskTracker.Models
{
    public class CreateTaskModel : IValidatableObject
    {
        [Required]
        public string Description { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Description == null)
            {
                yield break;
            }

            var sp = validationContext.GetService(typeof(IServiceProvider)) as IServiceProvider;
            var taskService = sp.GetRequiredService<TasksService>();
            if (!taskService.IsValidTaskName(Description))
            {
                yield return new ValidationResult("Task name is not valid");
            }
        }
    }
}
