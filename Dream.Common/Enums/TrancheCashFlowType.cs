namespace Dream.Common.Enums
{
    public enum TrancheCashFlowType
    {
        None = 0,

        Payment,
        PaymentShortfall,
        Principal,
        PrincipalShortfall,

        Interest,
        AccruedInterest,
        InterestShortfall,

        Fees,
        FeesShortfall,
        Reserves,
        ReservesReleased,
    }
}
