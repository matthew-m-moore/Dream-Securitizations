using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.BusinessLogic.ProjectedCashFlows;
using Dream.IO.Excel.Entities.CollateralTapeRecords;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.Converters.Excel.Collateral
{
    public class PrepaymentPenaltyPlanExcelConverter
    {
        private Dictionary<string, List<PrepaymentPenalty>> _prepaymentPenaltyDictionary;

        public PrepaymentPenaltyPlanExcelConverter(List<PrepaymentPenaltyRecord> listOfPrepaymentPenaltyRecords)
        {
            _prepaymentPenaltyDictionary = new Dictionary <string, List<PrepaymentPenalty>>();
            foreach (var prepaymentPenaltyRecord in listOfPrepaymentPenaltyRecords)
            {
                var prepaymentPenaltyPlanDescription = prepaymentPenaltyRecord.Description;
                var penaltyEndYear = prepaymentPenaltyRecord.PenaltyEndYear;
                var prepaymentPenalty = ConvertPrepaymentPenaltyRecord(prepaymentPenaltyRecord);

                if (_prepaymentPenaltyDictionary.ContainsKey(prepaymentPenaltyPlanDescription))
                {
                    if (_prepaymentPenaltyDictionary[prepaymentPenaltyPlanDescription].Any(p => p.PenaltyEndYear == penaltyEndYear))
                    {
                        throw new Exception(string.Format("ERROR: Cannot add duplicate ending year {0} for prepayment penalty with description '{1}'",
                            penaltyEndYear,
                            prepaymentPenaltyPlanDescription));
                    }
                    else
                    {
                        _prepaymentPenaltyDictionary[prepaymentPenaltyPlanDescription].Add(prepaymentPenalty);
                    }
                }
                else
                {
                    _prepaymentPenaltyDictionary.Add(prepaymentPenaltyPlanDescription, new List<PrepaymentPenalty>());
                    _prepaymentPenaltyDictionary[prepaymentPenaltyPlanDescription].Add(prepaymentPenalty);
                }
            }
        }

        /// <summary>
        /// Assigns a loan's prepayment penalty plan based on the internal dictionary of the converter.
        /// </summary>
        public void AssignPrepaymentPenaltyPlan(Loan loan, string prepaymentPenaltyPlanDescription)
        {
            if (_prepaymentPenaltyDictionary.ContainsKey(prepaymentPenaltyPlanDescription))
            {
                var prepaymentPenalties = _prepaymentPenaltyDictionary[prepaymentPenaltyPlanDescription];
                var prepaymentPenaltyPlan = new PrepaymentPenaltyPlan(prepaymentPenaltyPlanDescription);

                prepaymentPenalties.ForEach(p => prepaymentPenaltyPlan.AddPrepaymentPenalty(p));
                loan.PrepaymentPenaltyPlan = prepaymentPenaltyPlan;
            }
            else
            {
                throw new Exception(string.Format("ERROR: No prepayment penalty plan was found with description '{0}'",
                     prepaymentPenaltyPlanDescription));
            }
        }

        private PrepaymentPenalty ConvertPrepaymentPenaltyRecord(PrepaymentPenaltyRecord prepaymentPenaltyRecord)
        {
            return new PrepaymentPenalty(
                prepaymentPenaltyRecord.PenaltyEndYear,
                prepaymentPenaltyRecord.PenaltyAmount,
                0.0);
        }
    }
}
