using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;

namespace MCS.Library.WcfExtensions
{
	class WfWebScriptMetadataFomatter : IDispatchMessageFormatter
	{
		public void DeserializeRequest(Message message, object[] parameters)
		{
			if (parameters == null || parameters.Length == 0)
			{
				return;
			}
			parameters[0] = message;
		}

		public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
		{
			return (result as Message);
		}
	}
}
