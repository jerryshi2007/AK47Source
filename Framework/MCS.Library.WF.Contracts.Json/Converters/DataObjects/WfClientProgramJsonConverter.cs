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
    public class WfClientProgramJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientProgram) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientProgram program = new WfClientProgram();

            program.ApplicationCodeName = dictionary.GetValue("applicationCodeName", string.Empty);
            program.CodeName = dictionary.GetValue("codeName", string.Empty);
            program.Name = dictionary.GetValue("name", string.Empty);
            program.Sort = dictionary.GetValue("sort", 0);

            return program;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            WfClientProgram program = (WfClientProgram)obj;

            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            dictionary.AddNonDefaultValue("applicationCodeName", program.ApplicationCodeName);
            dictionary.AddNonDefaultValue("codeName", program.CodeName);
            dictionary.AddNonDefaultValue("name", program.Name);
            dictionary.AddNonDefaultValue("sort", program.Sort);

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
