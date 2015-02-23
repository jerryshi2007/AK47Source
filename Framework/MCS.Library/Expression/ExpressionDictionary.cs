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
    public class ExpressionDictionary
    {
        private ExpressionDictionaryItemCollection _Items = null;

        /// <summary>
        /// 
        /// </summary>
        public ExpressionDictionary()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        public ExpressionDictionary(ExpressionDictionaryConfigurationElement element)
        {
            element.NullCheck("element");

            this.Name = element.Name;
            this.Description = element.Description;
            this.Calculator = (IExpressionDictionaryCalculator)element.CreateInstance();

            this.Items.InitFromConfiguration(element.Items);
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
        public IExpressionDictionaryCalculator Calculator
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
        public ExpressionDictionaryItemCollection Items
        {
            get
            {
                if (this._Items == null)
                    this._Items = new ExpressionDictionaryItemCollection();

                return this._Items;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ExpressionDictionaryCollection : EditableKeyedDataObjectCollectionBase<string, ExpressionDictionary>
    {
        /// <summary>
        /// 
        /// </summary>
        public ExpressionDictionaryCollection()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        public void InitFromConfiguration(ExpressionDictionarySettings settings)
        {
            settings.NullCheck("settings");

            foreach (ExpressionDictionaryConfigurationElement element in settings.Dictionaries)
                this.Add(new ExpressionDictionary(element));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override string GetKeyForItem(ExpressionDictionary item)
        {
            return item.Name;
        }
    }
}
