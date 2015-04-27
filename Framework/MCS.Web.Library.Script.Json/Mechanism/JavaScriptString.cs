using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Web.Library.Script.Mechanism
{
    internal class JavaScriptString
    {
        // Fields
        private int _index;
        private string _s;

        // Methods
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal JavaScriptString(string s)
        {
            this._s = s;
        }

        internal string GetDebugString(string message)
        {
            return string.Concat(new object[] { message, " (", this._index, "): ", this._s });
        }

        internal char? GetNextNonEmptyChar()
        {
            while (this._s.Length > this._index)
            {
                char c = this._s[this._index++];

                if (!char.IsWhiteSpace(c))
                {
                    return new char?(c);
                }
            }

            return null;
        }

        internal int IndexOf(string substr)
        {
            if (this._s.Length > this._index)
            {
                return (this._s.IndexOf(substr, this._index, StringComparison.CurrentCulture) - this._index);
            }
            return -1;
        }

        internal char? MoveNext()
        {
            if (this._s.Length > this._index)
            {
                return new char?(this._s[this._index++]);
            }
            return null;
        }

        internal string MoveNext(int count)
        {
            if (this._s.Length >= (this._index + count))
            {
                string str = this._s.Substring(this._index, count);
                this._index += count;

                return str;
            }

            return null;
        }

        internal void MovePrev()
        {
            if (this._index > 0)
            {
                this._index--;
            }
        }

        internal void MovePrev(int count)
        {
            while ((this._index > 0) && (count > 0))
            {
                this._index--;
                count--;
            }
        }

        internal string Substring(int length)
        {
            if (this._s.Length > (this._index + length))
            {
                return this._s.Substring(this._index, length);
            }
            return this.ToString();
        }

        public override string ToString()
        {
            if (this._s.Length > this._index)
            {
                return this._s.Substring(this._index);
            }

            return string.Empty;
        }
    }
}
