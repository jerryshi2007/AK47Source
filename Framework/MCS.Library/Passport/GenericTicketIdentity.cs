using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Passport
{
    /// <summary>
    /// 通用的标识
    /// </summary>
    public class GenericTicketIdentity : GenericTicketTokenContainer, ITicketIdentity
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public GenericTicketIdentity()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public GenericTicketIdentity(ITicketToken user)
        {
            user.NullCheck("user");

            this.User = new GenericTicketToken(user);
            this.RealUser = new GenericTicketToken(user);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="realUser"></param>
        public GenericTicketIdentity(ITicketToken user, ITicketToken realUser)
        {
            user.NullCheck("user");
            realUser.NullCheck("realUser");

            this.User = new GenericTicketToken(user);
            this.RealUser = new GenericTicketToken(realUser);
        }

        /// <summary>
        /// 
        /// </summary>
        public string AuthenticationType
        {
            get
            {
                return "GenericTicketIdentity";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                string name = string.Empty;

                if (this.User != null)
                    name = this.User.Name;

                return name;
            }
        }

        /// <summary>
        /// 票据信息
        /// </summary>
        public ITicket Ticket
        {
            get;
            set;
        }
    }
}
