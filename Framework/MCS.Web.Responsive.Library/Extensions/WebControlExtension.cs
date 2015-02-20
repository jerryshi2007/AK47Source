using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.Core;

namespace MCS.Web.Responsive.Library
{
	/// <summary>
	/// Web Control的扩展方法
	/// </summary>
	public static class WebControlExtension
	{
		/// <summary>
		/// 绑定数据源到ListControl上
		/// </summary>
		/// <param name="listCtrl"></param>
		/// <param name="dataSource"></param>
		/// <param name="valueField"></param>
		/// <param name="textField"></param>
		public static void BindData(this ListControl listCtrl, object dataSource, string valueField, string textField)
		{
			if (listCtrl != null && dataSource != null)
			{
				listCtrl.DataValueField = valueField;
				listCtrl.DataTextField = textField;

				listCtrl.DataSource = dataSource;
				listCtrl.DataBind();
			}
		}

		/// <summary>
		/// 搜索指定ID的目标控件
		/// </summary>
		/// <param name="owner">容器控件</param>
		/// <param name="controlID">搜索的控件ID</param>
		/// <param name="searchNamingContainers">表示是否搜索命名容器</param>
		/// <returns><see cref="Control"/>或<see langword="null"/></returns>
		public static Control FindTargetControl(this Control owner, string controlID, bool searchNamingContainers)
		{
			controlID.CheckStringIsNullOrEmpty("controlID");
			owner.NullCheck("owner");

			if (searchNamingContainers)
			{
				Control namingContainer;
				Control control2 = null;
				if (owner is INamingContainer)
					namingContainer = owner;
				else
					namingContainer = owner.NamingContainer;

				do
				{
					control2 = namingContainer.FindControl(controlID);
					namingContainer = namingContainer.NamingContainer;
				}
				while ((control2 == null) && (namingContainer != null));
				return control2;
			}
			else
				return owner.FindControl(controlID);
		}




		/// <summary>
		/// 在containerControl中查找类型为controlType的控件
		/// </summary>
		/// <param name="containerControl">父控件</param>
		/// <param name="controlType">查找的控件类型</param>
		/// <param name="deepFind">是否进行深度查找</param>
		/// <returns>找到的控件</returns>
		/// <remarks>在containerControl中查找类型为controlType的控件</remarks>
		public static Control FindControl(this Control containerControl, Type controlType, bool deepFind)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(controlType != null, "controlType");

			Control result = null;

			if (containerControl != null)
			{
				foreach (Control ctr in containerControl.Controls)
				{
					if (ctr.GetType() == controlType)
					{
						result = ctr;
					}
					else
					{
						if (deepFind)
							result = FindControl(ctr, controlType, deepFind);
					}

					if (result != null)
						break;
				}
			}

			return result;
		}

		/// <summary>
		/// 在containerControl中查找类型为T的控件
		/// </summary>
		/// <typeparam name="T">查找的控件类型</typeparam>
		/// <param name="containerControl">父控件</param>
		/// <param name="deepFind">是否进行深度查找</param>
		/// <returns>找到的控件</returns>
		/// <remarks>在containerControl中查找类型为controlType的控件</remarks>
		public static T FindControl<T>(this Control containerControl, bool deepFind)
			where T : Control
		{
			return (T)FindControl(containerControl, typeof(T), deepFind);
		}

		/// <summary>
		/// 查找指定类型的所有子控件
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="containerControl"></param>
		/// <param name="deepFind"></param>
		/// <returns></returns>
		public static IList<T> FindControls<T>(this Control containerControl, bool deepFind) where T : Control
		{
			Type controlType = typeof(T);
			List<T> result = new List<T>();

			InternalFindControls(containerControl, controlType, deepFind, result);

			return result;
		}

		/// <summary>
		/// 查找指定类型的所有子控件
		/// </summary>
		/// <param name="containerControl"></param>
		/// <param name="controlType"></param>
		/// <param name="deepFind"></param>
		/// <returns></returns>
		public static IList<Control> FindControls(this Control containerControl, Type controlType, bool deepFind)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(controlType != null, "controlType");

			List<Control> result = new List<Control>();

			InternalFindControls(containerControl, controlType, deepFind, result);

