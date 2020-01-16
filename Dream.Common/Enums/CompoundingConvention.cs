using System.ComponentModel;

namespace Dream.Common.Enums
{
    public enum CompoundingConvention
    {
        [Description("N/A")]
        None = 0,

        [Description("Simple")]
        Simply = -1,

        [Description("Annual")]
        Annually = 1,
        [Description("Semi-Annual")]
        SemiAnnually = 2,
        [Description("Quarterly")]
        Quarterly = 4,
        [Description("Monthly")]
        Monthly = 12,

        [Description("Daily")]
        Daily,
        [Description("Continuous")]
        Continuously,
    }
}
