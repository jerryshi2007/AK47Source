using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Schemas.Actions;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace MCS.Library.SOA.DataObjects.Security.Actions
{
	internal class SCObjectUpdateStatusAction : ISchemaObjectUpdateAction
	{
		public void Prepare(VersionedSchemaObjectBase obj)
		{
		}

		public void Persist(VersionedSchemaObjectBase obj)
		{
			SchemaObjectBase schemaObj = (SchemaObjectBase)obj;
			obj.Schema.SnapshotTable.IsNotEmpty(tableName => SCSnapshotBasicAdapter.Instance.UpdateCurrentSnapshotStatus(schemaObj));
		}
	}
}
