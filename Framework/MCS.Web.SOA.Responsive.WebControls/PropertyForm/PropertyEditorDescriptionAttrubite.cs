using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Web.Responsive.WebControls
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
			if (string.IsNullOrEmpty(editorKey))
				throw new ArgumentNullException("editorKey");

			if (string.IsNullOrEmpty(componentType))
				throw new ArgumentNullException("componentType");

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
