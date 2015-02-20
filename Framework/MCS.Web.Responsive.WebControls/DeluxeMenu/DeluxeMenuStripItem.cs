using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Runtime;
using SCM = System.ComponentModel;
using System.Web;
using System.Globalization;

namespace MCS.Web.Responsive.WebControls
{
    public class DeluxeMenuStripItem : IStateManager, IParserAccessor, IAttributeAccessor
    {
        // Fields
        private AttributeCollection _attributes;
        private bool enabled;
        private bool enabledisdirty;
        private bool marked;
        private string text;
        private bool textisdirty;
        private string value;
        private bool valueisdirty;

        // Methods
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public DeluxeMenuStripItem()
            : this(null, null)
        {
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public DeluxeMenuStripItem(string text)
            : this(text, null)
        {
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public DeluxeMenuStripItem(string text, string value)
            : this(text, value, true)
        {
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public DeluxeMenuStripItem(string text, string value, bool enabled)
        {
            this.text = text;
            this.value = value;
            this.enabled = enabled;
        }

        public override bool Equals(object o)
        {
            DeluxeMenuStripItem item = o as DeluxeMenuStripItem;
            if (item == null)
            {
                return false;
            }
            return (this.Value.Equals(item.Value) && this.Text.Equals(item.Text));
        }

        public static DeluxeMenuStripItem FromString(string s)
        {
            return new DeluxeMenuStripItem(s);
        }

        public override int GetHashCode()
        {
            return CombineHashCodes(this.Value.GetHashCode(), this.Text.GetHashCode());
        }

        private static int CombineHashCodes(int h1, int h2)
        {
            return (((h1 << 5) + h1) ^ h2);
        }

        internal void LoadViewState(object state)
        {
            if (state != null)
            {
                if (state is Triplet)
                {
                    Triplet triplet = (Triplet)state;
                    if (triplet.First != null)
                    {
                        this.Text = (string)triplet.First;
                    }
                    if (triplet.Second != null)
                    {
                        this.Value = (string)triplet.Second;
                    }
                    if (triplet.Third != null)
                    {
                        try
                        {
                            this.Enabled = (bool)triplet.Third;
                        }
                        catch
                        {
                        }
                    }
                }
                else if (state is Pair)
                {
                    Pair pair = (Pair)state;
                    if (pair.First != null)
                    {
                        this.Text = (string)pair.First;
                    }
                    this.Value = (string)pair.Second;
                }
                else
                {
                    this.Text = (string)state;
                }
            }
        }

        internal void RenderAttributes(HtmlTextWriter writer)
        {
            if (this._attributes != null)
            {
                this._attributes.AddAttributes(writer);
            }
        }

        private void ResetText()
        {
            this.Text = null;
        }

        private void ResetValue()
        {
            this.Value = null;
        }

        internal object SaveViewState()
        {
            string x = null;
            string y = null;
            if (this.textisdirty)
            {
                x = this.Text;
            }
            if (this.valueisdirty)
            {
                y = this.Value;
            }
            if (this.enabledisdirty)
            {
                return new Triplet(x, y, this.Enabled);
            }
            if (this.valueisdirty)
            {
                return new Pair(x, y);
            }
            if (this.textisdirty)
            {
                return x;
            }
            return null;
        }

        private bool ShouldSerializeText()
        {
            return ((this.text != null) && (this.text.Length != 0));
        }

        private bool ShouldSerializeValue()
        {
            return ((this.value != null) && (this.value.Length != 0));
        }

        string IAttributeAccessor.GetAttribute(string name)
        {
            return this.Attributes[name];
        }

        void IAttributeAccessor.SetAttribute(string name, string value)
        {
            this.Attributes[name] = value;
        }

        void IParserAccessor.AddParsedSubObject(object obj)
        {
            if (obj is LiteralControl)
            {
                this.Text = ((LiteralControl)obj).Text;
            }
            else
            {
                if (obj is DataBoundLiteralControl)
                {
                    throw new HttpException(string.Format("{0} 不能数据绑定", new object[] { "DeluxeMenuItem" }));
                }
                throw new HttpException(string.Format("{0}不能 包含{1}类型的子对象", new object[] { "DeluxeMenuItem", obj.GetType().Name.ToString(CultureInfo.InvariantCulture) }));
            }
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        void IStateManager.LoadViewState(object state)
        {
            this.LoadViewState(state);
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        object IStateManager.SaveViewState()
        {
            return this.SaveViewState();
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        void IStateManager.TrackViewState()
        {
            this.TrackViewState();
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public override string ToString()
        {
            return this.Text;
        }

        internal void TrackViewState()
        {
            this.marked = true;
        }

        // Properties
        [SCM.DesignerSerializationVisibility(SCM.DesignerSerializationVisibility.Hidden), SCM.Browsable(false)]
        public System.Web.UI.AttributeCollection Attributes
        {
            get
            {
                if (this._attributes == null)
                {
                    this._attributes = new System.Web.UI.AttributeCollection(new StateBag(true));
                }
                return this._attributes;
            }
        }

        internal bool Dirty
        {
            get
            {
                return !this.textisdirty && !this.valueisdirty && !this.enabledisdirty;
            }
            set
            {
                this.textisdirty = value;
                this.valueisdirty = value;
                this.enabledisdirty = value;
            }
        }

        [SCM.DefaultValue(true)]
        public bool Enabled
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this.enabled;
            }
            set
            {
                this.enabled = value;
                if (((IStateManager)this).IsTrackingViewState)
                {
                    this.enabledisdirty = true;
                }
            }
        }

        internal bool HasAttributes
        {
            get
            {
                return ((this._attributes != null) && (this._attributes.Count > 0));
            }
        }

        bool IStateManager.IsTrackingViewState
        {
            get
            {
                return this.marked;
            }
        }

        [SCM.Localizable(true), SCM.DefaultValue(""), PersistenceMode(PersistenceMode.EncodedInnerDefaultProperty)]
        public string Text
        {
            get
            {
                if (this.text != null)
                {
                    return this.text;
                }
                if (this.value != null)
                {
                    return this.value;
                }
                return string.Empty;
            }
            set
            {
                this.text = value;
                if (((IStateManager)this).IsTrackingViewState)
                {
                    this.textisdirty = true;
                }
            }
        }

        [SCM.Localizable(true), SCM.DefaultValue("")]
        public string Value
        {
            get
            {
                if (this.value != null)
                {
                    return this.value;
                }
                if (this.text != null)
                {
                    return this.text;
                }
                return string.Empty;
            }
            set
            {
                this.value = value;
                if (((IStateManager)this).IsTrackingViewState)
                {
                    this.valueisdirty = true;
                }
            }
        }

        public void Render(HtmlTextWriter writer)
        {
            if (this.Text != "-")
            {
                writer.BeginRender();
                writer.AddAttribute("role", "menuitem");
                if (this.Enabled == false)
                    writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "");
                if (this.HasAttributes)
                {
                    this.Attributes.AddAttributes(writer);
                }

                if (this.Value != null)
                    writer.AddAttribute("data-value", this.Value);

                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write(this.Text); // 注意，不会自动转义的
                writer.RenderEndTag();
                writer.EndRender();
            }
        }
    }
}
