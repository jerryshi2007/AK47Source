#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Accredit
// FileName	��	DoesUserHasPermissionsQueue.cs
// Remark	��		DoesUserHasPermissions�ӿ�ʵ���ϵ����ݻ�����е�ʵ��
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
	internal class DoesUserHasPermissionsQueue : CacheQueue<string, bool>
	{
		//internal static object CacheQueueSync = new object();
		public static readonly DoesUserHasPermissionsQueue Instance = CacheManager.GetInstance<DoesUserHasPermissionsQueue>();
		private DoesUserHasPermissionsQueue() { }
	}
}
