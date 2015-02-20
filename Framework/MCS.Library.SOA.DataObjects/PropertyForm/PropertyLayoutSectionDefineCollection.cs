using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	public class PropertyLayoutSectionCollection : SerializableEditableKeyedDataObjectCollectionBase<string, PropertyLayoutSectionDefine>
    {

        public void LoadLayoutSectionFromConfiguration(string formLayoutName)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(formLayoutName, "formLayoutName");

            PropertyFormLayoutConfigurationElement formLayout = PropertyFormLayoutSettings.GetConfig().Layouts[formLayoutName];

            ExceptionHelper.FalseThrow<KeyNotFoundException>(formLayout != null, "不能根据'{0}'找到对应的属性组定义", formLayoutName);

            LoadPropertiesFromConfiguration(formLayout);
        }

        public void LoadPropertiesFromConfiguration(PropertyFormLayoutConfigurationElement formLayout)
        {
            this.Clear();

            AppendPropertiesFromConfiguration(formLayout);
        }

        public void AppendPropertiesFromConfiguration(PropertyFormLayoutConfigurationElement formLayout)
        {
            if (formLayout != null)
            {
                foreach (PropertyFormSectionConfigurationElement formSectionElement in formLayout.AllSections)
                {
                    if (this.ContainsKey(formSectionElement.Name))
                    {
                        if (formSectionElement.AllowOverride)
                        {
                            this.Remove(pd => pd.DisplayName == formSectionElement.Name);
                            this.Add(new PropertyLayoutSectionDefine(formSectionElement));
                        }
                    }
                    else
                        this.Add(new PropertyLayoutSectionDefine(formSectionElement));
                }
            }
        }


        protected override string GetKeyForItem(PropertyLayoutSectionDefine item)
        {
            return item.DisplayName;
        }
    }
}
