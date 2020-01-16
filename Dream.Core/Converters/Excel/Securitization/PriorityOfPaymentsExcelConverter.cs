using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using Dream.Core.Converters.Excel.Securitization;
using Dream.IO.Excel.Entities.SecuritizationRecords;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.Converters.Excel
{
    public class PriorityOfPaymentsExcelConverter
    {
        private const string _standardWaterfall = "Standard";
        private const string _redemptionWaterfall = "Redemption";
        private const string _postRedemptionWaterfall = "Post-Redemption";

        public static PriorityOfPayments ConvertListOfPriorityOfPaymentsRecords(List<PriorityOfPaymentsRecord> listOfPriorityOfPaymentsRecords, 
            bool isRedemptionPriorityOfPayments,
            bool checkPostRedemptionPriorityOfPayments = false)
        {
            var waterfallIndicator = isRedemptionPriorityOfPayments
                ? _redemptionWaterfall
                : _standardWaterfall;

            if (checkPostRedemptionPriorityOfPayments && isRedemptionPriorityOfPayments)
            {
                if (listOfPriorityOfPaymentsRecords.Any(p => p.WaterfallType == _postRedemptionWaterfall))
                {
                    waterfallIndicator = _postRedemptionWaterfall;
                }
            }

            var listOfPriorityOfPaymentsEntries = new List<PriorityOfPaymentsEntry>();
            foreach(var priorityOfPaymentsRecord in listOfPriorityOfPaymentsRecords.Where(p => p.WaterfallType == waterfallIndicator))
            {
                var priorityOfPaymentsEntry = ConvertPriorityOfPaymentsRecord(priorityOfPaymentsRecord);

                if (listOfPriorityOfPaymentsEntries.Any(e => 
                    e.TrancheName == priorityOfPaymentsEntry.TrancheName &&
                    e.TrancheCashFlowType == priorityOfPaymentsEntry.TrancheCashFlowType))
                {
                    throw new Exception(string.Format("ERROR: There is a duplicate entry in the priority of payments waterfall for tranche name '{0}' with cash flow type '{1}'",
                        priorityOfPaymentsEntry.TrancheName,
                        priorityOfPaymentsEntry.TrancheCashFlowType));
                }

                listOfPriorityOfPaymentsEntries.Add(priorityOfPaymentsEntry);
            }

            var priorityOfPayments = new PriorityOfPayments(listOfPriorityOfPaymentsEntries);
            return priorityOfPayments;
        }

        private static PriorityOfPaymentsEntry ConvertPriorityOfPaymentsRecord(PriorityOfPaymentsRecord priorityOfPaymentsRecord)
        {
            var trancheCashFlowType = TrancheCashFlowTypeExcelConverter.ConvertString(priorityOfPaymentsRecord.CashflowType);

            var priorityOfPaymentsEntry = new PriorityOfPaymentsEntry(
                priorityOfPaymentsRecord.Seniority,
                priorityOfPaymentsRecord.TrancheName,
                trancheCashFlowType);

            return priorityOfPaymentsEntry;
        }
    }
}
