using MCS.Library.Core;
using MCS.Web.Library.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MCS.Library.WcfExtensions
{
    public class WfClientErrorInspector : IClientMessageInspector
    {
        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            XmlDocument document = null;

            Message relayMessage = WcfUtils.SimpleCloneMessage(reply, out document);

            string jsonStr = WcfUtils.GetMessageRawContent(document);

            if (jsonStr.IsNotEmpty())
            {
                if (jsonStr.IndexOf("MCS.Library.WcfExtensions.WfErrorDTO") >= 0)
                {
                    WfErrorDTO error = JSONSerializerExecute.Deserialize<WfErrorDTO>(jsonStr);

                    throw new WfClientChannelException(error.Message) { Detail = error.Description };
                }
            }

            reply = relayMessage;
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            return null;
        }
    }
}
