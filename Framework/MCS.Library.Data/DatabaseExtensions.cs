using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;
using System.Data;
using System.Data.Common;

namespace MCS.Library.Data
{
    /// <summary>
    /// 数据库操作相关的扩展方法
    /// </summary>
    public static class DatabaseExtensions
    {
        /// <summary>
        /// 异步填充数据集
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public static async Task<int> FillAsync(this DbDataAdapter adapter, DataSet dataSet)
        {
            adapter.NullCheck("adapter");
            dataSet.NullCheck("dataSet");

            return await Task.Run(() => adapter.Fill(dataSet));
        }

        /// <summary>
        /// 异步填充一个数据集中一个数据表
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="dataSet"></param>
        /// <param name="srcTable"></param>
        /// <returns></returns>
        public static async Task<int> FillAsync(this DbDataAdapter adapter, DataSet dataSet, string srcTable)
        {
            adapter.NullCheck("adapter");
            dataSet.NullCheck("dataSet");
            srcTable.CheckStringIsNullOrEmpty("srcTable");

            return await Task.Run(() => adapter.Fill(dataSet, srcTable));
        }

        /// <summary>
        /// 异步填充数据表
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static async Task<int> FillAsync(this DbDataAdapter adapter, DataTable dataTable)
        {
            adapter.NullCheck("adapter");
            dataTable.NullCheck("dataTable");

            return await Task.Run(() => adapter.Fill(dataTable));
        }

        /// <summary>
        /// 异步填充一组数据表
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="startRecord"></param>
        /// <param name="maxRecords"></param>
        /// <param name="dataTables"></param>
        /// <returns></returns>
        public static async Task<int> FillAsync(this DbDataAdapter adapter, int startRecord, int maxRecords, params DataTable[] dataTables)
        {
            adapter.NullCheck("adapter");
            dataTables.NullCheck("dataTables");

            return await Task.Run(() => adapter.Fill(startRecord, maxRecords, dataTables));
        }

        /// <summary>
        /// 异步填充一个数据集中的一个数据表
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="dataSet"></param>
        /// <param name="startRecord"></param>
        /// <param name="maxRecords"></param>
        /// <param name="srcTable"></param>
        /// <returns></returns>
        public static async Task<int> FillAsync(this DbDataAdapter adapter, DataSet dataSet, int startRecord, int maxRecords, string srcTable)
        {
            adapter.NullCheck("adapter");
            dataSet.NullCheck("dataSet");
            srcTable.CheckStringIsNullOrEmpty("srcTable");

            return await Task.Run(() => adapter.Fill(dataSet, startRecord, maxRecords, srcTable));
        }
    }
}
