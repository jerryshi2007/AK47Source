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
    /// 用户数据填充
    /// </summary>
    public class UserDataFilling:IDataFilling
    {
        /// <summary>
        /// 填充用户数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="objs">数据</param>
        /// <returns>填充结果，以DataSet返回</returns>
        public System.Data.DataSet Fill<T>(List<T> objs)
        {
            DataSet result = new DataSet();
            if (typeof(T).Name != typeof(AAUserBO).Name)
            {
                return result;
            }
            IStructureBuilder structureBuilder = StructureBuilderFactory.CreateUserStructureBuilder();
            result = structureBuilder.Create();
            DataTable dataContainer = result.Tables[0];
            objs.ForEach(p =>
            {
                AAUserBO userBO = p as AAUserBO;

                if (null != userBO.AADomainUserBOes && userBO.AADomainUserBOes.Count > 0)
                {
                    userBO.AADomainUserBOes.ToList().ForEach(domain => {
                        DataRow newRow = dataContainer.NewRow();
                        //通用信息
                        newRow[FieldNames.OGUCommon.ALL_PATH_NAME] = string.Empty;
                        newRow[FieldNames.OGUCommon.CODE_NAME] = userBO.Account;
                        newRow[FieldNames.OGUCommon.DESCRIPTION] = userBO.Description;
                        newRow[FieldNames.OGUCommon.DISPLAY_NAME] = userBO.UserName;
                        newRow[FieldNames.OGUCommon.E_MAIL] = userBO.Email;
                        newRow[FieldNames.OGUCommon.GUID] = userBO.UserId;
                        newRow[FieldNames.OGUCommon.LOGON_NAME] = userBO.Account;
                        newRow[FieldNames.OGUCommon.OBJ_NAME] = userBO.UserName;
                        newRow[FieldNames.OGUCommon.OBJECTCLASS] = ObjectType.USERS;
                        newRow[FieldNames.OGUCommon.ORG_CLASS] = "0";
                        newRow[FieldNames.OGUCommon.ORG_TYPE] = "2";
                        newRow[FieldNames.OGUCommon.PERSON_ID] = userBO.UserId;
                        newRow[FieldNames.OGUCommon.RANK_CODE] = "10";
                        newRow[FieldNames.OGUCommon.RANK_NAME] = userBO.UserName;
                        newRow[FieldNames.OGUCommon.SIDELINE] = "0";
                        newRow[FieldNames.OGUCommon.STATUS] = 1;
                        newRow[FieldNames.OGUCommon.PARENT_GUID] = domain.DomainId;
                        //用户信息
                        newRow[FieldNames.User.AccountDisabled] = false;
                        newRow[FieldNames.User.AccountExpires] = DateTime.MaxValue;
                        newRow[FieldNames.User.AccountInspires] = DateTime.MinValue;
                        newRow[FieldNames.User.DontExpirePassword] = true;
                        newRow[FieldNames.User.END_NAME] = DateTime.MaxValue;
                        newRow[FieldNames.User.MP] = userBO.Mobile;
                        newRow[FieldNames.User.NAME] = userBO.UserName;
                        newRow[FieldNames.User.PasswordNotRequired] = false;
                        dataContainer.Rows.Add(newRow);
                    });
                }
                else
                {
                    DataRow newRow = dataContainer.NewRow();
                    //通用信息
                    newRow[FieldNames.OGUCommon.ALL_PATH_NAME] = string.Empty;
                    newRow[FieldNames.OGUCommon.CODE_NAME] = userBO.Account;
                    newRow[FieldNames.OGUCommon.DESCRIPTION] = userBO.Description;
                    newRow[FieldNames.OGUCommon.DISPLAY_NAME] = userBO.UserName;
                    newRow[FieldNames.OGUCommon.E_MAIL] = userBO.Email;
                    newRow[FieldNames.OGUCommon.GUID] = userBO.UserId;
                    newRow[FieldNames.OGUCommon.LOGON_NAME] = userBO.Account;
                    newRow[FieldNames.OGUCommon.OBJ_NAME] = userBO.UserName;
                    newRow[FieldNames.OGUCommon.OBJECTCLASS] = ObjectType.USERS;
                    newRow[FieldNames.OGUCommon.ORG_CLASS] = "0";
                    newRow[FieldNames.OGUCommon.ORG_TYPE] = "2";
                    newRow[FieldNames.OGUCommon.PERSON_ID] = userBO.UserId;
                    newRow[FieldNames.OGUCommon.RANK_CODE] = "10";
                    newRow[FieldNames.OGUCommon.RANK_NAME] = userBO.UserName;
                    newRow[FieldNames.OGUCommon.SIDELINE] = "0";
                    newRow[FieldNames.OGUCommon.STATUS] = 1;
                    //用户信息
                    newRow[FieldNames.User.AccountDisabled] = false;
                    newRow[FieldNames.User.AccountExpires] = DateTime.MaxValue;
                    newRow[FieldNames.User.AccountInspires] = DateTime.MinValue;
                    newRow[FieldNames.User.DontExpirePassword] = true;
                    newRow[FieldNames.User.END_NAME] = DateTime.MaxValue;
                    newRow[FieldNames.User.MP] = userBO.Mobile;
                    newRow[FieldNames.User.NAME] = userBO.UserName;
                    newRow[FieldNames.User.PasswordNotRequired] = false;
                    dataContainer.Rows.Add(newRow);
                }
            });

            return result;
        }
    }
}
