using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Logging;
using System.Diagnostics;
using Microsoft.SharePoint.Client;

namespace MCS.Library.Services.Log
{
	public class MossListLogFormatter : MCS.Library.Logging.LogFormatter
	{
		public override string Format(Logging.LogEntity log)
		{
			throw new NotImplementedException();
		}
	}

	public class MossListLogListener : FormattedTraceListenerWrapperBase
	{
		public MossListLogListener()
		{

		}

		public MossListLogListener(LoggerListenerConfigurationElement element)
		{

		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			if (data is LogEntity)
			{
				using (DocLibContext clientContext = new DocLibContext())
				{
					MossServerInfoConfigurationSettings section =
						MossServerInfoConfigurationSettings.GetConfig();

					MossServerInfoConfigurationElement document = section.Servers["documentServer"];
					if (document != null)
					{
						ListCollection lists = clientContext.Web.Lists;
						clientContext.Load(lists);
						List logList = lists.GetByTitle(document.LogListName);
						clientContext.Load(logList);
						clientContext.ExecuteQuery();
						ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
						ListItem oListItem = logList.AddItem(itemCreateInfo);
						oListItem["Title"] = ((LogEntity)data).Title;
						oListItem["operationInfo"] = ((LogEntity)data).Message;
						oListItem.Update();
						//clientContext.Load(itemCreateInfo);
						clientContext.ExecuteQuery();
					}

				}
			}
			else
			{
				base.TraceData(eventCache, source, eventType, id, data);
			}
		}

		public override void Flush()
		{

		}
	}
}