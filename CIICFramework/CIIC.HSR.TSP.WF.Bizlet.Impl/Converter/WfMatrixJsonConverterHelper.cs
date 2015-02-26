using CIIC.HSR.TSP.WF.Bizlet.Contract;
using MCS.Web.Library.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using CIIC.HSR.TSP.WF.Bizlet.Impl.Converter;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
   public class WfMatrixJsonConverterHelper
    {
       public static readonly WfMatrixJsonConverterHelper Instance = new WfMatrixJsonConverterHelper();

        /// <summary>
        /// 注册相关的序列化器
        /// </summary>
         public void RegisterConverters(JavaScriptSerializer serializer)
         {
             IEnumerable<JavaScriptConverter> converters = new JavaScriptConverter[] { 
                 new WfMatrixProcessJsonConverter(),
                 new WfMatrixActivityJsonConverter(),
                  new WfMatrixConditionGroupCollectionJsonConverter(),
                  new WfMatrixConditionGroupJsonConverter(),
                  new WfMatrixConditionJsonConverter(),
                  new WfMatrixCandidateJsonConverter(),
                  new WfMatrixParameterJsonConverter()
             };

             serializer.RegisterConverters(converters);
             foreach (JavaScriptConverter item in converters)
             {
                 JSONSerializerExecute.RegisterConverter(item.GetType());
             }
         }

         public void FillDeserializedParameterDefinition(object serializedData, IList<IWfMatrixParameterDefinition> target)
         {
             if (serializedData != null)
             {
                 WfMatrixParameterDefinition[] deserializedArray = JSONSerializerExecute.Deserialize<WfMatrixParameterDefinition[]>(serializedData);

                 target.Clear();

                 deserializedArray.ForEach(d => target.Add(d));
             }
         }
    
    }
}
