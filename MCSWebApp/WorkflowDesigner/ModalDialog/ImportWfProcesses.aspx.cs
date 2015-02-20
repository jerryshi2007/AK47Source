using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.IO;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Library.Principal;
using System.Transactions;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects;


namespace WorkflowDesigner.ModalDialog
{
	public partial class ImportWfProcesses : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			//ttaWfProcess.Value = prepareTestData();
		}

		protected void confirmButton_Click(object sender, EventArgs e)
		{
			string sWfProcess = ttaWfProcess.Value;
			ExceptionHelper.CheckStringIsNullOrEmpty(sWfProcess, "流程模板的XML描述信息");

			XElementFormatter formatter = new XElementFormatter();
			try
			{
				var xmlDoc = XDocument.Load(new StringReader(sWfProcess));
				var wfProcesses = xmlDoc.Descendants("Root");
				foreach (var wfProcess in wfProcesses)
				{
					IWfProcessDescriptor wfProcessDesc = (IWfProcessDescriptor)formatter.Deserialize(wfProcess);
					saveWfProcess(wfProcessDesc);
				}
				//Page.ClientScript.RegisterStartupScript(this.GetType(), "returnRole",
				//string.Format("window.returnValue = 'resultData'; top.close();"),
				//true);

				WebUtility.ResponseShowClientMessageScriptBlock("保存成功！", string.Empty, "提示");
				WebUtility.ResponseCloseWindowScriptBlock();

			}
			catch (Exception ex)
			{
				WebUtility.ResponseShowClientErrorScriptBlock(ex.Message, ex.StackTrace, "错误");
			}
			finally
			{
				Response.End();
			}
		}

		private void saveWfProcess(IWfProcessDescriptor wfProcess)
		{
			try
			{
				using (TransactionScope scope = TransactionScopeFactory.Create())
				{
					var pManager = WorkflowSettings.GetConfig().GetDescriptorManager();
					pManager.SaveDescriptor(wfProcess);

					//change import time
                    WfProcessDescriptorInfoAdapter.Instance.UpdateImportTime(wfProcess.Key, DeluxeIdentity.CurrentUser);

					//write log
					UserOperationLog log = new UserOperationLog()
					{
						ResourceID = wfProcess.Key,
						Operator = DeluxeIdentity.CurrentUser,
						OperationDateTime = DateTime.Now,
						Subject = "导入流程模板",
						OperationName = "导入",
						OperationDescription = "导入流程模板"
					};
					UserOperationLogAdapter.Instance.Update(log);

					scope.Complete();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}

		//private string prepareTestData()
		//{
		//    string sXML = "";
		//    WfProcessDescriptor wfProcessDesc = (WfProcessDescriptor)CreateSimpleProcessDescriptor();
		//    XElementFormatter formatter = new XElementFormatter();

		//    XElement root = formatter.Serialize(wfProcessDesc);
		//    sXML = root.ToString();
		//    return sXML;
		//}

		//public static IWfProcessDescriptor CreateSimpleProcessDescriptor()
		//{
		//    WfProcessDescriptor processDesp = new WfProcessDescriptor();

		//    processDesp.Key = "TestProcess";
		//    processDesp.Name = "测试流程";
		//    processDesp.ApplicationName = "TEST_APP_NAME";
		//    processDesp.ProgramName = "TEST_PROGRAM_NAME";
		//    processDesp.Url = "/MCS_Framework/WebTestProject/defaultHandler.aspx";

		//    WfActivityDescriptor initAct = new WfActivityDescriptor("Initial");
		//    initAct.Name = "Initial";
		//    initAct.CodeName = "Initial Activity";
		//    initAct.ActivityType = WfActivityType.InitialActivity;

		//    processDesp.Activities.Add(initAct);

		//    WfActivityDescriptor normalAct = new WfActivityDescriptor("NormalActivity");
		//    normalAct.Name = "Normal";
		//    normalAct.CodeName = "Normal Activity";
		//    normalAct.ActivityType = WfActivityType.NormalActivity;

		//    processDesp.Activities.Add(normalAct);

		//    WfActivityDescriptor completedAct = new WfActivityDescriptor("Completed");
		//    completedAct.Name = "Completed";
		//    completedAct.CodeName = "Completed Activity";
		//    completedAct.ActivityType = WfActivityType.CompletedActivity;

		//    processDesp.Activities.Add(completedAct);

		//    initAct.ToTransitions.AddForwardTransition(normalAct);
		//    normalAct.ToTransitions.AddForwardTransition(completedAct);
		//    //initAct.ToTransitions.AddForwardTransition(completedAct);

		//    return processDesp;
		//}
	}
}