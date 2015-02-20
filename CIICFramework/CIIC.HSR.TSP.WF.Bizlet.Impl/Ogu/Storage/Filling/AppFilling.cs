using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Ogu
{
    /// <summary>
    /// 应用数据填充
    /// </summary>
    public class AppFilling : IDataFilling<AppEntity>
    {
        /// <summary>
        /// 填充数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="objs">数据</param>
        /// <returns>填充结果</returns>
        public System.Data.DataSet Fill(List<AppEntity> objs)
        {
            DataSet result = new DataSet();

            IStructureBuilder structureBuilder = StructureBuilderFactory.CreateAppStructureBuilder();
            result = structureBuilder.Create();
            DataTable dataContainer = result.Tables[0];
            objs.ForEach(p =>
            {
                DataRow newRow = dataContainer.NewRow();
                //通用信息
                newRow[FieldNames.ARPCommon.CODE_NAME] = p.Code;
                newRow[FieldNames.ARPCommon.ID] = p.ID;
                newRow[FieldNames.ARPCommon.NAME] = p.Name;
                newRow[FieldNames.ARPCommon.DESCRIPTION] = p.Name;
                newRow[FieldNames.ARPCommon.SORT_ID] = 0;
                
                dataContainer.Rows.Add(newRow);
            });

            return result;
        }
    }
}
