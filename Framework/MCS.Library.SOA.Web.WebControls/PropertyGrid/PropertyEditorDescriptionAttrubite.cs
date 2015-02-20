using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 用于描述属性编辑器的属性
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class PropertyEditorDescriptionAttribute : Attribute
	{
		public PropertyEditorDescriptionAttribute()
		{
		}

		public PropertyEditorDescriptionAttribute(string editorKey, string componentType)
		{
			editorKey.CheckStringIsNullOrEmpty("editorKey");
			componentType.CheckStringIsNullOrEmpty("componentType");

			this.EditorKey = editorKey;
			this.ComponentType = componentType;
		}

		/// <summary>
		/// Editor的Key
		/// </summary>
		public string EditorKey { get; set; }

		/// <summary>
		/// 客户端Component的类型
		/// </summary>
		public string ComponentType { get; set; }
	}
}
