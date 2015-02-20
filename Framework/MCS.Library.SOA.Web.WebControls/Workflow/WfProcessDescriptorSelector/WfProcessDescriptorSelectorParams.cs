using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Web.Library;

namespace MCS.Web.WebControls
{
    [Serializable]
    public class WfProcessDescriptorSelectorParams :DialogControlParamsBase
    {
        private bool multiSelect = false;
        /// <summary>
        /// 是否多选
        /// </summary>
        public bool MultiSelect
        {
            get
            {
                return this.multiSelect;
            }
            set
            {
                this.multiSelect = value;
            }
        }

        protected override void BuildRequestParams(StringBuilder strB)
        {
            AppendNotNullStringParam(strB, "multiSelect", MultiSelect.ToString());

            base.BuildRequestParams(strB);
        }

        public override void LoadDataFromQueryString()
        {
            this.multiSelect = WebUtility.GetRequestQueryValue("multiSelect", false);

            base.LoadDataFromQueryString();

        }

    }
}
