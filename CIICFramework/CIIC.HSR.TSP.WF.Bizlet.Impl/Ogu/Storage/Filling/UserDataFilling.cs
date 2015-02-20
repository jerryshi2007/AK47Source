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
    /// 用户数据填充
    /// </summary>
    public class UserDataFilling:IDataFilling<User>
    {
        /// <summary>
        /// 填充用户数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="objs">数据</param>
        /// <returns>填充结果，以DataSet返回</returns>
        public System.Data.DataSet Fill(List<User> objs)
        {
            DataSet result = new DataSet();
            IStructureBuilder structureBuilder = StructureBuilderFactory.CreateUserStructureBuilder();
            result = structureBuilder.Create();
            DataTable dataContainer = result.Tables[0];
            objs.ForEach(user =>
            {
                DataRow newRow = dataContainer.NewRow();
                //通用信息
                newRow[FieldNames.OGUCommon.ALL_PATH_NAME] = string.Empty;
                newRow[FieldNames.OGUCommon.CODE_NAME] = user.Code;
                newRow[FieldNames.OGUCommon.DESCRIPTION] = user.Description;
                newRow[FieldNames.OGUCommon.DISPLAY_NAME] = user.DisplayName;
                newRow[FieldNames.OGUCommon.E_MAIL] = user.Email;
                newRow[FieldNames.OGUCommon.GUID] = user.Id;
                newRow[FieldNames.OGUCommon.LOGON_NAME] = user.LogonName;
                newRow[FieldNames.OGUCommon.OBJ_NAME] = user.DisplayName;
                newRow[FieldNames.OGUCommon.OBJECTCLASS] = ObjectType.USERSName;
                newRow[FieldNames.OGUCommon.ORG_CLASS] = "0";
                newRow[FieldNames.OGUCommon.ORG_TYPE] = "2";
                newRow[FieldNames.OGUCommon.PERSON_ID] = user.Id;
                newRow[FieldNames.OGUCommon.RANK_CODE] = "10";
                newRow[FieldNames.OGUCommon.RANK_NAME] = user.DisplayName;
                newRow[FieldNames.OGUCommon.SIDELINE] = "0";
                newRow[FieldNames.OGUCommon.STATUS] = 1;
                newRow[FieldNames.OGUCommon.PARENT_GUID] = user.ParentOrgId;
                //用户信息
                newRow[FieldNames.User.AccountDisabled] = false;
                newRow[FieldNames.User.AccountExpires] = DateTime.MaxValue;
                newRow[FieldNames.User.AccountInspires] = DateTime.MinValue;
                newRow[FieldNames.User.DontExpirePassword] = true;
                newRow[FieldNames.User.END_NAME] = DateTime.MaxValue;
                newRow[FieldNames.User.MP] = user.Mobile;
                newRow[FieldNames.User.NAME] = user.DisplayName;
                newRow[FieldNames.User.PasswordNotRequired] = false;
                dataContainer.Rows.Add(newRow);
            });

            return result;
        }
    }
}
