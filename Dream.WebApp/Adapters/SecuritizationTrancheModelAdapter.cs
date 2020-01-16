using System;
using Dream.WebApp.Models;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.ReserveFunds;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using Dream.WebApp.ModelEntries;
using System.Collections.Generic;
using System.Linq;
using Dream.Common.Enums;
using Dream.Core.BusinessLogic.SecuritizationEngine.Redemption;

namespace Dream.WebApp.Adapters
{
    public class SecuritizationTrancheModelAdapter
    {
        private Securitization _securitizationBusinessObject;

        private SecuritizationTrancheModel _securitizationTrancheModel;
        public SecuritizationTrancheModel SecuritizationTrancheModel => _securitizationTrancheModel;

        private FeeTrancheModel _feeTrancheModel;
        public FeeTrancheModel FeeTrancheModel => _feeTrancheModel;

        private ReserveAccountModel _reserveAccountModel;
        public ReserveAccountModel ReserveAccountModel => _reserveAccountModel;

        public SecuritizationTrancheModelAdapter(Securitization securitizationBusinessObject, bool isResecuritization)
        {
            _feeTrancheModel = new FeeTrancheModel { FeeTrancheModelEntries = new List<FeeTrancheModelEntry>() };
            _reserveAccountModel = new ReserveAccountModel { ReserveAccountModelEntries = new List<ReserveAccountModelEntry>() };         
            _securitizationTrancheModel = new SecuritizationTrancheModel { SecuritizationTrancheModelEntries = new List<SecuritizationTrancheModelEntry>() };

            var tranchesToBePaidOutAtRedemption = new List<Tranche>();
            var redemptionLogic = securitizationBusinessObject.RedemptionLogicList.SingleOrDefault(r => r is TranchesCanBePaidOutFromAvailableFundsRedemptionLogic);

            if (redemptionLogic != null)
            {
                var tranchesCanBePaidOutFromAvailableFundsRedemptionLogic = redemptionLogic as TranchesCanBePaidOutFromAvailableFundsRedemptionLogic;
                tranchesToBePaidOutAtRedemption = tranchesCanBePaidOutFromAvailableFundsRedemptionLogic.ListOfTranchesToBePaidOut;
            }

            var securitizationTrancheNodeStructs = securitizationBusinessObject.TranchesDictionary.Values;
            foreach (var securitizationTrancheNodeStruct in securitizationTrancheNodeStructs)
            {
                var securitizationTranche = securitizationTrancheNodeStruct.Tranche;
                var securitizationNode = securitizationTrancheNodeStruct.SecuritizationNode;

                var paysOutAtRedemption = tranchesToBePaidOutAtRedemption.Any(t => t.TrancheName == securitizationTranche.TrancheName);

                var securitizationFirstCashFlowDate = securitizationBusinessObject.Inputs.SecuritizationFirstCashFlowDate;
                if (securitizationTranche is FeeTranche)
                {
                    var isFeeShortFallInPriorityOfPayments = securitizationBusinessObject.PriorityOfPayments.OrderedListOfEntries
                        .Any(e => e.TrancheName == securitizationTranche.TrancheName 
                               && e.TrancheCashFlowType == TrancheCashFlowType.FeesShortfall);

                    FeeTrancheModelAdapter.AddInformationToFeeTrancheModel(
                        securitizationTranche, 
                        securitizationNode,
                        _feeTrancheModel, 
                        securitizationFirstCashFlowDate,
                        isFeeShortFallInPriorityOfPayments,
                        paysOutAtRedemption);
                }
                else if (securitizationTranche is ReserveFundTranche)
                {
                    var totalCollateralBalance = isResecuritization
                        ? ((Resecuritization) securitizationBusinessObject).GetResecuritizationCollateralBalance()
                        : securitizationBusinessObject.CollateralRetriever.GetTotalCollateralBalance();

                    ReserveAccountModelAdapter.AddInformationToReserveAccountModel(
                        securitizationTranche, 
                        securitizationNode, 
                        _reserveAccountModel,
                        securitizationFirstCashFlowDate,
                        totalCollateralBalance);
                }
                else
                {
                    AddInformationToSecuritizationTrancheModel(
                        securitizationTranche, 
                        securitizationNode, 
                        securitizationFirstCashFlowDate,
                        paysOutAtRedemption);
                }
            }
        }

        private void AddInformationToSecuritizationTrancheModel(
            Tranche securitizationTranche, 
            SecuritizationNodeTree securitizationNode, 
            DateTime securitizationFirstCashFlowDate,
            bool paysOutAtRedemption)
        {
            throw new NotImplementedException();
        }
    }
}