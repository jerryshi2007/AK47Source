using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.OGUPermission;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.PermissionBridge.Adapters;
using System.Diagnostics;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.PermissionBridge
{
	public sealed class BridgedOrganizationMechanism : IOrganizationMechanism, IOguImplInterface
	{
		#region IOrganizationMechanism
		public OguObjectCollection<T> GetObjects<T>(SearchOUIDType idType, params string[] ids) where T : IOguObject
		{
			return idType.ToQueryAdapter<T>(typeof(T).ToSchemaTypes(), ids).Query();
		}

		public IOrganization GetRoot()
		{
			var rootPath = OGUPermission.OguPermissionSettings.GetConfig().RootOUPath;

			return SearchOUIDType.FullPath.ToQueryAdapter<IOrganization>(typeof(IOrganization).ToSchemaTypes(), new string[] { rootPath }).Query().FirstOrDefault();
		}

		public bool AuthenticateUser(LogOnIdentity identity)
		{
			bool result = false;
			var user = this.GetObjects<IUser>(SearchOUIDType.LogOnName, identity.LogOnName).FirstOrDefault();
			if (user != null)
			{
				result = PC.Adapters.UserPasswordAdapter.Instance.CheckPassword(user.ID, PC.Adapters.UserPasswordAdapter.GetPasswordType(), identity.Password);
			}

			return result;

		}

		public void RemoveAllCache()
		{
		}
		#endregion IOrganizationMechanism

		#region IOguImplInterface
		public OguObjectCollection<T> GetChildren<T>(IOrganization parent, bool includeSideline, SearchLevel searchLevel) where T : IOguObject
		{
			List<T> result = new List<T>(20);

			FillOrganizationChildren(parent.ID, includeSideline, searchLevel, result);

			return new OguObjectCollection<T>(result);
		}

		private static void FillOrganizationChildren<T>(string parentID, bool includeSideline, SearchLevel searchLevel, List<T> result) where T : IOguObject
		{
			SCObjectAndRelationCollection relations =
				PC.Adapters.SCSnapshotAdapter.Instance.QueryObjectAndRelationByParentIDs(typeof(T).ToSchemaTypes(), new string[] { parentID }, false, includeSideline, Util.GetContextIncludeDeleted(), DateTime.MinValue);

			relations.Sort((x, y) => x.InnerSort - y.InnerSort);

			List<T> tempResult = relations.ConvertToOguObjects<T>();

			tempResult.ForEach(obj => result.Add(obj));

			if (searchLevel == SearchLevel.SubTree)
			{
				// 先将下级组织查询出来
				SCObjectAndRelationCollection subRelations =
				PC.Adapters.SCSnapshotAdapter.Instance.QueryObjectAndRelationByParentIDs(typeof(IOrganization).ToSchemaTypes(), new string[] { parentID }, false, includeSideline, Util.GetContextIncludeDeleted(), DateTime.MinValue);

				subRelations.Sort((x, y) => x.InnerSort - y.InnerSort);

				List<IOrganization> tempSubResult = subRelations.ConvertToOguObjects<IOrganization>();

				foreach (IOrganization obj in tempSubResult)
				{
					FillOrganizationChildren(obj.ID, includeSideline, searchLevel, result);
				}
			}
		}

		/// <summary>
		/// 在子对象进行查询（所有级别深度）
		/// </summary>
		/// <typeparam name="T">期望的类型</typeparam>
		/// <param name="parent">父机构对象</param>
		/// <param name="matchString">模糊查询的字符串</param>
		/// <param name="includeSideLine">是否包含兼职的人员</param>
		/// <param name="level">查询的深度</param>
		/// <param name="returnCount">返回的记录数</param>
		/// <returns>得到查询的子对象</returns>
		public OguObjectCollection<T> QueryChildren<T>(IOrganization parent, string matchString, bool includeSideLine, SearchLevel level, int returnCount) where T : IOguObject
		{
			//SCObjectAndRelationCollection objRelations = PC.Adapters.SCSnapshotAdapter.Instance.QueryObjectAndRelationByKeyword(parent.ID, matchString, null, returnCount, level == SearchLevel.SubTree, DateTime.MinValue);
			//TODO: 修改正确
			SCObjectAndRelationCollection objRelations = PC.Adapters.SCSnapshotAdapter.Instance.QueryObjectAndRelationByKeywordAndParentIDs(new string[] { "Users", "Groups", "Organizations", "Applications", "Permissions", "Roles", "Conditions" }, new string[] { parent.ID }, matchString, returnCount, level == SearchLevel.SubTree, true, false, DateTime.MinValue);
			return GetObjects<T>(SearchOUIDType.Guid, objRelations.ToIDArray());
		}

		public OguObjectCollection<IUser> GetAllRelativeUserInfo(IUser user)
		{
			var allRelations = PC.Adapters.SCSnapshotAdapter.Instance.QueryObjectAndRelationByIDs(typeof(IUser).ToSchemaTypes(), new string[] { user.ID }, Util.GetContextIncludeDeleted(), DateTime.MinValue);
			allRelations.Sort((x, y) => y.Default.CompareTo(x.Default));

			return new OguObjectCollection<IUser>(allRelations.ConvertToOguObjects<IUser>());
		}

		public OguObjectCollection<IGroup> GetGroupsOfUser(IUser user)
		{
			PC.SCUser objUser = (PC.SCUser)PC.Adapters.SchemaObjectAdapter.Instance.Load(user.ID);

			var groups = objUser.CurrentGroups;

			var groupIds = groups.ToIDArray();

			return this.GetObjects<IGroup>(SearchOUIDType.Guid, groupIds);

		}

		public OguObjectCollection<IUser> GetGroupMembers(IGroup group)
		{
			var gp = ((PC.SCGroup)PC.Adapters.SchemaObjectAdapter.Instance.Load(group.ID));
			var userIds = (from r in gp.CurrentMembersRelations where r.MemberSchemaType == "Users" && r.Status == SchemaObjectStatus.Normal select r.ID).ToArray();


			var users = this.GetObjects<IUser>(SearchOUIDType.Guid, userIds);

			return users;
		}

		public OguObjectCollection<IUser> GetSecretaries(IUser user)
		{
			var relations = PC.Adapters.SCMemberRelationAdapter.Instance.LoadByContainerID(user.ID);

			var ids = (from r in relations where r.MemberSchemaType == "Users" && r.ContainerSchemaType == "Users" && r.Status == SchemaObjectStatus.Normal select r.ID).ToArray();
			if (ids.Length > 0)
			{
				var objects = this.GetObjects<IUser>(SearchOUIDType.Guid, ids);

				return new OguObjectCollection<IUser>((from u in objects select (IUser)u).ToArray());
			}
			else
			{
				return new OguObjectCollection<IUser>();
			}
		}

		public OguObjectCollection<IUser> GetSecretaryOf(IUser user)
		{
			var relations = PC.Adapters.SCMemberRelationAdapter.Instance.LoadByMemberID(user.ID);

			var ids = (from r in relations where r.ContainerSchemaType == "Users" && r.MemberSchemaType == "Users" && r.Status == SchemaObjectStatus.Normal select r.ContainerID).ToArray();
			if (ids.Length > 0)
			{
				var objects = this.GetObjects<IUser>(SearchOUIDType.Guid, ids);

				return new OguObjectCollection<IUser>((from u in objects select (IUser)u).ToArray());
			}
			else
			{
				return new OguObjectCollection<IUser>();
			}
		}

		/// <summary>
		/// 初始化祖先的OUs
		/// </summary>
		/// <param name="currentObj">当前对象</param>
		public void InitAncestorOUs(IOguObject currentObj)
		{
			IOguPropertyAccessible wrapper = (IOguPropertyAccessible)currentObj;

			string parentID = (string)currentObj.Properties["ParentID"];

			Debug.Assert(string.IsNullOrEmpty(parentID) == false);

			Dictionary<string, SCSimpleObjectCollection> parentMap = PC.Adapters.SCSnapshotAdapter.Instance.LoadAllParentsInfo(true, parentID);

			SCSimpleObjectCollection parents = parentMap[parentID];

			List<string> idList = new List<string>(parents.ToIDArray());

			//idList.Add(parentID);

			OguObjectCollection<IOrganization> orgs = this.GetObjects<IOrganization>(SearchOUIDType.Guid, idList.ToArray());

			((IOguPropertyAccessible)currentObj).Parent = orgs.Find(obj => obj.ID == parentID);
			wrapper = (IOguPropertyAccessible)wrapper.Parent;
			for (int i = parents.Count - 1; i >= 0; i--)
			{
				var org = orgs.Find(obj => obj.ID == parents[i].ID);
				if (org != null && org.DepartmentType != DepartmentTypeDefine.XuNiJiGou)
				{
					wrapper.Parent = orgs.Find(obj => obj.ID == parents[i].ID);
					wrapper = (IOguPropertyAccessible)wrapper.Parent;
				}
			}
		}

		#endregion IOguImplInterface
	}
}
