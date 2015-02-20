#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	DefaultPrincipalBuilder.cs
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
using MCS.Library.Passport;

namespace MCS.Library.Principal
{
    /// <summary>
    /// ȱʡ��Principal�Ĺ�����
    /// </summary>
    internal class DefaultPrincipalBuilder : IPrincipalBuilder
    {
        #region IPrincipalBuilder ��Ա

        public IPrincipal CreatePrincipal(string userID, ITicket ticket)
        {
            DeluxeIdentity userIdentity = new DeluxeIdentity(userID, ticket);

            return new DeluxePrincipal(userIdentity);
        }

        #endregion
    }
}
