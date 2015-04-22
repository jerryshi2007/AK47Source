using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.HtmlParser
{
    /// <summary>
    /// Style中，属性值的表达式。例如属性值为width: expression(6 * 7)，那么Expression为expression，6*7为Value
    /// </summary>
    [Serializable]
    public class HtmlStyleValueExpression
    {
        /// <summary>
        /// 
        /// </summary>
        public string Expression
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Value
        {
            get;
            set;
        }
    }
}
