using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Ogu;

namespace MCS.Library.WF.Contracts.Converters.DataObjects
{
	public class WfClientOpinionConverter
	{
		public static readonly WfClientOpinionConverter Instance = new WfClientOpinionConverter();

		private WfClientOpinionConverter()
		{
		}

		public GenericOpinion ClientToServer(WfClientOpinion client, ref GenericOpinion server)
		{
			client.NullCheck("client");

			if (server == null)
				server = new GenericOpinion();

			server.ID = client.ID;
			server.ResourceID = client.ResourceID;
			server.ActivityID = client.ActivityID;
			server.ProcessID = client.ProcessID;
			server.Content = client.Content;
			server.OpinionType = client.OpinionType;
			server.LevelName = client.LevelName;
			server.LevelDesp = client.LevelDesp;
			server.IssuePerson = new OguUser(client.IssuePersonID) { Name = client.IssuePersonName, DisplayName = client.IssuePersonName };
			server.AppendPerson = new OguUser(client.AppendPersonID) { Name = client.AppendPersonName, DisplayName = client.AppendPersonName };
			server.IssueDatetime = client.IssueTime;
			server.AppendDatetime = client.AppendTime;
			server.RawExtData = client.ExtraData;

			return server;
		}

		public List<WfClientOpinion> ServerToClient(GenericOpinionCollection server)
		{
            server.NullCheck("server");

			List<WfClientOpinion> client = new List<WfClientOpinion>();

			foreach (GenericOpinion opinion in server)
			{
				WfClientOpinion clientOpinion = null;

				ServerToClient(opinion, ref clientOpinion);

				client.Add(clientOpinion);
			}

			return client;
		}

		public WfClientOpinion ServerToClient(GenericOpinion server, ref WfClientOpinion client)
		{
			server.NullCheck("server");

			if (client == null)
				client = new WfClientOpinion();

			client.ID = server.ID;
			client.ResourceID = server.ResourceID;
			client.ActivityID = server.ActivityID;
			client.ProcessID = server.ProcessID;
			client.Content = server.Content;
			client.OpinionType = server.OpinionType;
			client.LevelName = server.LevelName;
			client.LevelDesp = server.LevelDesp;
			client.IssueTime = server.IssueDatetime;
			client.AppendTime = server.AppendDatetime;
			client.ExtraData = server.RawExtData;

			if (server.IssuePerson != null)
			{
				client.IssuePersonID = server.IssuePerson.ID;
				client.IssuePersonName = server.IssuePerson.DisplayName;
			}

			if (server.AppendPerson != null)
			{
				client.AppendPersonID = server.AppendPerson.ID;
				client.AppendPersonName = server.AppendPerson.DisplayName;
			}

			return client;
		}

		/// <summary>
		/// 生成活动相关的意见
		/// </summary>
		/// <param name="originalActivity"></param>
		/// <param name="user"></param>
		/// <param name="delegateUser"></param>
		/// <param name="content"></param>
		/// <param name="extraData"></param>
		/// <returns></returns>
		public WfClientOpinion GenerateOpinionFromActivity(IWfActivity originalActivity, WfClientUser user, WfClientUser delegateUser, string content, string extraData)
		{
			WfClientOpinion opinion = new WfClientOpinion();

			if (originalActivity != null)
			{
				opinion.ResourceID = originalActivity.Process.ResourceID;
				opinion.ProcessID = originalActivity.Process.ID;
				opinion.ActivityID = originalActivity.ID;

				IWfActivity rootActivity = originalActivity.OpinionRootActivity;

				if (rootActivity.Process.MainStream != null && rootActivity.MainStreamActivityKey.IsNotEmpty())
				{
					opinion.LevelName = rootActivity.MainStreamActivityKey;
				}
				else
				{
					if (string.IsNullOrEmpty(rootActivity.Descriptor.AssociatedActivityKey))
						opinion.LevelName = rootActivity.Descriptor.Key;
					else
						opinion.LevelName = rootActivity.Descriptor.AssociatedActivityKey;
				}

				if (rootActivity.Process.MainStream != null)
					opinion.LevelDesp = rootActivity.Process.MainStream.Activities[opinion.LevelName].Name;
				else
					opinion.LevelDesp = rootActivity.Descriptor.Process.Activities[opinion.LevelName].Name;
			}

			opinion.Content = content;
			opinion.ExtraData = extraData;

			FillPersonInfo(opinion, user, delegateUser);

			return opinion;
		}

		private static void FillPersonInfo(WfClientOpinion opinion, WfClientUser user, WfClientUser delegateUser)
		{
			if (delegateUser == null)
				delegateUser = user;

			if (user != null)
			{
				opinion.IssuePersonID = user.ID;
				opinion.IssuePersonName = user.DisplayName;

				opinion.AppendPersonID = delegateUser.ID;
				opinion.AppendPersonName = delegateUser.DisplayName;
			}
		}
	}
}
