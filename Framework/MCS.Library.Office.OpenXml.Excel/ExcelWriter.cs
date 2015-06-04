using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Globalization;
using MCS.Library.Core;
using System.Security;
using System.IO.Packaging;
using System.Xml;
using System.Security.Cryptography;
using System.Drawing;
using System.Drawing.Imaging;

namespace MCS.Library.Office.OpenXml.Excel
{
	internal class ExcelWriter
	{
		public WorkBook Workbook { get; set; }
		public ExcelSaveContext Context { get; set; }

		#region workbook.xml
		/// <summary>
		/// FileName:workbook.xml 
		/// <para>NodePath:workbook</para>
		/// </summary>
		public void WriteWorkBook()
		{
			PackagePart partWorkbook = this.Context.Package.CreatePart(ExcelCommon.Uri_Workbook,
				ExcelCommon.ContentType_WorkBook, this.Workbook.Compression);

			XElement xmlWorkBook = new XElement(ExcelCommon.Schema_WorkBook_Main + "workbook");
			xmlWorkBook.Add(new XAttribute(XNamespace.Xmlns + "r", ExcelCommon.Schema_Relationships));
			xmlWorkBook.Add(WriteWorkBook_fileVersion());
			if (this.Workbook._Protection != null)
				xmlWorkBook.Add(WriteWorkBook_workbookProtection());

			if (this.Workbook._Properties != null)
				if (this.Workbook._Properties.Attributes.Count > 0)
					xmlWorkBook.Add(WriteAttributes(this.Workbook._Properties));

			xmlWorkBook.Add(WriteWorkBook_bookViews());
			xmlWorkBook.Add(WriteWorkBook_sheets(partWorkbook));
			xmlWorkBook.Add(WriteWorkBook_definedNames());

			XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), xmlWorkBook);
			using (Stream stream = partWorkbook.GetStream(FileMode.Create, FileAccess.Write))
			{
				doc.Save(stream);
				stream.Flush();
			}
		}

		private XElement WriteWorkBook_workbookProtection()
		{
			return WriteAttributes(this.Workbook._Protection);
		}

		/// <summary>
		/// FileName:workbook.xml 
		/// <para>NodePath:workbook/fileVersion</para>
		/// </summary>
		private XElement WriteWorkBook_fileVersion()
		{
			return WriteAttributes(this.Workbook.FileVersion);
		}

		/// <summary>
		/// FileName:workbook.xml 
		/// <para>NodePath:workbook/bookViews</para>
		/// </summary>
		private XElement WriteWorkBook_bookViews()
		{
			XElement bookViews = new XElement(ExcelCommon.Schema_WorkBook_Main + "bookViews");

			if (this.Workbook.Views.Count > 0)
			{
				foreach (var view in this.Workbook.Views)
				{
					bookViews.Add(WriteAttributes(view));
				}
			}
			else
			{
				bookViews.Add(new XElement(ExcelCommon.Schema_WorkBook_Main + "workbookView"));
			}

			return bookViews;
		}

		/// <summary>
		/// FileName:workbook.xml 
		/// <para>NodePath:workbook/sheets</para>
		/// </summary>
		private XElement WriteWorkBook_sheets(PackagePart partWorkbook)
		{
			XElement sheets = new XElement(ExcelCommon.Schema_WorkBook_Main + "sheets");

			int sheetID = 1;
			foreach (WorkSheet sheet in this.Workbook.Sheets)
			{
				foreach (var definedName in sheet.Names)
				{
					this.Context.DefinedNames.Add(definedName);
				}

				XElement sheetElement = new XElement(ExcelCommon.Schema_WorkBook_Main + "sheet");
				sheetElement.Add(new XAttribute("name", sheet.Name));
				sheetElement.Add(new XAttribute("sheetId", sheetID));

				sheet.SheetUri = new Uri("/xl/worksheets/sheet" + sheetID + ".xml", UriKind.Relative);
				sheet.RelationshipID = partWorkbook.CreateRelationship(PackUriHelper.GetRelativeUri(ExcelCommon.Uri_Workbook,
					sheet.SheetUri), TargetMode.Internal, ExcelCommon.Schema_Relationships + "/worksheet").Id;

				XName name = XName.Get("id", ExcelCommon.Schema_Relationships);
				sheetElement.Add(new XAttribute(name, sheet.RelationshipID));

				#region "Hidden"
				if (sheet.Hidden == ExcelWorksheetHidden.Hidden)
				{
					sheetElement.Add(new XAttribute("state", "hidden"));
				}
				else if (sheet.Hidden == ExcelWorksheetHidden.VeryHidden)
				{
					sheetElement.Add(new XAttribute("state", "veryHidden"));
				}
				#endregion

				sheets.Add(sheetElement);
				sheetID++;
			}

			return sheets;
		}

		/// <summary>
		/// FileName:workbook.xml 
		/// <para>NodePath:workbook/definedNames</para>
		/// </summary>
		/// <param name="calcPrRoot"></param>
		private XElement WriteWorkBook_definedNames()
		{
			if (this.Context.DefinedNames.Count == 0)
				return null;

			XElement definedNames = new XElement(ExcelCommon.Schema_WorkBook_Main + "definedNames");

			foreach (var name in this.Context.DefinedNames)
			{
				XElement definedName = new XElement(ExcelCommon.Schema_WorkBook_Main + "definedName");
				definedName.Add(new XAttribute("name", name.Name));

				if (name.IsNameHidden)
				{
					definedName.Add(new XAttribute("hidden", "1"));
				}
				if (name.NameComment.IsNotEmpty())
				{
					definedName.Add(new XAttribute("comment", name.NameComment));
				}
				if (name.LocalSheetId >= 0)
				{
					definedName.Add(new XAttribute("localSheetId", name.LocalSheetId));
				}
				definedName.Value = name.Address.ToDefinedNameAddress();

				definedNames.Add(definedName);
			}

			this.Context.ResetDefinedNames();
			return definedNames;
		}
		#endregion

		#region sheet.xml
		/// <summary>
		/// FileName:sheet.xml
		/// <para>NodePath:worksheet</para>
		/// </summary>
		public void WriteWorkSheet(WorkSheet sheet)
		{
			PackagePart worksheetPart = this.Context.Package.CreatePart(sheet.SheetUri,
				ExcelCommon.ContentType_WorkSheet, sheet.WorkBook.Compression);
			//存储当前工作表中所有单元评论
			List<Comment> sheetComments = new List<Comment>();
			List<Cell> sheetHyperLink = new List<Cell>();

			XElement xmlWorkSheet = new XElement(ExcelCommon.Schema_WorkBook_Main + "worksheet");
			xmlWorkSheet.Add(new XAttribute(XNamespace.Xmlns + "r", ExcelCommon.Schema_Relationships));

			WriteWorkSheet_sheetPr(sheet, xmlWorkSheet);
			xmlWorkSheet.Add(WriteWorkSheet_dimension(sheet));
			xmlWorkSheet.Add(WriteWorkSheet_sheetViews(sheet));
			xmlWorkSheet.Add(WriteWorkSheet_sheetFormatPr(sheet));
			WriteWorkSheet_cols(sheet, xmlWorkSheet);
			xmlWorkSheet.Add(WriteWorkSheet_sheetData(sheet, sheetComments, sheetHyperLink));

			if (sheetHyperLink.Count > 0)
			{
				xmlWorkSheet.Add(WriteWorkSheet_hyperLinks(sheet, sheetHyperLink, worksheetPart));
			}
			xmlWorkSheet.Add(WriteWorkSheet_mergeCells(sheet));
			XElement result = WriteWorkSheet_phoneticPr(sheet);
			if (result != null)
			{
				xmlWorkSheet.Add(result);
			}

			WriteWorkSheet_DataValidations(xmlWorkSheet, sheet);
			xmlWorkSheet.Add(WriteWorkSheet_pageMargins(sheet));
			xmlWorkSheet.Add(WriteWorkSheet_printOptions(sheet));
			xmlWorkSheet.Add(WriteWorkSheet_sheetProtection(sheet));

			//WriteWorkSheet_pageSetup(sheet, xmlWorkSheet);
			WriteWorkSheet_pageSetup(sheet, xmlWorkSheet);
			xmlWorkSheet.Add(WriteWorkSheet_tableParts(sheet));

			if (sheet._HeaderFooter != null)
			{
				xmlWorkSheet.Add(WriteWorkSheet_HeaderFooter(sheet, worksheetPart));
			}

			xmlWorkSheet.Add(WriteWorkSheet_drawing(sheet, worksheetPart));

			if (sheetComments.Count > 0)
			{
				this.Context.Comments.Add(sheet.Name, sheetComments);
				xmlWorkSheet.Add(WriteWorkSheet_legacyDrawing(sheet));
			}

			WriteWorkSheet_legacyDrawingHF(sheet, xmlWorkSheet);

			XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), xmlWorkSheet);
			using (Stream stream = worksheetPart.GetStream(FileMode.Create, FileAccess.Write))
			{
				doc.Save(stream);
				stream.Flush();
			}
		}

		private void WriteWorkSheet_cols(WorkSheet sheet, XElement xmlWorkSheet)
		{
			bool isAdd = false;
			int ColumnMax = 0;
			XElement colsNode = new XElement(XName.Get("cols", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			foreach (Column col in sheet.Columns)
			{
				if (ColumnMax == 0 || ColumnMax < col.Index)
				{
					if (col._Style != null || col.Hidden || (col.Width != sheet.DefaultColumnWidth && col.Width != 0 && col.Width != ExcelCommon.WorkSheet_DefaultColumnWidth))
					{
						XElement colNode = new XElement(XName.Get("col", ExcelCommon.Schema_WorkBook_Main.NamespaceName), new XAttribute(XName.Get("min"), col.Index));
						WriteWorkSheet_cols_col_ColumnMax(ref ColumnMax, col, colNode);
						WriteWorkSheet_cols_Col(colNode, col, sheet);
						colsNode.Add(colNode);
						isAdd = true;
					}
				}
			}
			if (isAdd)
			{
				xmlWorkSheet.Add(colsNode);
			}
		}

		private void WriteWorkSheet_cols_col_ColumnMax(ref int ColumnMax, Column col, XElement colNode)
		{
			if (col.ColumnMax > col.Index)
			{
				ColumnMax = col.ColumnMax;
				colNode.Add(new XAttribute(XName.Get("max"), col.ColumnMax));
			}
			else
				colNode.Add(new XAttribute(XName.Get("max"), col.Index));
		}

		private void WriteWorkSheet_cols_Col(XElement colNode, Column col, WorkSheet sheet)
		{
			if (col.Width != sheet.DefaultColumnWidth && col.Width != ExcelCommon.WorkSheet_DefaultColumnWidth)
			{
				colNode.Add(new XAttribute(XName.Get("width"), col.Width));
				colNode.Add(new XAttribute(XName.Get("customWidth"), 1));
			}
			if (col.BestFit)
			{
				colNode.Add(new XAttribute(XName.Get("bestFit"), 1));
			}
			if (col.Collapsed)
			{
				colNode.Add(new XAttribute(XName.Get("collapsed"), 1));
			}
			if (col.Style != null)
			{
				colNode.Add(new XAttribute(XName.Get("style"), GetStyleId(col.Style)));
			}
			if (col.Phonetic)
			{
				colNode.Add(new XAttribute(XName.Get("phonetic"), 1));
			}
			if (col.OutlineLevel != 0)
			{
				colNode.Add(new XAttribute(XName.Get("outlineLevel"), col.OutlineLevel));
			}
			if (col.Hidden)
			{
				colNode.Add(new XAttribute(XName.Get("hidden"), 1));
			}
		}

		#region "DataValidations“
		private void WriteWorkSheet_DataValidations(XElement xmlWorkSheet, WorkSheet sheet)
		{
			if (sheet._Validations != null)
			{
				XElement dataValidationsNode = new XElement(XName.Get("dataValidations", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
				int index = 0;
				foreach (IDataValidation item in sheet._Validations)
				{
					index++;
					XElement dataValidationNode = new XElement(XName.Get("dataValidation", ExcelCommon.Schema_WorkBook_Main.NamespaceName),
						new XAttribute("type", item.ValidationType.SchemaName),
						new XAttribute(XName.Get("sqref"), item.Address.ToAddress()));
					WriteWorkSheet_DataValidations_Attributes(dataValidationNode, item);
					WriteWorkSheet_DataValidations_formulas(item, dataValidationNode);

					dataValidationsNode.Add(dataValidationNode);
				}
				dataValidationsNode.Add(new XAttribute("count", index));
				xmlWorkSheet.Add(dataValidationsNode);
			}
		}

		private void WriteWorkSheet_DataValidations_formulas(IDataValidation item, XElement dataValidationNode)
		{
			if (item is DataValidationInt)
			{
				DataValidationInt intItem = item as DataValidationInt;
				WriteWorkSheet_DataValidations_formulas_int(intItem, dataValidationNode);
			}
			else if (item is DataValidationDecimal)
			{
				DataValidationDecimal decimalItem = item as DataValidationDecimal;
				WriteWorkSheet_DataValidations_formulas_decimal(decimalItem, dataValidationNode);
			}
			else if (item is DataValidationList)
			{
				DataValidationList listItem = item as DataValidationList;
				WriteWorkSheet_DataValidations_formulas_list(listItem, dataValidationNode);
			}
			else if (item is DataValidationDateTime)
			{
				DataValidationDateTime dateTime = item as DataValidationDateTime;
				WriteWorkSheet_DataValidations_formulas_dateTime(dateTime, dataValidationNode);
			}
			else if (item is DataValidationTime)
			{
				DataValidationTime timeItem = item as DataValidationTime;
				WriteWorkSheet_DataValidations_formulas_Time(timeItem, dataValidationNode);
			}
			else if (item is DataValidationCustom)
			{
				DataValidationCustom customItem = item as DataValidationCustom;
				WriteWorkSheet_DataValidations_formulas_Custom(customItem, dataValidationNode);
			}
		}

		private void WriteWorkSheet_DataValidations_formulas_list(DataValidationList listItem, XElement dataValidationNode)
		{
			dataValidationNode.Add(new XElement(XName.Get("formula1", ExcelCommon.Schema_WorkBook_Main.NamespaceName), listItem.Formula.GetValueAsString()));
		}

		private void WriteWorkSheet_DataValidations_formulas_Custom(DataValidationCustom customItem, XElement dataValidationNode)
		{

			dataValidationNode.Add(new XElement(XName.Get("formula1", ExcelCommon.Schema_WorkBook_Main.NamespaceName), customItem.Formula.GetValueAsString()));
		}

		private void WriteWorkSheet_DataValidations_formulas_Time(DataValidationTime timeItem, XElement dataValidationNode)
		{
			if (timeItem.ValidationType.AllowOperator)
			{
				if (timeItem.Operator != ExcelDataValidationOperator.between)
				{
					dataValidationNode.Add(new XAttribute(XName.Get("operator"), timeItem.Operator.ToString()));
				}
			}

			dataValidationNode.Add(new XElement(XName.Get("formula1", ExcelCommon.Schema_WorkBook_Main.NamespaceName), timeItem.Formula.GetValueAsString()));
			if (timeItem.Operator == ExcelDataValidationOperator.between || timeItem.Operator == ExcelDataValidationOperator.notBetween)
			{
				dataValidationNode.Add(new XElement(XName.Get("formula2", ExcelCommon.Schema_WorkBook_Main.NamespaceName), timeItem.Formula2.GetValueAsString()));
			}
		}

		private void WriteWorkSheet_DataValidations_formulas_dateTime(DataValidationDateTime dateTime, XElement dataValidationNode)
		{
			if (dateTime.ValidationType.AllowOperator)
			{
				if (dateTime.Operator != ExcelDataValidationOperator.between)
				{
					dataValidationNode.Add(new XAttribute(XName.Get("operator"), dateTime.Operator.ToString()));
				}
			}

			dataValidationNode.Add(new XElement(XName.Get("formula1", ExcelCommon.Schema_WorkBook_Main.NamespaceName), dateTime.Formula.GetValueAsString()));
			if (dateTime.Operator == ExcelDataValidationOperator.between || dateTime.Operator == ExcelDataValidationOperator.notBetween)
			{
				dataValidationNode.Add(new XElement(XName.Get("formula2", ExcelCommon.Schema_WorkBook_Main.NamespaceName), dateTime.Formula2.GetValueAsString()));
			}
		}

		private void WriteWorkSheet_DataValidations_formulas_decimal(DataValidationDecimal decimalItem, XElement dataValidationNode)
		{
			if (decimalItem.ValidationType.AllowOperator)
			{
				if (decimalItem.Operator != ExcelDataValidationOperator.between)
				{
					dataValidationNode.Add(new XAttribute(XName.Get("operator"), decimalItem.Operator.ToString()));
				}
			}

			dataValidationNode.Add(new XElement(XName.Get("formula1", ExcelCommon.Schema_WorkBook_Main.NamespaceName), decimalItem.Formula.GetValueAsString()));
			if (decimalItem.Operator == ExcelDataValidationOperator.between || decimalItem.Operator == ExcelDataValidationOperator.notBetween)
			{
				dataValidationNode.Add(new XElement(XName.Get("formula2", ExcelCommon.Schema_WorkBook_Main.NamespaceName), decimalItem.Formula2.GetValueAsString()));
			}
		}

		private void WriteWorkSheet_DataValidations_formulas_int(DataValidationInt intItem, XElement dataValidationNode)
		{
			if (intItem.ValidationType.AllowOperator)
			{
				if (intItem.Operator != ExcelDataValidationOperator.between)
				{
					dataValidationNode.Add(new XAttribute(XName.Get("operator"), intItem.Operator.ToString()));
				}
			}

			dataValidationNode.Add(new XElement(XName.Get("formula1", ExcelCommon.Schema_WorkBook_Main.NamespaceName), intItem.Formula.GetValueAsString()));
			if (intItem.Operator == ExcelDataValidationOperator.between || intItem.Operator == ExcelDataValidationOperator.notBetween)
			{
				dataValidationNode.Add(new XElement(XName.Get("formula2", ExcelCommon.Schema_WorkBook_Main.NamespaceName), intItem.Formula2.GetValueAsString()));
			}
		}

		private void WriteWorkSheet_DataValidations_Attributes(XElement dataValidationNode, IDataValidation item)
		{
			if (item.AllowBlank.ConvertBool())
			{
				dataValidationNode.Add(new XAttribute(XName.Get("allowBlank"), 1));
			}

			if (item.ShowInputMessage.ConvertBool())
			{
				dataValidationNode.Add(new XAttribute(XName.Get("showInputMessage"), 1));
			}
			if (item.ShowErrorMessage.ConvertBool())
			{
				dataValidationNode.Add(new XAttribute(XName.Get("showErrorMessage"), 1));
			}

			if (!string.IsNullOrEmpty(item.ErrorTitle))
			{
				dataValidationNode.Add(new XAttribute(XName.Get("errorTitle"), item.ErrorTitle));
			}

			if (!string.IsNullOrEmpty(item.Error))
			{
				dataValidationNode.Add(new XAttribute(XName.Get("error"), item.Error));
			}

			if (!string.IsNullOrEmpty(item.PromptTitle))
			{
				dataValidationNode.Add(new XAttribute(XName.Get("promptTitle"), item.PromptTitle));
			}

			if (!string.IsNullOrEmpty(item.Prompt))
			{
				dataValidationNode.Add(new XAttribute(XName.Get("prompt"), item.Prompt));
			}
		}
		#endregion

		private XElement WriteWorkSheet_drawing(WorkSheet sheet, PackagePart worksheetPart)
		{
			if (sheet.Drawings.Count > 0)
			{
				((IPersistable)sheet.Drawings).Save(this.Context);
				return new XElement(XName.Get("drawing", ExcelCommon.Schema_WorkBook_Main.NamespaceName), new XAttribute(XName.Get("id", ExcelCommon.Schema_Relationships), sheet.Drawings.RelationshipID));
			}
			else
			{
				return null;
			}
		}

		private void WriteWorkSheet_legacyDrawingHF(WorkSheet sheet, XElement xmlWorkSheet)
		{
			if (sheet._HeaderFooter != null)
			{
				if (sheet._HeaderFooter._Pictures != null)
				{
					if (sheet._HeaderFooter._Pictures.Count > 0)
					{
						xmlWorkSheet.Add(new XElement(XName.Get("legacyDrawingHF", ExcelCommon.Schema_WorkBook_Main.NamespaceName),
							new XAttribute(XName.Get("id", ExcelCommon.Schema_Relationships), sheet._HeaderFooter._Pictures.RelationshipID)));
					}
				}
			}
		}

		private XElement WriteWorkSheet_HeaderFooter(WorkSheet sheet, PackagePart worksheetPart)
		{
			XElement headerFooterNode = new XElement(XName.Get("headerFooter", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			WriteWorkSheet_HeaderFooter_Attribute(sheet._HeaderFooter, headerFooterNode);
			if (sheet._HeaderFooter._OddHeader != null)
			{
				WriteWorkSheet_HeaderFooter_oddHeader("oddHeader", sheet._HeaderFooter._OddHeader, headerFooterNode);
				WriteWorkSheet_HeaderFooter_AddPictures(sheet, sheet._HeaderFooter._OddHeader);
			}
			if (sheet._HeaderFooter._OddFooter != null)
			{
				WriteWorkSheet_HeaderFooter_oddHeader("oddFooter", sheet._HeaderFooter._OddFooter, headerFooterNode);
				WriteWorkSheet_HeaderFooter_AddPictures(sheet, sheet._HeaderFooter._OddFooter);
			}

			if (sheet._HeaderFooter.DifferentOddEven)
			{
				headerFooterNode.Add(new XAttribute("differentOddEven", 1));
				if (sheet._HeaderFooter._EvenHeader != null)
				{
					WriteWorkSheet_HeaderFooter_oddHeader("evenHeader", sheet._HeaderFooter._EvenHeader, headerFooterNode);
					WriteWorkSheet_HeaderFooter_AddPictures(sheet, sheet._HeaderFooter._EvenHeader);
				}
				if (sheet._HeaderFooter._EvenFooter != null)
				{
					WriteWorkSheet_HeaderFooter_oddHeader("evenFooter", sheet._HeaderFooter._EvenFooter, headerFooterNode);
					WriteWorkSheet_HeaderFooter_AddPictures(sheet, sheet._HeaderFooter._EvenFooter);
				}
			}

			if (sheet._HeaderFooter.DifferentFirst)
			{
				headerFooterNode.Add(new XAttribute("differentFirst", 1));
				if (sheet._HeaderFooter._FirstHeader != null)
				{
					WriteWorkSheet_HeaderFooter_oddHeader("firstHeader", sheet._HeaderFooter._FirstHeader, headerFooterNode);
					WriteWorkSheet_HeaderFooter_AddPictures(sheet, sheet._HeaderFooter._FirstHeader);
				}
				if (sheet._HeaderFooter._FirstFooter != null)
				{
					WriteWorkSheet_HeaderFooter_oddHeader("firstFooter", sheet._HeaderFooter._FirstFooter, headerFooterNode);
					WriteWorkSheet_HeaderFooter_AddPictures(sheet, sheet._HeaderFooter._FirstFooter);
				}
			}

			WriteWorkSheet_HeaderFooter_Pictures(sheet, worksheetPart);

			return headerFooterNode;
		}

		private void WriteWorkSheet_HeaderFooter_Pictures(WorkSheet sheet, PackagePart worksheetPart)
		{
			if (sheet._HeaderFooter._Pictures != null)
			{
				if (sheet._HeaderFooter._Pictures.Count > 0)
				{
					sheet._HeaderFooter._Pictures.PictureUri = this.Context.Package.GetNewUri(@"/xl/drawings/vmlDrawing{0}.vml");
					PackageRelationship rel = worksheetPart.CreateRelationship(PackUriHelper.GetRelativeUri(sheet.SheetUri, sheet._HeaderFooter._Pictures.PictureUri), TargetMode.Internal, ExcelCommon.Schema_Relationships + "/vmlDrawing");
					sheet._HeaderFooter._Pictures.RelationshipID = rel.Id;
					((IPersistable)sheet._HeaderFooter._Pictures).Save(this.Context);
				}
				else
				{
					sheet._HeaderFooter._Pictures.RelationshipID = string.Empty;
				}
			}
		}

		private void WriteWorkSheet_HeaderFooter_AddPictures(WorkSheet sheet, HeaderFooterText headerText)
		{
			if (headerText.LeftImag != null)
				sheet._HeaderFooter.Pictures.Add(headerText.LeftImag);

			if (headerText.CenteredImag != null)
				sheet._HeaderFooter.Pictures.Add(headerText.CenteredImag);

			if (headerText.RightAlignedImag != null)
				sheet._HeaderFooter.Pictures.Add(headerText.RightAlignedImag);
		}

		private void WriteWorkSheet_HeaderFooter_oddHeader(string tag, HeaderFooterText headerFooter, XElement headerFooterNode)
		{
			XElement oddHeaderNode = new XElement(XName.Get(tag, ExcelCommon.Schema_WorkBook_Main.NamespaceName));

			StringBuilder strRet = new StringBuilder();
			if (string.IsNullOrEmpty(headerFooter.LeftAlignedText) == false)
			{
				strRet.Append("&L");
				strRet.Append(headerFooter.LeftAlignedText);
			}

			if (string.IsNullOrEmpty(headerFooter.CenteredText) == false)
			{
				strRet.Append("&C");
				strRet.Append(headerFooter.CenteredText);
			}

			if (string.IsNullOrEmpty(headerFooter.RightAlignedText) == false)
			{
				strRet.Append("&R");
				strRet.Append(headerFooter.RightAlignedText);
			}

			oddHeaderNode.Value = strRet.ToString();

			headerFooterNode.Add(oddHeaderNode);
		}

		private void WriteWorkSheet_HeaderFooter_Attribute(HeaderFooter sheetFooter, XElement headerFooterNode)
		{
			if (sheetFooter.AlignWithMargins == false)
				headerFooterNode.Add(new XAttribute(XName.Get("alignWithMargins"), 0));

			if (sheetFooter.DifferentOddEven)
				headerFooterNode.Add(new XAttribute(XName.Get("differentOddEven"), 1));

			if (sheetFooter.AlignWithMargins == false)
				headerFooterNode.Add(new XAttribute(XName.Get("alignWithMargins"), 0));

			if (sheetFooter.ScaleWithDoc == false)
				headerFooterNode.Add(new XAttribute(XName.Get("scaleWithDoc"), 0));
		}

		private XElement WriteWorkSheet_hyperLinks(WorkSheet sheet, List<Cell> sheetHyperLink, PackagePart worksheetPart)
		{
			XElement sheethyperlinks = new XElement(XName.Get("hyperlinks", ExcelCommon.Schema_WorkBook_Main.NamespaceName));

			Dictionary<string, string> hyps = new Dictionary<string, string>();

			foreach (Cell celllink in sheetHyperLink)
			{
				XElement hyperlinkNode = new XElement(XName.Get("hyperlink", ExcelCommon.Schema_WorkBook_Main.NamespaceName));

				#region "location"
				//if (string.IsNullOrEmpty(celllink._Hyperlink.AbsoluteUri))
				if (celllink._Hyperlink.AbsoluteUri == "xl://internal/")
				{
					if (celllink._Hyperlink is ExcelHyperLink)
					{
						ExcelHyperLink hyperlink = celllink._Hyperlink as ExcelHyperLink;

						string address = Range.Parse(sheet, celllink.Column.Index, celllink.Row.Index, hyperlink.ColSpann + celllink.Column.Index, celllink.Row.Index + hyperlink.RowSpann).ToAddress();
						hyperlinkNode.Add(new XAttribute("ref", address));

						if (hyperlink.ReferenceAddress.IsNotEmpty())
						{
							string strlocation = hyperlink.ReferenceAddress;
							if (strlocation.Contains("!"))
							{
								if (strlocation[0] != '\'')
								{
									hyperlinkNode.Add(new XAttribute("location", string.Format("'{0}'!{1}", strlocation.Substring(0, strlocation.IndexOf('!')), strlocation.Substring(strlocation.IndexOf('!') + 1))));
								}
								else
								{
									hyperlinkNode.Add(new XAttribute("location", strlocation));
								}
							}
							else
							{
								hyperlinkNode.Add(new XAttribute("location", string.Format("'{0}'!{1}", sheet.Name, strlocation)));
							}
						}

						if (hyperlink.Display.IsNotEmpty())
						{
							hyperlinkNode.Add(new XAttribute("display", hyperlink.Display));
						}

						if (hyperlink.ToolTip.IsNotEmpty())
						{
							hyperlinkNode.Add(new XAttribute("tooltip", hyperlink.Display));
						}
					}
				}
				else
				{
					if (hyperlinkNode.Attribute(XName.Get("ref", ExcelCommon.Schema_WorkBook_Main.NamespaceName)) == null)
					{
						hyperlinkNode.Add(new XAttribute("ref", celllink.ToString()));
					}
					string idRelations = string.Empty;
					if (hyps.ContainsKey(celllink._Hyperlink.AbsoluteUri))
					{
						idRelations = hyps[celllink._Hyperlink.AbsoluteUri];
					}
					else
					{
						idRelations = worksheetPart.CreateRelationship(celllink._Hyperlink, TargetMode.External, ExcelCommon.Schema_Hyperlink).Id;
						hyps.Add(celllink._Hyperlink.AbsoluteUri, idRelations);
					}
					hyperlinkNode.Add(new XAttribute(XName.Get("id", ExcelCommon.Schema_Relationships), idRelations));
				}
				#endregion
				sheethyperlinks.Add(hyperlinkNode);
			}
			return sheethyperlinks;
		}

		private XElement WriteWorkSheet_legacyDrawing(WorkSheet sheet)
		{
			XElement commentsParts = new XElement(ExcelCommon.Schema_WorkBook_Main + "legacyDrawing");
			CommentCollection comments = new CommentCollection(sheet);
			((IPersistable)comments).Save(this.Context);

			commentsParts.Add(new XAttribute(XName.Get("id", ExcelCommon.Schema_Relationships), this.Context.CommentsSheetRelationships[sheet.Name]));

			return commentsParts;
		}

		/// <summary>
		/// FileName:sheet.xml
		/// <para>NodePath:worksheet/sheetPr</para>
		/// </summary>
		private void WriteWorkSheet_sheetPr(WorkSheet sheet, XElement xmlWorkSheet)
		{
			//todo:
			/* 	 "syncHorizontal", "syncVertical", "syncRef", "transitionEvaluation", "transitionEntry", 
			"published", "codeName", "filterMode", "enableFormatConditionsCalculation"  */

			if (sheet.OutLineApplyStyle || sheet.ShowOutlineSymbols || sheet.OutLineSummaryBelow || sheet.OutLineSummaryRight ||
				sheet.PageSetup.ShowAutoPageBreaks || sheet.PageSetup.FitToPage || sheet._TabColor != null || string.IsNullOrEmpty(sheet.SheetCode) == false)
			{
				XElement sheetPr = new XElement(ExcelCommon.Schema_WorkBook_Main + "sheetPr");

				WriteWorkSheet_sheetPr_Attributes(sheet, sheetPr);

				WriteWorkSheet_sheetPr_outlinePr(sheetPr, sheet);
				WriteWorkSheet_sheetPr_pageSetUpPr(sheetPr, sheet);
				if (sheet._TabColor != null)
				{
					XElement tabColor = new XElement(ExcelCommon.Schema_WorkBook_Main + "tabColor");
					tabColor.Add(WriteColor(sheet._TabColor));
					sheetPr.Add(tabColor);
				}

				xmlWorkSheet.Add(sheetPr);
			}
		}

		private void WriteWorkSheet_sheetPr_Attributes(WorkSheet sheet, XElement sheetPr)
		{
			/* 	 "syncHorizontal", "syncVertical", "syncRef", "transitionEvaluation", "transitionEntry", 
		   "published", "codeName", "filterMode", "enableFormatConditionsCalculation"  */

			if (string.IsNullOrEmpty(sheet.SheetCode) == false)
			{
				sheetPr.Add(new XAttribute("codeName", sheet.SheetCode));
			}
		}

		private void WriteWorkSheet_sheetPr_pageSetUpPr(XElement sheetPr, WorkSheet sheet)
		{
			if (sheet.PageSetup.ShowAutoPageBreaks || sheet.PageSetup.FitToPage)
			{
				XElement pageSetUpPr = new XElement(ExcelCommon.Schema_WorkBook_Main + "pageSetUpPr");
				if (sheet.PageSetup.ShowAutoPageBreaks)
				{
					pageSetUpPr.Add(new XAttribute("autoPageBreaks", "1"));
				}
				if (sheet.PageSetup.FitToPage)
				{
					pageSetUpPr.Add(new XAttribute("fitToPage", "1"));
				}

				sheetPr.Add(pageSetUpPr);
			}
		}

		private void WriteWorkSheet_sheetPr_outlinePr(XElement sheetPr, WorkSheet sheet)
		{
			if (sheet.OutLineApplyStyle || sheet.ShowOutlineSymbols || sheet.OutLineSummaryBelow || sheet.OutLineSummaryRight)
			{
				XElement outlinePr = new XElement(ExcelCommon.Schema_WorkBook_Main + "outlinePr");
				if (sheet.OutLineApplyStyle)
					outlinePr.Add(new XAttribute("applyStyles", "1"));

				if (sheet.ShowOutlineSymbols)
					outlinePr.Add(new XAttribute("showOutlineSymbols", "1"));

				if (sheet.OutLineSummaryBelow)
					outlinePr.Add(new XAttribute("summaryBelow", "1"));
				else
					outlinePr.Add(new XAttribute("summaryBelow", "0"));
				if (sheet.OutLineSummaryRight)
					outlinePr.Add(new XAttribute("summaryRight", "1"));

				sheetPr.Add(outlinePr);
			}
		}

		/// <summary>
		/// FileName:sheet.xml
		/// <para>NodePath:worksheet/dimension</para>
		/// </summary>
		private XElement WriteWorkSheet_dimension(WorkSheet sheet)
		{
			XElement dimension = new XElement(ExcelCommon.Schema_WorkBook_Main + "dimension");
			if (sheet.Dimension.StartColumn <= 0 || sheet.Dimension.StartRow <= 0)
				dimension.Add(new XAttribute("ref", "A1"));
			else
				dimension.Add(new XAttribute("ref", sheet.Dimension.ToAddress()));

			return dimension;
		}

		/// <summary>
		/// FileName:sheet.xml
		/// <para>NodePath:worksheet/sheetViews</para>
		/// </summary>
		private XElement WriteWorkSheet_sheetViews(WorkSheet sheet)
		{
			XElement sheetViews = new XElement(ExcelCommon.Schema_WorkBook_Main + "sheetViews");
			XElement sheetView = new XElement(ExcelCommon.Schema_WorkBook_Main + "sheetView", new XAttribute("workbookViewId", "0"));

			WriteWorkSheet_sheetViews_Attributes(sheetView, sheet.SheetView);

			sheetViews.Add(sheetView);

			return sheetViews;
		}

		private void WriteWorkSheet_sheetViews_Attributes(XElement sheetViewNode, SheetView sheetView)
		{
			if (sheetView.TabSelected)
				sheetViewNode.Add(new XAttribute("tabSelected", "1"));
			if (sheetView.ShowFormulas)
				sheetViewNode.Add(new XAttribute("showFormulas", "1"));

			if (this.Workbook._Protection != null)
			{
				if (this.Workbook._Protection.LockWindows || this.Workbook._Protection.LockStructure)
					sheetViewNode.Add(new XAttribute("windowProtection", "1"));
			}

			if (sheetView.ShowGridLines)
				sheetViewNode.Add(new XAttribute("showGridLines", "1"));
			else
				sheetViewNode.Add(new XAttribute("showGridLines", "0"));
			if (sheetView.ShowZeros)
				sheetViewNode.Add(new XAttribute("showZeros", "1"));

			if (sheetView.ShowRowColHeaders)
				sheetViewNode.Add(new XAttribute("showRowColHeaders", "1"));

			if (sheetView.ShowWhiteSpace)
				sheetViewNode.Add(new XAttribute("ShowWhiteSpace", 1));

			if (sheetView.ZoomScale >= 10 && sheetView.ZoomScale <= 400)
				sheetViewNode.Add(new XAttribute("zoomScale", sheetView.ZoomScale));

			if (sheetView.TopLeftCell.ColumnIndex > 0 && sheetView.TopLeftCell.RowIndex > 0)
				sheetViewNode.Add(new XAttribute("topLeftCell", sheetView.TopLeftCell.ToString()));

			if (sheetView.SelectedRange.StartColumn > 0 && sheetView.SelectedRange.StartRow > 0)
				sheetViewNode.Add(new XElement(ExcelCommon.Schema_WorkBook_Main + "selection",
				  new XAttribute("activeCell", string.Format("{0}{1}", ExcelHelper.GetColumnLetterFromNumber(sheetView.SelectedRange.StartColumn), sheetView.SelectedRange.StartRow)),
				  new XAttribute("sqref", sheetView.SelectedRange.ToAddress())));
		}

		/// <summary>
		/// FileName:sheet.xml
		/// <para>NodePath:worksheet/sheetFormatPr</para>
		/// </summary>
		private XElement WriteWorkSheet_sheetFormatPr(WorkSheet sheet)
		{
			/*
			 "baseColWidth", (double)
			"defaultColWidth",  (double)
			"defaultRowHeight",  (double)
			"customHeight",  (bool)
			"zeroHeight",  (bool)
			"thickTop",  (bool)
			"thickBottom",  (bool)
			"outlineLevelRow",  (bool)
			"outlineLevelCol",  (bool)
			"dyDescent" (bool) */
			XElement sheetFormatPr = null;
			if (sheet.DefaultColumnWidth != ExcelCommon.WorkSheet_DefaultColumnWidth || sheet.CustomHeight)
			{
				sheetFormatPr = new XElement(ExcelCommon.Schema_WorkBook_Main + "sheetFormatPr");
				if (sheet.DefaultColumnWidth != ExcelCommon.WorkSheet_DefaultColumnWidth)
					sheetFormatPr.Add(new XAttribute("defaultColWidth",
						sheet.DefaultColumnWidth.ToString(CultureInfo.InvariantCulture)));

				if (sheet.CustomHeight)
				{
					sheetFormatPr.Add(new XAttribute("customHeight", "1"));
					if (sheet.DefaultRowHeight != ExcelCommon.WorkSheet_DefaultRowHeight)
						sheetFormatPr.Add(new XAttribute("defaultRowHeight",
							sheet.DefaultRowHeight.ToString(CultureInfo.InvariantCulture)));
				}
			}

			return sheetFormatPr;
		}

		/// <summary>
		/// FileName:sheet.xml 
		/// <para>NodePath:worksheet/sheetData</para>
		/// </summary>
		private XElement WriteWorkSheet_sheetData(WorkSheet sheet, List<Comment> sheetComments, List<Cell> sheetLinks)
		{
			XElement sheetData = new XElement(ExcelCommon.Schema_WorkBook_Main + "sheetData");
			//用于保存生成的行信息
			SortedDictionary<int, XElement> rowDict = new SortedDictionary<int, XElement>();
			//用于保存行内列信息
			Dictionary<int, SortedDictionary<int, XElement>> rowCellsDict = new Dictionary<int, SortedDictionary<int, XElement>>();

			foreach (Row r in sheet.Rows)
			{
				rowDict.Add(r.Index, WriteWorkSheet_sheetData_row(sheet, r));
				rowCellsDict.Add(r.Index, new SortedDictionary<int, XElement>());
			}
			foreach (var cell in sheet.Cells)
			{
				SortedDictionary<int, XElement> rcDict = rowCellsDict[cell.Row.Index];
				WriteWorkSheet_sheetData_c(sheet, rcDict, cell);

				if (cell._Comment != null)
				{
					sheetComments.Add(cell._Comment);
				}

				if (cell._Hyperlink != null)
				{
					sheetLinks.Add(cell);
				}
			}

			foreach (var rowKV in rowDict)
			{
				var cellDict = rowCellsDict[rowKV.Key];
				foreach (var cellKV in cellDict)
				{
					rowKV.Value.Add(cellKV.Value);
				}
				sheetData.Add(rowKV.Value);
			}

			return sheetData;
		}

		/// <summary>
		/// FileName:sheet.xml 
		/// <para>NodePath:worksheet/sheetData</para>
		/// <para>NodePath:worksheet/sheetData/row</para>
		/// </summary>
		private XElement WriteWorkSheet_sheetData_row(WorkSheet sheet, Row row)
		{
			XElement rowXml = new XElement(ExcelCommon.Schema_WorkBook_Main + "row");

			rowXml.Add(new XAttribute("r", row.Index.ToString(CultureInfo.InvariantCulture)));
			if (sheet.Dimension.StartColumn > 0 && sheet.Dimension.EndColumn > 0)
			{
				rowXml.Add(new XAttribute("spans", string.Format(CultureInfo.InvariantCulture, "{0}:{1}",
					sheet.Dimension.StartColumn, sheet.Dimension.EndColumn)));
			}

			if (row.Hidden)
			{
				rowXml.Add(new XAttribute("ht", "0"));
				rowXml.Add(new XAttribute("hidden", "1"));
			}
			if (row.Height != sheet.DefaultRowHeight && row.Height != ExcelCommon.WorkSheet_DefaultRowHeight)
			{
				if (rowXml.Attributes("ht").Count() == 0)
					rowXml.Add(new XAttribute("ht", row.Height.ToString(CultureInfo.InvariantCulture)));
				rowXml.Add(new XAttribute("customHeight", "1"));
			}
			if (row._Style != null)
			{
				rowXml.Add(new XAttribute("s", GetStyleId(row.Style)));
				rowXml.Add(new XAttribute("customFormat", "1"));
			}
			if (row.OutlineLevel > 0)
			{
				rowXml.Add(new XAttribute("outlineLevel", row.OutlineLevel.ToString(CultureInfo.InvariantCulture)));
				if (row.Collapsed)
				{
					rowXml.Add(new XAttribute("collapsed", "1"));
					if (row.Hidden == false)
						rowXml.Add(new XAttribute("hidden", "1"));
				}
			}
			if (row.Phonetic)
				rowXml.Add(new XAttribute("ph", "1"));

			return rowXml;
		}

		/// <summary>
		/// FileName:sheet.xml 
		/// <para>NodePath:worksheet/sheetData/row/c</para>
		/// </summary>
		private void WriteWorkSheet_sheetData_c(WorkSheet sheet, SortedDictionary<int, XElement> cellDict, Cell cell)
		{
			XElement element = new XElement(ExcelCommon.Schema_WorkBook_Main + "c");
			element.Add(new XAttribute("r", cell.ToString()));

			string str = cell.Value as string;
			if (str.IsNullOrEmpty() && cell.Value == null)
			{
				if (cell.Formula.IsNotEmpty() || cell.FormulaSharedIndex > 0)
				{
					XElement formula = WriteWorkSheet_sheetData_c_f(sheet, cell);
					if (formula != null)
					{
						element.Add(formula);
						cellDict.Add(cell.Column.Index, element);
					}
				}
				else
				{
					if (cell.CheckStyleIsEmpty() == false)
					{
						WriteWorkSheet_sheetData_c_style(element, cell);
						cellDict.Add(cell.Column.Index, element);
					}
				}
				return;
			}

			if ((cell.Value.GetType().IsPrimitive || cell.Value is double || cell.Value is decimal || cell.Value is DateTime || cell.Value is TimeSpan))
			{
				try
				{
					if (cell.Value is DateTime)
					{
						str = ((DateTime)cell.Value).ToOADate().ToString(CultureInfo.InvariantCulture);

						//样式为空
						if (cell.CheckStyleIsEmpty())
						{
							cell._Style = new CellStyleXmlWrapper();
							cell._Style.NumberFormat.Format = "mm-dd-yy";
						}
						else
						{   //样式设置在列或行上，但主体单元格没有设置
							if ((cell._Style == null && cell.CheckStyleIsEmpty() == false) || (string.IsNullOrEmpty(cell.Style.NumberFormat.Format)))
								cell.Style.NumberFormat.Format = "mm-dd-yy";
						}

					}
					else if (cell.Value is TimeSpan)
					{
						str = new DateTime(((TimeSpan)cell.Value).Ticks).ToOADate().ToString(CultureInfo.InvariantCulture);
						if (cell._Style == null)
						{
							cell._Style = new CellStyleXmlWrapper();
						}
						if (string.IsNullOrEmpty(cell._Style.NumberFormat.Format))
						{
							cell.Style.NumberFormat.Format = "h:mm";
						}
					}
					else
					{
						if (cell.Value is double && double.IsNaN((double)cell.Value))
						{
							str = "0";
						}
						else
						{
							str = Convert.ToDouble(cell.Value, CultureInfo.InvariantCulture).ToString("g15", CultureInfo.InvariantCulture);
						}
					}
				}
				catch
				{
					str = "0";
				}

				if (cell.Value is bool)
				{
					element.Add(new XAttribute("t", "b"));
				}

				var formula = WriteWorkSheet_sheetData_c_f(sheet, cell);
				if (formula != null)
				{
					element.Add(formula);
				}
				element.Add(new XElement(ExcelCommon.Schema_WorkBook_Main + "v", str));
			}
			else if (str.IsNotEmpty())
			{
				element.Add(new XAttribute("t", "s"));
				var formula = WriteWorkSheet_sheetData_c_f(sheet, cell);
				if (formula != null)
				{
					element.Add(formula);
				}

				if (this.Context.SharedStrings.ContainsKey(str))
				{
					element.Add(new XElement(ExcelCommon.Schema_WorkBook_Main + "v", this.Context.SharedStrings[str].Pos.ToString(CultureInfo.InvariantCulture)));
				}
				else
				{
					int index = this.Context.SharedStrings.Count;
					this.Context.SharedStrings.Add(str, new SharedStringItem() { IsRichText = cell.IsRichText, Pos = index, Text = ExcelHelper.ExcelDecodeString(str) });
					element.Add(new XElement(ExcelCommon.Schema_WorkBook_Main + "v", index.ToString(CultureInfo.InvariantCulture)));
				}
			}

			WriteWorkSheet_sheetData_c_style(element, cell);
			cellDict.Add(cell.Column.Index, element);
		}

		// private void WriteWorkSheet_sheetData_c_byCellType(

		private void WriteWorkSheet_sheetData_c_style(XElement element, Cell cell)
		{
			#region "S" Style
			if (cell._Style != null)
			{
				element.Add(new XAttribute("s", GetStyleId(cell.Style)));
			}
			else if (cell.Row._Style != null)
			{
				element.Add(new XAttribute("s", GetStyleId(cell.Row._Style)));
			}
			else if (cell.Column._Style != null)
			{
				element.Add(new XAttribute("s", GetStyleId(cell.Column._Style)));
			}
			#endregion
		}

		/// <summary>
		///FileName:sheet.xml 
		/// <para>NodePath:worksheet/sheetData/row/c/f</para>
		/// </summary>
		private XElement WriteWorkSheet_sheetData_c_f(WorkSheet sheet, Cell cell)
		{
			XElement result = null;
			if (cell.Formula.IsNotEmpty() && cell.FormulaSharedIndex == int.MinValue)
			{
				result = new XElement(ExcelCommon.Schema_WorkBook_Main + "f");
				result.Value = cell.Formula;
			}
			else
			{
				if (cell.FormulaSharedIndex > 0)
				{
					result = new XElement(ExcelCommon.Schema_WorkBook_Main + "f");
					Formulas currentF = sheet._SharedFormulas.FirstOrDefault(f => f.Value.Index == cell.FormulaSharedIndex).Value;

					result.Add(new XAttribute("t", "shared"));
					result.Add(new XAttribute("si", cell.FormulaSharedIndex.ToString(CultureInfo.InvariantCulture)));

					if (cell.Row.Index == currentF.Address.StartRow && cell.Column.Index == currentF.Address.StartColumn)
					{
						result.Add(new XAttribute("ref", currentF.Address.ToAddress()));
						result.Value = currentF.Formula;
					}
				}
				else if (cell.HasDataTable)
				{
					result = new XElement(ExcelCommon.Schema_WorkBook_Main + "f");
					var currentF = sheet._DataTableFormulas.Where(f => f.Key == cell).Select(r => r.Value).FirstOrDefault();

					currentF.IsNotNull(o =>
					{
						var f = o as Formulas;

						result.Add(new XAttribute("t", "dataTable"));

						f.Dt2D.IsNotWhiteSpace(v => result.Add(new XAttribute("dt2D", v)));
						f.Dtr.IsNotWhiteSpace(v => result.Add(new XAttribute("dtr", f.Dtr)));
						f.R1.IsNotWhiteSpace(v => result.Add(new XAttribute("r1", f.R1)));
						f.R2.IsNotWhiteSpace(v => result.Add(new XAttribute("r2", f.R2)));
						f.Ca.IsNotWhiteSpace(v => result.Add(new XAttribute("ca", f.Ca)));

						result.Add(new XAttribute("ref", f.Address.ToAddress()));
					});
				}
			}

			return result;
		}

		/// <summary>
		///FileName:sheet.xml 
		/// <para>NodePath:worksheet/mergeCells</para>
		/// </summary>
		private XElement WriteWorkSheet_mergeCells(WorkSheet sheet)
		{
			if (sheet._MergeCells.Count == 0)
				return null;

			XElement mergeCellsXml = new XElement(ExcelCommon.Schema_WorkBook_Main + "mergeCells");
			foreach (Range mergeCell in sheet._MergeCells)
			{
				XElement mergeCellXml = new XElement(ExcelCommon.Schema_WorkBook_Main + "mergeCell", new XAttribute("ref", mergeCell.ToAddress()));
				mergeCellsXml.Add(mergeCellXml);
			}

			return mergeCellsXml;
		}

		/// <summary>
		///FileName:sheet.xml 
		/// <para>NodePath:worksheet/phoneticPr</para>
		/// </summary>
		private XElement WriteWorkSheet_phoneticPr(WorkSheet sheet)
		{
			if (sheet._PhoneticProperties != null)
			{
				//todo:待提取出来
				XElement phoneticPr = new XElement(ExcelCommon.Schema_WorkBook_Main + sheet.PhoneticProperties.NodeName);

				foreach (var item in sheet.PhoneticProperties.Attributes)
				{
					phoneticPr.Add(new XAttribute(item.Key, item.Value));
				}

				return phoneticPr;
			}
			else
			{
				return null;
			}
		}

		private XElement WriteWorkSheet_pageMargins(WorkSheet sheet)
		{
			XElement pageMargins = null;

			if (sheet.PageSetup.LeftMargin != decimal.MinValue || sheet.PageSetup.RightMargin != decimal.MinValue ||
				sheet.PageSetup.TopMargin != decimal.MinValue || sheet.PageSetup.BottomMargin != decimal.MinValue ||
				sheet.PageSetup.HeaderMargin != decimal.MinValue || sheet.PageSetup.FooterMargin != decimal.MinValue)
			{
				pageMargins = new XElement(ExcelCommon.Schema_WorkBook_Main + "pageMargins");
				if (sheet.PageSetup.LeftMargin != decimal.MinValue)
					pageMargins.Add(new XAttribute("left", sheet.PageSetup.LeftMargin.ToString(CultureInfo.InvariantCulture)));


				if (sheet.PageSetup.RightMargin != decimal.MinValue)
					pageMargins.Add(new XAttribute("right", sheet.PageSetup.RightMargin.ToString(CultureInfo.InvariantCulture)));

				if (sheet.PageSetup.TopMargin != decimal.MinValue)
					pageMargins.Add(new XAttribute("top", sheet.PageSetup.TopMargin.ToString(CultureInfo.InvariantCulture)));

				if (sheet.PageSetup.BottomMargin != decimal.MinValue)
					pageMargins.Add(new XAttribute("bottom", sheet.PageSetup.BottomMargin.ToString(CultureInfo.InvariantCulture)));

				if (sheet.PageSetup.HeaderMargin != decimal.MinValue)
					pageMargins.Add(new XAttribute("header", sheet.PageSetup.HeaderMargin.ToString(CultureInfo.InvariantCulture)));

				if (sheet.PageSetup.FooterMargin != decimal.MinValue)
					pageMargins.Add(new XAttribute("footer", sheet.PageSetup.FooterMargin.ToString(CultureInfo.InvariantCulture)));
			}

			return pageMargins;
		}

		private XElement WriteWorkSheet_printOptions(WorkSheet sheet)
		{
			XElement printOptions = null;

			if (sheet.PageSetup.ShowHeaders ||
				sheet.PageSetup.ShowGridLines ||
				sheet.PageSetup.HorizontalCentered ||
				 sheet.PageSetup.VerticalCentered)
			{
				printOptions = new XElement(ExcelCommon.Schema_WorkBook_Main + "printOptions");
				if (sheet.PageSetup.ShowHeaders)
					printOptions.Add(new XAttribute("headings", "1"));

				if (sheet.PageSetup.ShowGridLines)
					printOptions.Add(new XAttribute("gridLines", "1"));

				if (sheet.PageSetup.HorizontalCentered)
					printOptions.Add(new XAttribute("horizontalCentered", "1"));

				if (sheet.PageSetup.VerticalCentered)
					printOptions.Add(new XAttribute("verticalCentered", "1"));
			}
			return printOptions;
		}

		private void WriteWorkSheet_pageSetup(WorkSheet sheet, XElement xmlWorkSheet)
		{
			if (sheet._PageSetup != null)
			{
				XElement pageSetup = new XElement(ExcelCommon.Schema_WorkBook_Main + "pageSetup");
				if (sheet.PageSetup.BlackAndWhite)
					pageSetup.Add(new XAttribute("blackAndWhite", "1"));

				if (sheet.PageSetup.Draft)
					pageSetup.Add(new XAttribute("draft", "1"));

				if (sheet.PageSetup.NumberOfCopiesToPrint != int.MinValue)
					pageSetup.Add(new XAttribute("copies", sheet.PageSetup.NumberOfCopiesToPrint.ToString(CultureInfo.InvariantCulture)));

				if (sheet.PageSetup.FirstPageNumber != int.MinValue)
					pageSetup.Add(new XAttribute("firstPageNumber", sheet.PageSetup.FirstPageNumber.ToString(CultureInfo.InvariantCulture)));

				if (sheet.PageSetup.FitToHeight != int.MinValue)
					pageSetup.Add(new XAttribute("fitToHeight", sheet.PageSetup.FitToHeight.ToString(CultureInfo.InvariantCulture)));

				if (sheet.PageSetup.FitToWidth != int.MinValue)
					pageSetup.Add(new XAttribute("fitToHeight", sheet.PageSetup.FitToWidth.ToString(CultureInfo.InvariantCulture)));

				if (sheet.PageSetup.Scale != int.MinValue)
					pageSetup.Add(new XAttribute("scale", sheet.PageSetup.Scale.ToString(CultureInfo.InvariantCulture)));

				if (sheet.PageSetup.Orientation != ExcelOrientation.Default)
					pageSetup.Add(new XAttribute("orientation", sheet.PageSetup.Orientation.ToString().ToLower()));

				if (sheet.PageSetup.PageOrder == ExcelPageOrder.OverThenDown)
					pageSetup.Add(new XAttribute("pageOrder", "overThenDown"));

				if (sheet.PageSetup.PaperSize != ExcelPaperSize.Letter)
					pageSetup.Add(new XAttribute("paperSize", ((int)sheet.PageSetup.PaperSize).ToString(CultureInfo.InvariantCulture)));

				xmlWorkSheet.Add(pageSetup);
			}
		}

		private XElement WriteWorkSheet_sheetProtection(WorkSheet sheet)
		{
			if (sheet.SheetProtection.Attributes.Count == 0)
				return null;

			XElement sheetProtection = new XElement(ExcelCommon.Schema_WorkBook_Main + sheet.SheetProtection.NodeName);
			foreach (var item in sheet.SheetProtection.Attributes)
			{
				sheetProtection.Add(new XAttribute(item.Key, item.Value));
			}
			return sheetProtection;
		}

		private XElement WriteWorkSheet_tableParts(WorkSheet sheet)
		{
			int tableCount = sheet.Tables.Count;
			if (tableCount == 0)
			{
				return null;
			}
			XElement tableParts = new XElement(ExcelCommon.Schema_WorkBook_Main + "tableParts", new XAttribute("count", tableCount));
			foreach (Table table in sheet.Tables)
			{
				((IPersistable)table).Save(this.Context);
				var xName = XName.Get("id", ExcelCommon.Schema_Relationships);
				XElement tablePart = new XElement(ExcelCommon.Schema_WorkBook_Main + "tablePart",
					new XAttribute(xName, table.RelationshipID));
				tableParts.Add(tablePart);
			}

			return tableParts;
		}
		#endregion

		#region sharedStrings.xml
		/// <summary>
		/// FileName:sharedStrings.xml 
		/// <para>NodePath:sst</para>
		/// </summary>
		public void WriteSharedStrings()
		{
			PackagePart partStrings = this.Context.Package.CreatePart(ExcelCommon.Uri_SharedStrings,
				@"application/vnd.openxmlformats-officedocument.spreadsheetml.sharedStrings+xml", CompressionOption.Maximum);

			using (StreamWriter streamStrings = new StreamWriter(partStrings.GetStream(FileMode.Create, FileAccess.Write)))
			{
				if (this.Context.SharedStrings.Count > 0)
				{
					streamStrings.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\" ?><sst xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" count=\"{0}\" uniqueCount=\"{0}\">", this.Context.SharedStrings.Count);
					foreach (string t in this.Context.SharedStrings.Keys)
					{
						SharedStringItem ssi = this.Context.SharedStrings[t];
						if (ssi.IsRichText)
						{
							streamStrings.Write("<si>");
							ExcelHelper.ExcelEncodeString(streamStrings, t);
							streamStrings.Write("</si>");
						}
						else
						{
							if (t.Length > 0 && (t[0] == ' ' || t[t.Length - 1] == ' ' || t.Contains("  ") || t.Contains("\t")))
							{
								streamStrings.Write("<si><t xml:space=\"preserve\">");
							}
							else
							{
								streamStrings.Write("<si><t>");
							}
							ExcelHelper.ExcelEncodeString(streamStrings, SecurityElement.Escape(t));
							streamStrings.Write("</t></si>");
						}
					}
					streamStrings.Write("</sst>");
				}
				else
				{
					XDocument doc = XDocument.Parse(string.Format("<?xml version=\"1.0\" encoding=\"UTF-8\" ?><sst count=\"0\" uniqueCount=\"0\" xmlns=\"{0}\" />", ExcelCommon.Schema_WorkBook_Main));
					doc.Save(streamStrings);
				}

				this.Context.Package.GetPart(ExcelCommon.Uri_Workbook).CreateRelationship(ExcelCommon.Uri_SharedStrings, TargetMode.Internal, ExcelCommon.Schema_Relationships + "/sharedStrings");
				streamStrings.Flush();
			}

			this.Context.ResetSharedStrings();
		}
		#endregion

		#region core.xml
		/// <summary>
		/// FileName:core.xml
		/// <para>NodePath:</para>
		/// </summary>
		public void WriteCore()
		{
			PackagePart partCore = this.Context.Package.CreatePart(ExcelCommon.Uri_PropertiesCore,
				@"application/vnd.openxmlformats-package.core-properties+xml");

			XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement(XName.Get("coreProperties", ExcelCommon.Schema_Cp),
				new XAttribute(XNamespace.Xmlns + "cp", ExcelCommon.Schema_Cp),
				new XAttribute(XNamespace.Xmlns + "dc", ExcelCommon.Schema_Dc),
				new XAttribute(XNamespace.Xmlns + "dcterms", ExcelCommon.Schema_DcTerms),
				new XAttribute(XNamespace.Xmlns + "dcmitype", ExcelCommon.Schema_DcmiType),
				new XAttribute(XNamespace.Xmlns + "xsi", ExcelCommon.Schema_Xsi)));

			if (this.Workbook.FileDetails.Title.IsNotEmpty())
			{
				doc.Root.Add(new XElement(XName.Get("title", ExcelCommon.Schema_Dc), this.Workbook.FileDetails.Title));
			}
			if (this.Workbook.FileDetails.Subject.IsNotEmpty())
			{
				doc.Root.Add(new XElement(XName.Get("subject", ExcelCommon.Schema_Dc), this.Workbook.FileDetails.Subject));
			}
			if (this.Workbook.FileDetails.Author.IsNotEmpty())
			{
				doc.Root.Add(new XElement(XName.Get("creator", ExcelCommon.Schema_Dc), this.Workbook.FileDetails.Author));
			}
			if (this.Workbook.FileDetails.Comments.IsNotEmpty())
			{
				doc.Root.Add(new XElement(XName.Get("description", ExcelCommon.Schema_Dc), this.Workbook.FileDetails.Comments));
			}
			if (this.Workbook.FileDetails.Keywords.IsNotEmpty())
			{
				doc.Root.Add(new XElement(XName.Get("keywords", ExcelCommon.Schema_Cp), this.Workbook.FileDetails.Keywords));
			}
			if (this.Workbook.FileDetails.LastModifiedBy.IsNotEmpty())
			{
				doc.Root.Add(new XElement(XName.Get("lastModifiedBy", ExcelCommon.Schema_Cp), this.Workbook.FileDetails.LastModifiedBy));
			}
			if (this.Workbook.FileDetails.LastPrinted.IsNotEmpty())
			{
				doc.Root.Add(new XElement(XName.Get("lastPrinted", ExcelCommon.Schema_Cp), this.Workbook.FileDetails.LastPrinted));
			}
			if (this.Workbook.FileDetails.Category.IsNotEmpty())
			{
				doc.Root.Add(new XElement(XName.Get("category", ExcelCommon.Schema_Cp), this.Workbook.FileDetails.Category));
			}
			if (this.Workbook.FileDetails.Status.IsNotEmpty())
			{
				doc.Root.Add(new XElement(XName.Get("contentStatus", ExcelCommon.Schema_Cp), this.Workbook.FileDetails.Status));
			}

			using (StreamWriter streamCore = new StreamWriter(partCore.GetStream(FileMode.Create, FileAccess.Write)))
			{
				doc.Save(streamCore);
			}

			this.Context.Package.CreateRelationship(PackUriHelper.GetRelativeUri(new Uri("/xl", UriKind.Relative),
				ExcelCommon.Uri_PropertiesCore), TargetMode.Internal,
				@"http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties");
		}
		#endregion

		#region app.xml

		/// <summary>
		/// FileName:app.xml
		/// <para>NodePath:</para>
		/// </summary>
		public void WriteApp()
		{
			PackagePart partExtended = this.Context.Package.CreatePart(ExcelCommon.Uri_PropertiesExtended,
				@"application/vnd.openxmlformats-officedocument.extended-properties+xml");

			XNamespace ns = ExcelCommon.Schema_Extended;
			XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement(ns + "Properties",
				new XAttribute(XNamespace.Xmlns + "vt", ExcelCommon.Schema_Vt)));

			if (this.Workbook.FileDetails.Application.IsNotEmpty())
			{
				doc.Root.Add(new XElement(XName.Get("Application", ExcelCommon.Schema_Extended), this.Workbook.FileDetails.Application));
			}
			if (this.Workbook.FileDetails.AppVersion != null)
			{
				doc.Root.Add(new XElement(XName.Get("AppVersion", ExcelCommon.Schema_Extended), this.Workbook.FileDetails.AppVersion));
			}
			if (this.Workbook.FileDetails.Company != null)
			{
				doc.Root.Add(new XElement(XName.Get("Company", ExcelCommon.Schema_Extended), this.Workbook.FileDetails.Company));
			}
			if (this.Workbook.FileDetails.Manager.IsNotEmpty())
			{
				doc.Root.Add(new XElement(XName.Get("Manager", ExcelCommon.Schema_Extended), this.Workbook.FileDetails.Manager));
			}
			using (StreamWriter stream = new StreamWriter(partExtended.GetStream(FileMode.Create, FileAccess.Write)))
			{
				doc.Save(stream);
			}
			this.Context.Package.CreateRelationship(PackUriHelper.GetRelativeUri(new Uri("/xl", UriKind.Relative),
				ExcelCommon.Uri_PropertiesExtended), TargetMode.Internal,
				@"http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties");
		}
		#endregion

		#region custom.xml
		/// <summary>
		/// FileName:custom.xml
		/// <para>NodePath:</para>
		/// </summary>
		public void WriteCustom()
		{
			PackagePart partCustom = this.Context.Package.CreatePart(ExcelCommon.Uri_PropertiesCustom,
				@"application/vnd.openxmlformats-officedocument.custom-properties+xml");

			using (StreamWriter streamCustom = new StreamWriter(partCustom.GetStream(FileMode.Create, FileAccess.Write)))
			{
				this.Workbook.FileDetails.CustomPropertiesXml.Save(streamCustom);
			}
			this.Context.Package.CreateRelationship(PackUriHelper.GetRelativeUri(new Uri("/xl", UriKind.Relative), ExcelCommon.Uri_PropertiesCustom), TargetMode.Internal, @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/custom-properties");
		}
		#endregion

		#region styles.xml
		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet</para>
		/// </summary>
		/// <param name="calcPrRoot"></param>
		public void WriteStyles(WorkBookStylesWrapper style)
		{
			PackagePart partSyles = this.Context.Package.CreatePart(ExcelCommon.Uri_Styles,
				@"application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml", CompressionOption.Normal);

			XElement styleXml = new XElement(ExcelCommon.Schema_WorkBook_Main + "styleSheet",
				new XAttribute(XNamespace.Xmlns + "d", ExcelCommon.Schema_WorkBook_Main.NamespaceName));

			styleXml.Add(WriteStyles_numFmts(style));
			styleXml.Add(WriteStyles_fonts(style));
			styleXml.Add(WriteStyles_fills(style));
			styleXml.Add(WriteStyles_borders(style));
			styleXml.Add(WriteStyles_cellStyleXfs(style));
			styleXml.Add(WriteStyles_cellXfs(style));
			styleXml.Add(WriteStyles_cellStyles(style));
			styleXml.Add(WriteStyles_dxfs(style));
			styleXml.Add(WriteStyles_tableStyles(style));

			XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), styleXml);
			using (var stream = partSyles.GetStream(FileMode.Create, FileAccess.Write))
			{
				doc.Save(stream);
				stream.Flush();
			}

			this.Context.Package.GetPart(ExcelCommon.Uri_Workbook).CreateRelationship(
				PackUriHelper.GetRelativeUri(ExcelCommon.Uri_Workbook, ExcelCommon.Uri_Styles),
				TargetMode.Internal, ExcelCommon.Schema_Relationships + "/styles");
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/numFmts</para>
		/// </summary>
		private XElement WriteStyles_numFmts(WorkBookStylesWrapper style)
		{
			int count = style.NumberFormats.Count(n => n.BuildIn == false);
			if (count == 0) return null;

			XElement numFmts = new XElement(ExcelCommon.Schema_WorkBook_Main + "numFmts");
			numFmts.Add(new XAttribute("count", count.ToString(CultureInfo.InvariantCulture)));
			foreach (var numberFormat in style.NumberFormats)
			{
				if (numberFormat.BuildIn == false && numberFormat.Format.IsNotEmpty())
				{
					XElement numFmt = new XElement(ExcelCommon.Schema_WorkBook_Main + "numFmt");
					numFmt.Add(new XAttribute("numFmtId", numberFormat.NumFmtId.ToString(CultureInfo.InvariantCulture)));
					numFmt.Add(new XAttribute("formatCode", numberFormat.Format));
					numFmts.Add(numFmt);
				}
			}

			return numFmts;
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/fonts</para>
		/// </summary>
		private XElement WriteStyles_fonts(WorkBookStylesWrapper style)
		{
			XElement fontsXml = new XElement(ExcelCommon.Schema_WorkBook_Main + "fonts");

			int count = style.Fonts.Count;
			if (count > 0)
			{
				fontsXml.Add(new XAttribute("count", count.ToString(CultureInfo.InvariantCulture)));

				foreach (var font in style.Fonts)
				{
					fontsXml.Add(WriteFont(font));
				}
			}
			else
			{
				count = 1;
				fontsXml.Add();
				fontsXml.Add(new XAttribute("count", "1"),
					new XElement(ExcelCommon.Schema_WorkBook_Main + "font",
						new XElement(ExcelCommon.Schema_WorkBook_Main + "sz", new XAttribute("val", "11")),
						new XElement(ExcelCommon.Schema_WorkBook_Main + "name", new XAttribute("val", "Calibri"))
					)
				);
			}

			return fontsXml;
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/fills</para>
		/// </summary>
		private XElement WriteStyles_fills(WorkBookStylesWrapper style)
		{
			XElement fillsXml = new XElement(ExcelCommon.Schema_WorkBook_Main + "fills");

			int count = style.Fills.Count;
			if (count > 0)
			{
				fillsXml.Add(new XAttribute("count", count.ToString(CultureInfo.InvariantCulture)));

				foreach (var fill in style.Fills)
				{
					fillsXml.Add(WriteFill(fill));
				}
			}
			else
			{
				fillsXml.Add(new XAttribute("count", "2"),
					new XElement(ExcelCommon.Schema_WorkBook_Main + "fill",
						new XElement(ExcelCommon.Schema_WorkBook_Main + "patternFill", new XAttribute("patternType", "none")),
						new XElement(ExcelCommon.Schema_WorkBook_Main + "patternFill", new XAttribute("patternType", "gray125"))
					)
				);
			}

			return fillsXml;
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/borders</para>
		/// </summary>
		private XElement WriteStyles_borders(WorkBookStylesWrapper style)
		{
			XElement bordersXml = new XElement(ExcelCommon.Schema_WorkBook_Main + "borders");

			if (style.Borders.Count > 0)
			{
				bordersXml.Add(new XAttribute("count", style.Borders.Count.ToString(CultureInfo.InvariantCulture)));
				foreach (var border in style.Borders)
				{
					bordersXml.Add(WriteBorder(border));
				}
			}
			else
			{
				bordersXml.Add(new XAttribute("count", "1"),
					new XElement(ExcelCommon.Schema_WorkBook_Main + "border",
						new XElement(ExcelCommon.Schema_WorkBook_Main + "left"),
						new XElement(ExcelCommon.Schema_WorkBook_Main + "right"),
						new XElement(ExcelCommon.Schema_WorkBook_Main + "top"),
						new XElement(ExcelCommon.Schema_WorkBook_Main + "bottom"),
						new XElement(ExcelCommon.Schema_WorkBook_Main + "diagonal")
					)
				);
			}

			return bordersXml;
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/cellStyleXfs</para>
		/// </summary>
		private XElement WriteStyles_cellStyleXfs(WorkBookStylesWrapper style)
		{
			XElement result = new XElement(ExcelCommon.Schema_WorkBook_Main + "cellStyleXfs");

			if (style.CellStyleXfs.Count > 0)
			{
				result.Add(new XAttribute("count", style.CellStyleXfs.Count.ToString(CultureInfo.InvariantCulture)));

				foreach (var csxfs in style.CellStyleXfs)
				{
					XElement xfXml = new XElement(ExcelCommon.Schema_WorkBook_Main + "xf");
					if (csxfs.NumberFormatId > 0)
					{
						xfXml.Add(new XAttribute("numFmtId", csxfs.NumberFormatId.ToString(CultureInfo.InvariantCulture)));
					}
					if (csxfs.FontId > int.MinValue)
					{
						xfXml.Add(new XAttribute("fontId", csxfs.FontId.ToString(CultureInfo.InvariantCulture)));
					}
					if (csxfs.FillId > int.MinValue)
					{
						xfXml.Add(new XAttribute("fillId", csxfs.FillId.ToString(CultureInfo.InvariantCulture)));
					}
					if (csxfs.BorderId > int.MinValue)
					{
						xfXml.Add(new XAttribute("borderId", csxfs.BorderId.ToString(CultureInfo.InvariantCulture)));
					}

					result.Add(xfXml);
				}
			}
			else
			{
				result.Add(new XAttribute("count", "1"),
					new XElement(ExcelCommon.Schema_WorkBook_Main + "xf",
					new XAttribute("numFmtId", "0"),
					new XAttribute("fontId", "0"),
					new XAttribute("fillId", "0"),
					new XAttribute("borderId", "0")
					)
				);
			}
			return result;
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/cellXfs</para>
		/// </summary>
		private XElement WriteStyles_cellXfs(WorkBookStylesWrapper style)
		{
			XElement result = new XElement(ExcelCommon.Schema_WorkBook_Main + "cellXfs");

			if (style.CellXfs.Count > 0)
			{
				result.Add(new XAttribute("count", style.CellXfs.Count.ToString(CultureInfo.InvariantCulture)));

				foreach (var cellxfs in style.CellXfs)
				{
					result.Add(WriteStyles_cellXfs_xf(cellxfs));
				}
			}
			else
			{
				result.Add(new XAttribute("count", "1"),
					new XElement(ExcelCommon.Schema_WorkBook_Main + "xf",
					new XAttribute("numFmtId", "0"),
					new XAttribute("fontId", "0"),
					new XAttribute("fillId", "0"),
					new XAttribute("borderId", "0"),
					new XAttribute("xfId", "0"))
				);
			}
			return result;
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/cellXfs/xf</para>
		/// </summary>
		private XElement WriteStyles_cellXfs_xf(CellStyleXmlWrapper cellxfs)
		{
			XElement xfXml = new XElement(ExcelCommon.Schema_WorkBook_Main + "xf");
			if (cellxfs.NumberFormatId > int.MinValue)
			{
				xfXml.Add(new XAttribute("numFmtId", cellxfs.NumberFormatId.ToString(CultureInfo.InvariantCulture)));
			}
			if (cellxfs.FontId > int.MinValue)
			{
				xfXml.Add(new XAttribute("fontId", cellxfs.FontId.ToString(CultureInfo.InvariantCulture)));
			}
			if (cellxfs.FillId > int.MinValue)
			{
				xfXml.Add(new XAttribute("fillId", cellxfs.FillId.ToString(CultureInfo.InvariantCulture)));
			}
			if (cellxfs.BorderId > int.MinValue)
			{
				xfXml.Add(new XAttribute("borderId", cellxfs.BorderId.ToString(CultureInfo.InvariantCulture)));
			}
			if (cellxfs.XfId > int.MinValue)
			{
				xfXml.Add(new XAttribute("xfId", cellxfs.XfId.ToString(CultureInfo.InvariantCulture)));
			}


			if (cellxfs.HorizontalAlignment != ExcelHorizontalAlignment.Left ||
				cellxfs.VerticalAlignment != ExcelVerticalAlignment.Bottom || cellxfs.Indent > 0 ||
				cellxfs.TextRotation > 0 || cellxfs.ShrinkToFit)
			{
				xfXml.Add(new XAttribute("applyAlignment", "1"));
				if (cellxfs.Locked == false || cellxfs.Hidden)
				{
					xfXml.Add(new XAttribute("applyProtection", "1"));
				}

				XElement alignmentXml = new XElement(ExcelCommon.Schema_WorkBook_Main + "alignment");
				if (cellxfs.HorizontalAlignment != ExcelHorizontalAlignment.Left)
				{
					alignmentXml.Add(new XAttribute("horizontal", cellxfs.HorizontalAlignment.SetAlignString()));
				}
				if (cellxfs.VerticalAlignment != ExcelVerticalAlignment.Bottom)
				{
					alignmentXml.Add(new XAttribute("vertical", cellxfs.VerticalAlignment.SetAlignString()));
				}
				if (cellxfs.Indent > 0)
				{
					alignmentXml.Add(new XAttribute("indent", cellxfs.Indent.ToString(CultureInfo.InvariantCulture)));
				}
				if (cellxfs.TextRotation > 0)
				{
					alignmentXml.Add(new XAttribute("textRotation", cellxfs.TextRotation.ToString(CultureInfo.InvariantCulture)));
				}
				if (cellxfs.WrapText)
				{
					alignmentXml.Add(new XAttribute("wrapText", "1"));
				}
				if (cellxfs.ShrinkToFit)
				{
					alignmentXml.Add(new XAttribute("shrinkToFit", "1"));
				}
				if (cellxfs.ReadingOrder)
				{
					alignmentXml.Add(new XAttribute("readingOrder", "1"));
				}
				xfXml.Add(alignmentXml);
			}


			if (cellxfs.Locked == false || cellxfs.Hidden)
			{
				XElement protectionXml = new XElement(ExcelCommon.Schema_WorkBook_Main + "protection");
				if (cellxfs.Locked == false)
				{
					protectionXml.Add(new XAttribute("locked", "0"));
				}
				if (cellxfs.Hidden)
				{
					protectionXml.Add(new XAttribute("hidden", "1"));
				}
				xfXml.Add(protectionXml);
			}

			return xfXml;
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/cellStyles</para>
		/// </summary>
		private XElement WriteStyles_cellStyles(WorkBookStylesWrapper style)
		{
			XElement result = new XElement(ExcelCommon.Schema_WorkBook_Main + "cellStyles");

			if (style.NamedStyles.Count > 0)
			{
				result.Add(new XAttribute("count", style.NamedStyles.Count.ToString(CultureInfo.InvariantCulture)));

				foreach (var nameStyle in style.NamedStyles)
				{
					XElement cellStyleXml = new XElement(ExcelCommon.Schema_WorkBook_Main + "cellStyle");

					if (nameStyle.Name.IsNotEmpty())
					{
						cellStyleXml.Add(new XAttribute("name", nameStyle.Name));
					}
					if (nameStyle.BuildInId > int.MinValue)
					{
						cellStyleXml.Add(new XAttribute("builtinId", nameStyle.BuildInId.ToString(CultureInfo.InvariantCulture)));
					}
					if (nameStyle.XfId >= 0)
					{
						cellStyleXml.Add(new XAttribute("xfId", style.CellXfs[nameStyle.XfId].newID.ToString(CultureInfo.InvariantCulture)));
					}
					result.Add(cellStyleXml);
				}
			}
			else
			{
				result.Add(new XAttribute("count", "1"),
					new XElement(ExcelCommon.Schema_WorkBook_Main + "cellStyle",
					new XAttribute("name", "Normal"),
					new XAttribute("builtinId", "0"),
					new XAttribute("xfId", "0"))
				);
			}
			return result;
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/dxfs</para>
		/// </summary>
		private XElement WriteStyles_dxfs(WorkBookStylesWrapper style)
		{
			XElement result = new XElement(ExcelCommon.Schema_WorkBook_Main + "dxfs", new XAttribute("count", style.CellXfs.Count.ToString(CultureInfo.InvariantCulture)));

			foreach (CellStyleXmlWrapper item in style.CellXfs)
			{
				result.Add(WriteStyles_dxfs_dxf(item));
			}

			return result;
		}

		private XElement WriteStyles_dxfs_dxf(CellStyleXmlWrapper dxfStyle)
		{
			XElement result = new XElement(ExcelCommon.Schema_WorkBook_Main + "dxf");
			result.Add(WriteFont(dxfStyle.Font));
			result.Add(WriteFill(dxfStyle.Fill));
			result.Add(WriteStyles_dxfs_dxf_alignment(dxfStyle));

			return result;
		}

		private XElement WriteStyles_dxfs_dxf_alignment(CellStyleXmlWrapper dxfStyle)
		{
			XElement alignmentXml = new XElement(ExcelCommon.Schema_WorkBook_Main + "alignment");

			alignmentXml.Add(new XAttribute("horizontal", dxfStyle.HorizontalAlignment.SetAlignString()));

			alignmentXml.Add(new XAttribute("vertical", dxfStyle.VerticalAlignment.SetAlignString()));

			alignmentXml.Add(new XAttribute("textRotation", dxfStyle.TextRotation.ToString(CultureInfo.InvariantCulture)));

			alignmentXml.Add(new XAttribute("wrapText", dxfStyle.WrapText ? "1" : "0"));

			alignmentXml.Add(new XAttribute("shrinkToFit", dxfStyle.ShrinkToFit ? "1" : "0"));

			alignmentXml.Add(new XAttribute("readingOrder", dxfStyle.ShrinkToFit ? "1" : "0"));

			return alignmentXml;
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/tableStyles</para>
		/// </summary>
		private XElement WriteStyles_tableStyles(WorkBookStylesWrapper style)
		{
			XElement result = new XElement(ExcelCommon.Schema_WorkBook_Main + "tableStyles",
				new XAttribute("count", "0"),
				new XAttribute("defaultTableStyle", "TableStyleMedium9"),
				new XAttribute("defaultPivotStyle", "PivotStyleLight16")
			);
			return result;
		}
		#endregion

		#region calculation.xml
		public void WriteCalculation(CalculationChain calculationChain)
		{
			XElement calculationChainElement = new XElement(ExcelCommon.Schema_WorkBook_Main + "calcChain");

			if (calculationChain.CalculationCells.Count > 0)
			{
				foreach (var c in calculationChain.CalculationCells)
				{
					XElement t = new XElement("c");

					c.Cell.IsNotWhiteSpace(o => t.Add(new XAttribute("r", ((string)o))));
					c.WorkSheet.IsNotWhiteSpace(o => t.Add(new XAttribute("i", ((string)o))));
					c.NewLevel.IsNotWhiteSpace(o => t.Add(new XAttribute("l", ((string)o))));
					c.InChildChain.IsNotWhiteSpace(o => t.Add(new XAttribute("s", ((string)o))));

					calculationChainElement.Add(t);
				}

				PackagePart calculationChainPart = this.Context.Package.CreatePart(ExcelCommon.Uri_CalculationChain,
					ExcelCommon.ContentType_CalculationChain, CompressionOption.Maximum);

				XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), calculationChainElement);
				using (Stream stream = calculationChainPart.GetStream(FileMode.Create, FileAccess.Write))
				{
					doc.Save(stream);
					stream.Flush();
				}
			}
		}
		#endregion

		#region "Table.xml"
		public void WriteTable(Table table)
		{
			table.Attributes["ref"] = table.Address.ToAddress();

			table.Attributes.Remove("headerRowDxfId");
			table.Attributes.Remove("dataDxfId");
			table.Attributes.Remove("dataCellStyle");
			table.Attributes.Remove("tableBorderDxfId");

            //沈峥删除这两个属性，影响Table的格式
            table.Attributes.Remove("headerRowBorderDxfId");
            table.Attributes.Remove("totalsRowBorderDxfId");
            
			XElement tableXml = WriteAttributes(table);
			/*
			new XElement(ExcelCommon.Schema_WorkBook_Main + "table",
			new XAttribute("id", this.Context.NextTableID.ToString(CultureInfo.InvariantCulture)),
			new XAttribute("name", table.Name),
			new XAttribute("displayName", table.DisplayName),
			new XAttribute("ref", table.Address.ToAddress())
		);

		if (table.IsTotalsRowShown)
		{
			tableXml.Add(new XAttribute("totalsRowShown", "1"));
		}
		else
		{
			tableXml.Add(new XAttribute("totalsRowShown", "0"));
		} */

			tableXml.Add(
				new XElement(ExcelCommon.Schema_WorkBook_Main + "autoFilter",
				new XAttribute("ref", table.Address.ToAddress()))
			);

			tableXml.Add(WriteTable_tableColumns(table));

			tableXml.Add(WriteTable_tableStyleInfo(table));

			PackagePart tablePart = this.Context.Package.CreatePart(table.TableUri,
				ExcelCommon.ContentType_Table, table._WorkSheet.WorkBook.Compression);

			if (table.RelationshipID.IsNullOrEmpty())
			{
				table.RelationshipID = this.Context.Package.GetPart(table._WorkSheet.SheetUri).CreateRelationship(
					PackUriHelper.GetRelativeUri(table._WorkSheet.SheetUri, table.TableUri),
					TargetMode.Internal, ExcelCommon.Schema_Relationships + "/table").Id;
			}
			else
			{
				this.Context.Package.GetPart(table._WorkSheet.SheetUri).CreateRelationship(
					 PackUriHelper.GetRelativeUri(table._WorkSheet.SheetUri, table.TableUri),
					 TargetMode.Internal, ExcelCommon.Schema_Relationships + "/table", table.RelationshipID);
			}

			XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), tableXml);
			using (Stream stream = tablePart.GetStream(FileMode.Create, FileAccess.Write))
			{
				doc.Save(stream);
				stream.Flush();
			}
		}

		private XElement WriteTable_tableStyleInfo(Table table)
		{
			XElement result = new XElement(ExcelCommon.Schema_WorkBook_Main + "tableStyleInfo");

			if (table.TableStyle != ExcelTableStyles.Custom)
			{
				result.Add(new XAttribute("name", string.Format("TableStyle{0}", table.TableStyle)));
			}
			else if (table.StyleName.IsNotEmpty())
			{
				result.Add(new XAttribute("name", table.StyleName));
			}

			if (table.ShowFirstColumn)
			{
				result.Add(new XAttribute("showFirstColumn", "1"));
			}
			else
			{
				result.Add(new XAttribute("showFirstColumn", "0"));
			}

			if (table.ShowLastColumn)
			{
				result.Add(new XAttribute("showLastColumn", "1"));
			}
			else
			{
				result.Add(new XAttribute("showLastColumn", "0"));
			}

			if (table.ShowRowStripes)
			{
				result.Add(new XAttribute("showRowStripes", "1"));
			}
			else
			{
				result.Add(new XAttribute("showRowStripes", "0"));
			}

			if (table.ShowColumnStripes)
			{
				result.Add(new XAttribute("showColumnStripes", "1"));
			}
			else
			{
				result.Add(new XAttribute("showColumnStripes", "0"));
			}

			return result;
		}

		private XElement WriteTable_tableColumns(Table table)
		{
			XElement tableColumnsXml = new XElement(ExcelCommon.Schema_WorkBook_Main + "tableColumns",
				new XAttribute("count", table.Columns.Count.ToString(CultureInfo.InvariantCulture)));

			for (int i = 0; i < table.Columns.Count; i++)
			{
				TableColumn column = table.Columns[i];
				column.Position = i + 1;

				column.Attributes.Remove("dataDxfId");
				column.Attributes.Remove("dataCellStyle");

				XElement tcXml = WriteAttributes(column);

				/*new XElement(ExcelCommon.Schema_WorkBook_Main + "tableColumn",
				new XAttribute("id", i + 1),
				new XAttribute("name", column.Name));

			if (column.DataDxfId.IsNotEmpty())
				tcXml.Add(new XAttribute("dataDxfId", column.DataDxfId)); */

				if (column.ColumnFormula.IsNotEmpty())
					tcXml.Add(new XElement(ExcelCommon.Schema_WorkBook_Main + "calculatedColumnFormula", column.ColumnFormula));

				tableColumnsXml.Add(tcXml);
			}

			return tableColumnsXml;
		}
		#endregion

		#region "comments1.xml"
		internal void WriteComments(CommentCollection sheetComments)
		{
			if (this.Context.Comments.ContainsKey(sheetComments._WorkSheet.Name))
			{
				List<Comment> cellsComment = this.Context.Comments[sheetComments._WorkSheet.Name];

				Dictionary<string, int> authorsList = new Dictionary<string, int>();
				SortedDictionary<int, SortedDictionary<int, XElement>> cells_Comments = new SortedDictionary<int, SortedDictionary<int, XElement>>();
				SortedDictionary<int, SortedDictionary<int, XElement>> cells_Comments_Drawing_shape = new SortedDictionary<int, SortedDictionary<int, XElement>>();
				int index = 1;
				foreach (Comment cellComment in cellsComment)
				{
					int rowIndex = cellComment._Cell.Row.Index, columnIndex = cellComment._Cell.Column.Index;
					if (cells_Comments.ContainsKey(rowIndex))
					{
						cells_Comments[rowIndex].Add(columnIndex, WriteComments_comment(cellComment, authorsList));
						cells_Comments_Drawing_shape[rowIndex].Add(columnIndex, WriteCommentsVmlDrawing_shape(cellComment, rowIndex, columnIndex, index));
					}
					else
					{
						SortedDictionary<int, XElement> rowCell = new SortedDictionary<int, XElement>();
						rowCell.Add(columnIndex, WriteComments_comment(cellComment, authorsList));
						cells_Comments.Add(rowIndex, rowCell);

						SortedDictionary<int, XElement> rowCell_VmlDrawing_shap = new SortedDictionary<int, XElement>();
						rowCell_VmlDrawing_shap.Add(columnIndex, WriteCommentsVmlDrawing_shape(cellComment, rowIndex, columnIndex, index));
						cells_Comments_Drawing_shape.Add(rowIndex, rowCell_VmlDrawing_shap);
					}
					index++;
				}

				this.Context.CommentsDrawing.Add(sheetComments._WorkSheet.Name, cells_Comments_Drawing_shape);
				cells_Comments_Drawing_shape = null;

				XDocument commentsDocXml = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
				XElement commentsNode = new XElement(XName.Get("comments", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
				XElement authorsNode = new XElement(XName.Get("authors", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
				foreach (KeyValuePair<string, int> authorkey in authorsList)
				{
					authorsNode.Add(new XElement(XName.Get("author", ExcelCommon.Schema_WorkBook_Main.NamespaceName)) { Value = authorkey.Key });
				}
				commentsNode.Add(authorsNode);

				XElement commentListNode = new XElement(XName.Get("commentList", ExcelCommon.Schema_WorkBook_Main.NamespaceName));

				foreach (KeyValuePair<int, SortedDictionary<int, XElement>> rowKey in cells_Comments)
				{
					foreach (KeyValuePair<int, XElement> commentKey in rowKey.Value)
					{
						commentListNode.Add(commentKey.Value);
					}
				}
				commentsNode.Add(commentListNode);
				commentsDocXml.Add(commentsNode);

				Uri CommentUri = new Uri(string.Format(@"/xl/comments{0}.xml", sheetComments._WorkSheet.PositionID + 1), UriKind.Relative);
				sheetComments.RelationshipID = this.Context.Package.GetPart(sheetComments._WorkSheet.SheetUri).CreateRelationship(PackUriHelper.GetRelativeUri(sheetComments._WorkSheet.SheetUri, CommentUri), TargetMode.Internal, ExcelCommon.Schema_Relationships + "/comments").Id;
				PackagePart commentPart = this.Context.Package.CreatePart(CommentUri,
				ExcelCommon.ContentType_Comments, sheetComments._WorkSheet.WorkBook.Compression);

				using (Stream stream = commentPart.GetStream(FileMode.Create, FileAccess.Write))
				{
					commentsDocXml.Save(stream);
					stream.Flush();
				}
				cellsComment = null;
				authorsList = null;
				cells_Comments = null;
			}
		}

		private XElement WriteComments_comment(Comment cellComment, Dictionary<string, int> authorsList)
		{
			XElement comment = new XElement(XName.Get("comment", ExcelCommon.Schema_WorkBook_Main.NamespaceName), new XAttribute("ref", cellComment._Cell.ToString()));
			if (cellComment.Author.IsNotEmpty())
			{
				int authorId = 0;
				if (authorsList.ContainsKey(cellComment.Author))
				{
					authorId = authorsList[cellComment.Author];
				}
				else
				{
					authorId = authorsList.Count;
					authorsList.Add(cellComment.Author, authorId);
				}
				comment.Add(new XAttribute("authorId", authorId));
			}

			if (cellComment.Text.IsNotEmpty())
			{
				WriteComments_comment_text(comment, cellComment);
			}

			return comment;
		}

		private void WriteComments_comment_text(XElement comment, Comment cellComment)
		{
			XElement textNode = new XElement(XName.Get("text", ExcelCommon.Schema_WorkBook_Main.NamespaceName));

			foreach (RichText rt in cellComment.RichText)
			{
				XElement rNode = new XElement(XName.Get("r", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
				WriteComments_comment_text_rPr(rNode, rt);

				XElement tNode = new XElement(XName.Get("t", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
				if (rt.PreserveSpace)
				{
					tNode.Add(new XAttribute(XName.Get("space", ExcelCommon.Schema_Xml), "preserve"));
				}
				tNode.Value = rt.Text;
				rNode.Add(tNode);
				textNode.Add(rNode);
			}

			comment.Add(textNode);
		}

		private void WriteComments_comment_text_rPr(XElement textNode, RichText rt)
		{
			XElement rtnode = new XElement(XName.Get("rPr", ExcelCommon.Schema_WorkBook_Main.NamespaceName));

			XElement rPrNode = null;
			if (rt.Bold)
			{
				XElement b = new XElement(XName.Get("b", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
				rtnode.Add(b);
			}

			if (rt._Charset != int.MinValue)
			{
				rPrNode = new XElement(XName.Get("charset", ExcelCommon.Schema_WorkBook_Main.NamespaceName),
					new XAttribute("val", rt._Charset));
				rtnode.Add(rPrNode);
			}

			if (rt._DataBarColor != null)
			{
				rPrNode = WriteColor(rt._DataBarColor);
				if (rPrNode != null)
				{
					rtnode.Add(rPrNode);
				}
			}

			if (rt.Condense)
			{
				rPrNode = new XElement(XName.Get("condense", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
				rtnode.Add(rPrNode);
			}

			if (rt.Extend)
			{
				rPrNode = new XElement(XName.Get("extend", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
				rtnode.Add(rPrNode);
			}
			//todo: family

			if (rt.Italic)
			{
				rPrNode = new XElement(XName.Get("i", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
				rtnode.Add(rPrNode);
			}

			if (rt.Outline)
			{
				rPrNode = new XElement(XName.Get("outline", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
				rtnode.Add(rPrNode);
			}

			if (rt.FontName.IsNotEmpty())
			{
				rPrNode = new XElement(XName.Get("rFont", ExcelCommon.Schema_WorkBook_Main.NamespaceName),
					new XAttribute("val", rt.FontName));
				rtnode.Add(rPrNode);
			}

			if (rt.Scheme.IsNotEmpty())
			{
				rPrNode = new XElement(XName.Get("scheme", ExcelCommon.Schema_WorkBook_Main.NamespaceName),
					new XAttribute("val", rt.Scheme));
				rtnode.Add(rPrNode);
			}

			if (rt.Shadow)
			{
				rPrNode = new XElement(XName.Get("shadow", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
				rtnode.Add(rPrNode);
			}

			if (rt.Strike)
			{
				rPrNode = new XElement(XName.Get("strike", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
				rtnode.Add(rPrNode);
			}

			if (rt.Size != int.MinValue)
			{
				rPrNode = new XElement(XName.Get("sz", ExcelCommon.Schema_WorkBook_Main.NamespaceName),
					new XAttribute("val", rt.Size));
				rtnode.Add(rPrNode);
			}

			if (rt.UnderLine)
			{
				rPrNode = new XElement(XName.Get("u", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
				rtnode.Add(rPrNode);
			}

			if (rt.VerticalAlign != ExcelVerticalAlignmentFont.None)
			{
				rPrNode = new XElement(XName.Get("vertAlign", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
				rPrNode.Add(new XAttribute("val", rt.VerticalAlign.ToString().ToLower()));

				rtnode.Add(rPrNode);
			}

			textNode.Add(rtnode);
		}

		#endregion

		#region ”Comments——vmlDrawing.vml“
		internal void WriteCommentsVmlDrawing(CommentCollection sheetComments)
		{
			XDocument vmlDrawingXml = new XDocument();
			XElement vmlDrawingXml_xml = new XElement("xml");
			vmlDrawingXml_xml.Add(new XAttribute(XName.Get("v", XNamespace.Xmlns.NamespaceName), ExcelCommon.schemaMicrosoftVml));
			vmlDrawingXml_xml.Add(new XAttribute(XName.Get("o", XNamespace.Xmlns.NamespaceName), ExcelCommon.schemaMicrosoftOffice));
			vmlDrawingXml_xml.Add(new XAttribute(XName.Get("x", XNamespace.Xmlns.NamespaceName), ExcelCommon.schemaMicrosoftExcel));

			XElement vmlDrawingXml_shapelayout = new XElement(XName.Get("shapelayout", ExcelCommon.schemaMicrosoftOffice),
				new XAttribute(XName.Get("ext", ExcelCommon.schemaMicrosoftVml), "edit"));

			XElement vmlDrawingXml_shapelayout_idmap = new XElement(XName.Get("idmap", ExcelCommon.schemaMicrosoftOffice),
				new XAttribute(XName.Get("ext", ExcelCommon.schemaMicrosoftVml), "edit"),
				new XAttribute("data", 1));

			vmlDrawingXml_shapelayout.Add(vmlDrawingXml_shapelayout_idmap);
			vmlDrawingXml_xml.Add(vmlDrawingXml_shapelayout);

			XElement vmlDrawingXml_shapetype = new XElement(XName.Get("shapetype", ExcelCommon.schemaMicrosoftVml),
				new XAttribute("id", "_x0000_t202"),
				new XAttribute("coordsize", "21600,21600"),
				new XAttribute(XName.Get("spt", ExcelCommon.schemaMicrosoftOffice), 202),
				new XAttribute("path", "m,l,21600r21600,l21600,xe"));

			XElement vmlDrawingXml_shapetype_stroke = new XElement(XName.Get("stroke", ExcelCommon.schemaMicrosoftVml),
				new XAttribute("joinstyle", "miter"));

			vmlDrawingXml_shapetype.Add(vmlDrawingXml_shapetype_stroke);

			XElement vmlDrawingXml_shapetype_path = new XElement(XName.Get("path", ExcelCommon.schemaMicrosoftVml),
				new XAttribute("gradientshapeok", "t"),
				new XAttribute(XName.Get("connecttype", ExcelCommon.schemaMicrosoftOffice), "rect"));
			vmlDrawingXml_shapetype.Add(vmlDrawingXml_shapetype_path);

			vmlDrawingXml_xml.Add(vmlDrawingXml_shapetype);
			if (this.Context.CommentsDrawing.ContainsKey(sheetComments._WorkSheet.Name))
			{
				SortedDictionary<int, SortedDictionary<int, XElement>> shapeDic = this.Context.CommentsDrawing[sheetComments._WorkSheet.Name];

				foreach (KeyValuePair<int, SortedDictionary<int, XElement>> shapeKey in shapeDic)
				{
					foreach (KeyValuePair<int, XElement> itemXmlNode in shapeKey.Value)
					{
						vmlDrawingXml_xml.Add(itemXmlNode.Value);
					}
				}
				shapeDic = null;
				this.Context.CommentsDrawing.Remove(sheetComments._WorkSheet.Name);
			}

			vmlDrawingXml.Add(vmlDrawingXml_xml);

			string relationshipID = this.Context.Package.GetPart(sheetComments._WorkSheet.SheetUri).CreateRelationship(PackUriHelper.GetRelativeUri(sheetComments._WorkSheet.SheetUri, sheetComments.vmlDrawingsUri), TargetMode.Internal, ExcelCommon.Schema_Relationships + "/vmlDrawing").Id;
			PackagePart vmlDrawingsPart = this.Context.Package.CreatePart(sheetComments.vmlDrawingsUri,
			ExcelCommon.ContentType_vmlDrawing, sheetComments._WorkSheet.WorkBook.Compression);

			using (StreamWriter stream = new StreamWriter(vmlDrawingsPart.GetStream(FileMode.Create, FileAccess.Write)))
			{
				stream.Write(vmlDrawingXml.ToString(SaveOptions.None).Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", string.Empty));
				stream.Flush();
			}

			this.Context.CommentsSheetRelationships.Add(sheetComments._WorkSheet.Name, relationshipID);
		}

		private XElement WriteCommentsVmlDrawing_shape(Comment comment, int rowIndex, int columnIndex, int id)
		{
			XElement vmlDrawing_shapeNode = new XElement(XName.Get("shape", ExcelCommon.schemaMicrosoftVml),
				new XAttribute("id", string.Format("shapeDr{0}", id)),
				new XAttribute("type", "#_x0000_t202"));

			if (comment.Visible)
			{
				vmlDrawing_shapeNode.Add(new XAttribute("style", "position:absolute;z-index:1; visibility:visible"));
			}
			else
			{
				vmlDrawing_shapeNode.Add(new XAttribute("style", "position:absolute;z-index:1; visibility:hidden"));
			}

			string color = comment.BackgroundColor.ToArgb().ToString("x");
			if (color.Length > 6)
			{
				color = color.Substring(2, 6);
			}
			if (color.Length < 6)
			{
				color = "ffffe1";
			}
			color = "#" + color;
			vmlDrawing_shapeNode.Add(new XAttribute("fillcolor", color));

			vmlDrawing_shapeNode.Add(new XAttribute(XName.Get("insetmode", ExcelCommon.schemaMicrosoftOffice), "auto"));
			vmlDrawing_shapeNode.Add(new XElement(XName.Get("fill", ExcelCommon.schemaMicrosoftVml),
				new XAttribute("color2", "#ffffe1")));

			if (comment.LineStyle != ExcelLineStyleVml.Solid)
			{
				XElement strokeNode = new XElement(XName.Get("stroke", ExcelCommon.schemaMicrosoftVml));
				if (comment.LineStyle == ExcelLineStyleVml.Round || comment.LineStyle == ExcelLineStyleVml.Square)
				{
					strokeNode.Add(new XAttribute(XName.Get("dashstyle"), "1 1"));
					if (comment.LineStyle == ExcelLineStyleVml.Round)
					{
						strokeNode.Add(new XAttribute(XName.Get("endcap"), "round"));
					}
				}
				else
				{
					string v = comment.LineStyle.ToString();
					v = v.Substring(0, 1).ToLower() + v.Substring(1, v.Length - 1);
					strokeNode.Add(new XAttribute(XName.Get("dashstyle"), v));
				}
				vmlDrawing_shapeNode.Add(strokeNode);
			}

			vmlDrawing_shapeNode.Add(new XElement(XName.Get("shadow", ExcelCommon.schemaMicrosoftVml),
				new XAttribute("on", "t"),
				new XAttribute("color", "black"),
				new XAttribute("obscured", "t")));

			vmlDrawing_shapeNode.Add(new XElement(XName.Get("path", ExcelCommon.schemaMicrosoftVml),
				new XAttribute(XName.Get("connecttype", ExcelCommon.schemaMicrosoftOffice), "none")));

			WriteCommentsVmlDrawing_shape_textbox(vmlDrawing_shapeNode, comment);

			XElement clientDataElement = new XElement(XName.Get("ClientData", ExcelCommon.schemaMicrosoftExcel),
					new XAttribute("ObjectType", "Note"),
					new XElement(XName.Get("MoveWithCells", ExcelCommon.schemaMicrosoftExcel)),
					new XElement(XName.Get("SizeWithCells", ExcelCommon.schemaMicrosoftExcel)));

			WriteCommentsVmlDrawing_shape_Anchor(clientDataElement, comment, rowIndex, columnIndex);

			if (comment.Locked)
			{
				clientDataElement.Add(new XElement(XName.Get("Locked", ExcelCommon.schemaMicrosoftExcel)) { Value = "1" });
			}

			if (comment.AutoFill)
			{
				clientDataElement.Add(new XElement(XName.Get("AutoFill", ExcelCommon.schemaMicrosoftExcel)) { Value = "True" });
			}
			else
			{
				clientDataElement.Add(new XElement(XName.Get("AutoFill", ExcelCommon.schemaMicrosoftExcel)) { Value = "False" });
			}

			if (comment.VerticalAlignment != ExcelTextAlignVerticalVml.Top)
			{
				switch (comment.VerticalAlignment)
				{
					case ExcelTextAlignVerticalVml.Center:
						clientDataElement.Add(new XElement(XName.Get("TextVAlign", ExcelCommon.schemaMicrosoftExcel)) { Value = "Center" });
						break;
					case ExcelTextAlignVerticalVml.Bottom:
						clientDataElement.Add(new XElement(XName.Get("TextVAlign", ExcelCommon.schemaMicrosoftExcel)) { Value = "Bottom" });
						break;
				}
			}

			if (comment.HorizontalAlignment != ExcelTextAlignHorizontalVml.Left)
			{
				switch (comment.HorizontalAlignment)
				{
					case ExcelTextAlignHorizontalVml.Center:
						clientDataElement.Add(new XElement(XName.Get("TextHAlign", ExcelCommon.schemaMicrosoftExcel)) { Value = "Center" });
						break;
					case ExcelTextAlignHorizontalVml.Right:
						clientDataElement.Add(new XElement(XName.Get("TextHAlign", ExcelCommon.schemaMicrosoftExcel)) { Value = "Right" });
						break;
				}
			}
			clientDataElement.Add(new XElement(XName.Get("Row", ExcelCommon.schemaMicrosoftExcel)) { Value = (rowIndex - 1).ToString() });
			clientDataElement.Add(new XElement(XName.Get("Column", ExcelCommon.schemaMicrosoftExcel)) { Value = (columnIndex - 1).ToString() });
			if (comment.Visible)
			{
				clientDataElement.Add(new XElement(XName.Get("Visible", ExcelCommon.schemaMicrosoftExcel)));
			}

			vmlDrawing_shapeNode.Add(clientDataElement);

			return vmlDrawing_shapeNode;

		}

		private void WriteCommentsVmlDrawing_shape_textbox(XElement vmlDrawing_shapeNode, Comment comment)
		{
			XElement textboxNode = new XElement(XName.Get("textbox", ExcelCommon.schemaMicrosoftVml));
			if (comment.AutoFit)
			{
				textboxNode.Add(new XAttribute("style", "mso-direction-alt:auto;mso-fit-shape-to-text:t"));
			}
			else
			{
				textboxNode.Add(new XAttribute("style", "mso-direction-alt:auto"));
			}
			textboxNode.Add(new XElement("div", new XAttribute("style", "text-align:left")));
			vmlDrawing_shapeNode.Add(textboxNode);
		}

		private void WriteCommentsVmlDrawing_shape_Anchor(XElement clientDataElement, Comment comment, int rowIndex, int columnIndex)
		{
			XElement anchorElement = new XElement(XName.Get("Anchor", ExcelCommon.schemaMicrosoftExcel));
			if (!string.IsNullOrEmpty(comment.Anchor))
			{
				anchorElement.Value = comment.Anchor;
			}
			else
			{
				anchorElement.Value = string.Format("{0}, 15, {1}, 2, {2}, 31, {3}, 1", columnIndex, rowIndex - 1, columnIndex + 2, rowIndex + 3);
			}
			clientDataElement.Add(anchorElement);
		}
		#endregion

		#region "HeaderFooterVmlDrawingPicture——vmlDrawing.vml"
		internal void WriteReadHeaderFooterVmlDrawingPicture(VmlDrawingPictureCollection vmlDrawingPictureCollection)
		{
			PackagePart vmlDrawingHeaderFooter = this.Context.Package.CreatePart(vmlDrawingPictureCollection.PictureUri, ExcelCommon.ContentType_vmlDrawing, vmlDrawingPictureCollection._WorkSheet.WorkBook.Compression);

			XDocument vmlDrawingXml = new XDocument();
			XElement vmlDrawingXml_xml = new XElement("xml");
			vmlDrawingXml_xml.Add(new XAttribute(XName.Get("v", XNamespace.Xmlns.NamespaceName), ExcelCommon.schemaMicrosoftVml));
			vmlDrawingXml_xml.Add(new XAttribute(XName.Get("o", XNamespace.Xmlns.NamespaceName), ExcelCommon.schemaMicrosoftOffice));
			vmlDrawingXml_xml.Add(new XAttribute(XName.Get("x", XNamespace.Xmlns.NamespaceName), ExcelCommon.schemaMicrosoftExcel));

			XElement vmlDrawingXml_shapelayout = new XElement(XName.Get("shapelayout", ExcelCommon.schemaMicrosoftOffice),
				new XAttribute(XName.Get("ext", ExcelCommon.schemaMicrosoftVml), "edit"),
				new XElement(XName.Get("idmap", ExcelCommon.schemaMicrosoftOffice), new XAttribute(XName.Get("ext", ExcelCommon.schemaMicrosoftVml), "edit"), new XAttribute(XName.Get("data"), 1)));
			vmlDrawingXml_xml.Add(vmlDrawingXml_shapelayout);

			vmlDrawingXml_xml.Add(new XElement(XName.Get("shapetype", ExcelCommon.schemaMicrosoftVml),
				new XAttribute(XName.Get("id"), "_x0000_t75"),
				new XAttribute(XName.Get("coordsize"), "21600,21600"),
				new XAttribute(XName.Get("filled"), "f"),
				new XAttribute(XName.Get("path"), "m@4@5l@4@11@9@11@9@5xe"),
				new XAttribute(XName.Get("preferrelative", ExcelCommon.schemaMicrosoftOffice), "t"),
				new XAttribute(XName.Get("spt", ExcelCommon.schemaMicrosoftOffice), "75"),
				new XAttribute(XName.Get("stroked"), "f"),

				new XElement(XName.Get("stroke", ExcelCommon.schemaMicrosoftVml), new XAttribute(XName.Get("joinstyle"), "miter")),

				new XElement(XName.Get("formulas", ExcelCommon.schemaMicrosoftVml),
				   new XElement(XName.Get("f", ExcelCommon.schemaMicrosoftVml), new XAttribute(XName.Get("eqn"), "if lineDrawn pixelLineWidth 0")),
				   new XElement(XName.Get("f", ExcelCommon.schemaMicrosoftVml), new XAttribute(XName.Get("eqn"), "sum @0 1 0")),
				   new XElement(XName.Get("f", ExcelCommon.schemaMicrosoftVml), new XAttribute(XName.Get("eqn"), "sum 0 0 @1")),
				   new XElement(XName.Get("f", ExcelCommon.schemaMicrosoftVml), new XAttribute(XName.Get("eqn"), "prod @2 1 2")),
				   new XElement(XName.Get("f", ExcelCommon.schemaMicrosoftVml), new XAttribute(XName.Get("eqn"), "prod @3 21600 pixelWidth")),
				   new XElement(XName.Get("f", ExcelCommon.schemaMicrosoftVml), new XAttribute(XName.Get("eqn"), "prod @3 21600 pixelHeight")),
				   new XElement(XName.Get("f", ExcelCommon.schemaMicrosoftVml), new XAttribute(XName.Get("eqn"), "sum @0 0 1")),
				   new XElement(XName.Get("f", ExcelCommon.schemaMicrosoftVml), new XAttribute(XName.Get("eqn"), "prod @6 1 2")),
				   new XElement(XName.Get("f", ExcelCommon.schemaMicrosoftVml), new XAttribute(XName.Get("eqn"), "prod @7 21600 pixelWidth")),
				   new XElement(XName.Get("f", ExcelCommon.schemaMicrosoftVml), new XAttribute(XName.Get("eqn"), "sum @8 21600 0")),
				   new XElement(XName.Get("f", ExcelCommon.schemaMicrosoftVml), new XAttribute(XName.Get("eqn"), "prod @7 21600 pixelHeight")),
				   new XElement(XName.Get("f", ExcelCommon.schemaMicrosoftVml), new XAttribute(XName.Get("eqn"), "sum @10 21600 0"))),
				new XElement(XName.Get("path", ExcelCommon.schemaMicrosoftVml), new XAttribute(XName.Get("extrusionok", ExcelCommon.schemaMicrosoftOffice), "f"), new XAttribute(XName.Get("gradientshapeok"), "t"), new XAttribute(XName.Get("connecttype", ExcelCommon.schemaMicrosoftOffice), "rect")),
				new XElement(XName.Get("lock", ExcelCommon.schemaMicrosoftOffice), new XAttribute(XName.Get("ext", ExcelCommon.schemaMicrosoftVml), "edit"), new XAttribute(XName.Get("aspectratio"), "t"))
				   ));

			int index = 1;
			foreach (VmlDrawingPicture vdp in vmlDrawingPictureCollection)
			{
				ExcelImageInfo currentImage = SavePicture(vdp.Title, vdp.ImageFormat, vdp.Image, string.Empty);
				string relationship = string.Empty;
				if (this.Context.HashImageRelationships.ContainsKey(currentImage.Hash))
				{
					relationship = this.Context.HashImageRelationships[currentImage.Hash];
				}
				else
				{
					PackageRelationship picRelation = vmlDrawingHeaderFooter.CreateRelationship(PackUriHelper.GetRelativeUri(vmlDrawingPictureCollection.PictureUri, currentImage.Uri), TargetMode.Internal, ExcelCommon.Schema_Relationships + "/image");
					relationship = picRelation.Id;
					this.Context.HashImageRelationships.Add(currentImage.Hash, picRelation.Id);
				}

				XElement shapeNode = new XElement(
					XName.Get("shape", ExcelCommon.schemaMicrosoftVml),
					new XAttribute(XName.Get("id"), vdp.Id),
					new XAttribute(XName.Get("type"), "#_x0000_t75"),
					new XAttribute(XName.Get("style"), vdp.GetStyle(index)));

				XElement imagedataNode = new XElement(XName.Get("imagedata", ExcelCommon.schemaMicrosoftVml),
					new XAttribute(XName.Get("relid", ExcelCommon.schemaMicrosoftOffice), relationship));
				if (string.IsNullOrEmpty(vdp.Title))
					imagedataNode.Add(new XAttribute(XName.Get("title", ExcelCommon.schemaMicrosoftOffice), string.Format("图片{0}", index)));
				else
					imagedataNode.Add(new XAttribute(XName.Get("title", ExcelCommon.schemaMicrosoftOffice), vdp.Title));

				if (vdp.BiLevel)
					imagedataNode.Add(new XAttribute(XName.Get("bilevel"), "t"));

				if (vdp.GrayScale)
					imagedataNode.Add(new XAttribute(XName.Get("grayscale"), "t"));

				if (vdp.Gain != 1)
					imagedataNode.Add(new XAttribute(XName.Get("gain"), vdp.Gain.ToString("#.0#", CultureInfo.InvariantCulture)));

				if (vdp.Gamma != 0)
					imagedataNode.Add(new XAttribute(XName.Get("gamma"), vdp.Gamma.ToString("#.0#", CultureInfo.InvariantCulture)));

				if (vdp.BlackLevel != 0)
					imagedataNode.Add(new XAttribute(XName.Get("blacklevel"), vdp.BlackLevel.ToString("#.0#", CultureInfo.InvariantCulture)));

				shapeNode.Add(imagedataNode);

				shapeNode.Add(new XElement(XName.Get("lock", ExcelCommon.schemaMicrosoftOffice),
					new XAttribute(XName.Get("ext", ExcelCommon.schemaMicrosoftVml), "edit"),
					new XAttribute(XName.Get("rotation"), "t")));

				vmlDrawingXml_xml.Add(shapeNode);
			}

			using (StreamWriter stream = new StreamWriter(vmlDrawingHeaderFooter.GetStream(FileMode.Create, FileAccess.Write)))
			{
				vmlDrawingXml.Add(vmlDrawingXml_xml);
				stream.Write(vmlDrawingXml.ToString(SaveOptions.None).Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", string.Empty));
				stream.Flush();
			}
		}

		#endregion

		private static XElement WriteColor(ColorXmlWrapper color)
		{
			XElement colorXml = null;

			if (color.Rgb.IsNotEmpty() || color.Auto || color.Indexed != int.MinValue || color.Theme.IsNotEmpty())
			{
				colorXml = new XElement(ExcelCommon.Schema_WorkBook_Main + "color");
				if (color.Rgb.IsNotEmpty())
				{
					colorXml.Add(new XAttribute("rgb", color.Rgb));

					if (color.Theme.IsNotEmpty())
					{
						colorXml.Add(new XAttribute("theme", color.Theme));
					}
				}
				else if (color.Indexed != int.MinValue)
				{
					colorXml.Add(new XAttribute("indexed", color.Indexed.ToString(CultureInfo.InvariantCulture)));
				}

				if (color.Tint >= 0)
				{
					colorXml.Add(new XAttribute("tint", color.Tint.ToString(CultureInfo.InvariantCulture)));
				}

				if (color.Auto)
				{
					colorXml.Add(new XAttribute("auto", color.Auto));
				}
			}

			return colorXml;
		}

		private static XElement WriteFont(FontXmlWrapper font)
		{
			XElement fontXml = new XElement(ExcelCommon.Schema_WorkBook_Main + "font");

			if (font.Size > 0)
			{
				XElement szXml = new XElement(ExcelCommon.Schema_WorkBook_Main + "sz");
				szXml.Add(new XAttribute("val", font.Size.ToString(CultureInfo.InvariantCulture)));
				fontXml.Add(szXml);
			}

			if (font._Color != null)
				fontXml.Add(WriteColor(font._Color));

			fontXml.Add(new XElement(ExcelCommon.Schema_WorkBook_Main + "name",
				new XAttribute("val", font.Name.ToString(CultureInfo.InvariantCulture))));

			if (font.Family > int.MinValue)
				fontXml.Add(new XElement(ExcelCommon.Schema_WorkBook_Main + "family",
					new XAttribute("val", font.Family.ToString(CultureInfo.InvariantCulture))));

			if (font.Scheme.IsNotEmpty())
				fontXml.Add(new XElement(ExcelCommon.Schema_WorkBook_Main + "scheme",
					new XAttribute("val", font.Scheme.ToString(CultureInfo.InvariantCulture))));

			if (font.Bold)
				fontXml.Add(new XElement(ExcelCommon.Schema_WorkBook_Main + "b"));

			if (font.Italic)
				fontXml.Add(new XElement(ExcelCommon.Schema_WorkBook_Main + "i"));

			if (font.Strike)
				fontXml.Add(new XElement(ExcelCommon.Schema_WorkBook_Main + "strike"));

			if (font.UnderLine)
				fontXml.Add(new XElement(ExcelCommon.Schema_WorkBook_Main + "u"));

			if (font.VerticalAlign.IsNotEmpty())
				fontXml.Add(new XElement(ExcelCommon.Schema_WorkBook_Main + "vertAlign",
					new XAttribute("val", font.VerticalAlign.ToString(CultureInfo.InvariantCulture))));

			return fontXml;
		}

		private static XElement WriteFill(FillXmlWrapper fill)
		{
			XElement root = new XElement(ExcelCommon.Schema_WorkBook_Main + "fill");

			XElement fillXml = new XElement(ExcelCommon.Schema_WorkBook_Main + "patternFill",
					new XAttribute("patternType", FillXmlWrapper.SetPatternString(fill.PatternType)));

			root.Add(fillXml);

			if (fill.PatternType == ExcelFillStyle.None)
			{
				return root;
			}

			if (fill._PatternColor != null)
			{
				var colorXml = new XElement(ExcelCommon.Schema_WorkBook_Main + "fgColor");

				if (fill._PatternColor.Rgb.IsNotEmpty())
				{
					colorXml.Add(new XAttribute("rgb", fill._PatternColor.Rgb));
				}
				if (fill._PatternColor.Auto)
				{
					colorXml.Add(new XAttribute("auto", fill._PatternColor.Auto));
				}
				if (fill._PatternColor.Tint != 0)
				{
					colorXml.Add(new XAttribute("tint", fill._PatternColor.Tint.ToString(CultureInfo.InvariantCulture)));
				}
				if (fill._PatternColor.Indexed != int.MinValue)
				{
					colorXml.Add(new XAttribute("indexed", fill._PatternColor.Indexed.ToString(CultureInfo.InvariantCulture)));
				}
				if (fill._PatternColor.Theme.IsNotEmpty())
				{
					colorXml.Add(new XAttribute("theme", fill._PatternColor.Theme));
				}
				fillXml.Add(colorXml);
			}

			if (fill._BackgroundColor != null)
			{
				var bgColorXml = new XElement(ExcelCommon.Schema_WorkBook_Main + "bgColor");

				if (fill._BackgroundColor.Rgb.IsNotEmpty())
				{
					bgColorXml.Add(new XAttribute("rgb", fill._BackgroundColor.Rgb));
				}
				if (fill._BackgroundColor.Auto)
				{
					bgColorXml.Add(new XAttribute("auto", "1"));
				}
				if (fill._BackgroundColor.Tint != 0)
				{
					bgColorXml.Add(new XAttribute("tint", fill._BackgroundColor.Tint.ToString(CultureInfo.InvariantCulture)));
				}
				if (fill._BackgroundColor.Indexed != int.MinValue)
				{
					bgColorXml.Add(new XAttribute("indexed", fill._BackgroundColor.Indexed.ToString(CultureInfo.InvariantCulture)));
				}

				if (fill._BackgroundColor.Theme.IsNotEmpty())
				{
					bgColorXml.Add(new XAttribute("theme", fill._BackgroundColor.Theme));
				}

				fillXml.Add(bgColorXml);
			}
			return root;
		}

		private static XElement WriteBorder(BorderXmlWrapper border)
		{
			XElement borderXml = new XElement(ExcelCommon.Schema_WorkBook_Main + "border");

			if (border.DiagonalDown)
				borderXml.Add(new XAttribute("diagonalDown", "1"));

			if (border.DiagonalUp)
				borderXml.Add(new XAttribute("diagonalUp", "1"));
			if (border.Left != null)
			{
				XElement element = new XElement(ExcelCommon.Schema_WorkBook_Main + "left");
				WriteBorderItem(border.Left, element);
				borderXml.Add(element);
			}
			if (border.Right != null)
			{
				XElement element = new XElement(ExcelCommon.Schema_WorkBook_Main + "right");
				WriteBorderItem(border.Right, element);
				borderXml.Add(element);
			}
			if (border.Top != null)
			{
				XElement element = new XElement(ExcelCommon.Schema_WorkBook_Main + "top");
				WriteBorderItem(border.Top, element);
				borderXml.Add(element);
			}
			if (border.Bottom != null)
			{
				XElement element = new XElement(ExcelCommon.Schema_WorkBook_Main + "bottom");
				WriteBorderItem(border.Bottom, element);
				borderXml.Add(element);
			}
			if (border.Diagonal != null)
			{
				XElement element = new XElement(ExcelCommon.Schema_WorkBook_Main + "diagonal");
				WriteBorderItem(border.Diagonal, element);
				borderXml.Add(element);
			}

			return borderXml;
		}

		private static void WriteBorderItem(BorderItemXmlWrapper borderItem, XElement borderXml)
		{
			if (borderItem.Style != ExcelBorderStyle.None)
				borderXml.Add(new XAttribute("style", borderItem.SetBorderString(borderItem.Style)));

			if (borderItem._Color != null)
				borderXml.Add(WriteColor(borderItem._Color));
		}

		private static XElement WriteAttributes(ElementInfo info)
		{
			XElement element = new XElement(ExcelCommon.Schema_WorkBook_Main + info.NodeName);

			if (info.Attributes != null && info.Attributes.Count > 0)
			{
				foreach (KeyValuePair<string, string> keyValue in info.Attributes)
				{
					if (keyValue.Value.IsNotEmpty())
						element.Add(new XAttribute(keyValue.Key, keyValue.Value));
				}
			}
			return element;
		}

		/// <summary>
		/// 返回样式ID
		/// </summary>
		/// <param name="Style"></param>
		/// <returns></returns>
		private string GetStyleId(CellStyleXmlWrapper Style)
		{
			if (this.Context.GlobalStyles.CellXfs.ExistsKey(Style.Id))
			{
				return this.Context.GlobalStyles.CellXfs.FindIndexByID(Style.Id).ToString(CultureInfo.InvariantCulture);
			}
			else
			{
				#region “添加样数字格式化”
				NumberFormatXmlWrapper number = Style.NumberFormat;
				if (!this.Context.GlobalStyles.NumberFormats.FindByID(Style.NumberFormat.Id, ref number))
				{
					number.NumFmtId = this.Context.GlobalStyles.NumberFormats.NextId++;
					this.Context.GlobalStyles.NumberFormats.Add(Style.NumberFormat.Format, Style.NumberFormat);
				}

				Style.NumberFormatId = number.NumFmtId;
				#endregion

				if (this.Context.GlobalStyles.Fonts.ExistsKey(Style.Font.Id) == false)
				{
					this.Context.GlobalStyles.Fonts.Add(Style.Font.Id, Style.Font);
				}
				Style.FontId = this.Context.GlobalStyles.Fonts.FindIndexByID(Style.Font.Id);

				if (this.Context.GlobalStyles.Fills.ExistsKey(Style.Fill.Id) == false)
				{
					this.Context.GlobalStyles.Fills.Add(Style.Fill.Id, Style.Fill);
				}
				Style.FillId = this.Context.GlobalStyles.Fills.FindIndexByID(Style.Fill.Id);

				if (this.Context.GlobalStyles.Borders.ExistsKey(Style.Border.Id) == false)
				{
					Style.ApplyBorder = true;
					this.Context.GlobalStyles.Borders.Add(Style.Border.Id, Style.Border);
				}
				Style.BorderId = this.Context.GlobalStyles.Borders.FindIndexByID(Style.Border.Id);

				this.Context.GlobalStyles.CellXfs.Add(Style.Id, Style);

				return this.Context.GlobalStyles.CellXfs.FindIndexByID(Style.Id).ToString(CultureInfo.InvariantCulture);
			}
		}

		#region  “drawing.xml"
		internal void WriteWrokSheetDrawing(DrawingCollection drawingCollection)
		{
			drawingCollection.DrawingUri = this.Context.Package.GetNewUri("/xl/drawings/drawing{0}.xml");

			PackagePart wrokSheetDrawingPart = this.Context.Package.CreatePart(drawingCollection.DrawingUri, ExcelCommon.ContentType_sheetDrawing, drawingCollection._WorkSheet.WorkBook.Compression);
			PackageRelationship drawingRelationship = this.Context.Package.GetPart(drawingCollection._WorkSheet.SheetUri).CreateRelationship(PackUriHelper.GetRelativeUri(drawingCollection._WorkSheet.SheetUri, drawingCollection.DrawingUri), TargetMode.Internal, ExcelCommon.Schema_Relationships + "/drawing");
			drawingCollection.RelationshipID = drawingRelationship.Id;

			XDocument sheetDrawingDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
			XElement root = new XElement(XName.Get("wsDr", ExcelCommon.Schema_SheetDrawings), new XAttribute(XNamespace.Xmlns + "xdr", ExcelCommon.Schema_SheetDrawings),
				new XAttribute(XNamespace.Xmlns + "a", ExcelCommon.Schema_Drawings));
			WriteWrokSheetDrawing_Content(drawingCollection, root, wrokSheetDrawingPart);
			sheetDrawingDoc.Add(root);

			using (Stream stream = wrokSheetDrawingPart.GetStream(FileMode.Create, FileAccess.Write))
			{
				sheetDrawingDoc.Save(stream);
				stream.Flush();
			}
		}

		private void WriteWrokSheetDrawing_Content(DrawingCollection drawingCollection, XElement root, PackagePart wrokSheetDrawingPart)
		{
			int index = 1;
			foreach (ExcelDrawing drawing in drawingCollection)
			{
				XElement rootNode = new XElement(XName.Get("twoCellAnchor", ExcelCommon.Schema_SheetDrawings));
				if (drawing.EditAs != ExcelEditAs.TwoCell)
				{
					string strDra = drawing.EditAs.ToString();
					if (string.IsNullOrEmpty(strDra) == false)
						rootNode.Add(new XAttribute(XName.Get("editAs"), strDra.Substring(0, 1).ToLower() + strDra.Substring(1, strDra.Length - 1)));
				}

				WriteWrokSheetDrawing_Content_DrawingPosition("from", drawing.From, rootNode);
				WriteWrokSheetDrawing_Content_DrawingPosition("to", drawing.To, rootNode);

				//todo: 图表
				if (drawing is ExcelChart)
					rootNode.Add(WriteWorkSheetDrawing_Content_graphicFrame(drawing as ExcelChart, drawingCollection, wrokSheetDrawingPart, index));
				else if (drawing is ExcelPicture)
					rootNode.Add(WriteWrokSheetDrawing_Content_pic(drawing as ExcelPicture, drawingCollection, wrokSheetDrawingPart, index));

				XElement clientDataNode = new XElement(XName.Get("clientData", ExcelCommon.Schema_SheetDrawings));
				if (drawing.Locked)
					clientDataNode.Add(new XAttribute(XName.Get("fLocksWithSheet"), 1));
				if (drawing.Print)
					clientDataNode.Add(new XAttribute(XName.Get("fPrintsWithSheet"), 1));

				rootNode.Add(clientDataNode);
				root.Add(rootNode);
			}
		}

		private XElement WriteWorkSheetDrawing_Content_graphicFrame(ExcelChart excelchart, DrawingCollection drawingCollection, PackagePart wrokSheetDrawingPart, int index)
		{
			XElement graphicFrameNode = new XElement(XName.Get("graphicFrame", ExcelCommon.Schema_SheetDrawings), new XAttribute(XName.Get("macro"), ""));
			XElement nvGraphicFramePrNode = new XElement(XName.Get("nvGraphicFramePr", ExcelCommon.Schema_SheetDrawings));
			XElement cNvPrNode = new XElement(XName.Get("cNvPr", ExcelCommon.Schema_SheetDrawings),
				new XAttribute(XName.Get("id"), index + 2), new XAttribute(XName.Get("name"), excelchart.Name));
			nvGraphicFramePrNode.Add(cNvPrNode);
			nvGraphicFramePrNode.Add(new XElement(XName.Get("cNvGraphicFramePr", ExcelCommon.Schema_SheetDrawings)));
			graphicFrameNode.Add(nvGraphicFramePrNode);

			XElement xfrmElement = new XElement(XName.Get("xfrm", ExcelCommon.Schema_SheetDrawings),
				new XElement(XName.Get("off", ExcelCommon.Schema_Drawings), new XAttribute(XName.Get("x"), 0), new XAttribute(XName.Get("y"), 0)),
				new XElement(XName.Get("ext", ExcelCommon.Schema_Drawings), new XAttribute(XName.Get("cx"), 0), new XAttribute(XName.Get("cy"), 0)));
			graphicFrameNode.Add(xfrmElement);

			XElement graphicElement = new XElement(XName.Get("graphic", ExcelCommon.Schema_Drawings));
			XElement graphicDataElement = new XElement(XName.Get("graphicData", ExcelCommon.Schema_Drawings), new XAttribute(XName.Get("uri"), ExcelCommon.Schema_Chart));
			XElement chartElement = new XElement(XName.Get("chart", ExcelCommon.Schema_Chart), new XAttribute(XNamespace.Xmlns + "c", ExcelCommon.Schema_Chart),
				 new XAttribute(XNamespace.Xmlns + "r", ExcelCommon.Schema_Relationships));
			string chartRelationID = CreateChartPackage(excelchart, wrokSheetDrawingPart, drawingCollection.DrawingUri);
			chartElement.Add(new XAttribute(XName.Get("id", ExcelCommon.Schema_Relationships), chartRelationID));
			graphicDataElement.Add(chartElement);
			graphicElement.Add(graphicDataElement);
			//graphicElement.Add(new XElement(XName.Get("clientData", ExcelCommon.Schema_SheetDrawings)));

			graphicFrameNode.Add(graphicElement);

			return graphicFrameNode;
		}

		private string CreateChartPackage(ExcelChart excelchart, PackagePart wrokSheetDrawingPart, Uri drawingUri)
		{
			Uri uriChart = this.Context.Package.GetNewUri("/xl/charts/chart{0}.xml");
			PackageRelationship chartRelation = wrokSheetDrawingPart.CreateRelationship(PackUriHelper.GetRelativeUri(drawingUri, uriChart), TargetMode.Internal, ExcelCommon.Schema_Relationships + "/chart");
			PackagePart chartPackagePart = this.Context.Package.CreatePart(uriChart, ExcelCommon.ContentType_sheetChart, CompressionOption.NotCompressed);

			XDocument chartDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
			WriteWrokSheetChart(excelchart, chartDoc);
			using (Stream chartStream = chartPackagePart.GetStream(FileMode.Create, FileAccess.Write))
			{
				chartDoc.Save(chartStream);
				chartStream.Flush();
			}
			return chartRelation.Id;
		}

		private XElement WriteWrokSheetDrawing_Content_pic(ExcelPicture pic, DrawingCollection drawingCollection, PackagePart wrokSheetDrawingPart, int index)
		{
			XElement picNode = new XElement(XName.Get("pic", ExcelCommon.Schema_SheetDrawings));

			#region "nvPicPr"
			XElement nvPicPrNode = new XElement(XName.Get("nvPicPr", ExcelCommon.Schema_SheetDrawings));
			XElement cNvPrNode = new XElement(XName.Get("cNvPr", ExcelCommon.Schema_SheetDrawings),
				new XAttribute(XName.Get("id"), (index + 2).ToString()));
			if (string.IsNullOrEmpty(pic.Name))
				cNvPrNode.Add(new XAttribute(XName.Get("name"), string.Format("图片 {0}", index)));
			else
				cNvPrNode.Add(new XAttribute(XName.Get("name"), pic.Name));

			nvPicPrNode.Add(cNvPrNode);

			XElement cNvPicPrNode = new XElement(XName.Get("cNvPicPr", ExcelCommon.Schema_SheetDrawings));
			cNvPicPrNode.Add(new XElement(XName.Get("picLocks", ExcelCommon.Schema_Drawings), new XAttribute("noChangeAspect", 1)));
			nvPicPrNode.Add(cNvPicPrNode);
			picNode.Add(nvPicPrNode);
			#endregion "nvPicPr"

			#region "blipFill"
			XElement blipFillNode = new XElement(XName.Get("blipFill", ExcelCommon.Schema_SheetDrawings));
			XElement blipNode = new XElement(XName.Get("blip", ExcelCommon.Schema_Drawings));
			blipNode.Add(new XAttribute(XName.Get("R", XNamespace.Xmlns.NamespaceName), ExcelCommon.Schema_Relationships));

			ExcelImageInfo currentImage = SavePicture(pic.Name, pic.ImageFormat, pic.Image, pic.ContentType);
			if (this.Context.HashImageRelationships.ContainsKey(currentImage.Hash))
				blipNode.Add(new XAttribute(XName.Get("embed", ExcelCommon.Schema_Relationships), this.Context.HashImageRelationships[currentImage.Hash]));
			else
			{
				PackageRelationship picRelation = wrokSheetDrawingPart.CreateRelationship(PackUriHelper.GetRelativeUri(drawingCollection.DrawingUri, currentImage.Uri), TargetMode.Internal, ExcelCommon.Schema_Relationships + "/image");
				blipNode.Add(new XAttribute(XName.Get("embed", ExcelCommon.Schema_Relationships), picRelation.Id));
				this.Context.HashImageRelationships.Add(currentImage.Hash, picRelation.Id);
			}
			blipNode.Add(new XAttribute(XName.Get("cstate"), "print"));
			blipFillNode.Add(blipNode);

			XElement stretchNode = new XElement(XName.Get("stretch", ExcelCommon.Schema_Drawings),
				   new XElement(XName.Get("fillRect", ExcelCommon.Schema_Drawings)));
			blipFillNode.Add(stretchNode);
			picNode.Add(blipFillNode);
			#endregion

			#region "spPr"
			XElement spPrNode = new XElement(XName.Get("spPr", ExcelCommon.Schema_SheetDrawings),
				new XElement(XName.Get("xfrm", ExcelCommon.Schema_Drawings),
					new XElement(XName.Get("off", ExcelCommon.Schema_Drawings),
						new XAttribute("x", 0), new XAttribute("y", 0)),
					new XElement(XName.Get("ext", ExcelCommon.Schema_Drawings),
						new XAttribute("cx", 0), new XAttribute("cy", 0))),
				new XElement(XName.Get("prstGeom", ExcelCommon.Schema_Drawings),
					new XAttribute("prst", "rect"),
					new XElement(XName.Get("avLst", ExcelCommon.Schema_Drawings))));
			if (pic._Fill != null)
				WriteWrokSheetDrawing_pic_solidFill(pic._Fill, spPrNode);

			if (pic._Border != null)
				WriteWrokSheetDrawing_pic_ln(pic._Border, spPrNode);

			picNode.Add(spPrNode);
			#endregion

			return picNode;
		}

		private void WriteWrokSheetDrawing_Content_DrawingPosition(string tag, DrawingPosition item, XElement rootNode)
		{
			XElement fromNode = new XElement(XName.Get(tag, ExcelCommon.Schema_SheetDrawings));
			fromNode.Add(new XElement(XName.Get("col", ExcelCommon.Schema_SheetDrawings)) { Value = item.Column.ToString() });
			fromNode.Add(new XElement(XName.Get("colOff", ExcelCommon.Schema_SheetDrawings)) { Value = item.ColumnOff.ToString() });
			fromNode.Add(new XElement(XName.Get("row", ExcelCommon.Schema_SheetDrawings)) { Value = item.Row.ToString() });
			fromNode.Add(new XElement(XName.Get("rowOff", ExcelCommon.Schema_SheetDrawings)) { Value = item.RowOff.ToString() });
			rootNode.Add(fromNode);
		}

		private void WriteWrokSheetDrawing_pic_ln(DrawingBorder drawingBorder, XElement spPrNode)
		{
			XElement lnNode = new XElement(XName.Get("ln", ExcelCommon.Schema_Drawings));
			if (drawingBorder.Width != default(int))
				lnNode.Add(new XAttribute(XName.Get("w"), drawingBorder.Width * 12700));

			if (drawingBorder.LineCap != default(ExcelDrawingLineCap))
				lnNode.Add(new XAttribute(XName.Get("cap"), TranslateLineCapText(drawingBorder.LineCap)));

			if (drawingBorder._Fill != null)
			{
				WriteWrokSheetDrawing_pic_solidFill(drawingBorder._Fill, lnNode);

				if (drawingBorder.LineStyle != default(ExcelDrawingLineStyle))
					lnNode.Add(new XElement(XName.Get("prstDash", ExcelCommon.Schema_Drawings), new XAttribute(XName.Get("val"), TranslateLineStyleText(drawingBorder.LineStyle))));
			}
			spPrNode.Add(lnNode);
		}

		private void WriteWrokSheetDrawing_pic_solidFill(DrawingFill fill, XElement parentNode)
		{
			if (fill.FillStyle == ExcelDrawingFillStyle.SolidFill)
			{
				XElement solidFillNode = new XElement(XName.Get("solidFill", ExcelCommon.Schema_Drawings));
				if (fill.Color != Color.FromArgb(79, 129, 189))
				{
					string rgb = fill.Color.ToArgb().ToString("X");
					if (rgb.Length > 6)
						rgb = rgb.Substring(2, 6);

					if (rgb.Length < 6)
						rgb = rgb.PadLeft(6, '0');

					XElement srgbClrNode = new XElement(XName.Get("srgbClr", ExcelCommon.Schema_Drawings), new XAttribute(XName.Get("val"), rgb));
					srgbClrNode.Add(new XElement(XName.Get("alpha", ExcelCommon.Schema_Drawings), new XAttribute(XName.Get("val"), ((100 - fill.Transparancy) * 1000).ToString())));
					solidFillNode.Add(srgbClrNode);
				}
				parentNode.Add(solidFillNode);
			}
			else
				parentNode.Add(new XElement(XName.Get("noFill", ExcelCommon.Schema_Drawings)));
		}

		private ExcelImageInfo SavePicture(string name, ImageFormat imageFormat, Image image, string contentType)
		{
			ImageConverter ic = new ImageConverter();
			byte[] img = (byte[])ic.ConvertTo(image, typeof(byte[]));
			SHA1CryptoServiceProvider hashProvider = new SHA1CryptoServiceProvider();
			string hash = BitConverter.ToString(hashProvider.ComputeHash(img)).Replace("-", "");
			ExcelImageInfo excelImageInfo;
			if (this.Context.DrawingImages.ContainsKey(hash) == false)
			{
				Uri imagUri = CreateImageUri(name, imageFormat);

				if (string.IsNullOrEmpty(contentType))
					contentType = ExcelHelper.GetContentType(string.Format(".{0}", imageFormat.ToString()));

				PackagePart imagPackagePart = this.Context.Package.CreatePart(imagUri, contentType, CompressionOption.NotCompressed);
				using (Stream imagStream = imagPackagePart.GetStream(FileMode.Create, FileAccess.Write))
				{
					imagStream.Write(img, 0, img.GetLength(0));
				}
				excelImageInfo = new ExcelImageInfo() { Hash = hash, RefCount = 1, Uri = imagUri };
				this.Context.DrawingImages.Add(hash, excelImageInfo);
			}
			else
			{
				excelImageInfo = this.Context.DrawingImages[hash];
				excelImageInfo.RefCount++;
			}

			return excelImageInfo;
		}

		internal Uri CreateImageUri(string name, ImageFormat imageFormat)
		{
			Uri imagUri;
			if (!string.IsNullOrEmpty(name) && StringHelper.HasPattern("^[A-Za-z0-9]+$", name))
			{
				imagUri = new Uri(string.Format("/xl/media/{0}.{1}", name, imageFormat.ToString()), UriKind.Relative);
				if (this.Context.Package.PartExists(imagUri))
					imagUri = this.Context.Package.GetNewUri("/xl/media/" + name + "{0}." + imageFormat.ToString());
			}
			else
				imagUri = this.Context.Package.GetNewUri("/xl/media/image{0}." + imageFormat.ToString());

			return imagUri;
		}

		#endregion

		#region "chart.Xml"

		public void WriteWrokSheetChart(ExcelChart excelChart, XDocument chartDoc)
		{
			XElement root = new XElement(XName.Get("chartSpace", ExcelCommon.Schema_Chart), new XAttribute(XNamespace.Xmlns + "c", ExcelCommon.Schema_Chart),
			new XAttribute(XNamespace.Xmlns + "a", ExcelCommon.Schema_Drawings), new XAttribute(XNamespace.Xmlns + "r", ExcelCommon.Schema_Relationships));

			XElement dateElement = new XElement(XName.Get("date1904", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), 0));
			root.Add(dateElement);

			XElement langElement = new XElement(XName.Get("lang", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), "zh-CN"));
			root.Add(langElement);

			XElement roundedCornersElement = new XElement(XName.Get("roundedCorners", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), excelChart.RoundedCorners ? "1" : "0"));
			root.Add(roundedCornersElement);

			if (excelChart.Style != ExcelChartStyle.None)
			{
				XElement styleElement = new XElement(XName.Get("style", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), (int)excelChart.Style));
				root.Add(styleElement);
			}

			WriteWorkSheetChart_chart(excelChart, root);

			WriteWorkSheetChart_printSettings(excelChart, root);

			chartDoc.Add(root);

			/*
		 <c:plotArea>
			 <c:layout />
			 <c:pieChart>
				 <c:varyColors val="1" />
				 <c:ser>
					 <c:idx val="0" />
					 <c:order val="0" />
					 <c:tx>
						 <c:strRef>
							 <c:f></c:f>
							 <c:strCache>
								 <c:ptCount val="1" />
							 </c:strCache>
						 </c:strRef>
					 </c:tx>
					 <c:cat>
						 <c:numRef>
							 <c:f>PieChart!$U$19:$U$24</c:f>
							 <c:numCache>
								 <c:formatCode>yyyy\-mm\-dd</c:formatCode>
								 <c:ptCount val="6" />
								 <c:pt idx="0">
									 <c:v>40178</c:v>
								 </c:pt>
								 <c:pt idx="1">
									 <c:v>40179</c:v>
								 </c:pt>
								 <c:pt idx="2">
									 <c:v>40180</c:v>
								 </c:pt>
								 <c:pt idx="3">
									 <c:v>40181</c:v>
								 </c:pt>
								 <c:pt idx="4">
									 <c:v>40182</c:v>
								 </c:pt>
								 <c:pt idx="5">
									 <c:v>40183</c:v>
								 </c:pt>
							 </c:numCache>
						 </c:numRef>
					 </c:cat>
					 <c:val>
						 <c:numRef>
							 <c:f>PieChart!$V$19:$V$24</c:f>
							 <c:numCache>
								 <c:formatCode>General</c:formatCode>
								 <c:ptCount val="6" />
								 <c:pt idx="0">
									 <c:v>100</c:v>
								 </c:pt>
								 <c:pt idx="1">
									 <c:v>102</c:v>
								 </c:pt>
								 <c:pt idx="2">
									 <c:v>101</c:v>
								 </c:pt>
								 <c:pt idx="3">
									 <c:v>103</c:v>
								 </c:pt>
								 <c:pt idx="4">
									 <c:v>105</c:v>
								 </c:pt>
								 <c:pt idx="5">
									 <c:v>104</c:v>
								 </c:pt>
							 </c:numCache>
						 </c:numRef>
					 </c:val>
				 </c:ser>
				 <c:dLbls>
					 <c:showLegendKey val="0" />
					 <c:showVal val="0" />
					 <c:showCatName val="0" />
					 <c:showSerName val="0" />
					 <c:showPercent val="1" />
					 <c:showBubbleSize val="0" />
					 <c:separator></c:separator>
					 <c:showLeaderLines val="0" />
				 </c:dLbls>
				 <c:firstSliceAng val="0" />
			 </c:pieChart>
		 </c:plotArea>
		 <c:legend>
			 <c:legendPos val="l" />
			 <c:layout />
			 <c:overlay val="0" />
			 <c:txPr>
				 <a:bodyPr />
				 <a:lstStyle />
				 <a:p>
					 <a:pPr>
						 <a:defRPr>
							 <a:solidFill>
								 <a:srgbClr val="4682B4" />
							 </a:solidFill>
						 </a:defRPr>
					 </a:pPr>
					 <a:endParaRPr lang="zh-CN" />
				 </a:p>
			 </c:txPr>
		 </c:legend>
		 <c:plotVisOnly val="1" />
		 <c:dispBlanksAs val="zero" />
		 <c:showDLblsOverMax val="1" />
	 </c:chart>
 </c:chartSpace> */
		}

		private void WriteWorkSheetChart_printSettings(ExcelChart excelChart, XElement root)
		{
			XElement printSettingsElement = new XElement(XName.Get("printSettings", ExcelCommon.Schema_Chart));

			XElement headerFooterElement = new XElement(XName.Get("headerFooter", ExcelCommon.Schema_Chart));
			printSettingsElement.Add(headerFooterElement);

			XElement pageMarginsElement = new XElement(XName.Get("pageMargins", ExcelCommon.Schema_Chart));
			pageMarginsElement.Add(new XAttribute(XName.Get("b"), "0.75"));
			pageMarginsElement.Add(new XAttribute(XName.Get("l"), "0.7"));
			pageMarginsElement.Add(new XAttribute(XName.Get("r"), "0.7"));
			pageMarginsElement.Add(new XAttribute(XName.Get("t"), "0.75"));
			pageMarginsElement.Add(new XAttribute(XName.Get("header"), "0.3"));
			pageMarginsElement.Add(new XAttribute(XName.Get("footer"), "0.3"));
			printSettingsElement.Add(pageMarginsElement);

			XElement pageSetupElement = new XElement(XName.Get("pageSetup", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("orientation"), "portrait"));
			printSettingsElement.Add(pageSetupElement);

			root.Add(printSettingsElement);
		}

		private void WriteWorkSheetChart_chart_Ax(ExcelChart excelChart, XElement plotAreaElement)
		{
			bool isTypePieDoughnut = false, isTypeScatter = false;
			switch (excelChart.ChartType)
			{
				case ExcelChartType.Pie:
				case ExcelChartType.PieExploded:
				case ExcelChartType.PieOfPie:
				case ExcelChartType.Pie3D:
				case ExcelChartType.PieExploded3D:
				case ExcelChartType.BarOfPie:
				case ExcelChartType.Doughnut:
				case ExcelChartType.DoughnutExploded:
					isTypePieDoughnut = true;
					break;
				case ExcelChartType.XYScatter:
				case ExcelChartType.XYScatterLines:
				case ExcelChartType.XYScatterLinesNoMarkers:
				case ExcelChartType.XYScatterSmooth:
				case ExcelChartType.XYScatterSmoothNoMarkers:
					isTypeScatter = true;
					break;
			}

			if (isTypePieDoughnut == false)
			{
				if (isTypeScatter)
				{
					XElement valAxElement = new XElement(XName.Get("valAx", ExcelCommon.Schema_Chart));
					WriteWorkSheetChart_chart_Ax_Content(excelChart, valAxElement);
					plotAreaElement.Add(valAxElement);
				}
				else
				{
					XElement catAxElement = new XElement(XName.Get("catAx", ExcelCommon.Schema_Chart));
					WriteWorkSheetChart_chart_Ax_Content(excelChart, catAxElement);
					WriteWorkSheetChart_chart_catAx_Content(excelChart, catAxElement);
					plotAreaElement.Add(catAxElement);
				}

				WriteWorkSheetChart_chart_valAx(excelChart, plotAreaElement);
			}
		}

		private void WriteWorkSheetChart_chart_valAx(ExcelChart excelChart, XElement plotAreaElement)
		{
			XElement valAxElement = new XElement(XName.Get("valAx", ExcelCommon.Schema_Chart));
			WriteWorkSheetChart_chart_valAx_Content(excelChart, valAxElement);

			plotAreaElement.Add(valAxElement);
		}

		private void WriteWorkSheetChart_chart_valAx_Content(ExcelChart excelChart, XElement plotAreaElement)
		{
			XElement axIdElement = new XElement(XName.Get("axId", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), 2));
			plotAreaElement.Add(axIdElement);

			XElement scalingElement = new XElement(XName.Get("scaling", ExcelCommon.Schema_Chart), new XElement(XName.Get("orientation", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), "minMax")));
			plotAreaElement.Add(scalingElement);

			XElement deleteElement = new XElement(XName.Get("delete", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), 0));
			plotAreaElement.Add(deleteElement);

			XElement axPosElement = new XElement(XName.Get("axPos", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), "l"));
			plotAreaElement.Add(axPosElement);

			XElement majorGridlinesElement = new XElement(XName.Get("majorGridlines", ExcelCommon.Schema_Chart));
			plotAreaElement.Add(majorGridlinesElement);

			XElement tickLblPosElement = new XElement(XName.Get("tickLblPos", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), "nextTo"));
			plotAreaElement.Add(tickLblPosElement);

			XElement crossAxElement = new XElement(XName.Get("crossAx", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), "1"));
			plotAreaElement.Add(crossAxElement);

			XElement crossesElement = new XElement(XName.Get("crosses", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), "autoZero"));
			plotAreaElement.Add(crossesElement);

			XElement crossBetweenElement = new XElement(XName.Get("crossBetween", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), "between"));
			plotAreaElement.Add(crossBetweenElement);
		}

		private void WriteWorkSheetChart_chart_catAx_Content(ExcelChart excelChart, XElement catAxElement)
		{
			XElement autoElement = new XElement(XName.Get("auto", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), 1));
			catAxElement.Add(autoElement);

			XElement lblAlgnElement = new XElement(XName.Get("lblAlgn", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), "ctr"));
			catAxElement.Add(lblAlgnElement);

			XElement lblOffsetElement = new XElement(XName.Get("lblOffset", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), "100"));
			catAxElement.Add(lblOffsetElement);
		}

		private void WriteWorkSheetChart_chart_Ax_Content(ExcelChart excelChart, XElement parentElement)
		{
			XElement axIdElement = new XElement(XName.Get("axId", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), 1));
			parentElement.Add(axIdElement);

			XElement scalingElement = new XElement(XName.Get("scaling", ExcelCommon.Schema_Chart), new XElement(XName.Get("orientation", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), "minMax")));
			parentElement.Add(scalingElement);

			XElement deleteElement = new XElement(XName.Get("delete", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), 0));
			parentElement.Add(deleteElement);

			XElement axPosElement = new XElement(XName.Get("axPos", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), "b"));
			parentElement.Add(axPosElement);

			XElement tickLblPosElement = new XElement(XName.Get("tickLblPos", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), "nextTo"));
			parentElement.Add(tickLblPosElement);

			XElement crossAxElement = new XElement(XName.Get("crossAx", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), 2));
			parentElement.Add(crossAxElement);

			XElement crossesElement = new XElement(XName.Get("crosses", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), "autoZero"));
			parentElement.Add(crossesElement);
		}

		private void WriteWorkSheetChart_chart_plotArea(ExcelChart excelChart, XElement root)
		{
			XElement plotAreaElement = new XElement(XName.Get("plotArea", ExcelCommon.Schema_Chart));
			XElement layoutElement = new XElement(XName.Get("layout", ExcelCommon.Schema_Chart));
			plotAreaElement.Add(layoutElement);

			if (excelChart.IsType3D())
			{
				XElement view3DElement = new XElement(XName.Get("view3D", ExcelCommon.Schema_Chart));
				view3DElement.Add(new XElement(XName.Get("perspective", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), 30)));
				plotAreaElement.Add(view3DElement);
			}

			WriteWorkSheetChart_chart_chartType(excelChart, plotAreaElement);
			WriteWorkSheetChart_chart_Ax(excelChart, plotAreaElement);

			root.Add(plotAreaElement);
		}

		private void WriteWorkSheetChart_chart_chartType(ExcelChart excelChart, XElement plotAreaElement)
		{
			XElement chartTypeElement = new XElement(XName.Get(GetChartNodeTextByChartType(excelChart.ChartType), ExcelCommon.Schema_Chart));

			WriteWorkSheetChart_chart_plotArea_scatterStyle(excelChart, chartTypeElement);
			WriteWorkSheetChart_chart_plotArea_barDir(excelChart, chartTypeElement);
			WriteWorkSheetChart_chart_plotArea_grouping(excelChart, chartTypeElement);
			WriteWorkSheetChart_chart_plotArea_marker(excelChart, chartTypeElement);
			WriteWorkSheetChart_chart_varyColors(excelChart, chartTypeElement);
			WriteWorkSheetChart_chart_plotArea_shape(excelChart, chartTypeElement);
			WriteWorkSheetChart_chart_plotArea_firstSliceAng(excelChart, chartTypeElement);
			WriteWorkSheetChart_chart_plotArea_holeSize(excelChart, chartTypeElement);
			WriteWorkSheetChart_chart_plotArea_overlap(excelChart, chartTypeElement);
			WriteWorkSheetChart_chart_plotArea_axId(excelChart, chartTypeElement);

			switch (excelChart.ChartType)
			{
				case ExcelChartType.PieOfPie:
				case ExcelChartType.BarOfPie:
				case ExcelChartType.Pie:
				case ExcelChartType.Pie3D:


					WriteWorkSheetChart_chart_dLbls((ExcelPieChart)excelChart, chartTypeElement);
					WriteWorkSheetChart_chart_ser((ExcelPieChart)excelChart, chartTypeElement);



					break;

			}

			plotAreaElement.Add(chartTypeElement);
		}

		private void WriteWorkSheetChart_chart_varyColors(ExcelChart excelChart, XElement chartTypeElement)
		{
			chartTypeElement.Add(new XElement(XName.Get("varyColors", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), excelChart.VaryColors ? "1" : "0")));
		}



		private void WriteWorkSheetChart_chart_ser(ExcelPieChart excelPieChart, XElement chartTypeElement)
		{

			XElement serElement = new XElement(XName.Get("ser", ExcelCommon.Schema_Chart));
			WriteWorkSheetChart_chart_cat(excelPieChart, serElement);
			WriteWorkSheetChart_chart_val(excelPieChart, serElement);
			chartTypeElement.Add(serElement);
		}

		private void WriteWorkSheetChart_chart_dLbls(ExcelPieChart excelPieChart, XElement chartTypeElement)
		{
			if (excelPieChart._DataLabel != null)
			{
				XElement dLblsElement = new XElement(XName.Get("dLbls", ExcelCommon.Schema_Chart));
				dLblsElement.Add(new XElement(XName.Get("showLegendKey", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), excelPieChart._DataLabel.ShowLegendKey ? "1" : "0")));
				dLblsElement.Add(new XElement(XName.Get("showVal", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), excelPieChart._DataLabel.ShowValue ? "1" : "0")));
				dLblsElement.Add(new XElement(XName.Get("showCatName", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), excelPieChart._DataLabel.ShowCategory ? "1" : "0")));
				dLblsElement.Add(new XElement(XName.Get("showSerName", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), excelPieChart._DataLabel.ShowSeriesName ? "1" : "0")));
				dLblsElement.Add(new XElement(XName.Get("showPercent", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), excelPieChart._DataLabel.ShowPercent ? "1" : "0")));
				dLblsElement.Add(new XElement(XName.Get("showBubbleSize", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), excelPieChart._DataLabel.ShowBubbleSize ? "1" : "0")));
				if (string.IsNullOrEmpty(excelPieChart._DataLabel.Separator) == false)
				{
					XElement separatorElement = new XElement(XName.Get("separator", ExcelCommon.Schema_Chart));
					separatorElement.Value = excelPieChart._DataLabel.Separator;
					dLblsElement.Add(separatorElement);
				}

				dLblsElement.Add(new XElement(XName.Get("showLeaderLines", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), excelPieChart._DataLabel.ShowLeaderLines ? "1" : "0")));
				chartTypeElement.Add(dLblsElement);
			}
		}

		private void WriteWorkSheetChart_chart_cat(ExcelPieChart excelPieChart, XElement chartTypeElement)
		{
			if (excelPieChart.CategoryAxisData != null)
			{
				XElement catElement = new XElement(XName.Get("cat", ExcelCommon.Schema_Chart));
				XElement strRefElement = new XElement(XName.Get("strRef", ExcelCommon.Schema_Chart));
				XElement fElement = new XElement(XName.Get("f", ExcelCommon.Schema_Chart));
				fElement.Value = excelPieChart.CategoryAxisData.StringReference.Formula;
				strRefElement.Add(fElement);
				catElement.Add(strRefElement);



				XElement strCacheElement = new XElement(XName.Get("strCache", ExcelCommon.Schema_Chart));
				XElement ptCountElement = new XElement(XName.Get("ptCount", ExcelCommon.Schema_Chart));
				ptCountElement.Add(new XAttribute(XName.Get("val"), excelPieChart.CategoryAxisData.StringReference.StringPointCollection.Count));
				strCacheElement.Add(ptCountElement);
				foreach (var item in excelPieChart.CategoryAxisData.StringReference.StringPointCollection)
				{
					XElement ptElement = new XElement(XName.Get("pt", ExcelCommon.Schema_Chart));

					XAttribute ptAttribute = new XAttribute(XName.Get("idx"), item.PointIndex);
					ptElement.Add(ptAttribute);

					XElement vElement = new XElement(XName.Get("v", ExcelCommon.Schema_Chart));
					vElement.Value = item.PointValue;
					ptElement.Add(vElement);
					strCacheElement.Add(ptElement);
				}
				strRefElement.Add(strCacheElement);

				chartTypeElement.Add(catElement);
			}
		}

		private void WriteWorkSheetChart_chart_val(ExcelPieChart excelPieChart, XElement chartTypeElement)
		{
			if (excelPieChart.CategoryAxisValue != null)
			{
				XElement valElement = new XElement(XName.Get("val", ExcelCommon.Schema_Chart));
				XElement numRefElement = new XElement(XName.Get("numRef", ExcelCommon.Schema_Chart));
				XElement fElement = new XElement(XName.Get("f", ExcelCommon.Schema_Chart));
				fElement.Value = excelPieChart.CategoryAxisValue.NumberReference.Formula;
				numRefElement.Add(fElement);
				valElement.Add(numRefElement);



				XElement numCacheElement = new XElement(XName.Get("numCache", ExcelCommon.Schema_Chart));
				XElement ptCountElement = new XElement(XName.Get("ptCount", ExcelCommon.Schema_Chart));
				ptCountElement.Add(new XAttribute(XName.Get("val"), excelPieChart.CategoryAxisData.StringReference.StringPointCollection.Count));
				numCacheElement.Add(ptCountElement);
				foreach (var item in excelPieChart.CategoryAxisValue.NumberReference.NumericPointCollection)
				{
					XElement ptElement = new XElement(XName.Get("pt", ExcelCommon.Schema_Chart));

					XAttribute ptAttribute = new XAttribute(XName.Get("idx"), item.PointIndex);
					ptElement.Add(ptAttribute);

					XElement vElement = new XElement(XName.Get("v", ExcelCommon.Schema_Chart));
					vElement.Value = item.PointValue;
					ptElement.Add(vElement);
					numCacheElement.Add(ptElement);
				}
				numRefElement.Add(numCacheElement);

				chartTypeElement.Add(valElement);
			}
		}


		private void WriteWorkSheetChart_chart_plotArea_axId(ExcelChart excelChart, XElement chartTypeElement)
		{
			switch (excelChart.ChartType)
			{
				case ExcelChartType.Pie:
				case ExcelChartType.PieExploded:
				case ExcelChartType.PieOfPie:
				case ExcelChartType.Pie3D:
				case ExcelChartType.PieExploded3D:
				case ExcelChartType.BarOfPie:
				case ExcelChartType.Doughnut:
				case ExcelChartType.DoughnutExploded:
					break;
				default:
					chartTypeElement.Add(new XElement(XName.Get("axId", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), 1)));
					chartTypeElement.Add(new XElement(XName.Get("axId", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), 2)));
					break;
			}
		}

		private void WriteWorkSheetChart_chart_plotArea_overlap(ExcelChart excelChart, XElement chartTypeElement)
		{
			if (excelChart.ChartType == ExcelChartType.BarStacked100 ||
						excelChart.ChartType == ExcelChartType.BarStacked ||
						excelChart.ChartType == ExcelChartType.ColumnStacked ||
						excelChart.ChartType == ExcelChartType.ColumnStacked100)
			{
				chartTypeElement.Add(new XElement(XName.Get("overlap", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), 100)));
			}
		}

		private void WriteWorkSheetChart_chart_plotArea_holeSize(ExcelChart excelChart, XElement chartTypeElement)
		{
			if (excelChart.ChartType == ExcelChartType.Doughnut ||
				excelChart.ChartType == ExcelChartType.DoughnutExploded)
			{
				chartTypeElement.Add(new XElement(XName.Get("holeSize", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), 50)));
			}
		}

		private void WriteWorkSheetChart_chart_plotArea_firstSliceAng(ExcelChart excelChart, XElement chartTypeElement)
		{
			if (excelChart.ChartType == ExcelChartType.Doughnut ||
				excelChart.ChartType == ExcelChartType.DoughnutExploded)
			{
				chartTypeElement.Add(new XElement(XName.Get("firstSliceAng", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), 0)));
			}
		}

		private void WriteWorkSheetChart_chart_plotArea_shape(ExcelChart excelChart, XElement chartTypeElement)
		{
			switch (excelChart.ChartType)
			{
				case ExcelChartType.BarClustered3D:
				case ExcelChartType.BarStacked3D:
				case ExcelChartType.BarStacked1003D:
				case ExcelChartType.Column3D:
				case ExcelChartType.ColumnClustered3D:
				case ExcelChartType.ColumnStacked3D:
				case ExcelChartType.ColumnStacked1003D:
				case ExcelChartType.Bubble3DEffect:
				case ExcelChartType.ConeBarClustered:
				case ExcelChartType.ConeBarStacked:
				case ExcelChartType.ConeBarStacked100:
				case ExcelChartType.ConeCol:
				case ExcelChartType.ConeColClustered:
				case ExcelChartType.ConeColStacked:
				case ExcelChartType.ConeColStacked100:
				case ExcelChartType.CylinderBarClustered:
				case ExcelChartType.CylinderBarStacked:
				case ExcelChartType.CylinderBarStacked100:
				case ExcelChartType.CylinderCol:
				case ExcelChartType.CylinderColClustered:
				case ExcelChartType.CylinderColStacked:
				case ExcelChartType.CylinderColStacked100:
				case ExcelChartType.PyramidBarClustered:
				case ExcelChartType.PyramidBarStacked:
				case ExcelChartType.PyramidBarStacked100:
				case ExcelChartType.PyramidCol:
				case ExcelChartType.PyramidColClustered:
				case ExcelChartType.PyramidColStacked:
				case ExcelChartType.PyramidColStacked100:
					chartTypeElement.Add(new XElement(XName.Get("shape", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), "box")));
					break;
			}
		}

		private void WriteWorkSheetChart_chart_plotArea_marker(ExcelChart excelChart, XElement chartTypeElement)
		{
			if (excelChart.ChartType == ExcelChartType.LineMarkers || excelChart.ChartType == ExcelChartType.LineMarkersStacked ||
			excelChart.ChartType == ExcelChartType.LineMarkersStacked100)
			{
				chartTypeElement.Add(new XElement(XName.Get("marker", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), 1)));
			}
		}

		private void WriteWorkSheetChart_chart_plotArea_varyColors(ExcelChart excelChart, XElement chartTypeElement)
		{
			XElement varyColorsElement = new XElement(XName.Get("grouping", ExcelCommon.Schema_Chart));

			switch (excelChart.ChartType)
			{
				case ExcelChartType.Pie:
				case ExcelChartType.PieExploded:
				case ExcelChartType.PieOfPie:
				case ExcelChartType.Pie3D:
				case ExcelChartType.PieExploded3D:
				case ExcelChartType.BarOfPie:
				case ExcelChartType.Doughnut:
				case ExcelChartType.DoughnutExploded:
					varyColorsElement.Add(new XAttribute(XName.Get("val"), 1));
					break;
				default:
					varyColorsElement.Add(new XAttribute(XName.Get("val"), 0));
					break;
			}

			chartTypeElement.Add(varyColorsElement);
		}

		private void WriteWorkSheetChart_chart_plotArea_grouping(ExcelChart excelChart, XElement chartTypeElement)
		{
			switch (excelChart.ChartType)
			{
				case ExcelChartType.BarClustered3D:
				case ExcelChartType.BarStacked3D:
				case ExcelChartType.BarStacked1003D:
				case ExcelChartType.Column3D:
				case ExcelChartType.ColumnClustered3D:
				case ExcelChartType.ColumnStacked3D:
				case ExcelChartType.ColumnStacked1003D:
				case ExcelChartType.Bubble3DEffect:
				case ExcelChartType.ConeBarClustered:
				case ExcelChartType.ConeBarStacked:
				case ExcelChartType.ConeBarStacked100:
				case ExcelChartType.ConeCol:
				case ExcelChartType.ConeColClustered:
				case ExcelChartType.ConeColStacked:
				case ExcelChartType.ConeColStacked100:
				case ExcelChartType.CylinderBarClustered:
				case ExcelChartType.CylinderBarStacked:
				case ExcelChartType.CylinderBarStacked100:
				case ExcelChartType.CylinderCol:
				case ExcelChartType.CylinderColClustered:
				case ExcelChartType.CylinderColStacked:
				case ExcelChartType.CylinderColStacked100:
				case ExcelChartType.PyramidBarClustered:
				case ExcelChartType.PyramidBarStacked:
				case ExcelChartType.PyramidBarStacked100:
				case ExcelChartType.PyramidCol:
				case ExcelChartType.PyramidColClustered:
				case ExcelChartType.PyramidColStacked:
				case ExcelChartType.PyramidColStacked100:
				case ExcelChartType.Line:
				case ExcelChartType.LineMarkers:
				case ExcelChartType.LineMarkersStacked100:
				case ExcelChartType.LineStacked:
				case ExcelChartType.LineStacked100:
				case ExcelChartType.Line3D:
					{
						if (excelChart.Grouping != ExcelGrouping.Clustered)
							chartTypeElement.Add(new XElement(XName.Get("grouping", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), GetGroupingText(excelChart.Grouping))));
					}
					break;
			}
		}

		private void WriteWorkSheetChart_chart_plotArea_barDir(ExcelChart excelChart, XElement chartTypeElement)
		{
			switch (excelChart.ChartType)
			{
				case ExcelChartType.BarClustered3D:
				case ExcelChartType.BarStacked3D:
				case ExcelChartType.BarStacked1003D:
				case ExcelChartType.Column3D:
				case ExcelChartType.ColumnClustered3D:
				case ExcelChartType.ColumnStacked3D:
				case ExcelChartType.ColumnStacked1003D:
				case ExcelChartType.Bubble3DEffect:
				case ExcelChartType.ConeBarClustered:
				case ExcelChartType.ConeBarStacked:
				case ExcelChartType.ConeBarStacked100:
				case ExcelChartType.ConeCol:
				case ExcelChartType.ConeColClustered:
				case ExcelChartType.ConeColStacked:
				case ExcelChartType.ConeColStacked100:
				case ExcelChartType.CylinderBarClustered:
				case ExcelChartType.CylinderBarStacked:
				case ExcelChartType.CylinderBarStacked100:
				case ExcelChartType.CylinderCol:
				case ExcelChartType.CylinderColClustered:
				case ExcelChartType.CylinderColStacked:
				case ExcelChartType.CylinderColStacked100:
				case ExcelChartType.PyramidBarClustered:
				case ExcelChartType.PyramidBarStacked:
				case ExcelChartType.PyramidBarStacked100:
				case ExcelChartType.PyramidCol:
				case ExcelChartType.PyramidColClustered:
				case ExcelChartType.PyramidColStacked:
				case ExcelChartType.PyramidColStacked100:
					chartTypeElement.Add(new XElement(XName.Get("barDir", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), "col")));
					break;
			}
		}

		private void WriteWorkSheetChart_chart_plotArea_scatterStyle(ExcelChart excelChart, XElement chartTypeElement)
		{
			switch (excelChart.ChartType)
			{
				case ExcelChartType.XYScatter:
				case ExcelChartType.XYScatterLines:
				case ExcelChartType.XYScatterLinesNoMarkers:
				case ExcelChartType.XYScatterSmooth:
				case ExcelChartType.XYScatterSmoothNoMarkers:
					chartTypeElement.Add(new XElement(XName.Get("scatterStyle", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), string.Empty)));
					break;
			}
		}

		private void WriteWorkSheetChart_chart(ExcelChart excelChart, XElement root)
		{
			XElement chartElement = new XElement(XName.Get("chart", ExcelCommon.Schema_Chart));

			if (excelChart._Title != null)
				chartElement.Add(WriteWorkSheetChart_chart_title(excelChart._Title));

			chartElement.Add(new XElement(XName.Get("autoTitleDeleted", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), excelChart.AutoTitleDeleted ? "1" : "0")));

			WriteWorkSheetChart_chart_plotArea(excelChart, chartElement);

			WriteWorkSheetChart_char_legend(excelChart, chartElement);

			WriteWorkSheetChart_char_plotVisOnly(excelChart, chartElement);

			WriteWorkSheetChart_char_dispBlanksAs(excelChart, chartElement);

			WriteWorkSheetChart_char_showDLblsOverMax(excelChart, chartElement);

			root.Add(chartElement);
		}

		private void WriteWorkSheetChart_char_showDLblsOverMax(ExcelChart excelChart, XElement chartElement)
		{
			if (excelChart.DisplayBlanksAs != ExcelDisplayBlanksAs.Gap)
			{
				XElement dispBlanksAsElement = new XElement(XName.Get("showDLblsOverMax", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), "0"));
				chartElement.Add(dispBlanksAsElement);
			}
		}

		private void WriteWorkSheetChart_char_dispBlanksAs(ExcelChart excelChart, XElement chartElement)
		{
			if (excelChart.DisplayBlanksAs != ExcelDisplayBlanksAs.Gap)
			{
				XElement dispBlanksAsElement = new XElement(XName.Get("dispBlanksAs", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), excelChart.DisplayBlanksAs.ToString().ToLower()));
				chartElement.Add(dispBlanksAsElement);
			}
		}

		private void WriteWorkSheetChart_char_plotVisOnly(ExcelChart excelChart, XElement chartElement)
		{
			XElement plotVisOnlyElement = new XElement(XName.Get("plotVisOnly", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), 1));
			chartElement.Add(plotVisOnlyElement);
		}

		private void WriteWorkSheetChart_char_legend(ExcelChart excelChart, XElement chartElement)
		{
			if (excelChart._Legend != null)
			{
				XElement legendElement = new XElement(XName.Get("legend", ExcelCommon.Schema_Chart));

				XElement legendPosElement = new XElement(XName.Get("legendPos", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), "l"));
				legendElement.Add(legendPosElement);

				XElement layoutElement = new XElement(XName.Get("layout", ExcelCommon.Schema_Chart));
				legendElement.Add(layoutElement);

				XElement overlayElement = new XElement(XName.Get("overlay", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), "0"));
				legendElement.Add(overlayElement);

				XElement txPrElement = new XElement(XName.Get("txPr", ExcelCommon.Schema_Chart));
				XElement bodyPrElement = new XElement(XName.Get("bodyPr", ExcelCommon.Schema_Drawings));
				txPrElement.Add(bodyPrElement);

				XElement lstStyleElement = new XElement(XName.Get("lstStyle", ExcelCommon.Schema_Drawings));
				txPrElement.Add(lstStyleElement);

				WriteWorkSheetChart_chart_title_P(excelChart.Legend.Font, txPrElement);

				legendElement.Add(txPrElement);

				chartElement.Add(legendElement);
			}
		}

		private XElement WriteWorkSheetChart_chart_title(ExcelChartTitle title)
		{
			XElement titleElement = new XElement(XName.Get("title", ExcelCommon.Schema_Chart));

			WriteWorkSheetChart_chart_title_tx(title, titleElement);

			WriteWorkSheetChart_chart_title_layout(title, titleElement);

			XElement overlayElement = new XElement(XName.Get("overlay", ExcelCommon.Schema_Chart), new XAttribute(XName.Get("val"), title.Overlay ? "1" : "0"));
			titleElement.Add(overlayElement);

			WriteWorkSheetChart_chart_title_spPr(title, titleElement);

			return titleElement;
		}

		private void WriteWorkSheetChart_chart_title_tx(ExcelChartTitle title, XElement titleElement)
		{
			XElement txElement = new XElement(XName.Get("tx", ExcelCommon.Schema_Chart));

			if (string.IsNullOrEmpty(title.Text) == false)
			{
				XElement richElement = new XElement(XName.Get("rich", ExcelCommon.Schema_Chart));

				XElement bodyPrElement = new XElement(XName.Get("bodyPr", ExcelCommon.Schema_Drawings));
				if (title.AnchorCtr)
					bodyPrElement.Add(new XAttribute(XName.Get("anchorCtr"), "1"));

				if (title.TextHorz != default(ExcelTextHorzOverflowType))
					bodyPrElement.Add(new XAttribute(XName.Get("horzOverflow"), title.TextHorz.ToString().ToLower()));

				bodyPrElement.Add(new XAttribute(XName.Get("anchor"), GetTextAchoringText(title.Anchor)));
				bodyPrElement.Add(new XAttribute(XName.Get("vert"), GetTextVerticalText(title.TextVertical)));
				if (title.Rotation != default(double))
				{
					if (title.Rotation > 180)
						bodyPrElement.Add(new XAttribute(XName.Get("rot"), (int)((title.Rotation - 360) * 60000)));
					else
						bodyPrElement.Add(new XAttribute(XName.Get("rot"), (int)(title.Rotation * 60000)));
				}
				richElement.Add(bodyPrElement);

				richElement.Add(new XElement(XName.Get("lstStyle", ExcelCommon.Schema_Drawings)));

				foreach (Paragraph p in title.RichText)
				{
					WriteWorkSheetChart_chart_title_P(p, richElement);
				}

				txElement.Add(richElement);
				titleElement.Add(txElement);
			}
		}

		private void WriteWorkSheetChart_chart_title_spPr(ExcelChartTitle title, XElement titleElement)
		{
			XElement spPrElement = new XElement(XName.Get("spPr", ExcelCommon.Schema_Chart));

			if (title._Border != null || title._Fill != null)
			{
				XElement lnElement = new XElement(XName.Get("ln", ExcelCommon.Schema_Drawings));

				WriteWorkSheetChart_chart_title_spPr_LnBorder(title._Border, lnElement);

				spPrElement.Add(lnElement);
			}

			titleElement.Add(spPrElement);
		}

		private void WriteWorkSheetChart_chart_title_spPr_LnBorder(DrawingBorder drawingBorder, XElement lnElement)
		{
			if (drawingBorder.Width != default(int))
				lnElement.Add(new XAttribute(XName.Get("w"), drawingBorder.Width * 12700));

			if (drawingBorder.LineCap != default(ExcelDrawingLineCap))
				lnElement.Add(new XAttribute(XName.Get("cap"), TranslateLineCapText(drawingBorder.LineCap)));

			if (drawingBorder.LineStyle != default(ExcelDrawingLineStyle))
			{
				XElement prstDashElement = new XElement(XName.Get("prstDash", ExcelCommon.Schema_Drawings));
				prstDashElement.Add(new XAttribute(XName.Get("val"), TranslateLineStyleText(drawingBorder.LineStyle)));
			}

			if (drawingBorder._Fill != null)
				WriteWrokSheetDrawing_pic_solidFill(drawingBorder._Fill, lnElement);
		}

		private void WriteWorkSheetChart_chart_title_layout(ExcelChartTitle title, XElement titleElement)
		{
			XElement layoutElement = new XElement(XName.Get("layout", ExcelCommon.Schema_Chart));

			titleElement.Add(layoutElement);
		}

		private void WriteWorkSheetChart_chart_title_P(Paragraph p, XElement parentElement)
		{
			XElement pElement = new XElement(XName.Get("p", ExcelCommon.Schema_Drawings));

			if (string.IsNullOrEmpty(p.Text) == false)
			{
				XElement rElement = new XElement(XName.Get("r", ExcelCommon.Schema_Drawings));

				XElement rPrElement = new XElement(XName.Get("rPr", ExcelCommon.Schema_Drawings));
				if (p.Bold) rPrElement.Add(new XAttribute(XName.Get("b"), "1"));
				if (p.Italic) rPrElement.Add(new XAttribute(XName.Get("i"), "1"));
				if (p.Strike != ExcelStrikeType.No) rPrElement.Add(new XAttribute(XName.Get("strike"), TranslateStrikeText(p.Strike)));
				if (p.Size != 11) rPrElement.Add(new XAttribute(XName.Get("sz"), p.Size / 100));

				if (string.IsNullOrEmpty(p.LatinFont) == false && string.Compare(p.LatinFont, "Calibri", true) != 0)
					rPrElement.Add(new XElement(XName.Get("latin", ExcelCommon.Schema_Drawings), new XAttribute(XName.Get("typeface"), p.LatinFont)));
				if (string.IsNullOrEmpty(p.ComplexFont) == false && string.Compare(p.ComplexFont, "Calibri", true) != 0)
					rPrElement.Add(new XElement(XName.Get("cs", ExcelCommon.Schema_Drawings), new XAttribute(XName.Get("typeface"), p.ComplexFont)));

				if (p.UnderLineColor != Color.Empty)
				{
					XElement underLineElement = new XElement(XName.Get("uFill", ExcelCommon.Schema_Drawings));
					XElement solidFillElement = new XElement(XName.Get("solidFill", ExcelCommon.Schema_Drawings));
					XElement srgbClrEment = new XElement(XName.Get("srgbClr", ExcelCommon.Schema_Drawings), new XAttribute(XName.Get("val"), p.UnderLineColor.ToArgb().ToString("X").Substring(2, 6)));
					solidFillElement.Add(srgbClrEment);
					underLineElement.Add(solidFillElement);
					rPrElement.Add(underLineElement);
				}

				if (p.Color != Color.Empty)
				{
					XElement solidFillElement = new XElement(XName.Get("solidFill", ExcelCommon.Schema_Drawings));
					XElement srgbClrElement = new XElement(XName.Get("srgbClr", ExcelCommon.Schema_Drawings), new XAttribute(XName.Get("val"), p.Color.ToArgb().ToString("X").Substring(2, 6)));
					solidFillElement.Add(solidFillElement);
					rPrElement.Add(solidFillElement);
				}

				rElement.Add(rPrElement);
				XElement tElement = new XElement(XName.Get("t", ExcelCommon.Schema_Drawings));
				tElement.Value = p.Text;
				rElement.Add(tElement);

				pElement.Add(rElement);
			}

			parentElement.Add(pElement);
		}

		#endregion

		#region Common
		internal static string TranslateLineStyleText(ExcelDrawingLineStyle value)
		{
			string text = value.ToString();
			switch (value)
			{
				case ExcelDrawingLineStyle.Dash:
				case ExcelDrawingLineStyle.Dot:
				case ExcelDrawingLineStyle.DashDot:
				case ExcelDrawingLineStyle.Solid:
					return text.Substring(0, 1).ToLower() + text.Substring(1, text.Length - 1);
				case ExcelDrawingLineStyle.LongDash:
				case ExcelDrawingLineStyle.LongDashDot:
				case ExcelDrawingLineStyle.LongDashDotDot:
					return "lg" + text.Substring(4, text.Length - 4);
				case ExcelDrawingLineStyle.SystemDash:
				case ExcelDrawingLineStyle.SystemDashDot:
				case ExcelDrawingLineStyle.SystemDashDotDot:
				case ExcelDrawingLineStyle.SystemDot:
					return "sys" + text.Substring(6, text.Length - 6);
				default:
					throw (new Exception("没有找到指定的类型！"));
			}
		}

		internal static string TranslateLineCapText(ExcelDrawingLineCap value)
		{
			switch (value)
			{
				case ExcelDrawingLineCap.Round:
					return "rnd";
				case ExcelDrawingLineCap.Square:
					return "sq";
				default:
					return "flat";
			}
		}

		internal static string GetTextAchoringText(ExcelTextAnchoringType value)
		{
			switch (value)
			{
				case ExcelTextAnchoringType.Bottom:
					return "b";
				case ExcelTextAnchoringType.Center:
					return "ctr";
				case ExcelTextAnchoringType.Distributed:
					return "dist";
				case ExcelTextAnchoringType.Justify:
					return "just";
				default:
					return "t";
			}
		}

		internal static string GetTextVerticalText(ExcelTextVerticalType value)
		{
			switch (value)
			{
				case ExcelTextVerticalType.EastAsianVertical:
					return "eaVert";
				case ExcelTextVerticalType.MongolianVertical:
					return "mongolianVert";
				case ExcelTextVerticalType.Vertical:
					return "vert";
				case ExcelTextVerticalType.Vertical270:
					return "vert270";
				case ExcelTextVerticalType.WordArtVertical:
					return "wordArtVert";
				case ExcelTextVerticalType.WordArtVerticalRightToLeft:
					return "wordArtVertRtl";
				default:
					return "horz";
			}
		}

		private static string TranslateStrikeText(ExcelStrikeType value)
		{
			switch (value)
			{
				case ExcelStrikeType.Single:
					return "sngStrike";
				case ExcelStrikeType.Double:
					return "dblStrike";
				default:
					return "noStrike";
			}
		}

		private static string GetGroupingText(ExcelGrouping grouping)
		{
			switch (grouping)
			{
				case ExcelGrouping.Clustered:
					return "clustered";
				case ExcelGrouping.Stacked:
					return "stacked";
				case ExcelGrouping.PercentStacked:
					return "percentStacked";
				default:
					return "standard";

			}
		}

		private static string GetChartNodeTextByChartType(ExcelChartType chartType)
		{
			switch (chartType)
			{
				case ExcelChartType.Area3D:
				case ExcelChartType.AreaStacked3D:
				case ExcelChartType.AreaStacked1003D:
					return "area3DChart";
				case ExcelChartType.Area:
				case ExcelChartType.AreaStacked:
				case ExcelChartType.AreaStacked100:
					return "areaChart";
				case ExcelChartType.BarClustered:
				case ExcelChartType.BarStacked:
				case ExcelChartType.BarStacked100:
				case ExcelChartType.ColumnClustered:
				case ExcelChartType.ColumnStacked:
				case ExcelChartType.ColumnStacked100:
					return "barChart";
				case ExcelChartType.BarClustered3D:
				case ExcelChartType.BarStacked3D:
				case ExcelChartType.BarStacked1003D:
				case ExcelChartType.ColumnClustered3D:
				case ExcelChartType.ColumnStacked3D:
				case ExcelChartType.ColumnStacked1003D:
				case ExcelChartType.ConeBarClustered:
				case ExcelChartType.ConeBarStacked:
				case ExcelChartType.ConeBarStacked100:
				case ExcelChartType.ConeCol:
				case ExcelChartType.ConeColClustered:
				case ExcelChartType.ConeColStacked:
				case ExcelChartType.ConeColStacked100:
				case ExcelChartType.CylinderBarClustered:
				case ExcelChartType.CylinderBarStacked:
				case ExcelChartType.CylinderBarStacked100:
				case ExcelChartType.CylinderCol:
				case ExcelChartType.CylinderColClustered:
				case ExcelChartType.CylinderColStacked:
				case ExcelChartType.CylinderColStacked100:
				case ExcelChartType.PyramidBarClustered:
				case ExcelChartType.PyramidBarStacked:
				case ExcelChartType.PyramidBarStacked100:
				case ExcelChartType.PyramidCol:
				case ExcelChartType.PyramidColClustered:
				case ExcelChartType.PyramidColStacked:
				case ExcelChartType.PyramidColStacked100:
					return "bar3DChart";
				case ExcelChartType.Bubble:
					return "bubbleChart";
				case ExcelChartType.Doughnut:
				case ExcelChartType.DoughnutExploded:
					return "doughnutChart";
				case ExcelChartType.Line:
				case ExcelChartType.LineMarkers:
				case ExcelChartType.LineMarkersStacked:
				case ExcelChartType.LineMarkersStacked100:
				case ExcelChartType.LineStacked:
				case ExcelChartType.LineStacked100:
					return "lineChart";
				case ExcelChartType.Line3D:
					return "line3DChart";
				case ExcelChartType.Pie:
				case ExcelChartType.PieExploded:
					return "pieChart";
				case ExcelChartType.BarOfPie:
				case ExcelChartType.PieOfPie:
					return "ofPieChart";
				case ExcelChartType.Pie3D:
				case ExcelChartType.PieExploded3D:
					return "pie3DChart";
				case ExcelChartType.Radar:
				case ExcelChartType.RadarFilled:
				case ExcelChartType.RadarMarkers:
					return "radarChart";
				case ExcelChartType.XYScatter:
				case ExcelChartType.XYScatterLines:
				case ExcelChartType.XYScatterLinesNoMarkers:
				case ExcelChartType.XYScatterSmooth:
				case ExcelChartType.XYScatterSmoothNoMarkers:
					return "scatterChart";
				case ExcelChartType.Surface:
					return "surfaceChart";
				case ExcelChartType.StockHLC:
					return "stockChart";
				default:
					throw new NotImplementedException("没有实现的图表类型");
			}
		}
		#endregion
	}
}
