using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.Core
{
    /// <summary>
    /// 用来表示处理进度的类。它是上下文对象。运行时，当进度改变时，调用注册的插件进行进度处理。
    /// </summary>
    public sealed class ProcessProgress : IDisposable
    {
        private int _minStep = 0;
        private int _maxStep = 0;
        private int _currentStep = 0;

        private string statusText = string.Empty;
        private TextWriter _output = null;
        private TextWriter _error = null;

        private StringWriter _defaultOutput = null;

        private StringWriter _defaultError = null;

        private readonly Dictionary<Type, IProcessProgressResponser> _responsers = new Dictionary<Type, IProcessProgressResponser>();

        private ProcessProgress()
        {
        }

        #region Properties
        /// <summary>
        /// 最小步骤
        /// </summary>
        public int MinStep
        {
            get
            {
                return this._minStep;
            }
            set
            {
                this._minStep = value;
            }
        }

        /// <summary>
        /// 最大的步骤
        /// </summary>
        public int MaxStep
        {
            get
            {
                return this._maxStep;
            }
            set
            {
                this._maxStep = value;
            }
        }

        /// <summary>
        /// 当前步骤
        /// </summary>
        public int CurrentStep
        {
            get
            {
                return this._currentStep;
            }
            set
            {
                if (value < this._minStep || value > this._maxStep)
                    throw new ArgumentOutOfRangeException("CurrentStep",
                        string.Format("参数CurrentStep值{0}越界，必须在{1}和{2}之间", value, this._minStep, this._maxStep));

                this._currentStep = value;
            }
        }

        /// <summary>
        /// 当前的描述信息
        /// </summary>
        public string StatusText
        {
            get
            {
                return this.statusText;
            }
            set
            {
                this.statusText = value;
            }
        }

        /// <summary>
        /// 输出信息
        /// </summary>
        public TextWriter Output
        {
            get
            {
                if (this._output == null)
                    this._output = this.DefaultOutput;

                return this._output;
            }
            set
            {
                this._output = value;
            }
        }

        /// <summary>
        /// 输出的错误信息
        /// </summary>
        public TextWriter Error
        {
            get
            {
                if (this._error == null)
                    this._error = this.DefaultError;

                return this._error;
            }
            set
            {
                this._error = value;
            }
        }

        /// <summary>
        /// 返回当前上下文的ProcessProgress对象
        /// </summary>
        public static ProcessProgress Current
        {
            get
            {
                return (ProcessProgress)ObjectContextCache.Instance.GetOrAddNewValue("CurrentProcessProgress", (cache, key) =>
                    {
                        ProcessProgress result = new ProcessProgress();

                        cache.Add(key, result);

                        return result;
                    });
            }
        }
        #endregion Properties

        #region Public methods
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="minStep"></param>
        /// <param name="maxStep"></param>
        /// <param name="currentStep"></param>
        public void Initialize(int minStep, int maxStep, int currentStep)
        {
            Initialize(minStep, maxStep, currentStep, string.Empty);
        }

        /// <summary>
        /// 初始化步骤
        /// </summary>
        /// <param name="minStep"></param>
        /// <param name="maxStep"></param>
        /// <param name="currentStep"></param>
        /// <param name="status"></param>
        public void Initialize(int minStep, int maxStep, int currentStep, string status)
        {
            this.MinStep = minStep;
            this.MaxStep = maxStep;
            this.CurrentStep = currentStep;
            this.StatusText = status;
        }

        /// <summary>
        /// 注册输出器
        /// </summary>
        /// <param name="responser"></param>
        public void RegisterResponser(IProcessProgressResponser responser)
        {
            responser.NullCheck("responser");

            Type rType = responser.GetType();

            if (this._responsers.ContainsKey(rType) == false)
            {
                this._responsers.Add(rType, responser);
                responser.Register(this);
            }
        }

        /// <summary>
        /// 调用输出器进行输出
        /// </summary>
        public void Response()
        {
            _responsers.ForEach(kp => kp.Value.Response(this));
        }

        /// <summary>
        /// 设置状态信息，调用输出器进行输出
        /// </summary>
        /// <param name="status"></param>
        public void Response(string status)
        {
            this.StatusText = status;

            Response();
        }

        /// <summary>
        /// 得到默认的输出信息
        /// </summary>
        /// <returns></returns>
        public string GetDefaultOutput()
        {
            string result = string.Empty;

            if (this._defaultOutput != null)
                result = this._defaultOutput.GetStringBuilder().ToString();

            string error = GetDefaultError();

            if (error.IsNotEmpty())
                result += "\n错误\n" + error;

            return result;
        }

        /// <summary>
        /// 得到默认的错误信息
        /// </summary>
        /// <returns></returns>
        public string GetDefaultError()
        {
            string result = string.Empty;

            if (this._defaultError != null)
                result = this._defaultError.GetStringBuilder().ToString();

            return result;
        }

        /// <summary>
        /// CurrentStep加1
        /// </summary>
        public void Increment()
        {
            if (this._currentStep < this._maxStep)
                this._currentStep++;
        }

        /// <summary>
        /// CurrentStep增加nSteps，如果大于最大值，则等于最大值
        /// </summary>
        /// <param name="nSteps"></param>
        public void IncrementBy(int nSteps)
        {
            (nSteps >= 0).FalseThrow("需要增加的步骤必须大于0");

            this._currentStep = Math.Min(this._currentStep + nSteps, this._maxStep);
        }

        /// <summary>
        /// CurrentStep加1，并且设置状态信息
        /// </summary>
        /// <param name="status"></param>
        public void Increment(string status)
        {
            Increment();

            this.StatusText = status;
        }

        /// <summary>
        /// CurrentStep减少nSteps，如果小于最小值，则等于最小值
        /// </summary>
        /// <param name="nSteps"></param>
        public void Decrement(int nSteps)
        {
            (nSteps >= 0).FalseThrow("需要减少的步骤必须大于0");

            this._currentStep = Math.Max(this._currentStep - nSteps, this._minStep);
        }

        /// <summary>
        /// CurrentStep减1
        /// </summary>
        public void Decrement()
        {
            if (this._currentStep > this._minStep)
                this._currentStep--;
        }

        /// <summary>
        /// CurrentStep减1，并且设置状态信息
        /// </summary>
        /// <param name="status"></param>
        public void Decrement(string status)
        {
            Decrement();

            this.StatusText = status;
        }

        /// <summary>
        /// 清除当前上下文的ProcessProgress对象
        /// </summary>
        public static void Clear()
        {
            if (ObjectContextCache.Instance.ContainsKey("CurrentProcessProgress"))
                ObjectContextCache.Instance.Remove("CurrentProcessProgress");
        }
        #endregion

        #region Private
        private StringWriter DefaultOutput
        {
            get
            {
                if (this._defaultOutput == null)
                {
                    StringBuilder strB = new StringBuilder();
                    this._defaultOutput = new StringWriter(strB);
                }

                return this._defaultOutput;
            }
        }

        private StringWriter DefaultError
        {
            get
            {
                if (this._defaultError == null)
                {
                    StringBuilder strB = new StringBuilder();
                    this._defaultError = new StringWriter(strB);
                }

                return this._defaultError;
            }
        }
        #endregion Protected

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (this._defaultOutput != null)
                this._defaultOutput.Dispose();

            if (this._defaultError != null)
                this._defaultError.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
