using MCS.Library.Core;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Json.Converters
{
    public class WfClientAuthorizationInfoJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientAuthorizationInfo) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientAuthorizationInfo auInfo = new WfClientAuthorizationInfo();

            auInfo.OriginalActivityID = dictionary.GetValue("originalActivityID", string.Empty);
            auInfo.UserID = dictionary.GetValue("userID", string.Empty);
            auInfo.IsProcessAdmin = dictionary.GetValue("isProcessAdmin", false);
            auInfo.IsProcessViewer = dictionary.GetValue("isProcessViewer", false);
            auInfo.InMoveToMode = dictionary.GetValue("inMoveToMode", false);
            auInfo.IsInAcl = dictionary.GetValue("isInAcl", false);

            return auInfo;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            WfClientAuthorizationInfo auInfo = (WfClientAuthorizationInfo)obj;

            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            dictionary.AddNonDefaultValue("originalActivityID", auInfo.OriginalActivityID);
            dictionary.AddNonDefaultValue("userID", auInfo.UserID);
            dictionary.AddNonDefaultValue("isProcessAdmin", auInfo.IsProcessAdmin);
            dictionary.AddNonDefaultValue("isProcessViewer", auInfo.IsProcessViewer);
            dictionary.AddNonDefaultValue("inMoveToMode", auInfo.InMoveToMode);
            dictionary.AddNonDefaultValue("isInAcl", auInfo.IsInAcl);

            return dictionary;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return _SupportedTypes;
            }
        }
    }
}
