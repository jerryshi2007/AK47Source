using System;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using MCS.Library.Core;
using System.Text;
using System.Xml.Linq;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class AppTheme : IPersistable
	{
		private WorkBook _WorkBook;

		public AppTheme(WorkBook workBook)
		{
			this._WorkBook = workBook;
		}

		private string _Name;
		protected internal string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				_Name = value;
			}
		}

		private ThemeObjectDefaults _ObjectDefaults;
		public ThemeObjectDefaults ObjectDefaults
		{
			get
			{
				if (this._ObjectDefaults == null)
				{
					this._ObjectDefaults = new ThemeObjectDefaults(this);
				}
				return this._ObjectDefaults;
			}
		}

		private ThemeElements _ThemeElements;
		public ThemeElements ThemeElements
		{
			get
			{
				if (this._ThemeElements == null)
					this._ThemeElements = new ThemeElements(this);

				return this._ThemeElements;
			}
		}

		private ThemeExtraClrSchemeLst _ExtraClrSchemeLst;
		public ThemeExtraClrSchemeLst ExtraClrSchemeLst
		{
			get
			{
				if (this._ExtraClrSchemeLst == null)
					this._ExtraClrSchemeLst = new ThemeExtraClrSchemeLst(this);
				return this._ExtraClrSchemeLst;
			}
		}

		private XDocument _Theme;
		internal XDocument Theme
		{
			get { return this._Theme; }
			set { this._Theme = value; }
		}

		void IPersistable.Save(ExcelSaveContext context)
		{
			if (this._Theme != null)
			{
				PackagePart themePart = context.Package.CreatePart(ExcelCommon.Uri_Theme, ExcelCommon.ContentType_Theme, this._WorkBook.Compression);
				context.Package.GetPart(ExcelCommon.Uri_Workbook).CreateRelationship(PackUriHelper.GetRelativeUri(ExcelCommon.Uri_Workbook, ExcelCommon.Uri_Theme), TargetMode.Internal, ExcelCommon.Schema_Relationships + "/theme");

				using (StreamWriter streamCore = new StreamWriter(themePart.GetStream(FileMode.Create, FileAccess.Write)))
				{
					this._Theme.Save(streamCore);
				}

			  /*	using (XmlTextWriter writer = new XmlTextWriter(themePart.GetStream(FileMode.Create, FileAccess.Write), Encoding.UTF8))
				{
					context.Writer.WriteTheme(this, writer);
				} */
			}

			//PackagePart themePart = context.Package.GetPart(ExcelCommon.Uri_Workbook).CreatePart(ExcelCommon.Uri_Theme, ExcelCommon.ContentType_Theme, this._WorkBook.Compression);
			//using (XmlTextWriter writer = new XmlTextWriter(themePart.GetStream(FileMode.Create, FileAccess.Write), Encoding.UTF8))
			//{
			//    context.Writer.WriteTheme(this, writer);
			//}
		}

		void IPersistable.Load(ExcelLoadContext context)
		{
			if (context.Package.PartExists(ExcelCommon.Uri_Theme))
			{
				this._Theme = context.Package.GetXDocumentFromUri(ExcelCommon.Uri_Theme);
				//var root = context.Package.GetXElementFromUri(ExcelCommon.Uri_Theme);
				//context.Reader.ReadTheme(root);
			}
		}
	}
}
