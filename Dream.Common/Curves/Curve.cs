using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Common.Curves
{
    /// <summary>
    /// A modified List object that allows the user protection to pull items that would normally be out of range.
    /// Instead of an IndexOutOfRange error, the list's first and last values are assumed to extend forever.
    /// </summary>
    public class Curve<T> : IEnumerable<T>
    {
        private List<T> _itemsList;

        public Curve()
        {
            _itemsList = new List<T>();
        }
        public Curve(T item)
        {
            _itemsList = new List<T> { item };
        }
        public Curve(List<T> items)
        {
            _itemsList = items.ToList();
        }

        public Curve(IEnumerable<T> items)
        {
            _itemsList = items.ToList();
        }

        public T this[int index]
        {
            get
            {
                // Assume that the last element repeats forever
                if (index >= _itemsList.Count)
                {
                    return _itemsList.Last();
                }

                // Assume the first element came forever before
                if (index < 0 && _itemsList.Count > 0)
                {
                    return _itemsList.First();
                }

                return _itemsList[index];
            }
            set
            {
                // Add the value to the list for all necessary periods
                if (index >= _itemsList.Count)
                {
                    var numberOfPeriods = index - _itemsList.Count + 1;
                    var rangeOfValues = Enumerable.Repeat(value, numberOfPeriods);

                    _itemsList.AddRange(rangeOfValues);
                }
                // It's debatable what to do here, but let's go with an exception for now
                // One could argue that this behavior should prepend the curve with the value though
                else if (index < 0)
                {
                    throw new Exception("ERROR: Cannot add values to a curve prior to when the curve begins.");
                }
                else
                {
                    _itemsList[index] = value;
                }                
            }
        }

        public void Add(T item)
        {
            _itemsList.Add(item);
        }

        public void AddRange(List<T> items)
        {
            _itemsList.AddRange(items);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _itemsList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _itemsList.GetEnumerator();
        }
    }
}
