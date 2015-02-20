using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MCS.Library.SOA.DocServiceContract.DataObjects
{
    public interface IGeneralParser
    {
        object Parse(string strToParse);
    }

    public class GeneralParserService : Dictionary<Type, IGeneralParser>
    {
        public new IGeneralParser this[Type t]
        {
            get
            {
                if (base.ContainsKey(t))
                    return base[t];
                throw new NotSupportedException(string.Format("无法转换类型{0}:未提供支持.", t.FullName));
            }
            set
            {
                base[t] = value;
            }
        }

        public object Parse(string str, Type t)
        {
            return this[t].Parse(str);
        }

        public static GeneralParserService Default
        {
            get
            {
                GeneralParserService service = new GeneralParserService();
                service[typeof(int)] = new IntParser();
                service[typeof(int?)] = new NullableIntParser();
                service[typeof(long)] = new LongParser();
                service[typeof(long?)] = new NullableLongParser();
                service[typeof(float)] = new FloatParser();
                service[typeof(float?)] = new NullableFloatParser();
                service[typeof(double)] = new DoubleParser();
                service[typeof(double?)] = new NullableDoubleParser();
                service[typeof(decimal)] = new DecimalParser();
                service[typeof(decimal?)] = new NullableDecimalParser();
                service[typeof(DateTime)] = new DateTimeParser();
                service[typeof(DateTime?)] = new NullableDateTimeParser();
                service[typeof(string)] = new StringParser();
                return service;
            }
        }
    }

    public abstract class GeneralParser<T> : IGeneralParser
    {
        public object Parse(string strToParse)
        {
            return (object)DoParse(strToParse);
        }

        protected abstract T DoParse(string strToParse);

    }

    public class StringParser : GeneralParser<string>
    {

        protected override string DoParse(string strToParse)
        {
            return strToParse;
        }
    }

    public class IntParser : GeneralParser<int>
    {
        protected override int DoParse(string strToParse)
        {
            int result;
            int.TryParse(strToParse, System.Globalization.NumberStyles.Any, Thread.CurrentThread.CurrentCulture, out result);
            return result;
        }
    }

    public class NullableIntParser : GeneralParser<int?>
    {
        protected override int? DoParse(string strToParse)
        {
            int result;
            if (int.TryParse(strToParse, System.Globalization.NumberStyles.Any, Thread.CurrentThread.CurrentCulture, out result))
                return result;
            return null;
        }
    }

    public class LongParser : GeneralParser<long>
    {
        protected override long DoParse(string strToParse)
        {
            long result = 0;
            long.TryParse(strToParse, System.Globalization.NumberStyles.Any, Thread.CurrentThread.CurrentCulture, out result);
            return result;
        }
    }

    public class NullableLongParser : GeneralParser<long?>
    {
        protected override long? DoParse(string strToParse)
        {
            long result = 0;
            if (long.TryParse(strToParse, System.Globalization.NumberStyles.Any, Thread.CurrentThread.CurrentCulture, out result))
                return result;
            return null;
        }
    }

    public class FloatParser : GeneralParser<float>
    {
        protected override float DoParse(string strToParse)
        {
            float result = 0;
            float.TryParse(strToParse, System.Globalization.NumberStyles.Any, Thread.CurrentThread.CurrentCulture, out result);
            return result;
        }
    }

    public class NullableFloatParser : GeneralParser<float?>
    {
        protected override float? DoParse(string strToParse)
        {
            float result = 0;
            if (float.TryParse(strToParse, System.Globalization.NumberStyles.Any, Thread.CurrentThread.CurrentCulture, out result))
                return result;
            return null;
        }
    }

    public class DoubleParser : GeneralParser<double>
    {
        protected override double DoParse(string strToParse)
        {
            double result = 0;
            double.TryParse(strToParse, System.Globalization.NumberStyles.Any, Thread.CurrentThread.CurrentCulture, out result);
            return result;
        }
    }

    public class NullableDoubleParser : GeneralParser<double?>
    {
        protected override double? DoParse(string strToParse)
        {
            double result = 0;
            if (double.TryParse(strToParse, System.Globalization.NumberStyles.Any, Thread.CurrentThread.CurrentCulture, out result))
                return result;
            return null;
        }
    }

    public class DecimalParser : GeneralParser<decimal>
    {
        protected override decimal DoParse(string strToParse)
        {
            decimal result = 0;
            decimal.TryParse(strToParse, System.Globalization.NumberStyles.Any, Thread.CurrentThread.CurrentCulture, out result);
            return result;
        }
    }

    public class NullableDecimalParser : GeneralParser<decimal?>
    {
        protected override decimal? DoParse(string strToParse)
        {
            decimal result = 0;
            if (decimal.TryParse(strToParse, System.Globalization.NumberStyles.Any, Thread.CurrentThread.CurrentCulture, out result))
                return result;
            return null;
        }
    }

    public class DateTimeParser : GeneralParser<DateTime>
    {
        protected override DateTime DoParse(string strToParse)
        {
            DateTime result = DateTime.MinValue;
            DateTime.TryParse(strToParse, Thread.CurrentThread.CurrentCulture, System.Globalization.DateTimeStyles.AllowInnerWhite | System.Globalization.DateTimeStyles.AllowLeadingWhite | System.Globalization.DateTimeStyles.AllowTrailingWhite, out result);
            return result;
        }
    }

    public class NullableDateTimeParser : GeneralParser<DateTime?>
    {
        protected override DateTime? DoParse(string strToParse)
        {
            DateTime result = DateTime.MinValue;
            if (DateTime.TryParse(strToParse, Thread.CurrentThread.CurrentCulture, System.Globalization.DateTimeStyles.AllowInnerWhite | System.Globalization.DateTimeStyles.AllowLeadingWhite | System.Globalization.DateTimeStyles.AllowTrailingWhite, out result))
                return result;
            return null;
        }
    }
}
