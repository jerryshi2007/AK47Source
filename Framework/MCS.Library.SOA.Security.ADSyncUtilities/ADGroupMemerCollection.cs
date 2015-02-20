using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;
using System.DirectoryServices;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	internal class ADGroupMemberCollection : IEnumerable<ADObjectWrapper>
	{
		string[] propertiesToGet = null;
		private System.DirectoryServices.ResultPropertyValueCollection source;

		public ADGroupMemberCollection(System.DirectoryServices.ResultPropertyValueCollection input)
		{
			this.source = input;
		}

		public ADGroupMemberCollection(System.DirectoryServices.ResultPropertyValueCollection input, string[] neededProperties)
			: this(input)
		{
			this.propertiesToGet = neededProperties;
		}

		public IEnumerator<ADObjectWrapper> GetEnumerator()
		{
			int batchSize = 500;
			var context = SynchronizeContext.Current;

			var neededProperties = this.propertiesToGet ?? SynchronizeHelper.GetNeededProperties(ADSchemaType.Users);

			IEnumerable<SearchResult> aBatch;

			for (int i = 0; i < source.Count / batchSize; i++)
			{
				aBatch = GetBatchData(i * batchSize, batchSize, context, neededProperties);
				foreach (var item in aBatch)
				{
					yield return CreateADObjectWrapper(item, neededProperties, context);
				}
			}

			aBatch = GetBatchData(source.Count / batchSize * batchSize, source.Count % batchSize, context, neededProperties);

			foreach (var item in aBatch)
			{
				yield return CreateADObjectWrapper(item, neededProperties, context);
			}
		}

		private ADObjectWrapper CreateADObjectWrapper(SearchResult item, string[] neededProperties, SynchronizeContext context)
		{
			ADObjectWrapper wrapper = new ADObjectWrapper();
			foreach (var property in neededProperties)
			{
				wrapper.Properties.Add(property, context.ADHelper.GetSearchResultPropertyStrValue(property, item));
			}

			return wrapper;
		}

		private IEnumerable<SearchResult> GetBatchData(int startIndex, int batchSize, SynchronizeContext context, string[] neededProperties)
		{
			List<string> buffer = new List<string>(batchSize);
			for (int k = startIndex; k < startIndex + batchSize; k++)
			{
				buffer.Add((string)this.source[k]);
			}

			var results = buffer.Count > 0 ? SynchronizeHelper.GetSearchResultsByDNList(context.ADHelper, buffer, ADSchemaType.Users, neededProperties, batchSize) : new SearchResult[0];

			return results;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
