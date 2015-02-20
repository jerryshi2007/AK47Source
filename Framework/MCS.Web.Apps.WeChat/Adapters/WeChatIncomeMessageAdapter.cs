using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat.Adapters
{
    public class WeChatIncomeMessageAdapterBase<T, TCollection> : UpdatableAndLoadableAdapterBase<T, TCollection>
        where T : WeChatIncomeMessageBase
        where TCollection : EditableDataObjectCollectionBase<T>, new()
    {
        protected override string GetConnectionName()
        {
            return ConnectionDefine.WeChatInfoDBConnectionName;
        }

        public WeChatIncomeMessageBase Load(long messageID)
        {
            return this.Load(p => p.AppendItem("MessageID", messageID)).FirstOrDefault();
        }
    }

    public class WeChatTextIncomeMessageAdapter : WeChatIncomeMessageAdapterBase<WeChatTextIncomeMessage, WeChatTextIncomeMessageCollection>
    {
        public static readonly WeChatTextIncomeMessageAdapter Instance = new WeChatTextIncomeMessageAdapter();

        private WeChatTextIncomeMessageAdapter()
        {
        }
    }

    public class WeChatLocationIncomeMessageAdapter : WeChatIncomeMessageAdapterBase<WeChatLocationIncomeMessage, WeChatLocationIncomeMessageCollection>
    {
        public static readonly WeChatLocationIncomeMessageAdapter Instance = new WeChatLocationIncomeMessageAdapter();

        private WeChatLocationIncomeMessageAdapter()
        {
        }
    }


    public class WeChatImageIncomeMessageAdapter : WeChatIncomeMessageAdapterBase<WeChatImageIncomeMessage, WeChatImageIncomeMessageCollection>
    {
        public static readonly WeChatImageIncomeMessageAdapter Instance = new WeChatImageIncomeMessageAdapter();

        private WeChatImageIncomeMessageAdapter()
        {
        }
    }

    public class WeChatVideoIncomeMessageAdapter : WeChatIncomeMessageAdapterBase<WeChatVideoIncomeMessage, WeChatVideoIncomeMessageCollection>
    {
        public static readonly WeChatVideoIncomeMessageAdapter Instance = new WeChatVideoIncomeMessageAdapter();

        private WeChatVideoIncomeMessageAdapter()
        {
        }
    }

    public class WeChatVoiceIncomeMessageAdapter : WeChatIncomeMessageAdapterBase<WeChatVoiceIncomeMessage, WeChatVoiceIncomeMessageCollection>
    {
        public static readonly WeChatVoiceIncomeMessageAdapter Instance = new WeChatVoiceIncomeMessageAdapter();

        private WeChatVoiceIncomeMessageAdapter()
        {
        }
    }

    public class WeChatLinkIncomeMessageAdapter : WeChatIncomeMessageAdapterBase<WeChatLinkIncomeMessage, WeChatLinkIncomeMessageCollection>
    {
        public static readonly WeChatLinkIncomeMessageAdapter Instance = new WeChatLinkIncomeMessageAdapter();

        private WeChatLinkIncomeMessageAdapter()
        {
        }
    }
}
