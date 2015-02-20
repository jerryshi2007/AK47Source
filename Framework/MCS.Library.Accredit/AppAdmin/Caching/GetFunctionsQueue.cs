#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Accredit
// FileName	��	GetFunctionsQueue.cs
// Remark	��		GetFunctions�ӿ�ʵ���ϵ����ݻ�����е�ʵ��
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
	internal class GetFunctionsQueue : CacheQueue<string, System.Data.DataSet>
	{
		//internal static object CacheQueueSync = new object();
		public static readonly GetFunctionsQueue Instance = CacheManager.GetInstance<GetFunctionsQueue>();
		private GetFunctionsQueue() { }
	}
}
