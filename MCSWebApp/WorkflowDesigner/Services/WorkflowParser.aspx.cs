using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Compression;

namespace WorkflowDesigner.Services
{
	public partial class WorkflowParser : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void parseBtn_Click(object sender, EventArgs e)
		{
			try
			{
				PrepareUploadedFile();
				this.processDespText.Text.IsNotEmpty().FalseThrow("没有提供流程描述信息");

				XElement root = XElement.Parse(this.processDespText.Text);

				XElementFormatter formatter = new XElementFormatter();

				IWfProcess process = (IWfProcess)formatter.Deserialize(root);

				StringBuilder strB = new StringBuilder();

				using (StringWriter writer = new StringWriter(strB))
				{
					OutputProcessInfo(process, writer);
				}

				ShowMessage(strB.ToString());
			}
			catch (System.Exception ex)
			{
				ShowError(ex);
			}
			finally
			{
				NormalResultText();
			}
		}

		private void PrepareUploadedFile()
		{
			if (this.uploader.PostedFile != null)
			{
				Stream decompressedStream = CompressManager.ExtractStream(this.uploader.PostedFile.InputStream);

				decompressedStream.Seek(0, SeekOrigin.Begin);
				byte[] data = decompressedStream.ToBytes();

				if (data.Length > 0)
					this.processDespText.Text = Encoding.GetEncoding(encodingSelector.SelectedValue).GetString(data);
			}
		}

		private void OutputProcessInfo(IWfProcess process, TextWriter writer)
		{
			writer.WriteLine("ProcessID: {0}, Key: {1}, Name: {2}", process.ID, process.Descriptor.Key, process.Descriptor.Name);

			writer.WriteLine("Activities Begin");
			process.Activities.ForEach(a => OutputActivityInfo(a, writer));
			writer.WriteLine("Activities End");
		}

		private void OutputActivityInfo(IWfActivity activity, TextWriter writer)
		{
			writer.WriteLine("ActivityID: {0}, Key: {1}, Name: {2}", activity.ID, activity.Descriptor.Key, activity.Descriptor.Name);
		}

		private void ShowMessage(string message)
		{
			result.InnerText = message;
			result.Style["color"] = "black";
		}

		private void ShowError(System.Exception ex)
		{
			result.InnerText = ex.ToString();
			result.Style["color"] = "red";
		}

		private void NormalResultText()
		{
			result.InnerHtml = result.InnerHtml.Replace("\r\n", "<br/>");
		}
	}
}