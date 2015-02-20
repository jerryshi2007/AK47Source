using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel.Styles
{
    /// <summary>
    /// 
    /// </summary>
    public class CellFormat : ElementInfo
    {
        #region "Attribute"
        //"numFmtId", 
            //"fontId", 
            //"fillId", 
            //"borderId", 
            //"xfId", 
            //"quotePrefix", 
            //"pivotButton", 
            //"applyNumberFormat", 
            //"applyFont", 
            //"applyFill", 
            //"applyBorder", 
            //"applyAlignment", 
        //"applyProtection"
        #endregion

        #region  "ChildElementInfo"
        //ExtensionList (extLst)
        /* //Extension (ext)
           uri
         */

        // Protection (protection)
        //"locked", "hidden"

        //Alignment (alignment)
        //"horizontal", "vertical", "textRotation", "wrapText", "indent", "relativeIndent", "justifyLastLine", 	"shrinkToFit", "readingOrder", 	"mergeCell"
			
        #endregion

        public CellFormat()
        { 
        
        }


        protected internal override string NodeName
        {
            get { return "xf"; }
        }
    }
}
