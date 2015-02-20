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
    /// 组织数据填充
    /// </summary>
    public class OrgDataFilling:IDataFilling
    {
        /// <summary>
        /// 填充Org数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="objs">数据</param>
        /// <returns>填充结果</returns>
        public DataSet Fill<T>(List<T> objs)
        {
            DataSet result = new DataSet();
            if (typeof(T).Name != typeof(AADomainBO).Name)
            {
                return result;
            }
            IStructureBuilder structureBuilder= StructureBuilderFactory.CreateOrgStructureBuilder();
            result = structureBuilder.Create();
            DataTable dataContainer = result.Tables[0];
            objs.ForEach(p => {
                AADomainBO domainBO = p as AADomainBO;
                DataRow newRow = dataContainer.NewRow();
                //通用信息
                newRow[FieldNames.OGUCommon.ALL_PATH_NAME] = domainBO.Path;
                newRow[FieldNames.OGUCommon.CODE_NAME] = domainBO.DomainCode;
                newRow[FieldNames.OGUCommon.DESCRIPTION] = domainBO.LabelDescription;
                newRow[FieldNames.OGUCommon.DISPLAY_NAME] = domainBO.LabelNameCn;
                newRow[FieldNames.OGUCommon.E_MAIL] = string.Empty;
                newRow[FieldNames.OGUCommon.GUID] = domainBO.DomainId;
                newRow[FieldNames.OGUCommon.LOGON_NAME] = string.Empty;
                newRow[FieldNames.OGUCommon.OBJ_NAME] = domainBO.LabelNameCn;
                newRow[FieldNames.OGUCommon.OBJECTCLASS] = ObjectType.ORGANIZATIONS;
                newRow[FieldNames.OGUCommon.ORG_CLASS] = "0";
                newRow[FieldNames.OGUCommon.ORG_TYPE] = "2";
                newRow[FieldNames.OGUCommon.PERSON_ID] = domainBO.DomainId;
                newRow[FieldNames.OGUCommon.RANK_CODE] = "10";
                newRow[FieldNames.OGUCommon.RANK_NAME] = domainBO.LabelNameCn;
                newRow[FieldNames.OGUCommon.SIDELINE] = "0";
                newRow[FieldNames.OGUCommon.STATUS] = 1;
                newRow[FieldNames.OGUCommon.PARENT_GUID] = domainBO.ParentId;
                //组织信息
                newRow[FieldNames.Org.CHILDREN_COUNTER] = ObjectType.ORGANIZATIONS;
                newRow[FieldNames.Org.NAME] = domainBO.LabelNameCn;
                dataContainer.Rows.Add(newRow);
            });

            return result;
        }
    }
}
