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
	internal class SCObjectSnapshotAction : ISchemaObjectUpdateAction
	{
		public void Prepare(VersionedSchemaObjectBase obj)
		{
		}

		public void Persist(VersionedSchemaObjectBase obj)
		{
			SchemaObjectBase schemaObj = (SchemaObjectBase)obj;
			if (string.IsNullOrEmpty(obj.Schema.SnapshotTable) == false)
				SCSnapshotBasicAdapter.Instance.UpdateCurrentSnapshot(schemaObj, obj.Schema.SnapshotTable, SnapshotModeDefinition.IsInSnapshot);

			if (obj.Schema.ToSchemaObjectSnapshot)
				SCSnapshotBasicAdapter.Instance.UpdateCurrentSnapshot(schemaObj, "SC.SchemaObjectSnapshot", SnapshotModeDefinition.IsInCommonSnapshot);
		}
	}
}
