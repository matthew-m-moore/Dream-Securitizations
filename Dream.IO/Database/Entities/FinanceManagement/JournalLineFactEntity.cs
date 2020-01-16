using System;

namespace Dream.IO.Database.Entities.FinanceManagement
{
    public class JournalLineFactEntity
    {
        public int JournalLineFactDWId { get; set; }
        public DateTime? JournalLineFactLastUpdated { get; set; }
        public DateTime? ReportDate { get; set; }
        public DateTime? AccountingDate { get; set; }
        public decimal NetAmount { get; set; }
        public int? CompanyKey { get; set; }
        public int? EmployeeKey { get; set; }
        public int? LedgerAccountKey { get; set; }
        public int? LedgerAccountTypeKey { get; set; }
        public int? SpendCategoryKey { get; set; }
        public int? RevenueCategoryKey { get; set; }
        public int? ProductLineKey { get; set; }
        public int? RegionKey { get; set; }
        public string CFSupplier { get; set; }
        public string SupplierInvoice { get; set; }
        public string LineMemo { get; set; }
    }
}
