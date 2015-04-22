
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
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Northwoods.GoXam.Tool {

  /// <summary>
  /// The <c>ResizingTool</c> is used to interactively change the size of an element.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This tool allows the user to resize the <see cref="Part.SelectionElement"/> of the selected part.
  /// Normally this works with <see cref="Node"/>s; it does not make sense for <see cref="Group"/>s
  /// and <see cref="Link"/>s.
  /// </para>
  /// <para>
  /// You can limit the permitted minimum and maximum dimensions by setting <see cref="MinSize"/> and <see cref="MaxSize"/>.
  /// For example,
  /// <code>
  ///   &lt;go:Diagram.ResizingTool&gt;
  ///     &lt;go:ResizingTool MinSize="10 10" MaxSize="100 200"/&gt;
  ///   &lt;/go:Diagram.ResizingTool&gt;
  /// </code>
  /// will make sure every interactive resizing operation results in the <see cref="AdornedElement"/>
  /// in having a width between 10 and 100 and in having a height between 10 and 200.
  /// Furthermore,
  /// <code>
  ///   &lt;go:Diagram.ResizingTool&gt;
  ///     &lt;go:ResizingTool MinSize="NaN 10" MaxSize="NaN 200"/&gt;
  ///   &lt;/go:Diagram.ResizingTool&gt;
  /// </code>
  /// will restrict the width to be whatever the <see cref="AdornedElement"/>'s original width was,
  /// whereas the height may be varied between 10 and 200.
  /// </para>
  /// <para>
  /// The resizing will also respect the <c>FrameworkElement</c> properties: <c>MinWidth</c>,
  /// <c>MaxWidth</c>, <c>MinHeight</c>, and <c>MaxHeight</c>, that are one the <see cref="AdornedElement"/>.
  /// </para>
  /// <para>
  /// You can also limit the width and/or height to be multiples of a particular size by
  /// setting <see cref="CellSize"/>.  If either or both of these values are <c>NaN</c>,
  /// as they are by default,
  /// it will get the values from the node being resized: <see cref="Part.ResizeCellSize"/>.
  /// </para>
  /// <para>
  /// If either or both of the width and height are still <c>NaN</c>,
  /// it will look for a snapper node behind the <see cref="AdornedNode"/>
  /// (a node that has <see cref="Part.DragOverSnapEnabled"/> true)
  /// and then use its <see cref="Part.DragOverSnapCellSize"/>.
  /// If it cannot find any snapper node, it uses the diagram's
  /// <see cref="Northwoods.GoXam.Diagram.GridSnapCellSize"/> if the diagram's
  /// <see cref="Northwoods.GoXam.Diagram.GridSnapEnabled"/> is true.
  /// </para>
  /// <para>
  /// This tool makes use of an <see cref="Adornment"/>, shown when the <see cref="AdornedNode"/> is selected,
  /// that includes some number of resize handles.
  /// </para>
  /// <para>
  /// This tool conducts a model edit (<see cref="DiagramTool.StartTransaction"/> and <see cref="DiagramTool.StopTransaction"/>)
  /// while the tool is <see cref="DiagramTool.Active"/>.
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class ResizingTool : DiagramTool {

    static ResizingTool() {
      MaxSizeProperty = DependencyProperty.Register("MaxSize", typeof(Size), typeof(ResizingTool), new FrameworkPropertyMetadata(new Size(9999, 9999)));
      MinSizeProperty = DependencyProperty.Register("MinSize", typeof(Size), typeof(ResizingTool), new FrameworkPropertyMetadata(new Size(1, 1)));
      CellSizeProperty = DependencyProperty.Register("CellSize", typeof(Size), typeof(ResizingTool), new FrameworkPropertyMetadata(new Size(Double.NaN, Double.NaN)));
    }

    private const String ToolCategory = "Resize";

    /// <summary>
    /// Identifies the <see cref="MaxSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaxSizeProperty;

    /// <summary>
    /// Gets or sets the maximum size of the selected element.
    /// </summary>
    /// <value>
    /// The default value is 9999 x 9999.
    /// The width and height must not be negative.
    /// A width of <c>Double.NaN</c> uses the <see cref="AdornedElement"/>'s original width;
    /// a height of <c>Double.NaN</c> uses its original height (<see cref="OriginalBounds"/>).
    /// </value>

    [TypeConverter(typeof(Route.SizeConverter))]

    public Size MaxSize {
      get { return (Size)GetValue(MaxSizeProperty); }
      set { SetValue(MaxSizeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MinSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MinSizeProperty;

    /// <summary>
    /// Gets or sets the minimum size of the selected element.
    /// </summary>
    /// <value>
    /// The default value is 1 x 1.
    /// The width and height must not be negative or infinity.
    /// A width of <c>Double.NaN</c> uses the <see cref="AdornedElement"/>'s original width;
    /// a height of <c>Double.NaN</c> uses its original height (<see cref="OriginalBounds"/>).
    /// </value>

    [TypeConverter(typeof(Route.SizeConverter))]

    public Size MinSize {
      get { return (Size)GetValue(MinSizeProperty); }
      set { SetValue(MinSizeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CellSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CellSizeProperty;

    /// <summary>
    /// Gets or sets the cell size of the selected element, so that resizing is limited to multiples of the cell size.
    /// </summary>
    /// <value>
    /// The default value is NaN x NaN.
    /// A value of NaN means that <see cref="ComputeCellSize"/>
    /// will look at its <see cref="AdornedNode"/>'s <see cref="Part.ResizeCellSize"/>.
    /// Failing to find a positive number, it will look for a node underneath it
    /// that is <see cref="Part.DragOverSnapEnabled"/> to use its <see cref="Part.DragOverSnapCellSize"/>,
    /// or for the diagram if it is <see cref="Northwoods.GoXam.Diagram.GridSnapEnabled"/>
    /// to use its <see cref="Northwoods.GoXam.Diagram.GridSnapCellSize"/>.
    /// The width and height must not be negative or infinity.
    /// </value>

    [TypeConverter(typeof(Route.SizeConverter))]

    public Size CellSize {
      get { return (Size)GetValue(CellSizeProperty); }
      set { SetValue(CellSizeProperty, value); }
    }


    /// <summary>
    /// Gets or sets the current resize handle that is being dragged.
    /// </summary>
    protected FrameworkElement Handle { get; set; }

    /// <summary>
    /// Gets or sets the current <c>FrameworkElement</c> that is being resized.
    /// </summary>
    protected FrameworkElement AdornedElement { get; set; }

    /// <summary>
    /// Gets the current <see cref="Node"/> that the <see cref="AdornedElement"/> is in.
    /// </summary>
    protected Node AdornedNode {
      get { return Diagram.FindAncestor<Node>(this.AdornedElement); }
    }

    /// <summary>
    /// Gets or sets the bounds, in model coordinates, of the <see cref="AdornedElement"/>
    /// when the user started resizing.
    /// </summary>
    protected Rect OriginalBounds { get; set; }  // of the AdornedElement, not of the AdornedNode

    /// <summary>
    /// Gets or sets the location, in model coordinates, of the <see cref="AdornedNode"/>
    /// when the user started resizing.
    /// </summary>
    protected Point OriginalLocation { get; set; }  // of the AdornedNode

    private double Angle { get; set; }
    private Size EffectiveCellSize { get; set; }


    /// <summary>
    /// Show an <see cref="Adornment"/> with resize handles at points along the edge of bounds of the <see cref="AdornedElement"/>,
    /// if the node is selected and visible and if <see cref="Northwoods.GoXam.Part.CanResize"/> is true.
    /// </summary>
    /// <param name="part"></param>
    /// <remarks>
    /// You can change the template used to create the adornment by setting <see cref="Part.ResizeAdornmentTemplate"/>.
    /// If that property is null, this uses a default template with eight resize handles
    /// at the four corners and at the middles of the four sides.
    /// </remarks>
    public override void UpdateAdornments(Part part) {
      if (part == null || part is Link) return;  // can't resize links
      Adornment adornment = null;
      if (part.IsSelected) {
        FrameworkElement selelt = part.SelectionElement;
        if (selelt != null && part.CanResize() && Part.IsVisibleElement(selelt)) {
          adornment = part.GetAdornment(ToolCategory);
          if (adornment == null) {
            DataTemplate template = part.ResizeAdornmentTemplate;
            if (template == null) template = Diagram.FindDefault<DataTemplate>("DefaultResizeAdornmentTemplate");
            adornment = part.MakeAdornment(selelt, template);
            if (adornment != null) {
              adornment.Category = ToolCategory;
              adornment.LocationSpot = Spot.TopLeft;
            }
          }
          if (adornment != null) {
            Point loc = part.GetElementPoint(selelt, Spot.TopLeft);
            double angle = part.GetAngle(selelt);
            UpdateResizeHandles(adornment.VisualElement, angle);
            adornment.Location = loc;
            adornment.RotationAngle = angle;
            adornment.Remeasure();
          }
        }
      }
      part.SetAdornment(ToolCategory, adornment);
    }

    private void UpdateResizeHandles(FrameworkElement elt, double angle) {
      if (elt == null) return;
      FrameworkElement h = AsToolHandle(elt);
      if (h != null) {
        SetResizeCursor(h, angle);
      } else {
        int count = VisualTreeHelper.GetChildrenCount(elt);
        for (int i = 0; i < count; i++) {
          UpdateResizeHandles(VisualTreeHelper.GetChild(elt, i) as FrameworkElement, angle);
        }
      }
    }

    private void SetResizeCursor(FrameworkElement h, double angle) {
      Spot spot = SpotPanel.GetSpot(h);
      if (spot.IsNoSpot) spot = Spot.Center;
      double a = angle;
      if (spot.X <= 0) {  // left
        if (spot.Y <= 0) {  // top-left
          a += 225;
        } else if (spot.Y >= 1) {  // bottom-left
          a += 135;
        } else {  // middle-left
          a += 180;
        }
      } else if (spot.X >= 1) {  // right
        if (spot.Y <= 0) {  // top-right
          a += 315;
        } else if (spot.Y >= 1) {  // bottom-right
          a += 45;
        } else {  // middle-right
          // a += 0;
        }
      } else {  // middle-X
        if (spot.Y <= 0) {  // top-middle
          a += 270;
        } else if (spot.Y >= 1) {  // bottom-middle
          a += 90;
        } else {
          // handle is in the middle-middle -- don't do anything
          return;
        }
      }
      if (a < 0) a += 360;
      else if (a >= 360) a -= 360;
      if (a < 22.5f)
        h.Cursor = Part.SizeWECursor;
      else if (a < 67.5f)
        h.Cursor = Part.SizeNWSECursor;
      else if (a < 112.5f)
        h.Cursor = Part.SizeNSCursor;
      else if (a < 157.5f)
        h.Cursor = Part.SizeNESWCursor;
      else if (a < 202.5f)
        h.Cursor = Part.SizeWECursor;
      else if (a < 247.5f)
        h.Cursor = Part.SizeNWSECursor;
      else if (a < 292.5f)
        h.Cursor = Part.SizeNSCursor;
      else if (a < 337.5f)
        h.Cursor = Part.SizeNESWCursor;
      else
        h.Cursor = Part.SizeWECursor;
    }


    /// <summary>
    /// The <see cref="ResizingTool"/> may run when there is a mouse-down event on a resize handle.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// For this tool to be runnable, the diagram must be modifiable
    /// (not <see cref="Northwoods.GoXam.Diagram.IsReadOnly"/>),
    /// <see cref="Northwoods.GoXam.Diagram.AllowResize"/> must be true,
    /// <see cref="DiagramTool.IsLeftButtonDown"/> must be true,
    /// and <see cref="DiagramTool.FindToolHandleAt"/> must return a "Resize" tool handle.
    /// </remarks>
    public override bool CanStart() {
      if (!base.CanStart()) return false;

      Diagram diagram = this.Diagram;
      if (diagram == null || diagram.IsReadOnly) return false;
      if (!diagram.AllowResize) return false;
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
    /// Start resizing.
    /// </summary>
    /// <remarks>
    /// This sets the <see cref="Handle"/> to the result of <see cref="DiagramTool.FindToolHandleAt"/>;
    /// if it is null, this method does not actually activate this tool.
    /// If there is a resize handle,
    /// this sets <see cref="AdornedElement"/>,
    /// remembers the <see cref="OriginalBounds"/> and <see cref="OriginalLocation"/>,
    /// starts a transaction (<see cref="DiagramTool.StartTransaction"/>),
    /// and sets <see cref="DiagramTool.Active"/> to true.
    /// </remarks>
    public override void DoActivate() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      this.Handle = FindToolHandleAt(diagram.FirstMousePointInModel, ToolCategory);
      if (this.Handle == null) return;

      this.AdornedElement = FindAdornedElement(this.Handle);
      this.Angle = this.AdornedNode.GetAngle(this.AdornedElement);
      this.AdornedNode.SetAngle(this.AdornedElement, 0);
      this.OriginalBounds = this.AdornedNode.GetElementBounds(this.AdornedElement);
      this.AdornedNode.SetAngle(this.AdornedElement, this.Angle);
      this.OriginalLocation = this.AdornedNode.Location;
      this.EffectiveCellSize = ComputeCellSize();

      StartTransaction(ToolCategory);
      this.Active = true;
    }

    /// <summary>
    /// This stops the current edit (<see cref="DiagramTool.StopTransaction"/>).
    /// </summary>
    public override void DoDeactivate() {
      StopTransaction();
      this.Handle = null;
      this.AdornedElement = null;
      this.Active = false;
    }

    /// <summary>
    /// Release the mouse capture when stopping this tool.
    /// </summary>
    public override void DoStop() {
      ReleaseMouse();
    }

    /// <summary>
    /// Restore the <see cref="OriginalBounds"/> of the <see cref="AdornedElement"/>
    /// and stop this tool.
    /// </summary>
    public override void DoCancel() {
      FrameworkElement elt = this.AdornedElement;
      if (elt != null) {
        elt.Width = this.OriginalBounds.Width;
        elt.Height = this.OriginalBounds.Height;
      }
      if (this.AdornedNode != null) {
        this.AdornedNode.Location = this.OriginalLocation;
      }
      StopTool();
    }

    /// <summary>
    /// Call <see cref="DoResize"/> with a new bounds determined by the mouse point.
    /// </summary>
    /// <remarks>
    /// This determines a new resize bounds by calling
    /// <see cref="ComputeResize"/> with the latest mouse point
    /// limited by <see cref="ComputeMinSize"/> and <see cref="ComputeMaxSize"/>.
    /// The resulting rectangle is passed to <see cref="DoResize"/>.
    /// </remarks>
    public override void DoMouseMove() {
      Diagram diagram = this.Diagram;
      if (this.Active && diagram != null) {
        Size minSize = ComputeMinSize();
        Size maxSize = ComputeMaxSize();
        Rect newr = ComputeResize(diagram.LastMousePointInModel, SpotPanel.GetSpot(this.Handle), minSize, maxSize, true);
        DoResize(newr);
      }
    }

    /// <summary>
    /// Call <see cref="DoResize"/> with a rectangle based on the most recent mouse point,
    /// and raise an "object resized" event before stopping the tool.
    /// </summary>
    public override void DoMouseUp() {
      Diagram diagram = this.Diagram;
      if (this.Active && diagram != null) {
        Size minSize = ComputeMinSize();
        Size maxSize = ComputeMaxSize();
        Rect newr = ComputeResize(diagram.LastMousePointInModel, SpotPanel.GetSpot(this.Handle), minSize, maxSize, true);
        DoResize(newr);

        diagram.Panel.UpdateDiagramBounds();
        // set the EditResult before raising event, in case it changes the result or cancels the tool
        this.TransactionResult = ToolCategory;
        RaiseEvent(Diagram.NodeResizedEvent, new DiagramEventArgs(this.AdornedNode, this.AdornedElement));
      }
      StopTool();
    }


    /// <summary>
    /// Change the <see cref="AdornedElement"/>'s <c>Width</c> and/or <c>Height</c>,
    /// and perhaps the <see cref="AdornedNode"/>'s <see cref="Node.Location"/>,
    /// given a new rectangular bounds for the adorned element.
    /// </summary>
    /// <param name="newr"></param>
    /// <remarks>
    /// This is called within a "Resize" transaction.
    /// </remarks>
    protected virtual void DoResize(Rect newr) {
      Node node = this.AdornedNode;
      if (node != null) {
        Rect oldr = this.OriginalBounds;
        Point oldloc = this.OriginalLocation;
        Point newloc = oldloc;
        if (oldloc.X > oldr.Right) {
          newloc.X = newr.X + newr.Width + oldloc.X - oldr.Right;
        } else if (oldloc.X > oldr.X && oldr.Width > 0) {
          double fx = (oldloc.X-oldr.X)/oldr.Width;
          newloc.X = newr.X + fx*newr.Width;
        } else {
          newloc.X = newr.X + (oldloc.X - oldr.X);
        }
        if (oldloc.Y > oldr.Bottom) {
          newloc.Y = newr.Y + newr.Height + oldloc.Y - oldr.Bottom;
        } else if (oldloc.Y > oldr.Y && oldr.Height > 0) {
          double fx = (oldloc.Y-oldr.Y)/oldr.Height;
          newloc.Y = newr.Y + fx*newr.Height;
        } else {
          newloc.Y = newr.Y + (oldloc.Y - oldr.Y);
        }
        Point rotloc = Geo.RotatePoint(newloc, oldloc, this.Angle);
        //Diagram.Debug(Diagram.Str(oldloc) + Diagram.Str(newloc) + Diagram.Str(rotloc));
        node.Location = rotloc;
        FrameworkElement elt = this.AdornedElement;
        if (elt != null) {
          //?? depending on type of element, maybe should scale transform
          elt.Width = newr.Width;
          elt.Height = newr.Height;
          node.InvalidateVisual(elt);
        }
      }
    }

    /// <summary>
    /// Given a <c>Spot</c> in the <see cref="OriginalBounds"/> and a new <c>Point</c>,
    /// compute the new <c>Rect</c>.
    /// </summary>
    /// <param name="newPoint"></param>
    /// <param name="spot">The <see cref="Spot"/> in the rectangular bounds for which the <see cref="Handle"/> is being dragged</param>
    /// <param name="min">the computed rectangle must be at least as large as these dimensions</param>
    /// <param name="max">the computed rectangle must be no larger than these dimensions</param>
    /// <param name="reshape">whether the aspect ratio of the rectangle may change</param>
    /// <returns></returns>
    protected virtual Rect ComputeResize(Point newPoint, Spot spot, Size min, Size max, bool reshape) {
      if (spot.IsNoSpot) spot = Spot.Center;
      newPoint = Geo.RotatePoint(newPoint, Spot.Center.PointInRect(this.OriginalBounds), -this.Angle);

      Rect origRect = this.OriginalBounds;
      double left = origRect.X;
      double top = origRect.Y;
      double right = origRect.X + origRect.Width;
      double bottom = origRect.Y + origRect.Height;

      double oldratio = 1;
      if (!reshape) {
        double oldw = origRect.Width;
        double oldh = origRect.Height;
        if (oldw <= 0) oldw = 1;
        if (oldh <= 0) oldh = 1;
        oldratio = oldh/oldw;
      }

      Size cellSize = this.EffectiveCellSize;
      newPoint = DraggingTool.FindNearestInfiniteGridPoint(newPoint, new Point(0, 0), new Point(left, top), cellSize);

      Rect newRect = origRect;
      if (spot.X <= 0) {  // left
        if (spot.Y <= 0) {  // top-left
          newRect.X = Math.Max(newPoint.X, right - max.Width);
          newRect.X = Math.Min(newRect.X, right - min.Width);
          newRect.Width = Math.Max(right - newRect.X, min.Width);

          newRect.Y = Math.Max(newPoint.Y, bottom - max.Height);
          newRect.Y = Math.Min(newRect.Y, bottom - min.Height);
          newRect.Height = Math.Max(bottom - newRect.Y, min.Height);

          if (!reshape) {
            double newratio = newRect.Height/newRect.Width;
            if (oldratio < newratio) {
              newRect.Height = oldratio*newRect.Width;
              newRect.Y = bottom - newRect.Height;
            } else {
              newRect.Width = newRect.Height/oldratio;
              newRect.X = right - newRect.Width;
            }
          }
        } else if (spot.Y >= 1) {  // bottom-left
          newRect.X = Math.Max(newPoint.X, right - max.Width);
          newRect.X = Math.Min(newRect.X, right - min.Width);
          newRect.Width = Math.Max(right - newRect.X, min.Width);

          newRect.Height = Math.Max(Math.Min(newPoint.Y - top, max.Height), min.Height);

          if (!reshape) {
            double newratio = newRect.Height/newRect.Width;
            if (oldratio < newratio) {
              newRect.Height = oldratio*newRect.Width;
            } else {
              newRect.Width = newRect.Height/oldratio;
              newRect.X = right - newRect.Width;
            }
          }
        } else {  // middle-left
          newRect.X = Math.Max(newPoint.X, right - max.Width);
          newRect.X = Math.Min(newRect.X, right - min.Width);
          newRect.Width = Math.Max(right - newRect.X, min.Width);
        }
      } else if (spot.X >= 1) {  // right
        if (spot.Y <= 0) {  // top-right
          newRect.Width = Math.Max(Math.Min(newPoint.X - left, max.Width), min.Width);

          newRect.Y = Math.Max(newPoint.Y, bottom - max.Height);
          newRect.Y = Math.Min(newRect.Y, bottom - min.Height);
          newRect.Height = Math.Max(bottom - newRect.Y, min.Height);

          if (!reshape) {
            double newratio = newRect.Height/newRect.Width;
            if (oldratio < newratio) {
              newRect.Height = oldratio*newRect.Width;
              newRect.Y = bottom - newRect.Height;
            } else {
              newRect.Width = newRect.Height/oldratio;
            }
          }
        } else if (spot.Y >= 1) {  // bottom-right
          newRect.Width = Math.Max(Math.Min(newPoint.X - left, max.Width), min.Width);
          newRect.Height = Math.Max(Math.Min(newPoint.Y - top, max.Height), min.Height);

          if (!reshape) {
            double newratio = newRect.Height/newRect.Width;
            if (oldratio < newratio) {
              newRect.Height = oldratio*newRect.Width;
            } else {
              newRect.Width = newRect.Height/oldratio;
            }
          }
        } else {  // middle-right
          newRect.Width = Math.Max(Math.Min(newPoint.X - left, max.Width), min.Width);
        }
      } else {  // middle-X
        if (spot.Y <= 0) {  // top-middle
          newRect.Y = Math.Max(newPoint.Y, bottom - max.Height);
          newRect.Y = Math.Min(newRect.Y, bottom - min.Height);
          newRect.Height = Math.Max(bottom - newRect.Y, min.Height);
        } else if (spot.Y >= 1) {  // bottom-middle
          newRect.Height = Math.Max(Math.Min(newPoint.Y - top, max.Height), min.Height);
        } else {
          // handle is in the middle-middle -- don't do anything
        }
      }
      return newRect;
    }

    /// <summary>
    /// The effective minimum resizing size is the maximum of <see cref="MinSize"/> and the
    /// <c>FrameworkElement</c>'s <c>MinWidth</c> and <c>MinHeight</c>.
    /// </summary>
    /// <returns>the desired minimum <c>Size</c> during resizing</returns>
    protected virtual Size ComputeMinSize() {
      Size minSize = this.MinSize;
      if (Double.IsNaN(minSize.Width)) minSize.Width = this.OriginalBounds.Width;
      if (Double.IsNaN(minSize.Height)) minSize.Height = this.OriginalBounds.Height;
      FrameworkElement elt = this.AdornedElement;
      if (elt != null) {
        minSize.Width = Math.Max(minSize.Width, elt.MinWidth);
        minSize.Height = Math.Max(minSize.Height, elt.MinHeight);
      }
      return minSize;
    }

    /// <summary>
    /// The effective maximum resizing size is the minimum of <see cref="MaxSize"/> and the
    /// <c>FrameworkElement</c>'s <c>MaxWidth</c> and <c>MaxHeight</c>.
    /// </summary>
    /// <returns>the desired maximum <c>Size</c> during resizing</returns>
    protected virtual Size ComputeMaxSize() {
      Size maxSize = this.MaxSize;
      if (Double.IsNaN(maxSize.Width)) maxSize.Width = this.OriginalBounds.Width;
      if (Double.IsNaN(maxSize.Height)) maxSize.Height = this.OriginalBounds.Height;
      FrameworkElement elt = this.AdornedElement;
      if (elt != null) {
        maxSize.Width = Math.Min(maxSize.Width, elt.MaxWidth);
        maxSize.Height = Math.Min(maxSize.Height, elt.MaxHeight);
      }
      return maxSize;
    }

    /// <summary>
    /// The size should be a multiple of the value returned by this method.
    /// </summary>
    /// <returns>
    /// The <see cref="CellSize"/>, unless one or both of its width and height are <c>Double.NaN</c> or zero.
    /// If that is the case, it uses the width and/or height from the node's <see cref="Part.ResizeCellSize"/>.
    /// But if one or both of the width and height are still <c>Double.NaN</c> or zero,
    /// it searches for a grid snapping node behind the <see cref="AdornedNode"/>,
    /// one that is <see cref="Part.DragOverSnapEnabled"/>,
    /// and uses its <see cref="Part.DragOverSnapCellSize"/>.
    /// If it cannot find such a grid snapping node, or one or both of the width and height are <c>NaN</c>,
    /// it uses the diagram's <see cref="Northwoods.GoXam.Diagram.GridSnapCellSize"/>
    /// if the diagram is <see cref="Northwoods.GoXam.Diagram.GridSnapEnabled"/>.
    /// Finally, it defaults to 1x1.
    /// </returns>
    protected virtual Size ComputeCellSize() {
      Size cellSize = this.CellSize;
      if (!Double.IsNaN(cellSize.Width) && cellSize.Width > 0 &&
          !Double.IsNaN(cellSize.Height) && cellSize.Height > 0) return cellSize;

      Node node = this.AdornedNode;
      Diagram diagram = this.Diagram;
      if (node != null && diagram != null) {
        Size csz = node.ResizeCellSize;
        if (Double.IsNaN(cellSize.Width)) cellSize.Width = csz.Width;
        if (Double.IsNaN(cellSize.Height)) cellSize.Height = csz.Height;
        if (!Double.IsNaN(cellSize.Width) && cellSize.Width > 0 &&
            !Double.IsNaN(cellSize.Height) && cellSize.Height > 0) return cellSize;

        if (diagram.Panel != null) {
          bool found = false;
          Node snapper = diagram.Panel.FindElementAt<Node>(this.AdornedNode.Location, Diagram.FindAncestor<Node>,
            n => {
              if (n == this.AdornedNode) {
                found = true;
                return false;
              }
              if (!found) return false;
              return n.DragOverSnapEnabled;
            }, SearchLayers.Nodes);
          if (snapper != null) {
            csz = snapper.DragOverSnapCellSize;
            if (Double.IsNaN(cellSize.Width)) cellSize.Width = csz.Width;
            if (Double.IsNaN(cellSize.Height)) cellSize.Height = csz.Height;
          }
          if (!Double.IsNaN(cellSize.Width) && cellSize.Width > 0 &&
              !Double.IsNaN(cellSize.Height) && cellSize.Height > 0) return cellSize;
        }

        if (diagram.GridSnapEnabled) {
          csz = diagram.GridSnapCellSize;
          if (Double.IsNaN(cellSize.Width)) cellSize.Width = csz.Width;
          if (Double.IsNaN(cellSize.Height)) cellSize.Height = csz.Height;
        }
      }

      // finally, default to 1
      if (Double.IsNaN(cellSize.Width) || cellSize.Width == 0) cellSize.Width = 1;
      if (Double.IsNaN(cellSize.Height) || cellSize.Height == 0) cellSize.Height = 1;
      return cellSize;
    }

  }
}
