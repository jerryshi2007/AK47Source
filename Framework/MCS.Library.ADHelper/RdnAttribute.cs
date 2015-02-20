using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime;

namespace MCS.Library
{
	public struct RdnAttribute
	{
		private string ldif;
		private string stringValue;

		public string Ldif
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get { return ldif; }

			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			set { ldif = value; }
		}

		public string StringValue
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get
			{
				return this.stringValue;
			}

			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			set
			{
				this.stringValue = value;
			}
		}

		/// <summary>
		/// 将RDN转换为键值对
		/// </summary>
		/// <param stringValue="dnPart"></param>
		/// <returns></returns>
		/// <remarks>AD中不允许控制符和换行符等特殊字符，不识别键中的特殊字符</remarks>
		public static RdnAttribute Parse(string dnPart)
		{

			int eqlIndex = dnPart.IndexOf('=');
			if (eqlIndex > 0)
			{
				string k = dnPart.Substring(0, eqlIndex);
				string v = null;

				char[] buffer = dnPart.ToCharArray(eqlIndex + 1, dnPart.Length - eqlIndex - 1);

				if (buffer.Length > 0 && buffer[0] == '#')
					throw new FormatException(string.Format("{0} 的值 {1} 以#开头，这可能是一个二进制值，不应在RDN中使用。", k, new string(buffer)));

				int readIndex = 0, writeIndex = 0, len = buffer.Length;

				while (readIndex < buffer.Length)
				{
					if (buffer[readIndex] != '\\')
					{
						buffer[writeIndex++] = buffer[readIndex++];
					}
					else
					{
						//转义字符
						readIndex++;
						char c = buffer[readIndex++];
						if (AttributeHelper.IsHexDigit(c))
							throw new NotSupportedException("不支持十六进制转义");

						switch (c)
						{
							//必须转义的字符
							default:
								buffer[writeIndex++] = c;
								break;
						}
					}
				}

				v = new string(buffer, 0, writeIndex);

				return new RdnAttribute() { ldif = k, stringValue = v };
			}
			else
			{
				throw new FormatException("应该有等号来分隔属性和值");
			}
		}

		public override string ToString()
		{
			return this.ldif ?? string.Empty + "=" + ADHelper.EscapeString(this.stringValue ?? string.Empty);
		}
	}
}
