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
	public static class WindowFeatureHelper
	{
		private const int WindowFeatureDefaultWidth = 800;
		private const int WindowFeatureDefaultHeigth = 600;

		/// <summary>
		/// 获取调整窗口大小和位置脚本
		/// </summary>
		/// <param name="windowFeature"></param>
		/// <param name="addScriptTags"></param>
		/// <returns></returns>
		public static string ToAdjustWindowScriptBlock(this IWindowFeature windowFeature, bool addScriptTags)
		{
			string requireScript = WebUtility.GetRequiredScriptBlock(typeof(DeluxeScript));

			string clintObject = WindowFeatureHelper.GetClientObject(windowFeature);
			//string callScript = string.Format("$HGRootNS.WindowFeatureFunction.adjustWindow({0});", clintObject);
			string callScript = string.Format("$HGRootNS.WindowFeatureFunction.registerAdjustWindow({0});", clintObject);

			string script = string.Format("{0}\n{1}", requireScript, callScript);
			if (addScriptTags)
				script = DeluxeClientScriptManager.AddScriptTags(script);

			return script;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="windowFeature"></param>
		/// <returns></returns>
		public static string GetClientObject(this IWindowFeature windowFeature)
		{
			return JSONSerializerExecute.Serialize(windowFeature, typeof(IWindowFeature));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="windowFeature"></param>
		/// <returns></returns>
		public static string ToDialogFeatureClientString(this IWindowFeature windowFeature)
		{
			return ToDialogFeatureClientString(windowFeature, QuatationMarkType.Single);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="windowFeature"></param>
		/// <returns></returns>
		public static string ToWindowFeatureClientString(this IWindowFeature windowFeature)
		{
			return ToWindowFeatureClientString(windowFeature, QuatationMarkType.Single);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="windowFeature"></param>
		/// <param name="quatationMarkType"></param>
		/// <returns></returns>
		public static string ToDialogFeatureClientString(this IWindowFeature windowFeature, QuatationMarkType quatationMarkType)
		{
			StringBuilder strB = new StringBuilder(256);
			strB.Append(GetLengthString("dialogWidth", windowFeature.Width, windowFeature.WidthScript, ":", ";", quatationMarkType, "px"));
			strB.Append(GetLengthString("dialogHeight", windowFeature.Height, windowFeature.HeightScript, ":", ";", quatationMarkType, "px"));
			strB.Append(GetLengthString("dialogTop", windowFeature.Top, windowFeature.TopScript, ":", ";", quatationMarkType, "px"));
			strB.Append(GetLengthString("dialogLeft", windowFeature.Left, windowFeature.LeftScript, ":", ";", quatationMarkType, "px"));

			if (windowFeature.Center != null)
				strB.AppendFormat("center:{0};", BoolToStr(windowFeature.Center.Value));

			if (windowFeature.Resizable != null)
				strB.AppendFormat("resizable:{0};", BoolToStr(windowFeature.Resizable.Value));

			if (windowFeature.ShowScrollBars != null)
				strB.AppendFormat("scroll:{0};", BoolToStr(windowFeature.ShowScrollBars.Value));

			if (windowFeature.ShowStatusBar != null)
				strB.AppendFormat("status:{0};", BoolToStr(windowFeature.ShowStatusBar.Value));

			if (strB.Length > 0)
				strB.Remove(strB.Length - 1, 1);

			return strB.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static string ToWindowFeatureClientString(this IWindowFeature windowFeature, QuatationMarkType quatationMarkType)
		{
			StringBuilder strB = new StringBuilder(256);
			string strWidth = GetStringValue(windowFeature.Width, windowFeature.WidthScript, null, WindowFeatureDefaultWidth.ToString());
			string strHeight = GetStringValue(windowFeature.Height, windowFeature.HeightScript, null, WindowFeatureDefaultHeigth.ToString());

			strB.Append(GetLengthString("width", windowFeature.Width, strWidth, "=", ",", quatationMarkType, string.Empty));
			strB.Append(GetLengthString("height", windowFeature.Height, strHeight, "=", ",", quatationMarkType, string.Empty));

			string leftScript = windowFeature.LeftScript;
			if (windowFeature.Left == null && string.IsNullOrEmpty(windowFeature.LeftScript) && windowFeature.Center.HasValue && windowFeature.Center.Value)
				leftScript = string.Format("(window.screen.width - {0}) / 2",
					strWidth);

			string topScript = windowFeature.TopScript;
			if (windowFeature.Top == null && string.IsNullOrEmpty(windowFeature.TopScript) && windowFeature.Center.HasValue && windowFeature.Center.Value)
				topScript = string.Format("(window.screen.height - {0}) / 2",
					strHeight);

			strB.Append(GetLengthString("left", windowFeature.Left, leftScript, "=", ",", quatationMarkType, string.Empty));
			strB.Append(GetLengthString("top", windowFeature.Top, topScript, "=", ",", quatationMarkType, string.Empty));

			if (windowFeature.Resizable != null)
				strB.AppendFormat("resizable={0},", BoolToStr(windowFeature.Resizable.Value));

			if (windowFeature.ShowScrollBars != null)
				strB.AppendFormat("scrollbars={0},", BoolToStr(windowFeature.ShowScrollBars.Value));

			if (windowFeature.ShowStatusBar != null)
				strB.AppendFormat("status={0},", BoolToStr(windowFeature.ShowStatusBar.Value));

			if (windowFeature.ShowToolBar != null)
				strB.AppendFormat("toolbar={0},", BoolToStr(windowFeature.ShowToolBar.Value));

			if (windowFeature.ShowAddressBar != null)
				strB.AppendFormat("location={0},", BoolToStr(windowFeature.ShowAddressBar.Value));

			if (strB.Length > 0)
				strB.Remove(strB.Length - 1, 1);

			return strB.ToString();
		}

		private static string GetLengthString(string lengthName, Nullable<int> lengthValue, string lengthScript, string op, string split, QuatationMarkType quatationMarkType, string unitStr)
		{
			string lengthStr = string.Empty;
			string strValue = GetStringValue(lengthValue, lengthScript, quatationMarkType, string.Empty);

			if (!string.IsNullOrEmpty(strValue))
			{
				lengthStr = string.Format("{0}{1}{2}{3}{4}", lengthName, op, strValue, unitStr, split);
			}

			return lengthStr;
		}

		private static string GetStringValue(Nullable<int> nValue, string script, Nullable<QuatationMarkType> quatationMarkType, string defaultValue)
		{
			string strValue = string.Empty;
			if (nValue != null)
			{
				strValue = nValue.ToString();
			}
			else if (!string.IsNullOrEmpty(script))
			{
				strValue = quatationMarkType == null ? script : AppendQuotationMarkInString(string.Format(" + ({0}) + ", script), quatationMarkType.Value);
			}
			else
				strValue = defaultValue;


			return strValue;
		}

		private static string BoolToStr(bool b)
		{
			return b ? "yes" : "no";
		}

		private static string AppendQuotationMark(string str, QuatationMarkType quatationMarkType)
		{
			return string.Format("{0}{1}{0}", GetQuotationMarkString(quatationMarkType), str);
		}

		private static string AppendQuotationMarkInString(string str, QuatationMarkType quatationMarkType)
		{
			return string.Format("{0}{1}{0}", GetQuotationMarkInString(quatationMarkType), str);
		}

		private static string GetQuotationMarkString(QuatationMarkType quatationMarkType)
		{
			return quatationMarkType == QuatationMarkType.Double ? "\"" : "'";
		}

		private static string GetQuotationMarkInString(QuatationMarkType quatationMarkType)
		{
			return quatationMarkType == QuatationMarkType.Double ? "\"" : "'";
		}
	}
}
