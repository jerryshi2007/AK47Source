using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Expression
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ExpressionDictionaryItem
    {
        /// <summary>
        /// 
        /// </summary>
        public ExpressionDictionaryItem()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        public ExpressionDictionaryItem(ExpressionDictionaryItemConfigurationElement element)
        {
            element.NullCheck("element");

            this.Name = element.Name;
            this.Description = element.Description;
            this.DataType = element.DataType;
            this.DefaultValue = element.DefaultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public ExpressionDataType DataType
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string DefaultValue
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ExpressionDictionaryItemCollection : SerializableEditableKeyedDataObjectCollectionBase<string, ExpressionDictionaryItem>
    {
        /// <summary>
        /// 
        /// </summary>
        public ExpressionDictionaryItemCollection() :
            base(StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        public void InitFromConfiguration(ExpressionDictionaryItemConfigurationElementCollection elements)
        {
            elements.NullCheck("elements");

            foreach (ExpressionDictionaryItemConfigurationElement element in elements)
                this.Add(new ExpressionDictionaryItem(element));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ExpressionDictionaryItemCollection(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override string GetKeyForItem(ExpressionDictionaryItem item)
        {
            return item.Name.ToLower();
        }
    }
}
