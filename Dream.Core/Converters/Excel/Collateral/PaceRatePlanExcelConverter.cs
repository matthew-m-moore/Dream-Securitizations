using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.IO.Excel.Entities.CollateralTapeRecords;
using System;
using System.Collections.Generic;

namespace Dream.Core.Converters.Excel.Collateral
{
    public class PaceRatePlanExcelConverter
    {
        private Dictionary<string, Dictionary<int, PaceRatePlan>> _ratePlanMapping;

        public PaceRatePlanExcelConverter(List<RatePlanMappingRecord> listOfRatePlanMappingRecords)
        {
            _ratePlanMapping = new Dictionary<string, Dictionary<int, PaceRatePlan>>();
            foreach (var ratePlanMappingRecord in listOfRatePlanMappingRecords)
            {
                var ratePlanName = ratePlanMappingRecord.RatePlanName;
                var termInYears = ratePlanMappingRecord.TermInYears;
                var paceRatePlan = ConvertRatePlanMappingRecord(ratePlanMappingRecord);

                if (_ratePlanMapping.ContainsKey(ratePlanName))
                {
                    if (_ratePlanMapping[ratePlanName].ContainsKey(termInYears))
                    {
                        throw new Exception(string.Format("ERROR: There is a duplicate rate plan specified for maturity term {0} years with plan name '{1}'",
                            termInYears,
                            ratePlanName));
                    }
                    else
                    {
                        _ratePlanMapping[ratePlanName].Add(termInYears, paceRatePlan);
                    }
                }
                else
                {
                    _ratePlanMapping.Add(ratePlanName, new Dictionary<int, PaceRatePlan>());
                    _ratePlanMapping[ratePlanName].Add(termInYears, paceRatePlan);
                }
            }
        }

        /// <summary>
        /// Assigns the appropriate PACE rate plan per the rate plans provided. Note, this currently only affects the dealer buy-down rate used.
        /// </summary>
        public void AssignPaceRatePlan(PaceAssessment paceAssessment, int termInYears, string ratePlanName)
        {
            if (_ratePlanMapping.ContainsKey(ratePlanName))
            {
                var ratePlanSet = _ratePlanMapping[ratePlanName];
                if (ratePlanSet.ContainsKey(termInYears))
                {
                    paceAssessment.RatePlan = ratePlanSet[termInYears];
                }
                else
                {
                    throw new Exception(string.Format("ERROR: No rate plan was found for maturity term {0} years with plan name '{1}'",
                        termInYears,
                        ratePlanName));
                }
            }
            else
            {
                throw new Exception(string.Format("ERROR: No rate plan was found with plan name '{0}'",
                     ratePlanName));
            }
        }

        private PaceRatePlan ConvertRatePlanMappingRecord(RatePlanMappingRecord ratePlanMappingRecord)
        {
            return new PaceRatePlan
            {
                Description = ratePlanMappingRecord.RatePlanName,
                TermInYears = ratePlanMappingRecord.TermInYears,
                InterestRate = ratePlanMappingRecord.InterestRate,
                BuyDownRate = ratePlanMappingRecord.BuyDownRate,
            };
        }
    }
}
