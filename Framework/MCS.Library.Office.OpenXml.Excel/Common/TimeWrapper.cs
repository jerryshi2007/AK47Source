using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Globalization;

namespace MCS.Library.Office.OpenXml.Excel
{
	/// <summary>
	/// 表示在00:00:00和23:59:59之间时间
	/// </summary>
	public sealed class TimeWrapper
	{
		//private event EventHandler _TimeChanged;
		private readonly decimal _SecondsPerDay = 3600 * 24;
		private readonly decimal _SecondsPerHour = 3600;
		private readonly decimal _SecondsPerMinute = 60;

		public const int NumberOfDecimals = 15;

		public TimeWrapper()
		{

		}

		public TimeWrapper(decimal value)
		{
			ExceptionHelper.TrueThrow(value < 0M || value >= 1M, "值不能小于0,或者值不能大于等于1");

			Init(value);
		}

		private void Init(decimal value)
		{
			// handle hour
			decimal totalSeconds = value * _SecondsPerDay;
			decimal hour = Math.Floor(totalSeconds / _SecondsPerHour);
			Hour = (int)hour;

			// handle minute
			decimal remainingSeconds = totalSeconds - ((decimal)Hour * _SecondsPerHour);
			decimal minute = Math.Floor(remainingSeconds / _SecondsPerMinute);
			Minute = (int)minute;

			// handle second
			remainingSeconds = totalSeconds - ((decimal)Hour * _SecondsPerHour) - ((decimal)Minute * _SecondsPerMinute);
			decimal second = Math.Round(remainingSeconds, MidpointRounding.AwayFromZero);
			// Second might be rounded to 60... the SetSecond method handles that.
			SetSecond((int)second);
		}

		private void SetSecond(int value)
		{
			if (value == 60)
			{
				Second = 0;
				int minute = Minute + 1;
				SetMinute(minute);
			}
			else
			{
				Second = value;
			}
		}

		private void SetMinute(int value)
		{
			if (value == 60)
			{
				Minute = 0;
				var hour = Hour + 1;
				SetHour(hour);
			}
			else
			{
				Minute = value;
			}
		}

		private void SetHour(int value)
		{
			if (value == 24)
			{
				Hour = 0;
			}
		}

		//internal event EventHandler TimeChanged
		//{
		//    add { this._TimeChanged += value; }
		//    remove { this._TimeChanged -= value; }
		//}

		//private void OnTimeChanged()
		//{
		//    if (this._TimeChanged != null)
		//    {
		//        this._TimeChanged(this, EventArgs.Empty);
		//    }
		//}

		private int _Hour;
		public int Hour
		{
			get
			{
				return this._Hour;
			}
			set
			{
				ExceptionHelper.TrueThrow(value < 0 || value > 23, "值只能在0到23之间");
				this._Hour = value;
				//OnTimeChanged();
			}
		}

		private int _Minute;
		public int Minute
		{
			get
			{
				return this._Minute;
			}
			set
			{
				ExceptionHelper.TrueThrow(value < 0 || value > 59, "值只能在0到23之间");
				this._Minute = value;
				//OnTimeChanged();
			}
		}

		private int? _Second;
		public int? Second
		{
			get
			{
				return this._Second;
			}
			set
			{
				ExceptionHelper.TrueThrow(value < 0 || value > 59, "值只能在0到23之间");
				this._Second = value;
				//OnTimeChanged();
			}
		}

		private decimal Round(decimal value)
		{
			return Math.Round(value, NumberOfDecimals);
		}

		private decimal ToSeconds()
		{
			decimal result = Hour * _SecondsPerHour;
			result += Minute * _SecondsPerMinute;
			result += Second ?? 0;
			return result;
		}

		public decimal ToExcelTime()
		{
			decimal seconds = ToSeconds();
			return Round(seconds / (decimal)_SecondsPerDay);
		}

		public string ToExcelString()
		{
			return ToExcelTime().ToString(CultureInfo.InvariantCulture);
		}

		public override string ToString()
		{
			var second = Second ?? 0;
			return string.Format("{0}:{1}:{2}",
				Hour < 10 ? "0" + Hour.ToString() : Hour.ToString(),
				Minute < 10 ? "0" + Minute.ToString() : Minute.ToString(),
				second < 10 ? "0" + second.ToString() : second.ToString());
		}
	}
}
