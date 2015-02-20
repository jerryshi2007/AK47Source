using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using MCS.Web.Library;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// UserControl的参数
	/// </summary>
	[Serializable]
	public class ExtConsignUserSelectorParams : DialogControlParamsBase
	{
		public const string DefaultDialogTitle = "请选择人员";

		[NonSerialized]
		private IOrganization root;

		private string rootID = string.Empty;
		private UserControlObjectMask listMask = UserControlObjectMask.Organization | UserControlObjectMask.User | UserControlObjectMask.Sideline;
		private UserControlObjectMask selectMask = UserControlObjectMask.User;
		private bool multiSelect = true;
		private bool isConsign = false;

		/// <summary>
		/// 根部门
		/// </summary>
		public IOrganization Root
		{
			get
			{
				if (this.root == null)
					if (string.IsNullOrEmpty(this.rootID) == false)
						this.root = OguMechanismFactory.GetMechanism().GetObjects<IOrganization>(SearchOUIDType.Guid, this.rootID)[0];

				return this.root;
			}
			set
			{
				this.root = value;
			}
		}

		/// <summary>
		/// 列表的掩码
		/// </summary>
		public UserControlObjectMask ListMask
		{
			get
			{
				return this.listMask;
			}
			set
			{
				this.listMask = value;
			}
		}

		/// <summary>
		/// 选择的掩码
		/// </summary>
		public UserControlObjectMask SelectMask
		{
			get
			{
				return this.selectMask;
			}
			set
			{
				this.selectMask = value;
			}
		}

		/// <summary>
		/// 是否多选
		/// </summary>
		public bool MultiSelect
		{
			get
			{
				return this.multiSelect;
			}
			set
			{
				this.multiSelect = value;
			}
		}

		/// <summary>
		/// 是否是会签控件
		/// </summary>
		public bool IsConsign
		{
			get
			{
				return this.isConsign;
			}
			set
			{
				this.isConsign = value;
			}
		}

		/// <summary>
		/// 从Request的QueryString获得类参数
		/// </summary>
		public override void LoadDataFromQueryString()
		{
			this.rootID = WebUtility.GetRequestQueryValue("uspRoot", string.Empty);
			this.listMask = WebUtility.GetRequestQueryValue("uspListMask", UserControlObjectMask.All);
			this.selectMask = WebUtility.GetRequestQueryValue("uspSelectMask", UserControlObjectMask.All);
			this.multiSelect = WebUtility.GetRequestQueryValue("uspMultiSelect", false);
			this.isConsign = WebUtility.GetRequestQueryValue("uspIsConsign", false);

			base.LoadDataFromQueryString();
		}

		/// <summary>
		/// 将类参数添加到url中
		/// </summary>
		/// <param name="strB"></param>
		protected override void BuildRequestParams(StringBuilder strB)
		{
			base.BuildRequestParams(strB);

			if (this.root != null)
				base.AppendParam(strB, "uspRoot", this.root.ID);

			if (this.listMask != UserControlObjectMask.All)
				AppendParam(strB, "uspListMask", this.listMask);

			if (this.selectMask != UserControlObjectMask.All)
				AppendParam(strB, "uspSelectMask", this.selectMask);

			if (this.multiSelect)
				AppendParam(strB, "uspMultiSelect", this.multiSelect);

			if (this.isConsign)
				AppendParam(strB, "uspIsConsign", this.isConsign);
		}
	}
}
