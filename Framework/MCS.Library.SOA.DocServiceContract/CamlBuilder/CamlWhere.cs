using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.CamlBuilder
{
    public class CamlWhere:Caml
    {
        Caml camlInner;

        public Caml CamlInner
        {
            get { return camlInner; }
            set { camlInner = value; }
        }

        public CamlWhere(Caml camlInner)
        {
            this.camlInner = camlInner;
        }

        public override string ToCamlString()
        {
            return "<Where>" + camlInner.ToCamlString() + "</Where>";
        }
    }
}
