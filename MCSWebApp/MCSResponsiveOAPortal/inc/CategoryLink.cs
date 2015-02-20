using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MCSResponsiveOAPortal
{
    [Serializable]
    public class CategoryDirectory
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public virtual string Type { get { return "category"; } }

        public string Role { get; set; }
    }

    public class CategoryGroup : CategoryDirectory
    {
        private CategoryDirectory[] subCategories;

        public CategoryGroup()
        {
        }

        public CategoryGroup(string name, string title, CategoryDirectory[] subDirectories)
        {
            this.Name = name;
            this.Title = title;
            this.subCategories = subDirectories;
        }

        public override string Type { get { return "group"; } }

        public CategoryDirectory[] Categories
        {
            get { return this.subCategories; }
            set { this.subCategories = value; }
        }
    }

    [Serializable]
    public class CategoryLink : CategoryDirectory
    {
        public int Group { get; set; }

        public string Url { get; set; }

        public string Feature { get; set; }

        public override string Type
        {
            get
            {
                return "link";
            }
        }
    }
}