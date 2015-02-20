using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace MCS.Library.Office.SpreadSheet
{
	public class WorkbookNode : NodeBase
	{
		private DocumentPropertiesNode _DocumentProperties = new DocumentPropertiesNode();
		private WorksheetNodeCollection _Worksheets = new WorksheetNodeCollection();
		private StyleNodeCollection _Styles = new StyleNodeCollection();
		private NamedRangeCollection _Names = new NamedRangeCollection();
		private ExcelWorkbookNode _ExcelWorkbook = null;

		public ExcelWorkbookNode ExcelWorkbook
		{
			get
			{
				if (_ExcelWorkbook == null)
					_ExcelWorkbook = new ExcelWorkbookNode();

				return _ExcelWorkbook;
			}
			set
			{
				_ExcelWorkbook = value;
			}
		}

		public NamedRangeCollection Names
		{
			get
			{
				return _Names;
			}
		}

		public StyleNodeCollection Styles
		{
			get
			{
				return this._Styles;
			}
		}

		public WorksheetNodeCollection Worksheets
		{
			get
			{
				return this._Worksheets;
			}
		}

		public DocumentPropertiesNode DocumentProperties
		{
			get
			{
				return this._DocumentProperties;
			}
		}

		public void Save(TextWriter writer)
		{
			writer.Write(GenerateXmlDocument().OuterXml);
		}

		public void Save(string file)
		{
			GenerateXmlDocument().Save(file);
		}

		public void Save(Stream stream)
		{
			GenerateXmlDocument().Save(stream);
		}

		public void Load(string file)
		{
			using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				Load(stream);
			}
		}

		public void Load(Stream stream)
		{
			XmlDocument xmlDoc = new XmlDocument();

			xmlDoc.Load(stream);

			ParseXmlDocument(xmlDoc);
		}

		public void LoadXml(string xml)
		{
			XmlDocument xmlDoc = new XmlDocument();

			xmlDoc.LoadXml(xml);

			ParseXmlDocument(xmlDoc);
		}

		public override XmlNode AppendXmlNode(XmlNode parent)
		{
			XmlDocument xmlDoc = (XmlDocument)parent;

			XmlProcessingInstruction instruction = xmlDoc.CreateProcessingInstruction("xml", "version=\"1.0\"");

			xmlDoc.AppendChild(instruction);

			instruction = xmlDoc.CreateProcessingInstruction("mso-application", "progid=\"Excel.Sheet\"");

			xmlDoc.AppendChild(instruction);

			XmlElement node = (XmlElement)xmlDoc.CreateElement("Workbook", Namespaces.spreadsheet);

			node.SetAttribute("xmlns:o", Namespaces.o);
			node.SetAttribute("xmlns:x", Namespaces.x);
			node.SetAttribute("xmlns:ss", Namespaces.ss);
			node.SetAttribute("xmlns:html", Namespaces.html);

			parent.AppendChild(node);

			DocumentProperties.AppendXmlNode(node);

			if (_ExcelWorkbook != null)
				_ExcelWorkbook.AppendXmlNode(node);

			Styles.AppendXmlNode(node);
			Names.AppendXmlNode(node);
			Worksheets.AppendXmlNode(node);

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			DocumentProperties.FromXmlNode(node.SelectSingleNode("o:DocumentProperties", Namespaces.GetNamespaceManager()));
			ExcelWorkbook.FromXmlNode(node.SelectSingleNode("x:ExcelWorkbook", Namespaces.GetNamespaceManager()));
			Styles.FromXmlNode(node.SelectSingleNode("ss:Styles", Namespaces.GetNamespaceManager()));
			Worksheets.FromXmlNode(node);

			XmlNode namesNodes = node.SelectSingleNode("ss:Names", Namespaces.GetNamespaceManager());

			if (namesNodes != null)
				Names.FromXmlNode(namesNodes);
		}

		private XmlDocument GenerateXmlDocument()
		{
			XmlDocument xmlDoc = new XmlDocument();

			xmlDoc.PreserveWhitespace = true;
			xmlDoc.AppendChild(AppendXmlNode(xmlDoc));

			return xmlDoc;
		}

		private void ParseXmlDocument(XmlDocument xmlDoc)
		{
			Namespaces.InitNamespaceManager(xmlDoc.NameTable);

			try
			{
				this.FromXmlNode(xmlDoc.DocumentElement);
			}
			finally
			{
				Namespaces.RemoveNamespaceManager();
			}
		}
	}
}
