#region
// -------------------------------------------------
// Assembly	£º	DeluxeWorks.Library.Passport
// FileName	£º	PrincipalCache.cs
// Remark	£º	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          ºú×ÔÇ¿      2008-12-2       Ìí¼Ó×¢ÊÍ
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
