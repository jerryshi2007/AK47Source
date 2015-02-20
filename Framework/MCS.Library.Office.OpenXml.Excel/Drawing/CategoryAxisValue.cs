using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
    public class CategoryAxisValue
    {
        private NumberReference _numberReference;

        public NumberReference NumberReference
        {
            get
            {
                if (_numberReference == null)
                    _numberReference = new NumberReference();
                return _numberReference;
            }
            set { _numberReference = value; }
        }
    }
}
