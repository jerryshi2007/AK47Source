using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.PermissionManager.Storage
{
    /// <summary>
    /// 应用数据填充
    /// </summary>
    public class AppFilling:IDataFilling
    {
        /// <summary>
        /// 填充数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="objs">数据</param>
        /// <returns>填充结果</returns>
        public System.Data.DataSet Fill<T>(List<T> objs)
        {
            DataSet result = new DataSet();
            if (typeof(T).Name != typeof(AppEntity).Name)
            {
                return result;
            }
            IStructureBuilder structureBuilder = StructureBuilderFactory.CreateAppStructureBuilder();
            result = structureBuilder.Create();
            DataTable dataContainer = result.Tables[0];
            objs.ForEach(p =>
            {
                AppEntity app=p as AppEntity;
                DataRow newRow = dataContainer.NewRow();
                //通用信息
                newRow[FieldNames.ARPCommon.CODE_NAME] = app.Code;
                newRow[FieldNames.ARPCommon.ID] = app.ID;
                newRow[FieldNames.ARPCommon.NAME] = app.Name;
                newRow[FieldNames.ARPCommon.DESCRIPTION] = app.Name;
                newRow[FieldNames.ARPCommon.SORT_ID] = 0;
                
                dataContainer.Rows.Add(newRow);
            });

            return result;
        }
    }
}
