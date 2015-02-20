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
    /// 通用数据填充
    /// </summary>
    public class GroupDataFilling:IDataFilling<Group>
    {
        /// <summary>
        /// 开始填充数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="objs">数据</param>
        /// <returns>填充结果</returns>
        public System.Data.DataSet Fill(List<Group> objs)
        {
            DataSet result = new DataSet();
            IStructureBuilder structureBuilder = StructureBuilderFactory.CreateGroupStructureBuilder();
            result = structureBuilder.Create();
            DataTable dataContainer = result.Tables[0];
            objs.ForEach(p =>
            {
                DataRow newRow = dataContainer.NewRow();
                //通用信息
                newRow[FieldNames.OGUCommon.ALL_PATH_NAME] = p.Id;
                newRow[FieldNames.OGUCommon.CODE_NAME] = p.CodeName;
                newRow[FieldNames.OGUCommon.DESCRIPTION] = p.Description;
                newRow[FieldNames.OGUCommon.DISPLAY_NAME] = p.DisplayName;
                newRow[FieldNames.OGUCommon.E_MAIL] = string.Empty;
                newRow[FieldNames.OGUCommon.GUID] = p.Id;
                newRow[FieldNames.OGUCommon.LOGON_NAME] =string.Empty;
                newRow[FieldNames.OGUCommon.OBJ_NAME] = p.DisplayName;
                newRow[FieldNames.OGUCommon.OBJECTCLASS] = ObjectType.GROUPSName;
                newRow[FieldNames.OGUCommon.ORG_CLASS] = "0";
                newRow[FieldNames.OGUCommon.ORG_TYPE] = "2";
                newRow[FieldNames.OGUCommon.PERSON_ID] = p.Id;
                newRow[FieldNames.OGUCommon.RANK_CODE] = "10";
                newRow[FieldNames.OGUCommon.RANK_NAME] = p.DisplayName;
                newRow[FieldNames.OGUCommon.SIDELINE] = "0";
                newRow[FieldNames.OGUCommon.STATUS] = 1;
                newRow[FieldNames.OGUCommon.PARENT_GUID] = p.ParentOrgId;
                dataContainer.Rows.Add(newRow);
            });

            return result;
        }
    }
}
