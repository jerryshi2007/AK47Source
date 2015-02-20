#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	PrincipalCache.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          ����ǿ      2008-12-2       ���ע��
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Security.Principal;
using System.Collections.Generic;
using MCS.Library.Caching;

namespace MCS.Library.Passport
{
    internal class PrincipalCache : CacheQueue<string, IPrincipal>
    {
        public static readonly PrincipalCache Instance = CacheManager.GetInstance<PrincipalCache>();
    }
}
