using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;
using System.Text.RegularExpressions;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class NumberFormatXmlWrapper : StyleXmlBaseWrapper
	{
		public NumberFormatXmlWrapper(bool buildIn)
		{
			this.BuildIn = buildIn;
		}

		public NumberFormatXmlWrapper(int numFmtId, string Format, bool buildIn)
			: this(buildIn)
		{
			this._NumFmtId = numFmtId;
			this._Format = Format;
		}


		/// <summary>
		/// 如果True则生成，如果则是默认规格
		/// </summary>
		public bool BuildIn { get; private set; }

		private int _NumFmtId = 0;
		/// <summary>
		/// 格式化ID
		/// 
		/// 0   General 
		/// 1   0 
		/// 2   0.00 
		/// 3   #,##0 
		/// 4   #,##0.00 
		/// 9   0% 
		/// 10  0.00% 
		/// 11  0.00E+00 
		/// 12  # ?/? 
		/// 13  # ??/?? 
		/// 14  mm-dd-yy 
		/// 15  d-mmm-yy 
		/// 16  d-mmm 
		/// 17  mmm-yy 
		/// 18  h:mm AM/PM 
		/// 19  h:mm:ss AM/PM 
		/// 20  h:mm 
		/// 21  h:mm:ss 
		/// 22  m/d/yy h:mm 
		/// 37  #,##0 ;(#,##0) 
		/// 38  #,##0 ;[Red](#,##0) 
		/// 39  #,##0.00;(#,##0.00) 
		/// 40  #,##0.00;[Red](#,##0.00) 
		/// 45  mm:ss 
		/// 46  [h]:mm:ss 
		/// 47  mmss.0 
		/// 48  ##0.0E+0 
		/// 49  @
		/// </summary>            
		public int NumFmtId
		{
			get
			{
				return this._NumFmtId;
			}
			set
			{
				this._NumFmtId = value;
			}
		}

		internal override string Id
		{
			get
			{
				return this._Format;
			}
		}

		private const string FmtPath = "@formatCode";
		private string _Format = "General";
		public string Format
		{
			get
			{
				return this._Format;
			}
			set
			{

				int buildId = GetFromBuildIdFromFormat(value);
				//this._NumFmtId = GetFromBuildIdFromFormat(value);
				if (buildId == int.MinValue)
				{
					this.BuildIn = false;
				}
				this._Format = value;
			}
		}

		private string GetFromBuildInFromID(int numFmtId)
		{
			switch (numFmtId)
			{
				case 0:
					return "General";
				case 1:
					return "0";
				case 2:
					return "0.00";
				case 3:
					return "#,##0";
				case 4:
					return "#,##0.00";
				case 9:
					return "0%";
				case 10:
					return "0.00%";
				case 11:
					return "0.00E+00";
				case 12:
					return "# ?/?";
				case 13:
					return "# ??/??";
				case 14:
					return "mm-dd-yy";
				case 15:
					return "d-mmm-yy";
				case 16:
					return "d-mmm";
				case 17:
					return "mmm-yy";
				case 18:
					return "h:mm AM/PM";
				case 19:
					return "h:mm:ss AM/PM";
				case 20:
					return "h:mm";
				case 21:
					return "h:mm:ss";
				case 22:
					return "m/d/yy h:mm";
				case 37:
					return "#,##0 ;(#,##0)";
				case 38:
					return "#,##0 ;[Red](#,##0)";
				case 39:
					return "#,##0.00;(#,##0.00)";
				case 40:
					return "#,##0.00;[Red](#,#)";
				case 45:
					return "mm:ss";
				case 46:
					return "[h]:mm:ss";
				case 47:
					return "mmss.0";
				case 48:
					return "##0.0";
				case 49:
					return "@";
				default:
					return string.Empty;
			}
		}

		private int GetFromBuildIdFromFormat(string format)
		{
			switch (format)
			{
				case "General":
					return 0;
				case "0":
					return 1;
				case "0.00":
					return 2;
				case "#,##0":
					return 3;
				case "#,##0.00":
					return 4;
				case "0%":
					return 9;
				case "0.00%":
					return 10;
				case "0.00E+00":
					return 11;
				case "# ?/?":
					return 12;
				case "# ??/??":
					return 13;
				case "mm-dd-yy":
					return 14;
				case "d-mmm-yy":
					return 15;
				case "d-mmm":
					return 16;
				case "mmm-yy":
					return 17;
				case "h:mm AM/PM":
					return 18;
				case "h:mm:ss AM/PM":
					return 19;
				case "h:mm":
					return 20;
				case "h:mm:ss":
					return 21;
				case "m/d/yy h:mm":
					return 22;
				case "#,##0 ;(#,##0)":
					return 37;
				case "#,##0 ;[Red](#,##0)":
					return 38;
				case "#,##0.00;(#,##0.00)":
					return 39;
				case "#,##0.00;[Red](#,#)":
					return 40;
				case "mm:ss":
					return 45;
				case "[h]:mm:ss":
					return 46;
				case "mmss.0":
					return 47;
				case "##0.0":
					return 48;
				case "@":
					return 49;
				default:
					return int.MinValue;
			}
		}

		internal string GetNewID(int NumFmtId, string Format)
		{
			if (NumFmtId < 0)
			{
				NumFmtId = GetFromBuildIdFromFormat(Format);
			}

			return NumFmtId.ToString();
		}

		internal static void AddBuildIn(StyleCollection<NumberFormatXmlWrapper> NumberFormats)
		{
			NumberFormats.Add("General", new NumberFormatXmlWrapper(true) { NumFmtId = 0, Format = "General" });
			NumberFormats.Add("0", new NumberFormatXmlWrapper(true) { NumFmtId = 1, Format = "0" });
			NumberFormats.Add("0.00", new NumberFormatXmlWrapper(true) { NumFmtId = 2, Format = "0.00" });
			NumberFormats.Add("#,##0", new NumberFormatXmlWrapper(true) { NumFmtId = 3, Format = "#,##0" });
			NumberFormats.Add("#,##0.00", new NumberFormatXmlWrapper(true) { NumFmtId = 4, Format = "#,##0.00" });
			NumberFormats.Add("0%", new NumberFormatXmlWrapper(true) { NumFmtId = 9, Format = "0%" });
			NumberFormats.Add("0.00%", new NumberFormatXmlWrapper(true) { NumFmtId = 10, Format = "0.00%" });
			NumberFormats.Add("0.00E+00", new NumberFormatXmlWrapper(true) { NumFmtId = 11, Format = "0.00E+00" });
			NumberFormats.Add("# ?/?", new NumberFormatXmlWrapper(true) { NumFmtId = 12, Format = "# ?/?" });
			NumberFormats.Add("# ??/??", new NumberFormatXmlWrapper(true) { NumFmtId = 13, Format = "# ??/??" });
			NumberFormats.Add("mm-dd-yy", new NumberFormatXmlWrapper(true) { NumFmtId = 14, Format = "mm-dd-yy" });
			NumberFormats.Add("d-mmm-yy", new NumberFormatXmlWrapper(true) { NumFmtId = 15, Format = "d-mmm-yy" });
			NumberFormats.Add("d-mmm", new NumberFormatXmlWrapper(true) { NumFmtId = 16, Format = "d-mmm" });
			NumberFormats.Add("mmm-yy", new NumberFormatXmlWrapper(true) { NumFmtId = 17, Format = "mmm-yy" });
			NumberFormats.Add("h:mm AM/PM", new NumberFormatXmlWrapper(true) { NumFmtId = 18, Format = "h:mm AM/PM" });
			NumberFormats.Add("h:mm:ss AM/PM", new NumberFormatXmlWrapper(true) { NumFmtId = 19, Format = "h:mm:ss AM/PM" });
			NumberFormats.Add("h:mm", new NumberFormatXmlWrapper(true) { NumFmtId = 20, Format = "h:mm" });
			//NumberFormats.Add("h:mm:dd", new NumberFormatXmlWrapper(true) { NumFmtId = 21, Format = "h:mm:dd" });
			NumberFormats.Add("m/d/yy h:mm", new NumberFormatXmlWrapper(true) { NumFmtId = 22, Format = "m/d/yy h:mm" });
			NumberFormats.Add("#,##0 ;(#,##0)", new NumberFormatXmlWrapper(true) { NumFmtId = 37, Format = "#,##0 ;(#,##0)" });
			NumberFormats.Add("#,##0 ;[Red](#,##0)", new NumberFormatXmlWrapper(true) { NumFmtId = 38, Format = "#,##0 ;[Red](#,##0)" });
			NumberFormats.Add("#,##0.00;(#,##0.00)", new NumberFormatXmlWrapper(true) { NumFmtId = 39, Format = "#,##0.00;(#,##0.00)" });
			NumberFormats.Add("#,##0.00;[Red](#,#)", new NumberFormatXmlWrapper(true) { NumFmtId = 40, Format = "#,##0.00;[Red](#,#)" });
			NumberFormats.Add("mm:ss", new NumberFormatXmlWrapper(true) { NumFmtId = 45, Format = "mm:ss" });
			NumberFormats.Add("[h]:mm:ss", new NumberFormatXmlWrapper(true) { NumFmtId = 46, Format = "[h]:mm:ss" });
			NumberFormats.Add("mmss.0", new NumberFormatXmlWrapper(true) { NumFmtId = 47, Format = "mmss.0" });
			NumberFormats.Add("##0.0", new NumberFormatXmlWrapper(true) { NumFmtId = 48, Format = "##0.0" });
			NumberFormats.Add("@", new NumberFormatXmlWrapper(true) { NumFmtId = 49, Format = "@" });

			NumberFormats.NextId = 164;
		}

		public enum ExcelFormatType
		{
			Unknown = 0,
			Number = 1,
			DateTime = 2,
		}

		private FormatTranslatorWrappper _Translator = null;
		public FormatTranslatorWrappper FormatTranslator
		{
			get
			{
				if (this._Translator == null)
				{
					this._Translator = new FormatTranslatorWrappper(Format, NumFmtId);
				}

				return this._Translator;
			}
		}

		#region Excel --> .Net Format
		public class FormatTranslatorWrappper
		{
			internal FormatTranslatorWrappper(string format, int numFmtID)
			{
				if (numFmtID == 14)
				{
					NetFormat = NetFormatForWidth = "d";
					NetTextFormat = NetTextFormatForWidth = string.Empty;
					DataType = ExcelFormatType.DateTime;
				}
				else if (format.ToLower() == "general")
				{
					NetFormat = NetFormatForWidth = "#.#####";
					NetTextFormat = NetTextFormatForWidth = string.Empty;
					DataType = ExcelFormatType.Number;
				}
				else
				{
					ToNetFormat(format, false);
					ToNetFormat(format, true);
				}
			}

			internal string NetTextFormat { get; private set; }
			internal string NetFormat { get; private set; }

			private CultureInfo _ci;
			internal CultureInfo Culture
			{
				get
				{
					if (this._ci == null)
					{
						return CultureInfo.CurrentCulture;
					}
					return this._ci;
				}
				private set
				{
					this._ci = value;
				}
			}

			internal ExcelFormatType DataType { get; private set; }
			internal string NetTextFormatForWidth { get; private set; }
			internal string NetFormatForWidth { get; private set; }
			internal string FractionFormat { get; private set; }

			private void ToNetFormat(string ExcelFormat, bool forColWidth)
			{
				DataType = ExcelFormatType.Unknown;
				int secCount = 0;
				bool isText = false;
				bool isBracket = false;
				bool prevBslsh = false;
				bool useMinute = false;
				bool prevUnderScore = false;
				bool ignoreNext = false;
				int fractionPos = -1;
				bool containsAmPm = ExcelFormat.Contains("AM/PM");

				StringBuilder strb = new StringBuilder();
				Culture = null;
				string format = string.Empty, text = string.Empty, specialDateFormat = string.Empty, bracketText = string.Empty;

				char clc;
				if (containsAmPm)
				{
					ExcelFormat = Regex.Replace(ExcelFormat, "AM/PM", "");
					DataType = ExcelFormatType.DateTime;
				}

				for (int pos = 0; pos < ExcelFormat.Length; pos++)
				{
					char c = ExcelFormat[pos];
					if (c == '"')
					{
						isText = !isText;
					}
					else
					{
						if (ignoreNext)
						{
							ignoreNext = false;
							continue;
						}
						else if (isText && !isBracket)
						{
							strb.Append(c);
						}
						else if (isBracket)
						{
							if (c == ']')
							{
								isBracket = false;
								if (bracketText[0] == '$')
								{
									string[] li = Regex.Split(bracketText, "-");
									if (li[0].Length > 1)
									{
										strb.Append("\"" + li[0].Substring(1, li[0].Length - 1) + "\"");
									}
									if (li.Length > 1)
									{
										if (li[1].ToLower() == "f800")
										{
											specialDateFormat = "D";
										}
										else if (li[1].ToLower() == "f400")
										{
											specialDateFormat = "T";
										}
										else
										{
											int num = int.Parse(li[1], NumberStyles.HexNumber);
											try
											{
												Culture = CultureInfo.GetCultureInfo(num & 0xFFFF);
											}
											catch
											{
												Culture = null;
											}
										}
									}
								}
							}
							else
							{
								bracketText += c;
							}
						}
						else if (prevUnderScore)
						{
							if (forColWidth)
							{
								strb.AppendFormat("\"{0}\"", c);
							}
							prevUnderScore = false;
						}
						else
						{
							if (c == ';')
							{
								secCount++;
								if (DataType == ExcelFormatType.DateTime || secCount == 3)
								{
									format = strb.ToString();
									strb = new StringBuilder();
								}
								else
								{
									strb.Append(c);
								}
							}
							else
							{
								clc = c.ToString().ToLower()[0];

								if (DataType == ExcelFormatType.Unknown)
								{
									if (c == '0' || c == '#' || c == '.')
									{
										DataType = ExcelFormatType.Number;
									}
									else if (clc == 'y' || clc == 'm' || clc == 'd' || clc == 'h' || clc == 'm' || clc == 's')
									{
										DataType = ExcelFormatType.DateTime;
									}
								}

								if (prevBslsh)
								{
									strb.Append(c);
									prevBslsh = false;
								}
								else if (c == '[')
								{
									bracketText = string.Empty;
									isBracket = true;
								}
								else if (c == '\\')
								{
									prevBslsh = true;
								}
								else if (c == '0' ||
									c == '#' ||
									c == '.' ||
									c == ',' ||
									c == '%' ||
									clc == 'd' ||
									clc == 's')
								{
									strb.Append(c);
								}
								else if (clc == 'h')
								{
									if (containsAmPm)
									{
										strb.Append('h'); ;
									}
									else
									{
										strb.Append('H');
									}
									useMinute = true;
								}
								else if (clc == 'm')
								{
									if (useMinute)
									{
										strb.Append('m');
									}
									else
									{
										strb.Append('M');
									}
								}
								else if (c == '_') //Skip next but use for alignment
								{
									prevUnderScore = true;
								}
								else if (c == '?')
								{
									strb.Append(' ');
								}
								else if (c == '/')
								{
									if (DataType == ExcelFormatType.Number)
									{
										fractionPos = strb.Length;
										int startPos = pos - 1;
										while (startPos >= 0 &&
												(ExcelFormat[startPos] == '?' ||
												ExcelFormat[startPos] == '#' ||
												ExcelFormat[startPos] == '0'))
										{
											startPos--;
										}

										if (startPos > 0)  //RemovePart
										{
											strb.Remove(strb.Length - (pos - startPos - 1), (pos - startPos - 1));
										}

										int endPos = pos + 1;
										while (endPos < ExcelFormat.Length &&
												(ExcelFormat[endPos] == '?' ||
												ExcelFormat[endPos] == '#' ||
												(ExcelFormat[endPos] >= '0' && ExcelFormat[endPos] <= '9')))
										{
											endPos++;
										}
										pos = endPos;
										if (FractionFormat.IsNotEmpty())
										{
											FractionFormat = ExcelFormat.Substring(startPos + 1, endPos - startPos - 1);
										}
										strb.Append('?'); //Will be replaced later on by the fraction
									}
									else
									{
										strb.Append('/');
									}
								}
								else if (c == '*')
								{
									ignoreNext = true;
								}
								else if (c == '@')
								{
									strb.Append("{0}");
								}
								else
								{
									strb.Append(c);
								}
							}
						}
					}
				}

				if (containsAmPm)
				{
					format += "tt";
				}

				if (format.IsNullOrEmpty())
				{
					format = strb.ToString();
				}
				else
				{
					text = strb.ToString();
				}
				if (specialDateFormat.IsNotEmpty())
				{
					format = specialDateFormat;
				}

				if (forColWidth)
				{
					NetFormatForWidth = format;
					NetTextFormatForWidth = text;
				}
				else
				{
					NetFormat = format;
					NetTextFormat = text;
				}
				if (Culture == null)
				{
					Culture = CultureInfo.CurrentCulture;
				}
			}

			internal string FormatFraction(double d)
			{
				int numerator, denomerator;

				int intPart = (int)d;

				string[] fmt = FractionFormat.Split('/');

				int fixedDenominator;
				if (!int.TryParse(fmt[1], out fixedDenominator))
				{
					fixedDenominator = 0;
				}

				if (d == 0 || double.IsNaN(d))
				{
					if (fmt[0].Trim().IsNullOrEmpty() && fmt[1].Trim().IsNullOrEmpty())
					{
						return new string(' ', FractionFormat.Length);
					}
					else
					{
						return 0.ToString(fmt[0]) + "/" + 1.ToString(fmt[0]);
					}
				}

				int maxDigits = fmt[1].Length;
				string sign = d < 0 ? "-" : "";
				if (fixedDenominator == 0)
				{
					List<double> numerators = new List<double>() { 1, 0 };
					List<double> denominators = new List<double>() { 0, 1 };

					ExceptionHelper.TrueThrow(maxDigits < 1 && maxDigits > 12, "超出Rang位数(1-12)");

					int maxNum = 0;
					for (int i = 0; i < maxDigits; i++)
					{
						maxNum += 9 * (int)(Math.Pow((double)10, (double)i));
					}

					double divRes = 1 / ((double)Math.Abs(d) - intPart);
					double result, prevResult = double.NaN;
					int listPos = 2, index = 1;
					while (true)
					{
						index++;
						double intDivRes = Math.Floor(divRes);
						numerators.Add((intDivRes * numerators[index - 1] + numerators[index - 2]));
						if (numerators[index] > maxNum)
						{
							break;
						}

						denominators.Add((intDivRes * denominators[index - 1] + denominators[index - 2]));

						result = numerators[index] / denominators[index];
						if (denominators[index] > maxNum)
						{
							break;
						}
						listPos = index;

						if (result == prevResult) break;

						if (result == d) break;

						prevResult = result;

						divRes = 1 / (divRes - intDivRes);  //Rest
					}

					numerator = (int)numerators[listPos];
					denomerator = (int)denominators[listPos];
				}
				else
				{
					numerator = (int)Math.Round((d - intPart) / (1D / fixedDenominator), 0);
					denomerator = fixedDenominator;
				}
				if (numerator == denomerator || numerator == 0)
				{
					if (numerator == denomerator) intPart++;
					return sign + intPart.ToString(NetFormat).Replace("?", new string(' ', FractionFormat.Length));
				}
				else if (intPart == 0)
				{
					return sign + FmtInt(numerator, fmt[0]) + "/" + FmtInt(denomerator, fmt[1]);
				}
				else
				{
					return sign + intPart.ToString(NetFormat).Replace("?", FmtInt(numerator, fmt[0]) + "/" + FmtInt(denomerator, fmt[1]));
				}
			}

			private string FmtInt(double value, string format)
			{
				string v = value.ToString("#");
				string pad = string.Empty;
				if (v.Length < format.Length)
				{
					for (int i = format.Length - v.Length - 1; i >= 0; i--)
					{
						if (format[i] == '?')
						{
							pad += " ";
						}
						else if (format[i] == ' ')
						{
							pad += "0";
						}
					}
				}
				return pad + v;
			}
		}
		#endregion
	}
}
