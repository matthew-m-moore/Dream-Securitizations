using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dream.IO.Excel.Entities.SecuritizationRecords
{
    public class PriorityOfPaymentsRecord
    {
        public int Seniority { get; set; }
        public string TrancheName { get; set; }
        public string CashflowType { get; set; }
        public string WaterfallType { get; set; }
    }
}
