using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	internal class BatchFetcher<T> : IEnumerable<T>
	{
		private List<T> innerList = new List<T>();
		private int batchSize = 256;
		private IEnumerator<T> iterator = null;

		public int Count
		{
			get
			{
				return innerList.Count;
			}
		}

		public BatchFetcher(int batchSize, IEnumerator<T> source)
		{
			if (batchSize < 1)
				throw new ArgumentOutOfRangeException("batchSize", "批大小至少为1");
			this.batchSize = batchSize;
			this.iterator = source;
		}

		public int FetchABatch()
		{
			int countDown = this.batchSize;
			this.innerList.Clear();
			while (countDown > 0 && iterator.MoveNext())
			{
				this.innerList.Add(iterator.Current);
				countDown--;
			}

			return this.innerList.Count;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return innerList.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return innerList.GetEnumerator();
		}
	}
}
