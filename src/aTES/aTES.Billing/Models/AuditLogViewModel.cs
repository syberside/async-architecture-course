namespace aTES.Billing.Controllers
{
    public class AuditLogViewModel
    {
        public long CurrentBalance { get; set; }

        public IAuditLogEntry[] Log { get; set; }
        public long? TodayTotal { get; internal set; }
    }
}
