using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow.Builders;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public abstract class WfResourceDescriptor : WfDescriptorBase, ISimpleXmlSerializer
	{
		protected WfResourceDescriptor() { }

		protected internal abstract void FillUsers(OguDataCollection<IUser> users);

		internal void SetProcessInstance(IWfProcess process)
		{
			this.ProcessInstance = process;
		}

		#region ISimpleXmlSerializer Members

		/// <summary>
		/// 转换为简单的XElement
		/// </summary>
		/// <param name="element"></param>
		/// <param name="refNodeName"></param>
		public void ToXElement(XElement element, string refNodeName)
		{
			element.NullCheck("element");

			if (refNodeName.IsNotEmpty())
				element = element.AddChildElement(refNodeName);

			element.SetAttributeValue("type", this.GetType().Name);

			ToXElement(element);
		}

		#endregion

		/// <summary>
		/// 填充XElement节点
		/// </summary>
		/// <param name="element"></param>
		protected abstract void ToXElement(XElement element);
	}

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public class WfResourceDescriptorCollection : EditableDataObjectCollectionBase<WfResourceDescriptor>, ISimpleXmlSerializer
	{
		public WfResourceDescriptorCollection()
		{
		}

		public WfResourceDescriptorCollection(IWfDescriptor owner)
		{
			this.Owner = owner;
		}

		[XElementFieldSerialize(AlternateFieldName = "_Owner")]
		public IWfDescriptor Owner
		{
			get;
			private set;
		}

		/// <summary>
		/// 将所有包含originalUser的WfUserResourceDescriptor的资源替换为包含replaceUsers的一系列资源。
		/// 如果replaceUsers为null或者空集合，则相当于删除原始用户
		/// </summary>
		/// <param name="originalUser"></param>
		/// <param name="replaceUsers"></param>
		/// <returns>被替换的个数</returns>
		public int ReplaceAllUserResourceDescriptors(IUser originalUser, IEnumerable<IUser> replaceUsers)
		{
			int result = 0;

			if (originalUser != null)
			{
				replaceUsers = GetNormalizedReplaceUsers(originalUser, replaceUsers);

				this.Remove(r =>
				{
					bool matched = false;

					if (r is WfUserResourceDescriptor && ((WfUserResourceDescriptor)r).IsSameUser(originalUser))
					{
						matched = true;
						result++;
					}

					return matched;
				});

				if (result > 0)
					replaceUsers.ForEach(u => this.Add(new WfUserResourceDescriptor(u)));
			}

			return result;
		}

		/// <summary>
		/// 消除替换人员中，与被替换人员重复的，且已经在用户资源中存在的
		/// </summary>
		/// <param name="originalUser"></param>
		/// <param name="replaceUsers"></param>
		/// <returns></returns>
		private List<IUser> GetNormalizedReplaceUsers(IUser originalUser, IEnumerable<IUser> replaceUsers)
		{
			List<IUser> result = new List<IUser>();

			if (replaceUsers != null)
			{
				foreach (IUser user in replaceUsers)
				{
					if (user.ID != originalUser.ID && this.ExistsUserResource(user) == false)
						result.Add(user);
				}
			}

			return result;
		}

		private bool ExistsUserResource(IUser user)
		{
			bool result = false;

			foreach (WfResourceDescriptor resource in this)
			{
				if (resource is WfUserResourceDescriptor)
				{
					if (((WfUserResourceDescriptor)resource).IsSameUser(user))
					{
						result = true;
						break;
					}
				}
			}

			return result;
		}

		public void SyncPropertiesToFields(PropertyValue property)
		{
			if (property != null)
			{
				this.Clear();

				if (property.StringValue.IsNotEmpty())
				{
					IEnumerable<WfResourceDescriptor> deserializedData = (IEnumerable<WfResourceDescriptor>)JSONSerializerExecute.DeserializeObject(property.StringValue, this.GetType());

					this.CopyFrom(deserializedData);
				}
			}
		}

		public OguDataCollection<IUser> ToUsers()
		{
			OguDataCollection<IUser> result = new OguDataCollection<IUser>();

			this.ForEach(r => r.FillUsers(result));

			result.Distinct((src, dest) => string.Compare(src.FullPath, dest.FullPath) == 0);

			return result;
		}

		public WfAssigneeCollection ToAssignees()
		{
			OguDataCollection<IUser> users = ToUsers();

			WfAssigneeCollection result = new WfAssigneeCollection();

			users.ForEach(u => result.Add(u));

			return result;
		}

		/// <summary>
		/// 是否使用活动矩阵
		/// </summary>
		/// <returns></returns>
		public bool IsActivityMatrix()
		{
			bool result = false;

			foreach (WfResourceDescriptor resource in this)
			{
				if (resource is IWfCreateActivityParamsGenerator &&
						((IWfCreateActivityParamsGenerator)resource).UseCreateActivityParams)
				{
					result = true;
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// 转换成创建流程的参数集合
		/// </summary>
		/// <returns></returns>
		public WfCreateActivityParamCollection ToCreateActivityParams()
		{
			return ToCreateActivityParams(new PropertyDefineCollection());
		}

		/// <summary>
		/// 转换成创建流程的参数集合。
		/// </summary>
		/// <returns></returns>
		public WfCreateActivityParamCollection ToCreateActivityParams(PropertyDefineCollection definedProperties)
		{
			definedProperties.NullCheck("definedProperties");

			OguDataCollection<IUser> users = new OguDataCollection<IUser>();

			WfCreateActivityParamCollection tempResult = new WfCreateActivityParamCollection();

			foreach (WfResourceDescriptor resource in this)
			{
				if (resource is IWfCreateActivityParamsGenerator &&
						((IWfCreateActivityParamsGenerator)resource).UseCreateActivityParams)
				{
					((IWfCreateActivityParamsGenerator)resource).Fill(tempResult, definedProperties);
				}
				else
					resource.FillUsers(users);
			}

			WfCreateActivityParamCollection result = new WfCreateActivityParamCollection();

			result.CopyFrom(UsersToCreateActivityParams(users, definedProperties));
			result.CopyFrom(tempResult);

			result.Sort((x, y) => x.ActivitySN - y.ActivitySN);

			return result;
		}

		private static WfCreateActivityParamCollection UsersToCreateActivityParams(OguDataCollection<IUser> users, PropertyDefineCollection definedProperties)
		{
			WfCreateActivityParamCollection result = new WfCreateActivityParamCollection();

			int i = 0;

			foreach (IUser user in users)
			{
				WfCreateActivityParam param = new WfCreateActivityParam();

				param.ActivitySN = i++;
				param.Template.Properties.MergeDefinedProperties(definedProperties);
				param.Template.Resources.Add(new WfUserResourceDescriptor(user));
				param.Template.Variables.Add(new WfVariableDescriptor(WfProcessBuilderBase.AutoBuiltActivityVariableName, "True", DataType.Boolean));

				result.Add(param);
			}

			return result;
		}

		protected override void OnValidate(object value)
		{
			value.NullCheck("value");

			if (this.Owner != null && this.Owner.ProcessInstance != null)
				((WfDescriptorBase)value).ProcessInstance = this.Owner.ProcessInstance;

			base.OnValidate(value);
		}

		#region ISimpleXmlSerializer Members

		void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
		{
			if (refNodeName.IsNotEmpty())
				element = element.AddChildElement(refNodeName);

			foreach (WfResourceDescriptor resource in this)
			{
				XElement actElem = element.AddChildElement("Resource");

				((ISimpleXmlSerializer)resource).ToXElement(actElem, string.Empty);
			}
		}

		#endregion
	}
}
