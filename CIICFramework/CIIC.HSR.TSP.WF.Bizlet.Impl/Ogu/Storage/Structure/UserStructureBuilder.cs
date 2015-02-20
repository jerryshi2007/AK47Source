using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Ogu
{
    /// <summary>
    /// 人员结构创建
    /// </summary>
    public class UserStructureBuilder:OGUStructureBuilderBase
    {
        /// <summary>
        /// 用户扩展字段描述
        /// </summary>
        /// <returns></returns>
        protected override List<Column> GetObjectColumns()
        {
            List<Column> columns = new List<Column>()
            {
                new Column(){Name=FieldNames.User.START_TIME,CType=typeof(DateTime)},//对象的类型，不允许为空。值为"USERS"、"GROUPS"、"ORGANIZATIONS"
                new Column(){Name=FieldNames.User.END_NAME,CType=typeof(DateTime)},//对应于通用结构的OBJ_NAME
                new Column(){Name=FieldNames.User.MODIFY_TIME,CType=typeof(DateTime)},//废弃
                new Column(){Name=FieldNames.User.CREATE_TIME,CType=typeof(DateTime)},//废弃
                new Column(){Name=FieldNames.User.OUSYSDISTINCT1,CType=typeof(string)},//废弃
                new Column(){Name=FieldNames.User.OUSYSDISTINCT2,CType=typeof(string)},//废弃
                new Column(){Name=FieldNames.User.OUSYSCONTENT1,CType=typeof(string)},//废弃
                new Column(){Name=FieldNames.User.OUSYSCONTENT2,CType=typeof(string)},//废弃
                new Column(){Name=FieldNames.User.OUSYSCONTENT3,CType=typeof(string)},//废弃
                new Column(){Name=FieldNames.User.FIRST_NAME,CType=typeof(string)},//废弃
                new Column(){Name=FieldNames.User.LAST_NAME,CType=typeof(string)},//废弃
                new Column(){Name=FieldNames.User.IC_CARD,CType=typeof(string)},//废弃
                new Column(){Name=FieldNames.User.PWD_TYPE_GUID,CType=typeof(string)},//废弃
                new Column(){Name=FieldNames.User.USER_PWD,CType=typeof(string)},//废弃
                new Column(){Name=FieldNames.User.CREATE_TIME1,CType=typeof(DateTime)},//废弃
                new Column(){Name=FieldNames.User.MODIFY_TIME1,CType=typeof(DateTime)},//废弃
                new Column(){Name=FieldNames.User.AD_COUNT,CType=typeof(int)},//废弃
                new Column(){Name=FieldNames.User.SYSDISTINCT1,CType=typeof(string)},//废弃
                new Column(){Name=FieldNames.User.SYSDISTINCT2,CType=typeof(string)},//废弃
                new Column(){Name=FieldNames.User.SYSCONTENT1,CType=typeof(string)},//废弃
                new Column(){Name=FieldNames.User.SYSCONTENT2,CType=typeof(string)},//废弃
                new Column(){Name=FieldNames.User.SYSCONTENT3,CType=typeof(string)},//废弃
                new Column(){Name=FieldNames.User.PINYIN,CType=typeof(string)},//用户名称拼音，没有也可以
                new Column(){Name=FieldNames.User.SORT_ID,CType=typeof(int)},//废弃
                new Column(){Name=FieldNames.User.NAME,CType=typeof(string)},//对应于通用结构的OBJ_NAME
                new Column(){Name=FieldNames.User.VISIBLE,CType=typeof(int)},//废弃
                new Column(){Name=FieldNames.User.RANK_CLASS,CType=typeof(string)},//废弃
                new Column(){Name=FieldNames.User.AccountDisabled,CType=typeof(bool)},//账户是否禁用
                new Column(){Name=FieldNames.User.PasswordNotRequired,CType=typeof(bool)},//是否不需要密码
                new Column(){Name=FieldNames.User.DontExpirePassword,CType=typeof(bool)},//密码永不过期
                new Column(){Name=FieldNames.User.AccountInspires,CType=typeof(DateTime)},//账户生效日期，登录时才有用
                new Column(){Name=FieldNames.User.AccountExpires,CType=typeof(DateTime)},//账户失效日期，登录时才有用
                new Column(){Name=FieldNames.User.Address,CType=typeof(string)},//用户的地址
                new Column(){Name=FieldNames.User.MP,CType=typeof(string)},//用户的手机
                new Column(){Name=FieldNames.User.WP,CType=typeof(string)},//用户的座机
                new Column(){Name=FieldNames.User.OtherMP,CType=typeof(string)},//用户其它的手机，逗号分割的一串号码
                new Column(){Name=FieldNames.User.CompanyName,CType=typeof(string)},//公司名称
                new Column(){Name=FieldNames.User.DepartmentName,CType=typeof(string)},//部门名称
                new Column(){Name=FieldNames.User.PhotoTimestamp,CType=typeof(string)},//头像标识
                new Column(){Name=FieldNames.User.Sip,CType=typeof(string)}//Lync地址
            };

            return columns;
        }
    }
}
