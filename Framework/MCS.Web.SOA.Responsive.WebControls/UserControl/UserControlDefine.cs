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
	/// ���ػ�����Ա���ڵ�ʱ��ί�ж���
	/// </summary>
	/// <param name="oguObj">������Ա�ڵ�</param>
	/// <param name="newTreeNode">�µ����ڵ�</param>
	/// <param name="cancel">�����Ͳ���������ȡ���ڵ�ļ���</param>
	public delegate void LoadingObjectToTreeNodeDelegate(UserOUGraphControl treeControl, IOguObject oguObj, DeluxeTreeNode newTreeNode, ref bool cancel);

	/// <summary>
	/// �õ��Ӷ���
	/// </summary>
	/// <param name="parent"></param>
	/// <returns></returns>
	public delegate IEnumerable<IOguObject> GetChildrenDelegate(UserOUGraphControl treeControl, IOguObject parent);

	/// <summary>
	/// ����ѡ������
	/// </summary>
	[Flags]
	public enum UserControlObjectMask
	{
		/// <summary>
		/// ����
		/// </summary>
		Organization = 1,

		/// <summary>
		/// ��Ա
		/// </summary>
		User = 2,

		/// <summary>
		/// ��
		/// </summary>
		Group = 4,

		/// <summary>
		/// ��ְ
		/// </summary>
		Sideline = 8,

		/// <summary>
		/// ������Ա
		/// </summary>
		All = 15
	}
}
