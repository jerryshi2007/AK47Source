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
    public class WfMatrixConditionGroupJsonConverter:JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfMatrixConditionGroup) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfMatrixConditionGroup group = new WfMatrixConditionGroup();
            group.Relation = dictionary.GetValue("Relation", LogicalRelation.Or);
            FillDeserialized(dictionary.GetValue("Data", (object)null), group);
            return group;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            WfMatrixConditionGroup group = (WfMatrixConditionGroup)obj;
            dictionary.Add("Relation", group.Relation);
            dictionary.Add("Data", FillSerializedData(group));            
            return dictionary;
        }


        private static void FillDeserialized(object serializedData, IList<IWfMatrixCondition> target)
        {
            if (serializedData != null)
            {
                WfMatrixCondition[] deserializedArray =   JSONSerializerExecute.Deserialize<WfMatrixCondition[]>(serializedData);

                target.Clear();
                deserializedArray.ForEach(d => target.Add(d));
            }
        }
      

        private List<WfMatrixCondition> FillSerializedData(WfMatrixConditionGroup group)
        {
            List<WfMatrixCondition> list = new List<WfMatrixCondition>();

            foreach (IWfMatrixCondition item in group)
            {
                list.Add(item as WfMatrixCondition);
            }
            return list;
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
