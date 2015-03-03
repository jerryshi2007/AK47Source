using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Passport
{
    /// <summary>
    /// 票据中用户对象
    /// </summary>
    public interface ITicketToken
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        string ID
        {
            get;
        }

        /// <summary>
        /// 用户名称
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// 用户的显示名称
        /// </summary>
        string DisplayName
        {
            get;
        }
    }
}
