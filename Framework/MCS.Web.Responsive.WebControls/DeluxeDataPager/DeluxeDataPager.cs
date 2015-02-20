using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web;
using System.Security.Permissions;
using System.ComponentModel;
using System.Web.UI;
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.Drawing;

namespace MCS.Web.Responsive.WebControls
{
    [PersistChildren(false), Themeable(true), SupportsEventValidation, Designer("System.Web.UI.Design.WebControls.DataPagerDesigner, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"), ParseChildren(true), ToolboxItemFilter("System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", ToolboxItemFilterType.Require), ToolboxBitmap(typeof(DataPager), "DataPager.ico"), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class DeluxeDataPager : DataPager
    {
        protected override System.Web.UI.HtmlTextWriterTag TagKey
        {
            get
            {
                return System.Web.UI.HtmlTextWriterTag.Ul;
            }
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            if (string.IsNullOrEmpty(this.Attributes["class"]))
                this.Attributes["class"] = "pagination";
            base.AddAttributesToRender(writer);
        }
    }

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class DeluxeNextPreviousPagerField : DataPagerField
    {
        private int _maximumRows;
        private int _startRowIndex;
        private int _totalRowCount;

        #region 属性

        private bool EnableNextPage
        {
            get
            {
                return ((this._startRowIndex + this._maximumRows) < this._totalRowCount);
            }
        }

        private bool EnablePreviousPage
        {
            get
            {
                return (this._startRowIndex > 0);
            }
        }

        [Category("Behavior"), Description("NextPreviousPagerField_ShowFirstPageButton"), DefaultValue(false)]
        public bool ShowFirstPageButton
        {
            get
            {
                object obj2 = base.ViewState["ShowFirstPageButton"];
                return ((obj2 != null) && ((bool)obj2));
            }
            set
            {
                if (value != this.ShowFirstPageButton)
                {
                    base.ViewState["ShowFirstPageButton"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [DefaultValue(false), Category("Behavior"), Description("NextPreviousPagerField_ShowLastPageButton")]
        public bool ShowLastPageButton
        {
            get
            {
                object obj2 = base.ViewState["ShowLastPageButton"];
                return ((obj2 != null) && ((bool)obj2));
            }
            set
            {
                if (value != this.ShowLastPageButton)
                {
                    base.ViewState["ShowLastPageButton"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [DefaultValue(true), Description("NextPreviousPagerField_ShowNextPageButton"), Category("Behavior")]
        public bool ShowNextPageButton
        {
            get
            {
                object obj2 = base.ViewState["ShowNextPageButton"];
                if (obj2 != null)
                {
                    return (bool)obj2;
                }
                return true;
            }
            set
            {
                if (value != this.ShowNextPageButton)
                {
                    base.ViewState["ShowNextPageButton"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("NextPreviousPagerField_ShowPreviousPageButton")]
        public bool ShowPreviousPageButton
        {
            get
            {
                object obj2 = base.ViewState["ShowPreviousPageButton"];
                if (obj2 != null)
                {
                    return (bool)obj2;
                }
                return true;
            }
            set
            {
                if (value != this.ShowPreviousPageButton)
                {
                    base.ViewState["ShowPreviousPageButton"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        #endregion


        protected override void CopyProperties(DataPagerField newField)
        {
            ((NextPreviousPagerField)newField).ShowFirstPageButton = this.ShowFirstPageButton;
            ((NextPreviousPagerField)newField).ShowLastPageButton = this.ShowLastPageButton;
            ((NextPreviousPagerField)newField).ShowNextPageButton = this.ShowNextPageButton;
            ((NextPreviousPagerField)newField).ShowPreviousPageButton = this.ShowPreviousPageButton;
            base.CopyProperties(newField);
        }

        private Control CreateControl(string commandName, string iconClass, int fieldIndex, bool enabled)
        {
            HtmlGenericControl li = new HtmlGenericControl("li");

            IButtonControl control;
            if (!enabled)
            {
                Label label = new Label
                {
                    Text = GetIconHtml(iconClass)
                };

                li.Controls.Add(label);
            }
            else
            {
                control = new LinkButton()
                {
                    Enabled = enabled,
                    Text = GetIconHtml(iconClass)
                };

                control.CausesValidation = false;
                control.CommandName = commandName;
                control.CommandArgument = fieldIndex.ToString(CultureInfo.InvariantCulture);

                li.Controls.Add((Control)control);
            }

            return li;
        }

        [System.Runtime.TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        private string GetIconHtml(string iconClass)
        {
            return "<i class=\"" + iconClass + "\"></i>";
        }

        public override void CreateDataPagers(DataPagerFieldItem container, int startRowIndex, int maximumRows, int totalRowCount, int fieldIndex)
        {
            this._startRowIndex = startRowIndex;
            this._maximumRows = maximumRows;
            this._totalRowCount = totalRowCount;
            if (string.IsNullOrEmpty(base.DataPager.QueryStringField))
            {
                this.CreateDataPagersForCommand(container, fieldIndex);
            }
            else
            {
                this.CreateDataPagersForQueryString(container, fieldIndex);
            }
        }

        private void CreateDataPagersForCommand(DataPagerFieldItem container, int fieldIndex)
        {
            if (this.ShowFirstPageButton)
            {
                container.Controls.Add(this.CreateControl("First", "icon-double-angle-left", fieldIndex, this.EnablePreviousPage));
            }
            if (this.ShowPreviousPageButton)
            {
                container.Controls.Add(this.CreateControl("Prev", "icon-angle-left", fieldIndex, this.EnablePreviousPage));
            }
            if (this.ShowNextPageButton)
            {
                container.Controls.Add(this.CreateControl("Next", "icon-angle-right", fieldIndex, this.EnableNextPage));
            }
            if (this.ShowLastPageButton)
            {
                container.Controls.Add(this.CreateControl("Last", "icon-double-angle-right", fieldIndex, this.EnableNextPage));
            }
        }

        private void CreateDataPagersForQueryString(DataPagerFieldItem container, int fieldIndex)
        {
            bool flag = false;
            if (!base.QueryStringHandled)
            {
                int num;
                base.QueryStringHandled = true;
                if (int.TryParse(base.QueryStringValue, out num))
                {
                    num--;
                    int num1 = this._startRowIndex / this._maximumRows;
                    int num2 = (this._totalRowCount - 1) / this._maximumRows;
                    if ((num >= 0) && (num <= num2))
                    {
                        this._startRowIndex = num * this._maximumRows;
                        flag = true;
                    }
                }
            }
            if (this.ShowFirstPageButton)
            {
                container.Controls.Add(this.CreateLink("icon-double-angle-left", 0, this.EnablePreviousPage));
            }
            if (this.ShowPreviousPageButton)
            {
                int pageIndex = (this._startRowIndex / this._maximumRows) - 1;
                container.Controls.Add(this.CreateLink("icon-angle-left", pageIndex, this.EnablePreviousPage));
            }
            if (this.ShowNextPageButton)
            {
                int num4 = (this._startRowIndex + this._maximumRows) / this._maximumRows;
                container.Controls.Add(this.CreateLink("icon-angle-right", num4, this.EnableNextPage));
            }
            if (this.ShowLastPageButton)
            {
                int num5 = (this._totalRowCount / this._maximumRows) - (((this._totalRowCount % this._maximumRows) == 0) ? 1 : 0);
                container.Controls.Add(this.CreateLink("icon-double-angle-right", num5, this.EnableNextPage));
            }
            if (flag)
            {
                base.DataPager.SetPageProperties(this._startRowIndex, this._maximumRows, true);
            }
        }

        protected override DataPagerField CreateField()
        {
            return new DeluxeNextPreviousPagerField();
        }

        private HtmlGenericControl CreateLink(string iconClass, int pageIndex, bool enabled)
        {
            int pageNumber = pageIndex + 1;

            HtmlGenericControl li = new HtmlGenericControl("li");
            if (enabled == false)
                li.Attributes["class"] = "disabled";

            HyperLink link = new HyperLink
            {
                NavigateUrl = base.GetQueryStringNavigateUrl(pageNumber),
                Enabled = enabled,
                Text = GetIconHtml(iconClass)
            };

            li.Controls.Add(link);

            return li;
        }

        public override bool Equals(object o)
        {
            DeluxeNextPreviousPagerField field = o as DeluxeNextPreviousPagerField;
            return (field != null) && (field.ShowFirstPageButton == this.ShowFirstPageButton) && (field.ShowLastPageButton == this.ShowLastPageButton) && (field.ShowNextPageButton == this.ShowNextPageButton) && (field.ShowPreviousPageButton == this.ShowPreviousPageButton);
        }

        public override int GetHashCode()
        {
            return this.ShowFirstPageButton.GetHashCode() | this.ShowLastPageButton.GetHashCode() | this.ShowNextPageButton.GetHashCode() | this.ShowPreviousPageButton.GetHashCode();
        }

        public override void HandleEvent(CommandEventArgs e)
        {
            if (string.IsNullOrEmpty(base.DataPager.QueryStringField))
            {
                if (string.Equals(e.CommandName, "Prev"))
                {
                    int startRowIndex = this._startRowIndex - base.DataPager.PageSize;
                    if (startRowIndex < 0)
                    {
                        startRowIndex = 0;
                    }
                    base.DataPager.SetPageProperties(startRowIndex, base.DataPager.PageSize, true);
                }
                else if (string.Equals(e.CommandName, "Next"))
                {
                    int num2 = this._startRowIndex + base.DataPager.PageSize;
                    if (num2 > this._totalRowCount)
                    {
                        num2 = this._totalRowCount - base.DataPager.PageSize;
                    }
                    base.DataPager.SetPageProperties(num2, base.DataPager.PageSize, true);
                }
                else if (string.Equals(e.CommandName, "First"))
                {
                    base.DataPager.SetPageProperties(0, base.DataPager.PageSize, true);
                }
                else if (string.Equals(e.CommandName, "Last"))
                {
                    int num3;
                    int num4 = this._totalRowCount % base.DataPager.PageSize;
                    if (num4 == 0)
                    {
                        num3 = this._totalRowCount - base.DataPager.PageSize;
                    }
                    else
                    {
                        num3 = this._totalRowCount - num4;
                    }
                    base.DataPager.SetPageProperties(num3, base.DataPager.PageSize, true);
                }
            }
        }
    }

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class DeluxeNumericPagerField : DataPagerField
    {
        // Fields
        private int _maximumRows;
        private int _startRowIndex;
        private int _totalRowCount;


        protected override void CopyProperties(DataPagerField newField)
        {
            ((NumericPagerField)newField).ButtonCount = this.ButtonCount;
            base.CopyProperties(newField);
        }

        public override void CreateDataPagers(DataPagerFieldItem container, int startRowIndex, int maximumRows, int totalRowCount, int fieldIndex)
        {
            this._startRowIndex = startRowIndex;
            this._maximumRows = maximumRows;
            this._totalRowCount = totalRowCount;
            if (string.IsNullOrEmpty(base.DataPager.QueryStringField))
            {
                this.CreateDataPagersForCommand(container, fieldIndex);
            }
            else
            {
                this.CreateDataPagersForQueryString(container, fieldIndex);
            }
        }

        private void CreateDataPagersForCommand(DataPagerFieldItem container, int fieldIndex)
        {
            int aPageSize = this._startRowIndex / this._maximumRows;
            int num2 = (this._startRowIndex / (this.ButtonCount * this._maximumRows)) * this.ButtonCount;
            int num3 = (num2 + this.ButtonCount) - 1;
            int num4 = ((num3 + 1) * this._maximumRows) - 1;
            if (num2 != 0)
            {
                container.Controls.Add(this.CreateNextPrevButton("icon-angle-left", "Prev", fieldIndex.ToString(CultureInfo.InvariantCulture)));
            }
            for (int i = 0; (i < this.ButtonCount) && (this._totalRowCount > ((i + num2) * this._maximumRows)); i++)
            {
                if (i + num2 == aPageSize)
                {
                    container.Controls.Add(CreateCurrentPageLabel(i + num2));
                }
                else
                {
                    int num7 = (i + num2) + 1;
                    int num8 = i + num2;
                    container.Controls.Add(this.CreateNumericButton(num7.ToString(CultureInfo.InvariantCulture), fieldIndex.ToString(CultureInfo.InvariantCulture), num8.ToString(CultureInfo.InvariantCulture)));
                }

            }
            if (num4 < (this._totalRowCount - 1))
            {

                container.Controls.Add(this.CreateNextPrevButton("icon-angle-right", "Next", fieldIndex.ToString(CultureInfo.InvariantCulture)));

            }
        }

        private void CreateDataPagersForQueryString(DataPagerFieldItem container, int fieldIndex)
        {
            int pageIndex = this._startRowIndex / this._maximumRows;
            bool flag = false;
            if (!base.QueryStringHandled)
            {
                int requestPage;
                base.QueryStringHandled = true;
                if (int.TryParse(base.QueryStringValue, out requestPage))
                {
                    requestPage--;
                    int pageCount = (this._totalRowCount - 1) / this._maximumRows;
                    if ((requestPage >= 0) && (requestPage <= pageCount))
                    {
                        pageIndex = requestPage;
                        this._startRowIndex = pageIndex * this._maximumRows;
                        flag = true;
                    }
                }
            }
            int firstPageIndex = (this._startRowIndex / (this.ButtonCount * this._maximumRows)) * this.ButtonCount;
            int lastPage = (firstPageIndex + this.ButtonCount) - 1;
            int lastVisiblePagerRow = ((lastPage + 1) * this._maximumRows) - 1;
            if (firstPageIndex != 0)
            {
                container.Controls.Add(this.CreateNextPrevLink("icon-angle-left", firstPageIndex - 1));

            }
            for (int i = 0; (i < this.ButtonCount) && (this._totalRowCount > ((i + firstPageIndex) * this._maximumRows)); i++)
            {
                if ((i + firstPageIndex) == pageIndex)
                {
                    container.Controls.Add(CreateCurrentPageLabel(i + firstPageIndex));
                }
                else
                {
                    container.Controls.Add(this.CreateNumericLink(i + firstPageIndex));
                }

            }
            if (lastVisiblePagerRow < (this._totalRowCount - 1))
            {
                container.Controls.Add(this.CreateNextPrevLink("icon-angle-right", firstPageIndex + this.ButtonCount));
            }
            if (flag)
            {
                base.DataPager.SetPageProperties(this._startRowIndex, this._maximumRows, true);
            }
        }

        [System.Runtime.TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        private string GetIconHtml(string iconClass)
        {
            return "<i class=\"" + iconClass + "\"></i>";
        }

        protected override DataPagerField CreateField()
        {
            return new DeluxeNumericPagerField();
        }

        private Control CreateNextPrevButton(string iconClass, string commandName, string commandArgument)
        {
            HtmlGenericControl li = new HtmlGenericControl("li");

            IButtonControl control = new LinkButton();

            control.Text = GetIconHtml(iconClass);
            control.CausesValidation = false;
            control.CommandName = commandName;
            control.CommandArgument = commandArgument;

            li.Controls.Add((Control)control);

            return li;
        }

        private HtmlGenericControl CreateNextPrevLink(string iconClass, int pageIndex)
        {
            HtmlGenericControl li = new HtmlGenericControl("li");
            int pageNumber = pageIndex + 1;
            HyperLink link = new HyperLink();

            link.Text = GetIconHtml(iconClass);
            link.NavigateUrl = base.GetQueryStringNavigateUrl(pageNumber);

            li.Controls.Add(link);

            return li;
        }

        private Control CreateNumericButton(string iconClass, string commandArgument, string commandName)
        {
            HtmlGenericControl li = new HtmlGenericControl("li");

            IButtonControl control = new LinkButton();

            control.Text = GetIconHtml(iconClass);
            control.CausesValidation = false;
            control.CommandName = commandName;
            control.CommandArgument = commandArgument;

            li.Controls.Add((Control)control);

            return li;
        }

        private HtmlGenericControl CreateNumericLink(int pageIndex)
        {
            HtmlGenericControl li = new HtmlGenericControl("li");

            int pageNumber = pageIndex + 1;
            HyperLink link = new HyperLink
            {
                Text = pageNumber.ToString(CultureInfo.InvariantCulture),
                NavigateUrl = base.GetQueryStringNavigateUrl(pageNumber)
            };

            li.Controls.Add(link);

            return li;
        }

        private HtmlGenericControl CreateCurrentPageLabel(int pageIndex)
        {
            HtmlGenericControl li = new HtmlGenericControl("li");
            li.Attributes["class"] = "active";

            int pageNumber = pageIndex + 1;
            Label label = new Label
            {
                Text = pageNumber.ToString(CultureInfo.InvariantCulture),
            };

            li.Controls.Add(label);

            return li;
        }

        public override bool Equals(object o)
        {
            NumericPagerField field = o as NumericPagerField;
            return (field != null) && object.Equals(field.ButtonCount, this.ButtonCount);
        }

        public override int GetHashCode()
        {
            return this.ButtonCount.GetHashCode();
        }

        public override void HandleEvent(CommandEventArgs e)
        {
            if (string.IsNullOrEmpty(base.DataPager.QueryStringField))
            {
                int startRowIndex = -1;
                int num1 = this._startRowIndex / base.DataPager.PageSize;
                int num2 = (this._startRowIndex / (this.ButtonCount * base.DataPager.PageSize)) * this.ButtonCount;
                int num3 = (num2 + this.ButtonCount) - 1;
                int num4 = ((num3 + 1) * base.DataPager.PageSize) - 1;
                if (string.Equals(e.CommandName, "Prev"))
                {
                    startRowIndex = (num2 - 1) * base.DataPager.PageSize;
                    if (startRowIndex < 0)
                    {
                        startRowIndex = 0;
                    }
                }
                else if (string.Equals(e.CommandName, "Next"))
                {
                    startRowIndex = num4 + 1;
                    if (startRowIndex > this._totalRowCount)
                    {
                        startRowIndex = this._totalRowCount - base.DataPager.PageSize;
                    }
                }
                else
                {
                    startRowIndex = Convert.ToInt32(e.CommandName, CultureInfo.InvariantCulture) * base.DataPager.PageSize;
                }
                if (startRowIndex != -1)
                {
                    base.DataPager.SetPageProperties(startRowIndex, base.DataPager.PageSize, true);
                }
            }
        }

        // Properties
        [Description("NumericPagerField_ButtonCount"), DefaultValue(5), Category("Appearance")]
        public int ButtonCount
        {
            get
            {
                object obj2 = base.ViewState["ButtonCount"];
                if (obj2 != null)
                {
                    return (int)obj2;
                }
                return 5;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (value != this.ButtonCount)
                {
                    base.ViewState["ButtonCount"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        //[CssClassProperty, Category("Appearance"), Description("NumericPagerField_CurrentPageLabelCssClass"), DefaultValue("")]
        //public string CurrentPageLabelCssClass
        //{
        //    get
        //    {
        //        object obj2 = base.ViewState["CurrentPageLabelCssClass"];
        //        if (obj2 != null)
        //        {
        //            return (string)obj2;
        //        }
        //        return string.Empty;
        //    }
        //    set
        //    {
        //        if (value != this.CurrentPageLabelCssClass)
        //        {
        //            base.ViewState["CurrentPageLabelCssClass"] = value;
        //            this.OnFieldChanged();
        //        }
        //    }
        //}

        //[CssClassProperty, Category("Appearance"), Description("NumericPagerField_CurrentPageLabelCssClass"), DefaultValue("")]
        //public string CurrentPageLabelCssClass
        //{
        //    get
        //    {
        //        object obj2 = base.ViewState["CurrentPageLabelCssClass"];
        //        if (obj2 != null)
        //        {
        //            return (string)obj2;
        //        }
        //        return string.Empty;
        //    }
        //    set
        //    {
        //        if (value != this.CurrentPageLabelCssClass)
        //        {
        //            base.ViewState["CurrentPageLabelCssClass"] = value;
        //            this.OnFieldChanged();
        //        }
        //    }
        //}

        //[DefaultValue(""), Category("Appearance"), CssClassProperty, Description("NumericPagerField_NextPreviousButtonCssClass")]
        //public string NextPreviousButtonCssClass
        //{
        //    get
        //    {
        //        object obj2 = base.ViewState["NextPreviousButtonCssClass"];
        //        if (obj2 != null)
        //        {
        //            return (string)obj2;
        //        }
        //        return string.Empty;
        //    }
        //    set
        //    {
        //        if (value != this.NextPreviousButtonCssClass)
        //        {
        //            base.ViewState["NextPreviousButtonCssClass"] = value;
        //            this.OnFieldChanged();
        //        }
        //    }
        //}
    }
}
