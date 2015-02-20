#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.OGUPermission
// FileName	：	OguAdminMechanism.cs
// Remark	：	机构人员
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    沈峥	    20070430		创建
// -------------------------------------------------
#endregion
using MCS.Library.Core;
using MCS.Library.OGUPermission.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;

namespace MCS.Library.OGUPermission
{
	internal sealed class OguAdminMechanism : IOrganizationMechanism, IOguImplInterface
	{
		public static readonly OguAdminMechanism Instance = new OguAdminMechanism();

		private OguAdminMechanism()
		{
		}

		#region IOguOperations 成员

		public OguObjectCollection<T> GetObjects<T>(SearchOUIDType idType, params string[] ids) where T : IOguObject
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(ids != null, "ids");

			SchemaType objType = OguObjectHelper.GetSchemaTypeFromInterface<T>();

			ExceptionHelper.TrueThrow(idType == SearchOUIDType.LogOnName &&
				(objType & ~(SchemaType.Users | SchemaType.Sideline)) != SchemaType.Unspecified, Resource.OnlyUserCanUserLogOnNameQuery);

			OguObjectCollection<T> result = null;

			if (ids.Length > 0)
			{
				string[] notInCacheIds = null;
				IList<T> objsInCache = null;

				if (ServiceBrokerContext.Current.UseLocalCache)
					objsInCache = OguObjectCacheBase.GetInstance(idType).GetObjectsInCache<T>(ids, out notInCacheIds);
				else
				{
					notInCacheIds = ids;
					objsInCache = new List<T>();
				}

				if (notInCacheIds.Length > 0)
				{
					string multiIDString = BuildIDString(notInCacheIds);

					DataSet ds = OguReaderServiceBroker.Instance.GetObjectsDetail(
							SchemaTypeToString(objType),
							multiIDString,
							(int)idType,
							string.Empty,
							0,
							Common.DefaultAttrs);

					//RemoveDeletedObject(ds.Tables[0]); //原来有这句话,现在去掉,可以查询已经逻辑删除的对象

					OguObjectCollection<T> queryResult = new OguObjectCollection<T>(Common.BuildObjectsFromTable<T>(ds.Tables[0]));

					queryResult = queryResult.GetRemovedDuplicateDeletedObjectCollection();

					if (ServiceBrokerContext.Current.UseLocalCache)
						OguObjectCacheBase.GetInstance(idType).AddObjectsToCache<T>(queryResult);

					foreach (T obj in queryResult)
						objsInCache.Add(obj);
				}

				result = new OguObjectCollection<T>(objsInCache);
				result.Sort(OrderByPropertyType.GlobalSortID, SortDirectionType.Ascending);
			}
			else
				result = new OguObjectCollection<T>();

			return result;
		}

		public IOrganization GetRoot()
		{
			string rootPath = OguPermissionSettings.GetConfig().RootOUPath;

			OguObjectCollection<IOrganization> depts;

			if (string.IsNullOrEmpty(rootPath) == false)
				depts = GetObjects<IOrganization>(SearchOUIDType.FullPath, rootPath);
			else
			{
				DataTable table = OguReaderServiceBroker.Instance.GetRootDSE().Tables[0];

				depts = new OguObjectCollection<IOrganization>(Common.BuildObjectsFromTable<IOrganization>(table));
			}

			ExceptionHelper.FalseThrow(depts.Count > 0, Resource.CanNotFindRootOU, rootPath);
			return depts[0];
		}

		/// <summary>
		/// 用户认证，通常是判断用户的用户名和口令是否正确
		/// </summary>
		/// <param name="identity">用户的登录名、口令和域</param>
		/// <returns>是否认证成功</returns>
		public bool AuthenticateUser(LogOnIdentity identity)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(identity != null, "identity");

