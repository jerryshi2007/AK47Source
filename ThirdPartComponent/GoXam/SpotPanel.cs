
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
using System.Windows;
using System.Windows.Controls;

namespace Northwoods.GoXam {

  /// <summary>
  /// This panel is useful for positioning child elements relative either to the bounds of a particular child
  /// or to a computed rectangle.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The child elements of a <c>SpotPanel</c> are positioned according the values of two attached properties:
  /// <c>Spot</c> and <c>Alignment</c>.  Both property values are of type <see cref="Spot"/>.
  /// Typically each child will at least specify a value for the <c>Spot</c> attached property,
  /// and will also specify a value for <c>Alignment</c> for minor adjustments in the position.
  /// </para>
  /// <para>
  /// The value of <see cref="GetSpot"/> specifies a point in a rectangle where the element should be positioned.
  /// The value of <see cref="GetAlignment"/> specifies what point of the element should be positioned at
  /// the spot point of the rectangle.
  /// Both properties default to <see cref="Spot.Center"/> -- the center of the element is centered inside
  /// the rectangle.
  /// (This panel ignores the <c>FrameworkElement.HorizontalAlignment</c> and <c>VerticalAlignment</c> properties.)
  /// </para>
  /// <para>
  /// The rectangle used for determining spot points is
  /// given by the bounds of the child element of the panel that has the <see cref="MainProperty"/> set to true.
  /// If there is no such element, the value of <see cref="ComputeBorder"/> is used instead.
  /// By default <see cref="ComputeBorder"/> will return the desired size of the first child.
  /// However, the <see cref="GroupPanel"/> class overrides the <see cref="ComputeBorder"/> method
  /// to return values unrelated to any of this panel's children.
  /// </para>
  /// <para>
  /// Here's an example of a <c>SpotPanel</c> as a <c>DataTemplate</c> for a node.
  /// This places text at various points around the main <c>Rectangle</c>.
  /// <code>
  ///   &lt;go:SpotPanel go:Node.Resizable="True" go:Node.SelectionElementName="Rectangle"&gt;
  ///     &lt;Rectangle x:Name="Rectangle" go:SpotPanel.Main="True" Stroke="Black" StrokeThickness="1" Width="100" Height="100" /&gt;
  ///     &lt;TextBlock go:SpotPanel.Spot="TopLeft" go:SpotPanel.Alignment="TopLeft" Text="inside" /&gt;
  ///     &lt;TextBlock go:SpotPanel.Spot="TopLeft" go:SpotPanel.Alignment="BottomLeft" Text="atop" /&gt;
  ///     &lt;TextBlock go:SpotPanel.Spot="TopLeft" go:SpotPanel.Alignment="TopRight" Text="aside" /&gt;
  ///     &lt;TextBlock go:SpotPanel.Spot="Center" go:SpotPanel.Alignment="Center" Text="center" /&gt;
  ///     &lt;TextBlock go:SpotPanel.Spot="1 0.7 -20 0" Text="-20" /&gt;
  ///     &lt;TextBlock go:SpotPanel.Spot="1 0.7 0 0" Text="0" /&gt;
  ///     &lt;TextBlock go:SpotPanel.Spot="1 0.7 20 0" Text="+20" /&gt;
  ///   &lt;/go:SpotPanel&gt;
  /// </code>
  /// </para>
  /// <para>
  /// A <c>SpotPanel</c> has a special usage when it is the <see cref="Node.LocationElement"/>
  /// of an <see cref="Adornment"/> and there is no child that is the "Main" child --
  /// <see cref="ComputeBorder"/> returns the size of the <see cref="Adornment.AdornedElement"/>.
  /// </para>
  /// </remarks>
  public class SpotPanel : Panel {

    static SpotPanel() {
      MainProperty = DependencyProperty.RegisterAttached("Main", typeof(bool), typeof(SpotPanel),
        new FrameworkPropertyMetadata(false));
      SpotProperty = DependencyProperty.RegisterAttached("Spot", typeof(Spot), typeof(SpotPanel),
        new FrameworkPropertyMetadata(Spot.Center, Diagram.OnChangedInvalidateMeasure));
      AlignmentProperty = DependencyProperty.RegisterAttached("Alignment", typeof(Spot), typeof(SpotPanel),
        new FrameworkPropertyMetadata(Spot.Center, Diagram.OnChangedInvalidateMeasure));
    }


