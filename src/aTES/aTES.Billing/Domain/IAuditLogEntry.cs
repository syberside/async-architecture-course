using System;

namespace aTES.Billing.Controllers
{
    public interface IAuditLogEntry
    {
        long Credit { get; }

        long Debit { get; }

        DateTime CreatedAt { get; }

        string Details { get; }
    }
}
