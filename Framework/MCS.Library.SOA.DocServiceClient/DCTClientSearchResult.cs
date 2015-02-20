using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DocServiceContract;

namespace MCS.Library.SOA.DocServiceClient
{
	public class DCTClientSearchResult : DCTSearchResult
	{
		private DCSClient client;

		public DCSClient Client
		{
			get { return client; }
			set { client = value; }
		}

		public DCTClientFile GetFile()
		{
			//return client.GetFile(this.
			return client.GetFile(this.Path);
		}

	}
}
