using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat.Commands
{
	public class UpdateAppMessageCommand : CommandBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public UpdateAppMessageCommand(string name)
			: base(name)
		{
		}

		public override void Execute(string argument)
		{
			WeChatAppMessage message = PrepareMessage(argument);

			WeChatAppMessage responseMessage = WeChatHelper.UpdateAppMessage(message, WeChatRequestContext.Current.LoginInfo);

			Console.WriteLine(responseMessage.ToString());
		}

		/// <summary>
		/// 
		/// </summary>
		public override string HelperString
		{
			get
			{
				return "updateMessage {title} {fileID/fileName} {content} {author}";
			}
		}

		private static WeChatAppMessage PrepareMessage(string argument)
		{
			string[] parts = argument.Split(' ');

			WeChatAppMessage result = new WeChatAppMessage();

			result.Title = GetArgumentByIndex(parts, 0);

			string fileInfo = GetArgumentByIndex(parts, 1);
			int fileID = 0;

			if (int.TryParse(fileInfo, out fileID))
				result.FileID = fileID;
			else
				result.FileID = WeChatHelper.UploadFile(fileInfo, WeChatRequestContext.Current.LoginInfo).Content;
			
			result.Content = GetArgumentByIndex(parts, 2);
			result.Author = GetArgumentByIndex(parts, 3);

			return result;
		}

		private static string GetArgumentByIndex(string[] parts, int index)
		{
			string result = string.Empty;

			if (index < parts.Length)
				result = parts[index];

			return result;
		}
	}
}
