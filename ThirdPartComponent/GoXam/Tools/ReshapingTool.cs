
/*
 *  Copyright © Northwoods Software Corporation, 1998-2010. All Rights Reserved.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Northwoods.GoXam.Tool {

  /// <summary>
  /// The <c>ReshapingBaseTool</c> abstract class is used to interactively change the shape of a <see cref="Part"/>.
  /// </summary>
  /// <remarks>
  /// Currently only one tool subclass is defined: <see cref="LinkReshapingTool"/>.
  /// </remarks>
  [DesignTimeVisible(false)]
  public abstract class ReshapingBaseTool : DiagramTool {
    static ReshapingBaseTool() {
      ReshapeBehaviorProperty = DependencyProperty.RegisterAttached("ReshapeBehavior", typeof(ReshapeBehavior), typeof(ReshapingBaseTool),
        new FrameworkPropertyMetadata(ReshapeBehavior.None));
    }

    /// <summary>
    /// Identifies the <c>ReshapeBehavior</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty ReshapeBehaviorProperty;
    /// <summary>
    /// Gets the value of the <c>ReshapeBehavior</c> attached dependency property.
    /// </summary>
    public static ReshapeBehavior GetReshapeBehavior(DependencyObject d) { return (ReshapeBehavior)d.GetValue(ReshapeBehaviorProperty); }
    /// <summary>
    /// Sets the value of the <c>ReshapeBehavior</c> attached dependency property.
    /// </summary>
    public static void SetReshapeBehavior(DependencyObject d, ReshapeBehavior v) { d.SetValue(ReshapeBehaviorProperty, v); }


    /// <summary>
    /// Gets or sets the current reshape handle that is being dragged.
    /// </summary>
    protected FrameworkElement Handle { get; set; }

    /// <summary>
    /// Gets or sets the current <c>FrameworkElement</c> that is being reshaped.
    /// </summary>
    protected FrameworkElement AdornedElement { get; set; }
  }


  //?? NYI: public class NodeReshapingTool : ReshapingBaseTool { }


  /// <summary>
  /// The <c>LinkReshapingTool</c> is used to interactively change the route of a <see cref="Link"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This tool makes use of an <see cref="Adornment"/>, shown when the <see cref="AdornedLink"/> is selected,
  /// that includes some number of reshape handles.
  /// </para>
  /// <para>
  /// This tool conducts a model edit (<see cref="DiagramTool.StartTransaction"/> and <see cref="DiagramTool.StopTransaction"/>)
  /// while the tool is <see cref="DiagramTool.Active"/>.
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class LinkReshapingTool : ReshapingBaseTool {

    private const String ToolCategory = "ReshapeLink";

    /// <summary>
    /// Gets the current <see cref="Link"/> that the <see cref="ReshapingBaseTool.AdornedElement"/> is in.
    /// </summary>
    protected Link AdornedLink {
      get { return Diagram.FindAncestor<Link>(this.AdornedElement); }
    }

    /// <summary>
    /// Gets or sets the point of the link's route that is being moved.
    /// </summary>
    protected Point OriginalPoint { get; set; }

    /// <summary>
    /// Gets or sets the index of the handle being dragged.
    /// </summary>
    protected int HandleIndex { get; set; }

    /// <summary>
    /// Gets or sets a copy of the <see cref="AdornedLink"/>'s route's initial array of Points.
    /// </summary>
    /// <value>
    /// This is null until it is set by <see cref="DoActivate"/>.
    /// </value>
    protected IList<Point> OriginalPoints { get; set; }


    /// <summary>
    /// Show an <see cref="Adornment"/> with reshape handles at each of the interesting points of the link's <see cref="Route"/>,
    /// if the link is selected and visible and if <see cref="Northwoods.GoXam.Part.CanReshape"/> is true.
    /// </summary>
    /// <param name="part"></param>
    /// <remarks>
    /// <para>
    /// This produces reshape handles at each point of the route, starting with
    /// <see cref="Northwoods.GoXam.Route.FirstPickIndex"/> and ending with
    /// <see cref="Northwoods.GoXam.Route.LastPickIndex"/>.
    /// Depending on whether <see cref="Northwoods.GoXam.Route.Orthogonal"/> is true,
    /// this will call <see cref="ReshapingBaseTool.SetReshapeBehavior"/> and the <c>Cursor</c> to
    /// limit the directions in which the user may drag the handle.
    /// </para>
    /// <para>
    /// You can change the template used to create each reshape handle by setting <see cref="Link.LinkReshapeHandleTemplate"/>.
    /// If that property is null, this uses a default template produces a small square.
    /// </para>
    /// </remarks>
    public override void UpdateAdornments(Part part) {
      Link link = part as Link;
      if (link == null) return;  // no Nodes

      Adornment adornment = null;
      if (link.IsSelected) {
        FrameworkElement selelt = link.Path;
        if (selelt != null && link.CanReshape() && Part.IsVisibleElement(selelt)) {
          adornment = link.GetAdornment(ToolCategory);
          if (adornment == null) {
            Route route = link.Route;
            if (route != null) {
              IEnumerable<Point> pts = route.Points;
              int numpts = route.PointsCount;
              bool ortho = route.Orthogonal;
              if (pts != null && numpts > 2) {
                // LinkReshapeHandleTemplate: for each reshape handle, not for whole adornment
                DataTemplate template = link.LinkReshapeHandleTemplate;
                if (template == null) template = Diagram.FindDefault<DataTemplate>("DefaultLinkReshapeHandleTemplate");
                
                LinkPanel panel = new LinkPanel();
                int firstindex = route.FirstPickIndex;
                int lastindex = route.LastPickIndex;
                // don't bother creating handles for firstindex or lastindex
                for (int i = firstindex+1; i <= lastindex-1; i++) {
                  // expand the DataTemplate to make a copy of a reshape handle
                  FrameworkElement h = template.LoadContent() as FrameworkElement;
                  // needs to be a FrameworkElement so we can set its Cursor
                  if (h == null) continue;
                  // identify this particular handle within the LinkPanel
                  LinkPanel.SetIndex(h, i);
                  // now determines its reshape behavior and cursor, depending on whether Orthogonal et al.
                  if (i == firstindex) {
                    // default ReshapeBehavior.None
                  } else if (i == firstindex+1 && ortho) {
                    Point a = route.GetPoint(firstindex);
                    Point b = route.GetPoint(firstindex+1);
                    if (Geo.IsApprox(a.X, b.X)) {
                      SetReshapeBehavior(h, ReshapeBehavior.Vertical);
                      h.Cursor = Part.SizeNSCursor;
                    } else if (Geo.IsApprox(a.Y, b.Y)) {
                      SetReshapeBehavior(h, ReshapeBehavior.Horizontal);
                      h.Cursor = Part.SizeWECursor;
                    }
                  } else if (i == lastindex-1 && ortho) {
                    Point a = route.GetPoint(lastindex-1);
                    Point b = route.GetPoint(lastindex);
                    if (Geo.IsApprox(a.X, b.X)) {
                      SetReshapeBehavior(h, ReshapeBehavior.Vertical);
                      h.Cursor = Part.SizeNSCursor;
                    } else if (Geo.IsApprox(a.Y, b.Y)) {
                      SetReshapeBehavior(h, ReshapeBehavior.Horizontal);
                      h.Cursor = Part.SizeWECursor;
                    }
                  } else if (i == lastindex) {
                    // default ReshapeBehavior.None
                  } else {
                    SetReshapeBehavior(h, ReshapeBehavior.All);
                    h.Cursor = Part.SizeAllCursor;
                  }
                  panel.Children.Add(h);
                }
                adornment = new Adornment();  // for LinkReshapingTool.UpdateAdornments
                adornment.AdornedElement = selelt;
                adornment.Category = ToolCategory;
                adornment.Content = panel;  // just provide the FrameworkElement as the Content and as the Visual Child
                adornment.LocationSpot = Spot.TopLeft;
              }
            }
          }
          if (adornment != null) {
            Point loc = link.GetElementPoint(selelt, Spot.TopLeft);
            adornment.Location = loc;
            adornment.RotationAngle = link.GetAngle(selelt);
            adornment.Remeasure();
          }
        }
      }
      link.SetAdornment(ToolCategory, adornment);
    }


    /// <summary>
    /// The <see cref="LinkReshapingTool"/> may run when there is a mouse-down event on a reshape handle.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// For this tool to be runnable, the diagram must be modifiable
    /// (not <see cref="Northwoods.GoXam.Diagram.IsReadOnly"/>),
    /// <see cref="Northwoods.GoXam.Diagram.AllowReshape"/> must be true,
    /// <see cref="DiagramTool.IsLeftButtonDown"/> must be true,
    /// and <see cref="DiagramTool.FindToolHandleAt"/> must return a reshape tool handle.
    /// </remarks>
    public override bool CanStart() {
      if (!base.CanStart()) return false;

      Diagram diagram = this.Diagram;
      if (diagram == null || diagram.IsReadOnly) return false;
      if (!diagram.AllowReshape) return false;
      if (!IsLeftButtonDown()) return false;
      FrameworkElement h = FindToolHandleAt(diagram.FirstMousePointInModel, ToolCategory);
      return h != null;
    }


    /// <summary>
    /// Capture the mouse when starting this tool.
    /// </summary>
    public override void DoStart() {
      CaptureMouse();
    }

    /// <summary>
    /// Start reshaping.
    /// </summary>
    /// <remarks>
    /// This sets the <see cref="ReshapingBaseTool.Handle"/> to the result of <see cref="DiagramTool.FindToolHandleAt"/>;
    /// if it is null, this method does not actually activate this tool.
    /// If there is a reshape handle,
    /// this sets <see cref="ReshapingBaseTool.AdornedElement"/> and <see cref="HandleIndex"/>,
    /// remembers the <see cref="OriginalPoint"/>,
    /// starts a transaction (<see cref="DiagramTool.StartTransaction"/>),
    /// and sets <see cref="DiagramTool.Active"/> to true.
    /// </remarks>
    public override void DoActivate() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      this.Handle = FindToolHandleAt(diagram.FirstMousePointInModel, ToolCategory);
      if (this.Handle == null) return;

      this.AdornedElement = FindAdornedElement(this.Handle);
      this.HandleIndex = LinkPanel.GetIndex(this.Handle);
      Link link = this.AdornedLink;  // this should be non-null, checked by FindReshapeHandle
      this.OriginalPoint = link.Route.GetPoint(this.HandleIndex);
      this.OriginalPoints = link.Route.Points.ToList();

      StartTransaction("Reshape");
      this.Active = true;
    }

    /// <summary>
    /// This stops the current edit (<see cref="DiagramTool.StopTransaction"/>).
    /// </summary>
    public override void DoDeactivate() {
      StopTransaction();

      this.Handle = null;
      this.AdornedElement = null;
      this.OriginalPoints = null;
      this.Active = false;
    }

    /// <summary>
    /// Release the mouse capture when stopping this tool.
    /// </summary>
    public override void DoStop() {
      ReleaseMouse();
    }

    /// <summary>
    /// Restore the modified point to be the <see cref="OriginalPoint"/>
    /// and stop this tool.
    /// </summary>
    public override void DoCancel() {
      SetPoints(this.OriginalPoints);
      StopTool();
    }

    /// <summary>
    /// Call <see cref="DoReshape"/> with a new point determined by the mouse.
    /// </summary>
    /// <remarks>
    /// This determines a new reshape point by calling
    /// <see cref="ComputeReshape"/> with the latest mouse point.
    /// The resulting point is passed to <see cref="DoReshape"/>.
    /// </remarks>
    public override void DoMouseMove() {
      Diagram diagram = this.Diagram;
      if (this.Active && diagram != null) {
        Point newpt = ComputeReshape(diagram.LastMousePointInModel);
        DoReshape(newpt);
      }
    }

    /// <summary>
    /// Call <see cref="DoReshape"/> with a point based on the most recent mouse point,
    /// and raise a "link reshaped" event before stopping the tool.
    /// </summary>
    public override void DoMouseUp() {
      Diagram diagram = this.Diagram;
      if (this.Active && diagram != null) {
        Point newpt = ComputeReshape(diagram.LastMousePointInModel);
        DoReshape(newpt);
        SetPoints(this.AdornedLink.Route.Points.ToList());

        diagram.Panel.UpdateDiagramBounds();
        // set the EditResult before raising event, in case it changes the result or cancels the tool
        this.TransactionResult = "Reshaped";
        RaiseEvent(Diagram.LinkReshapedEvent, new DiagramEventArgs(this.AdornedLink));
      }
      StopTool();
    }

    private void SetPoints(IList<Point> points) {
      Link link = this.AdornedLink;
      if (link != null) {
        Route route = link.Route;
        if (route != null && points != null) {
          route.Points = points;
        }
      }
    }

    /// <summary>
    /// Modify the <see cref="Route"/> of the <see cref="AdornedLink"/> to a new point,
    /// considering also which reshape handle is being dragged and whether the route
    /// is <see cref="Route.Orthogonal"/>.
    /// </summary>
    /// <param name="newPoint"></param>
    /// <remarks>
    /// If the <see cref="Route"/> is <see cref="Route.Orthogonal"/> this
    /// modifies the adjacent points as well in order to keep adjacent segments orthogonal.
    /// For handles that are near either end of the route, the movement may be constrained
    /// to be only vertical or only horizontal, in order to maintain orthogonality.
    /// The decision is based on the value of <see cref="ReshapingBaseTool.GetReshapeBehavior"/>
    /// on the <see cref="ReshapingBaseTool.Handle"/>.
    /// This method is called during a "ReshapeLink" transaction.
    /// </remarks>
    protected virtual void DoReshape(Point newPoint) {
      Link link = this.AdornedLink;
      Route route = link.Route;
      ReshapeBehavior behavior = GetReshapeBehavior(this.Handle);
      if (route.Orthogonal) {  // need to adjust adjacent points as well
        if (this.HandleIndex == route.FirstPickIndex+1) {
          int midfirst = route.FirstPickIndex+1;
          if (behavior == ReshapeBehavior.Vertical) {
            // move segment vertically
            route.SetPoint(midfirst, new Point(route.GetPoint(midfirst-1).X, newPoint.Y));
            route.SetPoint(midfirst+1, new Point(route.GetPoint(midfirst+2).X, newPoint.Y));
          } else if (behavior == ReshapeBehavior.Horizontal) {
            // move segment horizontally
            route.SetPoint(midfirst, new Point(newPoint.X, route.GetPoint(midfirst-1).Y));
            route.SetPoint(midfirst+1, new Point(newPoint.X, route.GetPoint(midfirst+2).Y));
          }
        } else if (this.HandleIndex == route.LastPickIndex-1) {
          int midlast = route.LastPickIndex-1;
          if (behavior == ReshapeBehavior.Vertical) {
            // move segment vertically
            route.SetPoint(midlast-1, new Point(route.GetPoint(midlast-2).X, newPoint.Y));
            route.SetPoint(midlast, new Point(route.GetPoint(midlast+1).X, newPoint.Y));
          } else if (behavior == ReshapeBehavior.Horizontal) {
            // move segment horizontally
            route.SetPoint(midlast-1, new Point(newPoint.X, route.GetPoint(midlast-2).Y));
            route.SetPoint(midlast, new Point(newPoint.X, route.GetPoint(midlast+1).Y));
          }
        } else {
          // can move anywhere, but need to keep adjacent segments orthogonal
          int i = this.HandleIndex;
          Point oldpt = route.GetPoint(i);
          Point before = route.GetPoint(i-1);
          Point after = route.GetPoint(i+1);
          if (Geo.IsApprox(before.X, oldpt.X) && Geo.IsApprox(oldpt.Y, after.Y)) {
            route.SetPoint(i-1, new Point(newPoint.X, before.Y));
            route.SetPoint(i+1, new Point(after.X, newPoint.Y));
          } else if (Geo.IsApprox(before.Y, oldpt.Y) && Geo.IsApprox(oldpt.X, after.X)) {
            route.SetPoint(i-1, new Point(before.X, newPoint.Y));
            route.SetPoint(i+1, new Point(newPoint.X, after.Y));
          } else if (Geo.IsApprox(before.X, oldpt.X) && Geo.IsApprox(oldpt.X, after.X)) {
            route.SetPoint(i-1, new Point(newPoint.X, before.Y));
            route.SetPoint(i+1, new Point(newPoint.X, after.Y));
          } else if (Geo.IsApprox(before.Y, oldpt.Y) && Geo.IsApprox(oldpt.Y, after.Y)) {
            route.SetPoint(i-1, new Point(before.X, newPoint.Y));
            route.SetPoint(i+1, new Point(after.X, newPoint.Y));
          }
          route.SetPoint(this.HandleIndex, newPoint);
        }
      } else {  // no Orthogonal constraints, just set the new point
        route.SetPoint(this.HandleIndex, newPoint);
      }
    }

    /// <summary>
    /// Compute the new <c>Point</c> for reshaping a point in the <see cref="Route"/>.
    /// </summary>
    /// <param name="newPoint"></param>
    /// <returns></returns>
    /// <remarks>
    /// The value of <see cref="ReshapingBaseTool.GetReshapeBehavior"/> on the <see cref="ReshapingBaseTool.Handle"/> determines
    /// what reshape constraints exist on that handle, for the <see cref="HandleIndex"/> point of the <see cref="Route"/>.
    /// If there are no reshape constraints, this just returns the value of <paramref name="newPoint"/>.
    /// If the value is <see cref="ReshapeBehavior.Vertical"/>, the Y coordinate may vary
    /// according to the value provided by <paramref name="newPoint"/>, but the X coordinate remains the same.
    /// If the value is <see cref="ReshapeBehavior.Horizontal"/>, the X coordinate may change,
    /// but not the Y value.
    /// If the value is <see cref="ReshapeBehavior.None"/>, the route point may not move.
    /// </remarks>
    protected virtual Point ComputeReshape(Point newPoint) {
      Link link = this.AdornedLink;
      Route route = link.Route;
      switch (GetReshapeBehavior(this.Handle)) {
        case ReshapeBehavior.All:
          return newPoint;  // no constraints
        case ReshapeBehavior.Vertical: {
            Point oldpt = route.GetPoint(this.HandleIndex);
            return new Point(oldpt.X, newPoint.Y);
          }
        case ReshapeBehavior.Horizontal: {
            Point oldpt = route.GetPoint(this.HandleIndex);
            return new Point(newPoint.X, oldpt.Y);
          }
        default:
        case ReshapeBehavior.None:
          return route.GetPoint(this.HandleIndex);  // can't move -- return old point
      }
    }
  }

  /// <summary>
  /// This enumeration lists the permissible drag directions for a reshape handle.
  /// </summary>
  public enum ReshapeBehavior {
    /// <summary>
    /// Disallow dragging.
    /// </summary>
    None,
    /// <summary>
    /// Allow only horizontal (left-and-right) dragging.
    /// </summary>
    Horizontal,
    /// <summary>
    /// Allow only vertical (up-and-down) dragging.
    /// </summary>
    Vertical,
    /// <summary>
    /// Allow dragging in any direction.
    /// </summary>
    All
  }
}
