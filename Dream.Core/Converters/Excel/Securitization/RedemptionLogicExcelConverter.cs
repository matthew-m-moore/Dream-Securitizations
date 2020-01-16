using Dream.Common.Enums;
using Dream.Core.BusinessLogic.SecuritizationEngine.Redemption;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.IO.Excel.Entities.SecuritizationRecords;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Dream.Core.Converters.Excel.Securitization
{
    public class RedemptionLogicExcelConverter
    {
        public const string PercentOfCollateralRedemptionLogic = "% Of Collateral";

        private const string _tranchesPaidOutRedemptionLogic = "Standard HERO";      
        private const string _doNothingRedemptionLogic = "Do Nothing";

        private List<Tranche> _listOfTranchesInSecuritization;

        public RedemptionLogic RedemptionLogic { get; private set; }

        public RedemptionLogicExcelConverter(
            List<Tranche> listOfTranchesInSecuritization, 
            string redemptionLogicChoice, 
            double? redemptionTriggerValue = null)
        {
            _listOfTranchesInSecuritization = listOfTranchesInSecuritization;

            switch (redemptionLogicChoice)
            {
                case _tranchesPaidOutRedemptionLogic:
                    RedemptionLogic = new TranchesCanBePaidOutFromAvailableFundsRedemptionLogic();
                    break;

                case PercentOfCollateralRedemptionLogic:
                    if (!redemptionTriggerValue.HasValue) throw new Exception("ERROR: Redemption trigger value was not provided.");
                    RedemptionLogic = new LessThanPercentOfInitalCollateralBalanceRedemptionLogic(redemptionTriggerValue.GetValueOrDefault());
                    break;

                case _doNothingRedemptionLogic:
                    RedemptionLogic = new DoNothingRedemptionLogic();
                    break;

                default:
                    throw new Exception(string.Format("ERROR: The choice of redemption logic '{0}' is not yet supported.", redemptionLogicChoice));
            }
        }

        /// <summary>
        /// Attempts to add an allowed month for redemption. If the month provided fails to parse to an enum, throws an exception.
        /// </summary>
        public void TryAddAllowedMonthForRedemption(string allowedRedemptionMonthText)
        {
            try
            {
                var allowedRedemptionMonth = (Month)Enum.Parse(typeof(Month), allowedRedemptionMonthText);
                RedemptionLogic.AddAllowedMonthForRedemption(allowedRedemptionMonth);
            }
            catch
            {
                throw new Exception(string.Format("ERROR: Format or description of allowed redemption month '{0}' could not be parsed.", 
                    allowedRedemptionMonthText));
            }
        }

        /// <summary>
        /// Adds all non-fee tranches to the specified redemption logic, if applicable.
        /// </summary>
        public void AddTrancheStructureRecordsToRedemptionLogic(List<TrancheStructureRecord> listOfTrancheStructureRecords)
        {
            var tranchesPaidOutRedemptionLogic = RedemptionLogic as TranchesCanBePaidOutFromAvailableFundsRedemptionLogic;
            if (tranchesPaidOutRedemptionLogic == null) return;

            var listOfTranchesNamesToBePaidOutAtRedemption = listOfTrancheStructureRecords
                .Where(r => r.PaysOutAtRedemption)
                .Select(t => t.TrancheName).ToList();

            var listOfTranchesToBePaidOutAtRedemption = _listOfTranchesInSecuritization
                .Where(t => listOfTranchesNamesToBePaidOutAtRedemption.Contains(t.TrancheName))
                .ToList();

            tranchesPaidOutRedemptionLogic.ListOfTranchesToBePaidOut.AddRange(listOfTranchesToBePaidOutAtRedemption);
            RedemptionLogic = tranchesPaidOutRedemptionLogic;
        }

        /// <summary>
        /// Adds all fee tranches to the specified redemption logic, if applicable.
        /// </summary>
        public void AddFeeGroupRecordsToRedemptionLogic(List<FeeGroupRecord> listOfFeeGroupRecords)
        {
            var tranchesPaidOutRedemptionLogic = RedemptionLogic as TranchesCanBePaidOutFromAvailableFundsRedemptionLogic;
            if (tranchesPaidOutRedemptionLogic == null) return;

            var listOfFeeGroupNamesToBePaidOutAtRedemption = listOfFeeGroupRecords
                .Where(r => r.PaysOutAtRedemption)
                .Select(f => f.FeeGroupName).ToList();

            var listOfFeeGroupsToBePaidOutAtRedemption = _listOfTranchesInSecuritization
                .Where(t => listOfFeeGroupNamesToBePaidOutAtRedemption.Contains(t.TrancheName))
                .ToList();

            tranchesPaidOutRedemptionLogic.ListOfTranchesToBePaidOut.AddRange(listOfFeeGroupsToBePaidOutAtRedemption);
            RedemptionLogic = tranchesPaidOutRedemptionLogic;
        }
    }
}
