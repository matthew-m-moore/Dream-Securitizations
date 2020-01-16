using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.IO.Database.Entities.Collateral;
using System;
using System.Collections.Generic;

namespace Dream.Core.Converters.Database.Collateral
{
    public class PaceRatePlanDatabaseConverter
    {
        private Dictionary<int, string> _ratePlanDescriptionDictionary;
        private Dictionary<int, Dictionary<int, PaceRatePlan>> _ratePlanMapping;  

        public PaceRatePlanDatabaseConverter(
            Dictionary<int, (string Description, List<PaceAssessmentRatePlanEntity> PaceAssessmentRatePlanEntities)> paceAssesmentRatePlanEntitiesDictionary)
        {
            _ratePlanDescriptionDictionary = new Dictionary<int, string>();
            _ratePlanMapping = new Dictionary<int, Dictionary<int, PaceRatePlan>>();

            foreach (var paceAssessmentRatePlanEntry in paceAssesmentRatePlanEntitiesDictionary)
            {
                var ratePlanTermSetId = paceAssessmentRatePlanEntry.Key;
                var ratePlanName = paceAssessmentRatePlanEntry.Value.Description;

                _ratePlanDescriptionDictionary.Add(ratePlanTermSetId, ratePlanName);

                foreach (var paceAssessmentRatePlanEntity in paceAssessmentRatePlanEntry.Value.PaceAssessmentRatePlanEntities)
                {
                    var termInYears = paceAssessmentRatePlanEntity.TermInYears;
                    var paceRatePlan = ConvertRatePlanMappingRecord(paceAssessmentRatePlanEntity);

                    if (_ratePlanMapping.ContainsKey(ratePlanTermSetId))
                    {
                        _ratePlanMapping[ratePlanTermSetId].Add(termInYears, paceRatePlan);
                    }
                    else
                    {
                        _ratePlanMapping.Add(ratePlanTermSetId, new Dictionary<int, PaceRatePlan>());
                        _ratePlanMapping[ratePlanTermSetId].Add(termInYears, paceRatePlan);
                    }
                }
            }
        }

        /// <summary>
        /// Assigns the appropriate PACE rate plan per the rate plans provided. Note, this currently only affects the dealer buy-down rate used.
        /// </summary>
        public void AssignPaceRatePlan(PaceAssessment paceAssessment, int termInYears, int ratePlanTermSetId)
        {
            var ratePlanName = _ratePlanDescriptionDictionary[ratePlanTermSetId];

            if (_ratePlanMapping.ContainsKey(ratePlanTermSetId))
            {
                var ratePlanSet = _ratePlanMapping[ratePlanTermSetId];
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

        private PaceRatePlan ConvertRatePlanMappingRecord(PaceAssessmentRatePlanEntity paceAssessmentRatePlanEntity)
        {
            var ratePlanName = _ratePlanDescriptionDictionary[paceAssessmentRatePlanEntity.PaceAssessmentRatePlanTermSetId];

            return new PaceRatePlan
            {
                Description = ratePlanName,
                TermInYears = paceAssessmentRatePlanEntity.TermInYears,
                InterestRate = paceAssessmentRatePlanEntity.CouponRate,
                BuyDownRate = paceAssessmentRatePlanEntity.BuyDownRate,
            };
        }
    }
}
