using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat.Commands
{
	public class UploadFileCommand : CommandBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public UploadFileCommand(string name)
			: base(name)
		{
		}

		public override void Execute(string argument)
		{
			WeChatUploadFileRetInfo uploadedFileInfo = WeChatHelper.UploadFile(argument, WeChatRequestContext.Current.LoginInfo);

			Console.WriteLine(uploadedFileInfo.ToString());
		}

		/// <summary>
		/// 
		/// </summary>
		public override string HelperString
		{
			get
			{
				return "uploadFile {filePath}";
			}
		}
	}
}
