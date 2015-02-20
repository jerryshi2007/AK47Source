using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Core;
using System.Collections.ObjectModel;

namespace MCS.Library.Office.SpreadSheet
{
	public struct CellLocation
	{
		public string Name
		{
			get;
			set;
		}

		public string WorksheetName
		{
			get;
			set;
		}

		public int Row
		{
			get;
			set;
		}

		public int Column
		{
			get;
			set;
		}
	}

	public class NamedRangeNode : NodeBase
	{
		public string Name { get; set; }
		public string RefersTo { get; set; }
		public bool Hidden { get; set; }

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = XmlHelper.AppendNodeWithNamespace(parent, "NamedRange", Namespaces.spreadsheet);

			XmlHelper.AppendAttr(node, "Name", Name);
			XmlHelper.AppendAttr(node, "RefersTo", RefersTo);
			XmlHelper.AppendNotDefaultAttr(node, "Hidden", DataConverter.ChangeType<bool, int>(Hidden));

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			this.Name = XmlHelper.GetAttributeText(node, "Name");
			this.RefersTo = XmlHelper.GetAttributeText(node, "RefersTo");
			this.Hidden = XmlHelper.GetAttributeValue(node, "Hidden", false);
		}

		public CellLocation ParseReferTo()
		{
			CellLocation location = new CellLocation();

			location.Name = this.Name;

			if (string.IsNullOrEmpty(this.RefersTo) == false)
			{
				string[] parts = this.RefersTo.Split('!');

				if (parts.Length > 1)
				{
					location.WorksheetName = parts[0].TrimStart('=').Trim('\'');

					string[] locationParts = parts[1].Split('C');

					location.Row = int.Parse(locationParts[0].TrimStart('R'));
					location.Column = int.Parse(locationParts[1]);
				}
			}

			return location;
		}
	}

	public class NamedRangeCollection : KeyedCollection<string, NamedRangeNode>, INode
	{
		protected override string GetKeyForItem(NamedRangeNode item)
		{
			item.NullCheck("item");

			(item.Name.IndexOf(" ") == -1).FalseThrow("NamedRange的Name属性\"{0}\"中不能有空格", item.Name);

			return item.Name;
		}

		public NamedLocationCollection ToLocations()
		{
			NamedLocationCollection result = new NamedLocationCollection();

			foreach (NamedRangeNode nr in this)
				result.Add(nr.ParseReferTo());

			return result;
		}

		#region INode Members

		public void FromXmlNode(XmlNode node)
		{
			XmlNodeList nodes = node.SelectNodes("ss:NamedRange", Namespaces.GetNamespaceManager());

			foreach (XmlNode namedRangeNode in nodes)
			{
				NamedRangeNode namedRange = new NamedRangeNode();

				namedRange.FromXmlNode(namedRangeNode);
				this.Add(namedRange);
			}
		}

		public XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlNode node = null;

			if (this.Count > 0)
			{
				node = XmlHelper.AppendNode(parent, "Names");

				foreach (NamedRangeNode style in this)
					style.AppendXmlNode(node);
			}

			return node;
		}

		#endregion
	}

	public class NamedLocationCollection : KeyedCollection<string, CellLocation>
	{
		protected override string GetKeyForItem(CellLocation item)
		{
			return item.Name;
		}

		public void SortByColumn()
		{
			CellLocation[] locations = new CellLocation[this.Count];

			this.CopyTo(locations, 0);

			Array.Sort(locations, (x, y) => x.Column - y.Column);

			this.Clear();
			this.CopyFrom(locations);
		}

		public void SortByRow()
		{
			CellLocation[] locations = new CellLocation[this.Count];

			this.CopyTo(locations, 0);

			Array.Sort(locations, (x, y) => x.Row - y.Row);

			this.Clear();
			this.CopyFrom(locations);
		}

		public void CopyFrom(IEnumerable<CellLocation> locations)
		{
			foreach (CellLocation location in locations)
				this.Add(location);
		}
	}
}
