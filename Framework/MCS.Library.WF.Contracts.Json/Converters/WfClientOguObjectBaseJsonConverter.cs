using MCS.Library.Core;
using MCS.Library.WF.Contracts.Ogu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Json.Converters
{
    public abstract class WfClientOguObjectJsonConverterBase<T> : JavaScriptConverter where T : WfClientOguObjectBase
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            T oguObj = CreateInstance(dictionary, type, serializer);

            oguObj.ID = dictionary.GetValue("id", string.Empty);
            oguObj.Name = dictionary.GetValue("name", string.Empty);
            oguObj.DisplayName = dictionary.GetValue("displayName", string.Empty);

            return oguObj;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            T oguObj = (T)obj;

            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            dictionary.AddNonDefaultValue("id", oguObj.ID);
            dictionary.AddNonDefaultValue("name", oguObj.Name);
            dictionary.AddNonDefaultValue("displayName", oguObj.DisplayName);

            return dictionary;
        }

        protected abstract T CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer);
    }

    public class WfClientUserJsonConverter : WfClientOguObjectJsonConverterBase<WfClientUser>
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientUser) };

        protected override WfClientUser CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new WfClientUser();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return _SupportedTypes;
            }
        }
    }

    public class WfClientOrganizationJsonConverter : WfClientOguObjectJsonConverterBase<WfClientOrganization>
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientOrganization) };

        protected override WfClientOrganization CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new WfClientOrganization();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return _SupportedTypes;
            }
        }
    }

    public class WfClientGroupJsonConverter : WfClientOguObjectJsonConverterBase<WfClientGroup>
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientGroup) };

        protected override WfClientGroup CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new WfClientGroup();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return _SupportedTypes;
            }
        }
    }
}
