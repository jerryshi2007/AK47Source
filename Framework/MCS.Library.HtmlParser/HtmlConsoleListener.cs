using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.HtmlParser
{
    internal class HtmlConsoleListener : TraceListener
    {
        #region Public Methods

        public override void Write(string Message)
        {
            Write(Message, "");
        }

        public override void Write(string Message, string Category)
        {
            Console.Write("T:" + Category + ": " + Message);
        }

        public override void WriteLine(string Message)
        {
            Write(Message + "\n");
        }

        public override void WriteLine(string Message, string Category)
        {
            Write(Message + "\n", Category);
        }

        #endregion
    }
}
