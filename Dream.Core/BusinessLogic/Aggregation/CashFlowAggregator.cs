using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.BusinessLogic.Valuation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.Aggregation
{
    /// <summary>
    /// A class for aggregating lists of cash flow objects according to some specified aggregation grouping.
    /// </summary>
    public class CashFlowAggregator
    {
        private AggregationGroupings _aggregationGroupings;
        private CashFlowGenerationInput _cashFlowGenerationInputs;

        /// <summary>
        /// This is the publically accessible selected aggregation grouping that can be change as needed from outside this class.
        /// </summary>
        public string SelectedAggregationGrouping { get; set; }

        public CashFlowAggregator(AggregationGroupings aggregationGroupings, string selectedAggregationGrouping)
        {
            _aggregationGroupings = aggregationGroupings;
            SelectedAggregationGrouping = selectedAggregationGrouping;
        }

        public CashFlowAggregator(CashFlowGenerationInput cashFlowGenerationInputs, AggregationGroupings aggregationGroupings, string selectedAggregationGrouping)
            : this(aggregationGroupings, selectedAggregationGrouping)
        {
            _cashFlowGenerationInputs = cashFlowGenerationInputs;
        }

        /// <summary>
        /// Aggregates a multpile lists of cash flows into a single list of cash flows. Assumes the cash flow dates of each list are identical.
        /// </summary>
        public static List<T> AggregateCashFlows<T>(List<List<T>> listOfCashFlows) where T : CashFlow
        {
            var listOfAggregatedCashFlows = new List<T>();
            foreach (var listOfCashFlow in listOfCashFlows)
            {
                if (!listOfCashFlow.Any()) continue;

                var cashFlowsToAggregate = listOfCashFlow.Select(c => c.Copy() as T).ToList();
                var countOfCashFlowsToAdd = cashFlowsToAggregate.Count;
                var countOfAggregatedCashFlows = listOfAggregatedCashFlows.Count;
                var maximumCashFlowCount = Math.Max(countOfCashFlowsToAdd, countOfAggregatedCashFlows);

                for (var cashFlowCounter = 0; cashFlowCounter < maximumCashFlowCount; cashFlowCounter++)
                {
                    // If the algorithm runs out of cash flows to add, break out of the loop
                    if (cashFlowCounter >= countOfCashFlowsToAdd) break;

                    // For cash flows that extend beyond the existing that are aggregated, add them
                    var cashFlowToAggregate = cashFlowsToAggregate[cashFlowCounter];
                    if (cashFlowCounter >= countOfAggregatedCashFlows)
                    {
                        listOfAggregatedCashFlows.Add(cashFlowToAggregate);
                    }
                    // Otherwise, perform the aggregation
                    else
                    {
                        listOfAggregatedCashFlows[cashFlowCounter].Aggregate(cashFlowToAggregate);
                    }
                }
            }

            return listOfAggregatedCashFlows;
        }

        /// <summary>
        /// Aggregates a list of provided cash flows to a set of dates specified, where the cash flows after the first cash flow date are
        /// assumed to be one month apart therafter.
        /// </summary>
        public static List<T> AggregateCashFlowsByDate<T>(List<T> listOfCashFlows, DateTime startDate, DateTime firstCashFlowDate)
            where T : ContractualCashFlow, new()
        {
            // Note that going one period beyond the max date will make sure it is captured in the aggregation
            var maxPeriodDate = listOfCashFlows.Max(c => c.PeriodDate).AddMonths(1);
            var listOfDates = new List<DateTime>();
            if (startDate.Ticks != firstCashFlowDate.Ticks) listOfDates.Add(startDate);

            if (firstCashFlowDate < startDate || maxPeriodDate < startDate)
            {
                throw new Exception("ERROR: Cash flows cannot be aggregated when start date comes after first cash flow date or last cash flow date comes before start date");
            }

            var nextDate = firstCashFlowDate;
            while(nextDate <= maxPeriodDate)
            {
                listOfDates.Add(nextDate);
                nextDate = nextDate.AddMonths(1);
            }

            var dateAggregatedCashFlows = AggregateCashFlowsByDate(listOfCashFlows, listOfDates);
            return dateAggregatedCashFlows;
        }

        /// <summary>
        /// Aggregates a list of provided cash flows to a list of dates specified.
        /// </summary>
        public static List<T> AggregateCashFlowsByDate<T>(List<T> listOfCashFlows, List<DateTime> listOfDates)
            where T : ContractualCashFlow, new()
        {
            var numberOfDates = listOfDates.Count();
            if (numberOfDates < 2)
            {
                throw new Exception("ERROR: Cannot aggregate cash flows between less than two dates.");
            }

            var orderedListOfDates = listOfDates.OrderBy(d => d).ToList();
            var firstCashFlow = listOfCashFlows.First().Copy();
            var firstCashFlowDate = new DateTime(firstCashFlow.PeriodDate.Ticks);
            var firstOrderedDate = orderedListOfDates.First();
            if (firstCashFlowDate.Ticks != firstOrderedDate.Ticks) firstCashFlow.Clear();

            // First, maintain the sanctity of the zeroth cash flow, so that it has zero payments
            var dateAggregatedCashFlows = new List<T>();
            var zerothDateAggregatedCashFlow = firstCashFlow;           
            zerothDateAggregatedCashFlow.PeriodDate = firstOrderedDate;
            dateAggregatedCashFlows.Add(zerothDateAggregatedCashFlow as T);

            for (var dateCounter = 1; dateCounter < numberOfDates; dateCounter++)
            {
                // Now, in order not to miss any cash flows, make sure to grab
                // the very first date in the original list of cash flows or otherwise
                var lastOrderedDate = orderedListOfDates[dateCounter - 1];
                if (dateCounter == 1 && firstCashFlowDate < firstOrderedDate)
                {
                    lastOrderedDate = firstCashFlowDate.AddDays(-1);
                }

                var thisOrderedDate = orderedListOfDates[dateCounter];

                var cashFlowsSpanningDates = listOfCashFlows
                    .Where(c => c.PeriodDate > lastOrderedDate
                             && c.PeriodDate <= thisOrderedDate).ToList();

                T dateAggregatedCashFlow;
                if (cashFlowsSpanningDates.Any())
                {
                    dateAggregatedCashFlow = (T) cashFlowsSpanningDates.First().Copy();
                    foreach (var spanningCashFlow in cashFlowsSpanningDates.Skip(1))
                    {
                        dateAggregatedCashFlow.Aggregate(spanningCashFlow);
                    }

                    dateAggregatedCashFlow.StartingBalance = cashFlowsSpanningDates.First().StartingBalance;
                    dateAggregatedCashFlow.EndingBalance = cashFlowsSpanningDates.Last().EndingBalance;
                    dateAggregatedCashFlow.Count = cashFlowsSpanningDates.Last().Count;

                    if (dateAggregatedCashFlow.StartingBalance > 0.0)
                        dateAggregatedCashFlow.BondCount = cashFlowsSpanningDates.Last().BondCount;
                }
                else
                {
                    dateAggregatedCashFlow = new T();
                    dateAggregatedCashFlow.StartingBalance = dateAggregatedCashFlows.Last().EndingBalance;
                    dateAggregatedCashFlow.EndingBalance = dateAggregatedCashFlow.StartingBalance;
                    dateAggregatedCashFlow.Count = dateAggregatedCashFlows.Last().Count;

                    if (dateAggregatedCashFlow.StartingBalance > 0.0)
                        dateAggregatedCashFlow.BondCount = dateAggregatedCashFlows.Last().BondCount;
                }

                dateAggregatedCashFlow.Period = dateCounter;
                dateAggregatedCashFlow.PeriodDate = thisOrderedDate;
                dateAggregatedCashFlows.Add(dateAggregatedCashFlow);
            }

            return dateAggregatedCashFlows;
        }

        /// <summary>
        /// Aggregates a list of provided cash flows to one total cash flow for a provided list of products.
        /// </summary>
        public KeyValuePair<string, List<T2>> AggregateTotalCashFlows<T1, T2>(List<T1> listOfProducts, List<List<T2>> listOfCashFlows)
            where T1 : Product
            where T2 : ContractualCashFlow, new()
        {
            // Add the total aggregation grouping and store the current grouping
            _aggregationGroupings.AddTotalAggregationGrouping();
            var storedSelectedAggregationGrouping = string.Copy(SelectedAggregationGrouping);
            SelectedAggregationGrouping = AggregationGroupings.TotalAggregationGroupingIdentifier;

            var totalAggregatedCashFlowsDictionary = AggregateCashFlows(listOfProducts, listOfCashFlows);
            KeyValuePair<string, List<T2>> totalAggregatedCashFlowsKeyValuePair;
            try
            {
                totalAggregatedCashFlowsKeyValuePair = totalAggregatedCashFlowsDictionary.Single();
            }
            catch
            {
                throw new Exception("ERROR: The total aggregation grouping supplied does not result in a single grouping of all cash flows");
            }

            // Return the stored aggregation groping to its proper place
            SelectedAggregationGrouping = storedSelectedAggregationGrouping;
            return totalAggregatedCashFlowsKeyValuePair;
        }

        /// <summary>
        /// Aggregates a list of provided cash flows for a provided list of products.
        /// </summary>
        public Dictionary<string, List<T2>> AggregateCashFlows<T1, T2>(List<T1> listOfProducts, List<List<T2>> listOfCashFlows) 
            where T1 : Product 
            where T2 : ContractualCashFlow, new()
        {
            var numberOfProducts = listOfProducts.Count();
            var numberOfCashFlows = listOfCashFlows.Count();

            if (numberOfProducts != numberOfCashFlows)
            {
                throw new Exception("ERROR: Number of products does not match number of cash flows provided for aggregation");
            }

            var listOfProductCashFlows = new List<(T1 Product, List<T2> CashFlows)>();
            for (var productCounter = 0; productCounter < numberOfProducts; productCounter++)
            {
                var product = listOfProducts[productCounter];
                var cashFlows = listOfCashFlows[productCounter];

                if (_cashFlowGenerationInputs != null)
                {
                    var startDate = _cashFlowGenerationInputs.CashFlowStartDate;
                    var firstCashFlowDate = _cashFlowGenerationInputs.InterestAccrualStartDate;
                    cashFlows = AggregateCashFlowsByDate(cashFlows, startDate, firstCashFlowDate);
                }

                var productCashFlows = (Product: product, CashFlows: cashFlows);
                listOfProductCashFlows.Add(productCashFlows);
            }

            var dictionaryOfAggregatedResults = AggregateCashFlows(listOfProductCashFlows);
            return dictionaryOfAggregatedResults;
        }

        private Dictionary<string, List<T2>> AggregateCashFlows<T1, T2>(List<(T1 Product, List<T2> CashFlows)> listOfProductCashFlows)
            where T1 : Product
            where T2 : ContractualCashFlow, new()
        {
            var dictionaryOfAggregatedResults = new Dictionary<string, List<T2>>();
            foreach (var productCashFlows in listOfProductCashFlows)
            {
                var product = productCashFlows.Product;
                var productIdentifier = product.StringId;
                var groupingIdentifier = _aggregationGroupings[productIdentifier, SelectedAggregationGrouping];

                if (groupingIdentifier == string.Empty)
                {
                    throw new Exception(string.Format("ERROR: No aggregation grouping identifer found for product {0} in selected aggregation grouping {1}.",
                        productIdentifier,
                        SelectedAggregationGrouping));
                }

                // Need to use ToList() here to create a new list of cash flows and avoid a pass-by-reference
                var cashFlowsToAggregate = productCashFlows.CashFlows.Select(c => c.Copy() as T2).ToList();

                if (!dictionaryOfAggregatedResults.ContainsKey(groupingIdentifier))
                {
                    dictionaryOfAggregatedResults.Add(groupingIdentifier, cashFlowsToAggregate);
                    continue;
                }

                var countOfCashFlowsToAdd = cashFlowsToAggregate.Count;
                var countOfAggregatedCashFlows = dictionaryOfAggregatedResults[groupingIdentifier].Count;
                var maximumCashFlowCount = Math.Max(countOfCashFlowsToAdd, countOfAggregatedCashFlows);

                for(var cashFlowCounter = 0; cashFlowCounter < maximumCashFlowCount; cashFlowCounter++)
                {
                    // If the algorithm runs out of cash flows to add, break out of the loop
                    if (cashFlowCounter >= countOfCashFlowsToAdd) break;

                    // For cash flows that extend beyond the existing that are aggregated, add them
                    var cashFlowToAggregate = cashFlowsToAggregate[cashFlowCounter];
                    if (cashFlowCounter >= countOfAggregatedCashFlows)
                    {
                        dictionaryOfAggregatedResults[groupingIdentifier].Add(cashFlowToAggregate);
                    }
                    // Otherwise, perform the aggregation
                    else
                    {
                        dictionaryOfAggregatedResults[groupingIdentifier][cashFlowCounter].Aggregate(cashFlowToAggregate);
                    }                    
                }
            }

            return dictionaryOfAggregatedResults;
        }
    }
}
