using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Web.Library.Script;

namespace MCS.Web.Library
{
	/// <summary>
	/// 
	/// </summary>
	public interface IWindowFeature
	{
		/// <summary>
		/// 
		/// </summary>
		[ClientPropertyName("width")]
		Nullable<int> Width { get; }

		/// <summary>
		/// 
		/// </summary>
		[ClientPropertyName("height")]
		Nullable<int> Height { get; }

		/// <summary>
		/// 
		/// </summary>
		[ClientPropertyName("top")]
		Nullable<int> Top { get; }

		/// <summary>
		/// 
		/// </summary>
		[ClientPropertyName("left")]
		Nullable<int> Left { get; }

		/// <summary>
		/// 
		/// </summary>
		[ClientPropertyName("widthScript")]
		string WidthScript { get; }

		/// <summary>
		/// 
		/// </summary>
		[ClientPropertyName("heightScript")]
		string HeightScript { get; }

		/// <summary>
		/// 
		/// </summary>
		[ClientPropertyName("topScript")]
		string TopScript { get; }

		/// <summary>
		/// 
		/// </summary>
		[ClientPropertyName("leftScript")]
		string LeftScript { get; }

		/// <summary>
		/// 
		/// </summary>
		[ClientPropertyName("center")]
		Nullable<bool> Center { get; }

		/// <summary>
		/// 
		/// </summary>
		[ClientPropertyName("resizable")]
		Nullable<bool> Resizable { get; }

		/// <summary>
		/// 
		/// </summary>
		[ClientPropertyName("showScrollBars")]
		Nullable<bool> ShowScrollBars { get; }

		/// <summary>
		/// 
		/// </summary>
		[ClientPropertyName("showStatusBar")]
		Nullable<bool> ShowStatusBar { get; }

		/// <summary>
		/// 
		/// </summary>
		[ClientPropertyName("showToolBar")]
		Nullable<bool> ShowToolBar { get; }
		/// <summary>
		/// 
		/// </summary>
		[ClientPropertyName("showAddressBar")]
		Nullable<bool> ShowAddressBar { get; }

		/// <summary>
		/// 
		/// </summary>
		[ClientPropertyName("showMenuBar")]
		Nullable<bool> ShowMenuBar { get; }
		
		///// <summary>
		///// 
		///// </summary>
		///// <returns></returns>
		//string ToDialogFeatureClientString();

		///// <summary>
		///// 
		///// </summary>
		///// <returns></returns>
		//string ToWindowFeatureClientString();

		///// <summary>
		///// 
		///// </summary>
		///// <param name="addScriptTags"></param>
		///// <returns></returns>
		//string ToAdjustWindowScriptBlock(bool addScriptTags);
	}
}
