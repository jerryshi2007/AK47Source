﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Web.Library.Script.Mechanism
{
    internal static class Utf16StringValidator
    {
        // Fields
        private static readonly bool _skipUtf16Validation = AppSettings.AllowRelaxedUnicodeDecoding;
        private const char UNICODE_NULL_CHAR = '\0';
        private const char UNICODE_REPLACEMENT_CHAR = '�';

        // Methods
        public static string ValidateString(string input)
        {
            return ValidateString(input, _skipUtf16Validation);
        }

        internal static string ValidateString(string input, bool skipUtf16Validation)
        {
            if (skipUtf16Validation || string.IsNullOrEmpty(input))
            {
                return input;
            }
            int num = -1;
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsSurrogate(input[i]))
                {
                    num = i;
                    break;
                }
            }
            if (num < 0)
            {
                return input;
            }
            char[] chArray = input.ToCharArray();
            for (int j = num; j < chArray.Length; j++)
            {
                char c = chArray[j];
                if (char.IsLowSurrogate(c))
                {
                    chArray[j] = (char)System.Convert.ChangeType(0xfffd, typeof(char));
                }
                else if (char.IsHighSurrogate(c))
                {
                    if (((j + 1) < chArray.Length) && char.IsLowSurrogate(chArray[j + 1]))
                    {
                        j++;
                    }
                    else
                    {
                        chArray[j] = (char)System.Convert.ChangeType(0xfffd, typeof(char));
                    }
                }
            }
            return new string(chArray);
        }
    }


}
