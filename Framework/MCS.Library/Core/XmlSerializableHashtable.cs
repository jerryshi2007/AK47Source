using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MCS.Library.Core
{
	/// <summary>
	/// 支持Xml序列化的哈希表
	/// </summary>
	[Serializable]
	[DebuggerDisplay("Count = {Count}")]
	[ComVisible(true)]
	[XmlRoot("XmlSerializableHashtable")]
	public class XmlSerializableHashtable : Hashtable, IXmlSerializable
	{
		/// <summary>
		/// 
		/// </summary>
		public XmlSerializableHashtable()
			: base()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		public XmlSerializableHashtable(IDictionary d)
			: base(d)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="equalityComparer"></param>
		public XmlSerializableHashtable(IEqualityComparer equalityComparer)
			: base(equalityComparer)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="capacity"></param>
		public XmlSerializableHashtable(int capacity)
			: base(capacity)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <param name="loadFactor"></param>
		public XmlSerializableHashtable(IDictionary d, float loadFactor)
			: base(d, loadFactor)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <param name="equalityComparer"></param>
		public XmlSerializableHashtable(IDictionary d, IEqualityComparer equalityComparer)
			: base(d, equalityComparer)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="capacity"></param>
		/// <param name="loadFactor"></param>
		public XmlSerializableHashtable(int capacity, float loadFactor)
			: base(capacity, loadFactor)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="capacity"></param>
		/// <param name="equalityComparer"></param>
		public XmlSerializableHashtable(int capacity, IEqualityComparer equalityComparer)
			: base(capacity, equalityComparer)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected XmlSerializableHashtable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <param name="loadFactor"></param>
		/// <param name="equalityComparer"></param>
		public XmlSerializableHashtable(IDictionary d, float loadFactor, IEqualityComparer equalityComparer)
			: base(d, loadFactor, equalityComparer)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="capacity"></param>
		/// <param name="loadFactor"></param>
		/// <param name="equalityComparer"></param>
		public XmlSerializableHashtable(int capacity, float loadFactor, IEqualityComparer equalityComparer)
			: base(capacity, loadFactor, equalityComparer)
		{
		}

		#region IXmlSerializable Members

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		public void ReadXml(XmlReader reader)
		{
			var keySerializer = new XmlSerializer(typeof(object));
			var valueSerializer = new XmlSerializer(typeof(object));

			bool wasEmpty = reader.IsEmptyElement;

			reader.Read();

			if (wasEmpty)
				return;

			while (reader.NodeType != XmlNodeType.EndElement)
			{
				reader.ReadStartElement("item");
				reader.ReadStartElement("key");

				object key = keySerializer.Deserialize(reader);
				reader.ReadEndElement();
				reader.ReadStartElement("value");

				object value = (object)valueSerializer.Deserialize(reader);
				reader.ReadEndElement();

				Add(key, value);

				reader.ReadEndElement();
				reader.MoveToContent();
			}

			reader.ReadEndElement();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public void WriteXml(XmlWriter writer)
		{
			var keySerializer = new XmlSerializer(typeof(object));
			var valueSerializer = new XmlSerializer(typeof(object));

			foreach (var key in this.Keys)
			{
				writer.WriteStartElement("item");
				writer.WriteStartElement("key");

				keySerializer.Serialize(writer, key);

				writer.WriteEndElement();
				writer.WriteStartElement("value");

				object value = this[key];
				valueSerializer.Serialize(writer, value);

				writer.WriteEndElement();
				writer.WriteEndElement();
			}
		}

		#endregion
	}
}
