
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
using System.Windows.Media;
using System.Windows.Controls;  // for Silverlight Panel et al.

namespace Northwoods.GoXam {

  /// <summary>
  /// A <c>Layer</c> is a collection of <see cref="Node"/>s or <see cref="Link"/>s
  /// that are to be displayed in front of or behind the parts in other layers.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Each layer can only be a child of a <see cref="DiagramPanel"/> -- they cannot be nested.
  /// There are two classes inheriting from this abstract <c>Layer</c> class: <see cref="NodeLayer"/>
  /// and <see cref="LinkLayer"/>, which hold only nodes or only links, respectively.
  /// </para>
  /// <para>
  /// Each layer has an <see cref="Id"/> property that is used to identify the layer
  /// in the <see cref="DiagramPanel"/> amongst multiple layers of the same type.
  /// </para>
  /// <para>
  /// There are many properties, named "Allow...", that control what operations the user
  /// may perform on the parts held by this layer.  These correspond to the same named
  /// properties on <see cref="Northwoods.GoXam.Diagram"/> that govern the behavior for all parts in all layers.
  /// Furthermore for some of these properties there are corresponding properties on
  /// <see cref="Part"/>, named "...able", that govern the behavior for that individual part.
  /// For example, the <see cref="AllowCopy"/> property corresponds to
  /// <see cref="Northwoods.GoXam.Diagram.AllowCopy"/> and to the property
  /// <see cref="Part.Copyable"/>.
  /// The <see cref="Part.CanCopy"/> predicate is false if any of
  /// these properties is false.
  /// </para>
  /// <para>
  /// You may not apply any transforms to a <c>Layer</c>; that is reserved to GoXam for future use.
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public abstract class Layer :

    Panel



 {

    static Layer() {
      IdProperty = DependencyProperty.Register("Id", typeof(String), typeof(Layer), new FrameworkPropertyMetadata("", OnIdChanged));
      IsTemporaryProperty = DependencyProperty.Register("IsTemporary", typeof(bool), typeof(Layer), new FrameworkPropertyMetadata(false));

      AllowCopyProperty = DependencyProperty.Register("AllowCopy", typeof(bool), typeof(Layer), new FrameworkPropertyMetadata(true));
      AllowDeleteProperty = DependencyProperty.Register("AllowDelete", typeof(bool), typeof(Layer), new FrameworkPropertyMetadata(true));
      AllowEditProperty = DependencyProperty.Register("AllowEdit", typeof(bool), typeof(Layer), new FrameworkPropertyMetadata(true));
      AllowGroupProperty = DependencyProperty.Register("AllowGroup", typeof(bool), typeof(Layer), new FrameworkPropertyMetadata(true));
      //AllowRegroupProperty = DependencyProperty.Register("AllowRegroup", typeof(bool), typeof(Layer), new FrameworkPropertyMetadata(true));
      AllowUngroupProperty = DependencyProperty.Register("AllowUngroup", typeof(bool), typeof(Layer), new FrameworkPropertyMetadata(true));
      //AllowInsertProperty = DependencyProperty.Register("AllowInsert", typeof(bool), typeof(Layer), new FrameworkPropertyMetadata(true));
      AllowLinkProperty = DependencyProperty.Register("AllowLink", typeof(bool), typeof(Layer), new FrameworkPropertyMetadata(true));
      AllowRelinkProperty = DependencyProperty.Register("AllowRelink", typeof(bool), typeof(Layer), new FrameworkPropertyMetadata(true));
      AllowMoveProperty = DependencyProperty.Register("AllowMove", typeof(bool), typeof(Layer), new FrameworkPropertyMetadata(true));
      AllowPrintProperty = DependencyProperty.Register("AllowPrint", typeof(bool), typeof(Layer), new FrameworkPropertyMetadata(true));
      AllowReshapeProperty = DependencyProperty.Register("AllowReshape", typeof(bool), typeof(Layer), new FrameworkPropertyMetadata(true));
      AllowResizeProperty = DependencyProperty.Register("AllowResize", typeof(bool), typeof(Layer), new FrameworkPropertyMetadata(true));
      AllowRotateProperty = DependencyProperty.Register("AllowRotate", typeof(bool), typeof(Layer), new FrameworkPropertyMetadata(true));
      AllowSelectProperty = DependencyProperty.Register("AllowSelect", typeof(bool), typeof(Layer), new FrameworkPropertyMetadata(true));
    }



    /// <summary>
    /// Throw an exception if the current thread does not have access to this <c>DependencyObject</c>.
    /// </summary>
    protected void VerifyAccess() {
      if (!CheckAccess()) Diagram.Error("No access to thread");
    }


















    /// <summary>
    /// Get the <see cref="Northwoods.GoXam.Diagram"/> that this layer is in.
    /// </summary>
    /// <seealso cref="Panel"/>
    public Diagram Diagram {
      get {
        // DiagramPanel caches reference to containing Diagram
        DiagramPanel panel = this.Panel;
        if (panel != null) return panel.Diagram;
        return null;
      }
    }

    /// <summary>
    /// Get the <see cref="DiagramPanel"/> that this layer is in.
    /// </summary>
    /// <seealso cref="Diagram"/>
    public DiagramPanel Panel {
      get {
        if (_Panel == null) _Panel = Diagram.FindParent<DiagramPanel>(this);
        return _Panel;
      }
    }
    private DiagramPanel _Panel;  //?? clearing this cache in Silverlight


    /// <summary>
    /// Identifies the <see cref="Id"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IdProperty;
    /// <summary>
    /// Gets or sets a string naming the layer.
    /// </summary>
    /// <value>
    /// The default value is the empty string.
    /// </value>
    /// <remarks>
    /// You need to ensure that in one diagram all layers of the same type have unique <see cref="Id"/> names.
    /// At the current time this requirement is not enforced.
    /// </remarks>
    public String Id {
      get { return (String)GetValue(IdProperty); }
      set { SetValue(IdProperty, value); }
    }
    private static void OnIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Layer layer = (Layer)d;
      //?? check for duplicate layer names
    }

