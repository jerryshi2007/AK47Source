#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Passport
// FileName	：	DefaultPrincipalBuilder.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          胡自强      2008-12-2       添加注释
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Security.Principal;
using System.Collections.Generic;
using MCS.Library.Passport;

namespace MCS.Library.Principal
{
    /// <summary>
    /// 缺省的Principal的构造器
    /// </summary>
    internal class DefaultPrincipalBuilder : IPrincipalBuilder
    {
        #region IPrincipalBuilder 成员

        public IPrincipal CreatePrincipal(string userID, ITicket ticket)
        {
            DeluxeIdentity userIdentity = new DeluxeIdentity(userID, ticket);

            return new DeluxePrincipal(userIdentity);
        }

        #endregion
    }
}
