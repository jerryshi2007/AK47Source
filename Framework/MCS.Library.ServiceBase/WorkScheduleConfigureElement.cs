using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace MCS.Library.Services
{
	public class WorkScheduleConfigureElement : ConfigurationElement
	{
		[ConfigurationProperty("checkPoints", IsRequired = true)]
		public string CheckPoints
		{
			get
			{
				return (string)this["checkPoints"];
			}
		}

		[ConfigurationProperty("timeScope", DefaultValue = "00:00:10")]
		public TimeSpan TimeScope
		{
			get
			{
				return (TimeSpan)this["timeScope"];
			}
		}

		[ConfigurationProperty("enabled", IsRequired = false, DefaultValue = false)]
		public bool Enabled
		{
			get
			{
				return (bool)this["enabled"];
			}
		}

		/// <summary>
		/// �ж��Ƿ��ѵ�����ִ�е�
		/// </summary>
		/// <returns></returns>
		public bool IsCheckPointNeared()
		{
			if (this.GetNearestDailyTimes(this.CheckPoints, this.TimeScope) != DateTime.MinValue)
				return true;
			else
				return false;
		}

		/// <summary>
		/// ��ȡָ����ϵ��ʱ����У��ڵ�ǰʱ��һ����Χǰ��ģ�������Ǹ�ʱ���
		/// </summary>
		/// <param name="strTimes">ָ����ʱ��㴮����","����";"�ָ���</param>
		/// <param name="timeScope">ʱ�䷶Χ</param>
		/// <returns>����������ʱ��㣬���û���򷵻�DateTime.MinValue</returns>
		private DateTime GetNearestDailyTimes(string strTimes, TimeSpan timeScope)
		{
			DateTime result = DateTime.MinValue;

			if (string.IsNullOrEmpty(strTimes) == false)
			{
				if (string.Compare(strTimes, "now", true) != 0)
				{
					string[] timeArray = strTimes.Split(',', ';');

					DateTime dtCurrent = DateTime.Now;

					for (int i = 0; i < timeArray.Length; i++)
					{
						DateTime dtDefined = GetDateTimeFromTimeString(timeArray[i]);

						if (dtDefined > dtCurrent.AddSeconds(-timeScope.TotalSeconds) && dtDefined <= dtCurrent.AddSeconds(timeScope.TotalSeconds)) //�ڵ�ǰʱ���ǰn����(�൱��ÿ�����㷶Χ�ڵ�ǰʱ������)
							if (dtDefined > result) //ȡһ���뵱ǰʱ����ӽ���ʱ���
								result = dtDefined;
					}
				}
				else
					result = DateTime.Now;
			}

			return result;
		}

		/// <summary>
		/// ���ַ���ʱ��strTime��Ӧת��ΪDateTimeʱ�����
		/// </summary>
		/// <param name="?"></param>
		/// <returns></returns>
		private DateTime GetDateTimeFromTimeString(string strTime)
		{
			try
			{
				strTime = strTime.Trim();

				int nYear = DateTime.Now.Year;
				int nMonth = DateTime.Now.Month;
				int nDay = DateTime.Now.Day;

				string[] timePart = strTime.Split(':');

				int nHour = GetTimePart(timePart, 0);
				int nMin = GetTimePart(timePart, 1);
				int nSecond = GetTimePart(timePart, 2);

				return new DateTime(nYear, nMonth, nDay, nHour, nMin, nSecond);
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("ʱ���ʽ����\"{0}\"", strTime), ex);
			}
		}

		/// <summary>
		/// ��ȡtimePart�ַ��������еĵ�nIndex�����ݣ�ʱ���֡��룩
		/// </summary>
		/// <param name="timePart"></param>
		/// <param name="nIndex"></param>
		/// <returns></returns>
		private int GetTimePart(string[] timePart, int nIndex)
		{
			int nTime = 0;

			if (nIndex < timePart.Length)
				nTime = int.Parse(timePart[nIndex]);

			return nTime;
		}
	}
}
