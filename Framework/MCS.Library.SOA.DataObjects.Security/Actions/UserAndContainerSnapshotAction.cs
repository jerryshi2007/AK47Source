using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Schemas.Actions;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Configuration;

namespace MCS.Library.SOA.DataObjects.Security.Actions
{
	internal class UserAndContainerSnapshotAction : ISchemaObjectUpdateAction
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
					if (containerConfig.IsUsersContainer && memberConfig.IsUsersContainerMember)
					{
						SCActionContext.Current.Context["UserAndContainerSnapshotRelation"] = mr;
					}
				}
			}
		}

		public void Persist(VersionedSchemaObjectBase obj)
		{
			if (SCActionContext.Current.Context.ContainsKey("UserAndContainerSnapshotRelation"))
			{
				SCMemberRelation mr = (SCMemberRelation)SCActionContext.Current.Context["UserAndContainerSnapshotRelation"];

				if (mr.Status == SchemaObjectStatus.Normal)
					UserAndContainerSnapshotAdapter.Instance.Insert(mr);
				else
					UserAndContainerSnapshotAdapter.Instance.Delete(mr);
			}
		}
	}
}
