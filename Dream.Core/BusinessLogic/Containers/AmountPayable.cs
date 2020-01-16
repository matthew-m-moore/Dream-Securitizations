namespace Dream.Core.BusinessLogic.Containers
{
    public class AmountPayable
    {
        public double Amount { get; set; }
        public double Shortfall { get; set; }

        public AmountPayable(double amountPayable)
        {
            Amount = amountPayable;
            Shortfall = 0.0;
        }

        public AmountPayable(double amountPayable, double shortfall)
        {
            Amount = amountPayable;
            Shortfall = shortfall;
        }
    }
}
