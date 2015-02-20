using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Passport
{
    /// <summary>
    /// 带票据的Identity
    /// </summary>
    public interface ITicketIdentity : IIdentity
    {
        /// <summary>
        /// Identity中包含的票据
        /// </summary>
        ITicket Ticket
        {
            get;
        }
    }
}
