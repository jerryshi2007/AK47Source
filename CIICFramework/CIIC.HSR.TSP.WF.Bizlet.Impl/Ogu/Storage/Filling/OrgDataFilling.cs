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
    /// 组织数据填充
    /// </summary>
    public class OrgDataFilling:IDataFilling<Org>
    {
        /// <summary>
        /// 填充Org数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="objs">数据</param>
        /// <returns>填充结果</returns>
        public DataSet Fill(List<Org> objs)
        {
            DataSet result = new DataSet();
            IStructureBuilder structureBuilder= StructureBuilderFactory.CreateOrgStructureBuilder();
            result = structureBuilder.Create();
            DataTable dataContainer = result.Tables[0];
            objs.ForEach(org => {
                DataRow newRow = dataContainer.NewRow();
                //通用信息
                newRow[FieldNames.OGUCommon.ALL_PATH_NAME] = org.AllPath;
                newRow[FieldNames.OGUCommon.CODE_NAME] = org.Code;
                newRow[FieldNames.OGUCommon.DESCRIPTION] = org.Description;
                newRow[FieldNames.OGUCommon.DISPLAY_NAME] = org.DisplayName;
                newRow[FieldNames.OGUCommon.E_MAIL] = string.Empty;
                newRow[FieldNames.OGUCommon.GUID] = org.Id;
                newRow[FieldNames.OGUCommon.LOGON_NAME] = string.Empty;
                newRow[FieldNames.OGUCommon.OBJ_NAME] = org.DisplayName;
                newRow[FieldNames.OGUCommon.OBJECTCLASS] = ObjectType.ORGANIZATIONSName;
                newRow[FieldNames.OGUCommon.ORG_CLASS] = "0";
                newRow[FieldNames.OGUCommon.ORG_TYPE] = "2";
                newRow[FieldNames.OGUCommon.PERSON_ID] = org.Id;
                newRow[FieldNames.OGUCommon.RANK_CODE] = "10";
                newRow[FieldNames.OGUCommon.RANK_NAME] = org.DisplayName;
                newRow[FieldNames.OGUCommon.SIDELINE] = "0";
                newRow[FieldNames.OGUCommon.STATUS] = 1;
                newRow[FieldNames.OGUCommon.PARENT_GUID] = org.ParentOrgId;
                //组织信息
                newRow[FieldNames.Org.CHILDREN_COUNTER] = ObjectType.ORGANIZATIONS;
                newRow[FieldNames.Org.NAME] = org.DisplayName;
                dataContainer.Rows.Add(newRow);
            });

            return result;
        }
    }
}
