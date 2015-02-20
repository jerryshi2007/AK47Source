using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MCS.Library.Workflow.Descriptors
{
    /// <summary>
    /// Descriptor的公共接口
    /// </summary>
    public interface IWfDescriptor : ISerializable
    {
        /// <summary>
        /// Descriptor的Key
        /// </summary>
        string Key
        {
            get;
        }

        /// <summary>
        /// Descriptor的名称
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Descriptor的描述
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
