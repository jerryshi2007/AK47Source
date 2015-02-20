using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.PermissionManager.Storage
{

    /// <summary>
    /// 字段定义
    /// </summary>
    public class FieldNames
    {
        /// <summary>
        /// 组织机构字段定义
        /// </summary>
        public class OGUCommon
        { 
            /// <summary>
            /// 对象的类型，不允许为空。值为"USERS"、"GROUPS"、"ORGANIZATIONS"
            /// </summary>
            public static readonly string OBJECTCLASS="OBJECTCLASS";
            /// <summary>
            /// 员工ID
            /// </summary
            public static readonly string PERSON_ID="PERSON_ID";
            /// <summary>
            /// 废弃
            /// </summary
            public static readonly string POSTURAL="POSTURAL";
            /// <summary>
            /// 用户的Title
            /// </summary
            public static readonly string RANK_NAME="RANK_NAME";
            /// <summary>
            /// 对象的状态。1表示正常，3表示被删除
            /// </summary
            public static readonly string STATUS="STATUS";
            /// <summary>
            /// 如果属于某个组织，则是组织的ALL_PATH_NAME加上自己的OBJ_NAME
            /// </summary
            public static readonly string ALL_PATH_NAME="ALL_PATH_NAME";
            /// <summary>
            /// 暂时为空
            /// </summary
            public static readonly string GLOBAL_SORT="GLOBAL_SORT";
            /// <summary>
            /// 暂时为空
            /// </summary
            public static readonly string ORIGINAL_SORT="ORIGINAL_SORT";
            /// <summary>
            /// 对象的显示名称，例如“牛小超”
            /// </summary
            public static readonly string DISPLAY_NAME="DISPLAY_NAME";
            /// <summary>
            /// 对象的名称，会出现在ALL_PATH_NAME中。例如当前对象是会计“牛小超”，那么其ALL_PATH_NAME就是“集团总部\财务部\牛小超”，这个名称不应该随便改动
            /// </summary
            public static readonly string OBJ_NAME="OBJ_NAME";
            /// <summary>
            ///同对于人员而言，就是登录名，和下面的CODE_NAME相
            /// </summary
            public static readonly string LOGON_NAME="LOGON_NAME";
            /// <summary>
            /// 对象的父对象ID，36位的GUID，例如人员所属的组织ID。如果为空，则表示根组织
            /// </summary
            public static readonly string PARENT_GUID="PARENT_GUID";
            /// <summary>
            /// 对象的唯一ID，36位的GUID。不允许为空
            /// </summary
            public static readonly string GUID="GUID";
            /// <summary>
            /// 对象在组织内的排序号，一般是6位数字，不足六位用0补齐。影响对象在组织内的排序
            /// </summary
            public static readonly string INNER_SORT="INNER_SORT";
            /// <summary>
            /// 对象的描述信息
            /// </summary
            public static readonly string DESCRIPTION="DESCRIPTION";
            /// <summary>
            /// 对象的级别，例如人的级别或组织的级别，默认值“10”即可
            /// </summary
            public static readonly string RANK_CODE="RANK_CODE";
            /// <summary>
            /// 组织的类别，写0也可以
            /// </summary
            public static readonly string ORG_CLASS="ORG_CLASS";
            /// <summary>
            /// 废弃
            /// </summary
            public static readonly string CUSTOMS_CODE="CUSTOMS_CODE";
            /// <summary>
            /// 组织的类型。2表示一般部门
            /// </summary
            public static readonly string ORG_TYPE="ORG_TYPE";
            /// <summary>
            /// 用户的邮件地址。有时组织或群组也有
            /// </summary
            public static readonly string E_MAIL="E_MAIL";
            /// <summary>
            /// 对象的属性
            /// </summary
            public static readonly string ATTRIBUTES="ATTRIBUTES";
            /// <summary>
            /// 人员是否是兼职的，0表示不是，1表示是
            /// </summary
            public static readonly string SIDELINE="SIDELINE";
            /// <summary>
            /// 对于人员来说，等同于LOGON_NAME。对于其它对象，可以为空
            /// </summary
            public static readonly string CODE_NAME="CODE_NAME";
            /// <summary>
            /// 废弃
            /// </summary
            public static readonly string VERSION_START_TIME="VERSION_START_TIME";
        }
        /// <summary>
        /// 组织扩展字段
        /// </summary>
        public class Org
        { 
            /// <summary>
            /// 对象的类型，不允许为空。值为"USERS"、"GROUPS"、"ORGANIZATIONS"
            /// </summary>
            public static readonly string CHILDREN_COUNTER="CHILDREN_COUNTER";
            /// <summary>
            /// 对应于通用结构的OBJ_NAME
            /// </summary>
            public static readonly string NAME="NAME";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string MODIFY_TIME="MODIFY_TIME";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string CREATE_TIME="CREATE_TIME";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string SORT_ID="SORT_ID";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string SYSDISTINCT1="SYSDISTINCT1";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string SYSDISTINCT2="SYSDISTINCT2";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string SYSCONTENT1="SYSCONTENT1";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string SYSCONTENT2="SYSCONTENT2";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string SYSCONTENT3="SYSCONTENT3";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string VISIBLE="VISIBLE";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string RANK_CLASS="RANK_CLASS";
        }
        /// <summary>
        /// 用户扩展字段
        /// </summary>
        public class User
        { 
            //对象的类型，不允许为空。值为"USERS"、"GROUPS"、"ORGANIZATIONS"
            /// <summary>
            /// 
            /// </summary>
            public static readonly string START_TIME="START_TIME";
            /// <summary>
            /// 对应于通用结构的OBJ_NAME
            /// </summary>
            public static readonly string END_NAME="END_NAME";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string MODIFY_TIME="MODIFY_TIME";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string CREATE_TIME="CREATE_TIME";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string OUSYSDISTINCT1="OUSYSDISTINCT1";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string OUSYSDISTINCT2="OUSYSDISTINCT2";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string OUSYSCONTENT1="OUSYSCONTENT1";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string OUSYSCONTENT2="OUSYSCONTENT2";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string OUSYSCONTENT3="OUSYSCONTENT3";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string FIRST_NAME="FIRST_NAME";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string LAST_NAME="LAST_NAME";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string IC_CARD="IC_CARD";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string PWD_TYPE_GUID="PWD_TYPE_GUID";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string USER_PWD="USER_PWD";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string CREATE_TIME1="CREATE_TIME1";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string MODIFY_TIME1="MODIFY_TIME1";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string AD_COUNT="AD_COUNT";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string SYSDISTINCT1="SYSDISTINCT1";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string SYSDISTINCT2="SYSDISTINCT2";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string SYSCONTENT1="SYSCONTENT1";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string SYSCONTENT2="SYSCONTENT2";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string SYSCONTENT3="SYSCONTENT3";
            /// <summary>
            /// 用户名称拼音，没有也可以
            /// </summary>
            public static readonly string PINYIN="PINYIN";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string SORT_ID="SORT_ID";
            /// <summary>
            /// 对应于通用结构的OBJ_NAME
            /// </summary>
            public static readonly string NAME="NAME";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string VISIBLE="VISIBLE";
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string RANK_CLASS="RANK_CLASS";
            /// <summary>
            /// 账户是否禁用
            /// </summary>
            public static readonly string AccountDisabled="AccountDisabled";
            /// <summary>
            /// 是否不需要密码
            /// </summary>
            public static readonly string PasswordNotRequired="PasswordNotRequired";
            /// <summary>
            /// 密码永不过期
            /// </summary>
            public static readonly string DontExpirePassword="DontExpirePassword";
            /// <summary>
            /// 账户生效日期，登录时才有用
            /// </summary>
            public static readonly string AccountInspires="AccountInspires";
            /// <summary>
            /// 账户失效日期，登录时才有用
            /// </summary>
            public static readonly string AccountExpires="AccountExpires";
            /// <summary>
            /// 用户的地址
            /// </summary>
            public static readonly string Address="Address";
            /// <summary>
            /// 用户的手机
            /// </summary>
            public static readonly string MP="MP";
            /// <summary>
            /// 用户的座机
            /// </summary>
            public static readonly string WP="WP";
            /// <summary>
            /// 用户其它的手机，逗号分割的一串号码
            /// </summary>
            public static readonly string OtherMP="OtherMP";
            /// <summary>
            /// 公司名称
            /// </summary>
            public static readonly string CompanyName="CompanyName";
            /// <summary>
            /// 部门名称
            /// </summary>
            public static readonly string DepartmentName="DepartmentName";
            /// <summary>
            /// 头像标识
            /// </summary>
            public static readonly string PhotoTimestamp="PhotoTimestamp";
            /// <summary>
            /// Lync地址
            /// </summary>
            public static readonly string Sip="Sip";
        }
        /// <summary>
        /// 权限通用字段
        /// </summary>
        public class ARPCommon
        {
            /// <summary>
            /// 对象的唯一标识，36位的字符串，不允许为空
            /// </summary>
            public static readonly string ID = "ID";
            /// <summary>
            /// 对象的名称，不允许为空
            /// </summary>
            public static readonly string NAME = "NAME";
            /// <summary>
            /// 对象的代码名称，一般是便于记忆的。有唯一性的要求，不允许为空，在中智，可以与ID相同
            /// </summary>
            public static readonly string CODE_NAME = "CODE_NAME";
            /// <summary>
            /// 对象的描述信息
            /// </summary>
            public static readonly string DESCRIPTION = "DESCRIPTION";
            /// <summary>
            /// 排序号，一般用于显示
            /// </summary>
            public static readonly string SORT_ID = "SORT_ID";
        }
        /// <summary>
        /// 角色扩展字段
        /// </summary>
        public class Role
        {
            /// <summary>
            /// 废弃
            /// </summary>
            public static readonly string ALLOW_DELEGATE = "ALLOW_DELEGATE";
        }
        /// <summary>
        /// 应用扩展字段
        /// </summary>
        public class APP
        {
            /// <summary>
            /// 忽略
            /// </summary>
            public static readonly string RESOURCE_LEVEL = "RESOURCE_LEVEL";
            /// <summary>
            /// 忽略
            /// </summary>
            public static readonly string CHILDREN_COUNT = "CHILDREN_COUNT";
            /// <summary>
            /// 忽略
            /// </summary>
            public static readonly string ADD_SUBAPP = "ADD_SUBAPP";
            /// <summary>
            /// 忽略
            /// </summary>
            public static readonly string USE_SCOPE = "USE_SCOPE";
            /// <summary>
            /// 忽略
            /// </summary>
            public static readonly string INHERITED_STATE = "INHERITED_STATE";
        }
    }
}
