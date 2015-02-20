using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.CamlBuilder
{
    public class CamlQueryContainer:Caml
    {
        Caml innerCaml;

        public CamlQueryContainer(Caml innerCaml)
        {
            this.innerCaml = innerCaml;
        }

        public override string ToCamlString()
        {
            return "<Query>" + innerCaml.ToCamlString() + "</Query>";
        }
    }
}