    /// <summary>
    /// Identifies the <c>Main</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty MainProperty;
    /// <summary>
    /// Gets whether the element is considered the "main" element for the <see cref="SpotPanel"/>,
    /// around which all of the sibling elements are positioned.
    /// </summary>
    /// <param name="d">this should be a child of a <see cref="SpotPanel"/></param>
    /// <returns>This defaults to false.</returns>
    public static bool GetMain(DependencyObject d) { return (bool)d.GetValue(MainProperty); }
    /// <summary>
    /// Sets whether the element is considered the "main" element for the <see cref="SpotPanel"/>,
    /// around which all of the sibling elements are positioned.
    /// </summary>
    /// <param name="d">this should be a child of a <see cref="SpotPanel"/></param>
    /// <param name="v">only one such child of the <see cref="SpotPanel"/> should have this property set to true</param>
    public static void SetMain(DependencyObject d, bool v) { d.SetValue(MainProperty, v); }

    /// <summary>
    /// Identifies the <c>Spot</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty SpotProperty;
    /// <summary>
    /// Gets the <see cref="Spot"/> at which the element should be positioned.
    /// </summary>
    /// <param name="d">a <c>UIElement</c></param>
    /// <returns>This defaults to <see cref="Spot.Center"/></returns>
    public static Spot GetSpot(DependencyObject d) { return (Spot)d.GetValue(SpotProperty); }
    /// <summary>
    /// Sets the <see cref="Spot"/> at which the element should be positioned.
    /// </summary>
    /// <param name="d">a <c>UIElement</c></param>
    /// <param name="v">
    /// a <see cref="Spot"/> for which <see cref="Spot.IsSpot"/> is true;
    /// the panel assumes <see cref="Spot.Center"/> otherwise
    /// </param>
    public static void SetSpot(DependencyObject d, Spot v) { d.SetValue(SpotProperty, v); }

    /// <summary>
    /// Identifies the <c>Alignment</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty AlignmentProperty;
    /// <summary>
    /// Gets the <see cref="Spot"/> point of the element that should be positioned at the <see cref="GetSpot"/> point.
    /// </summary>
    /// <param name="d">a <c>UIElement</c></param>
    /// <returns>This defaults to <see cref="Spot.Center"/></returns>
    public static Spot GetAlignment(DependencyObject d) { return (Spot)d.GetValue(AlignmentProperty); }
    /// <summary>
    /// Sets the <see cref="Spot"/> point of the element that should be positioned at the <see cref="GetSpot"/> point.
    /// </summary>
    /// <param name="d">a <c>UIElement</c></param>
    /// <param name="v">
    /// a <see cref="Spot"/> for which <see cref="Spot.IsSpot"/> is true;
    /// the panel assumes <see cref="Spot.Center"/> otherwise
    /// </param>
    public static void SetAlignment(DependencyObject d, Spot v) { d.SetValue(AlignmentProperty, v); }


    /// <summary>
    /// Compute the rectangle in which the elements should be positioned.
    /// </summary>
    /// <returns>
    /// If this panel is in an <see cref="Adornment"/> and is its <see cref="Node.LocationElement"/>,
    /// return the <see cref="Adornment.AdornedElement"/>'s size.
    /// Otherwise return a <c>Rect</c> based on this first child's <c>DesiredSize</c>.
    /// Only the Width and Height of the Rect matter in the calculations performed by
    /// <see cref="MeasureOverride"/> and <see cref="ArrangeOverride"/>.
    /// </returns>
    protected virtual Rect ComputeBorder() {
      Adornment ad = Diagram.FindAncestor<Adornment>(this);
      if (ad != null) {
        FrameworkElement elt = ad.AdornedElement;
        if (elt != null && this == ad.LocationElement) {
          Size sz = ad.GetEffectiveSize(elt);
          if (sz.Width > 0 && sz.Height > 0) return new Rect(0, 0, sz.Width, sz.Height);
        }
      }
      Size s = this.DesiredSize;
      foreach (UIElement child in this.Children) {
        s = child.DesiredSize;
        break;
      }
      return new Rect(0, 0, s.Width, s.Height);
    }

