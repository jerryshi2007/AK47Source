
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
using System.Windows;

namespace Northwoods.GoXam.Tool {

  /// <summary>
  /// The <c>RotatingTool</c> is used to interactively change the angle of an element.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This tool allows the user to rotate the <see cref="Part.SelectionElement"/> of the selected part.
  /// Normally this works with <see cref="Node"/>s; it does not make sense for <see cref="Group"/>s
  /// and <see cref="Link"/>s.
  /// </para>
  /// <para>
  /// You can limit the permitted angles by setting <see cref="SnapAngleMultiple"/> and <see cref="SnapAngleEpsilon"/>.
  /// For example, if you want to permit only angles that are multiples of 90 degrees,
  /// set <see cref="SnapAngleMultiple"/> to 90 and <see cref="SnapAngleEpsilon"/> to 45.
  /// </para>
  /// <para>
  /// This tool makes use of an <see cref="Adornment"/>, shown when the <see cref="AdornedNode"/> is selected,
  /// that includes a rotate handle.
  /// </para>
  /// <para>
  /// This tool conducts a model edit (<see cref="DiagramTool.StartTransaction"/> and <see cref="DiagramTool.StopTransaction"/>)
  /// while the tool is <see cref="DiagramTool.Active"/>.
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class RotatingTool : DiagramTool {

    static RotatingTool() {
      SnapAngleMultipleProperty = DependencyProperty.Register("SnapAngleMultiple", typeof(double), typeof(RotatingTool), new FrameworkPropertyMetadata(45.0));
      SnapAngleEpsilonProperty = DependencyProperty.Register("SnapAngleEpsilon", typeof(double), typeof(RotatingTool), new FrameworkPropertyMetadata(2.0));
    }

    private const String ToolCategory = "Rotate";

    /// <summary>
    /// Identifies the <see cref="SnapAngleMultiple"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SnapAngleMultipleProperty;

    /// <summary>
    /// Gets or sets the preferred angles for the selected element.
    /// </summary>
    /// <value>
    /// The default value is 45 degrees, meaning that angles that are multiples
    /// of 45 degrees are automatically preferred, if the actual angle is
    /// close to that multiple.
    /// The closeness is determined by the <see cref="SnapAngleEpsilon"/> property.
    /// A value of zero for <c>SnapAngleMultiple</c> results in no snapping at all.
    /// </value>
    public double SnapAngleMultiple {
      get { return (double)GetValue(SnapAngleMultipleProperty); }
      set { SetValue(SnapAngleMultipleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SnapAngleEpsilon"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SnapAngleEpsilonProperty;

    /// <summary>
    /// Gets or sets the closeness to a desired angle at which the angle is "snapped to".
    /// </summary>
    /// <value>
    /// The default value is 2 degrees, meaning that any angle within 2 degrees
    /// of a multiple of the <see cref="SnapAngleMultiple"/> automatically
    /// snaps to that multiple.
    /// Values are limited to half of the <see cref="SnapAngleMultiple"/>;
    /// such values restrict user selected angles only to exact multiples of
    /// <see cref="SnapAngleMultiple"/> -- no other angles between them.
    /// </value>
    public double SnapAngleEpsilon {
      get { return (double)GetValue(SnapAngleEpsilonProperty); }
      set { SetValue(SnapAngleEpsilonProperty, value); }
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
    /// Gets or sets the point around which the element is being rotated.
    /// </summary>
    protected Point RotationPoint { get; set; }
    
    /// <summary>
    /// Gets or sets the angle of the <see cref="AdornedElement"/>
    /// when the user started rotating.
    /// </summary>
    protected double OriginalAngle { get; set; }


    /// <summary>
    /// Show an <see cref="Adornment"/> with a rotate handle at a point to the side of the <see cref="AdornedElement"/>,
    /// if the node is selected and visible and if <see cref="Northwoods.GoXam.Part.CanRotate"/> is true.
    /// </summary>
    /// <param name="part"></param>
    /// <remarks>
    /// You can change the template used to create the adornment by setting <see cref="Part.RotateAdornmentTemplate"/>.
    /// If that property is null, this uses a default template that produces a small circle.
    /// </remarks>
    public override void UpdateAdornments(Part part) {
      if (part == null || part is Link) return;  // this tool never applies to Links
      Adornment adornment = null;
      if (part.IsSelected) {
        FrameworkElement selelt = part.SelectionElement;
        if (selelt != null && part.CanRotate() && Part.IsVisibleElement(selelt)) {
          adornment = part.GetAdornment(ToolCategory);
          if (adornment == null) {
            DataTemplate template = part.RotateAdornmentTemplate;
            if (template == null) template = Diagram.FindDefault<DataTemplate>("DefaultRotateAdornmentTemplate");
            adornment = part.MakeAdornment(selelt, template);
            if (adornment != null) {
              adornment.Category = ToolCategory;
              adornment.LocationSpot = Spot.Center;
            }
          }
          if (adornment != null) {
            Rect rect = part.GetElementBounds(part.VisualElement);  //?? outside whole node
            Point center = Spot.Center.PointInRect(rect);
            double angle = part.GetAngle(selelt);
            adornment.Location = Geo.RotatePoint(new Point(rect.Right + 30, center.Y), center, angle);  //?? fixed distance
          }
        }
      }
      part.SetAdornment(ToolCategory, adornment);
    }


    /// <summary>
    /// The <see cref="RotatingTool"/> may run when there is a mouse-down event on a rotate handle.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// For this tool to be runnable, the diagram must be modifiable
    /// (not <see cref="Northwoods.GoXam.Diagram.IsReadOnly"/>),
    /// <see cref="Northwoods.GoXam.Diagram.AllowRotate"/> must be true,
    /// <see cref="DiagramTool.IsLeftButtonDown"/> must be true,
    /// and <see cref="DiagramTool.FindToolHandleAt"/> must return a "Rotate" tool handle.
    /// </remarks>
    public override bool CanStart() {
      if (!base.CanStart()) return false;

      Diagram diagram = this.Diagram;
      if (diagram == null || diagram.IsReadOnly) return false;
      if (!diagram.AllowRotate) return false;
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
    /// Start rotation.
    /// </summary>
    /// <remarks>
    /// This sets the <see cref="Handle"/> to the result of calling <see cref="DiagramTool.FindToolHandleAt"/>;
    /// if it is null, this method does not actually activate this tool.
    /// If there is a rotate handle,
    /// this sets <see cref="AdornedElement"/>,
    /// computes the <see cref="RotationPoint"/>,
    /// remembers the <see cref="OriginalAngle"/>,
    /// starts a transaction (<see cref="DiagramTool.StartTransaction"/>),
    /// and sets <see cref="DiagramTool.Active"/> to true.
    /// </remarks>
    public override void DoActivate() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      this.Handle = FindToolHandleAt(diagram.FirstMousePointInModel, ToolCategory);
      if (this.Handle == null) return;

      this.AdornedElement = FindAdornedElement(this.Handle);
      this.RotationPoint = this.AdornedNode.GetElementPoint(this.AdornedNode.VisualElement, Spot.Center);
      this.OriginalAngle = this.AdornedNode.GetAngle(this.AdornedElement);

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
    /// Restore the <see cref="OriginalAngle"/> of the <see cref="AdornedElement"/>
    /// and stop this tool.
    /// </summary>
    public override void DoCancel() {
      DoRotate(this.OriginalAngle);
      StopTool();
    }

    /// <summary>
    /// Call <see cref="DoRotate"/> with a new angle determined by the mouse point.
    /// </summary>
    /// <remarks>
    /// This determines a new angle by calling
    /// <see cref="ComputeRotate"/> with the latest mouse point.
    /// The resulting angle is passed to <see cref="DoRotate"/>.
    /// </remarks>
    public override void DoMouseMove() {
      Diagram diagram = this.Diagram;
      if (this.Active && diagram != null) {
        double newangle = ComputeRotate(diagram.LastMousePointInModel);
        DoRotate(newangle);
      }
    }

    /// <summary>
    /// Call <see cref="DoRotate"/> with an angle based on the most recent mouse point
    /// and raise an "object rotated" event before stopping the tool.
    /// </summary>
    public override void DoMouseUp() {
      Diagram diagram = this.Diagram;
      if (this.Active && diagram != null) {
        double newangle = ComputeRotate(diagram.LastMousePointInModel);
        DoRotate(newangle);

        diagram.Panel.UpdateDiagramBounds();
        // set the EditResult before raising event, in case it changes the result or cancels the tool
        this.TransactionResult = ToolCategory;
        RaiseEvent(Diagram.NodeRotatedEvent, new DiagramEventArgs(this.AdornedNode, this.AdornedElement));
      }
      StopTool();
    }


    /// <summary>
    /// Change the angle of the <see cref="AdornedElement"/>.
    /// </summary>
    /// <param name="newangle">the new angle, in degrees</param>
    /// <remarks>
    /// This is called within a "Rotate" transaction.
    /// </remarks>
    protected virtual void DoRotate(double newangle) {
      Node node = this.AdornedNode;
      if (node != null) {
        this.AdornedNode.SetAngle(this.AdornedElement, newangle);
      }
    }

    /// <summary>
    /// Compute the new angle given a <c>Point</c>.
    /// </summary>
    /// <param name="newPoint">a <c>Point</c> in model coordinates</param>
    /// <returns>the new angle, in degrees</returns>
    /// <remarks>
    /// If the angle is close (by <see cref="SnapAngleEpsilon"/> degrees)
    /// to a multiple of <see cref="SnapAngleMultiple"/> degrees, make it
    /// exactly that multiple.
    /// </remarks>
    protected virtual double ComputeRotate(Point newPoint) {
      double dx = newPoint.X - this.RotationPoint.X;
      double dy = newPoint.Y - this.RotationPoint.Y;
      double a = Geo.GetAngle(dx, dy);  // apparent angle from RotationPoint to newPoint

      double interval = Math.Min(Math.Abs(this.SnapAngleMultiple), 180);
      double epsilon = Math.Min(Math.Abs(this.SnapAngleEpsilon), interval/2);
      // if it's close to a multiple of INTERVAL degrees, make it exactly so
      if (!IsShiftKeyDown() && interval > 0 && epsilon > 0) {
        if (a % interval < epsilon) {
          a = ((int)(a / interval)) * interval;
        } else if (a % interval > interval - epsilon) {
          a = ((int)(a / interval) + 1) * interval;
        }
      }

      double ang = a;
      if (ang >= 360) ang -= 360;
      else if (ang < 0) ang += 360;
      return ang;
    }

  }
}
