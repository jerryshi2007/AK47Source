using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Passport
{
    /// <summary>
    /// 获取带票据的Identity
    /// </summary>
    public interface IGenericTokenPrincipal : IPrincipal
    {
        /// <summary>
        /// 获取票据中的Token容器
        /// </summary>
        /// <returns></returns>
        GenericTicketTokenContainer GetGenericTicketTokenContainer();
    }
}
