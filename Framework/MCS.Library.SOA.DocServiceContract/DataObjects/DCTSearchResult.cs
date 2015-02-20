using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DocServiceContract
{
    [DataContract]
    public class DCTSearchResult
    {
        /// <summary>
        /// 标题
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// 大小
        /// </summary>
        [DataMember]
        public int Size { get; set; }

        /// <summary>
        /// 上次修改日期
        /// </summary>
        [DataMember]
        public DateTime LastModifiedDate { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        [DataMember]
        public string HitHighlightedSummary { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        [DataMember]
        public string Path { get; set; }
    }
}
