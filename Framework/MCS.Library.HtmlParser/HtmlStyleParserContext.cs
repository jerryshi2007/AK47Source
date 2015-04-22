using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.HtmlParser
{
    /// <summary>
    /// 
    /// </summary>
    public class HtmlStyleParserContext : HtmlStyleParserContextBase<HtmlStyleParsingStage>
    {
        /// <summary>
        /// 
        /// </summary>
        public HtmlStyleParserContext() : base()
        {
            this.Stage = HtmlStyleParsingStage.None;
        }
    }
}
