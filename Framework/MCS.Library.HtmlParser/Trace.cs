using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.HtmlParser
{
    internal partial class Trace
    {
        internal static Trace _current;
        internal static Trace Current
        {
            get
            {
                if (_current == null)
                    _current = new Trace();
                return _current;
            }
        }
        partial void WriteLineIntern(string message, string category);
        public static void WriteLine(string message, string category)
        {
            Current.WriteLineIntern(message, category);
        }
    }
}
