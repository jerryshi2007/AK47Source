using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Client;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Library.SOA.DataObjects.Security.Conditions;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Schemas.Client;
using System.Collections;

namespace PermissionCenter.Clients
{
	/// <summary>
	/// 权限中心对象转换到客户端的对象
	/// </summary>
	public static class SCObjectToClientHelper
	{
		private static Dictionary<string, Type> _SchemaToType = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
		{
			{"Users", typeof(ClientSCUser)},
			{"Organizations", typeof(ClientSCOrganization)},
			{"Groups", typeof(ClientSCGroup)},
			{"Applications", typeof(ClientSCApplication)},
			{"Roles", typeof(ClientSCRole)},
			{"Permissions", typeof(ClientSCPermission)}
		};

		public static ClientSCBase CreateClientBaseObject(string schemaType)
		{
			schemaType.CheckStringIsNullOrEmpty("schemaType");

			ClientSCBase result = null;

			Type targetType = null;

			if (SCObjectToClientHelper._SchemaToType.TryGetValue(schemaType, out targetType))
				result = (ClientSCBase)TypeCreator.CreateInstance(targetType, schemaType);
			else
				result = new ClientSCBase(schemaType);

			return result;
		}

		public static SchemaObjectBase ToSchemaObject(this ClientSchemaObjectBase clientObj)
		{
			return ToSchemaObject(clientObj, true);
		}

		public static SchemaObjectBase ToSchemaObject(this ClientSchemaObjectBase clientObj, bool validateID)
		{
			clientObj.NullCheck("clientObj");
			clientObj.SchemaType.CheckStringIsNullOrEmpty("SchemaType");

			var config = SchemaDefine.GetSchemaConfig(clientObj.SchemaType);
			SchemaObjectBase scObj = (SchemaObjectBase)config.CreateInstance();

			scObj.ID = clientObj.ID;

			if (validateID && clientObj.ID.IsNullOrEmpty())
				throw new InvalidOperationException("客户端对象ID不存在");

			scObj.Status = (SchemaObjectStatus)clientObj.Status;
			scObj.CreateDate = clientObj.CreateDate;
			scObj.VersionStartTime = clientObj.VersionStartTime;
			scObj.VersionEndTime = clientObj.VersionEndTime;

			scObj.Properties.CopyFrom(clientObj.Properties);

			if (clientObj.Creator != null)
				scObj.Creator = (IUser)OguBase.CreateWrapperObject(clientObj.Creator.ID, SchemaType.Users);

			return scObj;
		}

		public static ClientSCBase ToClientSCBaseObject(this SCBase pcObject)
		{
			pcObject.NullCheck("pcObject");

			ClientSCBase result = CreateClientBaseObject(pcObject.SchemaType);

			result.ID = pcObject.ID;
			result.Status = (ClientSchemaObjectStatus)pcObject.Status;
			result.VersionStartTime = pcObject.VersionStartTime;
			result.VersionEndTime = pcObject.VersionEndTime;

			pcObject.Properties.CopyTo(result.Properties);

			if (pcObject.Creator != null)
				result.Creator = new ClientOguUser(pcObject.Creator);

			return result;
		}

		public static ClientSchemaObjectBase ToClientSchemaObject(this SchemaObjectBase pcObject)
		{
			pcObject.NullCheck("pcObject");

			if (pcObject is SCBase)
				return ToClientSCBaseObject((SCBase)pcObject);
			else
			{
				if (pcObject is SCRelationObject)
					return ((SCRelationObject)pcObject).ToClientSchemaObject();
				else if (pcObject is SCMemberRelation)
					return ((SCMemberRelation)pcObject).ToClientSchemaObject();
				else if (pcObject is SCSecretaryRelation)
					return ((SCSecretaryRelation)pcObject).ToClientSchemaObject();
				else
					throw new NotSupportedException("不支持关系等对象");
			}
		}

		public static TO[] ToClientSchemaObjectBaseObjectArray<T, TO>(this IEnumerable<T> pcObjects)
			where T : SchemaObjectBase
			where TO : ClientSchemaObjectBase
		{
			pcObjects.NullCheck("pcObjects");

			int count = (pcObjects is ICollection) ? ((ICollection)pcObjects).Count : pcObjects.Count();

			TO[] result = new TO[count];

			int i = 0;

			foreach (SchemaObjectBase pcObj in pcObjects)
			{
				result[i++] = (TO)pcObj.ToClientSchemaObjectBaseObject();
			}

			return result;
		}

