using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Passport
{
    /// <summary>
    /// 通用的票据Token的容器
    /// </summary>
    [Serializable]
    public class GenericTicketTokenContainer : ITicketTokenContainer<GenericTicketToken>
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public GenericTicketTokenContainer()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public GenericTicketTokenContainer(ITicketToken user)
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
        public GenericTicketTokenContainer(ITicketToken user, ITicketToken realUser)
        {
            user.NullCheck("user");
            realUser.NullCheck("realUser");

            this.User = new GenericTicketToken(user);
            this.RealUser = new GenericTicketToken(realUser);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TToken"></typeparam>
        /// <param name="container"></param>
        public void CopyFrom<TToken>(ITicketTokenContainer<TToken> container) where TToken : ITicketToken
        {
            container.NullCheck("container");

            this.User = new GenericTicketToken(container.User);
            this.RealUser = new GenericTicketToken(container.RealUser);
        }

        /// <summary>
        /// 
        /// </summary>
        public GenericTicketToken User
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public GenericTicketToken RealUser
        {
            get;
            set;
        }
    }
}
