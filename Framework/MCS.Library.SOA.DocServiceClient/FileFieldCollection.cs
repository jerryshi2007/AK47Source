using System.Runtime.Serialization;
using MCS.Library.Data.DataObjects;
using MCS.Library.Services.Contracts;
using MCS.Library.SOA.DocServiceContract;

namespace MCS.Library.SOA.DocServiceClient
{
	public class FileFieldCollection : SerializableEditableKeyedDataObjectCollectionBase<string, DCTFileField>
	{
		public FileFieldCollection()
		{
		}

		public FileFieldCollection(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		protected override string GetKeyForItem(DCTFileField item)
		{
			return item.Field.Title;
		}
	}
}
