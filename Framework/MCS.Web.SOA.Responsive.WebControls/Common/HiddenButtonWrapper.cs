using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using MCS.Web.Library;
using MCS.Web.Responsive.Library;

namespace MCS.Web.Responsive.WebControls
{
	/// <summary>
	/// �Բ��ɼ���asp.net Button��ҳ���ϵ�Link��Button��װ��
	/// </summary>
	[Serializable]
	public class HiddenButtonWrapper
	{
		private string targetControlID = string.Empty;

		[NonSerialized]
		private IAttributeAccessor targetControl = null;

		[NonSerialized]
		private SubmitButton hiddenButton = null;

		public string TargetControlID
		{
			get
			{
				return this.targetControlID;
			}
			set
			{
				this.targetControlID = value;
			}
		}

		public IAttributeAccessor TargetControl
		{
			get
			{
				if (this.targetControl == null)
				{
					if (string.IsNullOrEmpty(this.targetControlID) == false)
                        this.targetControl = (IAttributeAccessor)WebUtility.GetCurrentPage().FindControlByID(this.targetControlID, true);
				}

				return this.targetControl;
			}
			set
			{
				this.targetControl = value;
			}
		}

		public string HiddenButtonClientID
		{
			get
			{
				string result = string.Empty;

				if (this.hiddenButton != null)
					result = this.hiddenButton.ClientID;

				return result;
			}
		}

		public SubmitButton HiddenButton
		{
			get
			{
				return this.hiddenButton;
			}
		}

		public void CreateHiddenButton(string id, EventHandler clickHandler)
		{
			SubmitButton btn = new SubmitButton();

			btn.ID = id;
			btn.Click += clickHandler;
			btn.Style["display"] = "none";
			btn.RelativeControlID = this.targetControlID;

			this.hiddenButton = btn;
		}
	}
}
