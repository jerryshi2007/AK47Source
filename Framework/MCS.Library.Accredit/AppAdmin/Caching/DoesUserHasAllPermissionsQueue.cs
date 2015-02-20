#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Accredit
// FileName	��	DoesUserHasAllPermissionsQueue.cs
// Remark	��		DoesUserHasAllPermissions�ӿ�ʵ���ϵ����ݻ�����е�ʵ��
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
	internal class DoesUserHasAllPermissionsQueue : CacheQueue<string, bool>
	{
		//internal static object CacheQueueSync = new object();
		public static readonly DoesUserHasAllPermissionsQueue Instance = CacheManager.GetInstance<DoesUserHasAllPermissionsQueue>();
		private DoesUserHasAllPermissionsQueue() { }
	}
}
