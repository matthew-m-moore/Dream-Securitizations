using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dream.IO.Database.Entities.Securitization
{
    public class BalanceCapAndFloorDetailEntity
    {
        public int BalanceCapAndFloorDetailId { get; set; }
        public int BalanceCapAndFloorSetId { get; set; }
        public string BalanceCapOrFloor { get; set; }
        public string PercentageOrDollarAmount { get; set; }
        public double CapOrFloorValue { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
