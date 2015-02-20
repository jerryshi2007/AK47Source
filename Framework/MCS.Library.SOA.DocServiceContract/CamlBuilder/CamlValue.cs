using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.CamlBuilder
{
    public class CamlValue : Caml
    {
        public CamlValue(string type, string value)
        {
            this.camlType = type;
            this.value = value;
        
        }

        string camlType;

        public string CamlType
        {
            get { return camlType; }
            set { camlType = value; }
        }

        string value;



        public new string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public override string ToCamlString()
        {
            return "<Value Type='" + camlType + "'>" + value + "</Value>";
        }
    }
}
