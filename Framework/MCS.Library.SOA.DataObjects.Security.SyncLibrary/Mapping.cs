using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary
{
    public class PropertyMapping
    {
        private string setterKey;
        private NameValueCollection parameters;
        private string srcProperty;
        private string targetProperty;
        private string compareKey;


        public PropertyMapping(string srcProperty, string targetProperty, string comparerKey, string setterKey, NameValueCollection parameters)
        {
            this.srcProperty = srcProperty;
            this.targetProperty = targetProperty;
            this.compareKey = comparerKey;
            this.setterKey = setterKey;
            this.parameters = parameters;
        }

        public string SourceProperty
        {
            get { return srcProperty; }
        }

        public string TargetProperty
        {
            get { return targetProperty; }
        }

        public string ComparerKey
        {
            get { return compareKey; }
        }

        public string SetterKey
        {
            get { return setterKey; }
        }

        public NameValueCollection Parameters
        {
            get { return parameters; }
        }
    }

    public class PropertyMappingCollection : NameObjectCollectionBase<PropertyMapping>
    {
        private string sourceKeyProperty;

        public string SourceKeyProperty
        {
            get { return sourceKeyProperty; }
            set { sourceKeyProperty = value; }
        }
    }
}
