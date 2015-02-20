using CIIC.HSR.TSP.TA.BizObject;
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
    /// 角色填充
    /// </summary>
    public class RoleFilling:IDataFilling<Role>
    {
        /// <summary>
        /// 填充数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="objs">数据</param>
        /// <returns>填充结果</returns>
        public System.Data.DataSet Fill(List<Role> objs)
        {
            DataSet result = new DataSet();
            IStructureBuilder structureBuilder = StructureBuilderFactory.CreateRoleStructureBuilder();
            result = structureBuilder.Create();
            DataTable dataContainer = result.Tables[0];
            objs.ForEach(role =>
            {
                DataRow newRow = dataContainer.NewRow();
                //通用信息
                newRow[FieldNames.ARPCommon.CODE_NAME] = role.Code;
                newRow[FieldNames.ARPCommon.ID] = role.ID;
                newRow[FieldNames.ARPCommon.NAME] = role.Name;
                newRow[FieldNames.ARPCommon.DESCRIPTION] = role.Description;
                newRow[FieldNames.ARPCommon.SORT_ID] = 0;
                //角色信息
                //newRow[FieldNames.Role.ALLOW_DELEGATE] = ObjectType.ORGANIZATIONS;
                dataContainer.Rows.Add(newRow);
            });

            return result;
        }
    }
}
