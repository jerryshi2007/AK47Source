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

        /// <summary>
        /// 创建 <see cref="IPrincipal"/> 对象
        /// </summary>
        /// <param name="userID">用户标识</param>
        /// <param name="ticket">票据</param>
        /// <returns></returns>
        public IPrincipal CreatePrincipal(string userID, ITicket ticket)
        {
            DeluxeIdentity identity = new DeluxeIdentity(userID, ticket);

            return new DeluxePrincipal(identity);
        }

        /// <summary>
        /// 创建 <see cref="IPrincipal"/> 对象
        /// </summary>
        /// <param name="tokenContainer">token的容器</param>
        /// <param name="ticket">票据</param>
        /// <returns></returns>
        public IPrincipal CreatePrincipal(GenericTicketTokenContainer tokenContainer, ITicket ticket)
        {
            DeluxeIdentity identity = new DeluxeIdentity(tokenContainer, ticket);

            return new DeluxePrincipal(identity);
        }
        #endregion
    }
}
