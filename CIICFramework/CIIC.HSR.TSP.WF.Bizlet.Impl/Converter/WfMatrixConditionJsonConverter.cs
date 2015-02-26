using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using CIIC.HSR.TSP.WF.Bizlet.Common;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Converter
{
    public class WfMatrixConditionJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfMatrixCondition) };


        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfMatrixCondition group = new WfMatrixCondition();
            group.Value = dictionary.GetValue("Value", string.Empty);
            group.Sign = dictionary.GetValue("Sign", ComparsionSign.Equal);

            group.Parameter = JSONSerializerExecute.Deserialize<WfMatrixParameterDefinition>(dictionary.GetValue("Parameter", (object)null)); 
            
            return group;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            WfMatrixCondition condition = (WfMatrixCondition)obj;
            dictionary.Add("Parameter", condition.Parameter);
            dictionary.Add("Value", condition.Value);
            dictionary.Add("Sign", condition.Sign);
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
