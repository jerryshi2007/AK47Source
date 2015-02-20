using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using System.Web.Script.Serialization;
using MCS.Web.Library.Script;
using MCS.Library.Core;
using System.Collections;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	/// <summary>
	/// 为OU控件提供查询实现
	/// </summary>
	public class SCOuControlQueryImpl : IUserOUControlQuery
	{
		static SCOuControlQueryImpl()
		{
			JSONSerializerExecute.RegisterConverter(typeof(PhantomOguConverter)); // 注册序列化工具
		}

		/// <summary>
		/// 根据ID查询所有父级对象
		/// </summary>
		/// <param name="ids">一个或多个对象ID</param>
		/// <returns></returns>
		public Dictionary<string, IEnumerable<IOrganization>> QueryObjectsParents(params string[] ids)
		{
			Dictionary<string, IEnumerable<IOrganization>> result = new Dictionary<string, IEnumerable<IOrganization>>(20);

			var parentsRelations = this.LoadParentOguRelations(ids);

			var parentIds = parentsRelations.ToParentIDArray();

			Dictionary<string, SCSimpleObjectCollection> superParents = SCSnapshotAdapter.Instance.LoadAllParentsInfo(true, parentIds);

			foreach (string id in ids)
			{
				var firstParentId = (from p in parentsRelations where p.ID == id select p.ParentID).FirstOrDefault();

				if (firstParentId != null)
				{
					var tmpParents = superParents[firstParentId].ToOguObjects<IOrganization>();

					result[id] = tmpParents;
				}
			}

			return result;
		}

		private SCRelationObjectCollection LoadParentOguRelations(string[] ids)
		{
			InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("ObjectID");
			inBuilder.AppendItem(ids);

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendItem("ParentSchemaType", "Organizations");
			where.AppendItem("Status", (int)SchemaObjectStatus.Normal);

			return SchemaRelationObjectAdapter.Instance.Load(new ConnectiveSqlClauseCollection(inBuilder, where), DateTime.MinValue);
		}

		/// <summary>
		/// 查询用户扩展信息
		/// </summary>
		/// <param name="ids">一个或多个用户ID</param>
		/// <returns></returns>
		public UserInfoExtendCollection QueryUsersExtendedInfo(params string[] ids)
		{
			UserInfoExtendCollection result = new UserInfoExtendCollection();

			InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("ID");
			inBuilder.AppendItem(ids);

			if (inBuilder.IsEmpty == false)
			{
				WhereSqlClauseBuilder wBuilder = new WhereSqlClauseBuilder();
				wBuilder.AppendItem("Status", (int)SchemaObjectStatus.Normal);

				ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(LogicOperatorDefine.And, inBuilder, wBuilder);
				SchemaObjectCollection objs = SchemaObjectAdapter.Instance.Load(connectiveBuilder);

				objs.ForEach(o => result.Add(o.ToUserExtendedInfo()));
			}

			return result;
		}

		/// <summary>
		/// 查询用户的即时消息地址
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		public UserIMAddressCollection QueryUsersIMAddress(params string[] ids)
		{
			UserInfoExtendCollection extendedInfo = QueryUsersExtendedInfo(ids);

			UserIMAddressCollection result = new UserIMAddressCollection();

			extendedInfo.ForEach(eu => result.Add(new UserIMAddress(eu.ID, eu.IMAddress)));

			return result;
		}

		public OguObjectCollection<IOguObject> GetObjects(params string[] ids)
		{
			ids.NullCheck("ids");

			SCObjectAndRelationCollection relations = SCSnapshotAdapter.Instance.QueryObjectAndRelationByIDs(SchemaInfo.FilterByCategory("Users", "Groups", "Organizations").ToSchemaNames(), ids, false, DateTime.MinValue);
			relations.FillDetails();

			List<IOguObject> list = new List<IOguObject>(relations.Count);
			foreach (var item in relations)
			{
				SchemaObjectBase obj = item.Detail;
				if (obj != null)
				{
					IOguObject oguObj = obj.ToSimpleObject().ToOguObject();
					list.Add(oguObj);
				}

			}

			return new OguObjectCollection<IOguObject>(list);
		}

		public IOrganization GetOrganizationByPath(string rootPath)
		{
			return (IOrganization)SCOrganization.GetRoot().ToPhantom();
		}

		private OguObjectCollection<T> QueryDescendants<T>(IOrganization parent, string prefix, int maxCount, params string[] schemas) where T : IOguObject
		{
			var relations = SCSnapshotAdapter.Instance.QueryObjectAndRelationByKeywordAndParentIDs(schemas,
				new string[] { parent.ID }, prefix, maxCount, true, true, ServiceBrokerContext.Current.ListObjectCondition != ListObjectMask.Common, DateTime.MinValue);

			relations.FillDetails();

			// 验证
			#region 老
			// var parents = SCSnapshotAdapter.Instance.LoadAllParentsInfo(relations.ToIDArray());

			// return new OguObjectCollection<T>((from o in relations select (T)(IOguObject)FillParents(o.Detail.ToPhantom(), parents[o.ID])).ToArray()); 
			#endregion

			var parents = SCSnapshotAdapter.Instance.LoadAllParentsInfo(true, relations.ToParentIDArray());

			return new OguObjectCollection<T>((from o in relations select (T)(IOguObject)this.FillParents(o.Detail.ToPhantom(), parents[o.ParentID])).ToArray());
		}

		public OguObjectCollection<IOguObject> QueryDescendants(IOrganization parent, string prefix, int maxCount)
		{
			return this.QueryDescendants<IOguObject>(parent, prefix, maxCount, SchemaInfo.FilterByCategory("Users", "Groups", "Organizations").ToSchemaNames());
		}

		private PhantomOguBase FillParents(PhantomOguBase phantom, SCSimpleObjectCollection parents)
		{
			PhantomOrganization parent = null, tmpParent = null;
			string fullPath = "";
			for (int i = 0; i < parents.Count; i++)
			{
				var simpleObj = parents[i];

				tmpParent = new PhantomOrganization()
				{
					ID = simpleObj.ID,
					Name = simpleObj.Name,
					DisplayName = simpleObj.DisplayName,
					ObjectType = SchemaType.Organizations,
				};
				if (i > 0)
				{
					fullPath += "\\" + tmpParent.Name;
				}
				else
				{
					fullPath = tmpParent.Name;
				}

				tmpParent.Properties["STATUS"] = simpleObj.Status == SchemaObjectStatus.Normal ? 1 : 3;
				tmpParent.FullPath = fullPath;

				if (parent != null)
				{
					tmpParent.Dummy.Parent = parent;
				}

				parent = tmpParent;
			}

			phantom.Dummy.Parent = parent;
			if (fullPath.Length > 0)
			{
				phantom.FullPath = fullPath + "\\" + phantom.Name;
			}
			else
			{
				phantom.FullPath = phantom.Name;
			}

			return phantom;
		}

		public IEnumerable<IOguObject> GetChildren(IOrganization parent)
		{
			SCObjectAndRelationCollection relations = SCSnapshotAdapter.Instance.QueryObjectAndRelationByParentIDs(SchemaInfo.FilterByCategory("Users", "Groups", "Organizations").ToSchemaNames(), new string[] { parent.ID }, false, true, false, DateTime.MinValue);

			relations.FillDetails();

			var parentList = SCSnapshotAdapter.Instance.LoadAllParentsInfo(true, parent.ID)[parent.ID];

			StringBuilder strB = new StringBuilder(parentList.Count * 15);

			for (int i = 0; i < parentList.Count; i++)
			{
				strB.Append(parentList[i].Name);
				strB.Append("");
			}

			var parentPath = strB.ToString();

			foreach (var item in relations)
			{
				OguBase ogu = (OguBase)item.Detail.ToPhantom();
				ogu.FullPath = parentPath + ogu.Name;

				yield return ogu;
			}
		}

		public OguObjectCollection<T> QueryDescendants<T>(SchemaType type, IOrganization parent, string prefix, int maxCount) where T : IOguObject
		{
			List<string> schemas = new List<string>(8);
			if ((type & SchemaType.Users) == SchemaType.Users)
			{
				schemas.Add("Users");
			}

			if ((type & SchemaType.Groups) == SchemaType.Groups)
			{
				schemas.Add("Groups");
			}

			if ((type & SchemaType.Organizations) == SchemaType.Organizations)
			{
				schemas.Add("Organizations");
			}

			var result = this.QueryDescendants<T>(parent, prefix, maxCount, schemas.ToArray());
			return result;
		}

		public OguObjectCollection<T> QueryDescendants<T>(SchemaQueryType type, IOrganization parent, string prefix, int maxCount) where T : IOguObject
		{
			List<string> schemas = new List<string>(8);

			if ((type & SchemaQueryType.Users) == SchemaQueryType.Users)
			{
				schemas.Add("Users");
				CheckAssignable<T, IUser>();
			}

			if ((type & SchemaQueryType.Groups) == SchemaQueryType.Groups)
			{
				schemas.Add("Groups");
				CheckAssignable<T, IGroup>();
			}

			if ((type & SchemaQueryType.Organizations) == SchemaQueryType.Organizations)
			{
				schemas.Add("Organizations");
				CheckAssignable<T, IOrganization>();
			}

			var result = this.QueryDescendants<T>(parent, prefix, maxCount, schemas.ToArray());

			return result;
		}

		private void CheckAssignable<T1, T2>()
		{
			if (typeof(T1).IsAssignableFrom(typeof(T2)) == false)
				throw new ArgumentException(string.Format("泛型参数中指定的返回类型{0}与检索过滤条件中的{1}不兼容", typeof(T2).Name, typeof(T1).Name));
		}
	}
}
