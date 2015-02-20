using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace MCS.Web.Responsive.WebControls
{
    /// <summary>
    /// checkbox事件句柄
    /// </summary>
    public class DeluxeGridCheckBoxEventArgs : EventArgs
    {
        private string checkBoxValue = string.Empty;
        private bool checkBoxVisible = true;
        private GridViewRowEventArgs rowEventArgs = null;

        /// <summary>
        /// 行信息
        /// </summary>
        public GridViewRowEventArgs RowEventArgs
        {
            get { return rowEventArgs; }
            internal set { rowEventArgs = value; }
        }

        /// <summary>
        /// checkbox值
        /// </summary>
        public string CheckBoxValue
        {
            get { return this.checkBoxValue; }
            set { this.checkBoxValue = value; }
        }

        /// <summary>
        /// checkbox是否显示
        /// </summary>
        public bool CheckBoxVisible
        {
            get { return this.checkBoxVisible; }
            set { this.checkBoxVisible = value; }
        }
    }
}
