using System.IO;
using System.Text;

namespace Sutro.PathWorks.Plugins.Core.Engines
{
    public class SingleLineWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.ASCII;

        public string CurrentLine { get; private set; }

        public override void WriteLine(string value)
        {
            CurrentLine = value;
        }
    }
}