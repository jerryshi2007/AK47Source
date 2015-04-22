
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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

using System.Windows.Printing;

using Northwoods.GoXam.Model;

namespace Northwoods.GoXam {

  /// <summary>
  /// This class is responsible for printing a diagram.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Call the <see cref="Print"/> method to perform printing.
  /// The <see cref="Scale"/> property governs the scale at which it should print;
  /// a value of <c>Double.NaN</c> causes it to "scale-to-fit" the page.
  /// </para>
  /// <para>
  /// Although this class inherits from <c>FrameworkElement</c>
  /// in order to support data binding,
  /// it is not really a <c>FrameworkElement</c> or <c>UIElement</c>!
  /// Please ignore all of the properties, methods, and events defined by
  /// <c>FrameworkElement</c> and <c>UIElement</c>.
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class PrintManager : FrameworkElement



  {

    static PrintManager() {
      ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(PrintManager),
        new FrameworkPropertyMetadata(1.0));
      MarginProperty = DependencyProperty.RegisterAttached("Margin", typeof(Thickness), typeof(Node),
        new FrameworkPropertyMetadata(new Thickness(50)));
    }

    /// <summary>
    /// Identifies the <see cref="Scale"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ScaleProperty;
    /// <summary>
    /// Gets or sets the scale factor at which everything is printed.
    /// </summary>
    /// <value>
    /// The default value is 1.
    /// A value of <c>Double.NaN</c> causes it to print at a scale at which the diagram will fit on the single page.
    /// </value>
    public double Scale {
      get { return (double)GetValue(ScaleProperty); }
      set { SetValue(ScaleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Margin"/> dependency property
    /// </summary>
    new public static readonly DependencyProperty MarginProperty;
    /// <summary>
    /// Gets or sets the margins inside the page size, around the printed area.
    /// </summary>
    /// <value>
    /// The default value is a <c>Thickness</c> of 50 on all sides.
    /// </value>
    new public Thickness Margin {
      get { return (Thickness)GetValue(MarginProperty); }
      set { SetValue(MarginProperty, value); }
    }

    /// <summary>
    /// Gets the <see cref="Northwoods.GoXam.Diagram"/> for which this <see cref="PrintManager"/> performs printing.
    /// </summary>
    /// <value>
    /// This value is automatically set by the <see cref="Northwoods.GoXam.Diagram.PrintManager"/> setter.
    /// </value>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Diagram Diagram { get; internal set; }











































    /// <summary>
    /// Show a print dialog and start printing.
    /// </summary>
    public void Print() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return;
      IDiagramModel model = diagram.Model;
      if (model == null) return;
      String modelname = (model.Name != null ? model.Name : "Diagram");
      this.EffectivePageCount = 0;  // uninitialized
      this.PageIndex = 0;
      PrintDocument pdoc = new PrintDocument();
      pdoc.BeginPrint += pdoc_BeginPrint;
      pdoc.EndPrint += pdoc_EndPrint;
      pdoc.PrintPage += pdoc_PrintPage;
      pdoc.Print(modelname);
    }

    private void pdoc_BeginPrint(object sender, BeginPrintEventArgs e) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      diagram.Cursor = Cursors.Wait;
    }

    private void pdoc_EndPrint(object sender, EndPrintEventArgs e) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return;
      panel.ReleasePrintingPanel();
      panel.UpdateScrollTransform();
      diagram.Cursor = null;
    }

    private void pdoc_PrintPage(object sender, PrintPageEventArgs e) {
      if (this.EffectivePageCount <= 0) {
        this.PageSize = e.PrintableArea;
        this.EffectivePageCount = this.PageCount;
      }
      e.PageVisual = GetPage(this.PageIndex++);
      e.HasMorePages = this.PageIndex < this.EffectivePageCount;
    }

