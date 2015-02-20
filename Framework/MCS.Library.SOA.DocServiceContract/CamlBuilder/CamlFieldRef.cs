using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.CamlBuilder
{
    public class CamlFieldRef : Caml
    {
        public virtual CamlEqual Eq(CamlValue caml)
        {
            return new CamlEqual(this, caml);
        }

        public CamlFieldRef(string name)
        {
            this.name = name;
        }

        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }



        public override string ToCamlString()
        {
            return "<FieldRef Name='" + name + "' />";
        }
    }
}
