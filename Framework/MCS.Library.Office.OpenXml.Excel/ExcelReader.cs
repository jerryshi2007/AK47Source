using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using MCS.Library.Core;
using System.IO.Packaging;
using System.Drawing;
using System.IO;
using MCS.Library.Office.OpenXml.Excel.DataValidation;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class ExcelReader
	{
		public ExcelReader(WorkBook workBook)
		{
			this._WorkBook = workBook;
		}

		private WorkBook _WorkBook;

		internal WorkBook WorkBook
		{
			get
			{
				return this._WorkBook;
			}
		}

		internal ExcelLoadContext Context
		{ get; set; }

		#region workbook.xml
		/// <summary>
		/// FileName:workbook.xml 
		/// <para>NodePath:workbook/workbookProtection</para>
		/// </summary>
		/// <param name="workbookProtectionNode"></param>
		internal void ReadWorkBook_workbookProtection(XElement workbookProtectionNode)
		{
			if (workbookProtectionNode != null)
			{
				if (workbookProtectionNode.HasAttributes)
				{
					this._WorkBook._Protection = new WorkBookProtection();
					foreach (XAttribute xt in workbookProtectionNode.Attributes())
					{
						this._WorkBook._Protection.Attributes.Add(xt.Name.LocalName, xt.Value);
					}
				}
			}
		}

		/// <summary>
		/// FileName:workbook.xml 
		/// <para>NodePath:workbook/bookViews</para>
		/// </summary>
		/// <param name="bookViewsRoot"></param>
		internal void ReadWorkBook_bookViews(XElement bookViewsRoot)
		{
			foreach (XElement item in bookViewsRoot.Nodes())
			{
				WorkBookView view = new WorkBookView(this.WorkBook);

				foreach (XAttribute attr in item.Attributes())
				{
					view.SetAttribute(attr.Name.LocalName, attr.Value);
				}
				this.WorkBook.Views.Add(view);
			}
		}

		/// <summary>
		/// FileName:workbook.xml 
		/// <para>NodePath:workbook/definedNames</para>
		/// </summary>
		/// <param name="definedNamesRoot"></param>
		internal void ReadWorkBook_definedNames(XElement definedNamesRoot, ExcelLoadContext context)
		{
			if (definedNamesRoot != null)
			{
				foreach (XElement item in definedNamesRoot.Nodes())
				{
					Match rangeMatch = DefinedName.NameRangeReferenceRegex.Match(item.Value);
					if (rangeMatch.Success)
					{
						if (rangeMatch.Groups["Sheet"].Success)
						{
							string sheetName = rangeMatch.Groups["Sheet"].Value;
							if (sheetName.IsNotEmpty())
							{
								WorkSheet currentSheet = this.WorkBook.Sheets[sheetName];
								DefinedName namedRange = new DefinedName(item.Attribute("name").Value, currentSheet)
								{
									Address = Range.Parse(currentSheet, rangeMatch.Groups["Range"].Value),
									//LocalSheetId = int.Parse(item.Attribute("localSheetId").Value),
									NameComment = item.Attribute("comment") == null ? string.Empty : item.Attribute("comment").Value,
									IsNameHidden = item.Attribute("hidden") == null ? false : (int.Parse(item.Attribute("hidden").Value) == 1 ? true : false),
								};
								if (item.Attribute("localSheetId") != null)
								{
									int localsheetID;
									if (int.TryParse(item.Attribute("localSheetId").Value, out localsheetID))
										namedRange.LocalSheetId = localsheetID;
								}
								context.DefinedNames.Add(namedRange);
							}
						}

						//todo: pivottable 
						// if (rangeMatch.Groups["Table"].Success)

					}

					/*	DefinedName namedRange = new DefinedName(item.Attributes("name"),) 
						{ 
							Address = item.Value
						};

					   item.Attributes("name")
						foreach (XAttribute attr in item.Attributes())
						{
							switch (attr.Name.LocalName)
							{
								case "name":
									namedRange.Name = attr.Value;
									break;
								case "comment":
									namedRange.NameComment = attr.Value;
									break;
								case "localSheetId":
									namedRange.LocalSheetId = int.Parse(attr.Value);
									break;
								case "hidden":
									namedRange.IsNameHidden = int.Parse(attr.Value) == 1 ? true : false;
									break;
							}
						}
					   this.WorkBook.Names.Add(namedRange);*/
				}
			}
		}

		/// <summary>
		/// FileName:workbook.xml 
		/// <para>NodePath:workbook/calcPr</para>
		/// </summary>
		/// <param name="calcPrRoot"></param>
		internal void ReadWorkBook_calcPr(XElement calcPrRoot)
		{
			//todo
			throw new NotImplementedException();
		}

		/// <summary>
		/// FileName:workbook.xml 
		/// <para>NodePath:workbook/workbookPr</para>
		/// </summary>
		/// <param name="calcPrRoot"></param>
		internal void ReadWorkBook_workbookPr(XElement workbookPrRoot)
		{
			if (workbookPrRoot != null)
			{
				if (workbookPrRoot.HasAttributes)
				{
					foreach (XAttribute attr in workbookPrRoot.Attributes())
					{
						this.WorkBook.Properties.SetAttribute(attr.Name.LocalName, attr.Value);
					}
				}
			}
		}

		/// <summary>
		/// FileName:workbook.xml 
		/// <para>NodePath:workbook/fileVersion</para>
		/// </summary>
		/// <param name="calcPrRoot"></param>
		internal void ReadWorkBook_fileVersion(XElement fileVersionRoot)
		{
			if (fileVersionRoot != null)
			{
				if (fileVersionRoot.HasAttributes)
				{
					foreach (XAttribute attr in fileVersionRoot.Attributes())
					{
						this.WorkBook.FileVersion.SetAttribute(attr.Name.LocalName, attr.Value);
					}
				}
			}
		}

		/// <summary>
		/// FileName:workbook.xml 
		/// <para>NodePath:workbook/sheets</para>
		/// </summary>
		/// <param name="calcPrRoot"></param>
		internal void ReadWorkBook_sheets(XElement sheetsRoot)
		{
			foreach (XElement item in sheetsRoot.Nodes())
			{
				int positionID = 1;
				string sheetName = string.Empty, relationshipID = string.Empty;
				int sheetId = 0; ExcelWorksheetHidden hidden = ExcelWorksheetHidden.Visible;

				foreach (XAttribute attr in item.Attributes())
				{
					switch (attr.Name.LocalName)
					{
						case "id":
							relationshipID = attr.Value;
							break;
						case "name":
							sheetName = attr.Value;
							break;
						case "sheetId":
							sheetId = Convert.ToInt32(attr.Value);
							break;
						case "state":
							hidden = WorkSheet.TranslateHidden(attr.Value);
							break;
					}
				}

				this.WorkBook.Sheets.Add(new WorkSheet(this.WorkBook, sheetName, sheetId, relationshipID, positionID, hidden));
				positionID++;
			}
		}
		#endregion

		#region sharedStrings.xml
		/// <summary>
		/// FileName:sharedStrings.xml
		/// <para>NodePath:sst</para>
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public void ReadSharedStrings(XElement root)
		{
			List<SharedStringItem> result = new List<SharedStringItem>();

			foreach (XElement item in root.Nodes())
			{
				if (item.HasElements)
				{
					StringBuilder strValue = new StringBuilder();
					foreach (XElement tNode in item.Nodes())
					{
						switch (tNode.Name.LocalName)
						{
							case "t":
								strValue.Append(tNode.Value);
								break;
							case "r":
								{
									foreach (XElement bNode in tNode.Nodes())
									{
										switch (bNode.Name.LocalName)
										{
											case "t":
												strValue.Append(bNode.Value);
												break;
										}
									}
								}
								break;
						}
					}
					result.Add(new SharedStringItem() { Text = ExcelHelper.ExcelDecodeString(strValue.ToString()) });
				}
				else
				{
					result.Add(new SharedStringItem() { Text = item.FirstNode.ToString(), IsRichText = true });
				}
			}
			this.Context.SharedStrings = result;
		}
		#endregion

		#region calculation.xml
		public void ReadCalculation(XElement root)
		{
			if (root.HasElements)
			{
				var calculationCells = new CalculationCellCollection();

				StringBuilder strValue = new StringBuilder();
				foreach (XElement tNode in root.Nodes())
				{
					switch (tNode.Name.LocalName)
					{
						case "c":
							CalculationCell cell = new CalculationCell();
							tNode.Attribute("r").IsNotNull(o => cell.Cell = ((XAttribute)o).Value);
							tNode.Attribute("i").IsNotNull(o => cell.WorkSheet = ((XAttribute)o).Value);
							tNode.Attribute("l").IsNotNull(o => cell.NewLevel = ((XAttribute)o).Value);
							tNode.Attribute("s").IsNotNull(o => cell.InChildChain = ((XAttribute)o).Value);

							calculationCells.Add(cell);
							break;
					}
				}

				this.WorkBook.CalculationChain.CalculationCells.AddRange(calculationCells);
			}
		}
		#endregion

		#region styles.xml
		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet</para>
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public void ReadStyles(XElement root, WorkBookStylesWrapper target)
		{
			foreach (XElement item in root.Nodes())
			{
				switch (item.Name.LocalName)
				{
					case "numFmts":
						ReadStyles_numFmts(target, item);
						break;
					case "fonts":
						ReadStyles_fonts(target, item);
						break;
					case "fills":
						ReadStyles_fills(target, item);
						break;
					case "borders":
						ReadStyles_borders(target, item);
						break;
					case "cellStyleXfs":
						ReadStyles_cellStyleXfs(target, item);
						break;
					case "cellXfs":
						ReadStyles_cellXfs(target, item);
						break;
					case "cellStyles":
						ReadStyles_cellStyles(target, item);
						break;
					case "dxfs":
						ReadStyles_cellXfs(target, item);
						break;
					case "tableStyles":
						ReadStyles_tableStyles(target, item);
						break;
					case "colors":
						ReadStyles_colors(target, item);
						break;
				}
			}
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/numFmts</para>
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public void ReadStyles_numFmts(WorkBookStylesWrapper target, XElement item)
		{
			foreach (XElement numFmt in item.Nodes())
			{
				NumberFormatXmlWrapper nf = new NumberFormatXmlWrapper(false);
				foreach (var attr in numFmt.Attributes())
				{
					if (attr.Name.LocalName == "numFmtId")
					{
						nf.NumFmtId = int.Parse(attr.Value);
						continue;
					}
					if (attr.Name.LocalName == "formatCode")
					{
						nf.Format = attr.Value;
						continue;
					}

				}

				if (nf.NumFmtId >= target.NumberFormats.NextId)
				{
					target.NumberFormats.NextId = nf.NumFmtId + 1;
				}

				target.NumberFormats.Add(nf.Id, nf);
			}
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/fonts</para>
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public void ReadStyles_fonts(WorkBookStylesWrapper target, XElement item)
		{
			foreach (XElement fontElement in item.Nodes())
			{
				FontXmlWrapper fontObj = new FontXmlWrapper();
				foreach (XElement styleElement in fontElement.Nodes())
				{
					switch (styleElement.Name.LocalName)
					{
						case "b":
							{
								fontObj.Bold = true;
								break;
							}
						case "i":
							{
								fontObj.Italic = true;
								break;
							}
						case "u":
							{
								fontObj.UnderLine = true;
								break;
							}
						case "shadow":
							{
								fontObj.Shadow = true;
								break;
							}
						case "vertAlign":
							{
								if (styleElement.Attribute("val") != null)
								{
									fontObj.VerticalAlign = styleElement.Attribute("val").Value;
								}
								break;
							}
						case "sz":
							{
								if (styleElement.Attribute("val") != null)
								{
									fontObj.Size = float.Parse(styleElement.Attribute("val").Value);
								}
								break;
							}
						case "color":
							{
								fontObj.Color = new ColorXmlWrapper();
								this.ReadStyles_Color(styleElement, fontObj.Color);
								break;
							}
						case "rFont":
							{
								// this._ = reader.GetAttribute("val");
								break;
							}
						case "name":
							{
								if (styleElement.Attribute("val") != null)
								{
									fontObj.Name = styleElement.Attribute("val").Value;
								}
								break;
							}
						case "family":
							{
								if (styleElement.Attribute("val") != null)
								{
									fontObj.Family = int.Parse(styleElement.Attribute("val").Value);
								}
								break;
							}
						case "charset":
							{
								if (styleElement.Attribute("val") != null)
								{
									fontObj.Charset = int.Parse(styleElement.Attribute("val").Value);
								}
								break;
							}
						case "scheme":
							{
								if (styleElement.Attribute("val") != null)
								{
									fontObj.Scheme = styleElement.Attribute("val").Value;
								}
								break;
							}
					}
				}

				target.Fonts.Add(fontObj.Id, fontObj);
			}
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/fonts/font/color</para>
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public void ReadStyles_Color(XElement root, ColorXmlWrapper target)
		{
			if (root.Attribute("auto") != null)
			{
				target.Auto = root.Attribute("auto").Value.Equals("1") ? true : false;
			}
			if (root.Attribute("indexed") == null)
			{
				if (root.Attribute("theme") != null)
				{
					target.Theme = root.Attribute("theme").Value;
				}
				if (root.Attribute("rgb") != null)
				{
					target.Rgb = root.Attribute("rgb").Value;
				}
			}
			else
			{
				target.Indexed = int.Parse(root.Attribute("indexed").Value);
			}
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/fills</para>
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public void ReadStyles_fills(WorkBookStylesWrapper target, XElement item)
		{
			foreach (XElement fill in item.Nodes())
			{
				FillXmlWrapper fillObj = new FillXmlWrapper();
				ReadStyles_fill(fillObj, fill);
				target.Fills.Add(fillObj);
			}
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/fills/fill</para>
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public void ReadStyles_fill(FillXmlWrapper target, XElement item)
		{
			foreach (XElement fillNode in item.Nodes())
			{
				switch (fillNode.Name.LocalName)
				{
					case "patternFill":
						if (fillNode.Attribute("patternType") != null)
						{
							target.PatternType = target.GetPatternType(fillNode.Attribute("patternType").Value);
							ReadStyles_fill_Color(target, fillNode);
						}
						break;
				}
			}
		}

		public void ReadStyles_fill_Color(FillXmlWrapper target, XElement item)
		{
			foreach (XElement colorNode in item.Nodes())
			{
				switch (colorNode.Name.LocalName)
				{
					case "bgColor":
						target.BackgroundColor = new ColorXmlWrapper();
						if (colorNode.Attribute("indexed") != null)
						{
							target.BackgroundColor.Indexed = int.Parse(colorNode.Attribute("indexed").Value);
						}
						if (colorNode.Attribute("rgb") != null)
						{
							target.BackgroundColor.Rgb = colorNode.Attribute("rgb").Value;
						}
						if (colorNode.Attribute("theme") != null)
						{
							target.BackgroundColor.Theme = colorNode.Attribute("theme").Value;
						}
						if (colorNode.Attribute("tint") != null)
						{
							target.BackgroundColor.Tint = decimal.Parse(colorNode.Attribute("tint").Value);
						}
						break;
					case "fgColor":
						target.PatternColor = new ColorXmlWrapper();
						if (colorNode.Attribute("indexed") != null)
						{
							target.PatternColor.Indexed = int.Parse(colorNode.Attribute("indexed").Value);
						}
						if (colorNode.Attribute("rgb") != null)
						{
							target.PatternColor.Rgb = colorNode.Attribute("rgb").Value;
						}
						if (colorNode.Attribute("theme") != null)
						{
							target.PatternColor.Theme = colorNode.Attribute("theme").Value;
						}
						if (colorNode.Attribute("tint") != null)
						{
							decimal decTint;
							if (decimal.TryParse(colorNode.Attribute("tint").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out decTint) == true)
							{
								target.PatternColor.Tint = decTint;
								//decimal.Parse(colorNode.Attribute("tint").Value);
							}
						}
						break;
				}
			}
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/borders</para>
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public void ReadStyles_borders(WorkBookStylesWrapper target, XElement item)
		{
			foreach (XElement border in item.Nodes())
			{
				BorderXmlWrapper borderObj = new BorderXmlWrapper();
				ReadStyles_border(borderObj, border);
				target.Borders.Add(borderObj);
			}
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/borders/border</para>
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public void ReadStyles_border(BorderXmlWrapper target, XElement item)
		{
			if (item.Attribute("diagonalUp") != null)
			{
				target.DiagonalUp = string.Compare(item.Attribute("diagonalUp").Value, "1") == 1 ? true : false;
			}

			foreach (XElement borderSubNode in item.Nodes())
			{
				switch (borderSubNode.Name.LocalName)
				{
					case "left":
						target.Left = new BorderItemXmlWrapper();
						ReadStyles_borderitem(target.Left, borderSubNode);
						break;
					case "right":
						target.Right = new BorderItemXmlWrapper();
						ReadStyles_borderitem(target.Right, borderSubNode);
						break;
					case "top":
						target.Top = new BorderItemXmlWrapper();
						ReadStyles_borderitem(target.Top, borderSubNode);
						break;
					case "bottom":
						target.Bottom = new BorderItemXmlWrapper();
						ReadStyles_borderitem(target.Bottom, borderSubNode);
						break;
					case "diagonal":
						target.Diagonal = new BorderItemXmlWrapper();
						ReadStyles_borderitem(target.Diagonal, borderSubNode);
						break;
				}
			}
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/borders/border/[border items]</para>
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public void ReadStyles_borderitem(BorderItemXmlWrapper target, XElement item)
		{
			if (item.Attribute("style") != null)
			{
				target.Style = target.GetBorderStyle(item.Attribute("style").Value);
			}

			foreach (XElement itemSubNode in item.Nodes())
			{
				switch (itemSubNode.Name.LocalName)
				{
					case "color":
						ReadStyles_Color(itemSubNode, target.Color);
						break;
				}
			}
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/cellStyleXfs</para>
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public void ReadStyles_cellStyleXfs(WorkBookStylesWrapper target, XElement item)
		{
			foreach (XElement element in item.Nodes())
			{
				CellStyleXmlWrapper cellStyle = new CellStyleXmlWrapper();
				ReadStyles_cellStyleXfs_xf(cellStyle, element, target);
				target.CellStyleXfs.Add(cellStyle);
			}
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/cellStyleXfs/xf</para>
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public void ReadStyles_cellStyleXfs_xf(CellStyleXmlWrapper target, XElement item, WorkBookStylesWrapper currentStyle)
		{
			if (item.Attribute("numFmtId") != null)
			{
				target.NumberFormatId = int.Parse(item.Attribute("numFmtId").Value);
				if (target.NumberFormatId != 0)
				{
					target.NumberFormat = currentStyle.NumberFormats.FirstOrDefault(n => n.NumFmtId == target.NumberFormatId);
					//target.NumberFormat = currentStyle.NumberFormats[target.NumberFormatId];
				}
			}
			if (item.Attribute("fontId") != null)
			{
				target.FontId = int.Parse(item.Attribute("fontId").Value);
				target.Font = currentStyle.Fonts[target.FontId];
			}
			if (item.Attribute("fillId") != null)
			{
				target.FillId = int.Parse(item.Attribute("fillId").Value);
				target.Fill = currentStyle.Fills[target.FillId];
			}
			if (item.Attribute("borderId") != null)
			{
				target.BorderId = int.Parse(item.Attribute("borderId").Value);
				target.Border = currentStyle.Borders[target.BorderId];
			}
			if (item.Attribute("applyBorder") != null)
			{
				target.ApplyBorder = string.Compare(item.Attribute("applyBorder").Value, "1") == 0 ? true : false;
			}
			if (item.Attribute("applyAlignment") != null)
			{
				target.ApplyAlignment = string.Compare(item.Attribute("applyAlignment").Value, "1") == 0 ? true : false;
			}

			foreach (XElement node in item.Nodes())
			{
				switch (node.Name.LocalName)
				{
					case "alignment":
						ReadStyles_cellStyleXfs_xf_alignment(target, node);
						break;
					case "protection":
						ReadStyles_cellStyleXfs_xf_protection(target, node);
						break;
					case "extLst":
						ReadStyles_cellStyleXfs_xf_extLst(target, node);
						break;
				}
			}
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/cellStyleXfs/xf/alignment</para>
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public void ReadStyles_cellStyleXfs_xf_alignment(CellStyleXmlWrapper target, XElement item)
		{
			if (item.Attribute("horizontal") != null)
			{
				target.HorizontalAlignment = CellStyleXmlWrapper.GetHorizontalAlign(item.Attribute("horizontal").Value);
			}
			if (item.Attribute("vertical") != null)
			{
				target.VerticalAlignment = CellStyleXmlWrapper.GetVerticalAlign(item.Attribute("vertical").Value);
			}
			if (item.Attribute("wrapText") != null)
			{
				target.WrapText = item.Attribute("wrapText").Value.Equals("1") ? true : false;
			}
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/cellStyleXfs/xf/protection</para>
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public void ReadStyles_cellStyleXfs_xf_protection(CellStyleXmlWrapper target, XElement item)
		{
			if (item.Attribute("locked") != null)
			{
				target.Locked = item.Attribute("locked").Value.Equals("0") ? false : true;
			}
			if (item.Attribute("hidden") != null)
			{
				target.Hidden = item.Attribute("hidden").Value.Equals("1") ? true : false;
			}
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/cellStyleXfs/xf/extLst</para>
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public void ReadStyles_cellStyleXfs_xf_extLst(CellStyleXmlWrapper target, XElement item)
		{
			if (item.Attribute("uri") != null)
			{
				//todo:有待完善
			}
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/cellXfs</para>
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public void ReadStyles_cellXfs(WorkBookStylesWrapper target, XElement item)
		{
			foreach (XElement node in item.Nodes())
			{
				CellStyleXmlWrapper cellStyle = new CellStyleXmlWrapper();
				ReadStyles_cellStyleXfs_xf(cellStyle, node, target);
				target.CellXfs.Add(cellStyle);
			}
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/cellStyles</para>
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public void ReadStyles_cellStyles(WorkBookStylesWrapper target, XElement item)
		{
			foreach (XElement node in item.Nodes())
			{
				NamedStyleXmlWrapper nameStyle = new NamedStyleXmlWrapper(target);

				nameStyle.Name = node.Attribute("name").Value;
				if (node.Attribute("xfId") != null)
				{
					nameStyle.XfId = int.Parse(node.Attribute("xfId").Value);
				}
				if (node.Attribute("builtinId") != null)
				{
					nameStyle.BuildInId = int.Parse(node.Attribute("builtinId").Value);
				}

				target.NamedStyles.Add(nameStyle);
			}
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/dxfs</para>
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public void ReadStyles_dxfs(WorkBookStylesWrapper target, XElement item)
		{
			//todo:完善
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/tableStyles</para>
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public void ReadStyles_tableStyles(WorkBookStylesWrapper target, XElement item)
		{
			foreach (var attr in item.Attributes())
			{
				target.TableStyles.Attributes.Clear();
				target.TableStyles.Attributes.Add(attr.Name.LocalName, attr.Value);
			}
		}

		/// <summary>
		/// FileName:styles.xml
		/// <para>NodePath:styleSheet/colors</para>
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public void ReadStyles_colors(WorkBookStylesWrapper target, XElement item)
		{
			//todo:完善
		}
		#endregion

		#region core.xml
		/// <summary>
		/// FileName:core.xml
		/// <para>NodePath:cp</para>
		/// </summary>
		/// <param name="root"></param>
		public void ReadCore(XElement root)
		{
			foreach (XElement item in root.Nodes())
			{
				switch (item.Name.LocalName)
				{
					case "creator":
						this.WorkBook.FileDetails.Author = item.Value;
						break;
					case "title":
						this.WorkBook.FileDetails.Title = item.Value;
						break;
					case "subject":
						this.WorkBook.FileDetails.Subject = item.Value;
						break;
					case "description":
						this.WorkBook.FileDetails.Comments = item.Value;
						break;
					case "keywords":
						this.WorkBook.FileDetails.Keywords = item.Value;
						break;
					case "lastModifiedBy":
						this.WorkBook.FileDetails.LastModifiedBy = item.Value;
						break;
					case "category":
						this.WorkBook.FileDetails.Category = item.Value;
						break;
					case "contentStatus":
						this.WorkBook.FileDetails.Status = item.Value;
						break;
				}
			}
		}
		#endregion

		#region app.xml
		/// <summary>
		/// FileName:app.xml
		/// </summary>
		/// <param name="root"></param>
		public void ReadApp(XElement root)
		{
			foreach (XElement item in root.Nodes())
			{
				switch (item.Name.LocalName)
				{
					case "Application":
						this.WorkBook.FileDetails.Application = item.Value;
						break;
					case "HyperlinkBase":
						if (!string.IsNullOrEmpty(item.Value))
						{
							this.WorkBook.FileDetails.HyperlinkBase = new Uri(item.Value, UriKind.Absolute);
						}
						break;
					case "AppVersion":
						this.WorkBook.FileDetails.AppVersion = item.Value;
						break;
					case "Company":
						this.WorkBook.FileDetails.Company = item.Value;
						break;
					case "Manager":
						this.WorkBook.FileDetails.Manager = item.Value;
						break;
				}
			}
		}
		#endregion

		#region theme1.xml
		/// <summary>
		/// FileName:theme1.xml
		/// <para>NodePath:theme</para>
		/// </summary>
		/// <param name="bookViewsRoot"></param>
		internal void ReadTheme(XElement themeRoot)
		{
			if (themeRoot.Attribute("name") != null)
			{
				this.WorkBook.Theme.Name = themeRoot.Attribute("name").Value;
				foreach (XElement item in themeRoot.Nodes())
				{
					switch (item.Name.LocalName)
					{
						case "themeElements":
							ReadTheme_themeElements(item);
							break;
						case "objectDefaults":
							foreach (var attr in item.Attributes())
							{
								this.WorkBook.Theme.ObjectDefaults.Attributes.Add(attr.Name.LocalName, attr.Value);
							}
							break;
						case "extraClrSchemeLst":
							foreach (var attr in item.Attributes())
							{
								this.WorkBook.Theme.ExtraClrSchemeLst.Attributes.Add(attr.Name.LocalName, attr.Value);
							}
							break;
					}
				}
			}
		}

		/// <summary>
		/// FileName:theme1.xml
		/// <para>NodePath:theme/themeElements</para>
		/// </summary>
		/// <param name="bookViewsRoot"></param>
		internal void ReadTheme_themeElements(XElement item)
		{
			foreach (XElement node in item.Nodes())
			{
				switch (node.Name.LocalName)
				{
					case "clrScheme":
						ReadTheme_themeElements_clrScheme(this.WorkBook.Theme.ThemeElements.ColorScheme, node);
						break;
					case "fontScheme":
						foreach (var attr in node.Attributes())
						{
							this.WorkBook.Theme.ThemeElements.FontScheme.SetAttribute(attr.Name.LocalName, attr.Value);
						}
						break;
					case "fmtScheme":
						foreach (var attr in node.Attributes())
						{
							this.WorkBook.Theme.ThemeElements.FormatScheme.SetAttribute(attr.Name.LocalName, attr.Value);
						}
						break;
				}
			}
		}

		/// <summary>
		/// FileName:theme1.xml
		/// <para>NodePath:theme/themeElements/clrScheme</para>
		/// </summary>
		/// <param name="bookViewsRoot"></param>
		internal void ReadTheme_themeElements_clrScheme(ThemeColorScheme target, XElement item)
		{
			target.Name = item.Attribute("name").Value;

			foreach (XElement node_dk1 in item.Nodes())
			{
				foreach (XElement node in node_dk1.Nodes())
				{
					switch (node.Name.LocalName)
					{
						case "sysClr":
							ThemeSysColor themeSysColor = new ThemeSysColor(target, node.Name.LocalName);
							themeSysColor.Val = node.Attribute("val").Value;
							themeSysColor.LastClr = node.Attribute("lastClr").Value;
							target.Colors.Add(target.Colors.Count, themeSysColor);
							break;
						case "srgbClr":
							ThemeSrgbColor themeSrgbColor = new ThemeSrgbColor(target, node.Name.LocalName);
							themeSrgbColor.Val = node.Attribute("val").Value;
							target.Colors.Add(target.Colors.Count, themeSrgbColor);
							break;
					}
				}
			}
		}
		#endregion

		#region sheet1.xml
		/// <summary>
		/// FileName:sheet1.xml
		/// <para>NodePath:worksheet</para>
		/// </summary>
		/// <param name="bookViewsRoot"></param>
		internal void ReadWorkSheet(WorkSheet target, XElement sheetRoot)
		{
			XElement sheetNode = sheetRoot.Element(XName.Get("dimension", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (sheetNode != null)
			{
				if (sheetNode.Attribute("ref") != null)
				{
					target.Dimension = Range.Parse(target, sheetNode.Attribute("ref").Value);
				}
			}

			sheetNode = sheetRoot.Element(XName.Get("sheetViews", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (sheetNode != null)
			{
				ReadWorkSheet_sheetViews(target, sheetNode);
			}

			sheetNode = sheetRoot.Element(XName.Get("cols", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (sheetNode != null)
			{
				ReadWorkSheet_cols(target, sheetNode);
			}

			sheetNode = sheetRoot.Element(XName.Get("sheetData", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (sheetNode != null)
			{
				ReadWorkSheet_sheetData(target, sheetNode);
			}

			sheetNode = sheetRoot.Element(XName.Get("hyperlinks", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (sheetNode != null)
			{
				ReadWorkSheet_hyperlinks(target, sheetNode);
			}

			sheetNode = sheetRoot.Element(XName.Get("phoneticPr", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (sheetNode != null)
			{
				foreach (XAttribute att in sheetNode.Attributes())
				{
					target.PhoneticProperties.SetAttribute(att.Name.LocalName, att.Value);
				}
			}

			sheetNode = sheetRoot.Element(XName.Get("dataValidations", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (sheetNode != null)
			{
				ReadWorkSheet_dataValidations(target, sheetNode);
			}

			sheetNode = sheetRoot.Element(XName.Get("extLst", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (sheetNode != null)
			{
				ReadWorkSheet_dataValidationsExtList(target, sheetNode);
			}


			sheetNode = sheetRoot.Element(XName.Get("pageMargins", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (sheetNode != null)
			{
				ReadWorkSheet_pageMargins(target, sheetNode);
			}

			sheetNode = sheetRoot.Element(XName.Get("pageSetup", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (sheetNode != null)
			{
				ReadWorkSheet_pageSetup(target, sheetNode);
			}

			sheetNode = sheetRoot.Element(XName.Get("printOptions", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (sheetNode != null)
			{
				ReadWorkSheet_printOptions(target, sheetNode);
			}

			sheetNode = sheetRoot.Element(XName.Get("tableParts", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (sheetNode != null)
			{
				ReadWorkSheet_tableParts(target, sheetNode);
			}

			sheetNode = sheetRoot.Element(XName.Get("sheetProtection", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (sheetNode != null)
			{
				ReadWorkSheet_sheetProtection(target, sheetNode);
			}

			sheetNode = sheetRoot.Element(XName.Get("mergeCells", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (sheetNode != null)
			{
				ReadWorkSheet_mergeCells(target, sheetNode);
			}

			sheetNode = sheetRoot.Element(XName.Get("sheetPr", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (sheetNode != null)
			{
				ReadWorkSheet_sheetPr(target, sheetNode);
			}

			sheetNode = sheetRoot.Element(XName.Get("legacyDrawing", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (sheetNode != null)
			{
				CommentCollection sheetComments = new CommentCollection(target, sheetNode.Attribute(XName.Get("id", ExcelCommon.Schema_Relationships)).Value);
				((IPersistable)sheetComments).Load(this.Context);
				this.Context.Comments.Add(target.Name, sheetComments);
			}

			sheetNode = sheetRoot.Element(XName.Get("headerFooter", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (sheetNode != null)
			{
				target._HeaderFooter = new HeaderFooter(target);
				XElement drawingHFNode = sheetRoot.Element(XName.Get("legacyDrawingHF", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
				if (drawingHFNode != null)
				{
					target._HeaderFooter._Pictures = new VmlDrawingPictureCollection(target, drawingHFNode.Attribute(XName.Get("id", ExcelCommon.Schema_Relationships)).Value);
					((IPersistable)target._HeaderFooter._Pictures).Load(this.Context);
				}
				ReadWorkSheet_headerFooter(target, sheetNode);
			}
			//XElement(XName.Get("drawing", ExcelCommon.Schema_WorkBook_Main.NamespaceName), new XAttribute(XName.Get("id", ExcelCommon.Schema_Relationships)
			sheetNode = sheetRoot.Element(XName.Get("drawing", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (sheetNode != null)
			{
				target._Drawings = new DrawingCollection(target, sheetNode.Attribute(XName.Get("id", ExcelCommon.Schema_Relationships)).Value);
				((IPersistable)target._Drawings).Load(this.Context);
			}



		}

		private void ReadWorkSheet_dataValidationsExtList(WorkSheet target, XElement sheetNode)
		{

			foreach (XElement ext in sheetNode.Elements(XName.Get("ext", ExcelCommon.Schema_WorkBook_Main.NamespaceName)))
			{
				foreach (XElement validateCol in ext.Elements(XName.Get("dataValidations", ExcelCommon.Schema_Ext)))
				{
					foreach (XElement validate in validateCol.Elements(XName.Get("dataValidation", ExcelCommon.Schema_Ext)))
					{
						ReadWorkSheet_dataExtValidations(target, validate);
					}

				}
			}
		}

		private void ReadWorkSheet_dataExtValidations(WorkSheet target, XElement sheetNode)
		{

			DataValidationList dvList = new DataValidationList(sheetNode.Element(XName.Get("sqref", ExcelCommon.Schema_Ext_Sqref)).Value, DataValidationType.List);
			ReadWorkSheet_dataValidations_Item(dvList, sheetNode);
			ReadWorkSheet_dataExtValidations_ItemList(dvList, sheetNode);
			if (target._Validations == null)
			{
				target._Validations = new DataValidationCollection(target);
			}
			target._Validations.Add(dvList);
		}

		private void ReadWorkSheet_dataValidations(WorkSheet target, XElement sheetNode)
		{
			target._Validations = new DataValidationCollection(target);
			foreach (XElement node in sheetNode.Elements(XName.Get("dataValidation", ExcelCommon.Schema_WorkBook_Main.NamespaceName)))
			{
				XAttribute dvXA = node.Attribute(XName.Get("type"));
				if (dvXA != null)
				{
					DataValidationType dvType = DataValidationType.GetBySchemaName(dvXA.Value);
					string address = node.Attribute(XName.Get("sqref")).Value;
					IDataValidation item = null;
					switch (dvType.Type)
					{
						case ExcelDataValidationType.TextLength:
						case ExcelDataValidationType.Whole:
							{
								DataValidationInt dvint = new DataValidationInt(address, dvType);
								ReadWorkSheet_dataValidations_Item(dvint, node);
								ReadWorkSheet_dataValidations_ItemInt(dvint, node);
								item = dvint;
								break;
							}
						case ExcelDataValidationType.Decimal:
							{
								DataValidationDecimal dvDecima = new DataValidationDecimal(address, dvType);
								ReadWorkSheet_dataValidations_Item(dvDecima, node);
								ReadWorkSheet_dataValidations_ItemDecima(dvDecima, node);
								item = dvDecima;
								break;
							}
						case ExcelDataValidationType.List:
							{
								DataValidationList dvList = new DataValidationList(address, dvType);
								ReadWorkSheet_dataValidations_Item(dvList, node);
								ReadWorkSheet_dataValidations_ItemList(dvList, node);
								item = dvList;
								break;
							}
						case ExcelDataValidationType.DateTime:
							{
								DataValidationDateTime dvDateTime = new DataValidationDateTime(address, dvType);
								ReadWorkSheet_dataValidations_Item(dvDateTime, node);
								ReadWorkSheet_dataValidations_ItemDateTime(dvDateTime, node);
								item = dvDateTime;
								break;
							}
						case ExcelDataValidationType.Time:
							{
								DataValidationTime dvTime = new DataValidationTime(address, dvType);
								ReadWorkSheet_dataValidations_Item(dvTime, node);
								ReadWorkSheet_dataValidations_ItemTime(dvTime, node);
								item = dvTime;
								break;
							}
						case ExcelDataValidationType.Custom:
							{
								DataValidationCustom dvCustom = new DataValidationCustom(address, dvType);
								ReadWorkSheet_dataValidations_Item(dvCustom, node);
								ReadWorkSheet_dataValidations_ItemCustom(dvCustom, node);
								item = dvCustom;
								break;
							}
						default:
							throw new InvalidOperationException("不存在验证: " + dvType.Type.ToString());
					}
					target._Validations.Add(item);
				}
			}
		}

		private void ReadWorkSheet_dataValidations_ItemCustom(DataValidationCustom dvCustom, XElement node)
		{
			XElement formulaNode = node.Element(XName.Get("formula1", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (formulaNode != null)
			{
				dvCustom.Formula.Formula = formulaNode.Value;
			}
		}

		private void ReadWorkSheet_dataValidations_ItemTime(DataValidationTime dvTime, XElement node)
		{
			XElement formulaNode = node.Element(XName.Get("formula1", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (formulaNode != null)
			{
				DataValidationFormulaTime formulaValue = new DataValidationFormulaTime();
				ReadWorkSheet_dataValidations_ItemTime_formula(formulaNode.Value, ref formulaValue);
				dvTime.Formula = formulaValue;
			}

			formulaNode = node.Element(XName.Get("formula2", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (formulaNode != null)
			{
				DataValidationFormulaTime formulaValue = new DataValidationFormulaTime();
				ReadWorkSheet_dataValidations_ItemTime_formula(formulaNode.Value, ref formulaValue);
				dvTime.Formula2 = formulaValue;
			}
		}

		private void ReadWorkSheet_dataValidations_ItemTime_formula(string value, ref DataValidationFormulaTime target)
		{
			if (!string.IsNullOrEmpty(value))
			{
				decimal time = default(decimal);
				if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out time))
				{
					target.Value = new TimeWrapper(time);
				}
				else
				{
					target.Formula = value;
				}
			}
		}

		private void ReadWorkSheet_dataValidations_ItemDateTime(DataValidationDateTime dvDateTime, XElement node)
		{
			XElement formulaNode = node.Element(XName.Get("formula1", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (formulaNode != null)
			{
				DataValidationFormulaDateTime formulaValue = new DataValidationFormulaDateTime();
				ReadWorkSheet_dataValidations_ItemDateTime_formula(formulaNode.Value, ref formulaValue);
				dvDateTime.Formula = formulaValue;
			}

			formulaNode = node.Element(XName.Get("formula2", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (formulaNode != null)
			{
				DataValidationFormulaDateTime formulaValue = new DataValidationFormulaDateTime();
				ReadWorkSheet_dataValidations_ItemDateTime_formula(formulaNode.Value, ref formulaValue);
				dvDateTime.Formula2 = formulaValue;
			}
		}

		private void ReadWorkSheet_dataValidations_ItemDateTime_formula(string value, ref DataValidationFormulaDateTime target)
		{
			if (!string.IsNullOrEmpty(value))
			{
				double oADate = default(double);
				if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out oADate))
				{
					target.Value = DateTime.FromOADate(oADate);
				}
				else
				{
					target.Formula = value;
				}
			}
		}

		private void ReadWorkSheet_dataExtValidations_ItemList(DataValidationList dvList, XElement node)
		{

			XElement formulaNode = node.Element(XName.Get("formula1", ExcelCommon.Schema_Ext));
			SetListFormula(dvList, formulaNode);
		}

		private void SetListFormula(DataValidationList dvList, XElement formulaNode)
		{
			if (formulaNode != null)
			{
				var @value = formulaNode.Value;
				if (!string.IsNullOrEmpty(@value))
				{
					if (@value.StartsWith("\"") && @value.EndsWith("\""))
					{
						@value = @value.TrimStart('"').TrimEnd('"');
						var items = @value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
						foreach (var item in items)
						{
							dvList.Formula.Values.Add(item);
						}
					}
					else
					{
						dvList.Formula.Formula = @value;
					}
				}
			}
		}

		private void ReadWorkSheet_dataValidations_ItemList(DataValidationList dvList, XElement node)
		{
			XElement formulaNode = node.Element(XName.Get("formula1", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			SetListFormula(dvList, formulaNode);
		}

		private void ReadWorkSheet_dataValidations_ItemDecima(DataValidationDecimal dvDecima, XElement node)
		{
			XElement formulaNode = node.Element(XName.Get("formula1", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (formulaNode != null)
			{
				DataValidationFormulaDecimal formulaValue = new DataValidationFormulaDecimal();
				ReadWorkSheet_dataValidations_ItemDecima_formula(formulaNode.Value, ref formulaValue);
				dvDecima.Formula = formulaValue;
			}

			formulaNode = node.Element(XName.Get("formula2", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (formulaNode != null)
			{
				DataValidationFormulaDecimal formulaValue = new DataValidationFormulaDecimal();
				ReadWorkSheet_dataValidations_ItemDecima_formula(formulaNode.Value, ref formulaValue);
				dvDecima.Formula2 = formulaValue;
			}
		}

		private void ReadWorkSheet_dataValidations_ItemDecima_formula(string value, ref DataValidationFormulaDecimal formulaValue)
		{
			if (!string.IsNullOrEmpty(value))
			{
				double dValue = default(double);
				if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out dValue))
				{
					formulaValue.Value = dValue;
				}
				else
				{
					formulaValue.Formula = value;
				}
			}
		}

		private void ReadWorkSheet_dataValidations_ItemInt(DataValidationInt dvint, XElement node)
		{
			XElement formulaNode = node.Element(XName.Get("formula1", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (formulaNode != null)
			{
				DataValidationFormulaInt formulaValue = new DataValidationFormulaInt();
				ReadWorkSheet_dataValidations_ItemInt_formula(formulaNode.Value, ref formulaValue);
				dvint.Formula = formulaValue;
			}

			formulaNode = node.Element(XName.Get("formula2", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (formulaNode != null)
			{
				DataValidationFormulaInt formulaValue = new DataValidationFormulaInt();
				ReadWorkSheet_dataValidations_ItemInt_formula(formulaNode.Value, ref formulaValue);
				dvint.Formula2 = formulaValue;
			}
		}

		private void ReadWorkSheet_dataValidations_ItemInt_formula(string value, ref DataValidationFormulaInt formulaValue)
		{
			if (!string.IsNullOrEmpty(value))
			{
				int intValue = default(int);
				if (int.TryParse(value, out intValue))
				{
					formulaValue.Value = intValue;
				}
				else
				{
					formulaValue.Formula = value;
				}
			}
		}

		private void ReadWorkSheet_dataValidations_Item(ExcelDataValidation item, XElement dataValidationNode)
		{
			ReadWorkSheet_dataValidations_Item_attribute(item, dataValidationNode);
		}

		private void ReadWorkSheet_dataValidations_Item_attribute(ExcelDataValidation item, XElement dataValidationNode)
		{
			XAttribute attribute = dataValidationNode.Attribute(XName.Get("allowBlank"));
			if (attribute != null)
			{
				item.AllowBlank = string.Compare(attribute.Value, "1") == 0 ? true : false;
			}

			attribute = dataValidationNode.Attribute(XName.Get("operator"));
			if (attribute != null)
			{
				if (!string.IsNullOrEmpty(attribute.Value))
				{
					item.Operator = (ExcelDataValidationOperator)Enum.Parse(typeof(ExcelDataValidationOperator), attribute.Value);
				}
			}
			else
			{
				item.Operator = ExcelDataValidationOperator.between;
			}

			attribute = dataValidationNode.Attribute(XName.Get("showInputMessage"));
			if (attribute != null)
			{
				item.ShowInputMessage = string.Compare(attribute.Value, "1") == 0 ? true : false;
			}

			attribute = dataValidationNode.Attribute(XName.Get("showErrorMessage"));
			if (attribute != null)
			{
				item.ShowErrorMessage = string.Compare(attribute.Value, "1") == 0 ? true : false;
			}

			attribute = dataValidationNode.Attribute(XName.Get("errorTitle"));
			if (attribute != null)
			{
				item.ErrorTitle = attribute.Value;
			}

			attribute = dataValidationNode.Attribute(XName.Get("error"));
			if (attribute != null)
			{
				item.Error = attribute.Value;
			}

			attribute = dataValidationNode.Attribute(XName.Get("promptTitle"));
			if (attribute != null)
			{
				item.PromptTitle = attribute.Value;
			}

			attribute = dataValidationNode.Attribute(XName.Get("prompt"));
			if (attribute != null)
			{
				item.Prompt = attribute.Value;
			}
		}

		private void ReadWorkSheet_headerFooter(WorkSheet target, XElement sheetNode)
		{
			#region "XAttribute"
			XAttribute headerXt = sheetNode.Attribute(XName.Get("differentOddEven"));
			if (headerXt != null)
			{
				target._HeaderFooter.DifferentOddEven = headerXt.Value.Equals("1") ? true : false;
			}

			headerXt = sheetNode.Attribute(XName.Get("differentFirst"));
			if (headerXt != null)
			{
				target._HeaderFooter.DifferentFirst = headerXt.Value.Equals("1") ? true : false;
			}

			headerXt = sheetNode.Attribute(XName.Get("scaleWithDoc"));
			if (headerXt != null)
			{
				target._HeaderFooter.ScaleWithDoc = headerXt.Value.Equals("1") ? true : false;
			}

			headerXt = sheetNode.Attribute(XName.Get("alignWithMargins"));
			if (headerXt != null)
			{
				target._HeaderFooter.AlignWithMargins = headerXt.Value.Equals("1") ? true : false;
			}
			#endregion "XAttribute"

			#region "XElement"
			XElement childNode = sheetNode.Element(XName.Get("oddHeader", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (childNode != null)
			{
				if (!string.IsNullOrEmpty(childNode.Value))
				{
					target._HeaderFooter._OddHeader = new HeaderFooterText(target._HeaderFooter, "H");
					ReadWorkSheet_headerFooter_FooterText(ref target._HeaderFooter._OddHeader, childNode.Value);
					ReadWorkSheet_headerFooter_Image(target._HeaderFooter, ref target._HeaderFooter._OddHeader, "H");
				}
			}

			childNode = sheetNode.Element(XName.Get("oddFooter", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (childNode != null)
			{
				if (!string.IsNullOrEmpty(childNode.Value))
				{
					target._HeaderFooter._OddFooter = new HeaderFooterText(target._HeaderFooter, "F");
					ReadWorkSheet_headerFooter_FooterText(ref target._HeaderFooter._OddFooter, childNode.Value);
					ReadWorkSheet_headerFooter_Image(target._HeaderFooter, ref target._HeaderFooter._OddFooter, "F");
				}
			}

			if (target._HeaderFooter.DifferentFirst)
			{
				childNode = sheetNode.Element(XName.Get("firstHeader", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
				if (childNode != null)
				{
					if (!string.IsNullOrEmpty(childNode.Value))
					{
						target._HeaderFooter._FirstHeader = new HeaderFooterText(target._HeaderFooter, "HFIRST");
						ReadWorkSheet_headerFooter_FooterText(ref target._HeaderFooter._FirstHeader, childNode.Value);
						ReadWorkSheet_headerFooter_Image(target._HeaderFooter, ref target._HeaderFooter._FirstHeader, "HFIRST");
					}
				}
				childNode = sheetNode.Element(XName.Get("firstFooter", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
				if (childNode != null)
				{
					if (!string.IsNullOrEmpty(childNode.Value))
					{
						target._HeaderFooter._FirstFooter = new HeaderFooterText(target._HeaderFooter, "FFIRST");
						ReadWorkSheet_headerFooter_FooterText(ref target._HeaderFooter._FirstFooter, childNode.Value);
						ReadWorkSheet_headerFooter_Image(target._HeaderFooter, ref target._HeaderFooter._FirstFooter, "FFIRST");
					}
				}
			}

			if (target._HeaderFooter.DifferentOddEven)
			{
				childNode = sheetNode.Element(XName.Get("evenHeader", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
				if (childNode != null)
				{
					if (!string.IsNullOrEmpty(childNode.Value))
					{
						target._HeaderFooter._EvenHeader = new HeaderFooterText(target._HeaderFooter, "HEVEN");
						ReadWorkSheet_headerFooter_FooterText(ref target._HeaderFooter._EvenHeader, childNode.Value);
						ReadWorkSheet_headerFooter_Image(target._HeaderFooter, ref target._HeaderFooter._EvenHeader, "HEVEN");
					}
				}
				childNode = sheetNode.Element(XName.Get("evenFooter", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
				if (childNode != null)
				{
					if (!string.IsNullOrEmpty(childNode.Value))
					{
						target._HeaderFooter._EvenFooter = new HeaderFooterText(target._HeaderFooter, "FEVEN");
						ReadWorkSheet_headerFooter_FooterText(ref target._HeaderFooter._EvenFooter, childNode.Value);
						ReadWorkSheet_headerFooter_Image(target._HeaderFooter, ref target._HeaderFooter._EvenFooter, "FEVEN");
					}
				}
			}
			#endregion
		}

		private void ReadWorkSheet_headerFooter_Image(HeaderFooter headerFooter, ref HeaderFooterText headerFooterText, string tag)
		{
			if (headerFooter._Pictures != null)
			{
				//偶数页眉  <evenHeader> // 右 "RHEVEN"  中间 “CHEVEN", 左 "LHEVEN" 
				//首页页眉  <firstHeader> // 右 ”RHFIRST“ 中间 "CHFIRST" 左 "LHFIRST"
				//奇数页眉  <oddHeader>  // 右 “RH”  中 "CH"  左 "LH"

				//奇数页脚  <oddFooter>   //F
				//偶数页脚  <evenFooter>  //CFEVEN 
				//首页页脚  <firstFooter>   FFIRST
				string key = string.Format("{0}{1}", "R", tag);
				if (headerFooter._Pictures.ContainsKey(key))
				{
					headerFooterText.RightAlignedImag = headerFooter._Pictures[key];
					headerFooter._Pictures.Remove(key);
				}
				key = string.Format("{0}{1}", "C", tag);
				if (headerFooter._Pictures.ContainsKey(key))
				{
					headerFooterText.CenteredImag = headerFooter._Pictures[key];
					headerFooter._Pictures.Remove(key);
				}
				key = string.Format("{0}{1}", "L", tag);
				if (headerFooter._Pictures.ContainsKey(key))
				{
					headerFooterText.LeftImag = headerFooter._Pictures[key];
					headerFooter._Pictures.Remove(key);
				}
			}
		}

		private void ReadWorkSheet_headerFooter_FooterText(ref HeaderFooterText headerText, string text)
		{
			//string text = childNode.Value;
			string code = text.Substring(0, 2);
			int startPos = 2;
			for (int pos = startPos; pos < text.Length - 2; pos++)
			{
				string newCode = text.Substring(pos, 2);
				if (newCode == "&C" || newCode == "&R")
				{
					SetText(ref headerText, code, text.Substring(startPos, pos - startPos));
					startPos = pos + 2;
					pos = startPos;
					code = newCode;
				}
			}

			SetText(ref headerText, code, text.Substring(startPos, text.Length - startPos));
		}

		private void SetText(ref HeaderFooterText headerText, string code, string text)
		{
			switch (code)
			{
				case "&L":
					headerText.LeftAlignedText = text;
					break;
				case "&C":
					headerText.CenteredText = text;
					break;
				default:
					headerText.RightAlignedText = text;
					break;
			}
		}

		private void ReadWorkSheet_sheetProtection(WorkSheet target, XElement sheetNode)
		{
			foreach (XAttribute xt in sheetNode.Attributes())
			{
				target.SheetProtection.Attributes.Add(xt.Name.LocalName, xt.Value);
			}
		}

		private void ReadWorkSheet_hyperlinks(WorkSheet target, XElement sheetNode)
		{
			foreach (XElement node in sheetNode.Nodes())
			{
				int fromRow, fromCol, toRow, toCol;
				ExcelHelper.GetRowColFromAddress(node.Attribute(XName.Get("ref")).Value, out fromRow, out fromCol, out toRow, out toCol);
				if (node.Attribute(XName.Get("id", ExcelCommon.Schema_Relationships)) != null)
				{
					string relationshipsId = node.Attribute(XName.Get("id", ExcelCommon.Schema_Relationships)).Value;

					ExcelHyperLink urlcellLinks = new ExcelHyperLink(this.Context.Package.GetPart(target.SheetUri).GetRelationship(relationshipsId).TargetUri.AbsoluteUri, UriKind.Absolute);

					if (node.Attribute("tooltip") != null)
					{
						urlcellLinks.ToolTip = node.Attribute("tooltip").Value;
					}
					target.Cells[fromRow, fromCol]._Hyperlink = urlcellLinks;
				}
				else if (node.Attribute("location") != null)
				{
					ExcelHyperLink locationcellLinks = new ExcelHyperLink(node.Attribute("location").Value, node.Attribute("display").Value);
					locationcellLinks.RowSpann = toRow - fromRow;
					locationcellLinks.ColSpann = toCol - fromCol;
					if (node.Attribute("tooltip") != null)
					{
						locationcellLinks.ToolTip = node.Attribute("tooltip").Value;
					}

					target.Cells[fromRow, fromCol]._Hyperlink = locationcellLinks;
				}
			}
		}

		private void ReadWorkSheet_sheetViews(WorkSheet target, XElement sheetNode)
		{
			foreach (XElement node in sheetNode.Nodes())
			{
				if (string.Compare(node.Name.LocalName, "sheetView") == 0)
				{
					if (node.Attribute("tabSelected") != null)
					{
						target.SheetView.TabSelected = int.Parse(node.Attribute("tabSelected").Value) == 1 ? true : false;
					}

					if (node.Attribute("showFormulas") != null)
					{
						target.SheetView.ShowFormulas = int.Parse(node.Attribute("showFormulas").Value) == 1 ? true : false;
					}

					if (node.Attribute("showGridLines") == null)
					{
						target.SheetView.ShowGridLines = true;
					}
					else
					{
						target.SheetView.ShowGridLines = int.Parse(node.Attribute("showGridLines").Value) == 1 ? true : false;
					}

					if (node.Attribute("showZeros") != null)
					{
						target.SheetView.ShowZeros = int.Parse(node.Attribute("showZeros").Value) == 1 ? true : false;
					}

					if (node.Attribute("showRowColHeaders") != null)
					{
						target.SheetView.ShowRowColHeaders = int.Parse(node.Attribute("showRowColHeaders").Value) == 1 ? true : false;
					}

					if (node.Attribute("showWhiteSpace") != null)
					{
						target.SheetView.ShowWhiteSpace = int.Parse(node.Attribute("showWhiteSpace").Value) == 1 ? true : false;
					}

					if (node.Attribute("zoomScale") != null)
					{
						target.SheetView.ZoomScale = int.Parse(node.Attribute("zoomScale").Value);
					}

					if (node.Attribute("topLeftCell") != null)
					{
						target.SheetView.TopLeftCell = CellAddress.Parse(node.Attribute("topLeftCell").Value);
					}

					foreach (XElement selectionNode in node.Nodes())
					{
						switch (selectionNode.Name.LocalName)
						{
							case "selection":
								if (selectionNode.Attribute("sqref") != null)
								{
									target.SheetView.SelectedRange = Range.Parse(target, selectionNode.Attribute("sqref").Value);
								}
								break;
							case "pane":
								//todo: 
								break;
							case "pivotSelection":
								//todo: 
								break;

						}
					}
				}
			}

		}

		private void ReadWorkSheet_mergeCells(WorkSheet target, XElement sheetNode)
		{
			foreach (XElement node in sheetNode.Nodes())
			{
				Range currentRang = Range.Parse(target, node.Attribute("ref").Value);
				target._MergeCells.Add(currentRang);
				for (int i = currentRang.StartColumn; i <= currentRang.EndColumn; i++)
				{
					for (int j = currentRang.StartRow; j <= currentRang.EndRow; j++)
					{
						target.Cells[j, i].IsMerge = true;
					}
				}
			}
		}

		private void ReadWorkSheet_sheetPr(WorkSheet target, XElement sheetNode)
		{
			ReadWorkSheet_sheetPr_Attributes(target, sheetNode);

			foreach (XElement childNode in sheetNode.Nodes())
			{
				switch (childNode.Name.LocalName)
				{
					case "outlinePr":
						ReadWorkSheet_sheetPr_outlinePr(target, childNode);
						break;
					case "pageSetUpPr":
						ReadWorkSheet_sheetPr_pageSetUpPr(target, childNode);
						break;
					case "tabColor":
						ReadWorkSheet_sheetPr_tabColor(target, childNode);
						break;
				}
			}
		}

		private void ReadWorkSheet_sheetPr_Attributes(WorkSheet target, XElement sheetPropertiesNode)
		{
			//todo: 获取相关Attributes
			/*
			 "syncHorizontal", 
			"syncVertical", 
			"syncRef", 
			"transitionEvaluation", 
			"transitionEntry", 
			"published", 
			"codeName", 
			"filterMode", 
			"enableFormatConditionsCalculation"
			 */
			XAttribute property = sheetPropertiesNode.Attribute("codeName");
			if (property != null)
			{
				target.SheetCode = property.Value;
			}
		}

		private void ReadWorkSheet_sheetPr_tabColor(WorkSheet target, XElement childNode)
		{
			target.TabColor = new ColorXmlWrapper();
			this.ReadStyles_Color(childNode, target.TabColor);
		}

		private void ReadWorkSheet_sheetPr_pageSetUpPr(WorkSheet target, XElement childNode)
		{
			if (childNode.Attribute("autoPageBreaks") != null)
			{
				target.PageSetup.ShowAutoPageBreaks = int.Parse(childNode.Attribute("autoPageBreaks").Value) == 1 ? true : false;
			}
			if (childNode.Attribute("fitToPage") != null)
			{
				target.PageSetup.FitToPage = int.Parse(childNode.Attribute("autoPageBreaks").Value) == 1 ? true : false;
			}
		}

		private void ReadWorkSheet_sheetPr_outlinePr(WorkSheet target, XElement childNode)
		{
			if (childNode.Attribute("applyStyles") != null)
			{
				target.OutLineApplyStyle = int.Parse(childNode.Attribute("applyStyles").Value) == 1 ? true : false;
			}
			if (childNode.Attribute("showOutlineSymbols") != null)
			{
				target.ShowOutlineSymbols = int.Parse(childNode.Attribute("showOutlineSymbols").Value) == 1 ? true : false;
			}
			if (childNode.Attribute("summaryBelow") != null)
			{
				target.OutLineSummaryBelow = int.Parse(childNode.Attribute("summaryBelow").Value) == 1 ? true : false;
			}
			if (childNode.Attribute("summaryRight") != null)
			{
				target.OutLineSummaryRight = int.Parse(childNode.Attribute("summaryRight").Value) == 1 ? true : false;
			}
		}

		private void ReadWorkSheet_printOptions(WorkSheet target, XElement sheetNode)
		{
			if (sheetNode.Attribute("headings") != null)
			{
				target.PageSetup.ShowHeaders = int.Parse(sheetNode.Attribute("headings").Value) == 1 ? true : false;
			}

			if (sheetNode.Attribute("gridLines") != null)
			{
				target.PageSetup.ShowGridLines = int.Parse(sheetNode.Attribute("gridLines").Value) == 1 ? true : false;
			}

			if (sheetNode.Attribute("horizontalCentered") != null)
			{
				target.PageSetup.HorizontalCentered = int.Parse(sheetNode.Attribute("horizontalCentered").Value) == 1 ? true : false;
			}

			if (sheetNode.Attribute("verticalCentered") != null)
			{
				target.PageSetup.VerticalCentered = int.Parse(sheetNode.Attribute("verticalCentered").Value) == 1 ? true : false;
			}
		}

		private void ReadWorkSheet_pageSetup(WorkSheet target, XElement sheetNode)
		{
			if (sheetNode.Attribute("blackAndWhite") != null)
			{
				target.PageSetup.BlackAndWhite = int.Parse(sheetNode.Attribute("blackAndWhite").Value) == 1 ? true : false;
			}

			if (sheetNode.Attribute("draft") != null)
			{
				target.PageSetup.Draft = int.Parse(sheetNode.Attribute("draft").Value) == 1 ? true : false;
			}

			if (sheetNode.Attribute("copies") != null)
			{
				target.PageSetup.NumberOfCopiesToPrint = int.Parse(sheetNode.Attribute("copies").Value);
			}

			if (sheetNode.Attribute("firstPageNumber") != null)
			{
				target.PageSetup.FirstPageNumber = int.Parse(sheetNode.Attribute("firstPageNumber").Value);
			}

			if (sheetNode.Attribute("fitToHeight") != null)
			{
				target.PageSetup.FitToHeight = int.Parse(sheetNode.Attribute("fitToHeight").Value);
			}

			if (sheetNode.Attribute("fitToWidth") != null)
			{
				target.PageSetup.FitToWidth = int.Parse(sheetNode.Attribute("fitToWidth").Value);
			}

			if (sheetNode.Attribute("scale") != null)
			{
				target.PageSetup.Scale = int.Parse(sheetNode.Attribute("scale").Value);
			}

			if (sheetNode.Attribute("orientation") != null)
			{
				target.PageSetup.Orientation = (ExcelOrientation)Enum.Parse(typeof(ExcelOrientation), sheetNode.Attribute("orientation").Value, true);
			}

			if (sheetNode.Attribute("pageOrder") != null)
			{
				target.PageSetup.PageOrder = (ExcelPageOrder)Enum.Parse(typeof(ExcelPageOrder), sheetNode.Attribute("pageOrder").Value, true);
			}

			if (sheetNode.Attribute("paperSize") != null)
			{
				target.PageSetup.PaperSize = (ExcelPaperSize)int.Parse(sheetNode.Attribute("paperSize").Value);
			}
		}

		private void ReadWorkSheet_pageMargins(WorkSheet target, XElement sheetNode)
		{
			if (sheetNode.Attribute("left") != null)
			{
				target.PageSetup.LeftMargin = decimal.Parse(sheetNode.Attribute("left").Value);
			}
			if (sheetNode.Attribute("right") != null)
			{
				target.PageSetup.RightMargin = decimal.Parse(sheetNode.Attribute("right").Value);
			}
			if (sheetNode.Attribute("top") != null)
			{
				target.PageSetup.TopMargin = decimal.Parse(sheetNode.Attribute("top").Value);
			}
			if (sheetNode.Attribute("bottom") != null)
			{
				target.PageSetup.BottomMargin = decimal.Parse(sheetNode.Attribute("bottom").Value);
			}
			if (sheetNode.Attribute("header") != null)
			{
				target.PageSetup.HeaderMargin = decimal.Parse(sheetNode.Attribute("header").Value);
			}
			if (sheetNode.Attribute("footer") != null)
			{
				target.PageSetup.FooterMargin = decimal.Parse(sheetNode.Attribute("footer").Value);
			}
		}

		private void ReadWorkSheet_tableParts(WorkSheet target, XElement sheetNode)
		{
			foreach (XElement node in sheetNode.Nodes())
			{
				if (string.Compare(node.Name.LocalName, "tablePart") == 0)
				{
					Table tb = new Table(target, node.Attribute(XName.Get("id", ExcelCommon.Schema_Relationships)).Value);
					((IPersistable)tb).Load(this.Context);
				}
			}
		}

		/// <summary>
		/// FileName:sheet1.xml
		/// <para>NodePath:worksheet/cols/</para>
		/// </summary>
		/// <param name="bookViewsRoot"></param>
		internal void ReadWorkSheet_cols(WorkSheet target, XElement item)
		{
			foreach (XElement node in item.Nodes())
			{
				int min = int.Parse(node.Attribute("min").Value);
				int max = int.Parse(node.Attribute("max").Value);
				Column col = new Column(min);
				col.ColumnMax = max;
				int style = 0;
				if (node.Attribute("style") != null)
				{
					if (int.TryParse(node.Attribute("style").Value, out style))
					{
						col.StyleID = style;
						col.Style = this.Context.GlobalStyles.CellXfs[style];
					}
				}

				col.Width = node.Attribute("width") == null ? 0 : double.Parse(node.Attribute("width").Value, CultureInfo.InvariantCulture);
				col.BestFit = node.Attribute("bestFit") != null && node.Attribute("bestFit").Value == "1" ? true : false;
				col.Collapsed = node.Attribute("collapsed") != null && node.Attribute("collapsed").Value == "1" ? true : false;
				col.Phonetic = node.Attribute("phonetic") != null && node.Attribute("phonetic").Value == "1" ? true : false;
				col.OutlineLevel = node.Attribute("outlineLevel") == null ? 0 : int.Parse(node.Attribute("outlineLevel").Value, CultureInfo.InvariantCulture);
				col.Hidden = node.Attribute("hidden") != null && node.Attribute("hidden").Value == "1" ? true : false;
				target.Columns.Add(col);
				if (max > min)
				{
					for (int i = min + 1; i <= max; i++)
					{
						target.Columns.Add(col.Clone(i));
					}
				}
			}
		}

		/// <summary>
		/// FileName:sheet1.xml
		/// <para>NodePath:worksheet/sheetData</para>
		/// </summary>
		/// <param name="bookViewsRoot"></param>
		internal void ReadWorkSheet_sheetData(WorkSheet target, XElement item)
		{
			foreach (XElement rowNode in item.Nodes())
			{
				int rowIndex = Convert.ToInt32(rowNode.Attribute("r").Value);

				if (rowNode.Attributes().Count() > 2 || (rowNode.Attributes().Count() == 2 && rowNode.Attribute("spans") != null))
				{
					Row newRow = new Row(rowIndex);
					ReadWorkSheet_sheetData_row(newRow, rowNode);
					target.Rows.Add(newRow);
				}

				foreach (XElement cNode in rowNode.Nodes())
				{
					Cell newCell = ReadWorkSheet_sheetData_row_c(target, cNode);

					foreach (XElement node in cNode.Nodes())
					{
						switch (node.Name.LocalName)
						{
							case "v":
								ReadWorkSheet_sheetData_row_c_v(newCell, node);
								break;
							case "f":
								ReadWorkSheet_sheetData_row_c_f(target, newCell, node);
								break;
						}
					}
					target.Cells.Add(newCell);
				}
			}
		}

		/// <summary>
		/// FileName:sheet1.xml
		/// <para>NodePath:worksheet/sheetData/row</para>
		/// </summary>
		/// <param name="bookViewsRoot"></param>
		internal void ReadWorkSheet_sheetData_row(Row target, XElement item)
		{
			target.Collapsed = item.Attribute("collapsed") != null && item.Attribute("collapsed").Value == "1" ? true : false;
			if (item.Attribute("ht") != null)
			{
				target.Height = double.Parse(item.Attribute("ht").Value, CultureInfo.InvariantCulture);
			}
			target.Hidden = item.Attribute("hidden") != null && item.Attribute("hidden").Value == "1" ? true : false; ;
			target.OutlineLevel = item.Attribute("outlineLevel") == null ? 0 : int.Parse(item.Attribute("outlineLevel").Value, CultureInfo.InvariantCulture); ;
			target.Phonetic = item.Attribute("ph") != null && item.Attribute("ph").Value == "1" ? true : false; ;
			if (item.Attribute("s") != null)
			{
				target.StyleID = int.Parse(item.Attribute("s").Value);
				target.Style = this.Context.GlobalStyles.CellXfs[target.StyleID];
			}
			//r.StyleID = xr.GetAttribute("s") == null ? 0 : int.Parse(xr.GetAttribute("s"), CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// FileName:sheet1.xml
		/// <para>NodePath:worksheet/sheetData/row/c</para>
		/// </summary>
		/// <param name="bookViewsRoot"></param>
		internal Cell ReadWorkSheet_sheetData_row_c(WorkSheet target, XElement item)
		{
			int rowIndex, columnIndex;
			ExcelHelper.GetRowCol(item.Attribute("r").Value, out rowIndex, out columnIndex, true);

			Row row = null;
			Column col = null;

			if (target.Rows.ContainsKey(rowIndex))
			{
				row = target.Rows[rowIndex];
			}
			else
			{
				row = new Row(rowIndex);
				target.Rows.Add(row);
			}

			if (target.Columns.ContainsKey(columnIndex))
			{
				col = target.Columns[columnIndex];
			}
			else
			{
				col = new Column(columnIndex);
				target.Columns.Add(col);
			}

			Cell cell = new Cell(row, col);
			if (item.Attribute("t") != null)
			{
				cell.DataType = item.Attribute("t").Value;
			}
			if (item.Attribute("s") == null)
			{
				cell.StyleID = 0;
				cell.Style = this.Context.GlobalStyles.CellStyleXfs[0];
			}
			else
			{
				cell.StyleID = int.Parse(item.Attribute("s").Value);
				cell.Style = this.Context.GlobalStyles.CellXfs[cell.StyleID];
			}

			return cell;
		}

		/// <summary>
		/// FileName:sheet1.xml
		/// <para>NodePath:worksheet/sheetData/row/c/f</para>
		/// </summary>
		/// <param name="bookViewsRoot"></param>
		internal void ReadWorkSheet_sheetData_row_c_f(WorkSheet worksheet, Cell target, XElement item)
		{
			string t = string.Empty;
			if (item.Attribute("t") != null)
			{
				t = item.Attribute("t").Value;
			}

			if (string.IsNullOrEmpty(t))
			{
				target.Formula = item.Value;
				return;
			}

			switch (t)
			{
				case "shared":
					string si = item.Attribute("si").Value;
					if (si.IsNotEmpty())
					{
						target.FormulaSharedIndex = int.Parse(si);

						if (item.Attribute("ref") != null)
						{
							Formulas f = new Formulas();
							f.Address = Range.Parse(worksheet, item.Attribute("ref").Value);
							f.Formula = item.Value;
							f.Index = target.FormulaSharedIndex;
							worksheet._SharedFormulas.Add(item.Value, f);
						}
					}
					break;
				case "dataTable":
					//todo:ref="D22:N32" dt2D="1" dtr="1" r1="D94" r2="D95" ca="1" 
					if (item.Attribute("ref") != null)
					{
						Formulas f = new Formulas();
						f.Address = Range.Parse(worksheet, item.Attribute("ref").Value);

						item.Attribute("dt2D").IsNotNull(v => f.Dt2D = ((XAttribute)v).Value);
						item.Attribute("dtr").IsNotNull(v => f.Dtr = ((XAttribute)v).Value);
						item.Attribute("r1").IsNotNull(v => f.R1 = ((XAttribute)v).Value);
						item.Attribute("r2").IsNotNull(v => f.R2 = ((XAttribute)v).Value);
						item.Attribute("ca").IsNotNull(v => f.Ca = ((XAttribute)v).Value);

						target.HasDataTable = true;
						worksheet._DataTableFormulas.Add(target, f);
					}
					break;
				case "array":
					//todo:
					break;
			}
		}


		/// <summary>
		/// FileName:sheet1.xml
		/// <para>NodePath:worksheet/sheetData/row/c/v</para>
		/// </summary>
		/// <param name="bookViewsRoot"></param>
		internal void ReadWorkSheet_sheetData_row_c_v(Cell target, XElement item)
		{
			switch (target.DataType)
			{
				case "s":
					int ix = int.Parse(item.Value);
					target.Value = this.Context.SharedStrings[ix].Text;
					target.IsRichText = this.Context.SharedStrings[ix].IsRichText;
					break;
				case "str":
					target.Value = item.Value;
					break;
				case "b":
					target.Value = item.Value != "0";
					break;
				case "d":
					target.Value = CellValueConvertyDateTime(item.Value);
					break;
				default:
					int n = target.Style.NumberFormat.NumFmtId;
					string v = item.Value;
					if ((n >= 14 && n <= 22) || (n >= 45 && n <= 47))
					{
						DateTime cellReslut = CellValueConvertyDateTime(v);

						if (cellReslut != default(DateTime))
						{
							target.DataType = "d";
							target.Value = cellReslut;
						}
						else
						{
							target.Value = string.Empty;
						}
					}
					else
					{
						double d;
						if (double.TryParse(v, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
						{
							target.Value = d;
							//DateTime.FromOADate(d);
						}
						else
						{
							target.Value = double.NaN;
						}
					}
					break;
			}
		}

		private DateTime CellValueConvertyDateTime(string celllvalue)
		{
			DateTime reslut = default(DateTime);
			double res;
			if (double.TryParse(celllvalue, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
			{
				reslut = DateTime.FromOADate(res);
			}
			return reslut;
		}

		#endregion

		#region "Table.xml"
		internal void ReadTable(Table target, XElement tableRoot)
		{
			foreach (XAttribute xt in tableRoot.Attributes())
			{
				target.Attributes[xt.Name.LocalName] = xt.Value;
			}

			target.Address = Range.Parse(target._WorkSheet, tableRoot.Attribute("ref").Value);

			foreach (XElement tableNode in tableRoot.Nodes())
			{
				switch (tableNode.Name.LocalName)
				{
					case "tableColumns":
						ReadTable_tableColumns(target, tableNode);
						break;
					case "tableStyleInfo":
						ReadTable_tableStyleInfo(target, tableNode);
						break;
				}
			}
			target._WorkSheet.Tables.Add(target);
		}

		private void ReadTable_tableStyleInfo(Table target, XElement tableNode)
		{
			if (tableNode != null)
			{
				if (tableNode.Attribute("name") != null)
				{
					target.StyleName = tableNode.Attribute("name").Value;
				}

				if (tableNode.Attribute("showFirstColumn") != null)
				{
					target.ShowFirstColumn = int.Parse(tableNode.Attribute("showFirstColumn").Value) == 1 ? true : false;
				}

				if (tableNode.Attribute("showLastColumn") != null)
				{
					target.ShowLastColumn = int.Parse(tableNode.Attribute("showLastColumn").Value) == 1 ? true : false;
				}

				if (tableNode.Attribute("showRowStripes") != null)
				{
					target.ShowRowStripes = int.Parse(tableNode.Attribute("showRowStripes").Value) == 1 ? true : false;
				}

				if (tableNode.Attribute("showColumnStripes") != null)
				{
					target.ShowColumnStripes = int.Parse(tableNode.Attribute("showColumnStripes").Value) == 1 ? true : false;
				}
			}
		}

		internal void ReadTable_tableColumns(Table target, XElement tableColumnsNode)
		{
			int columnIndex = 0;
			foreach (XElement columnNode in tableColumnsNode.Nodes())
			{
				TableColumn tc = new TableColumn(target, columnNode.Attribute("name").Value, columnIndex);

				foreach (XAttribute xt in columnNode.Attributes())
				{
					tc.Attributes[xt.Name.LocalName] = xt.Value;
				}

				if (columnNode.FirstNode != null)
				{
					XElement columnFormulaNode = (XElement)columnNode.FirstNode;
					if (columnFormulaNode != null)
						tc.ColumnFormula = columnFormulaNode.Value;
				}
				target.Columns.LoadAdd(tc);
				columnIndex++;
			}
		}

		#endregion

		#region "comments1.xml"
		internal void ReadSheetComments(CommentCollection target, XElement commentElement)
		{
			List<string> authors = new List<string>();
			XElement childNode = commentElement.Element(XName.Get("authors", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (childNode != null)
			{
				IEnumerable<XElement> authorsNodes = childNode.Elements(XName.Get("author", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
				foreach (XElement author in authorsNodes)
				{
					authors.Add(author.Value);
				}
			}

			childNode = commentElement.Element(XName.Get("commentList", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (childNode != null)
			{
				IEnumerable<XElement> commentListNode = childNode.Elements(XName.Get("comment", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
				int index = 0;
				List<Comment> driwingComments = this.Context.DrawingShapes[target._WorkSheet.Name];
				foreach (XElement commentMode in commentListNode)
				{
					if (commentMode.Attribute("ref") != null)
					{
						Cell cell = target._WorkSheet.Cells[commentMode.Attribute("ref").Value];
						Comment cellComment = driwingComments[index];
						cellComment._Cell = cell;
						ReadSheetComments_Comment(authors, commentMode, cellComment);
						cell._Comment = cellComment;
						index++;
					}
				}
			}
		}

		internal void ReadSheetComments_Comment(List<string> authors, XElement commentMode, Comment cellComment)
		{
			if (commentMode.Attribute("authorId") != null)
			{
				cellComment.Author = authors[int.Parse(commentMode.Attribute("authorId").Value)];
			}

			XElement textPropertyNode = commentMode.Element(XName.Get("text", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
			if (textPropertyNode != null)
			{
				IEnumerable<XElement> richTextNodes = textPropertyNode.Elements(XName.Get("r", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
				foreach (XElement richtextNode in richTextNodes)
				{
					RichText rt = new RichText();
					XElement rtnode = richtextNode.Element(XName.Get("t", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
					if (rtnode != null)
					{
						if (rtnode.Attribute(XName.Get("space", ExcelCommon.Schema_Xml)) != null)
						{
							rt.PreserveSpace = true;
						}
						rt.Text = rtnode.Value;
					}

					rtnode = richtextNode.Element(XName.Get("rPr", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
					if (rtnode != null)
					{
						XElement rPrNode = rtnode.Element(XName.Get("b", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
						if (rPrNode != null)
						{
							rt.Bold = true;
						}

						rPrNode = rtnode.Element(XName.Get("charset", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
						if (rPrNode != null)
						{
							if (rPrNode.Attribute(XName.Get("val", ExcelCommon.Schema_WorkBook_Main.NamespaceName)) != null)
							{
								rt.Charset = int.Parse(rPrNode.Attribute(XName.Get("val", ExcelCommon.Schema_WorkBook_Main.NamespaceName)).Value);
							}
						}
						rPrNode = rtnode.Element(XName.Get("color", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
						if (rPrNode != null)
						{
							ColorXmlWrapper rtColor = new ColorXmlWrapper();
							ReadStyles_Color(rPrNode, rtColor);
							rt._DataBarColor = rtColor;
						}
						rPrNode = rtnode.Element(XName.Get("condense", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
						if (rPrNode != null)
						{
							rt.Condense = true;
						}
						rPrNode = rtnode.Element(XName.Get("extend", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
						if (rPrNode != null)
						{
							rt.Extend = true;
						}

						//todo: family
						//rPrNode = richtextNode.Element(XName.Get("family", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
						//if (rPrNode != null)
						//{
						//    if (rPrNode.Attribute(XName.Get("val", ExcelCommon.Schema_WorkBook_Main.NamespaceName)) != null)
						//    {
						//        rt.FontName = rPrNode.Attribute(XName.Get("val", ExcelCommon.Schema_WorkBook_Main.NamespaceName)).Value;
						//    }
						//}

						rPrNode = rtnode.Element(XName.Get("i", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
						if (rPrNode != null)
						{
							rt.Italic = true;
						}

						rPrNode = rtnode.Element(XName.Get("outline", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
						if (rPrNode != null)
						{
							rt.Outline = true;
						}

						rPrNode = rtnode.Element(ExcelCommon.Schema_WorkBook_Main + "rFont");
						if (rPrNode != null)
						{
							if (rPrNode.Attribute("val") != null)
							{
								rt.FontName = rPrNode.Attribute("val").Value;
							}
						}

						rPrNode = rtnode.Element(XName.Get("scheme", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
						if (rPrNode != null)
						{
							if (rPrNode.Attribute("val") != null)
							{
								rt.Scheme = rPrNode.Attribute("val").Value;
							}
						}

						rPrNode = rtnode.Element(XName.Get("shadow", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
						if (rPrNode != null)
						{
							rt.Shadow = true;
						}

						rPrNode = rtnode.Element(XName.Get("strike", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
						if (rPrNode != null)
						{
							rt.Strike = true;
						}

						rPrNode = rtnode.Element(XName.Get("sz", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
						if (rPrNode != null)
						{
							if (rPrNode.Attribute("val") != null)
							{
								rt.Size = float.Parse(rPrNode.Attribute("val").Value);
							}
						}

						rPrNode = rtnode.Element(XName.Get("u", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
						if (rPrNode != null)
						{
							rt.UnderLine = true;
						}

						rPrNode = rtnode.Element(XName.Get("vertAlign", ExcelCommon.Schema_WorkBook_Main.NamespaceName));
						if (rPrNode != null)
						{
							if (rPrNode.Attribute("val") != null)
							{
								rt.VerticalAlign = (ExcelVerticalAlignmentFont)Enum.Parse(typeof(ExcelVerticalAlignmentFont), rPrNode.Attribute("val").Value, true); ;
							}
						}
					}

					cellComment.RichText.Add(rt);
				}
			}
		}

		#endregion

		#region "Comment——vmlDrawing.vml"
		internal void ReadSheetCommentVmlDrawing(CommentCollection target, XElement vmlDrawingElement)
		{
			XElement childNode = vmlDrawingElement.Element(XName.Get("shapelayout", ExcelCommon.schemaMicrosoftOffice));
			if (childNode != null)
			{

			}
			childNode = vmlDrawingElement.Element(XName.Get("shapetype", ExcelCommon.schemaMicrosoftVml));
			if (childNode != null)
			{

			}
			ReadSheetCommentVmlDrawing_shape(target, vmlDrawingElement);
		}

		internal void ReadSheetCommentVmlDrawing_shape(CommentCollection target, XElement vmlDrawingElement)
		{
			IEnumerable<XElement> shapetypes = vmlDrawingElement.Elements(XName.Get("shape", ExcelCommon.schemaMicrosoftVml));

			List<Comment> listComment = new List<Comment>();
			foreach (XElement shapetypeNode in shapetypes)
			{
				Comment ct = new Comment();

				ReadSheetCommentVmlDrawing_shape_BackgroundColor(shapetypeNode, ct);
				ReadSheetCommentVmlDrawing_shape_stroke(shapetypeNode, ct);
				ReadSheetCommentVmlDrawing_shape_strokecolor(shapetypeNode, ct);
				ReadSheetCommentVmlDrawing_shape_strokeweight(shapetypeNode, ct);
				ReadSheetCommentVmlDrawing_shape_textbox(shapetypeNode, ct);

				if (shapetypeNode.Attribute(XName.Get("id", ExcelCommon.schemaMicrosoftVml)) != null)
				{
					ct.ID = shapetypeNode.Attribute(XName.Get("id", ExcelCommon.schemaMicrosoftVml)).Value;
				}

				ReadSheetCommentVmlDrawing_shape_ClientData(target, shapetypeNode, ct);

				listComment.Add(ct);
			}

			this.Context.DrawingShapes.Add(target._WorkSheet.Name, listComment);
		}

		private void ReadSheetCommentVmlDrawing_shape_textbox(XElement shapetypeNode, Comment ct)
		{
			XElement textboxNode = shapetypeNode.Element(XName.Get("textbox", ExcelCommon.schemaMicrosoftVml));
			if (textboxNode != null)
			{
				if (textboxNode.Attribute(XName.Get("style")) != null)
				{
					string value = textboxNode.Attribute(XName.Get("style")).Value;
					GetStyle(value, "mso-fit-shape-to-text", out value);
					if (value == "t")
					{
						ct.AutoFit = true;
					}
				}
			}
		}

		private void ReadSheetCommentVmlDrawing_shape_strokeweight(XElement shapetypeNode, Comment ct)
		{
			if (shapetypeNode.Attribute(XName.Get("strokeweight")) != null)
			{
				string wt = shapetypeNode.Attribute(XName.Get("strokeweight")).Value;
				if (!string.IsNullOrEmpty(wt))
				{
					if (wt.EndsWith("pt"))
						wt = wt.Substring(0, wt.Length - 2);

					Single ret;
					if (Single.TryParse(wt, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out ret))
					{
						ct.LineWidth = ret;
					}
					else
					{
						ct.LineWidth = 0;
					}
				}
			}
		}

		private void ReadSheetCommentVmlDrawing_shape_strokecolor(XElement shapetypeNode, Comment comment)
		{
			if (shapetypeNode.Attribute(XName.Get("strokecolor")) != null)
			{
				string col = shapetypeNode.Attribute(XName.Get("strokecolor")).Value;
				if (!string.IsNullOrEmpty(col))
				{
					if (col.StartsWith("#")) col = col.Substring(1, col.Length - 1);
					int res;
					if (int.TryParse(col, System.Globalization.NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out res))
					{
						comment.LineColor = Color.FromArgb(res);
					}
					else
					{
						comment.LineColor = Color.Empty;
					}
				}
			}
		}

		private void ReadSheetCommentVmlDrawing_shape_stroke(XElement shapetypeNode, Comment comment)
		{
			XElement stroke = shapetypeNode.Element(XName.Get("stroke", ExcelCommon.schemaMicrosoftVml));
			if (stroke != null)
			{
				if (stroke.Attribute(XName.Get("dashstyle")) != null)
				{
					string v = stroke.Attribute(XName.Get("dashstyle")).Value;
					if (string.IsNullOrEmpty(v))
					{
						comment.LineStyle = ExcelLineStyleVml.Solid;
					}
					else if (v == "1 1")
					{
						v = stroke.Attribute(XName.Get("endcap")).Value;
						comment.LineStyle = (ExcelLineStyleVml)Enum.Parse(typeof(ExcelLineStyleVml), v, true);
					}
					else
					{
						comment.LineStyle = (ExcelLineStyleVml)Enum.Parse(typeof(ExcelLineStyleVml), v, true);
					}
				}
			}
		}

		private void ReadSheetCommentVmlDrawing_shape_BackgroundColor(XElement shapetypeNode, Comment comment)
		{
			if (shapetypeNode.Attribute(XName.Get("fillcolor")) != null)
			{
				string col = shapetypeNode.Attribute(XName.Get("fillcolor")).Value;
				if (!string.IsNullOrEmpty(col))
				{
					if (col.StartsWith("#"))
					{
						col = col.Substring(1, col.Length - 1);
					}
					int res;
					if (int.TryParse(col, System.Globalization.NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out res))
					{
						comment.BackgroundColor = Color.FromArgb(res);
					}
					else
					{
						comment.BackgroundColor = ReadSheetCommentVmlDrawing_shape_Fill(shapetypeNode);
					}
				}
				else
				{
					comment.BackgroundColor = ReadSheetCommentVmlDrawing_shape_Fill(shapetypeNode);
				}
			}
			else
			{
				comment.BackgroundColor = ReadSheetCommentVmlDrawing_shape_Fill(shapetypeNode);
			}
		}

		private Color ReadSheetCommentVmlDrawing_shape_Fill(XElement shapetypeNode)
		{
			Color result = Color.Empty;
			XElement fillNode = shapetypeNode.Element(XName.Get("fill", ExcelCommon.schemaMicrosoftVml));
			if (fillNode != null)
			{
				if (fillNode.Attribute(XName.Get("color2")) != null)
				{
					string col = fillNode.Attribute(XName.Get("color2")).Value;
					if (col.StartsWith("#"))
					{
						col = col.Substring(1, col.Length - 1);
					}
					int res;
					if (int.TryParse(col, System.Globalization.NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out res))
					{
						result = Color.FromArgb(res);
					}
				}
			}
			return result;
		}

		internal void ReadSheetCommentVmlDrawing_shape_ClientData(CommentCollection target, XElement clientDataElement, Comment comment)
		{
			XElement childElement = clientDataElement.Element(XName.Get("ClientData", ExcelCommon.schemaMicrosoftExcel));
			if (childElement != null)
			{
				ReadSheetCommentVmlDrawing_shape_ClientData_TextVAlign(childElement, comment);

				ReadSheetCommentVmlDrawing_shape_ClientData_TextHAlign(childElement, comment);

				ReadSheetCommentVmlDrawing_shape_ClientData_Visible(childElement, comment);

				ReadSheetCommentVmlDrawing_shape_ClientData_Locked(childElement, comment);

				ReadSheetCommentVmlDrawing_shape_ClientData_AutoFill(childElement, comment);

				ReadSheetCommentVmlDrawing_shape_ClientData_LockText(childElement, comment);

				ReadSheetCommentVmlDrawing_shape_ClientData_anchor(childElement, comment);
			}
		}

		private void ReadSheetCommentVmlDrawing_shape_ClientData_AutoFill(XElement childElement, Comment comment)
		{
			XElement clientChildNode = childElement.Element(XName.Get("AutoFill", ExcelCommon.schemaMicrosoftExcel));
			if (clientChildNode != null)
			{
				comment.Locked = clientChildNode.Value.Equals("True") ? true : false;
			}
		}

		private void ReadSheetCommentVmlDrawing_shape_ClientData_anchor(XElement childElement, Comment comment)
		{
			XElement clientChildNode = childElement.Element(XName.Get("anchor", ExcelCommon.schemaMicrosoftExcel));
			if (clientChildNode != null)
			{
				string anchor = clientChildNode.Value;
				if (string.IsNullOrEmpty(anchor))
				{
					string[] numbers = anchor.Split(',');
					if (numbers.Length == 8)
					{
						int ret;
						if (int.TryParse(numbers[2], out ret))
						{
							comment.From.Row = ret;
						}
						if (int.TryParse(numbers[3], out ret))
						{
							comment.From.RowOffset = ret;
						}
						if (int.TryParse(numbers[0], out ret))
						{
							comment.From.Column = ret;
						}
						if (int.TryParse(numbers[1], out ret))
						{
							comment.From.ColumnOffset = ret;
						}

						if (int.TryParse(numbers[6], out ret))
						{
							comment.To.Row = ret;
						}
						if (int.TryParse(numbers[8], out ret))
						{
							comment.To.RowOffset = ret;
						}
						if (int.TryParse(numbers[4], out ret))
						{
							comment.To.Column = ret;
						}
						if (int.TryParse(numbers[5], out ret))
						{
							comment.To.ColumnOffset = ret;
						}
					}
				}
			}
		}

		private void ReadSheetCommentVmlDrawing_shape_ClientData_LockText(XElement childElement, Comment comment)
		{
			XElement clientChildNode = childElement.Element(XName.Get("LockText", ExcelCommon.schemaMicrosoftExcel));
			if (clientChildNode != null)
			{
				comment.LockText = clientChildNode.Value.Equals("1") ? true : false;
			}
		}

		private void ReadSheetCommentVmlDrawing_shape_ClientData_Locked(XElement childElement, Comment comment)
		{
			XElement clientChildNode = childElement.Element(XName.Get("Locked", ExcelCommon.schemaMicrosoftExcel));
			if (clientChildNode != null)
			{
				comment.Locked = clientChildNode.Value.Equals("1") ? true : false;
			}
		}

		private void ReadSheetCommentVmlDrawing_shape_ClientData_Visible(XElement childElement, Comment comment)
		{
			XElement clientChildNode = childElement.Element(XName.Get("Visible", ExcelCommon.schemaMicrosoftExcel));
			if (clientChildNode != null)
			{
				comment.Visible = true;
			}
		}

		private void ReadSheetCommentVmlDrawing_shape_ClientData_TextHAlign(XElement childElement, Comment comment)
		{
			XElement clientChildNode = childElement.Element(XName.Get("TextHAlign", ExcelCommon.schemaMicrosoftExcel));
			if (clientChildNode != null)
			{
				switch (clientChildNode.Value)
				{
					case "Center":
						comment.HorizontalAlignment = ExcelTextAlignHorizontalVml.Center;
						break;
					case "Right":
						comment.HorizontalAlignment = ExcelTextAlignHorizontalVml.Right;
						break;
				}
			}
		}

		private void ReadSheetCommentVmlDrawing_shape_ClientData_TextVAlign(XElement childElement, Comment comment)
		{
			XElement clientChildNode = childElement.Element(XName.Get("TextVAlign", ExcelCommon.schemaMicrosoftExcel));
			if (clientChildNode != null)
			{
				switch (clientChildNode.Value)
				{
					case "Center":
						comment.VerticalAlignment = ExcelTextAlignVerticalVml.Center;
						break;
					case "Bottom":
						comment.VerticalAlignment = ExcelTextAlignVerticalVml.Bottom;
						break;
				}
			}
		}

		public static bool GetStyle(string style, string key, out string value)
		{
			string[] styles = style.Split(';');
			foreach (string s in styles)
			{
				if (s.IndexOf(':') > 0)
				{
					string[] split = s.Split(':');
					if (split[0] == key)
					{
						value = split[1];

						return true;
					}
				}
				else if (s == key)
				{
					value = string.Empty;

					return true;
				}
			}
			value = string.Empty;

			return false;
		}

		#endregion

		#region "HeaderFooterVmlDrawingPicture——vmlDrawing.vml"
		internal void ReadHeaderFooterVmlDrawingPicture(VmlDrawingPictureCollection target, XElement targetElement)
		{
			XElement childNode = targetElement.Element(XName.Get("shapelayout", ExcelCommon.schemaMicrosoftOffice));
			if (childNode != null)
			{

			}
			childNode = targetElement.Element(XName.Get("shapetype", ExcelCommon.schemaMicrosoftVml));
			if (childNode != null)
			{

			}
			ReadHeaderFooterVmlDrawingPicture_shape(target, targetElement);
		}

		private void ReadHeaderFooterVmlDrawingPicture_shape(VmlDrawingPictureCollection target, XElement targetElement)
		{
			//奇数页眉  <oddHeader>  // 右 “RH”  中 "CH"  左 "LH"
			//偶数页眉  <evenHeader> // 右 "RHEVEN"  中间 “CHEVEN", 左 "LHEVEN" 
			//首页页眉  <firstHeader> // 右 ”RHFIRST“ 中间 "CHFIRST" 左 "LHFIRST"

			//奇数页脚  <oddFooter>   //F
			//偶数页脚  <evenFooter>  //CFEVEN 
			//首页页脚  <firstFooter>   FFIRST

			IEnumerable<XElement> shapetypes = targetElement.Elements(XName.Get("shape", ExcelCommon.schemaMicrosoftVml));
			VmlDrawingPicture itemPicture = null;
			XAttribute xAtbute = null;
			XElement childNode = null;
			foreach (XElement shapetypeNode in shapetypes)
			{
				itemPicture = new VmlDrawingPicture();
				xAtbute = shapetypeNode.Attribute(XName.Get("id"));
				if (xAtbute != null)
					itemPicture.Id = xAtbute.Value;

				xAtbute = shapetypeNode.Attribute(XName.Get("style"));
				if (xAtbute != null)
					ReadHeaderFooterVmlDrawingPicture_shape_style(itemPicture, xAtbute.Value);

				childNode = shapetypeNode.Element(XName.Get("imagedata", ExcelCommon.schemaMicrosoftVml));
				if (childNode != null)
				{
					xAtbute = childNode.Attribute(XName.Get("relid", ExcelCommon.schemaMicrosoftOffice));
					if (xAtbute != null)
					{
						PackageRelationship rel = this.Context.Package.GetPart(target.PictureUri).GetRelationship(xAtbute.Value);
						Uri imageUri = PackUriHelper.ResolvePartUri(rel.SourceUri, rel.TargetUri);
						itemPicture.Image = Image.FromStream(this.Context.Package.GetPart(imageUri).GetStream());
					}

					xAtbute = childNode.Attribute(XName.Get("title", ExcelCommon.schemaMicrosoftOffice));
					if (xAtbute != null)
						itemPicture.Title = xAtbute.Value;

					xAtbute = childNode.Attribute(XName.Get("bilevel"));
					if (xAtbute != null)
						itemPicture.BiLevel = xAtbute.Value.Equals("t") ? true : false;

					xAtbute = childNode.Attribute(XName.Get("grayscale"));
					if (xAtbute != null)
						itemPicture.BlackLevel = VmlDrawingPicture.GetFracDT(xAtbute.Value, 0);

					xAtbute = childNode.Attribute(XName.Get("gain"));
					if (xAtbute != null)
						itemPicture.Gain = VmlDrawingPicture.GetFracDT(xAtbute.Value, 0);

					xAtbute = childNode.Attribute(XName.Get("gamma"));
					if (xAtbute != null)
						itemPicture.Gamma = VmlDrawingPicture.GetFracDT(xAtbute.Value, 0);
				}

				/* <o:lock v:ext="edit" rotation="t"/>
				childNode = targetElement.Element(XName.Get("lock", ExcelCommon.schemaMicrosoftOffice));
				if (childNode != null)
				{
					itemPicture.Title = childNode.Value;
				} */

				target.Add(itemPicture);
			}
		}

		public void ReadHeaderFooterVmlDrawingPicture_shape_style(VmlDrawingPicture itemPicture, string style)
		{
			foreach (string prop in style.Split(';'))
			{
				string[] split = prop.Split(':');
				if (split.Length > 1)
				{
					switch (split[0])
					{
						case "width":
							itemPicture.Width = ReadHeaderFooterVmlDrawingPicture_shape_style_property(split);
							break;
						case "height":
							itemPicture.Height = ReadHeaderFooterVmlDrawingPicture_shape_style_property(split);
							break;
						case "left":
							itemPicture.Left = ReadHeaderFooterVmlDrawingPicture_shape_style_property(split);
							break;
						case "top":
							itemPicture.Top = ReadHeaderFooterVmlDrawingPicture_shape_style_property(split);
							break;
					}
				}
			}
		}

		public double ReadHeaderFooterVmlDrawingPicture_shape_style_property(string[] split)
		{
			string value = split[1].EndsWith("pt") ? split[1].Substring(0, split[1].Length - 2) : split[1];
			double ret;
			if (double.TryParse(value, out ret))
			{
				return ret;
			}
			else
			{
				return 0;
			}
		}
		#endregion

		#region "worksheet drawing.xml"
		internal void ReadWrokSheetDrawings(DrawingCollection target, XElement targetElement, ExcelLoadContext context)
		{

			IEnumerable<XElement> childNodes = targetElement.Elements(XName.Get("twoCellAnchor", ExcelCommon.Schema_SheetDrawings));
			ExcelDrawing drTarget = null;
			foreach (XElement xnode in childNodes)
			{
				XElement childNode = xnode.Element(XName.Get("sp", ExcelCommon.Schema_SheetDrawings));
				if (childNode != null)
				{
					//return new Shape(target);
				}

				childNode = xnode.Element(XName.Get("pic", ExcelCommon.Schema_SheetDrawings));
				if (childNode != null)
				{
					ExcelPicture drPictureTarget = new ExcelPicture(target._WorkSheet);
					ReadWrokSheetDrawings_DrawingPic(drPictureTarget, childNode, target.DrawingUri);
					drTarget = drPictureTarget;
				}

				childNode = xnode.Element(XName.Get("graphicFrame", ExcelCommon.Schema_SheetDrawings));
				if (childNode != null)
				{
					string rId = xnode.Element(XName.Get("graphicFrame", ExcelCommon.Schema_SheetDrawings)).Element(XName.Get("graphic", ExcelCommon.Schema_Drawings)).Element(XName.Get("graphicData", ExcelCommon.Schema_Drawings)).Element(XName.Get("chart", ExcelCommon.Schema_Chart)).LastAttribute.Value;
					PackageRelationship drawingsRelation = context.Package.GetPart(target.DrawingUri).GetRelationship(rId);
					Uri chartUri = PackUriHelper.ResolvePartUri(drawingsRelation.SourceUri, drawingsRelation.TargetUri);
					XElement drawingsElement = context.Package.GetXElementFromUri(chartUri);

					string chartType = ((XElement)(drawingsElement.Element(XName.Get("chart", ExcelCommon.Schema_Chart)).Element(XName.Get("plotArea", ExcelCommon.Schema_Chart)).Element(XName.Get("layout", ExcelCommon.Schema_Chart)).NextNode)).Name.LocalName;
					XElement element = childNode.Element(XName.Get("nvGraphicFramePr", ExcelCommon.Schema_SheetDrawings)).Element(XName.Get("cNvPr", ExcelCommon.Schema_SheetDrawings));
					if (element != null)
					{
						string chartName = element.Attribute(XName.Get("name")).Value;
						ExcelChartType chartTypeEnum;
						Enum.TryParse(chartType.Replace("Chart", string.Empty), true, out chartTypeEnum);
						ExcelChart chart = target.AddChart(chartName, chartTypeEnum);
						chart.DrawingUri = target.DrawingUri;
						chart.RelationshipID = rId;
						((IPersistable)chart).Load(context);
						drTarget = chart;
					}
				}
				if (drTarget != null)
				{
					ReadWrokSheetDrawings_Attribute(drTarget, xnode);
					ReadWrokSheetDrawings_clientData(drTarget, xnode);
					ReadWrokSheetDrawings_Common_Position(drTarget, xnode);
				}

			}

		}

		private void ReadWrokSheetDrawings_Attribute(ExcelDrawing drTarget, XElement xnode)
		{
			XAttribute xt = xnode.Attribute(XName.Get("editAs"));
			if (xt != null)
				drTarget.EditAs = (ExcelEditAs)Enum.Parse(typeof(ExcelEditAs), xt.Value, true);
		}

		private void ReadWrokSheetDrawings_Common_Position(ExcelDrawing drTarget, XElement xnode)
		{
			XElement childNode = xnode.Element(XName.Get("from", ExcelCommon.Schema_SheetDrawings));
			if (childNode != null)
			{
				drTarget._From = new DrawingPosition();
				ReadWrokSheetDrawings_Position(drTarget._From, childNode);
			}

			childNode = xnode.Element(XName.Get("to", ExcelCommon.Schema_SheetDrawings));
			if (childNode != null)
			{
				drTarget._To = new DrawingPosition();
				ReadWrokSheetDrawings_Position(drTarget._To, childNode);
			}
		}

		private void ReadWrokSheetDrawings_Position(DrawingPosition drawingPosition, XElement xnode)
		{
			XElement childNode = xnode.Element(XName.Get("col", ExcelCommon.Schema_SheetDrawings));
			if (childNode != null)
				drawingPosition.Column = GetInt(childNode.Value);

			childNode = xnode.Element(XName.Get("colOff", ExcelCommon.Schema_SheetDrawings));
			if (childNode != null)
				drawingPosition.ColumnOff = GetInt(childNode.Value);

			childNode = xnode.Element(XName.Get("row", ExcelCommon.Schema_SheetDrawings));
			if (childNode != null)
				drawingPosition.Row = GetInt(childNode.Value);

			childNode = xnode.Element(XName.Get("rowOff", ExcelCommon.Schema_SheetDrawings));
			if (childNode != null)
				drawingPosition.RowOff = GetInt(childNode.Value);
		}

		private void ReadWrokSheetDrawings_clientData(ExcelDrawing target, XElement xnode)
		{
			XElement childNode = xnode.Element(XName.Get("clientData", ExcelCommon.Schema_SheetDrawings));
			if (childNode != null)
			{
				XAttribute xt = childNode.Attribute(XName.Get("fLocksWithSheet"));
				if (xt != null)
					target.Locked = xt.Value.Equals("1") ? true : false;

				xt = childNode.Attribute(XName.Get("fPrintsWithSheet"));
				if (xt != null)
					target.Print = xt.Value.Equals("1") ? true : false;
			}
		}

		private void ReadWrokSheetDrawings_DrawingPic(ExcelPicture target, XElement xnode, Uri drawingUri)
		{
			XElement childNode = xnode.Element(XName.Get("nvPicPr", ExcelCommon.Schema_SheetDrawings));
			if (childNode != null)
			{
				XElement picChildNode = childNode.Element(XName.Get("cNvPr", ExcelCommon.Schema_SheetDrawings));
				if (picChildNode != null)
				{
					XAttribute xaName = picChildNode.Attribute(XName.Get("name"));
					if (xaName != null)
						target.Name = xaName.Value;


				}
			}
			childNode = xnode.Element(XName.Get("blipFill", ExcelCommon.Schema_SheetDrawings));
			if (childNode != null)
			{
				XElement blipFillNode = childNode.Element(XName.Get("blip", ExcelCommon.Schema_Drawings));
				if (blipFillNode != null)
				{
					XAttribute r = blipFillNode.Attribute(XName.Get("embed", ExcelCommon.Schema_Relationships));
					if (r != null)
					{
						PackagePart drawingsPart = this.Context.Package.GetPart(drawingUri);
						PackageRelationship drawingRelation = drawingsPart.GetRelationship(r.Value);
						Uri uriPic = PackUriHelper.ResolvePartUri(drawingUri, drawingRelation.TargetUri);

						PackagePart part = drawingsPart.Package.GetPart(uriPic);
						FileInfo picInfo = new FileInfo(uriPic.OriginalString);
						target.ImageFormat = ExcelHelper.GetImageFormat(picInfo.Extension);
						target.ContentType = ExcelHelper.GetContentType(picInfo.Extension);
						target.Image = Image.FromStream(part.GetStream());
					}
				}
			}

			childNode = xnode.Element(XName.Get("spPr", ExcelCommon.Schema_SheetDrawings));
			if (childNode != null)
				ReadWrokSheetDrawings_DrawingPic_spr(target, childNode);
		}

		private void ReadWrokSheetDrawings_DrawingPic_spr(ExcelPicture target, XElement blipFillNode)
		{
			XElement childElement = blipFillNode.Element(XName.Get("solidFill", ExcelCommon.Schema_Drawings));
			if (childElement != null)
			{
				target._Fill = new DrawingFill();
				ReadWrokSheetDrawings_DrawingPic_spr_fill(target._Fill, childElement);
			}

			childElement = blipFillNode.Element(XName.Get("noFill"));
			if (childElement != null)
			{
				target._Fill = new DrawingFill();
				target._Fill.FillStyle = ExcelDrawingFillStyle.NoFill;
			}

			childElement = blipFillNode.Element(XName.Get("ln", ExcelCommon.Schema_Drawings));
			if (childElement != null)
			{
				target._Border = new DrawingBorder();
				ReadWrokSheetDrawings_DrawingPic_spr_ln(target, childElement);
			}
		}

		private void ReadWrokSheetDrawings_DrawingPic_spr_ln(ExcelPicture target, XElement lnElement)
		{
			XAttribute xbLn = lnElement.Attribute(XName.Get("w"));
			if (xbLn != null)
			{
				target.Border.Width = GetInt(xbLn.Value) / 12700;
			}
			xbLn = lnElement.Attribute(XName.Get("cap"));
			if (xbLn != null)
			{
				target.Border.LineCap = TranslateLineCap(xbLn.Value);
			}

			XElement fillNode = lnElement.Element(XName.Get("solidFill", ExcelCommon.Schema_Drawings));
			if (fillNode != null)
			{
				target.Border._Fill = new DrawingFill();
				ReadWrokSheetDrawings_DrawingPic_spr_fill(target.Border._Fill, fillNode);
			}
			fillNode = lnElement.Element(XName.Get("prstDash", ExcelCommon.Schema_Drawings));
			if (fillNode != null)
			{
				xbLn = fillNode.Attribute(XName.Get("val"));
				if (xbLn != null)
				{
					target.Border.LineStyle = TranslateLineStyle(xbLn.Value);
				}
			}
		}

		private void ReadWrokSheetDrawings_DrawingPic_spr_fill(DrawingFill drawingFill, XElement fillElement)
		{
			XElement childNode = fillElement.Element(XName.Get("srgbClr", ExcelCommon.Schema_Drawings));
			if (childNode != null)
			{
				XAttribute valXB = childNode.Attribute(XName.Get("val"));
				if (valXB != null)
				{
					drawingFill.Color = Color.FromArgb(int.Parse(valXB.Value, System.Globalization.NumberStyles.AllowHexSpecifier));
				}

				XElement alphaNode = childNode.Element(XName.Get("alpha", ExcelCommon.Schema_Drawings));
				if (alphaNode != null)
				{
					valXB = alphaNode.Attribute(XName.Get("val"));
					if (valXB != null)
					{
						drawingFill.Transparancy = 100 - (int.Parse(valXB.Value) / 1000);
					}
				}
			}
		}

		internal static int GetInt(string value)
		{
			int i = int.MinValue;
			if (int.TryParse(value, out i))
			{
				return i;
			}
			return i;
		}

		#endregion

		#region chart.xml

		internal void ReadPieCharts(ExcelChart target, XElement targetElement, ExcelLoadContext context)
		{
			XElement childNode = targetElement.Element(XName.Get("chart", ExcelCommon.Schema_Chart)).Element(XName.Get("plotArea", ExcelCommon.Schema_Chart)).Element(XName.Get("pie3DChart", ExcelCommon.Schema_Chart));
			if (childNode != null)
			{
				ReadPieCharts_varyColors(target, childNode);
				ReadPieCharts_dLbls(target, childNode);
				ReadPieCharts_ser(target, childNode);
			}
		}

		private void ReadPieCharts_varyColors(ExcelChart target, XElement targetElement)
		{
			XElement childNode = targetElement.Element(XName.Get("varyColors", ExcelCommon.Schema_Chart));
			if (childNode != null)
			{
				target.VaryColors = childNode.Attribute("val").Value == "0" ? false : true;
			}
		}
		private void ReadPieCharts_ser(ExcelChart target, XElement targetElement)
		{
			XElement serElement = targetElement.Element(XName.Get("ser", ExcelCommon.Schema_Chart));
			ReadPieCharts_dLbls(target, serElement);
			ReadPieCharts_cat(target, serElement);
			ReadPieCharts_val(target, serElement);
		}

		private void ReadPieCharts_dLbls(ExcelChart target, XElement targetElement)
		{
			XElement childNode = targetElement.Element(XName.Get("dLbls", ExcelCommon.Schema_Chart));
			if (childNode != null)
			{
				target.DataLabel.ShowLegendKey = childNode.Element(XName.Get("showLegendKey", ExcelCommon.Schema_Chart)).Attribute("val").Value == "0" ? false : true;
				target.DataLabel.ShowValue = childNode.Element(XName.Get("showVal", ExcelCommon.Schema_Chart)).Attribute("val").Value == "0" ? false : true;
				target.DataLabel.ShowCategory = childNode.Element(XName.Get("showCatName", ExcelCommon.Schema_Chart)).Attribute("val").Value == "0" ? false : true;
				target.DataLabel.ShowSeriesName = childNode.Element(XName.Get("showSerName", ExcelCommon.Schema_Chart)).Attribute("val").Value == "0" ? false : true;
				target.DataLabel.ShowPercent = childNode.Element(XName.Get("showPercent", ExcelCommon.Schema_Chart)).Attribute("val").Value == "0" ? false : true;
				target.DataLabel.ShowBubbleSize = childNode.Element(XName.Get("showBubbleSize", ExcelCommon.Schema_Chart)).Attribute("val").Value == "0" ? false : true;
				target.DataLabel.ShowLeaderLines = childNode.Element(XName.Get("showLeaderLines", ExcelCommon.Schema_Chart)).Attribute("val").Value == "0" ? false : true;
			}
		}


		private void ReadPieCharts_cat(ExcelChart target, XElement targetElement)
		{
			XElement childNode = targetElement.Element(XName.Get("cat", ExcelCommon.Schema_Chart));
			ReadPieCharts_strRef(target, childNode);

		}

		private void ReadPieCharts_val(ExcelChart target, XElement targetElement)
		{
			XElement childNode = targetElement.Element(XName.Get("val", ExcelCommon.Schema_Chart));
			ReadPieCharts_numRef(target, childNode);

		}


		private void ReadPieCharts_numRef(ExcelChart target, XElement targetElement)
		{
			XElement childNode = targetElement.Element(XName.Get("numRef", ExcelCommon.Schema_Chart));
			ReadPieCharts_valf(target, childNode);
			ReadPieCharts_numCache(target, childNode);
		}

		private void ReadPieCharts_valf(ExcelChart target, XElement targetElement)
		{
			XElement childNode = targetElement.Element(XName.Get("f", ExcelCommon.Schema_Chart));
			target.CategoryAxisValue.NumberReference.Formula = childNode.Value;
		}


		private void ReadPieCharts_strRef(ExcelChart target, XElement targetElement)
		{
			XElement childNode = targetElement.Element(XName.Get("strRef", ExcelCommon.Schema_Chart));
			ReadPieCharts_f(target, childNode);
			ReadPieCharts_strCache(target, childNode);
		}

		private void ReadPieCharts_numCache(ExcelChart target, XElement targetElement)
		{
			XElement childNode = targetElement.Element(XName.Get("numCache", ExcelCommon.Schema_Chart));

			ReadPieCharts_numpt(target, childNode);
		}

		private void ReadPieCharts_f(ExcelChart target, XElement targetElement)
		{
			XElement childNode = targetElement.Element(XName.Get("f", ExcelCommon.Schema_Chart));
			target.CategoryAxisData.StringReference.Formula = childNode.Value;
		}

		private void ReadPieCharts_strCache(ExcelChart target, XElement targetElement)
		{
			XElement childNode = targetElement.Element(XName.Get("strCache", ExcelCommon.Schema_Chart));

			ReadPieCharts_pt(target, childNode);
		}

		private void ReadPieCharts_pt(ExcelChart target, XElement targetElement)
		{

			IEnumerable<XElement> childNodes = targetElement.Elements(XName.Get("pt", ExcelCommon.Schema_Chart));
			foreach (XElement xnode in childNodes)
			{
				target.CategoryAxisData.StringReference.StringPointCollection.Add(new StringPoint()
				{
					PointIndex = xnode.Attribute(XName.Get("idx")).Value,
					PointValue = xnode.Value,
				});
			}
		}

		private void ReadPieCharts_numpt(ExcelChart target, XElement targetElement)
		{

			IEnumerable<XElement> childNodes = targetElement.Elements(XName.Get("pt", ExcelCommon.Schema_Chart));
			foreach (XElement xnode in childNodes)
			{
				target.CategoryAxisValue.NumberReference.NumericPointCollection.Add(new NumericPoint()
				{
					PointIndex = xnode.Attribute(XName.Get("idx")).Value,
					PointValue = xnode.Value,
				});
			}
		}
		#endregion

		#region Commmon

		internal static ExcelDrawingLineStyle TranslateLineStyle(string text)
		{
			switch (text)
			{
				case "dash":
				case "dot":
				case "dashDot":
				case "solid":
					return (ExcelDrawingLineStyle)Enum.Parse(typeof(ExcelDrawingLineStyle), text, true);
				case "lgDash":
				case "lgDashDot":
				case "lgDashDotDot":
					return (ExcelDrawingLineStyle)Enum.Parse(typeof(ExcelDrawingLineStyle), "Long" + text.Substring(2, text.Length - 2));
				case "sysDash":
				case "sysDashDot":
				case "sysDashDotDot":
				case "sysDot":
					return (ExcelDrawingLineStyle)Enum.Parse(typeof(ExcelDrawingLineStyle), "System" + text.Substring(3, text.Length - 3));
				default:
					throw (new Exception("解析没有找到指定的类型"));
			}
		}

		internal static ExcelDrawingLineCap TranslateLineCap(string text)
		{
			switch (text)
			{
				case "rnd":
					return ExcelDrawingLineCap.Round;
				case "sq":
					return ExcelDrawingLineCap.Square;
				default:
					return ExcelDrawingLineCap.Flat;
			}
		}

		internal static ExcelTextAnchoringType GetTextAchoringEnum(string text)
		{
			switch (text)
			{
				case "b":
					return ExcelTextAnchoringType.Bottom;
				case "ctr":
					return ExcelTextAnchoringType.Center;
				case "dist":
					return ExcelTextAnchoringType.Distributed;
				case "just":
					return ExcelTextAnchoringType.Justify;
				default:
					return ExcelTextAnchoringType.Top;
			}
		}
		#endregion
	}
}

