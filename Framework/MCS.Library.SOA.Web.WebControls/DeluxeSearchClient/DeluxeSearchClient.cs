
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI;
using MCS.Library.Data.Mapping;
using MCS.Web.Library;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.DeluxeSearchClient.DeluxeSearchClient.js", "text/javascript")]
[assembly: WebResource("MCS.Web.WebControls.DeluxeSearchClient.DeluxeSearchClient.css", "text/css")]
namespace MCS.Web.WebControls
{
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientCssResource("MCS.Web.WebControls.DeluxeSearchClient.DeluxeSearchClient.css")]
	[ClientScriptResource("MCS.Web.WebControls.DeluxeSearchClient", "MCS.Web.WebControls.DeluxeSearchClient.DeluxeSearchClient.js")]
	[Designer(typeof(DeluxeSearchClientDesigner), typeof(IDesigner))]
	public class DeluxeSearchClient : ScriptControlBase
	{
		public DeluxeSearchClient()
			: base(false, HtmlTextWriterTag.Div)
		{
		}

		/// <summary>
		/// BindingControl的ID
		/// </summary>
		[DefaultValue("")]
		[ScriptControlProperty]
		[ClientPropertyName("bindingControlID")]
		[Browsable(true), Description("BindingContro的ID")]
		public string BindingControlID
		{
			get { return GetPropertyValue("BindingControlID", string.Empty); }
			set { SetPropertyValue("BindingControlID", value); }
		}

		/// <summary>
		/// 关闭并返回
		/// </summary>
		/// <param name="condition"></param>
		public void RegisterReturnValue(object condition)
		{
			var builder = ConditionMapping.GetWhereSqlClauseBuilder(condition);

			var result = JSONSerializerExecute.Serialize(builder);

			Page.ClientScript.RegisterStartupScript(Page.ClientScript.GetType(), "DeluxeSearchClientCloseDialog",
														string.Format(
															"window.returnValue = \"{0}\";top.close();",
															WebUtility.CheckScriptString(result, false)), true);
		}
	}
}
