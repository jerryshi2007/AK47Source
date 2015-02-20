using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PC = MCS.Library.SOA.DataObjects.Security;
using OP = MCS.Library.OGUPermission;
using MCS.Library.OGUPermission;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.PermissionBridge
{
	static class Util
	{
		/// <summary>
		/// 根据类型创建对应的对象
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="resultType"></param>
		/// <returns></returns>
		public static T CreateObject<T>(OP.SchemaType resultType)
		{
			return (T)OguPermissionSettings.GetConfig().OguObjectFactory.CreateObject(resultType);
		}

		public static PC.SchemaObjectCollection QuerySchemaObjectsById(string[] pIds)
		{
			if (pIds == null)
				throw new ArgumentNullException("pIds");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
			builder.AppendItem("Status", (int)SchemaObjectStatus.Normal);

			InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("ID");
			inBuilder.AppendItem(pIds);

			ConnectiveSqlClauseCollection connBuilder = new ConnectiveSqlClauseCollection();
			connBuilder.Add(builder);
			connBuilder.Add(inBuilder);

			return PC.Adapters.SchemaObjectAdapter.Instance.Load(connBuilder);
		}


		public static IOguObjectFactory GetOguObjectFactory()
		{
			return OguPermissionSettings.GetConfig().OguObjectFactory;
		}

		public static IPermissionObjectFactory GetPermissionObjectFactory()
		{
			return OguPermissionSettings.GetConfig().PermissionObjectFactory;
		}

		public static IOrganizationMechanism GetOrganizationMechanism()
		{
			return OguPermissionSettings.GetConfig().OguFactory;
		}

		internal static bool GetContextIncludeDeleted()
		{
			var flags = ServiceBrokerContext.Current.ListObjectCondition;
			return (flags & (ListObjectMask.DeletedByOrganization | ListObjectMask.DeletedByUser | ListObjectMask.DirectDeleted)) != 0;
		}
	}
}
