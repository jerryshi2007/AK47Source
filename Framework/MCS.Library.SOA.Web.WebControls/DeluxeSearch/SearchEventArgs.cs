using System;

namespace MCS.Web.WebControls
{
    public class SearchEventArgs : EventArgs
    {
		private string conditionKey;
		private object conditionValue;

		public string ConditionKey
		{
			get { return this.conditionKey; }
		}

		public object ConditionValue
		{
			get { return this.conditionValue; }
		}

        public SearchEventArgs()
        {
        }

        public SearchEventArgs(string conditionKey, object conditionValue)
        {
            this.conditionKey = conditionKey;
            this.conditionValue = conditionValue;            
        }

        public bool IsSaveCondition
        {
            get;
            set;
        }

        public string ConditionName
        {
            get;
            set;
        }
    }
}
