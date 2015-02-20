using CIIC.HSR.TSP.TA.BizObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.PermissionManager.Storage
{
    /// <summary>
    /// 功能点填充
    /// </summary>
    public class ResourceFilling:IDataFilling
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
            if (typeof(T).Name != typeof(AAResourceBO).Name)
            {
                return result;
            }
            IStructureBuilder structureBuilder = StructureBuilderFactory.CreateRoleStructureBuilder();
            result = structureBuilder.Create();
            DataTable dataContainer = result.Tables[0];
            objs.ForEach(p =>
            {
                AAResourceBO resourceBO = p as AAResourceBO;
                DataRow newRow = dataContainer.NewRow();
                //通用信息
                newRow[FieldNames.ARPCommon.CODE_NAME] = resourceBO.ResourceCode;
                newRow[FieldNames.ARPCommon.ID] = resourceBO.ResourceID;
                newRow[FieldNames.ARPCommon.NAME] = resourceBO.LabelNameCn;
                newRow[FieldNames.ARPCommon.DESCRIPTION] = resourceBO.LabelDescription;
                newRow[FieldNames.ARPCommon.SORT_ID] = 0;
                dataContainer.Rows.Add(newRow);
            });

            return result;
        }
    }
}
