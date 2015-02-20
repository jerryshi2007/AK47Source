using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MCS.Library.CamlBuilder
{
    public class CamlEmpty:Caml
    {

        public override string ToCamlString()
        {
            return "";
        }
    }
}