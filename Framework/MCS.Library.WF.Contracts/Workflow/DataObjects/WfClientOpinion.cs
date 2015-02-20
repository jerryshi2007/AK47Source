using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MCS.Library.WF.Contracts.Workflow.DataObjects
{
    [DataContract]
    [Serializable]
    public class WfClientOpinion
    {
        public string ID
        {
            get;
            set;
        }

        public string ResourceID
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public string IssuePersonID
        {
            get;
            set;
        }

        public string IssuePersonName
        {
            get;
            set;
        }

        public string AppendPersonID
        {
            get;
            set;
        }

        public string AppendPersonName
        {
            get;
            set;
        }

        public DateTime IssueTime
        {
            get;
            set;
        }

        public DateTime AppendTime
        {
            get;
            set;
        }

        public string ProcessID
        {
            get;
            set;
        }

        public string ActivityID
        {
            get;
            set;
        }

        public string LevelName
        {
            get;
            set;
        }

        public string LevelDesp
        {
            get;
            set;
        }

        public string OpinionType
        {
            get;
            set;
        }

        public string ExtraData
        {
            get;
            set;
        }

        /// <summary>
        /// 将字典填充到ExtraData属性中
        /// </summary>
        /// <param name="dictionary"></param>
        public void FillExtraDataFromDictionary(Dictionary<string, object> dictionary)
        {
            if (dictionary != null)
            {
                XElement root = new XElement("ExtraData");

                foreach (KeyValuePair<string, object> kp in dictionary)
                {
                    XElement itemElem = root.AddChildElement("Item");

                    itemElem.SetAttributeValue("key", kp.Key);

                    if (kp.Value != null)
                        itemElem.SetAttributeValue("value", kp.Value.ToString());
                }

                this.ExtraData = root.ToString();
            }
        }

        public Dictionary<string, object> GetDictionaryFromExtraData()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            if (this.ExtraData.IsNotEmpty())
            {
                XElement root = XElement.Parse(this.ExtraData);

                foreach (XElement itemElem in root.Elements("Item"))
                    result[itemElem.Attribute("key", string.Empty)] = itemElem.Attribute("value", string.Empty);
            }

            return result;
        }

        /// <summary>
        /// 得到填写意见时。下一步流程活动和线的信息
        /// </summary>
        /// <returns></returns>
        public WfClientNextStepCollection GetNextSteps()
        {
            WfClientNextStepCollection result = null;

            Dictionary<string, object> extraDataDict = GetDictionaryFromExtraData();

            string nextStepXml = extraDataDict.GetValue("NextSteps", string.Empty);

            if (nextStepXml.IsNotEmpty())
                result = new WfClientNextStepCollection(XElement.Parse(nextStepXml));
            else
                result = new WfClientNextStepCollection();

            return result;
        }
    }

    [DataContract]
    [Serializable]
    public class WfClientOpinionCollection : EditableDataObjectCollectionBase<WfClientOpinion>
    {
        public WfClientOpinionCollection()
        {
        }

        public WfClientOpinionCollection(IEnumerable<WfClientOpinion> source)
        {
            this.CopyFrom(source);
        }

        public WfClientOpinionCollection GetOpinions(string levelName)
        {
            WfClientOpinionCollection opinions = new WfClientOpinionCollection();

            foreach (WfClientOpinion o in this)
                if (o.LevelName == levelName)
                    opinions.Add(o);

            return opinions;
        }
    }
}
