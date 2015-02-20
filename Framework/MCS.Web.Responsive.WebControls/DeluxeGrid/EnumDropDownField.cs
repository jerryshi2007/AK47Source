using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;

namespace MCS.Web.Responsive.WebControls
{
    /// <summary>
    /// 绑定到枚举值
    /// </summary>
    public class EnumDropDownField : BoundField
    {
        private bool suppressPropertyThrows = false;
        private Type enumType;

        List<EnumItem> dataSource = new List<EnumItem>();

        private class EnumItem
        {
            public EnumItem()
            {
            }

            public EnumItem(string key, string text)
            {
                this.Key = key;
                this.Text = text;
            }

            public string Key { get; set; }
            public string Text { get; set; }
        }

        /// <summary>
        /// 获取或设置使用枚举的名称作为值，而不是整数。
        /// </summary>
        [DefaultValue(false), Description("枚举值列使用名称作为其值"), Localizable(false), Category("Behavior")]
        public bool UseNameAsValue
        {
            get
            {
                object o = this.ViewState["UseNameAsValue"];
                if (o is bool)
                    return (bool)o;
                return false;
            }

            set { this.ViewState["UseNameAsValue"] = value; }
        }

        /// <summary>
        /// 绑定的枚举类型
        /// </summary>
        [DefaultValue(""), Description("枚举类型的类型名称"), Localizable(false), Category("Behavior")]
        public string EnumTypeName
        {
            get
            {
                object o = this.ViewState["EnumTypeName"];
                if (o is string && o != null)
                    return (string)o;
                return string.Empty;
            }

            set
            {
                if (!(object.Equals(value, this.ViewState["EnumTypeName"])))
                {
                    this.ViewState["EnumTypeName"] = value;
                    this.enumType = null;
                }
            }
        }

        /// <summary>
        /// 初始化<see cref="DataControlField"/>对象。
        /// </summary>
        /// <param name="enableSorting">如果支持排序，则为<see langword="true"/>，否则为<see langword="false"/>。</param>
        /// <param name="control">拥有<see cref="DataControlField"/>的控件。</param>
        /// <returns>在所有情况下均为 false。 </returns>
        public override bool Initialize(bool enableSorting, Control control)
        {
            bool result = base.Initialize(enableSorting, control);

            if (string.IsNullOrEmpty(this.ViewState["EnumTypeName"] as string) == false)
            {
                this.enumType = Type.GetType(this.EnumTypeName);
                if (this.enumType == null)
                    throw new HttpException(string.Format("未能初始化EnumTypeName属性中指定的类型 {0}。", this.EnumTypeName));

                this.RefreshKeys();
            }

            return result;
        }

        private void RefreshKeys()
        {
            this.dataSource.Clear();

            if (this.enumType == null)
                throw new HttpException("未能初始化枚举类型。要初始化，请设置EnumTypeName属性，或者将枚举值绑定到列。");

            if (this.UseNameAsValue)
            {
                foreach (string name in this.enumType.GetEnumNames())
                {
                    object en = Enum.Parse(this.enumType, name);
                    this.dataSource.Add(new EnumItem(name, EnumItemDescriptionAttribute.GetDescription((Enum)en)));
                }
            }
            else
            {
                foreach (object val in this.enumType.GetEnumValues())
                {
                    this.dataSource.Add(new EnumItem(((int)val).ToString(), EnumItemDescriptionAttribute.GetDescription((Enum)val)));
                }
            }
        }

        /// <summary>
        /// 从单元格取出数值
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="cell"></param>
        /// <param name="rowState"></param>
        /// <param name="includeReadOnly"></param>
        public override void ExtractValuesFromCell(IOrderedDictionary dictionary, DataControlFieldCell cell, DataControlRowState rowState, bool includeReadOnly)
        {
            Control control = null;
            string dataField = this.DataField;
            object obj2 = null;
            if (cell.Controls.Count > 0)
            {
                control = cell.Controls[0];
                DropDownList box = control as DropDownList;
                if ((box != null) && (includeReadOnly || box.Enabled))
                {
                    obj2 = box.SelectedValue;
                }
            }
            if (obj2 != null)
            {
                if (dictionary.Contains(dataField))
                {
                    dictionary[dataField] = obj2;
                }
                else
                {
                    dictionary.Add(dataField, obj2);
                }
            }
        }

        /// <summary>
        /// 将字段的值绑定到<see cref="EnumDropDownField"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnDataBindField(object sender, EventArgs e)
        {
            Control control = (Control)sender;

            if (this.DesignMode == false)
            {
                Control namingContainer = control.NamingContainer;
                object dataValue = this.GetValue(namingContainer);
                bool encode = (this.SupportsHtmlEncode && this.HtmlEncode) && (control is TableCell);
                string valString;
                object enumValue;
                if (dataValue is Enum)
                {
                    enumValue = (Enum)dataValue;
                    if (this.enumType == null)
                    {
                        this.enumType = enumValue.GetType();
                        this.RefreshKeys();
                    }
                }
                else if (dataValue is int)
                {
                    enumValue = Enum.ToObject(enumType, (int)dataValue);
                }
                else if (dataValue is string)
                {
                    enumValue = Enum.Parse(enumType, (string)dataValue);
                }
                else
                    throw new HttpException(string.Format("EnumDropDownField不支持 {0} 类型的值。", dataValue.GetType().ToString()));

                if (this.UseNameAsValue)
                {
                    valString = enumType.GetEnumName(enumValue);
                }
                else
                {
                    valString = ((int)enumValue).ToString();
                }

                if (Enum.IsDefined(enumType, enumValue) == false)
                    throw new InvalidOperationException(string.Format("枚举值{0}未在{1}中定义", enumValue, enumType.ToString()));

                string description = EnumItemDescriptionAttribute.GetDescription((Enum)enumValue);

                if (control is TableCell)
                {
                    if (description.Length == 0)
                    {
                        description = "<??>";
                    }
                    ((TableCell)control).Text = description;
                }
                else
                {
                    if (!(control is DropDownList))
                    {
                        throw new HttpException(string.Format("{0} 不是EnumDropDownField类型。", new object[] { this.DataField }));
                    }

                    ((DropDownList)control).SelectedValue = valString;
                }
            }
            else
            {
                ((TableCell)control).Text = "枚举";
            }
        }

