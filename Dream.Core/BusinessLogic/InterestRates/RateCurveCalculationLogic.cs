using Dream.Common.Curves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dream.Core.BusinessLogic.InterestRates
{
    public abstract class RateCurveCalculationLogic
    {
        public abstract InterestRateCurve CalculateRateCurve();
        public abstract RateCurveCalculationLogic Copy();
    }
}
