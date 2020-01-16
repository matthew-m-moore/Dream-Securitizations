using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dream.Common.Utilities;

namespace Dream.Common.Tests.Utilities
{
    [TestClass]
    public class NumericalSearchUtilityTests
    {
        [TestMethod, Owner("Matthew Moore")]
        public void NewtonRaphsonWithBisection_TargetValueIsZero_ResultsAsExpected()
        {
            var targetValueForSearch = 0.0;
            var targetPrecision = 1e-14;

            var internalRateOfReturn = NumericalSearchUtility.NewtonRaphsonWithBisection(
                TestPresentValueOne,
                targetValueForSearch,
                targetPrecision);

            Assert.AreEqual(0.0820826354830335, internalRateOfReturn, targetPrecision);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void NewtonRaphsonWithBisection_TargetValueIsNonZero_ResultsAsExpected()
        {
            var targetValueForSearch = 50.0;
            var targetPrecision = 1e-14;

            var internalRateOfReturn = NumericalSearchUtility.NewtonRaphsonWithBisection(
                TestPresentValueTwo,
                targetValueForSearch,
                targetPrecision);

            Assert.AreEqual(0.0820826354830335, internalRateOfReturn, targetPrecision);
        }

        [TestMethod, Owner("Matthew Moore")]
        [ExpectedException(typeof(Exception))]
        public void NewtonRaphsonWithBisection_TargetPrecisionIsTooStringent_ThrowsException()
        {
            var targetValueForSearch = 0.0;
            var targetPrecision = 1e-20;

            var internalRateOfReturn = NumericalSearchUtility.NewtonRaphsonWithBisection(
                new Func<double, double>(x => x),
                targetValueForSearch,
                targetPrecision);
        }

        private double TestPresentValueOne(double yield)
        {
            var initialCashFlow = -50.0;
            var cashFlowOne = 10.0;
            var cashFlowTwo = 20.0;
            var cashFlowThree = 30.0;

            var presentValue = initialCashFlow;

            presentValue += cashFlowOne / Math.Pow(1 + yield, 1);
            presentValue += cashFlowTwo / Math.Pow(1 + yield, 2);
            presentValue += cashFlowThree / Math.Pow(1 + yield, 3);

            return presentValue;
        }

        private double TestPresentValueTwo(double yield)
        {
            var cashFlowOne = 10.0;
            var cashFlowTwo = 20.0;
            var cashFlowThree = 30.0;

            var presentValue = 0.0;

            presentValue += cashFlowOne / Math.Pow(1 + yield, 1);
            presentValue += cashFlowTwo / Math.Pow(1 + yield, 2);
            presentValue += cashFlowThree / Math.Pow(1 + yield, 3);

            return presentValue;
        }
    }
}
