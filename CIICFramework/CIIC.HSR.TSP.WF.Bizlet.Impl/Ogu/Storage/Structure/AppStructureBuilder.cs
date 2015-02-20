using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Ogu
{
    public class AppStructureBuilder:ARPStructureBuilderBase
    {
        protected override List<Column> GetObjectColumns()
        {
            List<Column> columns = new List<Column>()
            {
                new Column(){Name=FieldNames.APP.RESOURCE_LEVEL,CType=typeof(string)},//忽略
                new Column(){Name=FieldNames.APP.CHILDREN_COUNT,CType=typeof(int)},//忽略
                new Column(){Name=FieldNames.APP.ADD_SUBAPP,CType=typeof(string)},//忽略
                new Column(){Name=FieldNames.APP.USE_SCOPE,CType=typeof(string)},//忽略
                new Column(){Name=FieldNames.APP.INHERITED_STATE,CType=typeof(int)}//忽略
            };

            return columns;
        }
    }
}
