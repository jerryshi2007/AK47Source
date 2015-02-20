using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Ogu
{
    /// <summary>
    /// 组织、组合和人员结构构建器
    /// </summary>
    public abstract class OGUStructureBuilderBase:StructureBuilderBase
    {
        /// <summary>
        /// 通用组织、组、人员列描述
        /// </summary>
        /// <returns></returns>
        protected override List<Column> GetCommonColumns()
        {
            List<Column> columns = new List<Column>()
            {
                new Column(){Name=FieldNames.OGUCommon.OBJECTCLASS,CType=typeof(string)},//对象的类型，不允许为空。值为"USERS"、"GROUPS"、"ORGANIZATIONS"
                new Column(){Name=FieldNames.OGUCommon.PERSON_ID,CType=typeof(string)},//员工ID
                new Column(){Name=FieldNames.OGUCommon.POSTURAL,CType=typeof(int)},//废弃
                new Column(){Name=FieldNames.OGUCommon.RANK_NAME,CType=typeof(string)},//用户的Title
                new Column(){Name=FieldNames.OGUCommon.STATUS,CType=typeof(int)},//对象的状态。1表示正常，3表示被删除
                new Column(){Name=FieldNames.OGUCommon.ALL_PATH_NAME,CType=typeof(string)},//如果属于某个组织，则是组织的ALL_PATH_NAME加上自己的OBJ_NAME
                new Column(){Name=FieldNames.OGUCommon.GLOBAL_SORT,CType=typeof(string)},//暂时为空
                new Column(){Name=FieldNames.OGUCommon.ORIGINAL_SORT,CType=typeof(string)},//暂时为空
                new Column(){Name=FieldNames.OGUCommon.DISPLAY_NAME,CType=typeof(string)},//对象的显示名称，例如“牛小超”
                new Column(){Name=FieldNames.OGUCommon.OBJ_NAME,CType=typeof(string)},//对象的名称，会出现在ALL_PATH_NAME中。例如当前对象是会计“牛小超”，那么其ALL_PATH_NAME就是“集团总部\财务部\牛小超”，这个名称不应该随便改动
                new Column(){Name=FieldNames.OGUCommon.LOGON_NAME,CType=typeof(string)},//对于人员而言，就是登录名，和下面的CODE_NAME相同
                new Column(){Name=FieldNames.OGUCommon.PARENT_GUID,CType=typeof(string)},//对象的父对象ID，36位的GUID，例如人员所属的组织ID。如果为空，则表示根组织
                new Column(){Name=FieldNames.OGUCommon.GUID,CType=typeof(string)},//对象的唯一ID，36位的GUID。不允许为空
                new Column(){Name=FieldNames.OGUCommon.INNER_SORT,CType=typeof(string)},//对象在组织内的排序号，一般是6位数字，不足六位用0补齐。影响对象在组织内的排序
                new Column(){Name=FieldNames.OGUCommon.DESCRIPTION,CType=typeof(string)},//对象的描述信息
                new Column(){Name=FieldNames.OGUCommon.RANK_CODE,CType=typeof(string)},//对象的级别，例如人的级别或组织的级别，默认值“10”即可
                new Column(){Name=FieldNames.OGUCommon.ORG_CLASS,CType=typeof(int)},//组织的类别，写0也可以
                new Column(){Name=FieldNames.OGUCommon.CUSTOMS_CODE,CType=typeof(string)},//废弃
                new Column(){Name=FieldNames.OGUCommon.ORG_TYPE,CType=typeof(int)},//组织的类型。2表示一般部门
                new Column(){Name=FieldNames.OGUCommon.E_MAIL,CType=typeof(string)},//用户的邮件地址。有时组织或群组也有
                new Column(){Name=FieldNames.OGUCommon.ATTRIBUTES,CType=typeof(int)},//对象的属性
                new Column(){Name=FieldNames.OGUCommon.SIDELINE,CType=typeof(int)},//人员是否是兼职的，0表示不是，1表示是
                new Column(){Name=FieldNames.OGUCommon.CODE_NAME,CType=typeof(string)},//对于人员来说，等同于LOGON_NAME。对于其它对象，可以为空
                new Column(){Name=FieldNames.OGUCommon.VERSION_START_TIME,CType=typeof(DateTime)},//废弃
            };

            return columns;
        }
    }
}
