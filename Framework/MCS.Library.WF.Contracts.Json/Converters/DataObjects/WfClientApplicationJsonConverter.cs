using MCS.Library.Core;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Json.Converters.DataObjects
{
    public class WfClientApplicationJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientApplication) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientApplication application = new WfClientApplication();

            application.CodeName = dictionary.GetValue("codeName", string.Empty);
            application.Name = dictionary.GetValue("name", string.Empty);
            application.Sort = dictionary.GetValue("sort", 0);

            return application;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            WfClientApplication application = (WfClientApplication)obj;

            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            dictionary.AddNonDefaultValue("codeName", application.CodeName);
            dictionary.AddNonDefaultValue("name", application.Name);
            dictionary.AddNonDefaultValue("sort", application.Sort);

            return dictionary;
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
