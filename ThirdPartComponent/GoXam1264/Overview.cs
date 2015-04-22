
/*
 *  Copyright © Northwoods Software Corporation, 1998-2011. All Rights Reserved.
 *
 *  Restricted Rights: Use, duplication, or disclosure by the U.S.
 *  Government is subject to restrictions as set forth in subparagraph
 *  (c) (1) (ii) of DFARS 252.227-7013, or in FAR 52.227-19, or in FAR
 *  52.227-14 Alt. III, as applicable.
 *
 *  This software is proprietary to and embodies the confidential
 *  technology of Northwoods Software Corporation. Possession, use, or
 *  copying of this software and media is authorized only pursuant to a
 *  valid written license from Northwoods or an authorized sublicensor.
 */

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Shapes;
using Northwoods.GoXam.Model;
using Northwoods.GoXam.Tool;

namespace Northwoods.GoXam {

  /// <summary>
  /// The <c>Overview</c> control is a <see cref="Diagram"/> that tries to
  /// display all of the model shown by a different <see cref="Diagram"/>,
  /// with a rectangular box showing the viewport displayed by that other diagram.
  /// </summary>
  /// <remarks>
  /// The Overview shows the nodes and links from the <see cref="Diagram.Model"/>
  /// of the <see cref="Observed"/> <see cref="Diagram"/>.
  /// You should set the <see cref="Observed"/> property after both the Overview
  /// and the Diagram have been initialized:
  /// <code>
  ///   this.Loaded += (s,e) => { myOverview.Observed = myDiagram; };
  /// </code>
  /// </remarks>
  public class Overview : Diagram {
    /// <summary>
    /// Create an <see cref="Overview"/> control -- you need to set its
    /// <see cref="Observed"/> property to refer to a different <see cref="Diagram"/>
    /// before it becomes useful.
    /// </summary>
    public Overview() {

      this.DefaultStyleKey = typeof(Overview);

      this.LayoutManager = new OverviewLayoutManager();
      // allow the user to move the Box by clicking or dragging anywhere
      MoveBoxTool tool = new MoveBoxTool();
      tool.Diagram = this;
      this.MouseDownTools.Add(tool);
    }

    static Overview() {




      ObservedProperty = DependencyProperty.Register("Observed", typeof(Diagram), typeof(Overview),
        new FrameworkPropertyMetadata(null, OnObservedChanged));
      UsesObservedTemplatesProperty = DependencyProperty.Register("UsesObservedTemplates", typeof(bool), typeof(Overview),
        new FrameworkPropertyMetadata(true));
      UsesObservedNodeLocationProperty = DependencyProperty.Register("UsesObservedNodeLocation", typeof(bool), typeof(Overview),
        new FrameworkPropertyMetadata(true, OnUsesObservedNodeLocationChanged));
      UsesObservedLinkRouteProperty = DependencyProperty.Register("UsesObservedLinkRoute", typeof(bool), typeof(Overview),
        new FrameworkPropertyMetadata(true));
      UsesObservedPartVisibleProperty = DependencyProperty.Register("UsesObservedPartVisible", typeof(bool), typeof(Overview),
        new FrameworkPropertyMetadata(true));
      BoxTemplateProperty = DependencyProperty.Register("BoxTemplate", typeof(DataTemplate), typeof(Overview),
        new FrameworkPropertyMetadata(null, OnBoxTemplateChanged));
    }

    /// <summary>
    /// Identifies the <see cref="Observed"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ObservedProperty;
    /// <summary>
    /// Gets or sets the <see cref="Diagram"/> for which this <see cref="Overview"/> is
    /// displaying a model and showing its viewport into that model.
    /// </summary>
    /// <value>
    /// This is initially null.
    /// Any new value must not be an <see cref="Overview"/>.
    /// </value>
    public Diagram Observed {
      get { return (Diagram)GetValue(ObservedProperty); }
      set { SetValue(ObservedProperty, value); }
    }
    private static void OnObservedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Overview overview = (Overview)d;
      if (e.NewValue is Overview) {
        overview.Observed = e.OldValue as Diagram;
      } else {
        overview.UnbindFromDiagram(e.OldValue as Diagram);
        overview.BindToDiagram(e.NewValue as Diagram);
      }
    }