    /// <summary>
    /// Measure all of the children and return the size of the union of the bounds of the
    /// elements positioned according to the <c>Spot</c> and <c>Alignment</c> attached properties
    /// and the size of the element with the <c>Main</c> attached property.
    /// </summary>
    /// <param name="availableSize"></param>
    /// <returns></returns>
    /// <remarks>
    /// If there is no child element with a <c>Main</c> attached property value of true,
    /// this actually calls <see cref="ComputeBorder"/> to determine the desired rectangle
    /// about which to position the children.
    /// </remarks>
    protected override Size MeasureOverride(Size availableSize) {
      UIElement main = null;
      foreach (UIElement child in this.Children) {
        child.Measure(Geo.Unlimited);
        if (main == null && GetMain(child)) main = child;
      }
      double mainWidth = 0;
      double mainHeight = 0;
      Rect union = new Rect();
      if (main != null) {
        Size size = main.DesiredSize;
        mainWidth = size.Width;
        mainHeight =  size.Height;
        union.Width = size.Width;
        union.Height = size.Height;
      } else {
        Rect b = ComputeBorder();
        union.Width = b.Width;
        union.Height = b.Height;
        mainWidth = b.Width;
        mainHeight = b.Height;
      }
      foreach (UIElement child in this.Children) {
        Size size = child.DesiredSize;
        Spot spot = GetSpot(child);
        Spot align = GetAlignment(child);
        if (spot.IsNoSpot) spot = Spot.Center;
        if (align.IsNoSpot) align = Spot.Center;
        union.Union(new Rect(spot.X*mainWidth + spot.OffsetX - align.X*size.Width - align.OffsetX,
                             spot.Y*mainHeight + spot.OffsetY - align.Y*size.Height - align.OffsetY,
                             size.Width, size.Height));
      }
      _Union = union;
      return new Size(union.Width, union.Height);
    }

    private Rect _Union;

    /// <summary>
    /// Arrange all of the children according to the <c>Spot</c> and <c>Alignment</c> attached properties
    /// and the size of the first child with the <c>Main</c> attached property.
    /// </summary>
    /// <param name="finalSize"></param>
    /// <returns></returns>
    /// <remarks>
    /// If there is no child element with a <c>Main</c> attached property value of true,
    /// this actually calls <see cref="ComputeBorder"/> to determine the desired rectangle
    /// about which to position the children.
    /// </remarks>
    protected override Size ArrangeOverride(Size finalSize) {
      UIElement main = null;
      foreach (UIElement child in this.Children) {
        if (main == null && GetMain(child)) {
          main = child;
          break;
        }
      }
      double mainWidth = 0;
      double mainHeight = 0;
      Rect union = new Rect();
      if (main != null) {
        union = _Union;
        Size size = main.DesiredSize;
        mainWidth = Math.Max(finalSize.Width-union.Width, size.Width);
        mainHeight = Math.Max(finalSize.Height-union.Height, size.Height);
      } else {
        Rect b = ComputeBorder();
        union.Width = b.Width;
        union.Height = b.Height;
        mainWidth = b.Width;
        mainHeight = b.Height;
      }
      foreach (UIElement child in this.Children) {
        Part.ClearCachedValues(child);
        if (child == main) {
          child.Arrange(new Rect(-union.X, -union.Y, mainWidth, mainHeight));
        } else {
          Size size = child.DesiredSize;
          Spot spot = GetSpot(child);
          Spot align = GetAlignment(child);
          if (spot.IsNoSpot) spot = Spot.Center;
          if (align.IsNoSpot) align = Spot.Center;
          child.Arrange(new Rect(-union.X + spot.X*mainWidth + spot.OffsetX - align.X*size.Width - align.OffsetX,
                                 -union.Y + spot.Y*mainHeight + spot.OffsetY - align.Y*size.Height - align.OffsetY,
                                 size.Width, size.Height));
        }
      }
      return finalSize;
    }

  }
}