    /// <summary>
    /// Identifies the <see cref="IsTemporary"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsTemporaryProperty;
    /// <summary>
    /// Gets or sets whether the layer holds "temporary" objects, such as selection or tool adornments
    /// or other parts temporarily displayed by tools.
    /// </summary>
    /// <value>
    /// The default value is false.
    /// </value>
    /// <remarks>
    /// The normal <see cref="DiagramPanel"/> has temporary-object layers
    /// for tools and for adornments.
    /// </remarks>
    public bool IsTemporary {
      get { return (bool)GetValue(IsTemporaryProperty); }
      set { SetValue(IsTemporaryProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowCopy"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowCopyProperty;
    /// <summary>
    /// Gets or sets whether the user may copy
    /// the parts that are in this layer.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <seealso cref="Northwoods.GoXam.Diagram.AllowCopy"/>
    /// <seealso cref="Part.Copyable"/>
    /// <seealso cref="Part.CanCopy"/>
    public bool AllowCopy {
      get { return (bool)GetValue(AllowCopyProperty); }
      set { SetValue(AllowCopyProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowDelete"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowDeleteProperty;
    /// <summary>
    /// Gets or sets whether the user may delete
    /// the parts that are in this layer.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <seealso cref="Northwoods.GoXam.Diagram.AllowDelete"/>
    /// <seealso cref="Part.Deletable"/>
    /// <seealso cref="Part.CanDelete"/>
    public bool AllowDelete {
      get { return (bool)GetValue(AllowDeleteProperty); }
      set { SetValue(AllowDeleteProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowEdit"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowEditProperty;
    /// <summary>
    /// Gets or sets whether the user may edit in-place the text of
    /// the parts that are in this layer.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <seealso cref="Northwoods.GoXam.Diagram.AllowEdit"/>
    /// <seealso cref="Part.Editable"/>
    /// <seealso cref="Part.CanEdit"/>
    public bool AllowEdit {
      get { return (bool)GetValue(AllowEditProperty); }
      set { SetValue(AllowEditProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowGroup"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowGroupProperty;
    /// <summary>
    /// Gets or sets whether the user may group
    /// the parts that are in this layer.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <seealso cref="Northwoods.GoXam.Diagram.AllowGroup"/>
    /// <seealso cref="Part.Groupable"/>
    /// <seealso cref="Part.CanGroup"/>
    public bool AllowGroup {
      get { return (bool)GetValue(AllowGroupProperty); }
      set { SetValue(AllowGroupProperty, value); }
    }

    ///// <summary>
    ///// Identifies the <see cref="AllowRegroup"/> dependency property.
    ///// </summary>
    //public static readonly DependencyProperty AllowRegroupProperty;
    ///// <summary>
    ///// Gets or sets whether the user may regroup
    ///// the parts that are in this layer.
    ///// </summary>
    ///// <value>
    ///// The default value is true.
    ///// </value>
    ///// <seealso cref="Northwoods.GoXam.Diagram.AllowRegroup"/>
    //public bool AllowRegroup {
    //  get { return (bool)GetValue(AllowRegroupProperty); }
    //  set { SetValue(AllowRegroupProperty, value); }
    //}

    /// <summary>
    /// Identifies the <see cref="AllowUngroup"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowUngroupProperty;
    /// <summary>
    /// Gets or sets whether the user may ungroup
    /// the groups that are in this layer.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <seealso cref="Northwoods.GoXam.Diagram.AllowUngroup"/>
    /// <seealso cref="Group.Ungroupable"/>
    /// <seealso cref="Group.CanUngroup"/>
    public bool AllowUngroup {
      get { return (bool)GetValue(AllowUngroupProperty); }
      set { SetValue(AllowUngroupProperty, value); }
    }

    ///// <summary>
    ///// Identifies the <see cref="AllowInsert"/> dependency property.
    ///// </summary>
    //public static readonly DependencyProperty AllowInsertProperty;
    ///// <summary>
    ///// Gets or sets whether the user may insert
    ///// any parts into this layer.
    ///// </summary>
    ///// <value>
    ///// The default value is true.
    ///// </value>
    ///// <seealso cref="Northwoods.GoXam.Diagram.AllowInsert"/>
    //public bool AllowInsert {
    //  get { return (bool)GetValue(AllowInsertProperty); }
    //  set { SetValue(AllowInsertProperty, value); }
    //}

    /// <summary>
    /// Identifies the <see cref="AllowLink"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowLinkProperty;
    /// <summary>
    /// Gets or sets whether the user may link
    /// the nodes that are in this layer.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <seealso cref="Northwoods.GoXam.Diagram.AllowLink"/>
    /// <seealso cref="Northwoods.GoXam.Tool.LinkingBaseTool.IsValidFrom"/>
    /// <seealso cref="Northwoods.GoXam.Tool.LinkingBaseTool.IsValidTo"/>
    public bool AllowLink {
      get { return (bool)GetValue(AllowLinkProperty); }
      set { SetValue(AllowLinkProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowRelink"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowRelinkProperty;
    /// <summary>
    /// Gets or sets whether the user may relink
    /// the links that are in this layer.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <seealso cref="Northwoods.GoXam.Diagram.AllowRelink"/>
    /// <seealso cref="Link.RelinkableFrom"/>
    /// <seealso cref="Link.RelinkableTo"/>
    /// <seealso cref="Link.CanRelinkFrom"/>
    /// <seealso cref="Link.CanRelinkTo"/>
    public bool AllowRelink {
      get { return (bool)GetValue(AllowRelinkProperty); }
      set { SetValue(AllowRelinkProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowMove"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowMoveProperty;
    /// <summary>
    /// Gets or sets whether the user may move
    /// the parts that are in this layer.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <seealso cref="Northwoods.GoXam.Diagram.AllowMove"/>
    /// <seealso cref="Part.Movable"/>
    /// <seealso cref="Part.CanMove"/>
    public bool AllowMove {
      get { return (bool)GetValue(AllowMoveProperty); }
      set { SetValue(AllowMoveProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowPrint"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowPrintProperty;
    /// <summary>
    /// Gets or sets whether the user may print
    /// the parts that are in this layer.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <seealso cref="Northwoods.GoXam.Diagram.AllowPrint"/>
    /// <seealso cref="Part.Printable"/>
    /// <seealso cref="Part.CanPrint"/>
    public bool AllowPrint {
      get { return (bool)GetValue(AllowPrintProperty); }
      set { SetValue(AllowPrintProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowReshape"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowReshapeProperty;
    /// <summary>
    /// Gets or sets whether the user may reshape
    /// the parts that are in this layer.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <seealso cref="Northwoods.GoXam.Diagram.AllowReshape"/>
    /// <seealso cref="Part.Reshapable"/>
    /// <seealso cref="Part.CanReshape"/>
    public bool AllowReshape {
      get { return (bool)GetValue(AllowReshapeProperty); }
      set { SetValue(AllowReshapeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowResize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowResizeProperty;
    /// <summary>
    /// Gets or sets whether the user may resize
    /// the parts that are in this layer.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <seealso cref="Northwoods.GoXam.Diagram.AllowResize"/>
    /// <seealso cref="Part.Resizable"/>
    /// <seealso cref="Part.CanResize"/>
    public bool AllowResize {
      get { return (bool)GetValue(AllowResizeProperty); }
      set { SetValue(AllowResizeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowRotate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowRotateProperty;
    /// <summary>
    /// Gets or sets whether the user may rotate
    /// the parts that are in this layer.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <seealso cref="Northwoods.GoXam.Diagram.AllowRotate"/>
    /// <seealso cref="Part.Rotatable"/>
    /// <seealso cref="Part.CanRotate"/>
    public bool AllowRotate {
      get { return (bool)GetValue(AllowRotateProperty); }
      set { SetValue(AllowRotateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowSelect"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowSelectProperty;
    /// <summary>
    /// Gets or sets whether the user may select
    /// the parts that are in this layer.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <seealso cref="Northwoods.GoXam.Diagram.AllowSelect"/>
    /// <seealso cref="Part.Selectable"/>
    /// <seealso cref="Part.CanSelect"/>
    public bool AllowSelect {
      get { return (bool)GetValue(AllowSelectProperty); }
      set { SetValue(AllowSelectProperty, value); }
    }


    /// <summary>
    /// Search the parts in this layer that are at a given point that meet a given predicate,
    /// and return the first element that matches.
    /// </summary>
    /// <typeparam name="T">the type of element being searched for</typeparam>
    /// <param name="p">a <c>Point</c> in model coordinates</param>
    /// <param name="navig">
    /// This is a function that is given an element at the given point and
    /// returns an element of type <typeparamref name="T"/> to be considered by the predicate <paramref name="pred"/>.
    /// Typically the function will find the ancestor <see cref="Part"/> or <see cref="Node"/>.
    /// </param>
    /// <param name="pred">
    /// This is a predicate that is given an element of type <typeparamref name="T"/>;
    /// if the predicate returns true, this method returns that element.
    /// </param>
    /// <returns>an element of type <typeparamref name="T"/></returns>
    internal T FindElementAt<T>(Point p, Func<DependencyObject, T> navig, Predicate<T> pred)

      where T : UIElement



 {
      VerifyAccess();

      IEnumerable<UIElement> elts = VisualTreeHelper.FindElementsInHostCoordinates(this.Panel.TransformModelToGlobal(p), this);
      foreach (UIElement elt in elts) {
        T part = navig(elt);
        if (part != null && pred(part)) return part;
      }
      return null;



















    }

    /// <summary>
    /// Search the parts in this layer that are at a given point that meet a given predicate,
    /// and return a collection of elements that match.
    /// </summary>
    /// <typeparam name="T">the type of element being searched for</typeparam>
    /// <param name="p">a <c>Point</c> in model coordinates</param>
    /// <param name="navig">
    /// This is a function that is given an element at the given point and
    /// returns an element of type <typeparamref name="T"/> to be considered by the predicate <paramref name="pred"/>.
    /// Typically the function will find the ancestor <see cref="Part"/> or <see cref="Node"/>.
    /// </param>
    /// <param name="pred">
    /// This is a predicate that is given an element of type <typeparamref name="T"/>;
    /// if the predicate returns true, this method includes that element in its results.
    /// </param>
    /// <returns>a collection of elements of type <typeparamref name="T"/></returns>
    internal IEnumerable<T> FindElementsAt<T>(Point p, Func<DependencyObject, T> navig, Predicate<T> pred)

      where T : UIElement



 {
      VerifyAccess();
      HashSet<T> coll = new HashSet<T>();

      IEnumerable<UIElement> elts = VisualTreeHelper.FindElementsInHostCoordinates(this.Panel.TransformModelToGlobal(p), this);
      foreach (UIElement elt in elts) {
        T part = navig(elt);
        if (part != null && pred(part)) {
          coll.Add(part);
        }
      }













      return coll;
    }


    internal IEnumerable<T> FindElementsWithin<T>(Geometry geo, Func<DependencyObject, T> navig, Predicate<T> pred)

      where T : UIElement



 {
      VerifyAccess();
      List<T> coll = new List<T>();

      Rect r = geo.Bounds;
      Point p1 = this.Panel.TransformModelToGlobal(new Point(r.Left, r.Top));
      Point p2 = this.Panel.TransformModelToGlobal(new Point(r.Right, r.Bottom));
      Rect b = new Rect(p1, p2);
      IEnumerable<UIElement> elts = VisualTreeHelper.FindElementsInHostCoordinates(b, this);
      if (geo is EllipseGeometry) {
        Point[] ellipse = Geo.ApproximateEllipse(geo as EllipseGeometry);
        foreach (UIElement elt in elts) {
          if (Geo.EllipseOverlapsElement(ellipse, r, elt)) {
            T part = navig(elt);
            if (part != null && pred(part)) {
              coll.Add(part);
            }
          }
        }
      } else {
        foreach (UIElement elt in elts) {
          T part = navig(elt);
          if (part != null && pred(part)) {
            coll.Add(part);
          }
        }
      }


















      return coll;
    }


    internal IEnumerable<T> FindPartsWithin<T>(Geometry geo, SearchFlags srch, SearchInclusion overlap) where T : Part {
      VerifyAccess();
      bool isnodeslayer = this is NodeLayer;
      bool node = (srch & SearchFlags.SimpleNode) != 0;
      bool group = (srch & SearchFlags.Group) != 0;
      bool label = (srch & SearchFlags.Label) != 0;
      bool link = (srch & SearchFlags.Links) != 0;
      bool selectable = (srch & SearchFlags.Selectable) != 0;
      bool mightreject = (overlap == SearchInclusion.Inside);
      HashSet<T> partsfound = new HashSet<T>();
      HashSet<T> partsrejected = new HashSet<T>();

      Rect r = geo.Bounds;
      Point p1 = this.Panel.TransformModelToGlobal(new Point(r.Left, r.Top));
      Point p2 = this.Panel.TransformModelToGlobal(new Point(r.Right, r.Bottom));
      Rect b = new Rect(p1, p2);
      IEnumerable<UIElement> elts = VisualTreeHelper.FindElementsInHostCoordinates(b, this);
      if (geo is EllipseGeometry) {
        Point[] ellipse = Geo.ApproximateEllipse(geo as EllipseGeometry);
        foreach (UIElement elt in elts) { // For each element in the bounds of the ellipse
          T part = Diagram.FindAncestor<T>(elt);
          if (part != null && (mightreject ? !partsrejected.Contains(part) : !partsfound.Contains(part))) {
            if (part == null || !part.CanSelect()) continue;
            if (part is Link && !link) continue;
            if (isnodeslayer) {
              Node n = part as Node;
              if (n == null) continue;
              Group g = n as Group;
              if (!((group && g != null) || (label && n.IsLinkLabel) || (node && g == null))) continue;
            }

            if (Geo.EllipseOverlapsPart(ellipse, r, part)) { //?? Needs to return some sort of indication of whether there was an intersection
              partsfound.Add(part);
              if (overlap == SearchInclusion.Inside && !Geo.Contains(r, part.Bounds)) {
                partsrejected.Add(part);
              }
            }
          }
        } // end foreach
      } else {
        foreach (UIElement elt in elts) {
          T part = Diagram.FindAncestor<T>(elt);
          if (part != null && (mightreject ? !partsrejected.Contains(part) : !partsfound.Contains(part))) {
            if (isnodeslayer) {
              Node n = part as Node;
              if (n == null) continue;
              if (selectable && !n.CanSelect()) continue;
              Group g = n as Group;
              if ((group && g != null) || (label && n.IsLinkLabel) || (node && g == null)) {
                partsfound.Add(part);
                if (overlap == SearchInclusion.Inside && !Geo.Contains(r, part.Bounds)) {
                  partsrejected.Add(part);
                }
              }
            } else {
              Link l = part as Link;
              if (l == null) continue;
              if (selectable && !l.CanSelect()) continue;
              if (link) {
                partsfound.Add(part);
                if (overlap == SearchInclusion.Inside && !Geo.Contains(r, part.Bounds)) {
                  partsrejected.Add(part);
                }
              }
            }
          }
        } // end foreach
      }







































      foreach (T part in partsrejected) {
        partsfound.Remove(part);
      }
      return partsfound;
    }
  }


  /// <summary>
  /// This class holds only <see cref="Node"/>s together in one layer.
  /// </summary>
  /// <remarks>
  /// <para>
  /// There can be any mixture of node classes, including <see cref="Group"/> and <see cref="Adornment"/>.
  /// It automatically sorts the nodes within the layer so that each <see cref="Group"/> is behind
  /// its own children in this layer.
  /// </para>
  /// <para>
  /// You may not apply any transforms to a <c>NodeLayer</c>; that is reserved to GoXam for future use.
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class NodeLayer : Layer {


    /// <summary>
    /// Get a collection of <see cref="Node"/>s that this layer holds.
    /// </summary>
    public IEnumerable<Node> Nodes {
      get { return this.Children.Cast<Node>(); }
    }

    /// <summary>
    /// Measure all nodes.
    /// </summary>
    /// <param name="availableSize"></param>
    /// <returns></returns>
    protected override Size MeasureOverride(Size availableSize) {
      foreach (Node n in this.Children) {
        n.Measure(Geo.Unlimited);
      }
      DiagramPanel panel = this.Panel;
      // availableSize is 0x0 when inside a Viewbox -- assume this panel were actually sized to fit the whole diagram
      Size sz = availableSize;
      if ((sz.Width <= 0 || Double.IsInfinity(sz.Width)) && panel != null) {
        Rect b = panel.DiagramBounds;
        sz.Width = b.Width;
      }
      if ((sz.Height <= 0 || Double.IsInfinity(sz.Height)) && panel != null) {
        Rect b = panel.DiagramBounds;
        sz.Height = b.Height;
      }
      return sz;
    }

    /// <summary>
    /// Arrange all nodes that have a defined <see cref="Part.Bounds"/>.
    /// </summary>
    /// <param name="finalSize"></param>
    /// <returns></returns>
    protected override Size ArrangeOverride(Size finalSize) {
      this.DuringArrangeCore = true;
      foreach (Node n in this.Children) {
        Rect b = n.Bounds;
        // must not call Arrange with Double.NaN values
        if (Double.IsNaN(b.X)) b.X = -999999;
        if (Double.IsNaN(b.Y)) b.Y = -999999;
        n.Arrange(b);  //?? why is this needed
      }
      this.DuringArrangeCore = false;
      if (this.DelayRemove.Count > 0) {
        foreach (Node n in this.DelayRemove) InternalRemove(n);
        this.DelayRemove.Clear();
      }
      while (this.DelayAdd.Count > 0) {
        HashSet<Node> delayeds = this.DelayAdd;
        this.DelayAdd = new HashSet<Node>();
        foreach (Node n in delayeds) {
          InternalAdd(n);
          Rect b = n.Bounds;
          // must not call Arrange with Double.NaN values
          if (Double.IsNaN(b.X)) b.X = -999999;
          if (Double.IsNaN(b.Y)) b.Y = -999999;
          n.Arrange(b);  //?? why is this needed
        }
      }
      return finalSize;
    }

    internal void InternalAdd(Node part) {  // don't set Layer
      //if (!(part is Adornment)) Diagram.Debug("adding " + Diagram.Str(part) + ((part is Adornment) ? " adornment" : ""));
      if (this.DuringArrangeCore) {
        this.DelayAdd.Add(part);
        return;
      }
      this.Children.Add(part);
      part.Offscreen = false;
      SortZOrder(part as Group);
      // request calls to Measure and Arrange by the DiagramPanel
      DiagramPanel panel = this.Panel;
      if (panel != null) panel.Invalidate(part, false);
    }

    internal void InternalRemove(Node part) {  // don't set Layer
      //Diagram.Debug("removing " + Diagram.Str(part) + ((part is Adornment) ? " adornment" : ""));
      if (this.DuringArrangeCore) {
        this.DelayRemove.Add(part);
        return;
      }
      this.Children.Remove(part);
      part.Offscreen = true;
      part.ClearAdornments();
      // don't bother trying to measure/arrange/update if offscreen
      DiagramPanel panel = this.Panel;
      if (panel != null) panel.Uninvalidate(part);
    }

    internal void InternalClear(bool cache) {  // used by DiagramPanel
      if (cache) {
        foreach (Node n in this.Children) n.Layer = null;
      }
      this.Children.Clear();
    }








































































































    private bool DuringArrangeCore { get; set; }
    private HashSet<Node> DelayAdd = new HashSet<Node>();
    private HashSet<Node> DelayRemove = new HashSet<Node>();

    /// <summary>
    /// Add a <see cref="Node"/> to this layer.
    /// </summary>
    /// <param name="part"></param>
    /// <remarks>
    /// <para>
    /// This is infrequently called.
    /// You normally should set the part's <see cref="Part.LayerName"/>
    /// in order to specify or change the layer that a part will be in.
    /// </para>
    /// <para>
    /// This does not modify the model.
    /// It just changes the layer that the node is in,
    /// thereby making the node eligible to be seen by the user.
    /// If it had been part of another layer, it is removed from that other layer first.
    /// </para>
    /// </remarks>
    public void Add(Node part) {
      if (part == null) return;
      VerifyAccess();
      NodeLayer oldlayer = part.Layer as NodeLayer;
      if (oldlayer != this) {
        // remove from any other layer
        if (oldlayer != null) oldlayer.InternalRemove(part);
        part.Layer = this;  // backpointer
        InternalAdd(part);
      }
    }

    internal void SortZOrder() {

      UIElementCollection list = this.Children;
      int count = this.Children.Count;




      for (int i = 0; i < count; i++) {
        SortZOrder(list[i] as Group);
      }
    }

    internal void SortZOrder(Group g) {
      if (g == null) return;
      MoveGroupBehind(g);
      foreach (Group sg in g.ContainingGroups) {
        SortZOrder(sg);
      }
    }

    private void MoveGroupBehind(Group sg) {

      UIElementCollection list = this.Children;



      int idx = list.IndexOf(sg);  //?? inefficient
      if (idx >= 0) {
        IEnumerable<Node> members = sg.MemberNodes;
        if (members.Count() > 0) {
          int min = Int32.MaxValue;
          foreach (Node m in members) {
            int i = list.IndexOf(m);  //?? inefficient
            if (i >= 0) min = Math.Min(min, i);
          }
          if (min < idx) {
            list.RemoveAt(idx);  //?? inefficient
            list.Insert(min, sg);  //?? inefficient





          }
        }
      }
    }

    internal void MoveBehindAll(Node n) {

      UIElementCollection list = this.Children;



      int idx = list.IndexOf(n);  //?? inefficient
      if (idx >= 0) {
        list.RemoveAt(idx);
        list.Insert(0, n);





      }
    }

    /// <summary>
    /// Remove a <see cref="Node"/> from this layer.
    /// </summary>
    /// <param name="part"></param>
    /// <remarks>
    /// <para>
    /// This is infrequently called.
    /// You normally should set the <see cref="Part.LayerName"/>
    /// in order to specify or change the layer that a part will be in.
    /// </para>
    /// <para>
    /// This does not modify the model.
    /// It just changes the layer that the node is in,
    /// removing the node from the visual tree.
    /// If it was not part of this layer, nothing happens.
    /// </para>
    /// </remarks>
    public void Remove(Node part) {
      if (part == null) return;
      VerifyAccess();
      if (part.Layer == this) {
        InternalRemove(part);
        part.Layer = null;  // backpointer
      }
    }
  }  // end of NodeLayer


  internal sealed class DefaultLayer : Layer {

    public DefaultLayer() {
      this.IsHitTestVisible = false;
    }

    private void OnRender() {
      if (this.Children.Count > 1) this.Children.Clear();
      TextBlock tb = new TextBlock();
      Texted(tb);
      Locationed(tb);
      this.Children.Add(tb);
    }

    private bool Transformed(UIElement elt) {
      elt.Visibility = Visibility.Visible;
      elt.Opacity = 1;
      elt.RenderTransformOrigin = new Point(0, 0);
      Transform xfm = elt.RenderTransform;
      if (xfm == null) return false;
      MatrixTransform mx = xfm as MatrixTransform;
      if (mx == null) return true;
      return !mx.Matrix.IsIdentity;
    }

    private byte c;
    private bool Texted(TextBlock tb) {
      tb.FontSize = 12;
      SolidColorBrush br = tb.Foreground as SolidColorBrush;
      if (br == null || c == 0 || br.Color.R != c || br.Color.G != c || br.Color.B != c) {
        c = (byte)(150+_Random.Next(100));
        tb.Foreground = new SolidColorBrush(Color.FromArgb(255, c, c, c));
      }
      if (tb.Text.Length < 50 || !tb.Text.StartsWith("GoSilverlight")) {
        tb.Text = "GoSilverlight evaluation\n(c) Northwoods Software\nwww.nwoods.com\nNot for distribution";
      }
      return false;
    }

    private bool Locationed(UIElement elt) {
      elt.Measure(Geo.Unlimited);
      Size sz = elt.DesiredSize;
      Point p = new Point(10, 10);
      DiagramPanel panel = this.Panel;
      if (panel != null) {
        Rect vb = panel.ViewportBounds;
        p.X = Math.Max(0, vb.X + vb.Width / 2 - sz.Width / 2);
        p.Y = Math.Max(0, vb.Y + vb.Height / 2 - sz.Height / 2);
        p = panel.TransformModelToView(p);
        //Diagram.Debug(Diagram.Str(new Rect(p.X, p.Y, sz.Width, sz.Height)));
      }
      elt.Arrange(new Rect(p.X, p.Y, sz.Width, sz.Height));
      return false;
    }

    protected override Size ArrangeOverride(Size finalSize) {
      if (this.Children.Count == 0) {
        OnRender();
      }
      else if (this.Children.Count > 1) {
        OnRender();
      }
      else {
        TextBlock tb = this.Children[0] as TextBlock;
        if (tb == null || Transformed(tb) || Texted(tb) || Locationed(tb)) {
          OnRender();
        }
      }
      return base.ArrangeOverride(finalSize);
    }































    private Random _Random = new Random();

    internal const String S1 = "Northwoods.GoXam.Diagram.LicenseKey has not yet been set; running with an evaluation license.";
    internal const String S2 = "Component assembly has been modified ";
    internal const String S3 = "{0:X}";
    internal const String S4 = "Invalid Diagram.LicenseKey value: ";
    internal const String S5 = "\n  component: ";
    internal const String S6 = "\nno Deployment.Current";
    internal const String S7 = "\nno Application.Current";
    internal const String S8 = "\n  licensed component: ";
    internal const String S9 = "\n  licensed application: ";
    internal const String S10 = "\nPlease set Diagram.LicenseKey to a run-time license key for ";
    internal const String S11 = "\nPlease upgrade the Diagram.LicenseKey to at least version ";
    internal const String S12 = "\nPlease set Diagram.LicenseKey to a run-time license key for your application: ";
    internal const String S13 = "evaluation";
    internal const String S14 = "\nNorthwoods.GoXam license error:\n";
    internal const String S15 = "\nYou can generate a run-time license key value by running the GoXam License Manager and clicking the 'Deploy an Application' button.";
    internal const String S16 = "\nInsert the Northwoods.GoXam.Diagram.LicenseKey assignment statement in your custom System.Windows.Application constructor.\n";
    internal const String S17 = "SOFTWARE\\Northwoods Software\\GoXam";
    internal const String S18 = ".lic";
    internal const String S19 = "\nPlease use an application class that inherits from System.Windows.Application";
    internal const String SNL = "\n";
    internal const String SDOT = ".";
    internal const String SSBO = "[";
    internal const String SSBC = "]";
    internal const String SSP = " ";
    internal const char CEQ = '=';
    internal const char CDOT = '.';
    internal const char CCOM = ',';
    internal const char CVB = '|';
  }  // end of DefaultLayer


  /// <summary>
  /// This class holds only <see cref="Link"/>s together in one layer.
  /// </summary>
  /// <remarks>
  /// <para>
  /// You may not apply any transforms to a <c>LinkLayer</c>; that is reserved to GoXam for future use.
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class LinkLayer : Layer {


    /// <summary>
    /// Get a collection of <see cref="Link"/>s that this layer holds.
    /// </summary>
    public IEnumerable<Link> Links {
      get { return this.Children.Cast<Link>(); }
    }

    /// <summary>
    /// Measure all links.
    /// </summary>
    /// <param name="availableSize"></param>
    /// <returns></returns>
    protected override Size MeasureOverride(Size availableSize) {
      foreach (Link l in this.Children) {
        l.Measure(Geo.Unlimited);
      }
      DiagramPanel panel = this.Panel;
      // availableSize is 0x0 when inside a Viewbox -- assume this panel were actually sized to fit the whole diagram
      Size sz = availableSize;
      if ((sz.Width <= 0 || Double.IsInfinity(sz.Width)) && panel != null) {
        Rect b = panel.DiagramBounds;
        sz.Width = b.Width;
      }
      if ((sz.Height <= 0 || Double.IsInfinity(sz.Height)) && panel != null) {
        Rect b = panel.DiagramBounds;
        sz.Height = b.Height;
      }
      return sz;
    }

    /// <summary>
    /// Arrange all links that have a defined <see cref="Part.Bounds"/>.
    /// </summary>
    /// <param name="finalSize"></param>
    /// <returns></returns>
    protected override Size ArrangeOverride(Size finalSize) {
      this.DuringArrangeCore = true;
      foreach (Link l in this.Children) {
        Rect b = l.Bounds;
        // must not call Arrange with Double.NaN values
        if (!Double.IsNaN(b.X) && !Double.IsNaN(b.Y)) l.Arrange(b);  //?? why is this needed
      }
      this.DuringArrangeCore = false;
      if (this.DelayRemove.Count > 0) {
        foreach (Link l in this.DelayRemove) InternalRemove(l);
        this.DelayRemove.Clear();
      }
      while (this.DelayAdd.Count > 0) {
        HashSet<Link> delayeds = this.DelayAdd;
        this.DelayAdd = new HashSet<Link>();
        foreach (Link l in delayeds) {
          InternalAdd(l);
          Rect b = l.Bounds;
          // must not call Arrange with Double.NaN values
          if (!Double.IsNaN(b.X) && !Double.IsNaN(b.Y)) l.Arrange(b);  //?? why is this needed
        }
      }
      return finalSize;
    }

    internal void InternalAdd(Link part) {  // don't set Layer
      //Diagram.Debug("adding " + Diagram.Str(part));
      if (this.DuringArrangeCore) {
        this.DelayAdd.Add(part);
        return;
      }
      this.Children.Add(part);
      part.Offscreen = false;
      // request calls to Measure and Arrange by the DiagramPanel
      DiagramPanel panel = this.Panel;
      if (panel != null) panel.Invalidate(part, false);
    }

    internal void InternalRemove(Link part) {  // don't set Layer
      //Diagram.Debug("removing " + Diagram.Str(part));
      if (this.DuringArrangeCore) {
        this.DelayRemove.Add(part);
        return;
      }
      this.Children.Remove(part);
      part.Offscreen = true;
      part.ClearAdornments();
      // don't bother trying to measure/arrange/update if offscreen
      DiagramPanel panel = this.Panel;
      if (panel != null) panel.Uninvalidate(part);
    }

    internal void InternalClear(bool cache) {  // used by DiagramPanel
      if (cache) {
        foreach (Link l in this.Children) l.Layer = null;
      }
      this.Children.Clear();
    }









































































































    private bool DuringArrangeCore { get; set; }
    private HashSet<Link> DelayAdd = new HashSet<Link>();
    private HashSet<Link> DelayRemove = new HashSet<Link>();

    /// <summary>
    /// Add a <see cref="Link"/> to this layer.
    /// </summary>
    /// <param name="part"></param>
    /// <remarks>
    /// <para>
    /// This is infrequently called.
    /// You normally should set the part's <see cref="Part.LayerName"/>
    /// in order to specify or change the layer that a part will be in.
    /// </para>
    /// <para>
    /// This does not modify the model.
    /// It just changes the layer that the link is in,
    /// thereby making the link eligible to be seen by the user.
    /// If it had been part of another layer, it is removed from that other layer first.
    /// </para>
    /// </remarks>
    public void Add(Link part) {
      if (part == null) return;
      VerifyAccess();
      LinkLayer oldlayer = part.Layer as LinkLayer;
      if (oldlayer != this) {
        // remove from any other layer
        if (oldlayer != null) oldlayer.InternalRemove(part);
        part.Layer = this;  // backpointer
        InternalAdd(part);
      }
    }

    /// <summary>
    /// Remove a <see cref="Link"/> from this layer.
    /// </summary>
    /// <param name="part"></param>
    /// <remarks>
    /// <para>
    /// This is infrequently called.
    /// You normally should set the <see cref="Part.LayerName"/>
    /// in order to specify or change the layer that a part will be in.
    /// </para>
    /// <para>
    /// This does not modify the model.
    /// It just changes the layer that the link is in,
    /// removing the link from the visual tree.
    /// If it was not part of this layer, nothing happens.
    /// </para>
    /// </remarks>
    public void Remove(Link part) {
      if (part == null) return;
      VerifyAccess();
      if (part.Layer == this) {
        // If a link is removed we must make sure no others are still jumping over it
        part.Route.InvalidateOtherJumpOvers();
        InternalRemove(part);
        part.Layer = null;  // backpointer
      }
    }
  }  // end of LinkLayer
}
