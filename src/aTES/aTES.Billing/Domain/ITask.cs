using System;

namespace aTES.Billing.Services
{
    public interface ITask
    {
        Guid Id { get; }

        int BirdInCageCost { get; }

        int MilletInABowlCost { get; }

        string FullName { get; }
        string AssigneeId { get; }
    }
}
