using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

using MCS.Library.Configuration;

namespace MCS.Web.Library
{
    /// <summary>
    /// 
    /// </summary>
    public class NameWindowFeatureElementCollection : NamedConfigurationElementCollection<WindowFeatureElement>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public class WindowFeatureElement : NamedConfigurationElement
    {
        private WindowFeature feature;

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("width", DefaultValue = null)]
        private Nullable<int> Width
        {
            get { return (Nullable<int>)this["width"];}
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("height", DefaultValue = null)]
        private Nullable<int> Height
        {
            get { return (Nullable<int>)this["height"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("top", DefaultValue = null)]
        private Nullable<int> Top
        {
            get { return (Nullable<int>)this["top"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("left", DefaultValue = null)]
        private Nullable<int> Left
        {
            get { return (Nullable<int>)this["left"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("widthScript", DefaultValue = null)]
        private string WidthScript
        {
            get { return (string)this["widthScript"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("heightScript", DefaultValue = null)]
        private string HeightScript
        {
            get { return (string)this["heightScript"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("leftScript", DefaultValue = null)]
        private string LeftScript
        {
            get { return (string)this["leftScript"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("topScript", DefaultValue = null)]
        private string TopScript
        {
            get { return (string)this["topScript"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("center", DefaultValue = null)]
        private Nullable<bool> Center
        {
            get { return (Nullable<bool>)this["center"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("resizable", DefaultValue = null)]
        private Nullable<bool> Resizable
        {
            get { return (Nullable<bool>)this["resizable"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("showScrollBars", DefaultValue = null)]
        private Nullable<bool> ShowScrollBars
        {
            get { return (Nullable<bool>)this["showScrollBars"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("showStatusBar", DefaultValue = null)]
        private Nullable<bool> ShowStatusBar
        {
            get { return (Nullable<bool>)this["showStatusBar"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("showToolBar", DefaultValue = null)]
        private Nullable<bool> ShowToolBar
        {
            get { return (Nullable<bool>)this["showToolBar"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("showAddressBar", DefaultValue = null)]
        private Nullable<bool> ShowAddressBar
        {
            get { return (Nullable<bool>)this["showAddressBar"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("showMenuBar", DefaultValue = null)]
        private Nullable<bool> ShowMenuBar
        {
            get { return (Nullable<bool>)this["showMenuBar"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        public WindowFeature Feature
        {
            get
            {
                if (this.feature == null)
                {
                    this.feature = new WindowFeature();

                    this.feature.Width = this.Width;
                    this.feature.WidthScript = this.WidthScript;
                    this.feature.Height = this.Height;
                    this.feature.HeightScript = this.HeightScript;
                    this.feature.Left = this.Left;
                    this.feature.LeftScript = this.LeftScript;
                    this.feature.Top = this.Top;
                    this.feature.TopScript = this.TopScript;
                    this.feature.Center = this.Center;
                    this.feature.Resizable = this.Resizable;
                    this.feature.ShowAddressBar = this.ShowAddressBar;
                    this.feature.ShowMenuBar = this.ShowMenuBar;
                    this.feature.ShowScrollBars = this.ShowScrollBars;
                    this.feature.ShowStatusBar = this.ShowStatusBar;
                    this.feature.ShowToolBar = this.ShowToolBar;
                }

                return this.feature;
            }
        }        
    }
}
