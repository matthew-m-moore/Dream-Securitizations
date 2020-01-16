using System;

namespace Dream.Core.BusinessLogic.Containers
{
    public class CustomPayment
    {
        public DateTime PaymentDate { get; set; }
        public double PrincipalAmount { get; set; }
        public double InterestAmount { get; set; }
        public bool IsPrincipalAmountTotal { get; set; }
        public bool IsInterestAmountTotal { get; set; }

        public CustomPayment(DateTime paymentDate, double principalAmount, double interestAmount, 
            bool isPrincipalAmountTotal, 
            bool isInterestAmountTotal)
        {
            PaymentDate = paymentDate;

            PrincipalAmount = principalAmount;
            InterestAmount = interestAmount;

            IsPrincipalAmountTotal = isPrincipalAmountTotal;
            IsInterestAmountTotal = isInterestAmountTotal;
        }

        public CustomPayment(CustomPayment customPayment)
        {
            PaymentDate = new DateTime(customPayment.PaymentDate.Ticks);

            PrincipalAmount = customPayment.PrincipalAmount;
            InterestAmount = customPayment.InterestAmount;

            IsPrincipalAmountTotal = customPayment.IsPrincipalAmountTotal;
            IsInterestAmountTotal = customPayment.IsInterestAmountTotal;
        }
    }
}