		public static ClientSchemaObjectBase ToClientSchemaObjectBaseObject(this SchemaObjectBase pcObject)
		{
			pcObject.NullCheck("pcObject");
			ClientSchemaObjectBase result;

			if (pcObject is SCRelationObject)
			{
				result = new ClientSchemaRelation();
			}
			else if (pcObject is SCMemberRelation)
			{
				result = new ClientSchemaMember();
				result.SchemaType = pcObject.SchemaType;
				FillPropertiesFromPC(((ClientSchemaMember)result), (SCMemberRelation)pcObject);
			}
			else
			{
				result = CreateClientBaseObject(pcObject.SchemaType);
			}


			result.ID = pcObject.ID;
			result.Status = (ClientSchemaObjectStatus)pcObject.Status;
			result.VersionStartTime = pcObject.VersionStartTime;
			result.VersionEndTime = pcObject.VersionEndTime;

			pcObject.Properties.CopyTo(result.Properties);

			if (pcObject.Creator != null && string.IsNullOrEmpty(pcObject.Creator.ID) == false)
				result.Creator = new ClientOguUser(pcObject.Creator);

			return result;
		}

		private static void FillPropertiesFromPC(ClientSchemaMember clientObj, SCMemberRelation pcObj)
		{
			clientObj.ContainerID = pcObj.ContainerID;
			clientObj.ContainerSchemaType = pcObj.ContainerSchemaType;
			clientObj.MemberSchemaType = pcObj.MemberSchemaType;
			clientObj.ID = pcObj.ID;
			clientObj.SchemaType = pcObj.SchemaType;
		}

		private static void FillCommon(SchemaObjectBase pcObj, ClientSchemaObjectBase clientObj)
		{
			pcObj.NullCheck("pcObj"); clientObj.NullCheck("clientObj");

			clientObj.ID = pcObj.ID;
			clientObj.CreateDate = pcObj.CreateDate;
			clientObj.SchemaType = pcObj.SchemaType;
			clientObj.Status = (ClientSchemaObjectStatus)pcObj.Status;
			clientObj.VersionEndTime = pcObj.VersionEndTime;
			clientObj.VersionStartTime = pcObj.VersionStartTime;

			if (pcObj.Creator != null)
			{
				clientObj.Creator = new ClientOguUser() { DisplayName = string.IsNullOrEmpty(pcObj.Creator.DisplayName) ? pcObj.Creator.Name : pcObj.Creator.DisplayName, ID = pcObj.Creator.ID };
			}

			pcObj.Properties.CopyTo(clientObj.Properties);
		}

		public static ClientSchemaRelation ToClientSchemaObject(this SCRelationObject pcRelation)
		{
			ClientSchemaRelation result = new ClientSchemaRelation();

			FillCommon(pcRelation, result);

			result.ParentID = pcRelation.ParentID;
			result.ChildID = pcRelation.ID;
			result.ChildSchemaType = pcRelation.ChildSchemaType;
			result.ParentSchemaType = pcRelation.ParentSchemaType;
			result.Default = pcRelation.Default;
			result.FullPath = pcRelation.FullPath;
			result.InnerSort = pcRelation.InnerSort;
			result.GolbalSort = pcRelation.GlobalSort;

			return result;
		}

		public static ClientSchemaMember ToClientSchemaObject(this SCSimpleRelationBase pcRelation)
		{
			ClientSchemaMember result = new ClientSchemaMember();

			FillCommon(pcRelation, result);

			result.SchemaType = "Secretary";

			result.MemberSchemaType = pcRelation.MemberSchemaType;
			result.ContainerSchemaType = pcRelation.ContainerSchemaType;
			result.InnerSort = pcRelation.InnerSort;
			result.ContainerID = pcRelation.ContainerID;
			result.ID = pcRelation.ID;

			return result;
		}

		public static ClientSchemaMember ToClientSchemaObject(this SCMemberRelation pcMember)
		{
			pcMember.NullCheck("pcRelation");

			var result = new ClientSchemaMember();

			result.ContainerID = pcMember.ContainerID;
			result.ID = pcMember.ID;
			result.ContainerSchemaType = pcMember.ContainerSchemaType;
			result.MemberSchemaType = pcMember.MemberSchemaType;
			result.InnerSort = pcMember.InnerSort;

			FillCommon(pcMember, result);

			return result;
		}

