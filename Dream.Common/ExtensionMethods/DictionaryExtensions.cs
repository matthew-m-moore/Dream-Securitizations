using System.Collections.Generic;

namespace Dream.Common.ExtensionMethods
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Combines two dictionaries, similar to "AddRange" for a List<T>. Throws an exception if the keys clash.
        /// </summary>
        public static void Combine<T1, T2>(this Dictionary<T1, T2> existingDictionary, Dictionary<T1, T2> dictionaryToAdd)
        {
            foreach(var keyValuePair in dictionaryToAdd)
            {
                existingDictionary.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        /// <summary>
        /// Adds a key-value pair to a dictionary, similar to "Add" for a List<T>. Throws an exception if the keys clash.
        /// </summary>
        public static void Add<T1, T2>(this Dictionary<T1, T2> existingDictionary, KeyValuePair<T1, T2> keyValuePairToAdd)
        {
            existingDictionary.Add(keyValuePairToAdd.Key, keyValuePairToAdd.Value);
        }
    }
}
