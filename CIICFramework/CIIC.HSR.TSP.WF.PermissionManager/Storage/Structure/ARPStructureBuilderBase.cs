using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.PermissionManager.Storage
{
    /// <summary>
    /// 权限管理结构基类
    /// </summary>
    public abstract class ARPStructureBuilderBase : StructureBuilderBase
    {
        /// <summary>
        /// 权限管理通用字段
        /// </summary>
        /// <returns>权限管理通用字段</returns>
        protected override List<Column> GetCommonColumns()
        {
            List<Column> columns = new List<Column>()
            {
                new Column(){Name=FieldNames.ARPCommon.ID,CType=typeof(string)},//对象的唯一标识，36位的字符串，不允许为空
                new Column(){Name=FieldNames.ARPCommon.NAME,CType=typeof(string)},//对象的名称，不允许为空
                new Column(){Name=FieldNames.ARPCommon.CODE_NAME,CType=typeof(string)},//对象的代码名称，一般是便于记忆的。有唯一性的要求，不允许为空，在中智，可以与ID相同
                new Column(){Name=FieldNames.ARPCommon.DESCRIPTION,CType=typeof(string)},//对象的描述信息
                new Column(){Name=FieldNames.ARPCommon.SORT_ID,CType=typeof(int)}//排序号，一般用于显示
            };

            return columns;
        }
    }
}
