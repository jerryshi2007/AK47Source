using MCS.Web.Library.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace MCS.Library.WcfExtensions
{
	internal class WfErrorHandler : IErrorHandler
	{
		public bool HandleError(Exception error)
		{
			return true;
		}

		public void ProvideFault(Exception error, System.ServiceModel.Channels.MessageVersion version, ref System.ServiceModel.Channels.Message fault)
		{
			string errName = string.Format("调用 {0} 时异常", OperationContext.Current.IncomingMessageProperties["HttpOperationName"]);

			WfErrorDTO errorDTO = new WfErrorDTO()
			{
				Number = 100,
				Name = errName,
				Message = error.Message,
				Description = error.StackTrace
			};

			string jsonResult = JSONSerializerExecute.SerializeWithType(errorDTO);

			fault = WcfUtils.CreateJsonFormatReplyMessage(version, null, jsonResult);
            //fault = CreateJsonFaultMessage(version, jsonResult);
        }

        //public static Message CreateJsonFaultMessage(MessageVersion msgVersion, string msgContent)
        //{
        //    var bodyBytes = Encoding.UTF8.GetBytes(msgContent);

        //    MessageFault fault = MessageFault.CreateFault(new FaultCode("Server"), new FaultReason("Error"));

        //    Message replyMessage = Message.CreateMessage(msgVersion, fault, msgContent);
        //    replyMessage.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));

        //    HttpResponseMessageProperty respProp = new HttpResponseMessageProperty();
        //    respProp.Headers[HttpResponseHeader.ContentType] = WcfUtils.JSON_CONTENT_TYPE_STR;
        //    replyMessage.Properties.Add(HttpResponseMessageProperty.Name, respProp);

        //    return replyMessage;
        //}
	}
}
