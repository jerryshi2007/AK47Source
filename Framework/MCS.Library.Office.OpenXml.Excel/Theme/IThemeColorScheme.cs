using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MCS.Library.Office.OpenXml.Excel
{
    public interface IThemeColorScheme : IDisposable
    {
        string Color { get; }
    }
}