    private int PageCount {
      get {
        this.EffectiveBounds = new Rect(0, 0, 500, 500);
        this.EffectiveScale = 1;
        this.EffectivePageSize = new Size(500, 500);
        this.EffectiveRows = 1;
        this.EffectiveColumns = 1;
        if (this.Diagram == null) return 1;
        DiagramPanel panel = this.Diagram.Panel;
        if (panel == null) return 1;
        // check the PrintableAreaWidth/Height
        Size sz = this.PageSize;
        if (Double.IsNaN(sz.Width) || sz.Width < 1 || Double.IsInfinity(sz.Width) ||
            Double.IsNaN(sz.Height) || sz.Height < 1 || Double.IsInfinity(sz.Height)) {
          return 1;
        }
        Thickness th = this.Margin;
        if (!Double.IsNaN(th.Left) && !Double.IsInfinity(th.Left)) sz.Width -= th.Left;
        if (!Double.IsNaN(th.Right) && !Double.IsInfinity(th.Right)) sz.Width -= th.Right;
        if (!Double.IsNaN(th.Top) && !Double.IsInfinity(th.Top)) sz.Height -= th.Top;
        if (!Double.IsNaN(th.Bottom) && !Double.IsInfinity(th.Bottom)) sz.Height -= th.Bottom;
        // currently the printable region is the same as the DiagramBounds
        Rect b = panel.DiagramBounds;
        if (b.Width <= 0 || b.Height <= 0) return 1;
        // get the print scale; NaN means scale-to-fit
        double scale = this.Scale;
        if (Double.IsNaN(scale) || scale <= 0 || Double.IsInfinity(scale)) {
          this.EffectiveBounds = b;
          this.EffectiveScale = Math.Min(1, Math.Min(sz.Width / b.Width, sz.Height / b.Height));
          sz.Width /= this.EffectiveScale;
          sz.Height /= this.EffectiveScale;
          this.EffectivePageSize = sz;
          this.EffectiveRows = 1;
          this.EffectiveColumns = 1;
          //Diagram.Debug("PageCount: to-fit " + Diagram.Str(this.EffectiveBounds) + this.EffectiveScale.ToString());
          return 1;
        } else {
          this.EffectiveBounds = b;
          this.EffectiveScale = scale;
          sz.Width /= this.EffectiveScale;
          sz.Height /= this.EffectiveScale;
          this.EffectivePageSize = sz;
          int rows = (int)Math.Ceiling(b.Height / sz.Height);
          int cols = (int)Math.Ceiling(b.Width / sz.Width);
          this.EffectiveRows = rows;
          this.EffectiveColumns = cols;
          //??? PageRange
          //Diagram.Debug("PageCount: " + this.EffectiveColumns.ToString() + "x" + this.EffectiveRows.ToString() + " " + Diagram.Str(this.EffectiveBounds) + this.EffectiveScale.ToString());
          return rows*cols;
        }
      }
    }

    private Size PageSize { get; set; }
    private int PageIndex { get; set; }
    private int EffectivePageCount { get; set; }
    private Rect EffectiveBounds { get; set; }  // in model coordinates
    private double EffectiveScale { get; set; }
    private Size EffectivePageSize { get; set; }  // in model coordinates
    private int EffectiveRows { get; set; }
    private int EffectiveColumns { get; set; }

    private UIElement GetPage(int pageNumber) {
      if (this.Diagram == null) return null;
      DiagramPanel panel = this.Diagram.Panel;
      if (panel == null) return null;

      int x = pageNumber % this.EffectiveColumns;
      int y = pageNumber / this.EffectiveColumns;
      Rect b = this.EffectiveBounds;
      Size sz = this.EffectivePageSize;
      Rect viewb = new Rect(b.X + x * sz.Width, b.Y + y * sz.Height, sz.Width, sz.Height);

      double sc = this.EffectiveScale;
      Size pixsz = new Size(sz.Width * sc, sz.Height * sc);
      DiagramPanel img = panel.GrabPrintingPanel(viewb, sc);
      Point pos = new Point(viewb.X, viewb.Y);

      //img.UpdateScrollTransform(pos, sc, pixsz, false);  // instead of UpdateScrollTransform, do:
      GridPattern grid = null;
      Diagram diagram = this.Diagram;
      if (diagram != null) {
        grid = diagram.GridPattern;
        if (grid != null && diagram.GridVisible) {
          grid.DoUpdateBackgroundGrid(panel, new Rect(pos.X, pos.Y, sz.Width, sz.Height), sc, false);
        }
      }

      Canvas root = new Canvas();
      root.Children.Add(img);
      Thickness th = this.Margin;
      if (Double.IsNaN(th.Left) || th.Left > pixsz.Width/2) th.Left = Math.Min(50, pixsz.Width/2);
      if (Double.IsNaN(th.Top) || th.Top > pixsz.Height/2) th.Top = Math.Min(50, pixsz.Height/2);

      //Diagram.Debug("Page: " + pageNumber.ToString() + " " + Diagram.Str(b) + Diagram.Str(viewb) + Diagram.Str(sz) + Diagram.Str(pixsz) + th.ToString());
      var tg = new System.Windows.Media.TransformGroup();
      tg.Children.Add(new System.Windows.Media.TranslateTransform() { X = b.X-pos.X, Y = b.Y-pos.Y });
      tg.Children.Add(new System.Windows.Media.ScaleTransform() { ScaleX = sc, ScaleY = sc });
      tg.Children.Add(new System.Windows.Media.TranslateTransform() { X = th.Left, Y = th.Top });
      img.RenderTransform = tg;

      root.Clip = new System.Windows.Media.RectangleGeometry() { Rect = new Rect(th.Left, th.Top, pixsz.Width, pixsz.Height) };
      root.Measure(Geo.Unlimited);
      root.Arrange(new Rect(th.Left, th.Top, pixsz.Width, pixsz.Height));
      return root;
    }







  }












































































































}
