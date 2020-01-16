using System;

namespace Dream.IO.Database.Entities.FinanceManagement
{
    public class LoanFactEntity
    {
        public int LoanFactId { get; set; }
        public int SourceSystemKey { get; set; }
        public int RecordTypeKey { get; set; }
        public int LoanLevelKey { get; set; }
        public int LoanProductKey { get; set; }
        public string StateFullName { get; set; }
        public string SourceRecordId { get; set; }
        public DateTime? TransactionDate { get; set; }
        public int? TransactionDateKey { get; set; }
        public int? TermInYears { get; set; }
        public decimal? Rate { get; set; }
        public decimal? TransactionAmount { get; set; }
    }
}
