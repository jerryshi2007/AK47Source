using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Office.OpenXml.Excel;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library;
using MCS.Web.WebControls;

namespace WorkflowDesigner.MatrixModalDialog
{
	public partial class EditRoleProperty : System.Web.UI.Page
	{
		protected string AppID
		{
			get
			{
				return WebUtility.GetRequestQueryString("AppID", string.Empty);
			}
		}


		protected string RoleID
		{
			get
			{
				return WebUtility.GetRequestQueryString("RoleID", string.Empty);
			}
		}

		protected string DefinitionID
		{
			get
			{
				return WebUtility.GetRequestQueryString("definitionID", this.RoleID);
			}
		}

		protected string ControlID
		{
			get
			{
				return WebUtility.GetRequestQueryString("ControlID", string.Empty);
			}
		}

		public override void ProcessRequest(HttpContext context)
		{
			switch (context.Request.QueryString["editMode"])
			{
				case "download":
					{
						SOARole role = PrepareRole(this.RoleID, this.DefinitionID);

						WorkBook workBook = role.ToExcelWorkBook();

						context.Response.CacheControl = "no-cache";

						context.Response.AppendExcelOpenXmlHeader("matrix");
						workBook.Save(context.Response.OutputStream);

						context.Response.Flush();
					}
					break;
				case "readOnly":
					Server.Execute("~/MatrixModalDialog/DownloadMatrix.aspx", true);
					break;
				default:
					base.ProcessRequest(context);
					break;
			}
		}

		protected override void OnPreInit(EventArgs e)
		{
			base.OnPreInit(e);

			if (this.Request.QueryString["editMode"] == "readOnly")
			{
				this.ticket1.Enabled = false;
				this.btn_serverSave.Visible = false;
				this.editPan.Visible = false;
				this.Title = "角色矩阵在线编辑（只读模式）";
				this.MCRolePropertyEditTemplate.ReadOnly = true;
			}
			else
			{
				this.ticket1.Enabled = this.ControlID != this.MCRolePropertyEditTemplate.ID;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.RoleID.IsNotEmpty())
			{
				this.MCRolePropertyEditTemplate.RequestContext = string.Join(";", this.AppID, this.RoleID, this.DefinitionID);
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.Response.CacheControl = "no-cache";
		}

		protected void RolePropertyEdit_PrepareDownloadStream(object sender, PrepareDownloadStreamEventArgs args)
		{
			string[] itemsInfo = args.DownloadInfo.RequestContext.Split(';');

			string roleID = itemsInfo[1];
			string defID = itemsInfo[2];

			SOARole role = PrepareRole(roleID, defID);
			WorkBook workBook = role.ToExcelWorkBook();

			using (MemoryStream stream = new MemoryStream())
			{
				workBook.Save(stream);
				stream.Seek(0, SeekOrigin.Begin);
				stream.CopyTo(args.OutputStream);
			}
		}

		protected void btnServerSave_Click(object sender, EventArgs e)
		{
			if (Request.QueryString["editMode"] == "readOnly")
				throw new InvalidOperationException("只读模式不可以操作");

			int count = this.MCRolePropertyEditTemplate.Materials.Count;

			if (count > 0 && !this.MCRolePropertyEditTemplate.DeltaMaterials.IsEmpty())
			{
				Material roleTerial = null;

				if (this.MCRolePropertyEditTemplate.DeltaMaterials.Inserted.Count > 0)
					roleTerial = this.MCRolePropertyEditTemplate.DeltaMaterials.Inserted[this.MCRolePropertyEditTemplate.DeltaMaterials.Inserted.Count - 1];
				else if (this.MCRolePropertyEditTemplate.DeltaMaterials.Updated.Count > 0)
					roleTerial = this.MCRolePropertyEditTemplate.DeltaMaterials.Updated[this.MCRolePropertyEditTemplate.DeltaMaterials.Updated.Count - 1];

				if (roleTerial != null)
				{
					using (Stream LoadFile = roleTerial.GetTemporaryContent(this.MCRolePropertyEditTemplate.RootPathName))
					{
						Save(LoadFile);
					}
				}
			}
		}

		private void Save(Stream strem)
		{
			DataTable dt = DocumentHelper.GetRangeValuesAsTable(strem, "Matrix", "A3");
			string[] itemsInfo = this.MCRolePropertyEditTemplate.RequestContext.Split(';');

			string roleID = itemsInfo[1];
			string defID = itemsInfo[2];

			SOARole role = PrepareRole(roleID, defID);

			role.Rows.Clear();
			int rowIndex = 0;

			foreach (DataRow row in dt.Rows)
			{
				SOARolePropertyRow mRow = new SOARolePropertyRow(role) { RowNumber = rowIndex };

				foreach (var dimension in role.PropertyDefinitions)
				{
					SOARolePropertyValue mCell = new SOARolePropertyValue(dimension);
					mCell.Value = row[dimension.Name].ToString();

					switch (dimension.Name)
					{
						case "Operator":
							mRow.Operator = row[dimension.Name].ToString();
							break;
						case "OperatorType":
							SOARoleOperatorType opType = SOARoleOperatorType.User;
							Enum.TryParse(row[dimension.Name].ToString(), out opType);
							mRow.OperatorType = opType;
							break;
						default:
							break;
					}

					mRow.Values.Add(mCell);
				}

				rowIndex++;
				role.Rows.Add(mRow);
			}

			//更新数据库
			SOARolePropertiesAdapter.Instance.Update(role);
		}

		private static SOARole PrepareRole(string roleID, string definitionID)
		{
			SOARolePropertyDefinitionCollection definition = SOARolePropertyDefinitionAdapter.Instance.LoadByRoleID(definitionID);

			return new SOARole(definition) { ID = roleID };
		}
	}
}