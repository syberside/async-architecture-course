namespace aTES.TaskTracker.Domain
{
    public interface ITask
    {
        string PublicId { get; }
        bool IsCompeleted { get; }
        string Description { get; }
        string AssigneePublicId { get; }
        string AssigneeName { get; }

        string JiraId { get; }
        string FullName => $"[{JiraId}] {Description}";
    }
}
