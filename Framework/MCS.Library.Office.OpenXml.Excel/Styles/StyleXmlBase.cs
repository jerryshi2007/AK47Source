using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MCS.Library.Office.OpenXml.Excel
{
    //: SpreadSheetXml
    public abstract class StyleXmlBaseWrapper 
    {
        internal abstract string Id
        {
            get;
        }

        internal long useCnt = 0;
        internal int newID = int.MinValue;
    }
}
