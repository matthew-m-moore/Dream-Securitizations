using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.IO.Excel.Entities.SecuritizationRecords;
using System;

namespace Dream.Core.Converters.Excel.Securitization
{
    public class AvailableFundsRetrieverExcelConverter
    {
        private const string _principalAdvanceRetriever = "Principal Advances";
        private const string _allRemaingFundsRetriever = "All Remaining Funds";

        public static AvailableFundsRetriever ExtractAvailableFundsRetrieverFromTrancheStructureRecord(TrancheStructureRecord trancheStructureRecord, bool isInterestRetriever)
        {
            var fundsSourceDescription = isInterestRetriever
                ? trancheStructureRecord.InterestFundsSource
                : trancheStructureRecord.PrincipalFundsSource;

            switch (fundsSourceDescription)
            {
                case _principalAdvanceRetriever:
                    if (!trancheStructureRecord.PrinAdvanceRate.HasValue) throw new Exception("ERROR: No principal advance rate was provided for funds source.");
                    return new PrincipalRemittancesAvailableFundsRetriever(trancheStructureRecord.PrinAdvanceRate.GetValueOrDefault());

                case _allRemaingFundsRetriever:
                    return new AllFundsAvailableFundsRetriever();

                default:
                    throw new Exception(string.Format("ERROR: No known funds source was provided for tranche named '{0}'.", trancheStructureRecord.TrancheName));
            }
        }

        public static AvailableFundsRetriever ExtractAvailableFundsRetrieverFromFeeGroupRecord(FeeGroupRecord feeGroupRecord)
        {
            var fundsSourceDescription = feeGroupRecord.FundsSource;

            switch (fundsSourceDescription)
            {
                case _allRemaingFundsRetriever:
                    return new AllFundsAvailableFundsRetriever();

                default:
                    throw new Exception(string.Format("ERROR: No known funds source was provided for fee group named '{0}'.", feeGroupRecord.FeeGroupName));
            }
        }
    }
}
