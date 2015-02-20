#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.OGUPermission
// FileName	��	OguAdminMechanism.cs
// Remark	��	������Ա
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���	    20070430		����
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

		#region IOguOperations ��Ա

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

					//RemoveDeletedObject(ds.Tables[0]); //ԭ������仰,����ȥ��,���Բ�ѯ�Ѿ��߼�ɾ���Ķ���

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
		/// �û���֤��ͨ�����ж��û����û����Ϳ����Ƿ���ȷ
		/// </summary>
		/// <param name="identity">�û��ĵ�¼�����������</param>
		/// <returns>�Ƿ���֤�ɹ�</returns>
		public bool AuthenticateUser(LogOnIdentity identity)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(identity != null, "identity");

			return OguReaderServiceBroker.Instance.SignInCheck(identity.LogOnNameWithoutDomain, identity.Password);
		}

		/// <summary>
		/// ������еĻ���
		/// </summary>
		public void RemoveAllCache()
		{
			OguReaderServiceBroker.Instance.RemoveAllCache();
		}
		#endregion

		#region IOguImplInterface
		/// <summary>
		/// ���ػ���������Ӷ���
		/// </summary>
		/// <typeparam name="T">�������صĽ����������ͣ�IOrganization��IUser��IGroup��IOguObject</typeparam>
		/// <param name="parent">����������</param>
		/// <param name="includeSideline">���������</param>
		/// <param name="searchLevel">��ѯ����ȣ������������Ӷ���</param>
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
		/// ���Ӷ�����в�ѯ�����м�����ȣ�
		/// </summary>
		/// <typeparam name="T">����������</typeparam>
		/// <param name="parent">����������</param>
		/// <param name="matchString">ģ����ѯ���ַ���</param>
		/// <param name="includeSideLine">�Ƿ������ְ����Ա</param>
		/// <param name="level">��ѯ�����</param>
		/// <param name="returnCount">���صļ�¼��</param>
		/// <returns>�õ���ѯ���Ӷ���</returns>
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
		/// ��ʼ�����ȵ�OUs
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
		/// �õ�ĳ�û�����������û���Ϣ��������ְ�ͼ�ְ��
		/// </summary>
		/// <param name="user">�û�����</param>
		/// <returns>��������û���Ϣ��������ְ�ͼ�ְ��</returns>
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
		/// �õ�ĳ�û����ڵ�������
		/// </summary>
		/// <param name="user">�û�����</param>
		/// <returns>�û����ڵ���</returns>
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
		/// �õ��鵱�е��û�
		/// </summary>
		/// <param name="group">�����</param>
		/// <returns>�鵱�е��û�</returns>
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
		/// �õ�ĳ���û�������
		/// </summary>
		/// <param name="user">ĳ���û�</param>
		/// <returns>�û�������</returns>
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
		/// �õ�ĳ���û�������˭������
		/// </summary>
		/// <param name="user">ĳ���û�</param>
		/// <returns>��˭������</returns>
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
		/// ���������еĵ�һ�к͸�������ͬ����ɾ��֮���������ԭ���Ԭ�»�����Ա��ѯ��������Ϊ�йأ���������ֱ��������MP:13911568920
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
