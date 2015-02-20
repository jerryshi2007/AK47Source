using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace MCS.Web.Apps.WeChat.DataObjects
{
	/// <summary>
	/// 微信应用消息
	/// </summary>
	[Serializable]
    [ORTableMapping("WeChat.AppMessages")]
	public class WeChatAppMessage
	{
		private int _Count = 1;
		private WeChatAppMessageType _MessageType = WeChatAppMessageType.Html;

        [ORFieldMapping("AppMessageID", PrimaryKey = true)]
		public int AppMessageID
		{
			get;
			set;
		}

		public WeChatAppMessageType MessageType
		{
			get
			{
				return this._MessageType;
			}
			set
			{
				this._MessageType = value;
			}
		}

		/// <summary>
		/// 多少图文
		/// </summary>
		public int Count
		{
			get
			{
				return this._Count;
			}
			set
			{
				this._Count = value;
			}
		}

		/// <summary>
		/// 标题
		/// </summary>
		public string Title
		{
			get;
			set;
		}

		/// <summary>
		/// 内容
		/// </summary>
		public string Content
		{
			get;
			set;
		}

		/// <summary>
		/// 摘要
		/// </summary>
		public string Digest
		{
			get;
			set;
		}

		/// <summary>
		/// 作者
		/// </summary>
		public string Author
		{
			get;
			set;
		}

		/// <summary>
		/// 相关链接
		/// </summary>
		public string SourceUrl
		{
			get;
			set;
		}

		/// <summary>
		/// 封面的图片文件ID
		/// </summary>
		public int FileID
		{
			get;
			set;
		}

		/// <summary>
		/// 是否在正文中显示封面
		/// </summary>
		public bool ShowCover
		{
			get;
			set;
		}

		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreateTime
		{
			get;
			set;
		}

		/// <summary>
		/// 内容的URL
		/// </summary>
		public string ContentUrl
		{
			get;
			set;
		}

		/// <summary>
		/// 封面的URL
		/// </summary>
		public string ImageUrl
		{
			get;
			set;
		}

		public override string ToString()
		{
			return string.Format("AppMsgID: {0}, FileID: {1}, Title: {2}, ImageUrl: {3}, Author: {4}",
				this.AppMessageID, this.FileID, this.Title, this.ImageUrl, this.Author);
		}
	}

	[Serializable]
	public class WeChatAppMessageCollection : EditableDataObjectCollectionBase<WeChatAppMessage>
	{
	}
}
