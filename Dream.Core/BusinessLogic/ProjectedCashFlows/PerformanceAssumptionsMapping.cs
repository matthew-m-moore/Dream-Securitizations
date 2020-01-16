using Dream.Common.Enums;
using Dream.Core.BusinessLogic.ProductTypes;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.ProjectedCashFlows
{
    /// <summary>
    /// A class for storing the mapping of performance assumptions to string identifiers.
    /// </summary>
    public class PerformanceAssumptionsMapping
    {
        private Dictionary<string, Dictionary<string, Dictionary<PerformanceCurveType, string>>> _performanceAssumptionsMappingDictionary;

        public PerformanceAssumptionsMapping()
        {
            _performanceAssumptionsMappingDictionary = new Dictionary<string, Dictionary<string, Dictionary<PerformanceCurveType, string>>>();
        }

        protected PerformanceAssumptionsMapping(Dictionary<string, Dictionary<string, Dictionary<PerformanceCurveType, string>>> performanceAssumptionsMappingDictionary)
        {
            _performanceAssumptionsMappingDictionary = performanceAssumptionsMappingDictionary;
        }

        /// <summary>
        /// Returns a list of all the assumptions identifiers in the underlying performance assumptions mapping.
        /// </summary>
        public List<string> GetAllAssumptionsIdentifiers(string assumptionsGrouping)
        {
            if (!_performanceAssumptionsMappingDictionary.ContainsKey(assumptionsGrouping)) return new List<string>();
            return _performanceAssumptionsMappingDictionary[assumptionsGrouping].Keys.ToList();
        }

        /// <summary>
        /// Returns a list of all the assumptions groupings in the underlying performance assumptions mapping.
        /// </summary>
        public List<string> GetAllAssumptionsGroupings()
        {
            return _performanceAssumptionsMappingDictionary.Keys.ToList();
        }

        /// <summary>
        /// Returns a deep, member-wise copy of the object.
        /// </summary>
        public virtual PerformanceAssumptionsMapping Copy()
        {
            var copiedMappingDictionary = CopyMappingDictionary();
            var projectedPerformanceAssumptions = new PerformanceAssumptionsMapping(copiedMappingDictionary);
            return projectedPerformanceAssumptions;
        }

        /// <summary>
        /// Important Note: Attempting to use the getter here on an element that is not present will return an empty string.
        /// </summary>
        public string this[string assumptionsGrouping, string assumptionsIdentifier, PerformanceCurveType performanceCurveType]
        {
            get
            {
                if (_performanceAssumptionsMappingDictionary.ContainsKey(assumptionsGrouping) &&
                    _performanceAssumptionsMappingDictionary[assumptionsGrouping].ContainsKey(assumptionsIdentifier) &&
                    _performanceAssumptionsMappingDictionary[assumptionsGrouping][assumptionsIdentifier].ContainsKey(performanceCurveType))
                {
                    return _performanceAssumptionsMappingDictionary[assumptionsGrouping][assumptionsIdentifier][performanceCurveType];
                }

                return string.Empty;
            }

            set
            {
                if (!_performanceAssumptionsMappingDictionary.ContainsKey(assumptionsGrouping))
                {
                    _performanceAssumptionsMappingDictionary.Add(assumptionsGrouping, new Dictionary<string, Dictionary<PerformanceCurveType, string>>());
                }

                if (_performanceAssumptionsMappingDictionary[assumptionsGrouping].ContainsKey(assumptionsIdentifier))
                {
                    if (_performanceAssumptionsMappingDictionary[assumptionsGrouping][assumptionsIdentifier].ContainsKey(performanceCurveType))
                    {
                        _performanceAssumptionsMappingDictionary[assumptionsGrouping][assumptionsIdentifier][performanceCurveType] = value;
                    }
                    else
                    {
                        _performanceAssumptionsMappingDictionary[assumptionsGrouping][assumptionsIdentifier].Add(performanceCurveType, value);
                    }
                }
                else
                {
                    var performanceTypeDictionary = new Dictionary<PerformanceCurveType, string>();
                    performanceTypeDictionary.Add(performanceCurveType, value);

                    _performanceAssumptionsMappingDictionary[assumptionsGrouping].Add(assumptionsIdentifier, performanceTypeDictionary);
                }
            }
        }

        /// <summary>
        /// Returns the underlying dictionary of performance assumption assignments for a given assumptions identifer
        /// </summary>
        public Dictionary<PerformanceCurveType, string> this[string assumptionsGrouping, string assumptionsIdentifier]
        {
            get
            {
                return _performanceAssumptionsMappingDictionary[assumptionsGrouping][assumptionsIdentifier];
            }
        }

        /// <summary>
        /// Sets up a globally applied performance assumptions mapping to all loans provided.
        /// </summary>
        public virtual void SetupGlobalPerformanceAssumptionsMapping(string globalPerformanceAssumptionGrouping, string globalPerformanceAssumption, List<Loan> loans)
        {
            foreach (var loan in loans)
            {
                var assumptionsIdentifier = loan.StringId;
                var performanceCurveName = globalPerformanceAssumption;

                foreach (var performanceCurveType in ProjectedCashFlowLogic.ListOfPerformanceCurveTypes)
                {
                    this[globalPerformanceAssumptionGrouping, assumptionsIdentifier, performanceCurveType] = performanceCurveName;
                }
            }
        }

        protected Dictionary<string, Dictionary<string, Dictionary<PerformanceCurveType, string>>> CopyMappingDictionary()
        {
            var copiedMappingDictionary = _performanceAssumptionsMappingDictionary
                .ToDictionary(kvp1 => new string(kvp1.Key.ToCharArray()),
                              kvp1 => kvp1.Value.ToDictionary(kvp2 => new string(kvp2.Key.ToCharArray()),
                                                              kvp2 => kvp2.Value.ToDictionary(kvp3 => kvp3.Key,
                                                                                              kvp3 => new string(kvp3.Value.ToCharArray()))));

            return copiedMappingDictionary;
        }
    }
}
