using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
    public class StyleDifferentialFormat
    {
        public StyleDifferentialFormat()
        { }

        private int _DictKey;
        public int DictKey
        {
            get
            {
                return this._DictKey;
            }
            set
            {
                this._DictKey = value;
            }
        }

        private string _InnerXml;
        public string InnerXml
        {
            get
            {
                return this._InnerXml;
            }
            set
            {
                this._InnerXml = value;
            }
        }
    }
}
