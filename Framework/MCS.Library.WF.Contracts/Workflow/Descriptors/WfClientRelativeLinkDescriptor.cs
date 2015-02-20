using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Descriptors
{
    [DataContract]
    [Serializable]
    public class WfClientRelativeLinkDescriptor : WfClientKeyedDescriptorBase
    {
        public WfClientRelativeLinkDescriptor()
        {
        }

        public WfClientRelativeLinkDescriptor(string key)
            : base(key)
        {
        }

        public string Url
        {
            get;
            set;
        }

        public string Category
        {
            get;
            set;
        }
    }

    [Serializable]
    [DataContract]
    public class WfClientRelativeLinkDescriptorCollection : WfClientKeyedDescriptorCollectionBase<WfClientRelativeLinkDescriptor>
    {
        public WfClientRelativeLinkDescriptorCollection()
        {
        }

        protected WfClientRelativeLinkDescriptorCollection(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        public WfClientRelativeLinkDescriptorCollection FilterByCategory(string category)
        {
            category.CheckStringIsNullOrEmpty("category");

            WfClientRelativeLinkDescriptorCollection result = new WfClientRelativeLinkDescriptorCollection();

            result.CopyFrom(this.FindAll(link => link.Category == category));

            return result;
        }
    }
}
