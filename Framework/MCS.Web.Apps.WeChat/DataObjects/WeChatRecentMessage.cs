using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace MCS.Web.Apps.WeChat.DataObjects
{
    /// <summary>
    /// 微信账号的最近消息
    /// </summary>
    [Serializable]
    [ORTableMapping("WeChat.RecentMessages")]
    public class WeChatRecentMessage
    {
        /// <summary>
        /// 微信公众号的ID
        /// </summary>
        [ORFieldMapping("AccountID", PrimaryKey = true)]
        public string AccountID
        {
            get;
            set;
        }

        [ORFieldMapping("MessageID", PrimaryKey = true)]
        public long MessageID
        {
            get;
            set;
        }

        /// <summary>
        /// 1是文字，3是语音
        /// </summary>
        [ORFieldMapping("MessageType")]
        [SqlBehavior(EnumUsageTypes.UseEnumString)]
        public WeChatMessageType MessageType
        {
            get;
            set;
        }

        /// <summary>
        /// 发送者的FakeID
        /// </summary>
        public string FakeID
        {
            get;
            set;
        }

        /// <summary>
        /// 发送者的昵称
        /// </summary>
        public string NickName
        {
            get;
            set;
        }

        /// <summary>
        /// 接收者的FakeID
        /// </summary>
        public string ToFakeID
        {
            get;
            set;
        }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SentTime
        {
            get;
            set;
        }

        /// <summary>
        /// 发送内容
        /// </summary>
        public string Content
        {
            get;
            set;
        }

        public string Source
        {
            get;
            set;
        }

        public int MessageStatus
        {
            get;
            set;
        }

        public bool HasReply
        {
            get;
            set;
        }

        /// <summary>
        /// 拒绝原因
        /// </summary>
        public string RefuseReason
        {
            get;
            set;
        }

        public override string ToString()
        {
            return string.Format("ID: {0}, FakeID: {1}, NickName: {2}, ToFakeID: {3}, SentTime: {4}, MsgType: {5}, Content: {6}",
                this.MessageID, this.FakeID, this.NickName, this.ToFakeID, this.SentTime, this.MessageType, this.Content);
        }
    }

    [Serializable]
    public class WeChatRecentMessageCollection : EditableDataObjectCollectionBase<WeChatRecentMessage>
    {
    }
}
