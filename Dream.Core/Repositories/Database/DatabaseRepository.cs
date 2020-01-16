using System;
using System.Data.Entity;

namespace Dream.Core.Repositories.Database
{
    public abstract class DatabaseRepository
    {
        public abstract DbContext DatabaseContext { get; }

        protected DateTime _CutOffDate;

        public DatabaseRepository() { }

        public DatabaseRepository(DateTime cutOffDate)
        {
            _CutOffDate = cutOffDate;
        }
    }
}
