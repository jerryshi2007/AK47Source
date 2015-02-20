using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO.Packaging;
using System.Globalization;
using System.IO;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class FileProperties : IPersistable
	{
		public FileProperties()
		{

		}

		/// <summary>
		/// 获取或设置标题
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// 获取或设置文件的主题属性
		/// </summary>
		public string Subject { get; set; }

		/// <summary>
		///获取或设置文档的作者
		/// </summary>
		public string Author { get; set; }

		/// <summary>
		/// 获取或设置评论
		/// </summary>
		public string Comments { get; set; }

		/// <summary>
		/// 关键字
		/// </summary>
		public string Keywords { get; set; }

		/// <summary>
		/// 获取设置文档最后修改
		/// </summary>
		public string LastModifiedBy { get; set; }

		/// <summary>
		///获取设置文档最后打印
		/// </summary>
		public string LastPrinted { get; set; }

		/// <summary>
		/// 获取设置文档类别
		/// </summary>
		public string Category { get; set; }

		/// <summary>
		/// 状态
		/// </summary>
		public string Status { get; set; }

		private string _Application;
		/// <summary>
		/// 应用程序属性
		/// </summary>
		public string Application
		{
			get { return this._Application; }
			internal set { this._Application = value; }
		}

		private Uri _HyperlinkBase;
		/// <summary>
		/// 获取设置文档的超链接
		/// </summary>
		public Uri HyperlinkBase
		{
			get { return this._HyperlinkBase; }
			set { this._HyperlinkBase = value; }
		}

		private string _AppVersion;
		/// <summary>
		/// 获取应用程序版本
		/// </summary>
		public string AppVersion
		{
			get { return this._AppVersion; }
			internal set { _AppVersion = value; }
		}

		/// <summary>
		///  获取设置公司
		/// </summary>
		public string Company { get; set; }

		/// <summary>
		///  获取设置文件管理者
		/// </summary>
		public string Manager { get; set; }

		#region Custom Properties
		private XmlDocument _XmlPropertiesCustom;
		/// <summary>
		/// 保存文件的自定义属性的访问
		/// </summary>
		public XmlDocument CustomPropertiesXml
		{
			get
			{
				return this._XmlPropertiesCustom;
			}
			set
			{
				this._XmlPropertiesCustom = value;
			}
		}
		#endregion

		#region Get and Set Custom Properties
		public object GetCustomPropertyValue(string propertyName)
		{
			string searchString = string.Format("//ctp:Properties/ctp:property[@name='{0}']", propertyName);
			XmlElement node = CustomPropertiesXml.SelectSingleNode(searchString, ExcelCommon.DefaultNameSpaceManager) as XmlElement;
			if (node != null)
			{
				string value = node.LastChild.InnerText;
				switch (node.LastChild.LocalName)
				{
					case "filetime":
						DateTime dt;
						if (DateTime.TryParse(value, out dt))
						{
							return dt;
						}
						else
						{
							return null;
						}
					case "i4":
						int i;
						if (int.TryParse(value, out i))
						{
							return i;
						}
						else
						{
							return null;
						}
					case "r8":
						double d;
						if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
						{
							return d;
						}
						else
						{
							return null;
						}
					case "bool":
						if (value == "true")
						{
							return true;
						}
						else if (value == "false")
						{
							return false;
						}
						else
						{
							return null;
						}
					default:
						return value;
				}
			}
			else
			{
				return null;
			}
		}


		/// <summary>
		/// 设置自定义属性
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="propValue"></param>
		public void SetCustomPropertyValue(string propertyName, object value)
		{
			XmlNode allProps = CustomPropertiesXml.SelectSingleNode(@"ctp:Properties", ExcelCommon.DefaultNameSpaceManager);

			var prop = string.Format("//ctp:Properties/ctp:property[@name='{0}']", propertyName);
			XmlElement node = CustomPropertiesXml.SelectSingleNode(prop, ExcelCommon.DefaultNameSpaceManager) as XmlElement;
			if (node == null)
			{
				int pid;
				var MaxNode = CustomPropertiesXml.SelectSingleNode("//ctp:Properties/ctp:property[not(@pid <= preceding-sibling::ctp:property/@pid) and not(@pid <= following-sibling::ctp:property/@pid)]", ExcelCommon.DefaultNameSpaceManager);
				if (MaxNode == null)
				{
					pid = 2;
				}
				else
				{
					if (!int.TryParse(MaxNode.Attributes["pid"].Value, out pid))
					{
						pid = 2;
					}
					pid++;
				}
				node = CustomPropertiesXml.CreateElement("property", ExcelCommon.Schema_Custom);
				node.SetAttribute("fmtid", "{D5CDD505-2E9C-105B-9896-08002B2CF9AE}");
				node.SetAttribute("pid", pid.ToString());
				node.SetAttribute("name", propertyName);

				allProps.AppendChild(node);
			}
			else
			{
				while (node.ChildNodes.Count > 0)
				{
					node.RemoveChild(node.ChildNodes[0]);
				}
			}
			XmlElement valueElem;
			if (value is bool)
			{
				valueElem = CustomPropertiesXml.CreateElement("vt", "bool", ExcelCommon.Schema_Vt);
				valueElem.InnerText = value.ToString().ToLower();
			}
			else if (value is DateTime)
			{
				valueElem = CustomPropertiesXml.CreateElement("vt", "filetime", ExcelCommon.Schema_Vt);
				valueElem.InnerText = ((DateTime)value).AddHours(-1).ToString("yyyy-MM-ddTHH:mm:ssZ");
			}
			else if (value is short || value is int)
			{
				valueElem = CustomPropertiesXml.CreateElement("vt", "i4", ExcelCommon.Schema_Vt);
				valueElem.InnerText = value.ToString();
			}
			else if (value is double || value is decimal || value is float || value is long)
			{
				valueElem = CustomPropertiesXml.CreateElement("vt", "r8", ExcelCommon.Schema_Vt);
				if (value is double)
				{
					valueElem.InnerText = ((double)value).ToString(CultureInfo.InvariantCulture);
				}
				else if (value is float)
				{
					valueElem.InnerText = ((float)value).ToString(CultureInfo.InvariantCulture);
				}
				else if (value is decimal)
				{
					valueElem.InnerText = ((decimal)value).ToString(CultureInfo.InvariantCulture);
				}
				else
				{
					valueElem.InnerText = value.ToString();
				}
			}
			else
			{
				valueElem = CustomPropertiesXml.CreateElement("vt", "lpwstr", ExcelCommon.Schema_Vt);
				valueElem.InnerText = value.ToString();
			}
			node.AppendChild(valueElem);
		}
		#endregion

		void IPersistable.Save(ExcelSaveContext context)
		{
			context.LinqWriter.WriteCore();
			context.LinqWriter.WriteApp();

			if (this._XmlPropertiesCustom != null)
			{
				context.LinqWriter.WriteCustom();
			}
		}

		void IPersistable.Load(ExcelLoadContext context)
		{
			#region "LoadCore"
			if (context.Package.PartExists(ExcelCommon.Uri_PropertiesCore))
			{
				var root = context.Package.GetXElementFromUri(ExcelCommon.Uri_PropertiesCore);
				context.Reader.ReadCore(root);
			}
			#endregion

			if (context.Package.PartExists(ExcelCommon.Uri_PropertiesExtended))
			{
				var root = context.Package.GetXElementFromUri(ExcelCommon.Uri_PropertiesExtended);
				context.Reader.ReadApp(root);
			}

			if (context.Package.PartExists(ExcelCommon.Uri_PropertiesCustom))
			{
				this._XmlPropertiesCustom = context.Package.GetXmlFromUri(ExcelCommon.Uri_PropertiesCustom);
			}
		}
	}
}
