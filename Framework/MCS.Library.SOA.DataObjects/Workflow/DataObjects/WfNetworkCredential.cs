using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using System.Net;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    [Serializable]
    [XElementSerializable]
    public class WfNetworkCredential : LogOnIdentity
    {
        private string _Key = string.Empty;

        /// <summary>
        /// 构造方法
        /// </summary>
        public WfNetworkCredential()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logonName"></param>
        public WfNetworkCredential(string logonName)
            : base(logonName)
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logonUserName"></param>
        /// <param name="pwd"></param>
        public WfNetworkCredential(string logonUserName, string pwd)
            : base(logonUserName, pwd)
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logonUserName"></param>
        /// <param name="pwd"></param>
        /// <param name="logonDomain"></param>
        public WfNetworkCredential(string logonUserName, string pwd, string logonDomain)
            : base(logonUserName, pwd, logonDomain)
        {
        }

        public string Key
        {
            get
            {
                return this._Key;
            }
            set
            {
                this._Key = value;
            }
        }

        public static explicit operator NetworkCredential(WfNetworkCredential credential)
        {
            return new NetworkCredential(credential.LogOnName, credential.Password, credential.Domain);
        }
    }

    [Serializable]
    [XElementSerializable]
    public class WfNetworkCredentialCollection : SerializableEditableKeyedDataObjectCollectionBase<string, WfNetworkCredential>
    {
        protected override string GetKeyForItem(WfNetworkCredential item)
        {
            return item.Key;
        }
    }

}
