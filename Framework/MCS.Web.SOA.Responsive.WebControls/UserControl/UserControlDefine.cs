using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using MCS.Web.Library;
using MCS.Web.Responsive.WebControls;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;

namespace MCS.Web.Responsive.WebControls
{
	public delegate void OuUserObjectsLoadedHandler(object sender, IEnumerable data);
	public delegate OguDataCollection<IOguObject> ValidateInputOuUserHandler(string chkString, object context = null);

	/// <summary>
	/// 加载机构人员树节点时的委托定义
	/// </summary>
	/// <param name="oguObj">机构人员节点</param>
	/// <param name="newTreeNode">新的树节点</param>
	/// <param name="cancel">引用型参数，可以取消节点的加载</param>
	public delegate void LoadingObjectToTreeNodeDelegate(UserOUGraphControl treeControl, IOguObject oguObj, DeluxeTreeNode newTreeNode, ref bool cancel);

	/// <summary>
	/// 得到子对象
	/// </summary>
	/// <param name="parent"></param>
	/// <returns></returns>
	public delegate IEnumerable<IOguObject> GetChildrenDelegate(UserOUGraphControl treeControl, IOguObject parent);

	/// <summary>
	/// 对象选择掩码
	/// </summary>
	[Flags]
	public enum UserControlObjectMask
	{
		/// <summary>
		/// 机构
		/// </summary>
		Organization = 1,

		/// <summary>
		/// 人员
		/// </summary>
		User = 2,

		/// <summary>
		/// 组
		/// </summary>
		Group = 4,

		/// <summary>
		/// 兼职
		/// </summary>
		Sideline = 8,

		/// <summary>
		/// 所有人员
		/// </summary>
		All = 15
	}
}
