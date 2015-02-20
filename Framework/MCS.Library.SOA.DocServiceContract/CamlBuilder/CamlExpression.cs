using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.CamlBuilder
{
    public abstract class CamlExpression:Caml
    {
        public virtual CamlAnd And(CamlExpression caml)
        {
            return new CamlAnd(this, caml);
        }

        public virtual CamlOr Or(CamlExpression caml)
        {
            return new CamlOr(this, caml);
        }
    }
}
