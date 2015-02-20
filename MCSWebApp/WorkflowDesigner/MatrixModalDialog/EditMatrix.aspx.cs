using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using MCS.Library.Core;
using MCS.Web.WebControls;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Caching;

namespace WorkflowDesigner.MatrixModalDialog
{
	public partial class EditMatrix : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!this.IsPostBack)
			{
				bool roleAsPerson = false;
				string matrixKey = Request.QueryString["key"];
				ExceptionHelper.TrueThrow<ArgumentException>(string.IsNullOrEmpty(Request.QueryString["key"]), "参数key无效！");

				if (!Boolean.TryParse(Request.QueryString["roleAsPerson"], out roleAsPerson))
				{
					throw new ArgumentException("参数roleAsPerson无效！");
				}

				this.MCMatrixEditTemplate.RequestContext = matrixKey + ";" + this.Request.QueryString["roleAsPerson"] + ";" + Request.QueryString["processkey"] + ";";
			}
		}

		protected void RolePropertyEdit_PrepareDownloadStream(object sender, PrepareDownloadStreamEventArgs args)
		{
			bool roleAsPerson = false;
			string[] itemsInfio = args.DownloadInfo.RequestContext.Split(';');
			Boolean.TryParse(itemsInfio[1], out roleAsPerson);

			string downloadkey = itemsInfio[0];
			WfMatrix matrix = WfMatrixAdapter.Instance.Load(downloadkey);
			matrix.Loaded = true;

			using (MemoryStream excelStream = matrix.ExportToExcel2007(roleAsPerson))
			{
				excelStream.CopyTo(args.OutputStream);
			}
		}

		protected void btnServerSave_Click(object sender, EventArgs e)
		{
			int count = this.MCMatrixEditTemplate.Materials.Count;
			if (count > 0 && !this.MCMatrixEditTemplate.DeltaMaterials.IsEmpty())
			{
				Material keyTerial = null;
				if (this.MCMatrixEditTemplate.DeltaMaterials.Inserted.Count > 0)
				{
					keyTerial = this.MCMatrixEditTemplate.DeltaMaterials.Inserted[this.MCMatrixEditTemplate.DeltaMaterials.Inserted.Count - 1];
				}
				else if (this.MCMatrixEditTemplate.DeltaMaterials.Updated.Count > 0)
				{
					keyTerial = this.MCMatrixEditTemplate.DeltaMaterials.Updated[this.MCMatrixEditTemplate.DeltaMaterials.Updated.Count - 1];
				}

				if (keyTerial != null)
				{
					string[] itemsInfio = this.MCMatrixEditTemplate.RequestContext.Split(';');
					string processKey = itemsInfio[2];
					using (Stream LoadFile = keyTerial.GetTemporaryContent(this.MCMatrixEditTemplate.RootPathName))
					{
						WfMatrix.ImportExistMatrixFromExcel2007(LoadFile, null, processKey);
					}
				}
			}
		}
	}
}