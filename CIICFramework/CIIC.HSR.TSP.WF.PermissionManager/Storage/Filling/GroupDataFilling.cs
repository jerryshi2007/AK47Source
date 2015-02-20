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
    /// 通用数据填充
    /// </summary>
    public class GroupDataFilling:IDataFilling
    {
        /// <summary>
        /// 开始填充数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="objs">数据</param>
        /// <returns>填充结果</returns>
        public System.Data.DataSet Fill<T>(List<T> objs)
        {
            DataSet result = new DataSet();
            if(typeof(T).Name != typeof(AADomainRoleBO).Name)
            {
                return result;
            }
            IStructureBuilder structureBuilder = StructureBuilderFactory.CreateGroupStructureBuilder();
            result = structureBuilder.Create();
            DataTable dataContainer = result.Tables[0];
            objs.ForEach(p =>
            {
                AADomainRoleBO domainRoleBO = p as AADomainRoleBO;
                DataRow newRow = dataContainer.NewRow();
                //通用信息
                newRow[FieldNames.OGUCommon.ALL_PATH_NAME] = "CIIC/Object";
                newRow[FieldNames.OGUCommon.CODE_NAME] = domainRoleBO.Code;
                newRow[FieldNames.OGUCommon.DESCRIPTION] = domainRoleBO.LabelDescription;
                newRow[FieldNames.OGUCommon.DISPLAY_NAME] = domainRoleBO.LabelNameCn;
                newRow[FieldNames.OGUCommon.E_MAIL] = string.Empty;
                newRow[FieldNames.OGUCommon.GUID] = domainRoleBO.DomainRoleId;
                newRow[FieldNames.OGUCommon.LOGON_NAME] =string.Empty;
                newRow[FieldNames.OGUCommon.OBJ_NAME] = domainRoleBO.LabelNameCn;
                newRow[FieldNames.OGUCommon.OBJECTCLASS] = ObjectType.GROUPS;
                newRow[FieldNames.OGUCommon.ORG_CLASS] = "0";
                newRow[FieldNames.OGUCommon.ORG_TYPE] = "2";
                newRow[FieldNames.OGUCommon.PERSON_ID] = domainRoleBO.DomainRoleId;
                newRow[FieldNames.OGUCommon.RANK_CODE] = "10";
                newRow[FieldNames.OGUCommon.RANK_NAME] = domainRoleBO.LabelNameCn;
                newRow[FieldNames.OGUCommon.SIDELINE] = "0";
                newRow[FieldNames.OGUCommon.STATUS] = 1;
                newRow[FieldNames.OGUCommon.PARENT_GUID] = domainRoleBO.DomainId;
                dataContainer.Rows.Add(newRow);
            });

            return result;
        }
    }
}
