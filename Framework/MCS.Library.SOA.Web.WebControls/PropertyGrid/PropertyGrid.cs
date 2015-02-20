using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web;
using System.Web.UI.HtmlControls;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.Resources;
using MCS.Web.Library.Script;
using MCS.Web.Library;

[assembly: WebResource("MCS.Web.WebControls.PropertyGrid.PropertyGrid.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.PropertyGrid.PropertyGrid.css", "text/css", PerformSubstitution = true)]
[assembly: WebResource("MCS.Web.WebControls.PropertyGrid.categorySort.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.PropertyGrid.categorySort_hover.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.PropertyGrid.alphabetSort.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.PropertyGrid.alphabetSort_hover.gif", "image/gif")]

namespace MCS.Web.WebControls
{
	public enum PropertiesDisplayOrder
	{
		ByCategory = 1,
		ByAlphabet = 2,
	}

	[PersistChildren(false)]
	[ParseChildren(true)]
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(ClientMsgResources), 2)]
	[RequiredScript(typeof(ClientPropertyEditorControlBaseResources), 3)]
	[ClientCssResource("MCS.Web.WebControls.PropertyGrid.PropertyGrid.css")]
	[ClientScriptResource("MCS.Web.WebControls.PropertyGrid", "MCS.Web.WebControls.PropertyGrid.PropertyGrid.js")]
	public class PropertyGrid : PropertyEditorControlBase
	{
		public PropertyGrid()
			: base()
		{
		}

		[DefaultValue("")]
		[ClientPropertyName("caption")]
		[ScriptControlProperty]
		public string Caption
		{
			get
			{
				return GetPropertyValue("Caption", string.Empty);
			}
			set
			{
				SetPropertyValue("Caption", value);
			}
		}

		[DefaultValue(PropertiesDisplayOrder.ByCategory)]
		[ClientPropertyName("displayOrder")]
		[ScriptControlProperty]
		public PropertiesDisplayOrder DisplayOrder
		{
			get
			{
				return GetPropertyValue("DisplayOrder", PropertiesDisplayOrder.ByCategory);
			}
			set
			{
				SetPropertyValue("DisplayOrder", value);
			}
		}

		[Browsable(false)]
		private string CategorySortButtonImage
		{
			get
			{
				return this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "MCS.Web.WebControls.PropertyGrid.categorySort.gif");
			}
		}

		[Browsable(false)]
		private string CategorySortButtonHoverImage
		{
			get
			{
				return this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "MCS.Web.WebControls.PropertyGrid.categorySort_hover.gif");
			}
		}

		[Browsable(false)]
		private string AlphabetSortButtonImage
		{
			get
			{
				return this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "MCS.Web.WebControls.PropertyGrid.alphabetSort.gif");
			}
		}

		[Browsable(false)]
		private string AlphaSortButtonHoverImage
		{
			get
			{
				return this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "MCS.Web.WebControls.PropertyGrid.alphabetSort_hover.gif");
			}
		}

		private void PreloadAllImages()
		{
			PreloadImage(this.CategorySortButtonImage, this.CategorySortButtonImage);
			PreloadImage(this.CategorySortButtonHoverImage, this.CategorySortButtonHoverImage);
			PreloadImage(this.AlphabetSortButtonImage, this.AlphabetSortButtonImage);
			PreloadImage(this.AlphaSortButtonHoverImage, this.AlphaSortButtonHoverImage);
		}

		private void PreloadImage(string key, string imgSrc)
		{
			if (string.IsNullOrEmpty(imgSrc) == false)
				Page.ClientScript.RegisterClientScriptBlock(
					this.GetType(),
					key,
					string.Format("<img src=\"{0}\" style=\"display:none\"/>", HttpUtility.UrlPathEncode(imgSrc)));
		}

		protected override void LoadClientState(string clientState)
		{
			if (clientState.IsNotEmpty())
				this.Properties.CopyFrom(JSONSerializerExecute.Deserialize<PropertyValueCollection>(clientState));
		}

		/// <summary>
		/// 渲染到客户端之前的操作。在本控件中，预先加载图片
		/// </summary>
		/// <param name="e">事件参数</param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (this.DesignMode == false)
				this.PreloadAllImages();

            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "分类显示");
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "按照字母排序显示");
		}
	}
}
