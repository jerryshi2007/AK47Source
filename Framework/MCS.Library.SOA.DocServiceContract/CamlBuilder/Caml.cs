using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.CamlBuilder
{
	public abstract class Caml
	{

		public static CamlFieldRef Field(string name)
		{
			return new CamlFieldRef(name);
		}

		public static CamlValue Value(string type, string value)
		{
			return new CamlValue(type, value);
		}

		public static CamlWhere Where(Caml caml)
		{
			return new CamlWhere(caml);
		}

		public static CamlQueryContainer Query(Caml caml)
		{
			return new CamlQueryContainer(caml);
		}

		public static CamlQueryContainer SimpleQuery(Caml caml)
		{
			return Caml.Query(Caml.Where(caml));
		}

		public static CamlView SimpleView(Caml caml, ViewType viewType)
		{
			return new CamlView(SimpleQuery(caml), viewType);
		}

		public static CamlView View(Caml caml, ViewType viewType)
		{
			return new CamlView(caml, viewType);
		}

		public static CamlEmpty EmptyCaml()
		{
			return new CamlEmpty();
		}


		public abstract string ToCamlString();
	}
}
