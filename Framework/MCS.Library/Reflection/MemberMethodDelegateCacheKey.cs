using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;
using System.Reflection;

namespace MCS.Library.Core
{
	internal struct PropertyMethodDelegateCacheKey
	{
		public PropertyInfo MemberInfo;
		public Type ValueType;

		public PropertyMethodDelegateCacheKey(PropertyInfo pi, Type vt)
		{
			this.MemberInfo = pi;
			this.ValueType = vt;
		}
	}

	internal struct FieldMethodDelegateCacheKey
	{
		public FieldInfo MemberInfo;
		public Type ValueType;

		public FieldMethodDelegateCacheKey(FieldInfo pi, Type vt)
		{
			this.MemberInfo = pi;
			this.ValueType = vt;
		}
	}
}
