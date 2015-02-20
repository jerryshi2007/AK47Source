using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
    public class NumericPoint
    {
        public string PointIndex
        {
            get { return this._pointIndex; }
            set { this._pointIndex = value; }
        }
        public string PointValue
        {
            get { return this._pointValue; }
            set { this._pointValue = value; }
        }

        private string _pointIndex = string.Empty;
        private string _pointValue = string.Empty;
    }
}
