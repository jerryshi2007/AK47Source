using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.Data.DataObjects;
using System.Collections;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 外网用户类型
    /// </summary>
    public enum ExtUserTypes
    {
        Custom =1,
        Public =2,
        Investor =3,
        GovernmentSociety =4,
        Partner =5,
    }

    [Serializable]
    [XElementSerializable]
    [ORTableMapping("Ext_Users")]
    public class ExtUserMapping
    {
        [ORFieldMapping("UserID", PrimaryKey = true)]
        public string UserID
        {
            get;
            set;
        }

        [ORFieldMapping("UserName")]
        public string UserName
        {
            get;
            set;
        }

        [ORFieldMapping("UserType")]
        [SqlBehavior(EnumUsageTypes.UseEnumValue)]
        public ExtUserTypes UserType
        {
            get;
            set;
        }

        [ORFieldMapping("UserPassword")]
        public string UserPassword
        {
            get;
            set;
        }

        [ORFieldMapping("DisplayName")]
        public string DisplayName
        {
            get;
            set;
        }

        [ORFieldMapping("Status")]
        public int Status
        {
            get;
            set;
        }

        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime
        {
            get;
            set;
        }


        #region mapping user infomation
        public string MappingUserID
        {
            get;
            set;
        }
        public string MappingUserLoginName
        {
            get;
            set;
        }
        #endregion

    }

    public class ExtUserMappingCollection : EditableDataObjectCollectionBase<ExtUserMapping>
    {
    }
}
