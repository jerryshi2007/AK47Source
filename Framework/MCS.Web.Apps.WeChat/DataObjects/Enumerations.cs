using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Web.Apps.WeChat.DataObjects
{
	/// <summary>
	/// 微信消息类型
	/// </summary>
	public enum WeChatMessageType
	{
		[EnumItemDescription("未知类型")]
		None = 0,

		[EnumItemDescription("文本消息")]
		Text = 1,

		[EnumItemDescription("图片消息")]
		Image = 2,

		[EnumItemDescription("语音消息")]
		Voice = 3,

		[EnumItemDescription("视频消息")]
		Video = 4,

		[EnumItemDescription("地理位置消息")]
		Location = 5,

		[EnumItemDescription("链接消息")]
		Link = 6,
	}

	/// <summary>
	/// 素材库中的内容类型
	/// </summary>
	public enum WeChatAppMessageType
	{
		[EnumItemDescription("未知类型")]
		None = 0,

		[EnumItemDescription("文字")]
		Text = 1,

		[EnumItemDescription("图片")]
		Image = 2,

		[EnumItemDescription("语音")]
		Voice = 3,

		[EnumItemDescription("图文消息")]
		Html = 10,

		[EnumItemDescription("视频")]
		Video = 15,
	}
}
