using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MCS.Library.SOA.Contracts.DataObjects.Workflow;

namespace MCS.Library.SOA.Contracts.DataObjects
{
    [DataContract(IsReference = true)]
    public class WfClientExternalUser
    {
        private string _Key;
        /// <summary>
        /// 人员标识
        /// </summary>
        [DataMember]
        public string Key
        {
            get { return _Key; }
            set { _Key = value; }
        }

        private string _Name;
        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private ClientGender _Gender;
        /// <summary>
        /// 性别
        /// </summary>
        [DataMember]
        public ClientGender Gender
        {
            get { return _Gender; }
            set { _Gender = value; }
        }

        private string _Phone;
        /// <summary>
        /// 电话
        /// </summary>
        [DataMember]
        public string Phone
        {
            get { return _Phone; }
            set { _Phone = value; }
        }

        private string _MobilePhone;
        /// <summary>
        /// 手机
        /// </summary>
        [DataMember]
        public string MobilePhone
        {
            get { return _MobilePhone; }
            set { _MobilePhone = value; }
        }

        private string _Title;
        /// <summary>
        /// 职称
        /// </summary>
        [DataMember]
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        private string _Email;
        /// <summary>
        /// 邮箱
        /// </summary>
        [DataMember]
        public string Email
        {
            get { return _Email; }
            set { _Email = value; }
        }
    }

    [CollectionDataContract(IsReference = true)]
    public class WfClientExternalUserCollection : WfClientKeyedDescriptorCollectionBase<string, WfClientExternalUser>
    {
        protected override string GetKeyForItem(WfClientExternalUser item)
        {
            return item.Key;
        }
        public WfClientExternalUserCollection(WfClientKeyedDescriptorBase owner)
        {
            this.Owner = owner;
        }
        public WfClientExternalUserCollection()
        {

        }
    }
}
