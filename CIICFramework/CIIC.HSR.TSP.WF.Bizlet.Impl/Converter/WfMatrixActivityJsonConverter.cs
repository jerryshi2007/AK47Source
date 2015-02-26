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
    public class WfMatrixActivityJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfMatrixActivity) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfMatrixActivity activity = new WfMatrixActivity();

            activity.ActivityType = dictionary.GetValue("ActivityType", WfMaxtrixActivityType.NormalActivity);         
            activity.CodeName = dictionary.GetValue("CodeName", string.Empty);
            activity.Description = dictionary.GetValue("Description", string.Empty);         
            activity.Name = dictionary.GetValue("Name", string.Empty);         
            activity.Url = dictionary.GetValue("Url", string.Empty);

            JSONSerializerExecute.FillDeserializedDictionary(dictionary, "Properties", activity.Properties);

            FillDeserializedCandidates(dictionary.GetValue("Candidates", (object)null), activity.Candidates);

            FillDeserializedExpression(dictionary.GetValue("Expression", (object)null), activity.Expression);
                     
           
            return activity;
        }
        private static void FillDeserializedCandidates(object serializedData, IList<IWfMatrixCandidate> target)
        {
            if (serializedData != null)
            {
                WfMatrixCandidate[] deserializedArray = JSONSerializerExecute.Deserialize<WfMatrixCandidate[]>(serializedData);

                target.Clear();
                deserializedArray.ForEach(d => target.Add(d));
            }
        }
        private static void FillDeserializedExpression(object serializedData, IWfMatrixConditionGroupCollection target)
        {
            if (serializedData != null)
            {

                Dictionary<string, object> tmp = serializedData as Dictionary<string, object>;
                object subData = tmp.GetValue("Data", (object)null);
                if (subData == null)
                    return;


                WfMatrixConditionGroup[] deserializedArray = JSONSerializerExecute.Deserialize<WfMatrixConditionGroup[]>(subData);

                target.Clear();
                deserializedArray.ForEach(d => target.Add(d));
            }
        }
         

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            WfMatrixActivity activity = (WfMatrixActivity)obj;

            dictionary.Add("ActivityType", activity.ActivityType);
            dictionary.Add("Candidates", activity.Candidates);
            dictionary.Add("CodeName", activity.CodeName);
            dictionary.Add("Description", activity.Description);
            dictionary.Add("Expression", activity.Expression);

            string str = string.Empty;
            if (activity.Expression != null)
                str =  activity.Expression.ToExpression();
            dictionary.Add("strExpression",str);

            dictionary.Add("Name", activity.Name);
            dictionary.Add("Properties", activity.Properties);
            dictionary.Add("Url", activity.Url); 

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
