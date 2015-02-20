using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MCS.Library.Core;
using MCS.Web.Library.Script;
using MCS.Web.Library;

namespace MCS.Web.WebControls
{
	public class SubmitButtonProgressResponser : IProcessProgressResponser
	{
		public static readonly SubmitButtonProgressResponser Instance = new SubmitButtonProgressResponser();

		private SubmitButtonProgressResponser()
		{
		}

		public void Register(ProcessProgress progress)
		{
			JSONSerializerExecute.RegisterConverter(typeof(ProcessProgressConverter));

			progress.MinStep = 0;
			progress.MaxStep = 100;
			progress.CurrentStep = 1;
			progress.StatusText = string.Empty;
		}

		public void Response(ProcessProgress progress)
		{
			HttpResponse response = HttpContext.Current.Response;

			string script = SubmitButton.GetChangeParentProcessInfoScript(progress, true);
			response.Write(script);

			response.Flush();
		}
	}
}
