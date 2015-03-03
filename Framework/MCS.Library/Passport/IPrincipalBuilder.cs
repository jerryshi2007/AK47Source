using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Passport
{
    /// <summary>
    /// Principal的构造器
    /// </summary>
    public interface IPrincipalBuilder
    {
        /// <summary>
        /// 创建 <see cref="IPrincipal"/> 对象
        /// </summary>
        /// <param name="userID">用户标识</param>
        /// <param name="ticket">票据</param>
        /// <returns></returns>
        IPrincipal CreatePrincipal(string userID, ITicket ticket);

        /// <summary>
        /// 创建 <see cref="IPrincipal"/> 对象
        /// </summary>
        /// <param name="tokenContainer">token的容器</param>
        /// <param name="ticket">票据</param>
        /// <returns></returns>
        IPrincipal CreatePrincipal(GenericTicketTokenContainer tokenContainer, ITicket ticket);
    }
}
