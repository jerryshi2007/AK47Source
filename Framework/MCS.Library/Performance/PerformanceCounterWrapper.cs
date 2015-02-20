#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	PerformanceCounterWrapper.cs
// Remark	��	����Ӧ�û����ܼ������İ�װ�࣬�����ʼ�����ܼ�����ʧ�ܣ����Ժ��Ե����в��� 
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace MCS.Library.Core
{
    /// <summary>
    /// ���ܼ������İ�װ�࣬�����ʼ�����ܼ�����ʧ�ܣ����Ժ��Ե����в���
    /// </summary>
    public class PerformanceCounterWrapper
    {
        private PerformanceCounter counter = null;

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="data">���ܼ������ĳ�ʼ������</param>
        public PerformanceCounterWrapper(PerformanceCounterInitData data)
        {
            InitPerformanceCounter(data);
        }

        /// <summary>
        /// ���ܼ�������һ
        /// </summary>
        /// <returns>��һ֮��ļ�����ֵ</returns>
        public long Increment()
        {
            long result = 0;

            if (this.counter != null)
                result = this.counter.Increment();

            return result;
        }

        /// <summary>
        /// ���ܼ���������һ������
        /// </summary>
        /// <param name="value">��Ҫ���ӵ�����</param>
        /// <returns>����֮��ļ�����ֵ</returns>
        public long IncrementBy(long value)
        {
            long result = 0;

            if (this.counter != null)
                result = this.counter.IncrementBy(value);

            return result;
        }

        /// <summary>
        /// ���ܼ�����ֵ��һ
        /// </summary>
        /// <returns>��һ֮��ļ�����ֵ</returns>
        public long Decrement()
        {
            long result = 0;

            if (this.counter != null)
                this.counter.Decrement();

            return result;
        }

        /// <summary>
        /// ��ȡ���ܼ�������һ����ֵ
        /// </summary>
        /// <returns></returns>
        public float NextValue()
        {
            float result = 0;

            if (this.counter != null)
                result = this.counter.NextValue();

            return result;
        }

        /// <summary>
        /// ��ȡ���ܼ�������һ������
        /// </summary>
        /// <returns></returns>
        public CounterSample NextSample()
        {
            CounterSample sample = new CounterSample();

            if (this.counter != null)
                sample = this.counter.NextSample();

            return sample;
        }

        /// <summary>
        /// ��ȡ���������ܼ�������ԭʼֵ
        /// </summary>
        public long RawValue
        {
            get
            {
                long result = 0;

                if (this.counter != null)
                    result = this.counter.RawValue;

                return result;
            }
            set
            {
                if (this.counter != null)
                    this.counter.RawValue = value;
            }
        }

        [DebuggerNonUserCode]
        private void InitPerformanceCounter(PerformanceCounterInitData data)
        {
            PerformanceCounter pc;

            ExceptionHelper.FalseThrow<ArgumentNullException>(data != null, "data");
            ExceptionHelper.CheckStringIsNullOrEmpty(data.CategoryName, "CategoryName");
            ExceptionHelper.CheckStringIsNullOrEmpty(data.CounterName, "CounterName");

            try
            {
                if (string.IsNullOrEmpty(data.MachineName))
                    pc = new PerformanceCounter(data.CategoryName, data.CounterName, data.InstanceName, data.Readonly);
                else
                    pc = new PerformanceCounter(data.CategoryName, data.CounterName, data.InstanceName, data.MachineName);

                this.counter = pc;
            }
            catch (System.Exception)
            {
            }
        }
    }
}
