using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Executors;

namespace MCS.Library.SOA.DataObjects.Security.Permissions
{
	/// <summary>
	/// 权限检查的异常类
	/// </summary>
	public class SCAclPermissionCheckException : System.Exception
	{
		public SCAclPermissionCheckException()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="message"></param>
		public SCAclPermissionCheckException(string message) :
			base(message)
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		public SCAclPermissionCheckException(string message, System.Exception exception) :
			base(message, exception)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="opType"></param>
		/// <param name="permissionName"></param>
		public static SCAclPermissionCheckException CreateException(SCOperationType opType, SchemaDefine schemaInfo, string permissionName)
		{
			string opDesp = EnumItemDescriptionAttribute.GetDescription(opType);

			SCAclPermissionItem permissionInfo = schemaInfo.PermissionSet[permissionName];

			string permissionDesp = string.Empty;

			if (permissionInfo != null)
			{
				permissionDesp = permissionInfo.Description;

				if (permissionDesp.IsNullOrEmpty())
					permissionDesp = permissionInfo.Name;
			}

			return new SCAclPermissionCheckException(string.Format("不能执行\"{0}\"操作，您没有\"{0}\"权限", opDesp, permissionDesp));
		}
	}
}
