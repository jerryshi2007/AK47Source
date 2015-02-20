using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
   /// <summary>
	/// Activity变化的上下文
	/// </summary>
    [DataContract]
    [Serializable]
	public class WfClientActivityChangingContext
	{
        private string _AssociatedActivityKey = string.Empty;
        private string _CreatorInstanceID = string.Empty;

		internal WfClientActivityChangingContext()
		{
		}

        [DataMember]
        public string AssociatedActivityKey
        {
            get { return this._AssociatedActivityKey; }
            set { this._AssociatedActivityKey = value; }
        }

        [DataMember]
		public string CreatorInstanceID
		{
            get { return this._CreatorInstanceID; }
            set { this._AssociatedActivityKey = value; }
		}
	}
}
