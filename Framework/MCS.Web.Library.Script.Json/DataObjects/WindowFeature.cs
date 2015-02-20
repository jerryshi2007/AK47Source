using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Web.Library
{
	/// <summary>
	/// 
	/// </summary>
	public enum QuatationMarkType
	{
		/// <summary>
		/// 单引号
		/// </summary>
		Single,

		/// <summary>
		/// 双引号
		/// </summary>
		Double
	}

	/// <summary>
	/// 该类定义了浏览器窗口的特性
	/// </summary>
	public class WindowFeature : IWindowFeature
	{
		private Nullable<int> width = null;
		private Nullable<int> height = null;
		private Nullable<int> top = null;
		private Nullable<int> left = null;
		private string widthScript;
		private string heightScript;
		private string leftScript;
		private string topScript;
		private Nullable<bool> center;
		private Nullable<bool> resizable = null;
		private Nullable<bool> showScrollBars = null;
		private Nullable<bool> showStatusBar = null;
		private Nullable<bool> showToolBar = null;
		private Nullable<bool> showAddressBar = null;
		private Nullable<bool> showMenuBar = null;

		/// <summary>
		/// 
		/// </summary>
		public Nullable<int> Width
		{
			get { return width; }
			set { width = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public Nullable<int> Height
		{
			get { return height; }
			set { height = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public Nullable<int> Top
		{
			get { return top; }
			set { top = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public Nullable<int> Left
		{
			get { return left; }
			set { left = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string WidthScript
		{
			get { return widthScript; }
			set { widthScript = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string HeightScript
		{
			get { return heightScript; }
			set { heightScript = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string LeftScript
		{
			get { return leftScript; }
			set { leftScript = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string TopScript
		{
			get { return topScript; }
			set { topScript = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public Nullable<bool> Center
		{
			get { return center; }
			set { center = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public Nullable<bool> Resizable
		{
			get { return resizable; }
			set { resizable = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public Nullable<bool> ShowScrollBars
		{
			get { return showScrollBars; }
			set { showScrollBars = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public Nullable<bool> ShowStatusBar
		{
			get { return showStatusBar; }
			set { showStatusBar = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public Nullable<bool> ShowToolBar
		{
			get { return showToolBar; }
			set { showToolBar = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public Nullable<bool> ShowAddressBar
		{
			get { return showAddressBar; }
			set { showAddressBar = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public Nullable<bool> ShowMenuBar
		{
			get { return showMenuBar; }
			set { showMenuBar = value; }
		}

		///// <summary>
		///// 
		///// </summary>
		///// <returns></returns>
		//public string ToDialogFeatureClientString()
		//{
		//    return WindowFeatureHelper.GetDialogFeatureClientString(this);
		//}

		///// <summary>
		///// 
		///// </summary>
		///// <returns></returns>
		//public string ToWindowFeatureClientString()
		//{
		//    return WindowFeatureHelper.GetWindowFeatureClientString(this);
		//}

		///// <summary>
		///// 
		///// </summary>
		///// <param name="addScriptTags"></param>
		///// <returns></returns>
		//public string ToAdjustWindowScriptBlock(bool addScriptTags)
		//{
		//    return WindowFeatureHelper.GetAdjustWindowScriptBlock(this, addScriptTags);
		//}
	}
}
