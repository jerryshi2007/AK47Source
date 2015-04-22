
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
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;

namespace Northwoods.GoXam {

  /// <summary>
  /// This panel is useful for having a background shape as the primary object
  /// and positioning one or more child elements within that shape.
  /// </summary>
  /// <remarks>
  /// <para>
  /// A <c>NodePanel</c> is typically used as the implementation of a <see cref="Node"/>'s <c>DataTemplate</c>
  /// when you want the node to basically be some shape (a "figure", but it can be any kind of <c>UIElement</c>)
  /// and you want to position and align additional elements within that primary shape.
  /// If you want elements such as text outside of that shape, we suggest you use a <c>Panel</c>
  /// such as <c>StackPanel</c> or <c>Grid</c> to arrange the node the way you want.
  /// </para>
  /// <para>
  /// The first child element of the panel is treated as the main object.
  /// It can have two attached property values, <c>NodePanel.Spot1</c> and <c>NodePanel.Spot2</c>,
  /// that denote the top-left and bottom-right corners of an area where the other child elements are placed.
  /// The default spot values will just cover the whole element.
  /// However, a <c>NodeShape</c> (WPF), or a <c>Path</c> (Silverlight),
  /// with a <c>NodeFigure</c> attached property is frequently used as the first element of a <c>NodePanel</c>.
  /// Such shapes have predefined <c>Spot1</c> and <c>Spot2</c> values in addition to having particular geometries.
  /// </para>
  /// <para>
  /// The layout will observe the <c>FrameworkElement.HorizontalAlignment</c> and
  /// <c>VerticalAlignment</c> properties of each child element after the first one.
  /// </para>
  /// <para>
  /// As an example, here is the definition of a simple template that displays a resizable rounded rectangle
  /// surrounding some text:
  /// <code>
  ///   &lt;DataTemplate x:Key="ExampleNodeTemplate"&gt;
  ///     &lt;go:NodePanel go:Node.Location="{Binding Path=Data.XY, Mode=TwoWay}"
  ///                   go:Node.SelectionElementName="Shape" go:Node.Resizable="True"&gt;
  ///       &lt;go:NodeShape x:Name="Shape" go:NodePanel.Figure="RoundedRectangle" Width="50" Height="20"
  ///                     Stroke="Gray" StrokeThickness="1" Fill="LightYellow" /&gt;
  ///       &lt;TextBlock Text="{Binding Path=Data.Name}" TextWrapping="Wrap" /&gt;
  ///     &lt;/go:NodePanel&gt;
  ///   &lt;/DataTemplate&gt;
  /// </code>
  /// In WPF, you must use the <c>&lt;go:NodeShape&gt;</c> element instead of a <c>&lt;Path&gt;</c> element.
  /// In Silverlight 3, you must use the <c>&lt;Path&gt;</c> element instead of a <c>&lt;go:NodeShape&gt;</c> element.
  /// In Silverlight 4, you may use either the <c>&lt;Path&gt;</c> element or a <c>&lt;go:NodeShape&gt;</c> element.
  /// </para>
  /// </remarks>
  public class NodePanel : Panel {

    static NodePanel() {
      SizingProperty = DependencyProperty.Register("Sizing", typeof(NodePanelSizing), typeof(NodePanel),
        new FrameworkPropertyMetadata(NodePanelSizing.Fixed, Diagram.OnChangedInvalidateMeasure));

      // these two attached properties go on the first child element,
      // so that it can control where the siblings will be positioned
      Spot1Property = DependencyProperty.RegisterAttached("Spot1", typeof(Spot), typeof(NodePanel),
        new FrameworkPropertyMetadata(Spot.Default, Diagram.OnChangedInvalidateMeasure));
      Spot2Property = DependencyProperty.RegisterAttached("Spot2", typeof(Spot), typeof(NodePanel),
        new FrameworkPropertyMetadata(Spot.Default, Diagram.OnChangedInvalidateMeasure));

      // for a NodeShape (WPF) or ToolHandle (WPF) or Path (Silverlight), to control its geometry;
      // there may be more than one such shape in a Node
      FigureProperty = DependencyProperty.RegisterAttached("Figure", typeof(NodeFigure), typeof(NodePanel),
        new FrameworkPropertyMetadata(NodeFigure.None, OnFigureChanged));
      FigureParameter1Property = DependencyProperty.RegisterAttached("FigureParameter1", typeof(double), typeof(NodePanel),
        new FrameworkPropertyMetadata(0.0, OnFigureParameter1Changed));
      FigureParameter2Property = DependencyProperty.RegisterAttached("FigureParameter2", typeof(double), typeof(NodePanel),
        new FrameworkPropertyMetadata(0.0, OnFigureParameter2Changed));
    }

