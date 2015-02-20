using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace MCS.Library.SOA.Web.WebControls.Test.MultiProcessControl
{
	public partial class MultiProcessControlWithPrepareData : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void MutifyProcessControl1_ExecuteStep(object data)
		{
			System.Threading.Thread.Sleep(100);

			//throw new Exception("Error Testing");
		}

		protected void MultiProcessControl1_OnError(Exception ex, object data, ref bool isThrow)
		{
			isThrow = false;
			string msg = ex.Message;
			msg += data.ToString();
		}
			
		protected object MultiProcessControl1_ExecutePrepareData(object data, MCS.Web.WebControls.IMultiProcessPrepareData owner)
		{
			List<int> result = new List<int>();

			int i = 0;
			foreach (string item in (IEnumerable)data)
			{
				System.Threading.Thread.Sleep(600);

				for(int j = 0; j < 10; j++)
					result.Add(i * 10 + j);

				owner.ResponsePrepareDataInfo("正在准备数据..." + i++.ToString());
			}

			owner.ResponsePrepareDataInfo(string.Empty);

			return result.ToArray();
		}
	}
}