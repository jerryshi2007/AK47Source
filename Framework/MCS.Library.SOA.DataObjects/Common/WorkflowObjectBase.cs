using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.Validation;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 带流转功能对象基类
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public abstract class WorkflowObjectBase : IBusinessObject, ILoadableDataEntity, IClonableBusinessObject
	{
		private bool loaded = false;

		[NonSerialized]
		private GenericOpinionCollection opinions = null;

		[NonSerialized]
		private MaterialList attachments = null;

		[ORFieldMapping("ID", PrimaryKey = true)]
		public virtual string ID { get; set; }

		[Description("标题")]
		[ORFieldMapping("SUBJECT")]
		[StringLengthValidator(1, 255, MessageTemplate = "请填写标题，且长度必须小于255个字符")]
		public virtual string Subject { get; set; }

		[NoMapping]
		public bool Loaded
		{
			get
			{
				return this.loaded;
			}
			set
			{
				this.loaded = value;
			}
		}

		[ScriptIgnore()]
		[NoMapping]
		public virtual GenericOpinionCollection Opinions
		{
			get
			{
				if (this.opinions == null)
					if (this.loaded)
						this.opinions = GenericOpinionAdapter.Instance.LoadFromResourceID(this.ID);
					else
						this.opinions = new GenericOpinionCollection();

				return this.opinions;
			}
		}

		[ScriptIgnore()]
		[NoMapping]
		[Description("起草意见")]
		public virtual GenericOpinion DraftOpinion
		{
			get
			{
				GenericOpinion opinion = null;

				if (Opinions.Count > 0)
					opinion = Opinions[0];

				return opinion;
			}
		}

		[ScriptIgnore()]
		[NoMapping]
		public virtual MaterialList Attachments
		{
			get
			{
				if (this.attachments == null)
					if (this.loaded)
						this.attachments = MaterialAdapter.Instance.LoadMaterialsByResourceID(this.ID);
					else
						this.attachments = new MaterialList();

				return this.attachments;
			}
		}

		public virtual IClonableBusinessObject GenerateNewObject()
		{
			WorkflowObjectBase newData = (WorkflowObjectBase)SerializationHelper.CloneObject(this);

			newData.ID = UuidHelper.NewUuidString();

			newData.Attachments.Clear();

			foreach (Material m in Attachments)
			{
				Material newMaterial = (Material)SerializationHelper.CloneObject(m);

				newMaterial.ID = UuidHelper.NewUuidString();
				newMaterial.ResourceID = newData.ID;

				newData.Attachments.Add(newMaterial);
			}

			return newData;
		}

		/*
		[NoMapping]
		[Description("起草意见")]
		public virtual GenericOpinion DraftOpinion
		{
			get
			{
				GenericOpinion opinion = null;

				if (Opinions.Count > 0)
					opinion = Opinions[0];

				return opinion;
			}
			set
			{
				if (value != null)
				{
					if (Opinions.Count == 0)
						Opinions.Add(value);
					else
						Opinions[0] = value;
				}
			}
		}

		/*
		/// <summary>
		/// 修改对象和子对象中与流程相关的属性
		/// </summary>
		/// <param name="process"></param>
		public virtual void ChangeProcessInfo(IWfProcess process)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(process != null, "process");

			Attachments.ForEach(m =>
			{
				m.ResourceID = process.ResourceID;
				m.WfProcessID = process.ID;
				m.WfActivityID = process.CurrentActivity.ID;
			});

			if (DraftOpinion != null)
			{
				DraftOpinion.ResourceID = process.ResourceID;
				DraftOpinion.ProcessID = process.ID;
				DraftOpinion.ActivityID = process.CurrentActivity.ID;
			}
		}

		*/
	}
}
