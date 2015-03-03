using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Passport
{
    /// <summary>
    /// 票据中用户对象容器
    /// </summary>
    public interface ITicketTokenContainer<TToken> where TToken : ITicketToken
    {
        /// <summary>
        /// 用户
        /// </summary>
        TToken User
        {
            get;
        }

        /// <summary>
        /// 真实的用户
        /// </summary>
        TToken RealUser
        {
            get;
        }
    }
}
