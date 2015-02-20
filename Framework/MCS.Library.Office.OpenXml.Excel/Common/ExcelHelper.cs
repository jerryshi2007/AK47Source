using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Web;
using MCS.Library.Core;
using System.Data;

namespace MCS.Library.Office.OpenXml.Excel
{
    public static class ExcelHelper
    {
        /// <summary>
        /// 将.Net的数据类型转换为Excel的单元格数据类型
        /// b (Boolean)
        /// d (Date)
        /// e (Error)
        /// inlineStr (Inline String)
        /// n (Number)
        /// s (Shared String)
        /// str (String)
        /// </summary>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ToCellDataType(this Type type)
        {
            string result = "str";

            if (type != null)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                        result = "b";
                        break;
                    case TypeCode.DateTime:
                        result = "d";
                        break;
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.SByte:
                    case TypeCode.Single:
                        result = "n";
                        break;
                }
            }

            return result;
        }

        public static void AppendExcelOpenXmlHeader(this HttpResponse response, string fileNameWithoutExt)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(response != null, "response");

            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            response.AppendHeader("CONTENT-DISPOSITION",
                        string.Format("{0};filename={1}.xlsx", "inline", HttpUtility.UrlEncode(fileNameWithoutExt)));
        }

        [Obsolete("Stream有Copy方法，此方法没用")]
        public static void CopyStream(Stream sourceStream, Stream targetStream)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(sourceStream == null, "sourceStream");

            using (BinaryReader binaryReader = new BinaryReader(sourceStream))
            {
                byte[] buffer = new byte[4096];
                int num;
                do
                {
                    num = binaryReader.Read(buffer, 0, 4096);
                    if (num > 0)
                    {
                        targetStream.Write(buffer, 0, num);
                    }
                }
                while (num > 0);
            }
        }

        public static void GetRowColFromAddress(string cellAddress, out int fromRow, out int fromColumn, out int foRow, out int foColumn)
        {
            cellAddress = cellAddress.ToUpper();
            if (cellAddress.IndexOf(' ') > 0)
            {
                cellAddress = cellAddress.Substring(0, cellAddress.IndexOf(' '));
            }

            if (cellAddress.IndexOf(':') < 0)
            {
                GetRowCol(cellAddress, out fromRow, out fromColumn, true);
                foColumn = fromColumn;
                foRow = fromRow;
            }
            else
            {
                string[] cells = cellAddress.Split(':');
                GetRowCol(cells[0], out fromRow, out fromColumn, true);
                GetRowCol(cells[1], out foRow, out foColumn, true);

                if (fromColumn <= 0)
                    fromColumn = 1;
                if (fromRow <= 0)
                    fromRow = 1;
                if (foColumn <= 0)
                    foColumn = ExcelCommon.WorkSheet_MaxColumns;
                if (foRow <= 0)
                    foRow = ExcelCommon.WorkSheet_MaxRows;
            }
        }

        /// <summary>
        /// 获取单元格地址的行/列
        /// </summary>
        /// <param name="address">the address</param>
        /// <param name="row">returns the row</param>
        /// <param name="col">returns the column</param>
        /// <param name="throwException">如果无效返回false,抛出异常</param>
        /// <returns></returns>
        public static bool GetRowCol(string address, out int row, out int col, bool throwException)
        {
            bool colPart = true;
            string sRow = string.Empty, sCol = string.Empty;
            col = 0;
            if (address.IndexOf(':') > 0)
            {
                address = address.Substring(0, address.IndexOf(':'));
            }
            if (address.EndsWith("#REF!"))
            {
                row = 0;
                col = 0;
                return true;
            }

            int sheetMarkerIndex = address.IndexOf('!');
            if (sheetMarkerIndex >= 0)
            {
                address = address.Substring(sheetMarkerIndex + 1);
            }

            for (int i = 0; i < address.Length; i++)
            {
                if ((address[i] >= 'A' && address[i] <= 'Z') && colPart && sCol.Length <= 3)
                {
                    sCol += address[i];
                }
                else if (address[i] >= '0' && address[i] <= '9')
                {
                    sRow += address[i];
                    colPart = false;
                }
                else if (address[i] != '$') // $ is ignored here
                {
                    ExceptionHelper.TrueThrow<Exception>(throwException, string.Format("无效地址的地址{0}", address));
                    row = 0;
                    col = 0;

                    return false;
                }
            }

            // column number
            if (sCol.IsNotEmpty())
            {
                int len = sCol.Length - 1;
                for (int i = len; i >= 0; i--)
                {
                    col += (((int)sCol[i]) - 64) * (int)(Math.Pow(26, len - i));
                }
            }
            else
            {
                col = 0;
                int.TryParse(sRow, out row);
                return false;
            }

            // Get the row number
            if (sRow.IsNullOrEmpty())
            {
                row = 0;

                return false;
            }
            else
            {
                return int.TryParse(sRow, out row);
            }
        }

        public static string ParseEntireColumnSelections(string address)
        {
            string parsedAddress = address;
            MatchCollection matches = Regex.Matches(address, "[A-Z]+:[A-Z]+");
            foreach (Match match in matches)
            {
                AddRowNumbersToEntireColumnRange(ref parsedAddress, match.Value);
            }

            return parsedAddress;
        }

        private static void AddRowNumbersToEntireColumnRange(ref string address, string range)
        {
            string parsedRange = string.Format("{0}{1}", range, ExcelCommon.WorkSheet_MaxRows);
            string[] splitArr = parsedRange.Split(new char[] { ':' });
            address = address.Replace(range, string.Format("{0}1:{1}", splitArr[0], splitArr[1]));
        }

        public static int ColumnAddressToIndex(string columnAddress)
        {
            char[] base26 = columnAddress.ToCharArray();
            int total = 0;
            int place = 0;

            for (int i = (base26.Length - 1); i >= 0; i += -1)
            {
                char chr = char.ToUpper(base26[i]);
                int asc = (int)chr - 64;

                ExceptionHelper.TrueThrow<ApplicationException>(asc < 1 || asc > 26, string.Format("不存指定的列名 '{0}'.", columnAddress));
                if (place == 0)
                {
                    total = asc;
                    place = 26;
                }
                else
                {
                    total += (asc * place);
                    place *= 26;
                }
            }

            return total;
        }

        public static string GetColumnLetterFromNumber(int column)
        {
            ExceptionHelper.TrueThrow<ArgumentOutOfRangeException>(column <= 0 || column > ExcelCommon.WorkSheet_MaxColumns, "超出范围！");

            StringBuilder value = new StringBuilder(6);
            while (column > 0)
            {
                int residue = column % 26;
                column /= 26;
                if (residue == 0)
                {
                    residue = 26;
                    column--;
                }
                value.Insert(0, (char)(64 + residue));
            }

            return value.ToString();
        }

        public static string ExcelDecodeString(string t)
        {
            Match match = Regex.Match(t, "(_x005F|_x[0-9A-F]{4,4}_)");
            if (!match.Success)
            {
                return t;
            }

            bool useNextValue = false;
            StringBuilder ret = new StringBuilder();
            int prevIndex = 0;
            while (match.Success)
            {
                if (prevIndex < match.Index)
                {
                    ret.Append(t.Substring(prevIndex, match.Index - prevIndex));
                }
                if (!useNextValue && match.Value == "_x005F")
                {
                    useNextValue = true;
                }
                else
                {
                    if (useNextValue)
                    {
                        ret.Append(match.Value);
                        useNextValue = false;
                    }
                    else
                    {
                        ret.Append((char)int.Parse(match.Value.Substring(2, 4), NumberStyles.AllowHexSpecifier));
                    }
                }
                prevIndex = match.Index + match.Length;
                match = match.NextMatch();
            }
            ret.Append(t.Substring(prevIndex, t.Length - prevIndex));

            return ret.ToString();
        }

        public static void ExcelEncodeString(StreamWriter sw, string t)
        {
            if (Regex.IsMatch(t, "(_x[0-9A-F]{4,4}_)"))
            {
                var match = Regex.Match(t, "(_x[0-9A-F]{4,4}_)");
                int indexAdd = 0;
                while (match.Success)
                {
                    t = t.Insert(match.Index + indexAdd, "_x005F");
                    indexAdd += 6;
                    match = match.NextMatch();
                }
            }
            for (int i = 0; i < t.Length; i++)
            {
                if (t[i] < 0x1f && t[i] != '\t' && t[i] != '\n' && t[i] != '\r') //Not Tab, CR or LF
                {
                    sw.Write("_x00{0}_", (t[i] < 0xa ? "0" : "") + ((int)t[i]).ToString("X"));
                }
                else
                {
                    sw.Write(t[i]);
                }
            }
        }

        internal static string UserTableColumFormulaToExcelColum(string userColumFormula, string tableName)
        {
            StringBuilder newFormula = new StringBuilder(userColumFormula);
            string pattern = @"\[@[^\]]+\]";
            Match m = Regex.Match(userColumFormula, pattern);
            while (m.Success)
            {
                newFormula.Replace(m.Value, string.Format("{0}[[#This Row],{1}]", tableName, m.Value.Replace("@", string.Empty)));

                m = m.NextMatch();
            }

            return newFormula.ToString();
        }

        /// <summary>
        /// 根据文件的扩展名，返回存储Excel的类型
        /// </summary>
        /// <param name="extension">文件的扩展名（例：.jpg）</param>
        /// <returns></returns>
        internal static string GetContentType(string extension)
        {
            switch (extension.ToLower())
            {
                case ".bmp":
                    return "image/bmp";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".gif":
                    return "image/gif";
                case ".png":
                    return "image/png";
                case ".cgm":
                    return "image/cgm";
                case ".emf":
                    return "image/x-emf";
                case ".eps":
                    return "image/x-eps";
                case ".pcx":
                    return "image/x-pcx";
                case ".tga":
                    return "image/x-tga";
                case ".tif":
                case ".tiff":
                    return "image/x-tiff";
                case ".wmf":
                    return "image/x-wmf";
                default:
                    return "image/jpeg";

            }
        }

        internal static ImageFormat GetImageFormat(string extension)
        {
            switch (extension.ToLower())
            {
                case ".bmp":
                    return ImageFormat.Bmp;
                case ".jpg":
                case ".jpeg":
                    return ImageFormat.Jpeg;
                case ".gif":
                    return ImageFormat.Gif;
                case ".png":
                    return ImageFormat.Png;
                //case ".cgm":
                //    return "image/cgm";
                case ".emf":
                    return ImageFormat.Emf;
                //case ".eps":
                //    return ImageFormat.x"image/x-eps";
                //case ".pcx":
                //    return ImageFormat.;
                //case ".tga":
                //    return ImageFormat.t;
                case ".tif":
                case ".tiff":
                    return ImageFormat.Tiff;
                case ".wmf":
                    return ImageFormat.Wmf;
                default:
                    return ImageFormat.Jpeg;

            }
        }

        /// <summary>
        /// 更据类型将数据转换成值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v"></param>
        /// <returns></returns>
        internal static T GetTypedValue<T>(object v)
        {
            if (v == null)
                return default(T);

            Type fromType = v.GetType();
            Type toType = typeof(T);

            if (fromType == toType)
                return (T)v;

            var cnv = TypeDescriptor.GetConverter(fromType);
            if (toType == typeof(DateTime))    //Handle dates
            {
                if (fromType == typeof(TimeSpan))
                {
                    return ((T)(object)(new DateTime(((TimeSpan)v).Ticks)));
                }
                else if (fromType == typeof(string))
                {
                    DateTime dt;
                    if (DateTime.TryParse(v.ToString(), out dt))
                    {
                        return (T)(object)(dt);
                    }
                    else
                    {
                        return default(T);
                    }

                }
                else
                {
                    if (cnv.CanConvertTo(typeof(double)))
                    {
                        return (T)(object)(DateTime.FromOADate((double)cnv.ConvertTo(v, typeof(double))));
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
            else if (toType == typeof(TimeSpan))    //Handle timespan
            {
                if (fromType == typeof(DateTime))
                {
                    return ((T)(object)(new TimeSpan(((DateTime)v).Ticks)));
                }
                else if (fromType == typeof(string))
                {
                    TimeSpan ts;
                    if (TimeSpan.TryParse(v.ToString(), out ts))
                    {
                        return (T)(object)(ts);
                    }
                    else
                    {
                        return default(T);
                    }
                }
                else
                {
                    if (cnv.CanConvertTo(typeof(double)))
                    {

                        return (T)(object)(new TimeSpan(DateTime.FromOADate((double)cnv.ConvertTo(v, typeof(double))).Ticks));
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
            else
            {
                if (cnv.CanConvertTo(toType))
                {
                    return (T)cnv.ConvertTo(v, typeof(T));
                }
                else
                {
                    if (toType.IsGenericType && toType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        toType = Nullable.GetUnderlyingType(toType);
                        if (cnv.CanConvertTo(toType))
                        {
                            return (T)cnv.ConvertTo(v, typeof(T));
                        }
                    }

                    if (fromType == typeof(double) && toType == typeof(decimal))
                    {
                        return (T)(object)Convert.ToDecimal(v);
                    }
                    else if (fromType == typeof(decimal) && toType == typeof(double))
                    {
                        return (T)(object)Convert.ToDouble(v);
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
        }
    }
}
