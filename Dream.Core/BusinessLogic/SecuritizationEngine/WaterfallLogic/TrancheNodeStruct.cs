using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic
{
    public struct TrancheNodeStruct
    {
        public Tranche Tranche { get; set; }
        public SecuritizationNodeTree SecuritizationNode { get; set; }
    }
}
