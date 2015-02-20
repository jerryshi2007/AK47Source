using CIIC.HSR.TSP.WF.BizObject.Exchange;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Ogu
{
    /// <summary>
    /// 功能点填充
    /// </summary>
    public class ResourceFilling : IDataFilling<CIIC.HSR.TSP.WF.BizObject.Exchange.Resource>
    {
        /// <summary>
        /// 填充数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="objs">数据</param>
        /// <returns>填充结果</returns>
        public DataSet Fill(List<CIIC.HSR.TSP.WF.BizObject.Exchange.Resource> objs)
        {
            DataSet result = new DataSet();
            IStructureBuilder structureBuilder = StructureBuilderFactory.CreateRoleStructureBuilder();
            result = structureBuilder.Create();
            DataTable dataContainer = result.Tables[0];
            objs.ForEach(p =>
            {
                DataRow newRow = dataContainer.NewRow();
                //通用信息
                newRow[FieldNames.ARPCommon.CODE_NAME] = p.Code;
                newRow[FieldNames.ARPCommon.ID] = p.Id;
                newRow[FieldNames.ARPCommon.NAME] = p.Name;
                newRow[FieldNames.ARPCommon.DESCRIPTION] = p.Description;
                newRow[FieldNames.ARPCommon.SORT_ID] = 0;
                dataContainer.Rows.Add(newRow);
            });

            return result;
        }
    }
}
