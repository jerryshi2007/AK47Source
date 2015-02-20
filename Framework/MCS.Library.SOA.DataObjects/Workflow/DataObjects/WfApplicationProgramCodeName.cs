using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	public struct WfApplicationProgramCodeName
	{
		public WfApplicationProgramCodeName(string key)
		{
			key.CheckStringIsNullOrEmpty("key");

			string[] parts = key.Split('~');

			this.ApplicationCodeName = parts[0];

			if (parts.Length > 1)
				this.ProgramCodeName = parts[1];
			else
				this.ProgramCodeName = string.Empty;
		}

		public WfApplicationProgramCodeName(string appCodeName, string programName)
		{
			this.ApplicationCodeName = appCodeName;
			this.ProgramCodeName = programName;
		}

		public string ApplicationCodeName;

		public string ProgramCodeName;

		public static string ToKey(string appCodeName, string progCodeName)
		{
			if (appCodeName == null)
				appCodeName = string.Empty;

			if (progCodeName == null)
				progCodeName = string.Empty;

			return (appCodeName + "~" + progCodeName).ToLower();
		}
	}
}
