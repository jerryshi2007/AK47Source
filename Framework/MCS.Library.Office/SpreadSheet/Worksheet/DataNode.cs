using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public enum CellDataType
	{
		String,
		Number,
		DateTime,
		Boolean,
		Error
	}

	public class DataNode : NodeBase
	{
		private string innerText;

		internal bool OutputTypeAttribute { get; set; }

		public CellDataType Type { get; set; }
		public string Value { get; set; }

		public Decimal ToNumber()
		{
			return ToNumber(5);
		}

		public Decimal ToNumber(int precise)
		{
			Decimal result = 0;

			if (string.IsNullOrEmpty(Value) == false)
			{
				if (Decimal.TryParse(Value, out result))
					result = Math.Round(result, precise, MidpointRounding.AwayFromZero);
			}

			return result;
		}

		public string InnerText
		{
			get
			{
				string result = string.Empty;

				if (string.IsNullOrEmpty(this.innerText) == false)
					result = this.innerText;

				return result;
			}
		}

		public bool IsHtml { get; set; }

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = parent.OwnerDocument.CreateElement("ss:Data", Namespaces.spreadsheet);
			parent.AppendChild(node);

			if (OutputTypeAttribute)
				XmlHelper.AppendAttr(node, "Type", Type);

			if (IsHtml)
			{
				node.InnerXml = Value;

				if (node.FirstChild != null && node.FirstChild.Attributes != null)
					node.FirstChild.Attributes.RemoveAll();

				((XmlElement)node).SetAttribute("xmlns", Namespaces.html);
			}
			else
				node.InnerText = Value;

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			this.Type = XmlHelper.GetAttributeValue(node, "Type", CellDataType.String);

			if (HasNoTextChild(node))
				this.IsHtml = true;

			this.innerText = node.InnerText;

			if (string.IsNullOrEmpty(this.innerText))
				this.innerText = string.Empty;

			this.Value = node.InnerXml;
		}

		private static bool HasNoTextChild(XmlNode node)
		{
			bool result = false;
			XmlNode subNode = node.FirstChild;

			while (subNode != null && result == false)
			{
				if (subNode.NodeType != XmlNodeType.Text && subNode.NodeType != XmlNodeType.Comment)
				{
					result = true;
				}
				else
				{
					result = HasNoTextChild(subNode);
				}

				subNode = subNode.NextSibling;
			}

			return result;
		}
	}
}
