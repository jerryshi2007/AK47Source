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
    public class WfMatrixCandidateJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfMatrixCandidate) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfMatrixCandidate candidate = new WfMatrixCandidate();
            candidate.Candidate = JSONSerializerExecute.Deserialize<WfMatrixParameterDefinition>(dictionary.GetValue("Candidate", (object)null));

            candidate.ResourceType = dictionary.GetValue("ResourceType", string.Empty); 
            return candidate;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            WfMatrixCandidate candidate = (WfMatrixCandidate)obj;
            dictionary.Add("ResourceType", candidate.ResourceType);
            dictionary.Add("Candidate", candidate.Candidate);
          
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
