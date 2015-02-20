#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Accredit
// FileName	��	GetUserApplicationsForDelegationQueue.cs
// Remark	��		GetUserApplicationsForDelegation�ӿ�ʵ���ϵ����ݻ�����е�ʵ��
// -------------------------------------------------
// VERSION  	AUTHOR				DATE					CONTENT
// 1.0				ccic\yuanyong		2008121630		�´���
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.Accredit.AppAdmin.Caching
{
	internal class GetUserApplicationsForDelegationQueue : CacheQueue<string, System.Data.DataSet>
	{
		//internal static object CacheQueueSync = new object();
		public static readonly GetUserApplicationsForDelegationQueue Instance = CacheManager.GetInstance<GetUserApplicationsForDelegationQueue>();
		private GetUserApplicationsForDelegationQueue() { }
	}
}
