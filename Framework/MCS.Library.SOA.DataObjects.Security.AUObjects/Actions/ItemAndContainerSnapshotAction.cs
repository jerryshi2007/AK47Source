using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Schemas.Actions;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Adapters;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Actions
{
	/// <summary>
	/// 
	/// </summary>
	internal class ItemAndContainerSnapshotAction : ISchemaObjectUpdateAction
	{
		public void Prepare(VersionedSchemaObjectBase obj)
		{
			if (obj is SCMemberRelation)
			{
				SCMemberRelation mr = (SCMemberRelation)obj;

				ObjectSchemaConfigurationElement containerConfig = SchemaDefine.GetSchemaConfig(mr.ContainerSchemaType);
				ObjectSchemaConfigurationElement memberConfig = SchemaDefine.GetSchemaConfig(mr.MemberSchemaType);

				if (containerConfig != null && memberConfig != null)
				{
					if (containerConfig.Name == "AUAdminScopes" && memberConfig.Category == "AUScopeItems")
					{
						SCActionContext.Current.Context["ItemAndContainerSnapshotRelation"] = mr;
					}
				}
			}
		}

		public void Persist(VersionedSchemaObjectBase obj)
		{
			if (SCActionContext.Current.Context.ContainsKey("ItemAndContainerSnapshotRelation"))
			{
				SCMemberRelation mr = (SCMemberRelation)SCActionContext.Current.Context["ItemAndContainerSnapshotRelation"];

				if (mr.Status == SchemaObjectStatus.Normal)
					ItemAndContainerSnapshotAdapter.Instance.Insert(mr);
				else
					ItemAndContainerSnapshotAdapter.Instance.Delete(mr);
			}
		}
	}
}