    /// <summary>
    /// Defines how this panel should behave when it is resized.
    /// </summary>
    public static readonly DependencyProperty SizingProperty;
    /// <summary>
    /// Defines how this panel should behave when it is resized.
    /// Possible values are: Fixed (default) or Auto
    /// </summary>
    public NodePanelSizing Sizing {
      get { return (NodePanelSizing)GetValue(SizingProperty); }
      set { SetValue(SizingProperty, value); }
    }


    /// <summary>
    /// Identifies the <c>Spot1</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty Spot1Property;
    /// <summary>
    /// Gets the value of the <c>Spot1</c> attached property.
    /// </summary>
    /// <param name="d">a <c>UIElement</c></param>
    /// <returns>
    /// This defaults to <see cref="Spot.TopLeft"/>.
    /// </returns>
    public static Spot GetSpot1(DependencyObject d) { return (Spot)d.GetValue(Spot1Property); }
    /// <summary>
    /// Sets the value of the <c>Spot1</c> attached property.
    /// </summary>
    /// <param name="d">a <c>UIElement</c></param>
    /// <param name="v"></param>
    public static void SetSpot1(DependencyObject d, Spot v) { d.SetValue(Spot1Property, v); }

    /// <summary>
    /// Identifies the <c>Spot2</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty Spot2Property;
    /// <summary>
    /// Gets the value of the <c>Spot2</c> attached property.
    /// </summary>
    /// <param name="d">a <c>UIElement</c></param>
    /// <returns>
    /// This defaults to <see cref="Spot.BottomRight"/>.
    /// </returns>
    public static Spot GetSpot2(DependencyObject d) { return (Spot)d.GetValue(Spot2Property); }
    /// <summary>
    /// Sets the value of the <c>Spot2</c> attached property.
    /// </summary>
    /// <param name="d">a <c>UIElement</c></param>
    /// <param name="v"></param>
    public static void SetSpot2(DependencyObject d, Spot v) { d.SetValue(Spot2Property, v); }


    // shape properties
    // These are used on a NodeShape (WPF) or ToolHandle (WPF) or Path (Silverlight), to control its geometry

    /// <summary>
    /// Identifies the <c>Figure</c> attached dependency property,
    /// for a <c>NodeShape</c> element (WPF) or <c>Path</c> element (Silverlight) inside a <see cref="NodePanel"/>.
    /// </summary>
    public static readonly DependencyProperty FigureProperty;  // used on a NodeShape (WPF) or ToolHandle (WPF) or Path (Silverlight), to control its geometry
    /// <summary>
    /// Gets the value of the <see cref="FigureProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static NodeFigure GetFigure(DependencyObject d) { return (NodeFigure)d.GetValue(FigureProperty); }
    /// <summary>
    /// Sets the value of the <see cref="FigureProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="v"></param>
    public static void SetFigure(DependencyObject d, NodeFigure v) { d.SetValue(FigureProperty, v); }
    private static void OnFigureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      UpdateFigure(d as Shape);
    }

