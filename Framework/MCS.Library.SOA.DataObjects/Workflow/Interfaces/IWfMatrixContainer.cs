using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 矩阵的容器，包含了矩阵所需要基本属性
    /// </summary>
    public interface IWfMatrixContainer
    {
        /// <summary>
        /// 矩阵的列定义
        /// </summary>
        SOARolePropertyDefinitionCollection PropertyDefinitions
        {
            get;
        }

        /// <summary>
        /// 矩阵的行
        /// </summary>
        SOARolePropertyRowCollection Rows
        {
            get;
        }

        /// <summary>
        /// 矩阵的类型
        /// </summary>
        WfMatrixType MatrixType
        {
            get;
        }

        /// <summary>
        /// 从DataTable构造
        /// </summary>
        /// <param name="table"></param>
        void FromDataTable(DataTable table);
    }
}
