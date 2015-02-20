using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using System.IO;
using MCS.Library.Core;

namespace MCS.Web.Library
{
	/// <summary>
	/// WebControl����������
	/// </summary>
	/// <remarks>
	/// ����һЩ��WebControl����������ľ�̬����
	/// </remarks>
	public static class WebControlUtility
	{
		/// <summary>
		/// ������Դ��ListControl��
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

		private static V GetViewStateValueInternal<V>(StateBag viewState, string key, V nullValue, bool setNullValue, bool isTrackingViewState)
		{
			if (viewState[key] == null)
			{
				if (setNullValue)
				{
					if (isTrackingViewState)
					{
						IStateManager sm = nullValue as IStateManager;
						if (sm != null)
						{
							sm.TrackViewState();
						}
					}
					viewState[key] = nullValue;
				}

				return nullValue;
			}
			return (V)viewState[key];
		}

		/// <summary>
		/// ��ȡViewSate��ĳһ���ֵ
		/// </summary>
		/// <typeparam name="V">����ֵ������</typeparam>
		/// <param name="viewState">ViewSate</param>
		/// <param name="key">ĳһ���Key</param>
		/// <param name="nullValue">��������ֵΪ�գ��򷵻ش�Ĭ��ֵ</param>
		/// <returns>����ֵ</returns>
		/// <remarks>���ڿؼ������У���ȡ����ֵ</remarks>
		public static V GetViewStateValue<V>(this StateBag viewState, string key, V nullValue)
		{
			return GetViewStateValueInternal<V>(viewState, key, nullValue, false, false);
		}

		/// <summary>
		///	����ViewSate��ĳһ���ֵ
		/// </summary>
		/// <typeparam name="V">����ֵ������</typeparam>
		/// <param name="viewState">ViewSate</param>
		/// <param name="key">ĳһ���Key</param>
		/// <param name="value">���õ�ֵ</param>
		/// <remarks>���ڿؼ������У���������ֵ</remarks>
		public static void SetViewStateValue<V>(this StateBag viewState, string key, V value)
		{
			viewState[key] = value;
			IStateManager sm = value as IStateManager;
			if (sm != null)
				sm.TrackViewState();
		}

		/// <summary>
		/// ��ViewState������IStateManager���������TrackViewState
		/// </summary>
		/// <param name="viewState">ViewState</param>
		/// <remarks>
		/// �ڿؼ���TrackViewState�е���		
		/// </remarks>
		internal static void TrackViewState(StateBag viewState)
		{
			foreach (string key in viewState.Keys)
			{
				IStateManager o = viewState[key] as IStateManager;
				if (o != null)
				{
					o.TrackViewState();
				}
			}
		}

		/// <summary>
		/// ��LoadViewState֮ǰ����ViewState������IStateManager������
		/// </summary>
		/// <param name="viewState">ViewState</param>
		/// <returns>������</returns>		
		/// <remarks>
		/// ��LoadViewState֮ǰ����
		/// </remarks>
		/// <example>
		/// <![CDATA[
		///protected override void LoadViewState(object savedState)
		///{
		///    StateBag backState = WebControlUtility.PreLoadViewState(ViewState);
		///    base.LoadViewState(savedState);
		///    WebControlUtility.AfterLoadViewState(ViewState, backState);
		///}
		/// ]]>
		/// </example>
		internal static StateBag PreLoadViewState(StateBag viewState)
		{
			StateBag backState = new StateBag();
			foreach (string key in viewState.Keys)
			{
				IStateManager o = viewState[key] as IStateManager;
				if (o != null)
				{
					backState[key] = o;
				}
			}
			return backState;
		}

		/// <summary>
		/// ��LoadViewState֮��ӻ����лָ���ViewState��ViewSateItemInternal������ָ���IStateManager������
		/// </summary>
		/// <param name="viewState">ViewState</param>
		/// <param name="backState">PreLoadViewState�����������</param>
		/// <remarks>��LoadViewState֮�����</remarks>
		/// <example>
		/// <![CDATA[
		///protected override void LoadViewState(object savedState)
		///{
		///    StateBag backState = WebControlUtility.PreLoadViewState(ViewState);
		///    base.LoadViewState(savedState);
		///    WebControlUtility.AfterLoadViewState(ViewState, backState);
		///}
		/// ]]>
		/// </example>
		internal static void AfterLoadViewState(StateBag viewState, StateBag backState)
		{
			foreach (string key in viewState.Keys)
			{
				ViewSateItemInternal vTemp = viewState[key] as ViewSateItemInternal;
				if (vTemp != null)
				{
					if (backState[key] != null)
					{
						viewState[key] = backState[key];
						((IStateManager)viewState[key]).LoadViewState(vTemp.State);
					}
					else
						viewState[key] = ((ViewSateItemInternal)viewState[key]).GetObject();
				}
			}
		}

