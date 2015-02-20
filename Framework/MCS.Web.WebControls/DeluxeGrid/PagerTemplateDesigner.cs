using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Drawing;
using System.ComponentModel.Design;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.Web.UI.Design.WebControls;
using MCS.Web.Library;

namespace MCS.Web.WebControls
{
  
    /// <summary>
    /// Design-time 
    /// </summary>    
    /// <remarks>
    /// Design-time 
    /// </remarks>
	public class PagerTemplateDesigner : ControlDesigner
    {
        private DeluxeGrid deluxeGrid;
        private int currentRegion = -1;
        private int nbRegions = 0;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="component"></param>
        /// <remarks>
        /// Initialize
        /// </remarks>
        public override void Initialize(IComponent component)
        {
            this.deluxeGrid = (DeluxeGrid)component;
            base.Initialize(component);
            SetViewFlags(ViewFlags.DesignTimeHtmlRequiresLoadComplete, true);
            SetViewFlags(ViewFlags.TemplateEditing, true);
        }
        /// <summary>
        /// TemplateGroups
        /// </summary>
        /// <remarks>
        /// TemplateGroups
        /// </remarks>
        public override TemplateGroupCollection TemplateGroups
        {
            get
            {
                TemplateGroupCollection collection = new TemplateGroupCollection();
				TemplateGroup group = new TemplateGroup("GridPagerTemplate");
				TemplateDefinition definition = new TemplateDefinition(this, "GridPagerTemplate", this.deluxeGrid, "GridPagerTemplate", false);
                group.AddTemplateDefinition(definition);
                collection.Add(group);
                return collection;
            }
        }
        /// <summary>
		/// CreateChildControls
        /// </summary>
        /// <remarks>
        /// CreateChildControls
        /// </remarks>
        protected void CreateChildControls()
        {
			if (this.deluxeGrid.regions != null)
				for (int i = 0; i < this.deluxeGrid.regions.Count; i++)
                    this.deluxeGrid.regions[i].SetAttribute(DesignerRegion.DesignerRegionAttributeName, i.ToString());
        }

        /// <summary>
		/// GetDesignTimeHtml
        /// </summary>
        /// <param name="regions"></param>
        /// <returns></returns>
        /// <remarks>
        /// GetDesignTimeHtml
        /// </remarks>
        public override string GetDesignTimeHtml(DesignerRegionCollection regions)
        {
            this.CreateChildControls();

            for (int i = 0; i < this.nbRegions; i++)
            {
                DesignerRegion r;
                if (this.currentRegion == i)
                    r = new EditableDesignerRegion(this, i.ToString());
                else
                    r = new DesignerRegion(this, i.ToString());
                regions.Add(r);
            }

            if ((this.currentRegion >= 0) && (this.currentRegion < this.nbRegions))
                regions[this.currentRegion].Highlight = true;
			UpdateDesignTimeHtml();
            return base.GetDesignTimeHtml(regions);
        }
        /// <summary>
		/// OnClick
        /// </summary>
        /// <param name="e"></param>
        /// <remarks>
        /// OnClick
        /// </remarks>
        protected override void OnClick(DesignerRegionMouseEventArgs e)
        {
            base.OnClick(e);
            this.currentRegion = -1;
            if (e.Region != null)
            {
                for (int i = 0; i < this.nbRegions; i++)
                {
                    if (e.Region.Name == i.ToString())
                    {
                        this.currentRegion = i;
                        break;
                    }
                }
                UpdateDesignTimeHtml();
            }
        }
        /// <summary>
        /// 设置设计态的内容
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public override string GetEditableDesignerRegionContent(EditableDesignerRegion region)
        {
            IDesignerHost host = (IDesignerHost)Component.Site.GetService(typeof(IDesignerHost));
            if (host != null)
            {
                ITemplate contentTemplate;
                if (this.currentRegion == 0)
                {
                    contentTemplate = this.deluxeGrid.GridPagerTemplate;
                    return ControlPersister.PersistTemplate(contentTemplate, host);
                }
            }
            return String.Empty;
        }
        /// <summary>
        /// 设置设计态的内容
        /// </summary>
        /// <param name="region"></param>
        /// <param name="content"></param>
        /// <remarks>
        /// 设置设计态的内容
        /// </remarks>
        public override void SetEditableDesignerRegionContent(EditableDesignerRegion region, string content)
        {
            if (content == null)
                return;
            IDesignerHost host = (IDesignerHost)Component.Site.GetService(typeof(IDesignerHost));
            if (host != null)
            {
                ITemplate template = ControlParser.ParseTemplate(host, content);
                if (template != null)
                {
                    if (this.currentRegion == 0)
                    {
                        this.deluxeGrid.GridPagerTemplate = template;
                    }
                }
            }
        }
    }
   
}
