namespace aTES.TaskTracker.Domain
{
    public interface ITask
    {
        public string PublicId { get; }

        public bool IsCompeleted { get; }

        public string Description { get; }
        string AssigneePublicId { get; }
        string AssigneeName { get; }
    }
}
