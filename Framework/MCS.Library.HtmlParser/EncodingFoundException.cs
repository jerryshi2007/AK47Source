using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.HtmlParser
{
    internal class EncodingFoundException : Exception
    {
        #region Fields

        private Encoding _encoding;

        #endregion

        #region Constructors

        internal EncodingFoundException(Encoding encoding)
        {
            _encoding = encoding;
        }

        #endregion

        #region Properties

        internal Encoding Encoding
        {
            get { return _encoding; }
        }

        #endregion
    }
}
