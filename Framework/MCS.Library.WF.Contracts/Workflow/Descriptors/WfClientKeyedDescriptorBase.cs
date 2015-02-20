using MCS.Library.WF.Contracts.PropertyDefine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Descriptors
{
    [Serializable]
    [DataContract]
    public abstract class WfClientKeyedDescriptorBase
    {
        private ClientPropertyValueCollection _Properties = null;

        public WfClientKeyedDescriptorBase()
        {
        }

        public WfClientKeyedDescriptorBase(string key)
        {
            this.Key = key;
        }

        [DataMember]
        public string Key
        {
            get { return this.Properties.GetValue("Key", string.Empty); }
            set { this.Properties.AddOrSetValue("Key", value); }
        }

        [DataMember]
        public string Name
        {
            get { return this.Properties.GetValue("Name", string.Empty); }
            set { this.Properties.AddOrSetValue("Name", value); }
        }

        [DataMember]
        public string Description
        {
            get { return this.Properties.GetValue("Description", string.Empty); }
            set { this.Properties.AddOrSetValue("Description", value); }
        }

        [DataMember]
        public bool Enabled
        {
            get { return this.Properties.GetValue("Enabled", true); }
            set { this.Properties.AddOrSetValue("Enabled", value); }
        }

        [DataMember]
        public ClientPropertyValueCollection Properties
        {
            get
            {
                if (this._Properties == null)
                    this._Properties = new ClientPropertyValueCollection();

                return this._Properties;
            }
        }
    }
}
