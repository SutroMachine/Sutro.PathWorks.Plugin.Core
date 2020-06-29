using gs;

namespace Sutro.PathWorks.Plugins.Core.Meshers
{
    public partial class TubeMesher<TPrintVertex> where TPrintVertex : IExtrusionVertex
    {
        protected class ToolpathPreviewJoint
        {
            public int InRight { get; set; }
            public int InTop { get; set; }
            public int InLeft { get; set; }
            public int InBottom { get; set; }

            public int OutRight { get; set; }
            public int OutTop { get; set; }
            public int OutLeft { get; set; }
            public int OutBottom { get; set; }

            public JointStyle Style { get; set; } = JointStyle.Capped;
        }

        protected enum JointStyle { Connected, Capped };
    }
}