			return result;
		}

		private static void InternalFindControls<T>(Control containerControl, Type controlType, bool deepFind, List<T> result) where T : Control
		{
			if (containerControl != null)
			{
				foreach (Control ctr in containerControl.Controls)
				{
					if (ctr.GetType() == controlType)
						result.Add((T)ctr);

					if (deepFind)
						InternalFindControls(ctr, controlType, deepFind, result);
				}
			}
		}

		private static void InternalFindControls(Control containerControl, Type controlType, bool deepFind, List<Control> result)
		{
			if (containerControl != null)
			{
				foreach (Control ctr in containerControl.Controls)
				{
					if (ctr.GetType() == controlType)
						result.Add(ctr);

					if (deepFind)
						InternalFindControls(ctr, controlType, deepFind, result);
				}
			}
		}

		/// <summary>
		/// 在containerControl中查找ID为controlID的控件
		/// </summary>
		/// <param name="containerControl">父控件</param>
		/// <param name="controlID">子控件的ID</param>
		/// <param name="deepFind">是否进行深度查找</param>
		/// <returns>找到的控件</returns>
		public static Control FindControlByID(this Control containerControl, string controlID, bool deepFind)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(containerControl != null, "containerControl");
			ExceptionHelper.CheckStringIsNullOrEmpty(controlID, "controlID");

			Control result = containerControl.FindControl(controlID);

			if (result == null || (result == containerControl) && deepFind)
			{
				result = null;	//Radio Button List Find Control always return itself.
				foreach (Control innerCtrl in containerControl.Controls)
				{
					result = FindControlByID(innerCtrl, controlID, deepFind);

					if (result != null)
					{
						//Radio Button List Find Control always return itself.
						if (result != innerCtrl)
							break;
						else
							result = null;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// 查找子控件
		/// </summary>
		/// <param name="containerControl"></param>
		/// <param name="predicate"></param>
		/// <param name="deepFind"></param>
		/// <returns></returns>
		public static Control FindControl(this Control containerControl, Predicate<Control> predicate, bool deepFind)
		{
			Control result = null;

			if (containerControl != null)
			{
				foreach (Control control in containerControl.Controls)
				{
					if (predicate(control))
					{
						result = control;
						break;
					}
				}

				if (result == null && deepFind)
				{
					foreach (Control control in containerControl.Controls)
					{
						result = control.FindControl(predicate, deepFind);

						if (result != null)
							break;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// 在containerControl中查找html的id为controlID的控件
		/// </summary>
		/// <param name="containerControl">父控件</param>
		/// <param name="controlID">子控件的html id</param>
		/// <param name="deepFind">是否进行深度查找</param>
		/// <returns>找到的控件</returns>
		public static Control FindControlByHtmlIDProperty(this Control containerControl, string controlID, bool deepFind)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(containerControl != null, "containerControl");
			ExceptionHelper.CheckStringIsNullOrEmpty(controlID, "controlID");

			Control result = null;

			if (containerControl is IAttributeAccessor)
			{
				if (string.Compare(containerControl.ID, controlID, true) == 0)
					result = containerControl;
			}

			if (deepFind)
			{
				if (result == null)
				{
					foreach (Control innerCtrl in containerControl.Controls)
					{
						result = FindControlByHtmlIDProperty(innerCtrl, controlID, true);

						if (result != null)
							break;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// 查找类型为controlType的父亲及祖先控件
		/// </summary>
		/// <param name="currentControl">当前的控件</param>
		/// <param name="controlType">控件类型</param>
		/// <param name="recursively">是否多级查找</param>
		/// <returns>符合类型的控件</returns>
		public static Control FindParentControl(this Control currentControl, Type controlType, bool recursively)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(currentControl != null, "currentControl");

			Control result = currentControl;

			while (result != null && result.GetType() != controlType)
			{
				if (recursively)
					result = result.Parent;
				else
					result = null;
			}

			return result;
		}

		/// <summary>
		/// 查找类型为T的父亲及祖先控件
		/// </summary>
		/// <typeparam name="T">控件类型</typeparam>
		/// <param name="currentControl">当前的控件</param>
		/// <param name="recursively">是否多级查找</param>
		/// <returns>符合类型的控件</returns>
		public static T FindParentControl<T>(this Control currentControl, bool recursively)
			where T : Control
		{
			return (T)FindParentControl(currentControl, typeof(T), recursively);
		}

		/// <summary>
		/// 创建子元素，且在创建完成后执行回调
		/// </summary>
		/// <param name="container"></param>
		/// <param name="tagName"></param>
		/// <param name="action"></param>
		public static void CreateSubItems(this Control container, string tagName, Action<HtmlGenericControl> action)
		{
			if (container != null)
			{
				HtmlGenericControl subItem = new HtmlGenericControl(tagName);
				container.Controls.Add(subItem);

				if (action != null)
					action(subItem);
			}
		}

		/// <summary>
		/// 创建作为子元素的图片元素
		/// </summary>
		/// <param name="container"></param>
		/// <param name="src"></param>
		/// <returns></returns>
		public static HtmlImage CreateSubImage(this Control container, string src)
		{
			container.NullCheck("container");

			HtmlImage img = new HtmlImage();
			container.Controls.Add(img);

			img.Src = src;

			return img;
		}

		/// <summary>
		/// 得到控件的Html
		/// </summary>
		/// <param name="ctrl"></param>
		/// <returns></returns>
		public static string GetHtml(this Control ctrl)
		{
			string result = string.Empty;

			if (ctrl != null)
			{
				StringBuilder strB = new StringBuilder(1024);

				using (StringWriter sw = new StringWriter(strB))
				{
					using (HtmlTextWriter writer = new HtmlTextWriter(sw))
					{
						ctrl.RenderControl(writer);
					}
				}

				result = strB.ToString();
			}

			return result;
		}

		/// <summary>
		/// 根据控件输出模式，输出控件
		/// </summary>
		/// <param name="ctr">要输出的控件</param>
		/// <param name="baseRenderString">控件原输出内容</param>
		/// <param name="renderMode">输出模式</param>
		/// <remarks>根据控件输出模式，输出控件</remarks>
		internal static void RenderControlOnlySelf(this Control ctr, string baseRenderString, ControlRenderMode renderMode)
		{
			HttpResponse response = HttpContext.Current.Response;
			response.ClearContent();
			response.ContentEncoding = Encoding.UTF8;
			response.Charset = "utf-8";

			string htmlContentType = WebAppSettings.GetContentTypeByKey(ResponseContentTypeKey.HTML);
			string contentType = WebAppSettings.GetContentTypeByKey(renderMode.ContentTypeKey);

			if (contentType == string.Empty)
				contentType = htmlContentType;

			response.ContentType = contentType;

			string renderStr = contentType == htmlContentType ?
				GetRenderSingleControlString(ctr, baseRenderString) :
				GetFileRenderSingleControlString(baseRenderString, contentType);

			switch (renderMode.DispositionType)
			{
				case ResponseDispositionType.Inline:
				case ResponseDispositionType.Attachment:
					response.AppendHeader("CONTENT-DISPOSITION",
						string.Format("{0};filename={1}", renderMode.DispositionType, response.EncodeFileNameInContentDisposition(renderMode.AttachmentFileName)));
					break;
			}

			response.Write(renderStr);
			response.End();
		}

		private static string GetRenderSingleControlString(Control ctr, string baseRenderString)
		{
			StringBuilder strB = new StringBuilder();
			System.IO.StringWriter sw = new System.IO.StringWriter(strB);
			HtmlTextWriter w = new HtmlTextWriter(sw);

			Page page = ctr.Page;

			w.WriteFullBeginTag("html");

			if (page.Header != null)
				page.Header.GetType().GetMethod("Render", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(page.Header, new object[1] { w });

			w.WriteFullBeginTag("body");
			w.WriteBeginTag("form");

			page.Form.GetType().GetMethod("RenderAttributes", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(page.Form, new object[1] { w });

			w.Write(">");

			string formID = page.Form.ClientID;
			page.GetType().GetMethod("BeginFormRender", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(page, new object[2] { w, formID });

			w.Write(baseRenderString);

			ScriptManager sm = ScriptManager.GetCurrent(page);
			sm.GetType().GetMethod("Render", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(sm, new object[1] { w });

			page.GetType().GetMethod("EndFormRender", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(page, new object[2] { w, formID });
			w.WriteEndTag("body");
			w.WriteEndTag("html");

			return strB.ToString();
		}

		private static string GetFileRenderSingleControlString(string baseRenderString, string contentType)
		{
			StringBuilder strB = new StringBuilder();

			System.IO.StringWriter sw = new System.IO.StringWriter(strB);
			HtmlTextWriter w = new HtmlTextWriter(sw);

			w.WriteFullBeginTag("html");
			w.RenderBeginTag("head");
			w.Write(string.Format("<meta http-equiv=\"Content-Type\" content=\"{0}; charset=utf-8\" />", contentType));
			w.RenderEndTag();
			w.WriteFullBeginTag("body");

			w.Write(baseRenderString);

			w.WriteEndTag("body");
			w.WriteEndTag("html");

			return strB.ToString();
		}

		/// <summary>
		/// 得到控件的Html描述
		/// </summary>
		/// <param name="ctrl"></param>
		/// <returns></returns>
		public static string GetControlHtml(this Control ctrl)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(ctrl != null, "ctrl");

			StringBuilder strB = new StringBuilder(1024);

			using (StringWriter sw = new StringWriter(strB))
			{
				using (HtmlTextWriter writer = new HtmlTextWriter(sw))
				{
					ctrl.RenderControl(writer);
				}
			}

			return strB.ToString();
		}
	}
}
