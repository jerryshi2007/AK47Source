using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.OGUPermission
{
	/// <summary>
	/// OguObject的辅助方法
	/// </summary>
	public static class OguObjectHelper
	{
		/// <summary>
		/// 根据IOguObject的类型得到SchemaType
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static SchemaType GetSchemaTypeFromInterface<T>() where T : IOguObject
		{
			SchemaType type = SchemaType.Unspecified;

			if (typeof(T) == typeof(IOguObject))
				type = SchemaType.All & ~SchemaType.Sideline;
			else
				if (typeof(T) == typeof(IOrganization))
					type = SchemaType.Organizations;
				else
					if (typeof(T) == typeof(IOrganizationInRole))
						type = SchemaType.OrganizationsInRole;
					else
						if (typeof(T) == typeof(IUser))
							type = SchemaType.Users;
						else
							if (typeof(T) == typeof(IGroup))
								type = SchemaType.Groups;

			return type;
		}
	}
}
