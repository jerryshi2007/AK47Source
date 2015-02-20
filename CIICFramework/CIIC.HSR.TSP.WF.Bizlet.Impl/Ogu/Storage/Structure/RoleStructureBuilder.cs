using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Ogu
{
    /// <summary>
    /// 角色字段构造
    /// </summary>
    public class RoleStructureBuilder:ARPStructureBuilderBase
    {
        /// <summary>
        /// 获取角色的扩展字段描述
        /// </summary>
        /// <returns></returns>
        protected override List<Column> GetObjectColumns()
        {
            List<Column> columns = new List<Column>()
            {
                new Column(){Name=FieldNames.Role.ALLOW_DELEGATE,CType=typeof(string)}
            };

            return columns;
        }
    }
}
