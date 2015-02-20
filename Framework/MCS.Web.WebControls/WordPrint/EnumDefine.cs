using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 
	/// </summary>
	public enum OperationType
	{
		/// <summary>
		/// create new
		/// </summary>
		Add = 0,
		/// <summary>
		/// modify
		/// </summary>
		Edit = 1,
		/// <summary>
		/// readonly
		/// </summary>
		View = 2
	}

	/// <summary>
	/// 按钮类型
	/// </summary>
	/// <remarks>
	/// 按钮类型
	/// </remarks>
	public enum ButtonType
	{
		/// <summary>
		/// HtmlInput  Type=button
		/// </summary>
		/// <remarks>
		/// HtmlInput  Type=button
		/// </remarks>
		InputButton = 0,
		/// <summary>
		/// Image
		/// </summary>
		/// <remarks>
		/// Image
		/// </remarks>
		ImageButton = 1,
		/// <summary>
		/// Link
		/// </summary>
		/// <remarks>
		/// Link
		/// </remarks>
		LinkButton = 2
	}
}
