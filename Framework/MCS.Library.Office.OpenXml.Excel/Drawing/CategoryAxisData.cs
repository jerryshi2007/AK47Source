using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
    public class CategoryAxisData
    {
        private StringReference _stringReference;

        public StringReference StringReference
        {
            get
            {
                if (_stringReference == null)
                    _stringReference = new StringReference();
                return _stringReference;
            }
            set { _stringReference = value; }
        }
    }
}
