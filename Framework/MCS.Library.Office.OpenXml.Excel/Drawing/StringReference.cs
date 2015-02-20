using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
    public class StringReference
    {
        private string _formula = string.Empty;
        public string Formula
        {
            get { return _formula; }
            set { this._formula = value; }
        }

        private List<StringPoint> _stringPointCollection = new List<StringPoint>();
        public List<StringPoint> StringPointCollection { get { return _stringPointCollection; } }
    }
}
