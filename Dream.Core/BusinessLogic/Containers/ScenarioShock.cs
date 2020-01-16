using Dream.Common.Enums;

namespace Dream.Core.BusinessLogic.Containers
{
    public class ScenarioShock
    {
        public ShockStrategy ShockStrategy { get; set; }
        public double ShockValue { get; set; }

        public ScenarioShock(ShockStrategy shockStrategy, double shockValue)
        {
            ShockStrategy = shockStrategy;
            ShockValue = shockValue;
        }
    }
}
