using Dream.Core.BusinessLogic.Aggregation;
using Dream.IO.Database.Entities.Collateral;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.Converters.Database.Collateral
{
    public class AggregationGroupingsDatabaseConverter
    {
        /// <summary>
        /// Converts a collection of Excel rows into a ProjectedPerformanceAssumptions business object.
        /// </summary>
        public static AggregationGroupings ConvertToAggregationGroupings(List<AggregationGroupAssignmentEntity> aggregationGroupAssignmentEntities)
        {
            var aggregationGroupings = new AggregationGroupings();
            if (!aggregationGroupAssignmentEntities.Any() || aggregationGroupAssignmentEntities == null) return aggregationGroupings;

            // Again, note that column indexing in ClosedXML starts at unity, not zero
            foreach (var aggregationGroupAssignmentEntity in aggregationGroupAssignmentEntities)
            {
                var instrumentIdentifier = aggregationGroupAssignmentEntity.InstrumentIdentifier;
                var aggregationGroupingIdentifier = aggregationGroupAssignmentEntity.AggregationGroupingIdentifier;
                var aggregationGroupName = aggregationGroupAssignmentEntity.AggregationGroupName;

                aggregationGroupings[instrumentIdentifier, aggregationGroupingIdentifier] = aggregationGroupName;
            }

            return aggregationGroupings;
        }
    }
}
