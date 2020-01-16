using Dream.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.Converters.Database.Collateral
{
    public class PropertyStateDatabaseConverter
    {
        private Dictionary<int, string> _propertyStatesDictionary;
        private Dictionary<string, int> _propertyStatesDictionaryReversed;

        public PropertyStateDatabaseConverter(Dictionary<int, string> propertyStatesDictionary)
        {
            _propertyStatesDictionary = propertyStatesDictionary;
            _propertyStatesDictionaryReversed = propertyStatesDictionary.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }

        public PropertyState ConvertId(int? propertyStateId)
        {
            // Assume the state is CA, if nothing is provided in the data
            if (!propertyStateId.HasValue) return default(PropertyState);

            if (!_propertyStatesDictionary.ContainsKey(propertyStateId.Value))
            {
                throw new Exception(string.Format("INTERNAL ERROR: Property state not found for ID '{0}'. Please report this error.",
                    propertyStateId));
            }

            var propertyStateDescription = _propertyStatesDictionary[propertyStateId.Value];
            Enum.TryParse(propertyStateDescription, out PropertyState propertyState);

            return propertyState;
        }

        public int ConvertToId(PropertyState propertyState)
        {
            var propertyStateDescription = propertyState.ToString();

            if (!_propertyStatesDictionaryReversed.ContainsKey(propertyStateDescription))
            {
                throw new Exception(string.Format("INTERNAL ERROR: Property state ID not found for property state '{0}'. Please report this error.",
                    propertyStateDescription));
            }

            var propertyStateId = _propertyStatesDictionaryReversed[propertyStateDescription];
            return propertyStateId;
        }
    }
}
