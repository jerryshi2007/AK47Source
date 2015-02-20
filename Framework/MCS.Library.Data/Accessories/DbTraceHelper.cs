using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

using MCS.Library.Core;

namespace MCS.Library.Data.Accessories
{
    /// <summary>
    /// 将数据结果输出到Trace的工具类
    /// </summary>
    public static class DbTraceHelper
    {
        #region Helper Methods
        private static string TruncateString(string str, int len)
        {
            if (str.Length >= len)
                return str.Substring(0, len);
            else
                return str.PadRight(len, ' ');
        }
        #endregion

        #region TraceException
        /// <summary>
        /// 输出异常信息 （包括对内部异常信息的输出）
        /// </summary>
        /// <param name="exception">异常</param>
        public static void TraceException(Exception exception)
        {
            if (exception != null)
            {
                Trace.WriteLine("\nException:");
                Trace.WriteLine(exception.Source);
                Trace.WriteLine(exception.Message);
                Trace.WriteLine(exception.StackTrace);
                Trace.WriteLine(string.Empty);
            }
            if (exception.InnerException == null) 
                return;
            TraceException(exception.InnerException);
        }
        #endregion

        #region TraceHeader
        private static void TraceHeader(DataView result, int[] widths, bool fromDataTable)
        {
            Trace.WriteLine("\n\n");
            if (fromDataTable)
                Trace.WriteLine("Table Name : " + result.Table.TableName);
            else
                Trace.WriteLine("Table Name : " + result.Table.TableName + "( View )");
            Trace.WriteLine("===============================");
            int totalLen = 0;
            for (int i = 0; i < result.Table.Columns.Count; i++)
            {
                string columnName = TruncateString(result.Table.Columns[i].ColumnName, widths[i]);
                totalLen += widths[i];
                Trace.Write(columnName);
            }
            Trace.WriteLine(string.Empty);
            Trace.WriteLine(new string('-', totalLen));
        }

        private static void TraceHeader(DataView result, int[] widths)
        {
            TraceHeader(result, widths, false);
        }

        private static void TraceHeader(DataTable result, int[] widths)
        {
            TraceHeader(result.DefaultView, widths, true);
        }
        #endregion

        #region GetWidths
        private static int[] GetWidths(DataView result)
        {
            if (result == null) return null;
            int[] widths = new int[result.Table.Columns.Count];
            for (int i = 0; i < widths.Length; i++)
                widths[i] = result.Table.Columns[i].ColumnName.Length;

            int count = 0;
            foreach (DataRowView row in result)
            {
                count++;
                for (int j = 0; j < result.Table.Columns.Count; j++)
                    widths[j] = widths[j] > row[j].ToString().Length ? widths[j] : row[j].ToString().Length;
            }
            for (int i = 0; i < widths.Length; i++)
                widths[i] += 2;
            return widths;
        }

        private static int[] GetWidths(DataTable result)
        {
            if (result == null) return null;
            int[] widths = new int[result.Columns.Count];
            for (int i = 0; i < widths.Length; i++)
                widths[i] = result.Columns[i].ColumnName.Length;

            int count = 0;
            foreach (DataRow row in result.Rows)
            {
                count++;
                for (int j = 0; j < result.Columns.Count; j++)
                    widths[j] = widths[j] > row[j].ToString().Length ? widths[j] : row[j].ToString().Length;
            }

            for (int i = 0; i < widths.Length; i++)
                widths[i] += 2;
            return widths;
        }
        #endregion

        /// <summary>
        /// 由于DataSet得灵活性，提供了一个将DataReader转换为DataSet的工具方法
        /// </summary>
        /// <param name="reader">DbDataReader对象</param>
        /// <returns>DataSet</returns>
        public static DataSet ConvertDataReaderToDataSet(DbDataReader reader)
        {
			ExceptionHelper.TrueThrow<ArgumentNullException>(reader == null, "reader");

            DataSet dataSet = new DataSet();
			dataSet.Locale = System.Globalization.CultureInfo.InvariantCulture;

            do
            {
				DataTable schemaTable = reader.GetSchemaTable();
                DataTable dataTable = new DataTable();

                if (schemaTable != null)
                {
                    // A query returning records was executed
                    for (int i = 0; i < schemaTable.Rows.Count; i++)
                    {
                        DataRow dataRow = schemaTable.Rows[i];
                        // Create a column name that is unique in the data table
                        string columnName = (string)dataRow["ColumnName"]; //+ "<C" + i + "/>";
                        // Add the column definition to the data table
                        DataColumn column = new DataColumn(columnName, (Type)dataRow["DataType"]);
                        dataTable.Columns.Add(column);
                    }
                    dataSet.Tables.Add(dataTable);
                    // Fill the data table we just created
                    //try
                    //{
                        while (reader.Read())
                        {
                            DataRow dataRow = dataTable.NewRow();
                            for (int i = 0; i < reader.FieldCount; i++)
                                dataRow[i] = reader.GetValue(i);
                            dataTable.Rows.Add(dataRow);
                        }
                    //}
                    //catch(Exception exception)
                    //{
                    //    TraceException(exception);
                    //}
                }
                else
                {
                    // No records were returned
                    DataColumn column = new DataColumn("RowsAffected");
                    dataTable.Columns.Add(column);
                    dataSet.Tables.Add(dataTable);
                    DataRow dataRow = dataTable.NewRow();
                    dataRow[0] = reader.RecordsAffected;
                    dataTable.Rows.Add(dataRow);
                }
            }
            while (reader.NextResult());
            return dataSet;
        }

        /// <summary>
        /// 将数据内容输出到Trace
        /// </summary>
        /// <remarks>该方法主要协助开发人员对提取的数据内容进行观察</remarks>
        /// <param name="result">数据</param>
        public static void TraceData(DataView result)
        {
            if (result == null) return;
            int[] colWidths = GetWidths(result);
            TraceHeader(result, colWidths);
            foreach (DataRowView row in result)
            {
                for (int j = 0; j < result.Table.Columns.Count; j++)
                    Trace.Write(TruncateString(row[j].ToString().Trim(), colWidths[j]));
                Trace.WriteLine(string.Empty);
            }
        }

        /// <summary>
        /// 将数据内容输出到Trace
        /// </summary>
        /// <remarks>该方法主要协助开发人员对提取的数据内容进行观察</remarks>
        /// <param name="result">数据</param>
        public static void TraceData(DataTable result)
        {
            if (result == null) return;
            int[] colWidths = GetWidths(result);
            TraceHeader(result, colWidths);
            foreach (DataRow row in result.Rows)
            {
                for (int j = 0; j < result.Columns.Count; j++)
                    Trace.Write(TruncateString(row[j].ToString().Trim(), colWidths[j]));
                Trace.WriteLine(string.Empty);
            }
        }

        /// <summary>
        /// 将数据内容输出到Trace
        /// </summary>
        /// <remarks>该方法主要协助开发人员对提取的数据内容进行观察</remarks>
        /// <param name="result">数据</param>
        public static void TraceData(DbDataReader result)
        {
            TraceData(ConvertDataReaderToDataSet(result));
        }

        /// <summary>
        /// 将数据内容输出到Trace
        /// </summary>
        /// <remarks>该方法主要协助开发人员对提取的数据内容进行观察</remarks>
        /// <param name="result">数据</param>
        public static void TraceData(DataSet result)
        {
            if (result == null) return;
            if (result.Tables.Count <= 0) return;
            for (int i = 0; i < result.Tables.Count; i++)
                TraceData(result.Tables[i]);
        }
    }
}
