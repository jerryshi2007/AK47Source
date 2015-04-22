using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.HtmlParser
{
    /// <summary>
    /// 
    /// </summary>
    public class HtmlStyleValueParserContext : HtmlStyleParserContextBase<HtmlStyleValueParsingStage>
    {
        /// <summary>
        /// 
        /// </summary>
        public HtmlStyleValueParserContext()
            : base()
        {
            this.Stage = HtmlStyleValueParsingStage.None;
            this.ParenthesesLevel = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public int ParenthesesLevel
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string ResetWriter()
        {
            this.ParenthesesLevel = 0;

            return base.ResetWriter();
        }
    }
}