			return OguReaderServiceBroker.Instance.SignInCheck(identity.LogOnNameWithoutDomain, identity.Password);
		}

		/// <summary>
		/// 清除所有的缓存
		/// </summary>
		public void RemoveAllCache()
		{
			OguReaderServiceBroker.Instance.RemoveAllCache();
		}
		#endregion

		#region IOguImplInterface
		/// <summary>
		/// 返回机构对象的子对象
		/// </summary>
		/// <typeparam name="T">期望返回的结果对象的类型，IOrganization、IUser、IGroup或IOguObject</typeparam>
		/// <param name="parent">父机构对象</param>
		/// <param name="includeSideline">对象的类型</param>
		/// <param name="searchLevel">查询的深度，单级或所有子对象</param>
		/// <returns></returns>
		public OguObjectCollection<T> GetChildren<T>(IOrganization parent, bool includeSideline, SearchLevel searchLevel) where T : IOguObject
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(parent != null, "parent");

			SchemaType objType = OguObjectHelper.GetSchemaTypeFromInterface<T>();

			if (includeSideline)
				objType |= SchemaType.Sideline;

			DataTable table = OguReaderServiceBroker.Instance.GetOrganizationChildren(
									parent.ID,
									(int)SearchOUIDType.Guid,
									(int)objType,
									(int)ServiceBrokerContext.Current.ListObjectCondition,
									(int)searchLevel,
									string.Empty,
									string.Empty,
									string.Empty,
									Common.DefaultAttrs).Tables[0];

			if ((objType & SchemaType.Organizations) == SchemaType.Organizations)
				RemoveParentDeptRow(table.Rows, parent);

			return new OguObjectCollection<T>(Common.BuildObjectsFromTable<T>(table, parent));
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
			ExceptionHelper.FalseThrow<ArgumentNullException>(parent != null, "parent");
			//ExceptionHelper.CheckStringIsNullOrEmpty(matchString, "matchString");

			SchemaType objType = OguObjectHelper.GetSchemaTypeFromInterface<T>();

			if (includeSideLine)
				objType |= SchemaType.Sideline;

			int nDep = 0;

			if (level == SearchLevel.OneLevel)
				nDep = 1;

			DataTable table = OguReaderServiceBroker.Instance.QueryOGUByCondition3(
									parent.ID,
									(int)SearchOUIDType.Guid,
									matchString,
									true,
									Common.DefaultAttrs,
									(int)objType,
									(int)ServiceBrokerContext.Current.ListObjectCondition,
									nDep,
									string.Empty,
									returnCount).Tables[0];

			if ((objType & SchemaType.Organizations) == SchemaType.Organizations)
				RemoveParentDeptRow(table.Rows, parent);

			return new OguObjectCollection<T>(Common.BuildObjectsFromTable<T>(table, parent));
		}

		/// <summary>
		/// 初始化祖先的OUs
		/// </summary>
		/// <param name="current"></param>
		public void InitAncestorOUs(IOguObject current)
		{
            IList<IOrganization> ancestorOUs = null;

            if (current.FullPath.IsNotEmpty())
                ancestorOUs = InitAncestorOUsByFullPath(current);
            else
                ancestorOUs = InitAncestorOUsByParentID(current);

            for (int i = ancestorOUs.Count - 1; i >= 0; i--)
            {
                IOrganization org = ancestorOUs[i];

                if (org.DepartmentType != DepartmentTypeDefine.XuNiJiGou)
                {
                    ((IOguPropertyAccessible)current).Parent = org;
                    current = ((IOguPropertyAccessible)current).Parent;
                }
            }
		}

        private static IList<IOrganization> InitAncestorOUsByFullPath(IOguObject current)
        {
            string[] allFullPath = GetAncestorsFullPath(current.FullPath);

            List<IOrganization> result = null;

            if (allFullPath.Length > 0)
            {
                OguObjectCollection<IOrganization> organizations = OguMechanismFactory.GetMechanism().GetObjects<IOrganization>(
                    SearchOUIDType.FullPath,
                    allFullPath);

                organizations.Sort(OrderByPropertyType.FullPath, SortDirectionType.Ascending);

                result = organizations.ToList();
            }
            else
                result = new List<IOrganization>();

            return result;
        }

        private static IList<IOrganization> InitAncestorOUsByParentID(IOguObject current)
        {
            List<IOrganization> result = new List<IOrganization>();

            string parentID = current.Properties.GetValue("PARENT_GUID", string.Empty);

            while (parentID.IsNotEmpty())
            {
                OguObjectCollection<IOrganization> parents = 
                    OguMechanismFactory.GetMechanism().GetObjects<IOrganization>(SearchOUIDType.Guid, parentID);

                if (parents.Count > 0)
                {
                    result.Insert(0, parents[0]);
                    current = parents[0];
                    parentID = current.Properties.GetValue("PARENT_GUID", string.Empty);
                }
                else
                    break;
            }

            return result;
        }

		/// <summary>
		/// 得到某用户的所有相关用户信息，包括主职和兼职的
		/// </summary>
		/// <param name="user">用户对象</param>
		/// <returns>所有相关用户信息，包括主职和兼职的</returns>
		public OguObjectCollection<IUser> GetAllRelativeUserInfo(IUser user)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(user != null, "user");

			DataTable table = OguReaderServiceBroker.Instance.GetObjectsDetail(SchemaType.Users.ToString().ToUpper(),
										user.ID,
										(int)SearchOUIDType.Guid,
										string.Empty,
										(int)SearchOUIDType.Guid,
										Common.DefaultAttrs).Tables[0];

			RemoveDeletedObject(table);

			return new OguObjectCollection<IUser>(Common.BuildObjectsFromTable<IUser>(table));
		}

		/// <summary>
		/// 得到某用户属于的所有组
		/// </summary>
		/// <param name="user">用户对象</param>
		/// <returns>用户属于的组</returns>
		public OguObjectCollection<IGroup> GetGroupsOfUser(IUser user)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(user != null, "user");

			string multiIDString = BuildIDString(user.ID);

			DataTable table = OguReaderServiceBroker.Instance.GetGroupsOfUsers(
								multiIDString,
								(int)SearchOUIDType.Guid,
								string.Empty,
								1,
								string.Empty,
								1).Tables[0];

			return new OguObjectCollection<IGroup>(Common.BuildObjectsFromTable<IGroup>(table));
		}

		/// <summary>
		/// 得到组当中的用户
		/// </summary>
		/// <param name="group">组对象</param>
		/// <returns>组当中的用户</returns>
		public OguObjectCollection<IUser> GetGroupMembers(IGroup group)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(group != null, "group");

			string multiIDString = BuildIDString(group.ID);

			DataTable table = OguReaderServiceBroker.Instance.GetUsersInGroups(
								multiIDString,
								(int)SearchOUIDType.Guid,
								Common.DefaultAttrs,
								string.Empty,
								1,
								string.Empty,
								1).Tables[0];

			return new OguObjectCollection<IUser>(Common.BuildObjectsFromTable<IUser>(table));
		}

		/// <summary>
		/// 得到某个用户的秘书
		/// </summary>
		/// <param name="user">某个用户</param>
		/// <returns>用户的秘书</returns>
		public OguObjectCollection<IUser> GetSecretaries(IUser user)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(user != null, "user");

			string multiIDString = BuildIDString(user.ID);

			DataTable table = OguReaderServiceBroker.Instance.GetSecretariesOfLeaders
								(multiIDString,
								(int)SearchOUIDType.Guid,
								Common.DefaultAttrs,
								1).Tables[0];

			return new OguObjectCollection<IUser>(Common.BuildObjectsFromTable<IUser>(table));
		}

		/// <summary>
		/// 得到某个用户，属于谁的秘书
		/// </summary>
		/// <param name="user">某个用户</param>
		/// <returns>是谁的秘书</returns>
		public OguObjectCollection<IUser> GetSecretaryOf(IUser user)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(user != null, "user");

			string multiIDString = BuildIDString(user.ID);

			DataTable table = OguReaderServiceBroker.Instance.GetLeadersOfSecretaries
								(multiIDString,
								(int)SearchOUIDType.Guid,
								Common.DefaultAttrs,
								1).Tables[0];

			return new OguObjectCollection<IUser>(Common.BuildObjectsFromTable<IUser>(table));
		}
		#endregion

		private static string SchemaTypeToString(SchemaType objType)
		{
			string result = string.Empty;

			if (((objType & SchemaType.Users) == SchemaType.Unspecified) ||
				((objType & SchemaType.Organizations) == SchemaType.Unspecified) ||
				((objType & SchemaType.Groups) == SchemaType.Unspecified))
				result = objType.ToString().ToUpper();

			return result;
		}

		private static string BuildIDString(params string[] strIDs)
		{
			StringBuilder strB = new StringBuilder(512);

			for (int i = 0; i < strIDs.Length; i++)
			{
				if (strB.Length > 0)
					strB.Append(",");

				strB.Append(strIDs[i]);
			}

			return strB.ToString();
		}

		/// <summary>
		/// 如果结果集中的第一行和父部门相同，则删除之。如此做的原因和袁勇机构人员查询的特殊行为有关，有疑问请直接问他，MP:13911568920
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="parent"></param>
		private static void RemoveParentDeptRow(DataRowCollection rows, IOguObject parent)
		{
			if (rows.Count > 0)
				if (rows[0]["GUID"].ToString() == parent.ID)
					rows.RemoveAt(0);
		}

		private static void RemoveDeletedObject(DataTable table)
		{
			int i = 0;

			while (i < table.Rows.Count)
			{
				DataRow row = table.Rows[i];

				if ((int)row["STATUS"] != 1)
					table.Rows.RemoveAt(i);
				else
					i++;
			}
		}

		private static string[] GetAncestorsFullPath(string fullPath)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(fullPath, "FullPath");

			string[] strParts = fullPath.Split('\\');

			string[] result = new string[strParts.Length - 1];

			for (int i = 0; i < result.Length; i++)
				if (i > 0)
					result[i] = result[i - 1] + "\\" + strParts[i];
				else
					result[i] = strParts[i];

			return result;
		}
	}
}