        /// <summary>
        /// 不支持
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public override bool ConvertEmptyStringToNull
        {
            get
            {
                if (!this.suppressPropertyThrows)
                {
                    throw new NotSupportedException(string.Format("EnumDropDownField 不支持 {0}", new object[] { "ConvertEmptyStringToNull" }));
                }
                return false;
            }

            set
            {
                if (!this.suppressPropertyThrows)
                {
                    throw new NotSupportedException(string.Format("EnumDropDownField 不支持 {0}", new object[] { "ConvertEmptyStringToNull" }));
                }
            }
        }

        /// <summary>
        /// 不支持
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool ApplyFormatInEditMode
        {
            get
            {
                if (!this.suppressPropertyThrows)
                {
                    throw new NotSupportedException(string.Format("EnumDropDownField 不支持 {0}", new object[] { "ApplyFormatInEditMode" }));
                }
                return false;
            }
            set
            {
                if (!this.suppressPropertyThrows)
                {
                    throw new NotSupportedException(string.Format("EnumDropDownField 不支持 {0}", new object[] { "ApplyFormatInEditMode" }));
                }
            }
        }

        /// <summary>
        /// 不支持
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string DataFormatString
        {
            get
            {
                if (!this.suppressPropertyThrows)
                {
                    throw new NotSupportedException(string.Format("EnumDropDownField 不支持 {0}", new object[] { "DataFormatString" }));
                }
                return string.Empty;
            }

            set
            {
                if (!this.suppressPropertyThrows)
                {
                    throw new NotSupportedException(string.Format("EnumDropDownField 不支持 {0}", new object[] { "DataFormatString" }));
                }
            }
        }

        /// <summary>
        /// 确定包含在<see cref="EnumDropDownField"/>中的控件是否支持回调。
        /// </summary>
        public override void ValidateSupportsCallback()
        {
        }

        /// <summary>
        /// 将指定的<see cref="TableCell"/>初始化为指定的行状态。
        /// </summary>
        /// <param name="cell">要初始化的<see cref="TableCell"/></param>
        /// <param name="rowState">行状态</param>
        protected override void InitializeDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            DropDownList child = null;
            Control readOnlyControl = null;
            if ((((rowState & DataControlRowState.Edit) != DataControlRowState.Normal) && !this.ReadOnly) || ((rowState & DataControlRowState.Insert) != DataControlRowState.Normal))
            {
                if (string.IsNullOrEmpty(this.ViewState["EnumTypeName"] as string))
                    throw new NotSupportedException("要使 EnumDropDownField 可用于编辑，必须设置 EnumTypeName属性。");

                DropDownList box = new DropDownList
                {
                    ToolTip = this.HeaderText
                };
                box.DataSource = this.dataSource;
                box.DataTextField = "Text";
                box.DataValueField = "Key";
                child = box;
                if ((this.DataField.Length != 0) && ((rowState & DataControlRowState.Edit) != DataControlRowState.Normal))
                {
                    readOnlyControl = box;
                }
            }
            else if (this.DataField.Length != 0)
            {
                readOnlyControl = cell;
            }

            if (child != null)
            {
                cell.Controls.Add(child);
            }
            if ((readOnlyControl != null) && this.Visible)
            {
                readOnlyControl.DataBinding += new EventHandler(this.OnDataBindField);
            }
        }

        /// <summary>
        /// 将当前<see cref="EnumDropDownField"/>控件的属性复制到指定的<see cref="EnumDropDownField"/>。
        /// </summary>
        /// <param name="newField">要被复制到的<see cref="DataControlField"/>。</param>
        protected override void CopyProperties(DataControlField newField)
        {
            EnumDropDownField other = (EnumDropDownField)newField;
            this.suppressPropertyThrows = other.suppressPropertyThrows = true;
            base.CopyProperties(newField);
            this.suppressPropertyThrows = other.suppressPropertyThrows = false;
            other.UseNameAsValue = this.UseNameAsValue;
            other.EnumTypeName = this.EnumTypeName;
        }

        /// <summary>
        /// 创建一个空的<see cref="DataControlField"/>。
        /// </summary>
        /// <returns></returns>
        protected override DataControlField CreateField()
        {
            return new EnumDropDownField();
        }

        /// <summary>
        /// 获取设计时的值
        /// </summary>
        /// <returns></returns>
        protected override object GetDesignTimeValue()
        {
            if (this.UseNameAsValue == false)
                return 0;
            else
                return "枚举值";
        }
    }
}
