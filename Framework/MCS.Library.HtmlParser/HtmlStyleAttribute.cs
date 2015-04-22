﻿using MCS.Library.Data.DataObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.HtmlParser
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HtmlStyleAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Key
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// 属性结构
        /// </summary>
        public HtmlStyleValueExpression Expression
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HtmlStyleAttributeCollection : SerializableEditableKeyedDataObjectCollectionBase<string, HtmlStyleAttribute>
    {
        /// <summary>
        /// 
        /// </summary>
        public HtmlStyleAttributeCollection()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected HtmlStyleAttributeCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override string GetKeyForItem(HtmlStyleAttribute item)
        {
            return item.Key;
        }
    }
}
