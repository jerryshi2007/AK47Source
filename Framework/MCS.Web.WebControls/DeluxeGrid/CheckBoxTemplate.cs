using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using htmlControls = System.Web.UI.HtmlControls;


namespace MCS.Web.WebControls
{
	/// <summary>
	/// checkboxģ��
	/// </summary>
	/// <remarks>
	/// checkboxģ��
	/// </remarks>
	public class CheckBoxTemplate : ITemplate
	{
		private DataControlRowType templateType;
		private bool isMultiSelect = true;
		private string checkItemName = "checkItem";

		/// <summary>
		/// checkboxģ�湹�캯��
		/// </summary>
		/// <remarks>
		/// checkboxģ�湹�캯��
		/// </remarks>
		public CheckBoxTemplate()
		{

		}

		/// <summary>
		/// ģ�湹�캯��
		/// </summary>
		/// <param name="type">ָ�����ݿؼ����еĹ�������</param>
		/// <param name="multiSelect">�Ƿ��ѡ</param>
		/// <param name="chkItemName"></param>
		/// <remarks>
		/// ģ�湹�캯��
		/// </remarks>
		public CheckBoxTemplate(DataControlRowType type, string chkItemName, bool multiSelect)
		{
			this.templateType = type;
			this.isMultiSelect = multiSelect;
			this.checkItemName = chkItemName;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="container"></param>
		public void InstantiateIn(Control container)
		{
			switch (this.templateType)
			{
				case DataControlRowType.Header:
					htmlControls.HtmlInputCheckBox checkBoxAll = new htmlControls.HtmlInputCheckBox();
					checkBoxAll.ID = "checkall";
					checkBoxAll.Name = this.checkItemName;
					checkBoxAll.Value = "ѡȫ";
					container.Controls.Add(checkBoxAll);

					if (this.isMultiSelect == false)
						checkBoxAll.Style["display"] = "none";

					break;
				case DataControlRowType.DataRow:
					InputButton checkBox = new InputButton();

					checkBox.Name = this.checkItemName;
					checkBox.ID = "checkitem";

					if (this.isMultiSelect)
						checkBox.ButtonType = InputButtonType.CheckBox;
					else
						checkBox.ButtonType = InputButtonType.Radio;

					checkBox.Value = "";
					container.Controls.Add(checkBox);
					break;
			}
		}
	}
}
