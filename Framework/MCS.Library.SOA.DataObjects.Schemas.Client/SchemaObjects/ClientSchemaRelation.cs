using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Schemas.Client
{
	public class ClientSchemaRelation : ClientSchemaObjectBase
	{
		public string ChildID { get; set; }

		public string ChildSchemaType { get; set; }

		public string ParentSchemaType { get; set; }

		public string ParentID { get; set; }

		public bool Default { get; set; }

		public string FullPath { get; set; }

		public int InnerSort { get; set; }

		public string GolbalSort { get; set; }

		public ClientSchemaRelation()
		{
		}
	}
}
