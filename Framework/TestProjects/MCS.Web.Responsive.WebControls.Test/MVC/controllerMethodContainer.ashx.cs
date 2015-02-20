using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Web.Responsive.Library.MVC;

namespace MCS.Web.Responsive.WebControls.Test.MVC
{
	/// <summary>
	/// Summary description for controllerMethodContainer
	/// </summary>
	public class controllerMethodContainer : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
			ControllerHelper.ExecuteMethodByRequest(this);
		}

		[ControllerMethod(true)]
		protected void DefaultMethod()
		{
			HttpContext.Current.Response.Write("DefaultMethod");
		}

		[ControllerMethod]
		protected void OneParamMethod(string name)
		{
			HttpContext.Current.Response.Write(HttpUtility.HtmlEncode(name));
		}

		[ControllerMethod]
		protected void TwoParamMethod(string name, bool isMale)
		{
			HttpContext.Current.Response.Write(
				string.Format("Name={0}, Is Male={1}", HttpUtility.HtmlEncode(name), isMale));
		}

		[ControllerMethod("ActivityID")]
		protected void IgnoreParamMethod(string name, string activityID)
		{
			HttpContext.Current.Response.Write(
				string.Format("Name={0}, ActivityID={1}", HttpUtility.HtmlEncode(name), HttpUtility.HtmlEncode(activityID)));
		}

		[ControllerMethod]
		protected void ClassParamMethod(ParameterClass pClass)
		{
			HttpContext.Current.Response.Write(
				string.Format("Name={0}, Age={1}", HttpUtility.HtmlEncode(pClass.Name), pClass.Age));
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}