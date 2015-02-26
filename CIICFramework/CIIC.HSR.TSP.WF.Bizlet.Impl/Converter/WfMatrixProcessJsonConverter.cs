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
    public class WfMatrixProcessJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfMatrixProcess) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfMatrixProcess process = new WfMatrixProcess();

            process.Key = dictionary.GetValue("Key", string.Empty);
            process.Name = dictionary.GetValue("Name", string.Empty);
            process.ProgramName = dictionary.GetValue("ProgramName", string.Empty);
            process.Url = dictionary.GetValue("Url", string.Empty);
            process.TenantCode = dictionary.GetValue("TenantCode", string.Empty);
            process.ApplicationName = dictionary.GetValue("ApplicationName", string.Empty);
            process.Description = dictionary.GetValue("Description", string.Empty);

            //Properties
            JSONSerializerExecute.FillDeserializedDictionary(dictionary, "Properties", process.Properties);

            //ParameterDefinitions    
            WfMatrixJsonConverterHelper.Instance.FillDeserializedParameterDefinition(dictionary.GetValue("ParameterDefinitions", (object)null), process.ParameterDefinitions);

            //Activities
            FillDeserializedActivity(dictionary.GetValue("Activities", (object)null), process.Activities);

            //GlobalParameterDefinitions
            WfMatrixJsonConverterHelper.Instance.FillDeserializedParameterDefinition(dictionary.GetValue("GlobalParameterDefinitions", (object)null), process.GlobalParameterDefinitions);
            
            return process;
        }

       private static void FillDeserializedActivity(object serializedData, IList<IWfMatrixActivity> target)
        {
            if (serializedData != null)
            {
                WfMatrixActivity[] deserializedArray = JSONSerializerExecute.Deserialize<WfMatrixActivity[]>(serializedData);

                target.Clear();
                deserializedArray.ForEach(d => target.Add(d));
            }
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            WfMatrixProcess process = (WfMatrixProcess)obj;
            dictionary.Add("Key", process.Key);
            dictionary.Add("Name", process.Name);
            dictionary.Add("ProgramName", process.ProgramName);
            dictionary.Add("Url", process.Url);
            dictionary.Add("TenantCode", process.TenantCode);
            dictionary.Add("ApplicationName", process.ApplicationName);
            dictionary.Add("Description", process.Description);
             
            //Properties
            dictionary.Add("Properties", process.Properties);
            //ParameterDefinitions  
            dictionary.Add("ParameterDefinitions", process.ParameterDefinitions);
            //Activities
            dictionary.Add("Activities", process.Activities);
            //GlobalParameterDefinitions
            dictionary.Add("GlobalParameterDefinitions", process.GlobalParameterDefinitions);


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
