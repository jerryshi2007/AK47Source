using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Library.Data.Builder;
using MCS.Library.Core;

namespace AUCenter
{
	public static class DbUtil
	{
		internal static MCS.Library.SOA.DataObjects.UserCustomSearchConditionCollection LoadSearchCondition(string resourceKey, string conditionType)
		{
			return UserCustomSearchConditionAdapter.Instance.Load(c =>
			{
				c.AppendItem("RESOURCE_ID", resourceKey);
				c.AppendItem("CONDITION_TYPE", conditionType);
				c.AppendItem("USER_ID", Util.CurrentUser.ID);
			});
		}

		internal static T GetEffectiveObject<T>(string id) where T : SchemaObjectBase
		{
			return GetEffectiveObject<T>(id, true);
		}

		internal static T GetEffectiveObject<T>(string id, bool normalOnly) where T : SchemaObjectBase
		{
			if (id == null)
				throw new ArgumentNullException("id");

			T result = null;

			AUCommon.DoDbAction(() =>
			{
				result = (T)PC.Adapters.SchemaObjectAdapter.Instance.Load(id);
			});

			if (result == null)
				throw new ObjectNotFoundException(string.Format("未找到指定ID：{0} 的 {1} 类型的对象。", id, typeof(T).Name));

			if (normalOnly && result.Status != SchemaObjectStatus.Normal)
				throw new ObjectNotFoundException(string.Format("{0} 无效，对象为已经删除。", result is SCBase ? (result as SCBase).ToDescription() : result.ID));

			return result;
		}

		internal static SchemaObjectBase GetEffectiveObject(string id, string message)
		{
			SchemaObjectBase result = null;
			AUCommon.DoDbAction(() =>
			{
				result = PC.Adapters.SchemaObjectAdapter.Instance.Load(id);
			});

			if (result == null)
				throw message != null ? new ObjectNotFoundException(message) : ObjectNotFoundException.CreateForID(id);
			else if (result.Status != SchemaObjectStatus.Normal)
				throw message != null ? new ObjectNotFoundException(message) : ObjectNotFoundException.CreateForID(id);

			return result;
		}

		internal static SchemaObjectCollection GetEffectiveObjects(string[] ids)
		{
			ids.NullCheck("ids");
			SchemaObjectCollection result = null;
			if (ids.Length > 0)
			{
				AUCommon.DoDbAction(() =>
				{
					InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("ID").AppendItem(ids);
					WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
					where.AppendItem("Status", (int)SchemaObjectStatus.Normal);
					result = PC.Adapters.SchemaObjectAdapter.Instance.Load(new ConnectiveSqlClauseCollection(inBuilder, where));
				});
			}
			else
			{
				result = new SchemaObjectCollection();
			}

			return result;
		}
	}
}