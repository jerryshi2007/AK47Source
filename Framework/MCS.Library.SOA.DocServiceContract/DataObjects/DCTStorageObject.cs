using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ServiceModel.ComIntegration;

namespace MCS.Library.SOA.DocServiceContract
{
    /// <summary>
    /// 存储对象（文件和文件夹的基类
    /// </summary>
    [DataContract(IsReference = true)]
    [Serializable]
    [KnownType(typeof(DCTFile))]
    [KnownType(typeof(DCTFolder))]
    public abstract class DCTStorageObject
    {
		[DataMember]
		public int ID
		{
			get;
			set;
		}

		[DataMember]
		public string Uri
		{
			get;
			set;
		}

		[DataMember]
		public string Name
		{
			get;
			set;
		}

		[DataMember]
		public string AbsoluteUri
		{
			get;
			set;
		}
    }
}