    /// <summary>
    /// Identifies the <see cref="UsesObservedTemplates"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty UsesObservedTemplatesProperty;
    /// <summary>
    /// Gets or sets whether this <see cref="Overview"/> uses the <c>DataTemplates</c>
    /// used by the <see cref="Observed"/> <see cref="Diagram"/>.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    public bool UsesObservedTemplates {
      get { return (bool)GetValue(UsesObservedTemplatesProperty); }
      set { SetValue(UsesObservedTemplatesProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="UsesObservedNodeLocation"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty UsesObservedNodeLocationProperty;
    /// <summary>
    /// Gets or sets whether this <see cref="Overview"/> uses the <see cref="Node.Location"/>
    /// of each <see cref="Node"/> in the <see cref="Observed"/> <see cref="Diagram"/>
    /// to position the nodes in this overview.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <remarks>
    /// When this property is true, the <see cref="Overview"/> uses a special <see cref="LayoutManager"/>,
    /// because the position of each node is determined by the position of the corresponding node in
    /// the <see cref="Observed"/> diagram.
    /// </remarks>
    public bool UsesObservedNodeLocation {
      get { return (bool)GetValue(UsesObservedNodeLocationProperty); }
      set { SetValue(UsesObservedNodeLocationProperty, value); }
    }
    private static void OnUsesObservedNodeLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Overview ov = (Overview)d;
      ov.EnsureLayoutManager();
    }

    private void EnsureLayoutManager() {
      if (this.UsesObservedNodeLocation) {
        if (!(this.LayoutManager is OverviewLayoutManager)) {
          this.LayoutManager = new OverviewLayoutManager();
        }
      } else {
        if (this.LayoutManager is OverviewLayoutManager) {
          this.LayoutManager = new LayoutManager();
          LayoutDiagram();
        }
      }
    }

    /// <summary>
    /// Identifies the <see cref="UsesObservedLinkRoute"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty UsesObservedLinkRouteProperty;
    /// <summary>
    /// Gets or sets whether this <see cref="Overview"/> uses the <see cref="Link.Route"/>'s <see cref="Route.Points"/>
    /// of each <see cref="Link"/> in the <see cref="Observed"/> <see cref="Diagram"/>
    /// to route the links in this overview.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    public bool UsesObservedLinkRoute {
      get { return (bool)GetValue(UsesObservedLinkRouteProperty); }
      set { SetValue(UsesObservedLinkRouteProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="UsesObservedPartVisible"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty UsesObservedPartVisibleProperty;
    /// <summary>
    /// Gets or sets whether this <see cref="Overview"/> uses the <see cref="Part.Visible"/>
    /// of each <see cref="Node"/> or <see cref="Link"/> in the <see cref="Observed"/> <see cref="Diagram"/>
    /// to show the nodes and links in this overview.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    public bool UsesObservedPartVisible {
      get { return (bool)GetValue(UsesObservedPartVisibleProperty); }
      set { SetValue(UsesObservedPartVisibleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="BoxTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BoxTemplateProperty;
    /// <summary>
    /// Gets or sets the <c>DataTemplate</c> used to render the <see cref="Box"/>
    /// showing the <see cref="Observed"/>'s viewport into its model.
    /// </summary>
    public DataTemplate BoxTemplate {
      get { return (DataTemplate)GetValue(BoxTemplateProperty); }
      set { SetValue(BoxTemplateProperty, value); }
    }
    private static void OnBoxTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Overview overview = (Overview)d;
      overview.Box = null;
      overview.BindBox();
    }

    /// <summary>
    /// Gets or sets the <see cref="Node"/> used to display the rectangle whose bounds are the
    /// <see cref="Observed"/>'s viewport.
    /// </summary>
    /// <value>
    /// By default this is null.
    /// </value>
    protected Node Box { get; set; }

    /// <summary>
    /// Create the <see cref="Box"/> using the <see cref="BoxTemplate"/> data template.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// If <see cref="BoxTemplate"/> is null, this uses a default template.
    /// </remarks>
    protected virtual Node CreateBox() {
      Node box = new Node();  // for Overview.CreateBox
      box.Id = "OverviewBox";
      DataTemplate template = this.BoxTemplate;
      if (template == null) template = Diagram.FindDefault<DataTemplate>("DefaultOverviewBoxTemplate");
      box.ContentTemplate = template;  // but not bound to data, so no .Content
      return box;
    }

    private void BindBox() {
      if (this.Box == null && this.PartsModel != null) {
        this.Box = CreateBox();
        this.PartsModel.AddNode(this.Box);
      }
      UpdateBox();
    }


    private void ObservedPanel_ViewportBoundsChanged(object sender, EventArgs e) {
      UpdateBox();
    }

    private void UpdateBox() {
      // don't let animated scrolling cause this to scroll
      if (this.IsAnimatingScrolling) return;
      Node box = this.Box;
      if (box == null) return;
      Diagram observed = this.Observed;
      if (observed == null) return;
      DiagramPanel dpanel = observed.Panel;
      if (dpanel == null) return;
      FrameworkElement orect = box.VisualElement;
      if (orect == null) return;
      Rect viewportbounds = dpanel.ViewportBounds;
      orect.Width = viewportbounds.Width;
      orect.Height = viewportbounds.Height;
      box.Position = new Point(viewportbounds.X, viewportbounds.Y);
      // try to keep the box visible in the overview
      this.Panel.MakeVisible(box, Rect.Empty);
    }

    private void ObservedPanel_DiagramBoundsChanged(object sender, EventArgs e) {
      Diagram observed = this.Observed;
      if (observed == null) return;
      DiagramPanel dpanel = observed.Panel;
      if (dpanel == null) return;
      Rect r = dpanel.DiagramBounds;
      if (this.Box != null) {
        Rect b = this.Box.Bounds;
        Geo.Inflate(ref b, 10, 10);
        r.Union(b);
      }
      this.Panel.FixedBounds = r;
    }

    private void ObservedPanel_PartBoundsChanged(object sender, EventArgs e) {
      // don't let animated movements mess up data via TwoWay data-bindings
      if (this.IsAnimatingLayout) return;
      Diagram observed = this.Observed;
      if (observed == null) return;
      PartManager observedpartmgr = observed.PartManager;
      if (observedpartmgr == null) return;
      PartManager partmgr = this.PartManager;
      if (partmgr == null) return;
      Node observednode = sender as Node;
      if (observednode != null && this.UsesObservedNodeLocation) {
        // find the corresponding node in this Overview
        Node overviewnode = partmgr.FindNodeForData(observednode.Data, observed.Model);
        if (overviewnode != null) {
          // need to remove any Binding for Node.Location on this node,
          // in case it is Mode=TwoWay, trying to update the data
          FrameworkElement ve = overviewnode.EnsuredVisualElement;
          if (ve != null) ve.ClearValue(Node.LocationProperty);
          // the observed node has moved -- also move this overview's node
          Point ovloc = overviewnode.Location;
          Point obloc = observednode.Location;
          if (!Geo.IsApprox(ovloc, obloc)) overviewnode.Location = obloc;
        } else if (observedpartmgr.NodesCount > partmgr.NodesCount-1) {  // account for Box
          UpdateNodesAndLinks();
        }
      } else {
        Link observedlink = sender as Link;
        if (observedlink != null && this.UsesObservedLinkRoute) {
          Link overviewlink = partmgr.FindLinkForData(observedlink.Data, observed.Model);
          if (overviewlink != null) {
            // the observed link has moved or changed size -- replace this overview's link's route
            // with that of the observed link's, so it doesn't need to be recomputed
            //??? doesn't handle case where the points have changed, but the Bounds haven't
            overviewlink.Route.Points = observedlink.Route.Points;
          } else if (observedpartmgr.LinksCount > partmgr.LinksCount) {
            UpdateNodesAndLinks();
          }
        }
      }
    }

    private void ObservedPanel_PartVisibleChanged(object sender, EventArgs e) {
      Diagram observed = this.Observed;
      if (observed == null) return;
      PartManager observedpartmgr = observed.PartManager;
      if (observedpartmgr == null) return;
      PartManager partmgr = this.PartManager;
      if (partmgr == null) return;
      Node observednode = sender as Node;
      if (observednode != null && this.UsesObservedPartVisible) {
        // find the corresponding node in this Overview
        Node overviewnode = partmgr.FindNodeForData(observednode.Data, observed.Model);
        if (overviewnode != null) {
          // need to remove any Binding for Part.Visible on this node,
          // in case it is Mode=TwoWay, trying to update the data
          FrameworkElement ve = overviewnode.EnsuredVisualElement;
          if (ve != null) ve.ClearValue(Part.VisibleProperty);
          overviewnode.Visible = observednode.Visible;
        } else if (observedpartmgr.NodesCount > partmgr.NodesCount-1) {  // account for Box
          UpdateNodesAndLinks();
        }
      } else {
        Link observedlink = sender as Link;
        if (observedlink != null && this.UsesObservedPartVisible) {
          Link overviewlink = partmgr.FindLinkForData(observedlink.Data, observed.Model);
          if (overviewlink != null) {
            // need to remove any Binding for Part.Visible on this node,
            // in case it is Mode=TwoWay, trying to update the data
            FrameworkElement ve = overviewlink.EnsuredVisualElement;
            if (ve != null) ve.ClearValue(Part.VisibleProperty);
            overviewlink.Visible = observedlink.Visible;
          } else if (observedpartmgr.LinksCount > partmgr.LinksCount) {
            UpdateNodesAndLinks();
          }
        }
      }
    }

    private void ObservedPanel_AnimatingChanged(object sender, ModelChangedEventArgs e) {
      if (sender is DiagramPanel) {
        this.IsAnimatingScrolling = (bool)e.NewValue;
        UpdateBox();
      } else if (sender is LayoutManager) {
        this.IsAnimatingLayout = (bool)e.NewValue;
        // UpdateNodesAndLinks when no longer IsAnimating
        if (!this.IsAnimatingLayout) {
          UpdateNodesAndLinks();
        }
      }
    }

    private bool IsAnimatingLayout { get; set; }
    private bool IsAnimatingScrolling { get; set; }

    private void ObservedDiagram_TemplatesChanged(object sender, EventArgs e) {
      UpdateTemplates();
    }

    private void UpdateTemplates() {
      Diagram observed = this.Observed;
      if (observed == null) return;
      if (this.UsesObservedTemplates) {
        this.NodeTemplate = observed.NodeTemplate;
        this.NodeTemplateDictionary = observed.NodeTemplateDictionary;
        this.GroupTemplate = observed.GroupTemplate;
        this.GroupTemplateDictionary = observed.GroupTemplateDictionary;
        this.LinkTemplate = observed.LinkTemplate;
        this.LinkTemplateDictionary = observed.LinkTemplateDictionary;
      }
    }

    private void ObservedDiagram_ModelReplaced(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
      UpdateModel();
    }

    private void UpdateModel() {
      Diagram observed = this.Observed;
      if (observed == null) return;
      if (observed.PartManager == null) return;
      if (this.Model != observed.Model) {
        EnsureLayoutManager();
        this.PartManager = (PartManager)Activator.CreateInstance(observed.PartManager.GetType());
        this.Model = observed.Model;
      }
    }

    /// <summary>
    /// Make sure the <see cref="Overview"/> is bound to the <see cref="Observed"/> <see cref="Diagram"/>.
    /// </summary>
    public override void OnApplyTemplate() {
      base.OnApplyTemplate();
      BindToDiagram(this.Observed);
    }

    private void DelayBindingForObserved(Object sender, DiagramEventArgs e) {
      Diagram observed = sender as Diagram;
      if (observed != null) {
        observed.TemplateApplied -= DelayBindingForObserved;
        BindToDiagram(observed);
      }
    }

    private void BindToDiagram(Diagram diagram) {
      if (diagram == null) {
        this.Model = null;
        return;
      }

      if (diagram.Model is PartsModel) {
        Diagram.Error("cannot show an Overview of a Diagram with a PartsModel");
        return;
      }

      // Overview's Template not yet applied?
      // Wait for OnApplyTemplate to call BindToDiagram.
      DiagramPanel vpanel = this.Panel;
      if (vpanel == null) return;
      //vpanel.IsVirtualizing = false;

      // Observed diagram's Template not yet applied?
      // When its template has been applied, call DelayBindingForObserved.
      DiagramPanel dpanel = diagram.Panel;
      if (dpanel == null) {
        diagram.TemplateApplied += DelayBindingForObserved;
        return;
      }

      if (!this.BoundEventHandlers) {
        // try to avoid executing this twice
        this.BoundEventHandlers = true;
        dpanel.ViewportBoundsChanged += this.ObservedPanel_ViewportBoundsChanged;
        dpanel.DiagramBoundsChanged += this.ObservedPanel_DiagramBoundsChanged;
        dpanel.PartBoundsChanged += this.ObservedPanel_PartBoundsChanged;
        dpanel.PartVisibleChanged += this.ObservedPanel_PartVisibleChanged;
        dpanel.AnimatingChanged += this.ObservedPanel_AnimatingChanged;
        diagram.TemplatesChanged += this.ObservedDiagram_TemplatesChanged;
        diagram.ModelReplaced += this.ObservedDiagram_ModelReplaced;
      }

      UpdateModel();

      UpdateTemplates();

      UpdateNodesAndLinks();

      BindBox();
    }

    private bool BoundEventHandlers { get; set; }

    private void UnbindFromDiagram(Diagram diagram) {
      if (diagram == null) return;
      DiagramPanel dpanel = diagram.Panel;
      if (dpanel == null) return;

      dpanel.ViewportBoundsChanged -= this.ObservedPanel_ViewportBoundsChanged;
      dpanel.DiagramBoundsChanged -= this.ObservedPanel_DiagramBoundsChanged;
      dpanel.PartBoundsChanged -= this.ObservedPanel_PartBoundsChanged;
      dpanel.PartVisibleChanged -= this.ObservedPanel_PartVisibleChanged;
      dpanel.AnimatingChanged -= this.ObservedPanel_AnimatingChanged;
      diagram.TemplatesChanged -= this.ObservedDiagram_TemplatesChanged;
      diagram.ModelReplaced -= this.ObservedDiagram_ModelReplaced;
      this.BoundEventHandlers = false;
    }

    private void UpdateNodesAndLinks() {
      VerifyAccess();
      if (!this.UpdateNodesAndLinksNeeded) {
        this.UpdateNodesAndLinksNeeded = true;
        Diagram.InvokeLater(this, DoUpdateNodesAndLinks);
      }
    }

    private bool UpdateNodesAndLinksNeeded { get; set; }

    private void DoUpdateNodesAndLinks() {
      VerifyAccess();
      this.UpdateNodesAndLinksNeeded = false;

      Diagram observed = this.Observed;
      if (observed == null) return;
      PartManager obmgr = observed.PartManager;
      if (obmgr == null) return;

      // make sure each Node and Link have the same locations and routes and visibility as those in the Observed diagram
      bool usevis = this.UsesObservedPartVisible;
      if (this.UsesObservedNodeLocation) {
        foreach (Node overviewnode in this.Nodes) {
          Node observednode = obmgr.FindNodeForData(overviewnode.Data, this.Model);
          if (observednode != null) {
            Point ovloc = overviewnode.Location;
            Point obloc = observednode.Location;
            if (!Geo.IsApprox(ovloc, obloc)) overviewnode.Location = obloc;
            if (usevis) overviewnode.Visible = observednode.Visible;
          }
        }
      }
      if (this.UsesObservedLinkRoute) {
        foreach (Link overviewlink in this.Links) {
          Link observedlink = obmgr.FindLinkForData(overviewlink.Data, this.Model);
          if (observedlink != null) {
            overviewlink.Route.Points = observedlink.Route.Points;
            if (usevis) overviewlink.Visible = observedlink.Visible;
          }
        }
      }
      // afterwards, Node.Location is updated dynamically in Overview.ObservedPanel_PartBoundsChanged
      // and Part.Visible is updated dynamically in Overview.ObservedPanel_PartVisibleChanged
    }


    // don't need to do any layout, because we'll get node Locations from the observed diagram's nodes
    private class OverviewLayoutManager : LayoutManager {
      public override void LayoutDiagram(LayoutInitial init, bool immediate) { return; }
      public override bool CanLayoutPart(Part p, Northwoods.GoXam.Layout.IDiagramLayout lay) { return false; }
      public override void InvalidateLayout(Part p, Northwoods.GoXam.Layout.LayoutChange change) {}
      protected override void PerformLayout() {}
    }


    // support for clicking or dragging to move the Box
    private class MoveBoxTool : DiagramTool {
      public override void DoStart() {
        CaptureMouse();
      }
      public override void DoMouseMove() {
        if (this.Active) {
          Overview ov = this.Diagram as Overview;
          if (ov != null && ov.Observed != null) {
            Diagram diag = ov.Observed;
            DiagramPanel obpanel = diag.Panel;
            if (obpanel != null) {
              Rect viewport = obpanel.ViewportBounds;
              Point pt = ov.LastMousePointInModel;
              obpanel.Position = new Point(pt.X-viewport.Width/2, pt.Y-viewport.Height/2);
            }
          }
        }
      }
      public override void DoMouseUp() {
        if (this.Active) {
          DoMouseMove();
        }
        StopTool();
      }
      public override void DoStop() {
        ReleaseMouse();
      }
    }

    //?? allow the OverviewRectangle to be resized
  }
}
