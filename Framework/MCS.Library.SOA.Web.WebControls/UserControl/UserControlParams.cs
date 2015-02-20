using System;
using System.Web;
using System.Text;
using System.Collections.Generic;
using MCS.Web.Library;
using MCS.Web.WebControls;
using MCS.Library.OGUPermission;
using MCS.Library.Core;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// UserControl�Ĳ���
	/// </summary>
	[Serializable]
	public class UserControlParams : DialogControlParamsBase
	{
		public const string DefaultDialogTitle = "��ѡ���������Ա";

		[NonSerialized]
		private IOrganization root;

		private string rootPath = string.Empty;
		private UserControlObjectMask listMask = UserControlObjectMask.All;
		private UserControlObjectMask selectMask = UserControlObjectMask.All;
		private bool multiSelect = false;
		private bool mergeSelectResult = false;
		private bool showDeletedObjects = false;
		private bool enableUserPresence = true;

		/// <summary>
		/// ������
		/// </summary>
		public IOrganization Root
		{
			get
			{
				if (this.root == null)
					if (this.rootPath.IsNotEmpty())
					{
						IOrganization org = UserOUControlSettings.GetConfig().UserOUControlQuery.GetOrganizationByPath(this.RootPath);

						ExceptionHelper.NullCheck(org, string.Format("���ܸ���Path�ҵ�����'{0}'", rootPath));

						this.root = org;
					}

				return this.root;
			}
			set
			{
				this.root = value;
			}
		}

		/// <summary>
		/// ��·��
		/// </summary>
		public string RootPath
		{
			get
			{
				return this.rootPath;
			}
			set
			{
				this.rootPath = value;

			}
		}

		/// <summary>
		/// �б������
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
		/// ѡ�������
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
		/// �Ƿ��ѡ
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
		/// �Ƿ�ϲ�ѡ����
		/// </summary>
		public bool MergeSelectResult
		{
			get
			{
				return this.mergeSelectResult;
			}
			set
			{
				this.mergeSelectResult = value;
			}
		}

		/// <summary>
		/// �Ƿ���ʾ�߼�ɾ���Ķ���
		/// </summary>
		public bool ShowDeletedObjects
		{
			get
			{
				return this.showDeletedObjects;
			}
			set
			{
				this.showDeletedObjects = value;
			}
		}

		/// <summary>
		/// �Ƿ���ʾ�û���״̬
		/// </summary>
		public bool EnableUserPresence
		{
			get
			{
				return this.enableUserPresence;
			}
			set
			{
				this.enableUserPresence = value;
			}
		}

		/// <summary>
		/// ��Request��QueryString��������
		/// </summary>
		public override void LoadDataFromQueryString()
		{
			this.rootPath = WebUtility.GetRequestQueryValue("ucpRoot", string.Empty);
			this.listMask = WebUtility.GetRequestQueryValue("ucpListMask", UserControlObjectMask.All);
			this.selectMask = WebUtility.GetRequestQueryValue("ucpSelectMask", UserControlObjectMask.All);
			this.multiSelect = WebUtility.GetRequestQueryValue("ucpMultiSelect", false);
			this.mergeSelectResult = WebUtility.GetRequestQueryValue("msr", false);
			this.showDeletedObjects = WebUtility.GetRequestQueryValue("sdo", false);
			this.enableUserPresence = WebUtility.GetRequestQueryValue("eup", true);

			base.LoadDataFromQueryString();
		}

		/// <summary>
		/// ���������ӵ�url��
		/// </summary>
		/// <param name="strB"></param>
		protected override void BuildRequestParams(StringBuilder strB)
		{
			base.BuildRequestParams(strB);

			if (this.root != null)
				base.AppendParam(strB, "ucpRoot", this.root.FullPath);

			if (this.listMask != UserControlObjectMask.All)
				AppendParam(strB, "ucpListMask", this.listMask);

			if (this.selectMask != UserControlObjectMask.All)
				AppendParam(strB, "ucpSelectMask", this.selectMask);

			if (this.multiSelect)
				AppendParam(strB, "ucpMultiSelect", this.multiSelect);

			if (this.mergeSelectResult)
				AppendParam(strB, "msr", this.mergeSelectResult);

			if (this.showDeletedObjects)
				AppendParam(strB, "sdo", this.showDeletedObjects);

			if (this.enableUserPresence == false)
				AppendParam(strB, "eup", this.enableUserPresence);
		}
	}
}
