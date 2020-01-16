using Dream.Common.Curves;
using Dream.Common.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.ProjectedCashFlows
{
    /// <summary>
    /// A class for storing and retreiving projected performance assumptions such as CPR and CDR.
    /// </summary>
    public class ProjectedPerformanceAssumptions
    {
        // It should be possible to swap out the mapping on the fly for scenarios, so it is best to make this property public
        public PerformanceAssumptionsMapping PerformanceAssumptionsMapping { get; set; }

        private Dictionary<string, Dictionary<PerformanceCurveType, PerformanceCurve>> _performanceAssumptionsDictionary;

        public ProjectedPerformanceAssumptions()
        {
            PerformanceAssumptionsMapping = new PerformanceAssumptionsMapping();
            _performanceAssumptionsDictionary = new Dictionary<string, Dictionary<PerformanceCurveType, PerformanceCurve>>();
        }

        private ProjectedPerformanceAssumptions(Dictionary<string, Dictionary<PerformanceCurveType, PerformanceCurve>> performanceAssumptionsDictionary)
        {
            _performanceAssumptionsDictionary = performanceAssumptionsDictionary;
        }

        /// <summary>
        /// Returns a deep, member-wise copy of the object.
        /// </summary>
        public ProjectedPerformanceAssumptions Copy()
        {
            var copiedAssumptionsDictionary = _performanceAssumptionsDictionary
                .ToDictionary(kvp1 => new string(kvp1.Key.ToCharArray()),
                              kvp1 => kvp1.Value.ToDictionary(kvp2 => kvp2.Key,
                                                              kvp2 => kvp2.Value.Copy()));

            var projectedPerformanceAssumptions = new ProjectedPerformanceAssumptions(copiedAssumptionsDictionary)
            {
                PerformanceAssumptionsMapping = PerformanceAssumptionsMapping.Copy()
            };

            return projectedPerformanceAssumptions;
        }

        /// <summary>
        /// Returns a list of all the performance curve names in the underlying projected performance assumptions.
        /// </summary>
        public List<string> GetAllPerformanceCurveNames()
        {
            return _performanceAssumptionsDictionary.Keys.ToList();
        }

        /// <summary>
        /// Converts all internal performance curves to annualized rates and changes performance curve types accordingly.
        /// </summary>
        public void ConvertAllCurvesToAnnualRate()
        {
            foreach (var performanceCurveName in _performanceAssumptionsDictionary.Keys)
            {
                var performanceCurveTypes = _performanceAssumptionsDictionary[performanceCurveName].Keys.ToList();

                foreach (var performanceCurveType in performanceCurveTypes)
                {
                    var newPerformanceCurveType = 
                        _performanceAssumptionsDictionary[performanceCurveName][performanceCurveType].ConvertToAnnualRate();

                    ReplacePerformanceCurveType(performanceCurveName, newPerformanceCurveType, performanceCurveType);
                }
            }
        }

        /// <summary>
        /// Converts all internal performance curves to montly rates and changes performance curve types accordingly.
        /// </summary>
        public void ConvertAllCurvesToMonthlyRate()
        {
            foreach (var performanceCurveName in _performanceAssumptionsDictionary.Keys)
            {
                var performanceCurveTypes = _performanceAssumptionsDictionary[performanceCurveName].Keys.ToList();

                foreach (var performanceCurveType in performanceCurveTypes)
                {
                    var newPerformanceCurveType =
                        _performanceAssumptionsDictionary[performanceCurveName][performanceCurveType].ConvertToMonthlyRate();

                    ReplacePerformanceCurveType(performanceCurveName, newPerformanceCurveType, performanceCurveType);
                }
            }
        }

        /// <summary>
        /// Retrieves the proper performance curves according to the mapping provided for all standard curve types.
        /// </summary>
        public Dictionary<PerformanceCurveType, Curve<double>> GetPerformanceCurves(
            List<PerformanceCurveType> listOfPerformanceCurveTypes,
            string assumptionsGrouping,
            string assumptionsIdentifier)
        {
            var performanceCurveDictionary = new Dictionary<PerformanceCurveType, Curve<double>>();

            foreach (var performanceCurveType in listOfPerformanceCurveTypes)
            {
                var performanceCurve = GetPerformanceCurve(assumptionsGrouping, assumptionsIdentifier, performanceCurveType);
                performanceCurveDictionary.Add(performanceCurveType, performanceCurve);
            }

            return performanceCurveDictionary;
        }

        /// <summary>
        /// Retrieves the proper performance curve according to the mapping provided for the given curve type.
        /// </summary>
        public Curve<double> GetPerformanceCurve(string assumptionsGrouping, string assumptionsIdentifier, PerformanceCurveType performanceCurveType)
        {
            var performanceCurveName = PerformanceAssumptionsMapping[assumptionsGrouping, assumptionsIdentifier, performanceCurveType];
            var performanceCurve = this[performanceCurveName, performanceCurveType];

            return performanceCurve;
        }

        /// <summary>
        /// Important Note: Attempting to use the getter here on an element that is not present will return a curve of zeroes.
        /// Additionally, this getter will populate a space for future curves with a curve of zeroes.
        /// </summary>
        public Curve<double> this[string performanceCurveName, PerformanceCurveType performanceCurveType]
        {
            get
            {
                lock(_performanceAssumptionsDictionary)
                {
                    if (_performanceAssumptionsDictionary.ContainsKey(performanceCurveName) &&
                        _performanceAssumptionsDictionary[performanceCurveName].ContainsKey(performanceCurveType))
                    {
                        return _performanceAssumptionsDictionary[performanceCurveName][performanceCurveType].Vector;
                    }

                    var performanceCurve = new PerformanceCurve(performanceCurveType, new Curve<double>(0.0));

                    if (_performanceAssumptionsDictionary.ContainsKey(performanceCurveName))
                    {
                        _performanceAssumptionsDictionary[performanceCurveName].Add(performanceCurveType, performanceCurve);
                    }
                    else
                    {
                        var performanceCurveDictionary = new Dictionary<PerformanceCurveType, PerformanceCurve>();
                        performanceCurveDictionary.Add(performanceCurveType, performanceCurve);

                        _performanceAssumptionsDictionary.Add(performanceCurveName, performanceCurveDictionary);
                    }

                    return _performanceAssumptionsDictionary[performanceCurveName][performanceCurveType].Vector;
                }
            }

            set
            {
                var performanceCurve = new PerformanceCurve(performanceCurveType, value);              

                if (_performanceAssumptionsDictionary.ContainsKey(performanceCurveName))
                {
                    if (_performanceAssumptionsDictionary[performanceCurveName].ContainsKey(performanceCurveType))
                    {
                        _performanceAssumptionsDictionary[performanceCurveName][performanceCurveType] = performanceCurve;
                    }
                    else
                    {
                        _performanceAssumptionsDictionary[performanceCurveName].Add(performanceCurveType, performanceCurve);
                    }
                }
                else
                {
                    var performanceCurveDictionary = new Dictionary<PerformanceCurveType, PerformanceCurve>();
                    performanceCurveDictionary.Add(performanceCurveType, performanceCurve);

                    _performanceAssumptionsDictionary.Add(performanceCurveName, performanceCurveDictionary);
                }
            }
        }

        /// <summary>
        /// Returns the specific value of a specific curve at a specific monthly period number
        /// </summary>
        public double this[string performanceCurveName, PerformanceCurveType performanceCurveType, int periodNumber]
        {
            get
            {
                return this[performanceCurveName, performanceCurveType][periodNumber];
            }
            set
            {
                var performanceCurve = this[performanceCurveName, performanceCurveType];
                performanceCurve[periodNumber] = value;
            }
        }

        /// <summary>
        /// Returns the underlying dictionary of performance curves for a given performance curve name
        /// </summary>
        public Dictionary<PerformanceCurveType, PerformanceCurve> this[string performanceCurveName]
        {
            get
            {
                return _performanceAssumptionsDictionary[performanceCurveName];
            }
        }

        private void ReplacePerformanceCurveType(string curveName, PerformanceCurveType newType, PerformanceCurveType oldType)
        {
            if (newType != oldType)
            {
                var newCurve = _performanceAssumptionsDictionary[curveName][oldType];
                _performanceAssumptionsDictionary[curveName].Remove(oldType);
                _performanceAssumptionsDictionary[curveName].Add(newType, newCurve);
            }
        }
    }
}
