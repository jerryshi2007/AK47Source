using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;

namespace MCS.Library.SOA.DataObjects.Schemas.Client.ServiceBroker
{
	[Serializable]
	public class SchemaServiceUpdatableClientSoapHeader : SoapHeader
	{
		private bool _WithLock;

		public bool WithLock
		{
			get
			{
				return this._WithLock;
			}
			set
			{
				this._WithLock = value;
			}
		}
	}
}
