using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 删除活动时，相关的连线信息
    /// </summary>
    internal class WfActivityDeleteInfo
    {
        public IWfActivityDescriptor FromActivity
        {
            get;
            set;
        }

        public IWfActivityDescriptor ToActivity
        {
            get;
            set;
        }

        public IWfTransitionDescriptor TemplateTransition
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 删除活动前后相关活动的信息
    /// </summary>
    internal class WfActivityDeleteInfoCollection : EditableDataObjectCollectionBase<WfActivityDeleteInfo>
    {
        /// <summary>
        /// 从即将删除的活动上，提取相关线的信息
        /// </summary>
        /// <param name="needToDeleteActDesp"></param>
        public WfActivityDeleteInfoCollection(IWfActivityDescriptor needToDeleteActDesp)
        {
            foreach (IWfTransitionDescriptor fromTransition in needToDeleteActDesp.FromTransitions)
            {
                foreach (IWfTransitionDescriptor toTransition in needToDeleteActDesp.ToTransitions)
                {
                    if (string.Compare(fromTransition.FromActivity.Key, toTransition.ToActivity.Key, true) != 0)
                    {
                        WfActivityDeleteInfo adi = new WfActivityDeleteInfo()
                        {
                            FromActivity = fromTransition.FromActivity,
                            ToActivity = toTransition.ToActivity,
                            TemplateTransition = toTransition
                        };

                        this.Add(adi);
                    }
                }
            }
        }

        public void MergeOriginalActivitiesTranstions()
        {
            foreach (WfActivityDeleteInfo adi in this)
            {
                IWfTransitionDescriptor existedTransition = adi.FromActivity.ToTransitions.Find(t => t.ToActivity.Key == adi.ToActivity.Key);

                if (existedTransition == null)
                {
                    //原来不存在前后两端的线
                    adi.TemplateTransition.ConnectActivities(adi.FromActivity, adi.ToActivity);
                }
                else
                {
                    //原来存在前后两端的线，且是退回线，新线不是退回线，则覆盖此线
                    if (existedTransition.IsBackward && adi.TemplateTransition.IsBackward == false)
                    {
                        adi.FromActivity.ToTransitions.Remove(existedTransition);
                        adi.TemplateTransition.ConnectActivities(adi.FromActivity, adi.ToActivity);
                    }
                }
            }
        }
    }
}
