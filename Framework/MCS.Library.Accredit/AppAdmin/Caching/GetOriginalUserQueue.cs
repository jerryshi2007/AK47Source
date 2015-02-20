#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Accredit
// FileName	��	GetOriginalUserQueue.cs
// Remark	��		GetOriginalUser�ӿ�ʵ���ϵ����ݻ�����е�ʵ��
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
	internal class GetOriginalUserQueue : CacheQueue<string, System.Data.DataSet>
	{
		//internal static object CacheQueueSync = new object();
		public static readonly GetOriginalUserQueue Instance = CacheManager.GetInstance<GetOriginalUserQueue>();
		private GetOriginalUserQueue() { }
	}
}
