using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web.UI;

namespace MCS.Web.Responsive.WebControls
{
	[Serializable]
	public class ClientGridColumnEditTemplate
	{
		public ClientGridColumnEditMode EditMode
		{
			get;
			set;
		}

		[DefaultValue(""), IDReferenceProperty(), TypeConverter(typeof(DialogStartUpControlConverter))]
		public string TemplateControlID
		{
			get;
			set;
		}

		public string TemplateControlClientID
		{
			get;
			set;
		}

		/// <summary>
		/// 模板控件设置，用Json表示
		/// </summary>
		public string TemplateControlSettings
		{
			get;
			set;
		}

		[DefaultValue("#")]
		public string TextFieldOfA
		{
			get;
			set;
		}
		[DefaultValue("#")]
		public string HrefFieldOfA
		{
			get;
			set;
		}

		[DefaultValue("Link")]
		public string DefaultTextOfA
		{
			get;
			set;
		}
		[DefaultValue("#")]
		public string DefaultHrefOfA
		{
			get;
			set;
		}

		[DefaultValue("_blank")]
		public string TargetOfA
		{
			get;
			set;
		}

		[DefaultValue("")]
		public string ControlClientPropName
		{
			get;
			set;
		}

		[DefaultValue("")]
		public string ControlClientSetPropName
		{
			get;
			set;
		}
	}
}
