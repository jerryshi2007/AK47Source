using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Web.Apps.WeChat.DataObjects
{
	/// <summary>
	/// 修改组信息的返回值
	/// </summary>
	[Serializable]
	public class WeChatModifyGroupRetInfo
	{
		public int ErrCode
		{
			get;
			set;
		}

		public string ErrMsg
		{
			get;
			set;
		}

		public int GroupId
		{
			get;
			set;
		}

		public string GroupName
		{
			get;
			set;
		}

		public int MemberCnt
		{
			get;
			set;
		}

		public void CheckResult()
		{
			if (this.ErrCode != 0)
				throw new ApplicationException(this.ErrMsg);
		}

		public override string ToString()
		{
			return string.Format("ErrCode: {0}, ErrMsg: {1}, GroupID: {2}, GroupName: {3}, MemberCount: {4}",
				this.ErrCode, this.ErrMsg, this.GroupId, this.GroupName, this.MemberCnt);
		}
	}
}
