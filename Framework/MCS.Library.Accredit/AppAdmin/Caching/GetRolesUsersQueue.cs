#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Accredit
// FileName	��	GetRolesUsersQueue.cs
// Remark	��		GetRolesUsers�ӿ�ʵ���ϵ����ݻ�����е�ʵ��
// -------------------------------------------------
// VERSION  	AUTHOR				DATE					CONTENT
// 1.0				ccic\yuanyong		20081216		�´���
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.Accredit.AppAdmin.Caching
{
	internal class GetRolesUsersQueue : CacheQueue<string, System.Data.DataSet>
	{
		//internal static object CacheQueueSync = new object();
		public static readonly GetRolesUsersQueue Instance = CacheManager.GetInstance<GetRolesUsersQueue>();
		private GetRolesUsersQueue() { }
	}
}
