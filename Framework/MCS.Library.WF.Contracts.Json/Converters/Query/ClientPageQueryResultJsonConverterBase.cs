using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.WF.Contracts.Query;
using MCS.Web.Library.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Json.Converters.Query
{
    public abstract class ClientPageQueryResultJsonConverterBase<TData, TItem, TCollection> : JavaScriptConverter
        where TItem : new()
        where TCollection : EditableDataObjectCollectionBase<TItem>, new()
        where TData : ClientPageQueryResultBase<TItem, TCollection>
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            TData data = CreateInstance(dictionary, serializer);

            data.TotalCount = dictionary.GetValue("totalCount", 0);
            JSONSerializerExecute.FillDeserializedCollection(dictionary.GetValue("queryResult", (object)null), data.QueryResult);

            return data;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            IDictionary<string, object> dictionary = new Dictionary<string, object>();

            TData data = (TData)obj;

            dictionary["totalCount"] = data.TotalCount;
            dictionary["queryResult"] = data.QueryResult;

            return dictionary;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new Type[] { typeof(TData) };
            }
        }

        public abstract TData CreateInstance(IDictionary<string, object> dictionary, JavaScriptSerializer serializer);
    }
}
