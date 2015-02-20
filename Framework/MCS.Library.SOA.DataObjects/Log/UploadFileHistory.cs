using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 文件上传的状态
    /// </summary>
    public enum UploadFileHistoryStatus
    {
        [EnumItemDescription("上传失败", "Fail", 0)]
        Fail = 0,
        [EnumItemDescription("上传成功", "Success", 1)]
        Success = 1,
        [EnumItemDescription("上传错误", "HaveErrors", 2)]
        HaveErrors = 2
    }

    /// <summary>
    /// 文件上传的历史记录
    /// </summary>
    [Serializable]
    [ORTableMapping("WF.UPLOAD_FILE_HISTORY")]
    public class UploadFileHistory
    {
        [ORFieldMapping("ID", PrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        [ORFieldMapping("APPLICATION_NAME")]
        public string ApplicationName { get; set; }

        [ORFieldMapping("PROGRAM_NAME")]
        public string ProgramName { get; set; }

        private IUser opUser = null;

        [SubClassORFieldMapping("ID", "OPERATOR_ID")]
        [SubClassORFieldMapping("DisplayName", "OPERATOR_NAME")]
        [SubClassType(typeof(OguUser))]
        public IUser Operator
        {
            get
            {
                return this.opUser;
            }
            set
            {
                this.opUser = (IUser)OguBase.CreateWrapperObject(value);
            }
        }

        [ORFieldMapping("STATUS")]
        public UploadFileHistoryStatus Status { get; set; }

        [ORFieldMapping("ORIGINAL_FILE_NAME")]
        public string OriginalFileName { get; set; }

        [ORFieldMapping("CURRENT_FILE_NAME")]
        public string CurrentFileName { get; set; }

        [ORFieldMapping("CREATE_TIME")]
        [SqlBehavior(BindingFlags = ClauseBindingFlags.Select)]
        public DateTime CreateTime { get; set; }

        [ORFieldMapping("STATUS_TEXT")]
        public string StatusText { get; set; }

        /// <summary>
        /// 得到对应文件的流
        /// </summary>
        /// <returns></returns>
        public Stream GetMaterialContentStream()
        {
            SqlMaterialContentPersistManager manager = new SqlMaterialContentPersistManager();

            return manager.GetMaterialContent(UploadFileHistoryAdapter.GetMaterialContentID(this));
        }
    }

    [Serializable]
    public class UploadFileHistoryCollection : EditableDataObjectCollectionBase<UploadFileHistory>
    {
    }
}
