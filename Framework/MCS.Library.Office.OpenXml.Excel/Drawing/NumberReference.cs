using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
    public class NumberReference
    {
        private string _formula = string.Empty;
        public string Formula
        {
            get { return _formula; }
            set { this._formula = value; }
        }

        private string _format = string.Empty;

        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }
        private List<NumericPoint> _numericPointCollection = new List<NumericPoint>();
        public List<NumericPoint> NumericPointCollection { get { return _numericPointCollection; } }
    }
}