		public static ClientAclItem ToClientObject(this SCAclItem pcAcl)
		{
			pcAcl.NullCheck("pcAcl");

			ClientAclItem result = new ClientAclItem()
			{
				ContainerID = pcAcl.ContainerID,
				ContainerPermission = pcAcl.ContainerPermission,
				ContainerSchemaType = pcAcl.ContainerSchemaType,
				MemberID = pcAcl.MemberID,
				MemberSchemaType = pcAcl.MemberSchemaType,
				SortID = pcAcl.SortID,
				VersionEndTime = pcAcl.VersionStartTime,
				VersionStartTime = pcAcl.VersionEndTime,
				Status = (ClientSchemaObjectStatus)pcAcl.Status
			};

			return result;
		}

		public static ClientSchemaObjectBase[] ToClientSchemaObjectBaseObjectArray(this SchemaObjectCollection pcObjects)
		{
			pcObjects.NullCheck("pcObjects");

			ClientSchemaObjectBase[] result = new ClientSchemaObjectBase[pcObjects.Count];

			int i = 0;

			foreach (SchemaObjectBase pcObj in pcObjects)
			{
				result[i++] = pcObj.ToClientSchemaObjectBaseObject();
			}

			return result;
		}

		public static SCAclItem ToSCAcl(this ClientAclItem clientAcl)
		{
			clientAcl.NullCheck("pcAcl");

			SCAclItem result = new SCAclItem()
			{
				ContainerID = clientAcl.ContainerID,
				ContainerPermission = clientAcl.ContainerPermission,
				ContainerSchemaType = clientAcl.ContainerSchemaType,
				MemberID = clientAcl.MemberID,
				MemberSchemaType = clientAcl.MemberSchemaType,
				SortID = clientAcl.SortID,
				VersionEndTime = clientAcl.VersionStartTime,
				VersionStartTime = clientAcl.VersionEndTime,
				Status = (SchemaObjectStatus)clientAcl.Status
			};

			return result;
		}

		public static ClientConditionItem ToClientObject(this SCCondition pcCondition)
		{
			pcCondition.NullCheck("pcCondition");

			ClientConditionItem result = new ClientConditionItem()
			{
				Condition = pcCondition.Condition,
				Description = pcCondition.Description,
				OwnerID = pcCondition.OwnerID,
				SortID = pcCondition.SortID,
				Type = pcCondition.Type,
				Status = (ClientSchemaObjectStatus)pcCondition.Status,
				VersionEndTime = pcCondition.VersionEndTime,
				VersionStartTime = pcCondition.VersionStartTime
			};

			return result;
		}

		public static ClientSCBaseKeyedCollection ToClientObjects(this SchemaObjectCollection pcObjects)
		{
			pcObjects.NullCheck("pcObjects");

			ClientSCBaseKeyedCollection result = new ClientSCBaseKeyedCollection();

			foreach (SchemaObjectBase pcObj in pcObjects)
				result.Add(((SCBase)pcObj).ToClientSCBaseObject());

			return result;
		}

		public static ClientSCBase[] ToClientSCBaseObjectArray(this SchemaObjectCollection pcObjects)
		{
			pcObjects.NullCheck("pcObjects");

			ClientSCBase[] result = new ClientSCBase[pcObjects.Count];

			int i = 0;

			foreach (SCBase pcObj in pcObjects)
			{
				result[i++] = pcObj.ToClientSCBaseObject();
			}

			return result;
		}

		public static ClientConditionItem[] ToClientObjectsArray(this SCConditionCollection pcObjects)
		{
			pcObjects.NullCheck("pcObjects");

			ClientConditionItem[] result = new ClientConditionItem[pcObjects.Count];

			int i = 0;

			foreach (SCCondition pcObj in pcObjects)
			{
				result[i++] = pcObj.ToClientObject();
			}

			return result;
		}

		public static SchemaObjectCollection ToSchemaObjectCollection(this ClientSchemaObjectCollection clientObjects)
		{
			clientObjects.NullCheck("clientObjects");

			SchemaObjectCollection collection = new SchemaObjectCollection(clientObjects.Count);
			foreach (var item in clientObjects)
			{
				collection.Add(item.ToSchemaObject());
			}

			return collection;


		}
	}
}