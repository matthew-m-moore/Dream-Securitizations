using Dream.Core.BusinessLogic.Paydown;
using Dream.IO.Excel.Entities;
using System.Collections.Generic;

namespace Dream.Core.Converters.Excel
{
    public class PaydownScenarioExcelConverter
    {
        public static List<PaydownScenario> ConvertListOfPaydownCalculatorInputRecords (List<PaydownCalculatorInputRecord> paydownCalculatorInputRecords)
        {
            var listOfPaydownScenarios = new List<PaydownScenario>();

            foreach(var paydownCalculatorInputRecord in paydownCalculatorInputRecords)
            {
                var paydownScenario = new PaydownScenario
                {
                    ScenarioName = paydownCalculatorInputRecord.ScenarioName,
                    BondCallDate = paydownCalculatorInputRecord.BondCallDate,                    
                                        
                    PercentageFees = paydownCalculatorInputRecord.TaxCollectionFeePercentage,
                    FixedFees = paydownCalculatorInputRecord.OtherFixedFees,
                    AdditionalCustomerFee = paydownCalculatorInputRecord.AdditionalCustomerFee,

                    PaymentAlreadyMadeForPeriod = paydownCalculatorInputRecord.TaxesAlreadyPaidForYear,
                    NextBillAmendedForPayment = paydownCalculatorInputRecord.TaxBillAmendedForPayment,
                    SendRefundIfPossible = paydownCalculatorInputRecord.SendRefundIfPossible,

                    TryAllFutureBondCallDates = paydownCalculatorInputRecord.TryAllFutureBondCallDates,
                };

                // Only invoke solving for the the maximum paydown percentage if no value was provided
                paydownScenario.SolveForMaxPaydownPercentage = !paydownCalculatorInputRecord.PaydownPercentageAmount.HasValue;
                if (!paydownScenario.SolveForMaxPaydownPercentage)
                {
                    paydownScenario.PaydownPercentageAmount = paydownCalculatorInputRecord.PaydownPercentageAmount.Value;
                }

                listOfPaydownScenarios.Add(paydownScenario);
            }

            return listOfPaydownScenarios;
        }
    }
}
