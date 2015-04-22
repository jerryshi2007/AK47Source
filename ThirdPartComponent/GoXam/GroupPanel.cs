
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
using System.Linq;
using System.Windows;

namespace Northwoods.GoXam {

  /// <summary>
  /// An auto-resizing <see cref="SpotPanel"/> that always surrounds its <see cref="Group"/>'s
  /// <see cref="Group.MemberNodes"/> plus some <see cref="Padding"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Note that the member nodes of a group are NOT children of this panel.
  /// Typically only decorative elements and ports are part of a <c>GroupPanel</c>.
  /// </para>
  /// <para>
  /// A node template representing a group, if it contains a <c>GroupPanel</c>,
  /// must name that <c>GroupPanel</c> as the node's <see cref="Node.LocationElementName"/>,
  /// resulting in <see cref="Node.LocationElement"/> referring to that panel.
  /// </para>
  /// <para>
  /// For example, the following <c>DataTemplate</c> for <see cref="Group"/>s
  /// results in a thick border around the group's member nodes with some text above the
  /// top-left corner of the border and a port on each side in the middle of the border.
  /// <code>
  ///   &lt;DataTemplate x:Key="ExampleGroupTemplate"&gt;
  ///     &lt;StackPanel go:Node.LocationElementName="GroupPanel"&gt;
  ///       &lt;TextBlock Text="{Binding Path=Data.Name}" /&gt;
  ///       &lt;Border BorderBrush="Gray" BorderThickness="6" CornerRadius="5" Margin="0"&gt;
  ///         &lt;go:GroupPanel x:Name="GroupPanel" Padding="10" &gt;
  ///           &lt;Path go:SpotPanel.Spot="0 0.5 -3 0" Data="M0 0 L6 3 L 0 6 Z" Fill="Blue"
  ///                 Width="6" Height="6" go:Node.PortId="input" go:Node.LinkableTo="True" /&gt;
  ///           &lt;Path go:SpotPanel.Spot="1 0.5 3 0" Data="M0 0 L6 3 L 0 6 Z" Fill="Green"
  ///                 Width="6" Height="6" go:Node.PortId="output" go:Node.LinkableFrom="True" /&gt;
  ///         &lt;/go:GroupPanel&gt;
  ///       &lt;/Border&gt;
  ///     &lt;/StackPanel&gt;
  ///   &lt;/DataTemplate&gt;
  /// </code>
  /// </para>
  /// </remarks>
  public class GroupPanel : SpotPanel {

    static GroupPanel() {
      // Extra space between all member nodes and the border
      PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(GroupPanel),
        new FrameworkPropertyMetadata(new Thickness(5), Diagram.OnChangedInvalidateMeasure));
      SurroundsMembersAfterDropProperty = DependencyProperty.Register("SurroundsMembersAfterDrop", typeof(bool), typeof(GroupPanel),
        new FrameworkPropertyMetadata(false));
    }


