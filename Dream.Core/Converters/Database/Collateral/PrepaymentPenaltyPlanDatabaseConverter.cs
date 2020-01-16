using Dream.Common;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.BusinessLogic.ProjectedCashFlows;
using Dream.IO.Database.Entities.Collateral;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.Converters.Database.Collateral
{
    public class PrepaymentPenaltyPlanDatabaseConverter
    {
        private const string _percentagePenaltyAmount = "Percentage";
        private const string _dollarPenaltyAmount = "Dollars";

        private Dictionary<int, List<PrepaymentPenalty>> _prepaymentPenaltyDictionary;
        private Dictionary<int, string> _prepaymentPenaltyDescriptionDictionary;

        public PrepaymentPenaltyPlanDatabaseConverter(
            Dictionary<int, (string Description, List<PrepaymentPenaltyPlanDetailEntity> PrepaymentPenaltyPlanDetailEntities)> prepaymentPenaltyPlanDetailEntitiesDictionary)
        {
            _prepaymentPenaltyDictionary = new Dictionary<int, List<PrepaymentPenalty>>();
            _prepaymentPenaltyDescriptionDictionary = new Dictionary<int, string>();

            foreach (var prepaymentPenaltyPlanDetailEntry in prepaymentPenaltyPlanDetailEntitiesDictionary)
            {
                var prepaymentPenaltyPlanId = prepaymentPenaltyPlanDetailEntry.Key;
                var prepaymentPenaltyPlanDescription = prepaymentPenaltyPlanDetailEntry.Value.Description;

                _prepaymentPenaltyDescriptionDictionary.Add(prepaymentPenaltyPlanId, prepaymentPenaltyPlanDescription);

                foreach (var prepaymentPenaltyPlanDetail in prepaymentPenaltyPlanDetailEntry.Value.PrepaymentPenaltyPlanDetailEntities)
                {
                    var penaltyEndYear = prepaymentPenaltyPlanDetail.EndingMonthlyPeriodOfPenalty / Constants.MonthsInOneYear;

                    PrepaymentPenalty prepaymentPenalty;
                    if (prepaymentPenaltyPlanDetail.PenaltyType == _percentagePenaltyAmount)
                    {
                        prepaymentPenalty = new PrepaymentPenalty
                            (
                                prepaymentPenaltyPlanDetail.EndingMonthlyPeriodOfPenalty / Constants.MonthsInOneYear,
                                prepaymentPenaltyPlanDetail.PenaltyAmount,
                                0.0
                            );
                    }
                    else if (prepaymentPenaltyPlanDetail.PenaltyType == _dollarPenaltyAmount)
                    {
                        prepaymentPenalty = new PrepaymentPenalty
                            (
                                prepaymentPenaltyPlanDetail.EndingMonthlyPeriodOfPenalty / Constants.MonthsInOneYear,                               
                                0.0,
                                prepaymentPenaltyPlanDetail.PenaltyAmount
                            );
                    }
                    else
                    {
                        throw new Exception(string.Format("ERROR: No such penalty type of '{0}' is valid for prepayment penalty with description '{1}'",
                                prepaymentPenaltyPlanDetail.PenaltyType,
                                prepaymentPenaltyPlanDescription));
                    }

                    if (_prepaymentPenaltyDictionary.ContainsKey(prepaymentPenaltyPlanId))
                    {
                        if (_prepaymentPenaltyDictionary[prepaymentPenaltyPlanId].Any(p => p.PenaltyEndYear == penaltyEndYear))
                        {
                            throw new Exception(string.Format("ERROR: Cannot add duplicate ending year {0} for prepayment penalty with description '{1}'",
                                penaltyEndYear,
                                prepaymentPenaltyPlanDescription));
                        }
                        else
                        {
                            _prepaymentPenaltyDictionary[prepaymentPenaltyPlanId].Add(prepaymentPenalty);
                        }
                    }
                    else
                    {
                        _prepaymentPenaltyDictionary.Add(prepaymentPenaltyPlanId, new List<PrepaymentPenalty>());
                        _prepaymentPenaltyDictionary[prepaymentPenaltyPlanId].Add(prepaymentPenalty);
                    }
                }
            }
        }

        /// <summary>
        /// Assigns a loan's prepayment penalty plan based on the internal dictionary of the converter.
        /// </summary>
        public void AssignPrepaymentPenaltyPlan(Loan loan, int prepaymentPenaltyPlanId)
        {
            var prepaymentPenaltyDescription = _prepaymentPenaltyDescriptionDictionary[prepaymentPenaltyPlanId];

            if (_prepaymentPenaltyDictionary.ContainsKey(prepaymentPenaltyPlanId))
            {
                var prepaymentPenalties = _prepaymentPenaltyDictionary[prepaymentPenaltyPlanId];
                var prepaymentPenaltyPlan = new PrepaymentPenaltyPlan(prepaymentPenaltyDescription);

                prepaymentPenalties.ForEach(p => prepaymentPenaltyPlan.AddPrepaymentPenalty(p));
                loan.PrepaymentPenaltyPlan = prepaymentPenaltyPlan;
            }
            else
            {
                throw new Exception(string.Format("ERROR: No prepayment penalty plan was found with description '{0}'",
                     prepaymentPenaltyDescription));
            }
        }
    }
}
