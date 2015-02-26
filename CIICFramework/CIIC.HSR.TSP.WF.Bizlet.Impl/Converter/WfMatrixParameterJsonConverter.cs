using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;
using CIIC.HSR.TSP.WF.Bizlet.Contract;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public class WfMatrixParameterJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfMatrixParameterDefinition) };


        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfMatrixParameterDefinition parameter = new WfMatrixParameterDefinition();

            parameter.Name = dictionary.GetValue("Name", string.Empty);
            parameter.DisplayName = dictionary.GetValue("DisplayName", string.Empty);
            parameter.DefaultValue = dictionary.GetValue("DefaultValue", string.Empty);
            parameter.Description = dictionary.GetValue("Description", string.Empty);
            parameter.Enabled = dictionary.GetValue("Enabled", true);
            parameter.ParameterType = dictionary.GetValue("ParameterType", Common.ParaType.String);
                     
            return parameter;
        }     

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            WfMatrixParameterDefinition parameter = (WfMatrixParameterDefinition)obj;
             

            dictionary.Add("Name", parameter.Name);
            dictionary.Add("DisplayName", parameter.DisplayName);
            dictionary.Add("DefaultValue", parameter.DefaultValue);
            dictionary.Add("Description", parameter.Description);
            dictionary.Add("Enabled", parameter.Enabled);
            dictionary.Add("ParameterType", parameter.ParameterType);
          

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
