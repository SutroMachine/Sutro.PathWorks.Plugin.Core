using gs;

namespace Sutro.PathWorks.Plugins.Core.Meshers
{
    public class ToolpathPreviewJoint
    {
        public int InRight { get; set; }
        public int InTop { get; set; }
        public int InLeft { get; set; }
        public int InBottom { get; set; }

        public int OutRight { get; set; }
        public int OutTop { get; set; }
        public int OutLeft { get; set; }
        public int OutBottom { get; set; }
    }
}