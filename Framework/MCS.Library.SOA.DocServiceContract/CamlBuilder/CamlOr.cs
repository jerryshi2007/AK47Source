using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.CamlBuilder
{
    public class CamlOr:CamlExpression
    {
        public CamlOr(Caml left, Caml right)
        {
            this.left = left;
            this.right = right;
        }

        Caml left;

        public Caml Left
        {
            get { return left; }
            set { left = value; }
        }

        Caml right;

        public Caml Right
        {
            get { return right; }
            set { right = value; }
        }

        public override string ToCamlString()
        {
            return "<Or>" + left.ToCamlString() + right.ToCamlString() + "</Or>";
        }
    }
}
