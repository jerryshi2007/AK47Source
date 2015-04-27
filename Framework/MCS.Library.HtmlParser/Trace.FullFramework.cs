using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.HtmlParser
{
    partial class Trace
    {
        partial void WriteLineIntern(string message, string category)
        {
            System.Diagnostics.Debug.WriteLine(message, category);
        }
    }
}