		/// <summary>
		/// ��SaveViewState֮ǰ����ViewState������IStateManager������ת��Ϊ�����л���ViewSateItemInternal������
		/// </summary>
		/// <param name="viewState">ViewState</param>
		/// <remarks>��SaveViewState֮ǰ����</remarks>		
		/// <example>
		/// <![CDATA[
		///protected override object SaveViewState()
		///{
		///    WebControlUtility.PreSaveViewState(ViewState);
		///    object o = base.SaveViewState();
		///    WebControlUtility.AfterSavedViewState(ViewState);
		///    return o;
		///}
		/// ]]>
		/// </example>
		internal static void PreSaveViewState(StateBag viewState)
		{
			foreach (string key in viewState.Keys)
			{
				IStateManager o = viewState[key] as IStateManager;
				if (o != null)
				{
					viewState[key] = new ViewSateItemInternal(o);
				}
			}
		}

		/// <summary>
		/// ��SaveViewState֮�󣬽�ViewState������ViewSateItemInternal������ָ���IStateManager������
		/// </summary>
		/// <param name="viewState">ViewState</param>
		/// <remarks>��SaveViewState֮��
		/// 
		/// ����</remarks>		
		/// <example>
		/// <![CDATA[
		///protected override object SaveViewState()
		///{
		///    WebControlUtility.PreSaveViewState(ViewState);
		///    object o = base.SaveViewState();
		///    WebControlUtility.AfterSavedViewState(ViewState);
		///    return o;
		///}
		/// ]]>
		/// </example>
		internal static void AfterSavedViewState(StateBag viewState)
		{
			foreach (string key in viewState.Keys)
			{
				object o = viewState[key];
				if (o is ViewSateItemInternal)
				{
					viewState[key] = ((ViewSateItemInternal)o).GetObject();
				}
			}
		}

		/// <summary>
		/// ��containerControl�в�������ΪcontrolType�Ŀؼ�
		/// </summary>
		/// <param name="containerControl">���ؼ�</param>
		/// <param name="controlType">���ҵĿؼ�����</param>
		/// <param name="deepFind">�Ƿ������Ȳ���</param>
		/// <returns>�ҵ��Ŀؼ�</returns>
		/// <remarks>��containerControl�в�������ΪcontrolType�Ŀؼ�</remarks>
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
		/// ��containerControl�в�������ΪT�Ŀؼ�
		/// </summary>
		/// <typeparam name="T">���ҵĿؼ�����</typeparam>
		/// <param name="containerControl">���ؼ�</param>
		/// <param name="deepFind">�Ƿ������Ȳ���</param>
		/// <returns>�ҵ��Ŀؼ�</returns>
		/// <remarks>��containerControl�в�������ΪcontrolType�Ŀؼ�</remarks>
		public static T FindControl<T>(this Control containerControl, bool deepFind)
			where T : Control
		{
			return (T)FindControl(containerControl, typeof(T), deepFind);
		}

		/// <summary>
		/// ����ָ�����͵������ӿؼ�
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
		/// ����ָ�����͵������ӿؼ�
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
		/// ��containerControl�в���IDΪcontrolID�Ŀؼ�
		/// </summary>
		/// <param name="containerControl">���ؼ�</param>
		/// <param name="controlID">�ӿؼ���ID</param>
		/// <param name="deepFind">�Ƿ������Ȳ���</param>
		/// <returns>�ҵ��Ŀؼ�</returns>
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
		/// �����ӿؼ�
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
		/// ��containerControl�в���html��idΪcontrolID�Ŀؼ�
		/// </summary>
		/// <param name="containerControl">���ؼ�</param>
		/// <param name="controlID">�ӿؼ���html id</param>
		/// <param name="deepFind">�Ƿ������Ȳ���</param>
		/// <returns>�ҵ��Ŀؼ�</returns>
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
		/// ��������ΪcontrolType�ĸ��׼����ȿؼ�
		/// </summary>
		/// <param name="currentControl">��ǰ�Ŀؼ�</param>
		/// <param name="controlType">�ؼ�����</param>
		/// <param name="recursively">�Ƿ�༶����</param>
		/// <returns>�������͵Ŀؼ�</returns>
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
		/// ��������ΪT�ĸ��׼����ȿؼ�
		/// </summary>
		/// <typeparam name="T">�ؼ�����</typeparam>
		/// <param name="currentControl">��ǰ�Ŀؼ�</param>
		/// <param name="recursively">�Ƿ�༶����</param>
		/// <returns>�������͵Ŀؼ�</returns>
		public static T FindParentControl<T>(this Control currentControl, bool recursively)
			where T : Control
		{
			return (T)FindParentControl(currentControl, typeof(T), recursively);
		}

