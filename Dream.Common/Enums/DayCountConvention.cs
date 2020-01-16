using System.ComponentModel;

namespace Dream.Common.Enums
{
    public enum DayCountConvention
    {
        [Description("N/A")]
        None = 0,

        [Description("30/360")]
        Thirty360,
        [Description("Act/360")]
        Actual360,
        [Description("Act/365")]
        Actual365,
        [Description("Act/Act-ISDA")]
        ActualActual,
    }
}
