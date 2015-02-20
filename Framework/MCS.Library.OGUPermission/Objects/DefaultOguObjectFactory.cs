using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission.Properties;

namespace MCS.Library.OGUPermission
{
	/// <summary>
	/// 缺省的OguObjectFactory
	/// </summary>
	internal class DefaultOguObjectFactory : IOguObjectFactory
	{
		public static readonly IOguObjectFactory Instance = new DefaultOguObjectFactory();

		private DefaultOguObjectFactory()
		{
		}

		#region IOguObjectFactory Members

		/// <summary>
		/// 创建机构人员组对象
		/// </summary>
		/// <param name="type">需要创建的对象类型</param>
		/// <returns></returns>
		public IOguObject CreateObject(SchemaType type)
		{
			OguBaseImpl oBase = null;

			switch (type)
			{
				case SchemaType.Users:
					oBase = new OguUserImpl();
					break;
				case SchemaType.Organizations:
					oBase = new OguOrganizationImpl();
					break;
				case SchemaType.OrganizationsInRole:
					oBase = new OguOrganizationInRoleImpl();
					break;
				case SchemaType.Groups:
					oBase = new OguGroupImpl();
					break;
				default:
					throw new InvalidOperationException(string.Format(Resource.InvalidObjectTypeCreation, type.ToString()));
			}

			return oBase;
		}

		#endregion
	}
}
