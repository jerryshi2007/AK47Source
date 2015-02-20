using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DocServiceContract;
using System.Collections;
using MCS.Library.Core;

namespace MCS.Library.SOA.DocServiceClient
{
	public class WordTemplate
	{
		private DCSClient client;

		public DCSClient Client
		{
			get { return client; }
			set { client = value; }
		}

		public WordTemplate()
		{
			wordDataObject = new DCTWordDataObject();
		}

		private string uri;

		public string Uri
		{
			get { return uri; }
			set { uri = value; }
		}

		DCTWordDataObject wordDataObject;

		public WordTemplate Load(object obj)
		{
			wordDataObject.Load(obj);
			return this;
		}

		public WordTemplate Load(ICollection objs, string dataName)
		{
			wordDataObject.LoadCollection(objs, dataName);
			return this;
		}

		public byte[] GenerateDocument()
		{
			byte[] result = null;
			ServiceProxy.SingleCall<IDCSDocumentBuilderService>(client.Binding, client.WordBuilderEndpointAddress, client.UserBehavior, action =>
		   {
			   result = action.DCMBuildDocument(uri, wordDataObject);
		   });
			return result;
		}

	}
}
