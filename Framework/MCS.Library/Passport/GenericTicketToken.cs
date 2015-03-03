using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Passport
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class GenericTicketToken : ITicketToken
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public GenericTicketToken()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="token"></param>
        public GenericTicketToken(ITicketToken token)
        {
            token.NullCheck("token");

            this.ID = token.ID;
            this.Name = token.Name;
            this.DisplayName = token.DisplayName;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ID
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string DisplayName
        {
            get;
            set;
        }
    }
}
