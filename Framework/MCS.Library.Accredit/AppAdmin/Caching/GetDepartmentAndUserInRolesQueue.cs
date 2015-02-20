#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Accredit
// FileName	��	GetDepartmentAndUserInRolesQueue.cs
// Remark	��		GetDepartmentAndUserInRoles�ӿ�ʵ���ϵ����ݻ�����е�ʵ��
// -------------------------------------------------
// VERSION  	AUTHOR				DATE					CONTENT
// 1.0				ccic\yuanyong		20081216			�´���
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.Accredit.AppAdmin.Caching
{
	internal class GetDepartmentAndUserInRolesQueue : CacheQueue<string, System.Data.DataSet>
	{
		//internal static object CacheQueueSync = new object();
		public static readonly GetDepartmentAndUserInRolesQueue Instance = CacheManager.GetInstance<GetDepartmentAndUserInRolesQueue>();
		private GetDepartmentAndUserInRolesQueue() { }
	}
}
