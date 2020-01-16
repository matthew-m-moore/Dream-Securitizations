using System;

namespace Dream.IO.Database.Entities.FinanceManagement
{
    public class EmployeeFactEntity
    {
        public int EmployeeFactDWId { get; set; }
        public DateTime? ReportEffectiveDate { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public int? EmployeeKey { get; set; }
        public int? RegionKey { get; set; }
        public int? ProductLineKey { get; set; }
        public int? CostCenterKey { get; set; }
        public bool? ActiveStatus { get; set; }
    }
}
