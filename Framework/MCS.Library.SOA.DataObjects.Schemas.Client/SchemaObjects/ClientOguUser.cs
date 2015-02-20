using System;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Schemas.Client
{
	/// <summary>
	/// 封装后的OguUser
	/// </summary>
	[Serializable]
	public class ClientOguUser
	{
		#region 构造方法
		public ClientOguUser()
		{
		}

		public ClientOguUser(IOguObject oguObject)
		{
			oguObject.NullCheck("oguObject");

			this.ID = oguObject.ID;
			this.DisplayName = oguObject.DisplayName;
		}
		#endregion 构造方法

		#region 属性定义
		/// <summary>
		/// 对象ID
		/// </summary>
		public string ID
		{
			get;
			set;
		}

		/// <summary>
		/// 显示名称
		/// </summary>
		public string DisplayName
		{
			get;
			set;
		}
		#endregion 属性定义

		///删除此方法，去掉对SOA.DataObject的依赖性
		///// <summary>
		///// 转换为OguUser
		///// </summary>
		///// <returns></returns>
		//public IUser ToOguUser()
		//{
		//    OguUser user = new OguUser(this.ID);

		//    user.DisplayName = this.DisplayName;
		//    user.Name = this.DisplayName;

		//    return user;
		//}
	}
}
