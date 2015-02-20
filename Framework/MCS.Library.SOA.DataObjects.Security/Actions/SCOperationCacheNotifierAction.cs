using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects.Security.Actions
{
	/// <summary>
	/// 执行操作时，更新Cache的插件
	/// </summary>
	public class SCOperationCacheNotifierAction : ISCObjectOperationAction
	{
		#region ISCObjectOperationAction Members

		public void BeforeExecute(SCOperationType operationType)
		{
		}

		public void AfterExecute(SCOperationType operationType)
		{
			switch (operationType)
			{
				case SCOperationType.UpdateObjectAcl:
				case SCOperationType.ReplaceAclRecursively:
					//内部授权操作，不影响一般查询的缓存
					break;
				case SCOperationType.UpdateObjectImage:
					//图像操作也不影响缓存
					break;
				default:
					SCCacheHelper.InvalidateAllCache();
					break;
			}
		}
		#endregion
	}
}
