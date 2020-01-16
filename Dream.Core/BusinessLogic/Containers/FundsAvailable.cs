namespace Dream.Core.BusinessLogic.Containers
{
    public class FundsAvailable
    {
        public double SpecificFundsAvailable { get; set; }
        public double TotalFundsAvailable { get; set; }

        public FundsAvailable(double specificFundsAvailable, double totalFundsAvailable)
        {
            SpecificFundsAvailable = specificFundsAvailable;
            TotalFundsAvailable = totalFundsAvailable;
        }
    }
}
