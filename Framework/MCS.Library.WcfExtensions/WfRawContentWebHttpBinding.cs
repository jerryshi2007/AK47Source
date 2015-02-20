using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace MCS.Library.WcfExtensions
{
	public class WfRawContentWebHttpBinding : WebHttpBinding
	{
		public WfRawContentWebHttpBinding()
			: base()
		{
			this.ContentTypeMapper = new WfRawWebContentTypeMapper();
            this.MaxReceivedMessageSize = int.MaxValue;
		}
	}
}
