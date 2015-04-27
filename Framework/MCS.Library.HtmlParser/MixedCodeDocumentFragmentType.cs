using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.HtmlParser
{
    /// <summary>
    /// Represents the type of fragment in a mixed code document.
    /// </summary>
    public enum MixedCodeDocumentFragmentType
    {
        /// <summary>
        /// The fragment contains code.
        /// </summary>
        Code,

        /// <summary>
        /// The fragment contains text.
        /// </summary>
        Text,
    }
}
