using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using htmlControls = System.Web.UI.HtmlControls;

namespace MCS.Web.Responsive.WebControls
{
    /// <summary>
    /// checkbox模版
    /// </summary>
    /// <remarks>
    /// checkbox模版
    /// </remarks>
    public class CheckBoxTemplate : ITemplate
    {
        private DataControlRowType templateType;
        private bool isMultiSelect = true;
        private string checkItemName = "checkItem";

        /// <summary>
        /// checkbox模版构造函数
        /// </summary>
        /// <remarks>
        /// checkbox模版构造函数
        /// </remarks>
        public CheckBoxTemplate()
        {
        }

        /// <summary>
        /// 模版构造函数
        /// </summary>
        /// <param name="type">指定数据控件中行的功能类型</param>
        /// <param name="multiSelect">是否多选</param>
        /// <param name="chkItemName"></param>
        /// <remarks>
        /// 模版构造函数
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
                    checkBoxAll.Value = "选全";
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
