using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Caching;

namespace MCS.Library.Data.Mapping
{
	/// <summary>
	/// ORMapping过程中的上下文对象
	/// </summary>
	internal class ORMappingContext : IDisposable
	{
		private const string ORMappingContextKey = "ORMappingContext";
		private ORMappintItemEncryptionCollection _ItemEncryptors = null;

		private ORMappingContext OriginalContext
		{
			get;
			set;
		}

		private ORMappingContext()
		{
		}

		public ORMappintItemEncryptionCollection ItemEncryptors
		{
			get
			{
				if (OriginalContext._ItemEncryptors == null)
					OriginalContext._ItemEncryptors = new ORMappintItemEncryptionCollection();

				return OriginalContext._ItemEncryptors;
			}
		}

		public static ORMappingContext GetContext()
		{
			object objContext = null;
			ORMappingContext context = new ORMappingContext();

			if (ObjectContextCache.Instance.TryGetValue(ORMappingContext.ORMappingContextKey, out objContext))
			{
				//如果已经存在，则返回的实例中，指向原始的context
				context = new ORMappingContext();
				context.OriginalContext = (ORMappingContext)objContext;
			}
			else
			{
				//新创建的context是第一次使用的context
				context = new ORMappingContext();
				context.OriginalContext = context;
				ObjectContextCache.Instance.Add(ORMappingContext.ORMappingContextKey, context);
			}

			return context;
		}

		public void Dispose()
		{
			if (this.OriginalContext == this)
			{
				//如果是第一次使用的context,则从上下文缓存中删除
				ObjectContextCache.Instance.Remove(ORMappingContext.ORMappingContextKey);
			}
		}
	}
}
