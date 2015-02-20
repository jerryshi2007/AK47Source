using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public interface INode
	{
		void FromXmlNode(XmlNode node);
		XmlNode AppendXmlNode(XmlNode parent);
	}

	/// <summary>
	/// Node
	/// </summary>
	public abstract class NodeBase : INode
	{
		public abstract void FromXmlNode(XmlNode node);

		public abstract XmlNode AppendXmlNode(XmlNode parent);
	}
}
