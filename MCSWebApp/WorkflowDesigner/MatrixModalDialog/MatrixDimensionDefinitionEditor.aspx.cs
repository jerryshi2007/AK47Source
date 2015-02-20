using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Web.WebControls;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.Script;

namespace WorkflowDesigner.MatrixModalDialog
{
	public partial class MatrixDimensionDefinitionEditor : System.Web.UI.Page, ICallbackEventHandler
	{
		string matrixKey = string.Empty;

		protected override void OnPreRender(EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				SetHiddenJsonData();
			}

			base.OnPreRender(e);
		}
		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetCacheability(HttpCacheability.NoCache);
			WebUtility.RequiredScript(typeof(ClientGrid));

			var dataTypeList = EnumItemDescriptionAttribute.GetDescriptionList(typeof(PropertyDataType));
			dataTypeDropDownList.DataSource = dataTypeList;
			dataTypeDropDownList.DataTextField = "Name";

			dataTypeDropDownList.DataValueField = "EnumValue";
			dataTypeDropDownList.DataBind();
			//dataTypeDropDownList.Items.Insert(0, (new ListItem("请选择数据类型", "")));

			if (!Page.IsPostBack)
			{
				detailGrid.InitialData = new WfMatrixDimensionDefinitionCollection() {
				 new WfMatrixDimensionDefinition()
				 { 
					DataType = PropertyDataType.String,
					Description = "ActivityKey",
				    DimensionKey = "ActivityKey",
					Name="活动KEY",
					SortOrder = 0
				 },
				 new WfMatrixDimensionDefinition()
				 { 
					DataType = PropertyDataType.String,
					Description = "Operator",
				    DimensionKey = "Operator",
					Name="操作人",
					SortOrder = 1},
				 new WfMatrixDimensionDefinition()
				 { 
					DataType = PropertyDataType.String,
					Description = "OperatorType",
				    DimensionKey = "OperatorType",
					Name="操作人类型",
					SortOrder = 2}
				};
			}
		}

		private void SetHiddenJsonData()
		{
			try
			{
				string matrixKey = MCS.Web.Library.WebUtility.GetRequestParamString("matrixKey", string.Empty);
				if (!string.IsNullOrEmpty(matrixKey))
				{
					var matrixDef = WfMatrixDefinitionAdapter.Instance.Load(matrixKey);
					hiddenMatrixDimDefJsonData.Value = JSONSerializerExecute.Serialize(matrixDef);
					/*
					txtMatrixDesc.Text = matrixDef.Description;
					txtMatrixKey.Text = matrixDef.Key;
					txtMatrixKey.Enabled = false;
					txtmatrixName.Text = matrixDef.Name;
					this.detailGrid.InitialData = matrixDef.Dimensions;
					 */
				}
			}
			catch (Exception ex)
			{
				WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
			}
		}

		public string GetCallbackResult()
		{
			return CheckMatrixKey(matrixKey).ToString();
		}

		public bool CheckMatrixKey(string matrixKey)
		{
			try
			{
				WfMatrixDefinitionAdapter.Instance.Load(matrixKey);
				return false;
			}
			catch (Exception)
			{
				return true;
			}
		}

		private string GetMatrixKey()
		{
			return MCS.Library.Core.UuidHelper.NewUuidString();
		}
		public void RaiseCallbackEvent(string eventArgument)
		{
			matrixKey = eventArgument;
		}

		protected void btnConfirm_Click(object sender, EventArgs e)
		{
			string matrixKey = MCS.Web.Library.WebUtility.GetRequestParamString("matrixKey", string.Empty);
			try
			{
				string strMatrixKey = string.IsNullOrEmpty(matrixKey) ? txtMatrixKey.Text : hiddenMatrixDefKey.Value;
				string matrixName = txtmatrixName.Text;
				string matrixDesc = txtMatrixDesc.Text;
				bool isEnabled = bool.Parse(ddlEnabled.Value);

				var matrixInstanceCollection = WfMatrixAdapter.Instance.Load(p => p.AppendItem("DEF_KEY", strMatrixKey));

				if (matrixInstanceCollection.Count > 0)
				{
					Page.ClientScript.RegisterStartupScript(this.GetType(), "returnProcesses",
					string.Format("alert('矩阵定义已经被使用，无法再修改！');"),
					true);

					return;
				}

				var matrixDef = new WfMatrixDefinition()
				{
					Key = strMatrixKey,
					Description = matrixDesc,
					Enabled = isEnabled,
					Name = matrixName,
				};
				var dimesions = detailGrid.InitialData as WfMatrixDimensionDefinitionCollection;
				dimesions.ForEach(dimesion => dimesion.MatrixKey = strMatrixKey);
				matrixDef.Dimensions.CopyFrom(dimesions);

				WfMatrixDefinitionAdapter.Instance.Update(matrixDef);

				Page.ClientScript.RegisterStartupScript(this.GetType(), "returnProcesses",
					string.Format("window.returnValue = true; top.close();"),
					true);

			}
			catch (Exception ex)
			{
				WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
			}

		}
	}
}