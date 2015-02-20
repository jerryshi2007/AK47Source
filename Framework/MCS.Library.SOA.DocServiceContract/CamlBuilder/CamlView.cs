using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.CamlBuilder
{
    public class CamlView:Caml
    {
        Caml camlInner;

        ViewType viewType;

        public CamlView(Caml camlInner,ViewType viewType)
        {
            this.camlInner = camlInner;
            this.viewType = viewType;
        }

        public override string ToCamlString()
        {
            string result = "<View";
            if (viewType == ViewType.RecursiveAll)
                result += " Scope=\"RecursiveAll\"";
            result += ">";
            result += camlInner.ToCamlString();
            result += "</View>";
            return result;
        }
    }

    public enum ViewType
    {
        Default,
        RecursiveAll
    }
}
