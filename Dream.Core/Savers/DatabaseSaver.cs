using System;
using System.Data.Entity;

namespace Dream.Core.Savers
{
    public abstract class DatabaseSaver
    {
        public abstract DbContext DatabaseContext { get; }

        protected DateTime _CutOffDate { get; set; }
        protected string _Description { get; set; }

        public DatabaseSaver() { }

        public DatabaseSaver(DateTime cutOffDate)
        {
            _CutOffDate = cutOffDate;
        }

        public DatabaseSaver(DateTime cutOffDate, string description)
        {
            _CutOffDate = cutOffDate;
            _Description = description;
        }
    }
}
