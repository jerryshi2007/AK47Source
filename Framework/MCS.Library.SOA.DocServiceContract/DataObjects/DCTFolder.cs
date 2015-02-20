using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DocServiceContract
{
    /// <summary>
    /// 文件夹
    /// </summary>
    [DataContract(IsReference=true)]
	[Serializable]
    public class DCTFolder : DCTStorageObject
    {
        public DCTFolder()
        {
            IsRootFolder = false;
        }
		/// <summary>
        /// 是否根文件夹
        /// </summary>
		[DataMember]
		public bool IsRootFolder
		{
			get;
			set;
		}
    }
}