		/// <summary>
		/// �õ��ؼ���Html����
		/// </summary>
		/// <param name="ctrl"></param>
		/// <returns></returns>
		public static string GetControlHtml(Control ctrl)
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

		/// <summary>
		/// �õ��ؼ���Html
		/// </summary>
		/// <param name="ctrl"></param>
		/// <returns></returns>
		public static string GetHtml(this Control ctrl)
		{
			string result = string.Empty;

			if (ctrl != null)
				result = GetControlHtml(ctrl);

			return result;
		}

		private static DataBoundControlInternal GetChildDataBoundControl(Control ctr)
		{
			string ctrID = ctr.ID + "__DataBoundControlInternal";
			DataBoundControlInternal dataCtr = (DataBoundControlInternal)ctr.FindControl(ctrID);
			if (dataCtr == null)
			{
				dataCtr = new DataBoundControlInternal();
				dataCtr.ID = ctrID;
				ctr.Controls.Add(dataCtr);
			}

			return dataCtr;
		}

		/// <summary>
		/// ������ԴdataSource�����IEnumerable���͵�����Դ
		/// </summary>
		/// <param name="ctr">��Ҫ��������Դ�Ŀؼ�</param>
		/// <param name="dataSource">��Ҫ���������Դ</param>
		/// <returns>������</returns>
		/// <remarks>������ԴdataSource�����IEnumerable���͵�����Դ</remarks>
		public static IEnumerable GetDataSourceResult(Control ctr, object dataSource)
		{
			DataBoundControlInternal dataCtr = GetChildDataBoundControl(ctr);
			dataCtr.DataSourceID = string.Empty;
			dataCtr.DataSource = dataSource;
			dataCtr.DataBind();

			return dataCtr.DataSourceResult;
		}

		/// <summary>
		/// ������Դ�ؼ������IEnumerable���͵�����Դ
		/// </summary>
		/// <param name="ctr">��Ҫ��������Դ�Ŀؼ�</param>
		/// <param name="dataSourceID">��Ҫ���������Դ�ؼ�ID</param>
		/// <returns>������</returns>
		/// <remarks>������Դ�ؼ������IEnumerable���͵�����Դ</remarks>
		public static IEnumerable GetDataSourceResult(Control ctr, string dataSourceID)
		{
			DataBoundControlInternal dataCtr = GetChildDataBoundControl(ctr);
			dataCtr.DataSource = null;
			dataCtr.DataSourceID = dataSourceID;
			dataCtr.DataBind();

			return dataCtr.DataSourceResult;
		}

		/// <summary>
		/// ����RenderMode���
		/// </summary>
		/// <param name="baseRenderString"></param>
		/// <param name="renderMode"></param>
		public static void RenderPageOnlySelf(this PageRenderMode renderMode, string baseRenderString)
		{
			HttpResponse response = HttpContext.Current.Response;
			response.ClearContent();
			response.ContentEncoding = Encoding.UTF8;
			response.Charset = "utf-8";

			string htmlContentType = WebUtility.GetContentTypeByKey(ResponseContentTypeKey.HTML);
			string contentType = WebUtility.GetContentTypeByKey(renderMode.ContentTypeKey);

			if (contentType == string.Empty)
				contentType = htmlContentType;

			response.ContentType = contentType;

			switch (renderMode.DispositionType)
			{
				case ResponseDispositionType.Inline:
				case ResponseDispositionType.Attachment:
					response.AppendHeader("CONTENT-DISPOSITION",
						string.Format("{0};filename={1}", renderMode.DispositionType, response.EncodeFileNameInContentDisposition(renderMode.AttachmentFileName)));
					break;
			}

			response.Write(baseRenderString);
			response.End();
		}

		/// <summary>
		/// ���ݿؼ����ģʽ������ؼ�
		/// </summary>
		/// <param name="ctr">Ҫ����Ŀؼ�</param>
		/// <param name="baseRenderString">�ؼ�ԭ�������</param>
		/// <param name="renderMode">���ģʽ</param>
		/// <remarks>���ݿؼ����ģʽ������ؼ�</remarks>
		internal static void RenderControlOnlySelf(Control ctr, string baseRenderString, ControlRenderMode renderMode)
		{
			HttpResponse response = HttpContext.Current.Response;
			response.ClearContent();
			response.ContentEncoding = Encoding.UTF8;
			response.Charset = "utf-8";

			string htmlContentType = WebUtility.GetContentTypeByKey(ResponseContentTypeKey.HTML);
			string contentType = WebUtility.GetContentTypeByKey(renderMode.ContentTypeKey);

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
	}
}
