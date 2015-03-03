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
    /// 通用的带Token和票据的Principal
    /// </summary>
    public class GenericTicketPrincipal : IGenericTokenPrincipal
    {
        private GenericTicketIdentity _Identity = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identity"></param>
        public GenericTicketPrincipal(GenericTicketIdentity identity)
        {
            identity.NullCheck("identity");

            this._Identity = identity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public GenericTicketPrincipal(ITicketToken user)
        {
            user.NullCheck("user");

            this._Identity = new GenericTicketIdentity(user);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="realUser"></param>
        public GenericTicketPrincipal(ITicketToken user, ITicketToken realUser)
        {
            user.NullCheck("user");
            realUser.NullCheck("realUser");

            this._Identity = new GenericTicketIdentity(user, realUser);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GenericTicketTokenContainer GetGenericTicketTokenContainer()
        {
            return this._Identity;
        }

        /// <summary>
        /// 
        /// </summary>
        public IIdentity Identity
        {
            get
            {
                return this._Identity;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public virtual bool IsInRole(string role)
        {
            return false;
        }
    }
}
