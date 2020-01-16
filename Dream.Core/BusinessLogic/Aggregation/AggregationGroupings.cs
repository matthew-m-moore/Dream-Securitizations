using Dream.Core.BusinessLogic.ProductTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.Aggregation
{
    /// <summary>
    /// A class for storing the aggregation groupings associated with string identifiers of specified products.
    /// </summary>
    public class AggregationGroupings
    {
        public const string TotalAggregationGroupingIdentifier = "TotalGrouping";
        public const string TotalAggregationGroupName = "Total";

        public static readonly string FundingDateAggregationGroupingIdentifier = "FundingDateAggregation";
        public static readonly string MaturityTermAggregationGroupingIdentifier = "MaturityTermAggregation";
        public static readonly string LoanLevelAggregationGroupingIdentifier = "LoanLevelAggregation";

        private Dictionary<string, Dictionary<string, string>> _aggregationGroupingsMappingDictionary;

        public AggregationGroupings()
        {
            _aggregationGroupingsMappingDictionary = new Dictionary<string, Dictionary<string, string>>();
        }

        private AggregationGroupings(Dictionary<string, Dictionary<string, string>> aggregationGroupingsMappingDictionary)
        {
            _aggregationGroupingsMappingDictionary = aggregationGroupingsMappingDictionary;
        }

        /// <summary>
        /// Returns a deep, member-wise copy of the object.
        /// </summary>
        public AggregationGroupings Copy()
        {
            var copiedMappingDictionary = _aggregationGroupingsMappingDictionary
                .ToDictionary(kvp1 => new string(kvp1.Key.ToCharArray()),
                              kvp1 => kvp1.Value.ToDictionary(kvp2 => new string(kvp2.Key.ToCharArray()),
                                                              kvp2 => new string(kvp2.Value.ToCharArray())));

            var aggregationGrouping = new AggregationGroupings(copiedMappingDictionary);
            return aggregationGrouping;
        }

        /// <summary>
        /// Adds a grouping for the total of all products to this object
        /// </summary>
        public void AddTotalAggregationGrouping()
        {
            // No need to add the total grouping if it has already been provided
            if (_aggregationGroupingsMappingDictionary.Values.Any(d => d.ContainsKey(TotalAggregationGroupingIdentifier))) return;

            foreach (var productIdentifier in _aggregationGroupingsMappingDictionary.Keys)
            {
                _aggregationGroupingsMappingDictionary[productIdentifier].Add(TotalAggregationGroupingIdentifier, TotalAggregationGroupName);
            }
        }

        /// <summary>
        /// Important Note: Attempting to use the getter here on an element that is not present will return an empty string.
        /// </summary>
        public string this[string productIdentifier, string groupingIdentifier]
        {
            get
            {
                if (_aggregationGroupingsMappingDictionary.ContainsKey(productIdentifier) &&
                    _aggregationGroupingsMappingDictionary[productIdentifier].ContainsKey(groupingIdentifier))
                {
                    return _aggregationGroupingsMappingDictionary[productIdentifier][groupingIdentifier];
                }

                return string.Empty;
            }

            set
            {
                if (_aggregationGroupingsMappingDictionary.ContainsKey(productIdentifier))
                {
                    if (_aggregationGroupingsMappingDictionary[productIdentifier].ContainsKey(groupingIdentifier))
                    {
                        _aggregationGroupingsMappingDictionary[productIdentifier][groupingIdentifier] = value;
                    }
                    else
                    {
                        _aggregationGroupingsMappingDictionary[productIdentifier].Add(groupingIdentifier, value);
                    }
                }
                else
                {
                    var mappingForSpecificGrouping = new Dictionary<string, string>();
                    mappingForSpecificGrouping.Add(groupingIdentifier, value);

                    _aggregationGroupingsMappingDictionary.Add(productIdentifier, mappingForSpecificGrouping);
                }
            }
        }

        public Dictionary<string, List<string>> GetDictionaryOfIdentifersByGrouping()
        {
            var dictionaryOfIdentifiersByGrouping = new Dictionary<string, List<string>>();

            var distinctListOfGroupingIdentifiers = GetDistinctGroupingIdentifiers();
            foreach(var distinctGroupingIdentifer in distinctListOfGroupingIdentifiers)
            {
                var listOfIdentifiersForGrouping = GetAllIdentifiersAssociatedWithGivenGrouping(distinctGroupingIdentifer);
                dictionaryOfIdentifiersByGrouping.Add(distinctGroupingIdentifer, listOfIdentifiersForGrouping);
            }

            return dictionaryOfIdentifiersByGrouping;
        }

        public List<string> GetDistinctGroupingIdentifiers()
        {
            var distinctListOfGroupingIdentifiers = new List<string>();
            distinctListOfGroupingIdentifiers = _aggregationGroupingsMappingDictionary
                .Values.SelectMany(kvp => kvp.Values)
                .Distinct().ToList();

            return distinctListOfGroupingIdentifiers;
        }

        public List<string> GetAllIdentifiersAssociatedWithGivenGrouping(string groupingIdentifier)
        {
            var listOfIdentifiers = new List<string>();
            listOfIdentifiers = _aggregationGroupingsMappingDictionary
                .Where(entry => entry.Value.Values.Contains(groupingIdentifier))
                .Select(kvp => kvp.Key).ToList();

            return listOfIdentifiers;
        }

        /// <summary>
        /// Sets up a default "Total"  and "Maturity Term" aggregation groupings for cases where no grouping was provided or selected.
        /// </summary>
        public static AggregationGroupings SetupAutomaticTotalCollateralAggregation(List<Loan> loans)
        {
            var aggregationGroupings = new AggregationGroupings();

            // Note, any future attempt to add the total aggregation grouping will be skipped, not adding computational time
            foreach (var loan in loans)
            {
                var productIdentifier = loan.StringId;
                aggregationGroupings[productIdentifier, TotalAggregationGroupingIdentifier] = TotalAggregationGroupName;
                aggregationGroupings[productIdentifier, MaturityTermAggregationGroupingIdentifier] = loan.MaturityTermInYears.ToString("00") + "-Yr Term";
            }

            return aggregationGroupings;
        }

        /// <summary>
        /// Sets up a default "Total" and loan-level aggregation groupings for cases where no grouping was provided or selected.
        /// </summary>
        public static AggregationGroupings SetupAutomaticLoanLevelAggregation(List<Loan> loans)
        {
            var aggregationGroupings = new AggregationGroupings();

            // Note, any future attempt to add the total aggregation grouping will be skipped, not adding computational time
            foreach (var loan in loans)
            {
                var productIdentifier = loan.StringId;
                aggregationGroupings[productIdentifier, TotalAggregationGroupingIdentifier] = TotalAggregationGroupName;
                aggregationGroupings[productIdentifier, LoanLevelAggregationGroupingIdentifier] = productIdentifier;
            }

            return aggregationGroupings;
        }

        /// <summary>
        /// Sets up a default aggreagtion groupings for the warehouse mark to market analysis.
        /// </summary>
        public static AggregationGroupings SetupAutomaticPaceAssessmentFundingDateAggregation(List<Loan> paceAssessments)
        {
            var aggregationGroupings = new AggregationGroupings();

            // Note, any future attempt to add the total aggregation grouping will be skipped, not adding computational time
            foreach (var paceAssessment in paceAssessments)
            {
                var castedPaceAssessment = paceAssessment as PaceAssessment;
                if (castedPaceAssessment == null)
                {
                    throw new Exception("ERROR: All loans must be PACE assessments to use the warehouse mark-to-market automatic funding date aggregation feature");
                }

                var productIdentifier = paceAssessment.StringId;
                aggregationGroupings[productIdentifier, FundingDateAggregationGroupingIdentifier] = "Funded: " + castedPaceAssessment.FundingDate.ToString("yyyy-MM-dd");
            }

            return aggregationGroupings;
        }
    }
}
