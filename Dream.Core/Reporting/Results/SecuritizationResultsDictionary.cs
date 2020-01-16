using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.Reporting.Results
{
    public class SecuritizationResultsDictionary
    {
        private Dictionary<string, SecuritizationResult> _resultsDictionary;

        public List<string> SecuritizationNodeNamesOfDisplayableResults => _resultsDictionary.Values
            .SelectMany(r => r.SecuritizationResultsDictionary.Values
                .Where(t => string.IsNullOrEmpty(t.TrancheName) &&
                           !string.IsNullOrEmpty(t.SecuritizationNodeName) &&
                            Math.Abs(t.PresentValue) > 0.0))
            .OrderBy(n => n.ChildToParentOrder)
            .Select(x => x.SecuritizationNodeName).Distinct().ToList();

        public List<string> SecuritizationTrancheNamesOfDisplayableResults => _resultsDictionary.Values
            .SelectMany(r => r.SecuritizationResultsDictionary.Values
                .Where(t => !string.IsNullOrEmpty(t.TrancheName)))
            .OrderBy(n => n.ChildToParentOrder)
            .Select(x => x.TrancheName).Distinct().ToList();

        public SecuritizationResultsDictionary()
        {
            _resultsDictionary = new Dictionary<string, SecuritizationResult>();
        }

        public SecuritizationResultsDictionary(Dictionary<string, SecuritizationResult> resultsDictionary)
        {
            _resultsDictionary = resultsDictionary;
        }

        public SecuritizationResult this[string securitizationNodeOrTrancheName]
        {
            get { return _resultsDictionary[securitizationNodeOrTrancheName]; }
            set { _resultsDictionary[securitizationNodeOrTrancheName] = value; }
        }

        public bool ContainsKey(string securitizationNodeOrTrancheName)
        {
            return _resultsDictionary.ContainsKey(securitizationNodeOrTrancheName);
        }

        public bool Any()
        {
            return _resultsDictionary.Any();
        }

        public IEnumerator<KeyValuePair<string, SecuritizationResult>> GetEnumerator()
        {
            return _resultsDictionary.GetEnumerator();
        }

        public void Add(string scenarioDescription, SecuritizationResult securitizationResult)
        {
            _resultsDictionary.Add(scenarioDescription, securitizationResult);
        }
    }
}
