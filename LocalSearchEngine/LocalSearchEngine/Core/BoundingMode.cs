using System.ComponentModel;

namespace LocalSearchEngine.Core
{
    public enum BoundingMode
    {
        [Description("_")]
        Open,

        [Description("[")]
        ClosedBegin,


        [Description("]")]
        ClosedEnd
    }
}
