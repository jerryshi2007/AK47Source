using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.PermissionManager.Storage
{
    /// <summary>
    /// 结构构件
    /// </summary>
    public abstract class StructureBuilderBase : IStructureBuilder
    {
        /// <summary>
        /// 创建数据结构
        /// </summary>
        /// <returns>返回创建好的数据结构，DataSet</returns>
        public DataSet Create()
        {
            DataSet structure = new DataSet();
            DataTable result = new DataTable();

            List<Column> commonColumns = GetCommonColumns();
            List<Column> objectColumns = GetObjectColumns();

            CreateStructure(commonColumns, result);
            CreateStructure(objectColumns, result);

            structure.Tables.Add(result);
            return structure;
        }
        /// <summary>
        /// 根据列描述创建数据结构
        /// </summary>
        /// <param name="columns">列集合</param>
        /// <returns>数据结构</returns>
        private DataTable CreateStructure(List<Column> columns)
        {
            return new DataTable();
        }
        /// <summary>
        /// 将指定的列追加到结构
        /// </summary>
        /// <param name="columns">待追加的列集合</param>
        /// <param name="appendedStructure">已有的机构对象</param>
        /// <returns>合并后的结果</returns>
        private DataTable CreateStructure(List<Column> columns, DataTable appendedStructure)
        {
            columns.ForEach(p => appendedStructure.Columns.Add(p.Name, p.CType));
            return appendedStructure;
        }
        /// <summary>
        ///获取通用列描述
        /// </summary>
        /// <returns>列集合</returns>
        protected abstract List<Column> GetCommonColumns();
        /// <summary>
        /// 获取具体对象的列描述
        /// </summary>
        /// <returns></returns>
        protected abstract List<Column> GetObjectColumns();
    }
}
