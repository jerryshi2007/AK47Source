using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using System.Xml.Linq;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    [ORTableMapping("WF.PROCESS_DESCRIPTORS")]
    [Serializable]
    public class WfProcessDescriptorInfo
    {
        private bool _Enabled = true;

        [ORFieldMapping("PROCESS_KEY", PrimaryKey = true)]
        public string ProcessKey
        {
            get;
            set;
        }

        [ORFieldMapping("APPLICATION_NAME")]
        public string ApplicationName
        {
            get;
            set;
        }

        [ORFieldMapping("PROGRAM_NAME")]
        public string ProgramName
        {
            get;
            set;
        }

        [ORFieldMapping("PROCESS_NAME")]
        public string ProcessName
        {
            get;
            set;
        }

        [ORFieldMapping("DATA")]
        public string Data
        {
            get;
            set;
        }

        [ORFieldMapping("CREATE_TIME")]
        [SqlBehavior(BindingFlags = ClauseBindingFlags.Select | ClauseBindingFlags.Where)]
        public DateTime CreateTime
        {
            get;
            set;
        }

        private IUser _Creator = null;

        [SubClassORFieldMapping("ID", "CREATOR_ID")]
        [SubClassORFieldMapping("DisplayName", "CREATOR_NAME")]
        [SubClassSqlBehavior(SubPropertyName = "ID", BindingFlags = ClauseBindingFlags.Select | ClauseBindingFlags.Where | ClauseBindingFlags.Insert)]
        [SubClassSqlBehavior(SubPropertyName = "DisplayName", BindingFlags = ClauseBindingFlags.Select | ClauseBindingFlags.Where | ClauseBindingFlags.Insert)]
        [SubClassType(typeof(OguUser))]
        [SqlBehavior(BindingFlags = ClauseBindingFlags.Select | ClauseBindingFlags.Where | ClauseBindingFlags.Insert)]
        public IUser Creator
        {
            get
            {
                return this._Creator;
            }
            set
            {
                this._Creator = (IUser)OguUser.CreateWrapperObject(value);
            }
        }

        [ORFieldMapping("MODIFY_TIME")]
        [SqlBehavior(DefaultExpression = "GETDATE()")]
        public DateTime ModifyTime
        {
            get;
            set;
        }

        private IUser _Modifier = null;

        [SubClassORFieldMapping("ID", "MODIFIER_ID")]
        [SubClassORFieldMapping("DisplayName", "MODIFIER_NAME")]
        [SubClassType(typeof(OguUser))]
        public IUser Modifier
        {
            get
            {
                return this._Modifier;
            }
            set
            {
                this._Modifier = (IUser)OguUser.CreateWrapperObject(value);
            }
        }

        [ORFieldMapping("ENABLED")]
        public bool Enabled
        {
            get { return this._Enabled; }
            set { this._Enabled = value; }
        }

        [ORFieldMapping("IMPORT_TIME")]
        [SqlBehavior(BindingFlags = ClauseBindingFlags.Select | ClauseBindingFlags.Where)]
        public DateTime ImportTime
        {
            get;
            set;
        }

        private IUser _ImportUser = null;
        [NoMapping]
        public IUser ImportUser
        {
            get
            {
                return this._ImportUser;
            }
            set
            {
                this._ImportUser = (IUser)OguUser.CreateWrapperObject(value);
            }
        }

        public static WfProcessDescriptorInfo FromProcessDescriptor(IWfProcessDescriptor processDesp)
        {
            XElementFormatter formatter = new XElementFormatter();

            formatter.OutputShortType = WorkflowSettings.GetConfig().OutputShortType;

            XElement root = formatter.Serialize(processDesp);

            return FromProcessDescriptor(processDesp, root);
        }

        public static WfProcessDescriptorInfo FromProcessDescriptor(IWfProcessDescriptor processDesp, XElement xml)
        {
            processDesp.NullCheck("processDesp");
            xml.NullCheck("xml");

            WfProcessDescriptorInfo result = new WfProcessDescriptorInfo();

            result.ProcessName = processDesp.Name;
            result.ApplicationName = processDesp.ApplicationName;
            result.ProgramName = processDesp.ProgramName;
            result.ProcessKey = processDesp.Key;
            result.Enabled = processDesp.Enabled;

            result.Data = xml.ToString();

            return result;
        }
    }

    public class WfProcessDescriptorInfoCollection : SerializableEditableKeyedDataObjectCollectionBase<string, WfProcessDescriptorInfo>
    {
        protected override string GetKeyForItem(WfProcessDescriptorInfo item)
        {
            return item.ProcessKey;
        }
    }
}
