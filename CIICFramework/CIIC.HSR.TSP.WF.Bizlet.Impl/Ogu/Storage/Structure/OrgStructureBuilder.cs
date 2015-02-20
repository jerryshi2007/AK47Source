using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Ogu
{
    public class OrgStructureBuilder:OGUStructureBuilderBase
    {
        /// <summary>
        /// 获取对象结构
        /// </summary>
        /// <returns></returns>
        protected override List<Column> GetObjectColumns()
        {
            List<Column> columns = new List<Column>()
            {
                new Column(){Name=FieldNames.Org.CHILDREN_COUNTER,CType=typeof(int)},//对象的类型，不允许为空。值为"USERS"、"GROUPS"、"ORGANIZATIONS"
                new Column(){Name=FieldNames.Org.NAME,CType=typeof(string)},//对应于通用结构的OBJ_NAME
                new Column(){Name=FieldNames.Org.MODIFY_TIME,CType=typeof(DateTime)},//废弃
                new Column(){Name=FieldNames.Org.CREATE_TIME,CType=typeof(DateTime)},//废弃
                new Column(){Name=FieldNames.Org.SORT_ID,CType=typeof(int)},//废弃
                new Column(){Name=FieldNames.Org.SYSDISTINCT1,CType=typeof(string)},//废弃
                new Column(){Name=FieldNames.Org.SYSDISTINCT2,CType=typeof(string)},//废弃
                new Column(){Name=FieldNames.Org.SYSCONTENT1,CType=typeof(string)},//废弃
                new Column(){Name=FieldNames.Org.SYSCONTENT2,CType=typeof(string)},//废弃
                new Column(){Name=FieldNames.Org.SYSCONTENT3,CType=typeof(string)},//废弃
                new Column(){Name=FieldNames.Org.VISIBLE,CType=typeof(int)},//废弃
                new Column(){Name=FieldNames.Org.RANK_CLASS,CType=typeof(string)}//废弃
            };

            return columns;
        }
    }
}
