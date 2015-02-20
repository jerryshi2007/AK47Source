using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Descriptors
{
    public abstract class WfClientKeyedDescriptorCollectionConverterBase<TClient, TServer>
        where TClient: WfClientKeyedDescriptorBase
        where TServer: WfKeyedDescriptorBase
    {
        public virtual void ClientToServer(IEnumerable<TClient> cc, ICollection<TServer> sc)
        {
            WfClientKeyedDescriptorConverterBase<TClient, TServer> converter = GetItemConverter();

            sc.Clear();

            foreach (TClient c in cc)
            {
                TServer s = CreateServerItem();
                converter.ClientToServer(c, ref s);

                sc.Add(s);
            }
        }

        public virtual void ServerToClient(IEnumerable<TServer> sc, ICollection<TClient> cc)
        {
            WfClientKeyedDescriptorConverterBase<TClient, TServer> converter = GetItemConverter();

            cc.Clear();

            foreach (TServer s in sc)
            {
                TClient c = CreateClientItem();
                converter.ServerToClient(s, ref c);

                cc.Add(c);
            }
        }

        protected abstract TClient CreateClientItem();
        
        protected abstract TServer CreateServerItem();

        protected abstract WfClientKeyedDescriptorConverterBase<TClient, TServer> GetItemConverter();
    }
}
