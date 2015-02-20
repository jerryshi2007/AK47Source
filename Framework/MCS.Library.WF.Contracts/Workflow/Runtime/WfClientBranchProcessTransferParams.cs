using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Descriptors;

namespace MCS.Library.WF.Contracts.Workflow.Runtime
{
    [DataContract]
    [Serializable]
    public class WfClientBranchProcessTransferParams
    {
        private WfClientBranchProcessTemplateDescriptor _Template = null;
        private WfClientBranchProcessStartupParamsCollection _BranchParams = null;

        public WfClientBranchProcessTransferParams()
        {
        }

        public WfClientBranchProcessTransferParams(WfClientBranchProcessTemplateDescriptor template)
        {
            template.NullCheck("template");

            this._Template = template;
        }

        public WfClientBranchProcessTemplateDescriptor Template
        {
            get
            {
                return this._Template;
            }
            set
            {
                this._Template = value;
            }
        }

        public WfClientBranchProcessStartupParamsCollection BranchParams
        {
            get
            {
                if (this._BranchParams == null)
                    this._BranchParams = new WfClientBranchProcessStartupParamsCollection();

                return this._BranchParams;
            }
        }
    }

    [Serializable]
    public class WfClientBranchProcessTransferParamsCollection : SerializableEditableKeyedDataObjectCollectionBase<string, WfClientBranchProcessTransferParams>
    {
        protected override string GetKeyForItem(WfClientBranchProcessTransferParams item)
        {
            return item.Template.Key;
        }
    }
}