    /// <summary>
    /// Identifies the <see cref="Padding"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PaddingProperty;
    /// <summary>
    /// Gets or sets the extra space inside the border but around the member nodes.
    /// </summary>
    /// <value>
    /// The default value is a uniform <c>Thickness</c> of 5.
    /// </value>
    public Thickness Padding {
      get { return (Thickness)GetValue(PaddingProperty); }
      set { SetValue(PaddingProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SurroundsMembersAfterDrop"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SurroundsMembersAfterDropProperty;
    /// <summary>
    /// Gets or sets whether <see cref="ComputeBorder"/> should keep the same bounds
    /// during a <see cref="Northwoods.GoXam.Tool.DraggingTool"/> move.
    /// </summary>
    /// <value>
    /// The default value is false -- this <see cref="GroupPanel"/> surrounds all of its members
    /// even while the dragging tool is moving any of its member nodes.
    /// </value>
    /// <remarks>
    /// In other words, when the value is true, re-computing the bounds of the
    /// members is suspended until a drop occurs, at which time the border is recomputed,
    /// perhaps not including some members that had been dragged out and reparented.
    /// </remarks>
    public bool SurroundsMembersAfterDrop {
      get { return (bool)GetValue(SurroundsMembersAfterDropProperty); }
      set { SetValue(SurroundsMembersAfterDropProperty, value); }
    }

    /// <summary>
    /// Compute the union of the <see cref="Part.Bounds"/> of this <see cref="Group"/>'s <see cref="Group.MemberNodes"/>.
    /// </summary>
    /// <returns>a <c>Rect</c> in model coordinates</returns>
    /// <remarks>
    /// If there are no members, this returns a <c>Rect</c> with Width and Height of zero
    /// and an X and Y that are this panel's original position in model coordinates.
    /// </remarks>
    protected virtual Rect ComputeMemberBounds() {
      Group sg = Diagram.FindAncestor<Group>(this);
      if (sg == null) return new Rect();
      double minx = Double.MaxValue;
      double miny = Double.MaxValue;
      double maxx = Double.MinValue;
      double maxy = Double.MinValue;
      foreach (Node m in sg.MemberNodes) {
        if (m.Visibility != Visibility.Visible) continue;
        Rect b = m.Bounds;
        //Diagram.Debug("  " + Diagram.Str(m) + " " + Diagram.Str(b));
        if (b.Left < minx) minx = b.Left;
        if (b.Top < miny) miny = b.Top;
        if (b.Right > maxx) maxx = b.Right;
        if (b.Bottom > maxy) maxy = b.Bottom;
      }
      foreach (Link m in sg.MemberLinks) {
        if (m.Visibility != Visibility.Visible) continue;
        if (!m.ValidMeasure) continue;  //???
        // ignore links connecting to this Group
        if (m.FromNode == sg || m.ToNode == sg) continue;
        Rect b = m.Bounds;
        //Diagram.Debug("  " + Diagram.Str(m) + " " + Diagram.Str(b));
        if (b.Left < minx) minx = b.Left;
        if (b.Top < miny) miny = b.Top;
        if (b.Right > maxx) maxx = b.Right;
        if (b.Bottom > maxy) maxy = b.Bottom;
      }
      Rect result;
      if (minx == Double.MaxValue || miny == Double.MaxValue) {
        Rect sgb = sg.GetElementBounds(this);
        result = new Rect(sgb.X, sgb.Y, 0, 0);
      } else {
        result = new Rect(minx, miny, maxx-minx, maxy-miny);
      }
      //Diagram.Debug(Diagram.Str(sg) + ": " + Diagram.Str(result) + "\n");
      return result;
    }

    /// <summary>
    /// Normally this just returns the result of <see cref="ComputeMemberBounds"/> expanded by the <see cref="Padding"/>.
    /// </summary>
    /// <returns>
    /// However, if <see cref="SurroundsMembersAfterDrop"/> is true,
    /// and if the <see cref="Diagram.CurrentTool"/> is the <see cref="Northwoods.GoXam.Tool.DraggingTool"/>,
    /// and if this <see cref="Group"/> is not being dragged,
    /// this method returns the last value of <see cref="ComputeBorder"/> before dragging began.
    /// </returns>
    protected override Rect ComputeBorder() {
      if (this.SurroundsMembersAfterDrop && this.SavedBounds != Rect.Empty) {
        Group sg = Diagram.FindAncestor<Group>(this);
        if (sg != null) {
          Diagram diagram = sg.Diagram;
          if (diagram != null) {
            Northwoods.GoXam.Tool.DraggingTool tool = diagram.CurrentTool as Northwoods.GoXam.Tool.DraggingTool;
            if (tool != null && !tool.Dropped && tool.DraggedParts != null && !tool.DraggedParts.ContainsKey(sg)) {
              return this.SavedBounds;
            }
          }
        }
      }
      Rect r = ComputeMemberBounds();
      Thickness pad = this.Padding;
      return new Rect(r.X - pad.Left, r.Y - pad.Top, r.Width + pad.Left + pad.Right, r.Height + pad.Top + pad.Bottom);
    }
    private Rect SavedBounds { get; set; }

    /// <summary>
    /// Measure any children.
    /// </summary>
    /// <param name="availableSize"></param>
    /// <returns>This returns the size computed by <see cref="ComputeBorder"/></returns>
    /// <remarks>
    /// <para>
    /// <see cref="GroupPanel"/> requires that the <see cref="Group"/>'s <see cref="Node.LocationElement"/>
    /// (i.e. the element in the group's visual tree named by <see cref="Node.LocationElementName"/>)
    /// is this panel itself, not some other element.
    /// </para>
    /// <para>
    /// The member nodes and links, as described by <see cref="ComputeBorder"/>, act as the "main" child.
    /// Any child elements for which <see cref="SpotPanel.GetMain"/> are true are measured and arranged
    /// to occupy the whole <see cref="ComputeBorder"/> area.
    /// </para>
    /// </remarks>
    protected override Size MeasureOverride(Size availableSize) {
      Rect b = ComputeBorder();
      this.SavedBounds = b;
      foreach (UIElement child in this.Children) {
        if (GetMain(child)) {
          child.Measure(new Size(b.Width, b.Height));
        } else {
          child.Measure(Geo.Unlimited);
        }
      }
      Group sg = Diagram.FindAncestor<Group>(this);
      if (sg == null || !sg.IsMeasuringArranging) return new Size(b.Width, b.Height);
      if (sg.LocationElement != this) {
        Diagram.Trace("WARNING: the GroupPanel is not its node's LocationElement, specified by go:Node.LocationElementName");
      }
      // don't assign Location if there aren't any visible member nodes
      if (sg.MemberNodes.Any(n => n.Visibility == Visibility.Visible)) {
        if (!Double.IsNaN(b.X) && !Double.IsNaN(b.Y)) {
          bool wasma = sg.IsMeasuringArranging;
          sg.IsMeasuringArranging = false;
          //Diagram.Debug("GroupPanel.MeasureOverride: " + Diagram.Str(sg) + " " + Diagram.Str(sg.Location) + "==> " + Diagram.Str(b));
          sg.Location = new Point(b.X, b.Y);
          sg.IsMeasuringArranging = wasma;
        }
      }
      return new Size(b.Width, b.Height);
    }

    /// <summary>
    /// Arrange any children according to their <see cref="SpotPanel.GetSpot"/> and <see cref="SpotPanel.GetAlignment"/> values.
    /// </summary>
    /// <param name="finalSize"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// The member nodes and links, as described by <see cref="ComputeBorder"/>, act as the "main" child.
    /// Any child elements for which <see cref="SpotPanel.GetMain"/> are true are measured and arranged
    /// to occupy the whole <see cref="ComputeBorder"/> area.
    /// </para>
    /// </remarks>
    protected override Size ArrangeOverride(Size finalSize) {
      double w = this.SavedBounds.Width;
      double h = this.SavedBounds.Height;
      foreach (UIElement child in this.Children) {
        if (GetMain(child)) {
          child.Arrange(new Rect(0, 0, w, h));
        } else {
          Size size = child.DesiredSize;
          Spot spot = GetSpot(child);
          Spot align = GetAlignment(child);
          if (spot.IsNoSpot) spot = Spot.Center;
          if (align.IsNoSpot) align = Spot.Center;
          child.Arrange(new Rect(spot.X*w + spot.OffsetX - align.X*size.Width - align.OffsetX,
                                 spot.Y*h + spot.OffsetY - align.Y*size.Height - align.OffsetY,
                                 size.Width, size.Height));
        }
      }
      return finalSize;
    }
  }
}
