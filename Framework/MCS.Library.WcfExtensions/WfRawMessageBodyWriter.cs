using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.Xml;

namespace MCS.Library.WcfExtensions
{
	public class WfRawMessageBodyWriter : BodyWriter
	{
		private readonly byte[] _Content;

		public WfRawMessageBodyWriter(byte[] content)
			: base(true)
		{
			this._Content = content;
		}

		protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
		{
			writer.WriteStartElement("Binary");
			writer.WriteBase64(_Content, 0, _Content.Length);
			writer.WriteEndElement();
		}
	}
}