    /// <summary>
    /// Identifies the <c>FigureParameter1</c> attached dependency property,
    /// for a <c>NodeShape</c> element (WPF) or <c>Path</c> element (Silverlight) inside a <see cref="NodePanel"/>.
    /// </summary>
    internal /*?? public */ static readonly DependencyProperty FigureParameter1Property;  // used on a NodeShape (WPF) or ToolHandle (WPF) or Path (Silverlight), to control its geometry
    /// <summary>
    /// Gets the value of the <see cref="FigureParameter1Property"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    internal /*?? public */ static double GetFigureParameter1(DependencyObject d) { return (double)d.GetValue(FigureParameter1Property); }
    /// <summary>
    /// Sets the value of the <see cref="FigureParameter1Property"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="v"></param>
    internal /*?? public */ static void SetFigureParameter1(DependencyObject d, double v) { d.SetValue(FigureParameter1Property, v); }
    private static void OnFigureParameter1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      UpdateFigure(d as Shape);
    }

    /// <summary>
    /// Identifies the <c>FigureParameter2</c> attached dependency property,
    /// for a <c>NodeShape</c> element (WPF) or <c>Path</c> element (Silverlight) inside a <see cref="NodePanel"/>.
    /// </summary>
    internal /*?? public */ static readonly DependencyProperty FigureParameter2Property;  // used on a NodeShape (WPF) or ToolHandle (WPF) or Path (Silverlight), to control its geometry
    /// <summary>
    /// Gets the value of the <see cref="FigureParameter2Property"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    internal /*?? public */ static double GetFigureParameter2(DependencyObject d) { return (double)d.GetValue(FigureParameter2Property); }
    /// <summary>
    /// Sets the value of the <see cref="FigureParameter2Property"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="v"></param>
    internal /*?? public */ static void SetFigureParameter2(DependencyObject d, double v) { d.SetValue(FigureParameter2Property, v); }
    private static void OnFigureParameter2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      UpdateFigure(d as Shape);
    }

    private static void UpdateFigure(Shape shape) {
      Part part = Diagram.FindAncestor<Part>(shape);
      if (part != null) {
        part.InvalidateVisual(shape);
      }
    }



    private void RenderNodeShape(UIElement elt, Size sz) {
      if (elt == null) return;
      Path path = elt as Path;
      if (path != null) {  // if it's a Path
        // and if NodePanel.Figure is specified, it's a NodeShape equivalent
        NodeFigure fig = NodePanel.GetFigure(elt);
        if (fig != NodeFigure.None) {
          path.Stretch = Stretch.Fill;
          path.Data = new NodeGeometry(path).GetGeometry(sz);
          // this also has the side-effect of setting any default Spot1/Spot2 parameters
        }
      }
    }













    private Spot ComputeSpot1(UIElement elt) {
      Spot spot = GetSpot1(elt);
      if (spot.IsDefault)
        return Spot.TopLeft;
      else
        return spot;
    }

    private Spot ComputeSpot2(UIElement elt) {
      Spot spot = GetSpot2(elt);
      if (spot.IsDefault)
        return Spot.BottomRight;
      else
        return spot;
    }

    /// <summary>
    /// Calculates the longest word in <paramref name="input"/> and returns it.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static string FindLongestWord(string input) {
      string result = "";
      int pos = 0;
      string word = "";
      while (true) {
        int index = input.IndexOf(' ');
        if (index >= 0)
          word = input.Substring(pos, index);
        else {
          result = (Math.Max(input.Length, result.Length) == result.Length) ? result : input;
          break;
        }
        result = (Math.Max(word.Length, result.Length) == result.Length) ? result : word;
        if (index + 1 < input.Length)
          input = input.Substring(index + 1);
        else break;
        pos = 0;
      }
      return result;
    }

    internal static Size MeasureUnwrappedString(String s, TextBlock tb) {

      TextBlock temp = new TextBlock() {
        Text = s,
        TextAlignment = tb.TextAlignment,
        FontSize = tb.FontSize,
        FontFamily = tb.FontFamily,
        FontWeight = tb.FontWeight,
        FontStyle = tb.FontStyle,
        FontStretch = tb.FontStretch,
        Padding = tb.Padding
      };
      temp.Measure(Geo.Unlimited);
      Size sz = EffectiveDesiredSize(temp);
      Thickness margin = tb.Margin;
      sz.Width += margin.Left + margin.Right;
      sz.Height += margin.Top + margin.Bottom;
      if (sz.Width < 5) sz.Width = 5;
      return sz;










    }

    private static Size EffectiveDesiredSize(UIElement e) {
      Size sz = e.DesiredSize;

      FrameworkElement elt = e as FrameworkElement;
      if (elt != null) {
        if (sz.Width == 0) {
          sz.Width = elt.ActualWidth;
          if (sz.Width == 0) {
            sz.Width = elt.Width;
            if (Double.IsNaN(sz.Width)) sz.Width = 100;
          }
        }
        if (sz.Height == 0) {
          sz.Height = elt.ActualHeight;
          if (sz.Height == 0) {
            sz.Height = elt.Height;
            if (Double.IsNaN(sz.Height)) sz.Height = 100;
          }
        }
      }

      return sz;
    }

    // estimate the desired text width of a TextBlock, assuming wrapping
    // include Padding and Margin in the result
    internal static double MeasureWrappedTextWidth(TextBlock tb, Size maxSize) {
      // calculate ideal width, given the area of the text and desired aspect ratio:
      tb.Measure(Geo.Unlimited);
      Size ds = EffectiveDesiredSize(tb);
      double aspectratio = Part.GetTextAspectRatio(tb);
      double idealwidth;
      // if the aspectratio is invalid, assume no wrapping
      if (Double.IsNaN(aspectratio) || Double.IsInfinity(aspectratio) || aspectratio <= 0) {
        return Math.Max(tb.MinWidth, Math.Min(ds.Width, maxSize.Width));
      }
      // calculate area of text, excluding any margin
      Thickness margin = tb.Margin;
      double area = Math.Max(1, ds.Width-margin.Left-margin.Right) * Math.Max(1, ds.Height-margin.Top-margin.Bottom);
      idealwidth = Math.Sqrt(area/aspectratio)*aspectratio;
      // is there a height limit, and is it shorter than the desired height?
      if (maxSize.Height < ds.Height && maxSize.Height > 0) {
        // add width to account for area that would be cut off by the shorter max height
        idealwidth += idealwidth*(ds.Height-maxSize.Height)/maxSize.Height;
      }
      // now add margin back in
      idealwidth += margin.Left + margin.Right;

      // Calculate minimum preferred width:
      String shortest = FindLongestWord(tb.Text);
      Size shortestsize = MeasureUnwrappedString(shortest, tb);

      // Restrict the width
      double width = Math.Max(idealwidth, shortestsize.Width);
      width = Math.Max(tb.MinWidth, Math.Min(width, maxSize.Width));
      //Diagram.Debug("ideal: " + Diagram.Str(idealwidth) + " short: (" + shortest + ") " + Diagram.Str(shortestsize.Width) + " exp: " + Diagram.Str(width));
      return width;
    }

    /// <summary>
    /// Measures the children according to the rules defined by NodePanelSizing.Auto.
    /// Resizes to fit its children and wraps text appropriately if requested.
    /// </summary>
    /// <param name="availableSize"></param>
    /// <returns></returns>
    private Size MeasureAuto(Size availableSize) {
      UIElementCollection children = this.Children;
      if (children.Count == 0) return new Size(0, 0);
      if (children.Count == 1) {
        UIElement onlychild = children[0];
        RenderNodeShape(onlychild, availableSize);  // specify Path.Data according to NodeFigure
        onlychild.Measure(availableSize);
        actualSize = EffectiveDesiredSize(onlychild);
        return actualSize;
      }
      // when more than one child, measure the rest of them first
      Size totalsize = new Size();
      for (int i = 0; i < children.Count; i++) {
        RenderNodeShape(children[i], availableSize);  // specify Path.Data according to NodeFigure
        if (i == 0) continue;
        TextBlock tb = children[i] as TextBlock;
        // First check if the child is text
        if (tb != null && tb.TextWrapping != TextWrapping.NoWrap) {
          Size maxsize = new Size(Math.Min(availableSize.Width, tb.MaxWidth), Math.Min(availableSize.Height, tb.MaxHeight));
          double length = MeasureWrappedTextWidth(tb, maxsize);
          tb.Measure(new Size(length, maxsize.Height));
          Size newSize = EffectiveDesiredSize(tb);
          //Diagram.Debug("NodePanel: " + tb.Text + " " + Diagram.Str(tb.DesiredSize) + Diagram.Str(newSize) + "len: " + Diagram.Str(length));
          totalsize = new Size(Math.Max(newSize.Width, totalsize.Width), Math.Max(newSize.Height, totalsize.Height));
        } else { // if it's not text, or if it's NoWrap, just measure normally
          UIElement child = children[i];
          child.Measure(availableSize);
          Size ds = EffectiveDesiredSize(child);
          // Keep track of the size we need to display all children
          totalsize = new Size(Math.Max(ds.Width, totalsize.Width), Math.Max(ds.Height, totalsize.Height));
        }
      }

      // Measure first child after since now we know what size to be
      UIElement first = children[0];
      Spot spot1 = ComputeSpot1(first);
      Spot spot2 = ComputeSpot2(first);
      if (Math.Abs(spot2.X - spot1.X) > 0 && Math.Abs(spot2.Y - spot1.Y) > 0)
        actualSize = new Size(totalsize.Width / Math.Abs(spot2.X - spot1.X), totalsize.Height / Math.Abs(spot2.Y - spot1.Y));
      else
        actualSize = new Size();
      actualSize.Width += Math.Abs(spot1.OffsetX) + Math.Abs(spot2.OffsetX);
      actualSize.Height += Math.Abs(spot1.OffsetY) + Math.Abs(spot2.OffsetY);
      // Respect minimum width's and height's
      actualSize.Width = Math.Max(this.MinWidth, Math.Min(actualSize.Width, this.MaxWidth));
      actualSize.Height = Math.Max(this.MinHeight, Math.Min(actualSize.Height, this.MaxHeight));
      RenderNodeShape(first, actualSize);  // specify Path.Data according to NodeFigure
      first.Measure(actualSize);
      //Diagram.Debug("NodePanel main: " + Diagram.Str(availableSize) + Diagram.Str(totalsize) + Diagram.Str(actualSize));
      return actualSize;
    }

    private Size actualSize;  // remember between MeasureAuto and ArrangeAuto

    /// <summary>
    /// Arranges the children according to the rules defined by NodePanelSizing.Auto.
    /// Resizes the first shape to fit around the other children and wraps text appropriately if requested.
    /// </summary>
    /// <param name="finalSize"></param>
    /// <returns></returns>
    private Size ArrangeAuto(Size finalSize) {
      finalSize = actualSize;
      Rect outer = new Rect(0, 0, finalSize.Width, finalSize.Height);
      Rect inner = outer;
      bool first = true;
      foreach (UIElement child in this.Children) {
        Part.ClearCachedValues(child);
        if (first) { // first child is the background
          first = false;
          child.Arrange(outer);
          Spot spot1 = ComputeSpot1(child);
          Spot spot2 = ComputeSpot2(child);
          inner = new Rect(spot1.PointInRect(outer), spot2.PointInRect(outer));
          //Diagram.Debug("NodePanel arrange1: " + Diagram.Str(outer) + Diagram.Str(inner));
        } else {
          child.Arrange(inner);
        }
      }
      return finalSize;
    }


    private Size MeasureFixed(Size availableSize) {
      bool first = true;
      Rect outer = new Rect(0, 0, availableSize.Width, availableSize.Height);
      Rect inner = outer;
      Size final = new Size();

      foreach (UIElement child in this.Children) {
        RenderNodeShape(child, availableSize);  // specify Path.Data according to NodeFigure
        // first child contains all other children
        if (first) {
          first = false;
          child.Measure(availableSize);
          Size ds = EffectiveDesiredSize(child);
          final = ds;
          outer.Width = ds.Width;
          outer.Height = ds.Height;
          Point p1 = ComputeSpot1(child).PointInRect(outer);
          Point p2 = ComputeSpot2(child).PointInRect(outer);
          inner = new Rect(p1, p2);
        } else {  // measure everything else to be within the spot1 and spot2 of the first child
          child.Measure(new Size(inner.Width, inner.Height));
        }
      }
      if (first) return new Size();  // no children
      if (!(double.IsNaN(availableSize.Width) || double.IsNaN(availableSize.Height)
          || double.IsInfinity(availableSize.Width) || double.IsInfinity(availableSize.Height)))
        return availableSize;
      return final;
    }

    /// <summary>
    /// Measures the children according to the rules defined by NodePanelSizing.Fixed.
    /// </summary>
    /// <param name="finalSize"></param>
    /// <returns></returns>
    private Size ArrangeFixed(Size finalSize) {
      bool first = true;
      Rect outer = new Rect(0, 0, finalSize.Width, finalSize.Height);
      Rect inner = outer;

      foreach (UIElement child in this.Children) {
        Part.ClearCachedValues(child);
        if (finalSize == new Size(0, 0)) { child.Arrange(outer); continue; }  //???
        if (first) {
          first = false;
          Spot spot1 = ComputeSpot1(child);
          Spot spot2 = ComputeSpot2(child);
          inner = new Rect(spot1.PointInRect(outer), spot2.PointInRect(outer));
          child.Arrange(outer);
        } else { // Alignments are auto-accounted for, so just arrange in whatever rect is given
          Rect r = inner;
          // adjust R according to each child's HorizontalAlignment and VerticalAlignment
          FrameworkElement elt = child as FrameworkElement;
          if (elt != null) {
            Size sz = EffectiveDesiredSize(child);
            if (sz.Width < inner.Width) {
              switch (elt.HorizontalAlignment) {
                case HorizontalAlignment.Left:
                  r.Width = sz.Width;
                  r.X = inner.X;
                  break;
                case HorizontalAlignment.Center:
                  r.Width = sz.Width;
                  r.X = inner.X+inner.Width/2-sz.Width/2;
                  break;
                case HorizontalAlignment.Right:
                  r.Width = sz.Width;
                  r.X = inner.X+inner.Width-sz.Width;
                  break;
                default: // Stretch means use full inner.Width
                  break;
              }
            }
            if (sz.Height < inner.Height) {
              switch (elt.VerticalAlignment) {
                case VerticalAlignment.Top:
                  r.Height = sz.Height;
                  r.Y = inner.Y;
                  break;
                case VerticalAlignment.Center:
                  r.Height = sz.Height;
                  r.Y = inner.Y+inner.Height/2-sz.Height/2;
                  break;
                case VerticalAlignment.Bottom:
                  r.Height = sz.Height;
                  r.Y = inner.Y+inner.Height-sz.Height;
                  break;
                default:  // Stretch means use full inner.Height
                  break;
              }
            }
          }
          child.Arrange(r);
        }
      }
      return finalSize;
    }

    /// <summary>
    /// Measure all of the children after the first one to fit within the
    /// rectangular area specified by <see cref="GetSpot1"/> and <see cref="GetSpot2"/>
    /// of the first child element.
    /// </summary>
    /// <param name="availableSize"></param>
    /// <returns></returns>
    protected override Size MeasureOverride(Size availableSize) {
      // Measure according to what type of resize behavior is set
      switch (this.Sizing) {
        case NodePanelSizing.Auto:
          return MeasureAuto(availableSize);
        default:
          return MeasureFixed(availableSize);
      }
    }

    /// <summary>
    /// Position all of the children after the first one to be within the
    /// rectangular area specified by <see cref="GetSpot1"/> and <see cref="GetSpot2"/>
    /// of the first child element.
    /// </summary>
    /// <param name="finalSize"></param>
    /// <returns></returns>
    /// <remarks>
    /// If the child is narrower than the <c>Spot1</c>-<c>Spot2</c> determined
    /// area of the first element, the child's <c>FrameworkElement.HorizontalAlignment</c> applies.
    /// Similarly, if the child is shorter than the <c>Spot1</c>-<c>Spot2</c> determined
    /// area of the first element, the child's <c>FrameworkElement.VerticalAlignment</c> applies.
    /// </remarks>
    protected override Size ArrangeOverride(Size finalSize) {
      // Arrange according to what type of resize behavior is set
      switch (this.Sizing) {
        case NodePanelSizing.Auto:
          return ArrangeAuto(finalSize);
        default:
          return ArrangeFixed(finalSize);
      }
    }
  }

  /// <summary>
  /// Defines how this <see cref="NodePanel"/> will resize its children.
  /// </summary>
  /// <remarks>
  /// In each scenario, the first child is treated as a background object in the panel
  /// and all children are placed within the rectangular area defined by its
  /// <c>NodePanel.Spot1</c> and <c>NodePanel.Spot2</c> properties.
  /// The <see cref="NodePanel"/> observes the <c>HorizontalAlignment</c>
  /// and <c>VerticalAlignment</c> properties on the other children.
  /// </remarks>
  public enum NodePanelSizing {
    /// <summary>
    /// The <see cref="NodePanel"/> takes the size of its first child.
    /// All other elements are placed within the bounds defined by the
    /// <c>NodePanel.Spot1</c> and <c>NodePanel.Spot2</c> properties of that first child.
    /// </summary>
    Fixed,
    /// <summary>
    /// The <see cref="NodePanel"/> is automatically sized to fit all of its children.
    /// Basically, the first child (presumably a convex shape) will surround the
    /// second child, which in simple nodes is typically a <c>TextBlock</c>, but could
    /// be a more complicated element.
    /// If the child is a <c>TextBlock</c> with text wrapping set to true,
    /// it will be appropriately wrapped so that all of its text is visible.
    /// </summary>
    Auto
  }
}
