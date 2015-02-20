using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MCS.Library.Data;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Logs;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.Principal;
using System.Runtime;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	/// <summary>
	/// 管理单元公共数据
	/// </summary>
	public static class AUCommon
	{
		// 内定的Schema类型

		public const string SchemaAUSchema = "AUSchemas";
		public const string SchemaAUSchemaRole = "AUSchemaRoles";
		public const string SchemaAdminUnit = "AdminUnits";
		public const string SchemaAUAdminScope = "AUAdminScopes";
		public const string SchemaAdminScopeItem = "AUScopeItems";
		public const string SchemaAdminUnitRole = "AURoles";
		public const string ConditionType = "ADM";
		/// <summary>
		/// 数据库连接名称
		/// </summary>
		public const string DBConnectionName = "AdminUnit";

		// 管理单元的Acl
		public const string AUAclEditRoleMembers = "EditRoleMembers";
		public const string AUAclEditAdminScope = "EditAdminScope";
		public const string AUAclEditProperty = "EditProperty";
		public const string AUAclAddSubUnit = "AddSubUnit";
		public const string AUAclDeleteSubUnit = "DeleteSubUnit";
		public const string AUAclEditSubUnitAcl = "EditSubUnitAcl";
		internal static readonly char[] Spliter = { ',' };
		public static readonly string[] ZeroLengthStringArray = { };

		/// <summary>
		/// 创建一个基本的日志
		/// </summary>
		/// <param name="data"></param>
		/// <param name="opType"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		internal static Logs.AUOperationLog ToOperationLog(this SchemaObjectBase data, Executors.AUOperationType opType, string message)
		{
			data.NullCheck("data");

			AUOperationLog log = AUOperationLog.CreateLogFromEnvironment();

			log.ResourceID = data.ID;
			log.SchemaType = data.SchemaType;
			log.OperationType = opType;
			log.Category = data.Schema.Category;
			log.Subject = message;

			log.SearchContent = data.ToFullTextString();

			return log;
		}

		/// <summary>
		/// 创建一个基本的日志
		/// </summary>
		/// <param name="data"></param>
		/// <param name="aUOperationType"></param>
		/// <returns></returns>
		internal static Logs.AUOperationLog ToOperationLog(this SchemaObjectBase data, Executors.AUOperationType opType)
		{
			return ToOperationLog(data, opType, string.Format("{0}: {1}",
				EnumItemDescriptionAttribute.GetDescription(opType), AUCommon.DisplayNameFor(data)));
		}

		[DebuggerNonUserCode]
		public static void DoDbAction(Action method)
		{
			DbConnectionMappingContext.DoMappingAction(MCS.Library.SOA.DataObjects.Security.Adapters.SCConnectionDefine.DBConnectionName, GetMappedName(AUCommon.DBConnectionName), () =>
			{
				method();
			});

			return;
		}

		[DebuggerNonUserCode]
		public static R DoDbProcess<R>(Func<R> method)
		{
			R result = default(R);
			DbConnectionMappingContext.DoMappingAction(PC.Adapters.SCConnectionDefine.DBConnectionName, GetMappedName(AUCommon.DBConnectionName), () =>
			{
				result = method();
			});

			return result;
		}

		[TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
		private static string GetMappedName(string name)
		{
			//name = DbConnectionMappingContext.GetMappedConnectionName(name);
			//return name;

			return name;
		}

		internal static string DisplayNameFor(SchemaObjectBase obj)
		{
			if (obj is SCBase)
			{
				SCBase sc = (SCBase)obj;
				return string.IsNullOrEmpty(sc.DisplayName) ? sc.Name : sc.DisplayName;
			}
			else
			{
				return obj.SchemaType + "对象";
			}
		}

		internal static T GetUniqueNormalObject<T>(this SchemaObjectCollection obj) where T : SchemaObjectBase
		{
			if (obj == null) throw new ArgumentNullException("obj");

			T result = null;

			foreach (var item in obj)
			{
				if (item.Status == Schemas.SchemaProperties.SchemaObjectStatus.Normal && item is T)
				{
					if (result == null)
						result = (T)item;
					else
						throw new DuplicateObjectException();
				}
			}

			return result;
		}

		/// <summary>
		/// 获取唯一的正常状态的对象
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		internal static bool GetUniqueNormalObject<T>(this SchemaObjectCollection obj, out T result) where T : SchemaObjectBase
		{
			if (obj == null) throw new ArgumentNullException("obj");
			bool isOk = true;

			result = null;

			foreach (var item in obj)
			{
				if (item.Status == Schemas.SchemaProperties.SchemaObjectStatus.Normal && item is T)
				{
					if (result == null)
						result = (T)item;
					else
					{
						isOk = false;
						result = null;
						break;
					}
				}
			}

			return isOk;
		}

		internal static AURole FindMatchRole(SchemaObjectCollection auRoles, AUSchemaRole schemaRole)
		{
			return (AURole)auRoles.Find(m => ((AURole)m).SchemaRoleID == schemaRole.ID);
		}

		internal static AURole FindMatchRole(IEnumerable<AURole> auRoles, AUSchemaRole schemaRole)
		{
			AURole result = null;
			foreach (var item in auRoles)
			{
				if (item.SchemaRoleID == schemaRole.ID)
				{
					result = item;
					break;
				}
			}

			return result;
		}

		internal static AUAdminScope FindMatchScope(SchemaObjectCollection scopes, string name)
		{
			return (AUAdminScope)scopes.Find(m => ((AUAdminScope)m).ScopeSchemaType == name);
		}

		internal static AUAdminScope FindMatchScope(IEnumerable<AUAdminScope> scopes, string name)
		{
			AUAdminScope result = null;
			foreach (var item in scopes)
			{
				if (item.ScopeSchemaType == name)
				{
					result = item;
					break;
				}
			}

			return result;
		}

		internal static string FullTextFor(SchemaObjectBase obj)
		{
			return obj.ToFullTextString();
		}

		public static AUAdminScopeTypeCollection GetAdminScopeTypes()
		{
			AUAdminScopeTypeCollection all = new AUAdminScopeTypeCollection();

			var schemas = SchemaInfo.FilterByCategory("AUScopeItems");

			foreach (var item in schemas)
			{
				all.Add(new AUAdminScopeType() { SchemaName = item.Description, SchemaType = item.Name });
			}

			return all;
		}
	}
}
