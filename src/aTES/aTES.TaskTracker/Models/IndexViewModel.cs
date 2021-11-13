using aTES.TaskTracker.Domain;
using System.Collections.Generic;

namespace aTES.TaskTracker.Models
{
    public class IndexViewModel
    {
        public IReadOnlyCollection<TaskViewModel> Tasks { get; set; }

        public bool OnlyMyTasks { get; set; }

        public bool CanAssignTasks { get; set; }
        public bool CanStreamAllTasks { get; internal set; }
    }

    public class TaskViewModel
    {
        public ITask Task { get; set; }
        public bool CanComplete { get; set; }
    }
}
