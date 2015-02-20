using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Validation;

namespace WfFormTemplate.DataObjects
{
    [Serializable]
    [XElementSerializable]
    public class DynamicFormData
    {
        [XmlObjectMapping]
        public string PropertiesXElementStr
        {
            get;
            set;
        }

        private Dictionary<string, object> _dic = new Dictionary<string, object>();

        public PropertyValueCollection Properties
        {
            get
            {
                return _dic.ToProperties();
            }
            set
            {
                _dic.Clear();
                value.FillDictionary(_dic);
            }
        }
    }

    //[Serializable]
    //public class DynamicFormDataCollection : EditableDataObjectCollectionBase<DynamicFormData>
    //{ }

    //public class DynamicFormDataAdapter : GenericFormDataAdapterBase<DynamicFormData, DynamicFormDataCollection>
    //{
    //    private DynamicFormDataAdapter()
    //    { }

    //    public static readonly DynamicFormDataAdapter Instance = new DynamicFormDataAdapter();
    //}

}