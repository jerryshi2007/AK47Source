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

        /// <summary>
        /// ���� <see cref="IPrincipal"/> ����
        /// </summary>
        /// <param name="userID">�û���ʶ</param>
        /// <param name="ticket">Ʊ��</param>
        /// <returns></returns>
        public IPrincipal CreatePrincipal(string userID, ITicket ticket)
        {
            DeluxeIdentity identity = new DeluxeIdentity(userID, ticket);

            return new DeluxePrincipal(identity);
        }

        /// <summary>
        /// ���� <see cref="IPrincipal"/> ����
        /// </summary>
        /// <param name="tokenContainer">token������</param>
        /// <param name="ticket">Ʊ��</param>
        /// <returns></returns>
        public IPrincipal CreatePrincipal(GenericTicketTokenContainer tokenContainer, ITicket ticket)
        {
            DeluxeIdentity identity = new DeluxeIdentity(tokenContainer, ticket);

            return new DeluxePrincipal(identity);
        }
        #endregion
    }
}
