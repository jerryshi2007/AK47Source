using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;
using System.Web.Script.Serialization;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using MCS.Web.Library.Script;
using CIIC.HSR.TSP.WF.Bizlet.Common;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public class WfMatrixConditionGroupCollectionJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfMatrixConditionGroupCollection) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfMatrixConditionGroupCollection group = new WfMatrixConditionGroupCollection();
            group.Relation = dictionary.GetValue("Relation", LogicalRelation.Or);
            FillDeserialized(dictionary.GetValue("Data", (object)null), group);
            
            return group;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            WfMatrixConditionGroupCollection group = (WfMatrixConditionGroupCollection)obj;          
            dictionary.Add("Relation", group.Relation);
            dictionary.Add("Data", FillSerializedData(group));
        
            return dictionary;
        }

        private static void FillDeserialized(object serializedData, IList<IWfMatrixConditionGroup> target)
        {
            if (serializedData != null)
            {
                WfMatrixConditionGroup[] deserializedArray = JSONSerializerExecute.Deserialize<WfMatrixConditionGroup[]>(serializedData);

                target.Clear();
                deserializedArray.ForEach(d => target.Add(d));
            }
        }
      

        private List<WfMatrixConditionGroup> FillSerializedData(WfMatrixConditionGroupCollection group)
        {
            List<WfMatrixConditionGroup> list = new List<WfMatrixConditionGroup>();

            foreach (IWfMatrixConditionGroup item in group)
            {
                list.Add(item as WfMatrixConditionGroup);
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
