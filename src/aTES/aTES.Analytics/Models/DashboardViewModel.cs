namespace aTES.Analytics.Models
{
    public class DashboardViewModel
    {
        public DashboardType Type { get; internal set; }
        public int DeptorsCount { get; internal set; }
        public long IncomeSize { get; internal set; }
        public long MostExpensiveTaskCost { get; internal set; }
        public DateTime GeneratedAt { get; internal set; }
    }

    public enum DashboardType
    {
        MostExpensiveByDay,
        MostExpensiveByWeek,
        MostExpensiveByMonth,
    }

}
