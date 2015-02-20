using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Debugger
{
	[DebuggerDisplay("C:{ChildID}({ChildSchemaType})→P:{ParentID}({ParentSchemaType}),{Status},S:{InnerSort}", Name = "{key}")]
	internal class ListRelations
	{
		private IList list;
		private object key;
		private SCRelationObject value;

		public ListRelations(IList list, object key, SCRelationObject value)
		{
			this.value = value;
			this.key = key;
			this.list = list;
		}

		public SCRelationObject Relation
		{
			get
			{
				return this.value ;
			}
		}

		public string ParentSchemaType
		{
			get
			{
				return this.Relation != null ? this.Relation.ParentSchemaType : string.Empty;
			}
		}

		public string ChildSchemaType
		{
			get
			{
				return this.Relation != null ? this.Relation.ChildSchemaType : string.Empty;
			}
		}

		public string ParentID
		{
			get
			{
				return this.Relation != null ? this.Relation.ParentID : string.Empty;
			}
		}

		public string ChildID
		{
			get
			{
				return this.Relation != null ? this.Relation.ID : string.Empty;
			}
		}

		public SchemaObjectStatus Status
		{
			get
			{
				return this.Relation != null ? this.Relation.Status : SchemaObjectStatus.Unspecified;
			}
		}

		public int InnerSort
		{
			get
			{
				return this.Relation != null ? this.Relation.InnerSort : -1;
			}
		}
	}
}
