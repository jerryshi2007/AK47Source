using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Diagnostics;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Adapters;
using MCS.Library.SOA.DataObjects.Security.Conditions;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.Expression;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Actions
{
	public class AUConditionCalculator
	{
		private AUConditionCalculatingContext _Context;

		public AUConditionCalculator(string schemaType)
		{
			schemaType.NullCheck("schemaType");
			this._Context = new AUConditionCalculatingContext(schemaType);
		}

		public AUConditionCalculatingContext Context
		{
			get
			{
				return this._Context;
			}
		}

		/// <summary>
		/// 生成所有的用户容器下的用户信息快照
		/// </summary>
		public void GenerateAllItemAndContainerSnapshot()
		{
			ProcessProgress.Current.MinStep = 0;
			ProcessProgress.Current.MaxStep = 100;
			ProcessProgress.Current.CurrentStep = 0;
			ProcessProgress.Current.StatusText = string.Format("正在加载所有管理范围");
			ProcessProgress.Current.Response();

			Stopwatch sw = new Stopwatch();

			sw.Start();
			try
			{
				AUCommon.DoDbAction(() =>
				{
					SchemaObjectCollection containers = AUCommon.DoDbProcess<SchemaObjectCollection>(() => PC.Adapters.SchemaObjectAdapter.Instance.LoadBySchemaType(new string[] { AUCommon.SchemaAUAdminScope }, DateTime.MinValue));

					containers.Sort((c1, c2) => string.Compare(c1.SchemaType, c2.SchemaType, true));

					ProcessProgress.Current.StatusText = string.Format("加载所有管理范围完成，总共{0}个对象", containers.Count);
					ProcessProgress.Current.MaxStep = containers.Count;

					ProcessProgress.Current.Response();

					containers.ForEach(c => this.GenerateOneItemAndContainerSnapshot((IAdminScopeItemContainer)c));
				});
			}
			finally
			{
				sw.Stop();
			}

			ProcessProgress.Current.StatusText = string.Format("计算人员完成，耗时{0:#,##0.00}秒", sw.Elapsed.TotalSeconds);
			ProcessProgress.Current.Response();
		}

		/// <summary>
		/// 生成用户容器下的管理范围信息快照
		/// </summary>
		/// <param name="containers"></param>
		public void GenerateItemAndContainerSnapshot(IEnumerable<SchemaObjectBase> containers)
		{
			ProcessProgress.Current.MinStep = 0;
			ProcessProgress.Current.MaxStep = containers.Count();
			ProcessProgress.Current.CurrentStep = 0;

			Stopwatch sw = new Stopwatch();

			sw.Start();
			try
			{
				AUCommon.DoDbAction(() =>
				{
					foreach (IAdminScopeItemContainer container in containers)
					{
						this.GenerateOneItemAndContainerSnapshot(container);
					}
				});
			}
			finally
			{
				sw.Stop();
			}

			ProcessProgress.Current.CurrentStep = ProcessProgress.Current.MaxStep;
			ProcessProgress.Current.StatusText = string.Format("计算完成，耗时{0:#,##0.00}秒", sw.Elapsed.TotalSeconds);
			ProcessProgress.Current.Response();
		}

		private void GenerateOneItemAndContainerSnapshot(IAdminScopeItemContainer container)
		{
			SchemaObjectBase obj = container as SchemaObjectBase;

			string objName = obj.Properties.GetValue("Name", string.Empty);
			string description = string.Format("对象\"{0}\"({1})", objName, obj.ID);

			ProcessProgress.Current.StatusText = string.Format("正在生成\"{0}\"({1})所包含的对象",
				objName, obj.Schema.Name);

			ProcessProgress.Current.Response();
			try
			{
				SchemaObjectCollection currentObjects = container.GetCurrentObjects();

				SchemaObjectCollection calculatedObjects = this.Calculate(AUConditionAdapter.Instance.Load(obj.ID, AUCommon.ConditionType), description);

				currentObjects.Merge(calculatedObjects);

				ProcessProgress.Current.StatusText = string.Format("正在提交\"{0}\"({1})的生成结果", objName, obj.Schema.Name);

				ProcessProgress.Current.MaxStep += calculatedObjects.Count;
				ProcessProgress.Current.Response();

				AUConditionCalculateResultAdapter.Instance.Update(obj.ID, calculatedObjects);
				ItemAndContainerSnapshotAdapter.Instance.Merge(obj.ID, obj.SchemaType, currentObjects);
			}
			catch (System.Exception ex)
			{
				string exPrefix = string.Format("生成{0}时所包含的对象出错：", description);
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
		/// <param name="conditions"></param>
		/// <param name="description">辅助描述信息，用于错误信息或输出日志</param>
		/// <returns></returns>
		public SchemaObjectCollection Calculate(SCConditionCollection conditions, string description = "")
		{
			SchemaObjectCollection result = new SchemaObjectCollection();

			// 初始化基础数据，包括所有项目
			this.Context.EnsureInitialized();

			// 根据条件筛选结果
			this.Context.ExecutionWrapper(string.Format("计算{0}的条件", description),
				() => this.FilterItemsByConditions(conditions, result, description));

			return result;
		}

		private void FilterItemsByConditions(SCConditionCollection conditions, SchemaObjectCollection result, string description)
		{
			ProcessProgress.Current.MaxStep += conditions.Count;
			ProcessProgress.Current.Response();

			foreach (SCCondition condition in conditions)
			{
				if (condition.Condition.IsNotEmpty() && condition.Status == SchemaObjectStatus.Normal)
				{
					try
					{
						foreach (SchemaObjectBase obj in this.Context.AllObjects)
						{
							this.Context.CurrentObject = obj;

							try
							{
								//计算表达式
								if ((bool)ExpressionParser.Calculate(condition.Condition, new CalculateUserFunction(CalculateObjectFunction), this.Context))
								{
									result.AddNotExistsItem(obj);
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

		private static object CalculateObjectFunction(string funcName, ParamObjectCollection arrParams, object callerContext)
		{
			AUConditionCalculatingContext context = (AUConditionCalculatingContext)callerContext;

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

		private static SCPropertyAccessorBase GetPropertyAccessor(string funcName, AUConditionCalculatingContext context)
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

		class AUBuildInFunction
		{
			
		}
	}
}
