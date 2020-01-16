using Dream.Common.Enums;
using System;
using System.Collections.Generic;

namespace Dream.Core.BusinessLogic.Containers
{
    public class DbrsStressModelInput
    {
        public DateTime AssumptionsStartDate { get; set; }

        public Dictionary<PropertyState, double> StateLevelDefaultRateDictionary { get; set; }
        public Dictionary<PropertyState, double?> StateLevelLossGivenDefaultDictionary { get; set; }
        public Dictionary<PropertyState, int> StateLevelForeclosureTermInMonthsDictionary { get; set; }
        public Dictionary<PropertyState, int> ReperformanceTermInMonths { get; set; }
        public int TotalNumberOfDefaultSequences { get; set; }

        public int TotalMonthsToNextDefault(PropertyState propertyState)
        {
            return StateLevelForeclosureTermInMonthsDictionary[propertyState] + ReperformanceTermInMonths[propertyState] + 1;
        }

    }
}
