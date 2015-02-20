using System.Runtime.Serialization;
using MCS.Library.Data.DataObjects;
using MCS.Library.Services.Contracts;
using MCS.Library.SOA.DocServiceContract;

namespace MCS.Library.SOA.DocServiceClient
{
	public class DCTFieldInfoDictionary : SerializableEditableKeyedDataObjectCollectionBase<string, DCTFieldInfo>
	{
		public DCTFieldInfoDictionary()
		{
		}

		public DCTFieldInfoDictionary(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		protected override string GetKeyForItem(DCTFieldInfo item)
		{
			return item.Title;
		}
	}
}
