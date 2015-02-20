using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MCS.Library.Workflow.Descriptors
{
    /// <summary>
    /// Descriptor�Ĺ����ӿ�
    /// </summary>
    public interface IWfDescriptor : ISerializable
    {
        /// <summary>
        /// Descriptor��Key
        /// </summary>
        string Key
        {
            get;
        }

        /// <summary>
        /// Descriptor������
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Descriptor������
        /// </summary>
        string Description
        {
            get;
        }

        bool Enabled
        {
            get;
        }
    }
}
