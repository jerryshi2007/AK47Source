using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.AppAdmin
{
	/// <summary>
	/// 人员标识类型
	/// </summary>
	public enum UserValueType
	{
		/// <summary>
		/// 登录名
		/// </summary>
		LogonName = 1,
		/// <summary>
		/// 人员全路径
		/// </summary>
		AllPath = 2,
		/// <summary>
		/// 人员编号
		/// </summary>
		PersonID = 3,
		/// <summary>
		/// IC卡号
		/// </summary>
		ICCode = 4,
		/// <summary>
		/// 人员Guid值
		/// </summary>
		Guid = 8,
		/// <summary>
		/// 根据唯一索引查询(为配合南京海关统一平台切换，新增加字段ID[自增唯一字段])
		/// </summary>
		Identity = 16
	}
}
