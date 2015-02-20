using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Web.WebControls
{

	/// <summary>
	/// 电话显示模式
	/// </summary>
	/*
         ShowExtNumber = 1
         PhoneNumber=2
         AreaNumber=4;
         StateNumber=8;
    */
	[Flags]
	public enum PhoneNumberUseMode
	{
		/// <summary>
		/// 电话
		/// </summary>
		OnlyPhoneNumber = 2,

		/// <summary>
		/// 电话 - 分机
		/// </summary>
		PhoneAndExtNumber = 3,

		/// <summary>
		/// 区号 - 电话
		/// </summary>
		AreaAndPhoneNumber = 6,

		/// <summary>
		/// 区号 - 电话 - 分机
		/// </summary>
		AreaAndPhoneAndExtNumber = 7,

		/// <summary>
		/// 国别 - 手机
		/// </summary>
		StateAndCellPhone = 10,

		/// <summary>
		/// 国别 - 区号 - 电话 - 分机
		/// </summary>
		StateAndAreaAndPhoneAndExtNumber = 15
	}

    /// <summary>
    /// 电话类别
    /// </summary>
    public enum PhoneNumberCategory
    {
        /// <summary>
        /// 手机
        /// </summary>
        Cellphone = 1,

        /// <summary>
        /// 固定电话
        /// </summary>
        Phone = 2
    }
}
