using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Expression;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Conditions
{
	public class SCConditionCalculator
	{
		private SCConditionCalculatingContext _Context = new SCConditionCalculatingContext();

		public SCConditionCalculatingContext Context
		{
			get
			{
				return this._Context;
			}
		}

		/// <summary>
		/// 生成所有的用户容器下的用户信息快照
		/// </summary>
		public void GenerateAllUserAndContainerSnapshot()
		{
			ProcessProgress.Current.MinStep = 0;
			ProcessProgress.Current.MaxStep = 100;
			ProcessProgress.Current.CurrentStep = 0;
			ProcessProgress.Current.StatusText = string.Format("正在加载所有群组和角色");
			ProcessProgress.Current.Response();

			Stopwatch sw = new Stopwatch();

			sw.Start();
			try
			{
				SchemaObjectCollection containers = SchemaObjectAdapter.Instance.LoadBySchemaType(new string[] { "Groups", "Roles" }, DateTime.MinValue);

				// Groups排在前面
				containers.Sort((c1, c2) => string.Compare(c1.SchemaType, c2.SchemaType, true));

				ProcessProgress.Current.StatusText = string.Format("加载所有群组和角色完成，总共{0}个对象", containers.Count);
				ProcessProgress.Current.MaxStep = containers.Count;

				ProcessProgress.Current.Response();

				containers.ForEach(c => this.GenerateOneUserAndContainerSnapshot((ISCUserContainerObject)c));
			}
			finally
			{
				sw.Stop();
			}

			ProcessProgress.Current.StatusText = string.Format("计算人员完成，耗时{0:#,##0.00}秒", sw.Elapsed.TotalSeconds);
			ProcessProgress.Current.Response();
		}

		/// <summary>
		/// 生成用户容器下的用户信息快照
		/// </summary>
		/// <param name="containers"></param>
		public void GenerateUserAndContainerSnapshot(IEnumerable<ISCUserContainerObject> containers)
		{
			ProcessProgress.Current.MinStep = 0;
			ProcessProgress.Current.MaxStep = containers.Count();
			ProcessProgress.Current.CurrentStep = 0;

			Stopwatch sw = new Stopwatch();

			sw.Start();
			try
			{
				foreach (ISCUserContainerObject container in containers)
				{
					this.GenerateOneUserAndContainerSnapshot(container);
				}
			}
			finally
			{
				sw.Stop();
			}

			ProcessProgress.Current.CurrentStep = ProcessProgress.Current.MaxStep;
			ProcessProgress.Current.StatusText = string.Format("计算人员完成，耗时{0:#,##0.00}秒", sw.Elapsed.TotalSeconds);
			ProcessProgress.Current.Response();
		}

		private void GenerateOneUserAndContainerSnapshot(ISCUserContainerObject container)
		{
			SchemaObjectBase obj = container as SchemaObjectBase;

			string objName = obj.Properties.GetValue("Name", string.Empty);
			string description = string.Format("对象\"{0}\"({1})", objName, obj.ID);

			ProcessProgress.Current.StatusText = string.Format("正在生成\"{0}\"({1})所包含的用户",
				objName, obj.Schema.Name);

			ProcessProgress.Current.Response();
			try
			{
				SchemaObjectCollection currentUsers = container.GetCurrentUsers();

				SchemaObjectCollection calculatedUsers = this.Calculate(SCConditionAdapter.Instance.Load(obj.ID), description);

				currentUsers.Merge(calculatedUsers);

				ProcessProgress.Current.StatusText = string.Format("正在提交\"{0}\"({1})的生成结果", objName, obj.Schema.Name);

				ProcessProgress.Current.MaxStep += calculatedUsers.Count;
				ProcessProgress.Current.Response();

				ConditionCalculateResultAdapter.Instance.Update(obj.ID, calculatedUsers);
				UserAndContainerSnapshotAdapter.Instance.Merge(obj.ID, obj.SchemaType, currentUsers);
			}
			catch (System.Exception ex)
			{
				string exPrefix = string.Format("生成{0}时所包含的用户出错：", description);
				ProcessProgress.Current.Error.WriteLine("{0}: {1}", exPrefix, ex.ToString());
			}
			finally
			{
				ProcessProgress.Current.Increment();
				ProcessProgress.Current.Response();
			}
		}

		/// <summary>
		/// 计算一个Owner下的条件
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="description">辅助描述信息，用于错误信息或输出日志</param>
		/// <returns></returns>
		public SchemaObjectCollection Calculate(SCConditionOwner owner, string description = "")
		{
			SchemaObjectCollection result = new SchemaObjectCollection();

			// 初始化基础数据，包括所有用户
			this.Context.EnsureInitialized();

			if (description.IsNullOrEmpty())
				description = owner.OwnerID;

			// 根据条件筛选结果
			this.Context.ExecutionWrapper(string.Format("计算{0}的条件", description),
				() => this.FilterUsersByConditions(owner.Conditions, result, description));

			return result;
		}

		private void FilterUsersByConditions(SCConditionCollection conditions, SchemaObjectCollection result, string description)
		{
			ProcessProgress.Current.MaxStep += conditions.Count;
			ProcessProgress.Current.Response();

			foreach (SCCondition condition in conditions)
			{
				if (condition.Condition.IsNotEmpty() && condition.Status == SchemaObjectStatus.Normal)
				{
					try
					{
						foreach (SCUser user in this.Context.AllObjects)
						{
							this.Context.CurrentObject = user;

							try
							{
								//计算表达式
								object booleanResult = ExpressionParser.Calculate(condition.Condition, new CalculateUserFunction(CalculateUserFunction), this.Context);
								if ((booleanResult is bool) == false)
									throw new FormatException("指定的表达式未能解析为Bool值:" + Environment.NewLine + condition.Condition);

								if ((bool)booleanResult)
								{
									result.AddNotExistsItem(user);
								}
							}
							finally
							{
								this.Context.CurrentObject = null;
							}
						}
					}
					catch (ParsingException ex)
					{
						ProcessProgress.Current.Error.WriteLine("解析{0}的条件{1}出错: {2}", description, condition.Condition, ex.Message);
					}
				}
			}

			ProcessProgress.Current.Increment();
			ProcessProgress.Current.Response();
		}

		private static object CalculateUserFunction(string funcName, ParamObjectCollection arrParams, object callerContext)
		{
			SCConditionCalculatingContext context = (SCConditionCalculatingContext)callerContext;

			object result = null;

			if (SCBuiltInFunctionsCalculator.Instance.IsFunction(funcName))
			{
				//如果是内置函数，则返回内置函数的结果
				result = SCBuiltInFunctionsCalculator.Instance.Calculate(funcName, arrParams, context);
			}
			else
			{
				//根据函数名称，获取属性访问器
				SCPropertyAccessorBase accessor = GetPropertyAccessor(funcName, context);
				result = accessor.GetValue(context, arrParams);
			}

			return result;
		}

		private static SCPropertyAccessorBase GetPropertyAccessor(string funcName, SCConditionCalculatingContext context)
		{
			SCPropertyAccessorBase result = null;

			SchemaNameAndPropertyName snpn = SchemaNameAndPropertyName.FromFullName(funcName);

			snpn.CheckIsValid();

			if (context.PropertyAccessors.TryGetValue(snpn, out result) == false)
			{
				result = new DefaultUsersPropertyAccessor(snpn);
				context.PropertyAccessors.Add(snpn, result);
			}

			return result;
		}
	}
}
