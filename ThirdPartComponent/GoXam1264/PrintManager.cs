
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
  /// If the area to be printed is large enough, it will print enough pages to cover the area.
  /// The sheets of paper may be taped together to produce a large drawing.
  /// </para>
  /// <para>
  /// Although this class inherits from <c>FrameworkElement</c>
  /// in order to support data binding,
  /// it is not really a <c>FrameworkElement</c> or <c>UIElement</c>!
  /// Please ignore all of the properties, methods, and events defined by
  /// <c>FrameworkElement</c> and <c>UIElement</c>.
  /// </para>
  /// <para>
  /// Normally this will print all of the <see cref="Node"/>s and <see cref="Link"/>s
  /// in the <see cref="Diagram"/> within the <see cref="DiagramPanel.DiagramBounds"/>.
  /// You can specify what parts it prints by supplying the <see cref="Parts"/> collection.
  /// You can also limit the area of the model that is printed by setting the <see cref="Bounds"/> property.
  /// </para>
  /// <para>
  /// You can also control whether the <see cref="Northwoods.GoXam.Diagram"/>'s <c>Background</c> or
  /// any background <see cref="Northwoods.GoXam.Diagram.GridPattern"/> is printed, by setting the
  /// <see cref="PageOptions"/> property.  By default, only the grid is printed.
  /// If you want to have the grid and templates fill the whole page,
  /// set the <see cref="PageOptions"/> property to include <see cref="PrintPageOptions.Full"/>,
  /// for example: <c>PageOptions="FullGridBackground"</c>.
  /// </para>
  /// <para>
  /// The size on the page for the printed parts is governed by the <see cref="Scale"/> property.
  /// You can set that property to zero to cause it to scale everything to fit on one page.
  /// Otherwise the number of pages that will be printed will depend on the area to be printed
  /// (determined by either the parts that are printed or the specified <see cref="Bounds"/>)
  /// times the <see cref="Scale"/>, as will fit in the size of each page minus the <see cref="Margin"/>s.
  /// </para>
  /// <para>
  /// You can print decorations on each page by specifying the
  /// <see cref="BackgroundTemplate"/> and/or the <see cref="ForegroundTemplate"/>.
  /// These templates are applied to each page, data-bound to an instance of
  /// <see cref="PageInfo"/> describing that page.
  /// This makes it easy to customize the header or footer.
  /// For example:
  /// <code>
  /// </code>
  /// &lt;go:Diagram . . .&gt;
  ///   &lt;go:Diagram.PrintManager&gt;
  ///     &lt;go:PrintManager&gt;
  ///       &lt;go:PrintManager.BackgroundTemplate&gt;
  ///         &lt;DataTemplate&gt;
  ///           &lt;go:SpotPanel&gt;
  ///             &lt;TextBlock Text="Confidential"
  ///                        Foreground="LightGray" FontSize="50"
  ///                        RenderTransformOrigin="0.5 0.5"&gt;
  ///               &lt;TextBlock.RenderTransform&gt;
  ///                 &lt;RotateTransform Angle="-45" /&gt;
  ///               &lt;/TextBlock.RenderTransform&gt;
  ///             &lt;/TextBlock&gt;
  ///           &lt;/go:SpotPanel&gt;
  ///         &lt;/DataTemplate&gt;
  ///       &lt;/go:PrintManager.BackgroundTemplate&gt;
  ///     &lt;/go:PrintManager&gt;
  ///   &lt;/go:Diagram.PrintManager&gt;
  /// &lt;/go:Diagram&gt;
  /// </para>
  /// <para>
  /// You can see the definition of the standard <see cref="ForegroundTemplate"/>
  /// in the Generic.XAML file that is in the docs subdirectory of the GoXam installation.
  /// It includes cut marks so that you can cut off the right sides and the bottoms
  /// of each of the pages that are not in the last column or the last row, allowing you
  /// to tape the sheets together to form a visually continuous diagram.
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class PrintManager : FrameworkElement



  {
    /// <summary>
    /// Create a default <see cref="PrintManager"/>.
    /// </summary>
    public PrintManager() {
      this.ForegroundTemplate = Diagram.FindDefault<DataTemplate>("DefaultPrintManagerForegroundTemplate");
    }

    static PrintManager() {
      PartsProperty = DependencyProperty.Register("Parts", typeof(IEnumerable<Part>), typeof(PrintManager),
        new FrameworkPropertyMetadata(null));
      BoundsProperty = DependencyProperty.Register("Bounds", typeof(Rect), typeof(PrintManager),
        new FrameworkPropertyMetadata(Rect.Empty));
      ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(PrintManager),
        new FrameworkPropertyMetadata(1.0));
      MarginProperty = DependencyProperty.Register("Margin", typeof(Thickness), typeof(PrintManager),
        new FrameworkPropertyMetadata(new Thickness(50)));
      PageOptionsProperty = DependencyProperty.Register("PageOptions", typeof(PrintPageOptions), typeof(PrintManager),
        new FrameworkPropertyMetadata(PrintPageOptions.Grid));
      BackgroundTemplateProperty = DependencyProperty.Register("BackgroundTemplate", typeof(DataTemplate), typeof(PrintManager),
        new FrameworkPropertyMetadata(null));
      ForegroundTemplateProperty = DependencyProperty.Register("ForegroundTemplate", typeof(DataTemplate), typeof(PrintManager),
        new FrameworkPropertyMetadata(null));
    }

    /// <summary>
    /// Identifies the <see cref="Parts"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PartsProperty;
    /// <summary>
    /// Gets or sets the collection of <see cref="Part"/>s to be printed.
    /// </summary>
    /// <value>
    /// The default value is <c>null</c>.
    /// A <c>null</c> value or an empty collection causes it to print all printable parts in the diagram.
    /// All given <see cref="Part"/>s must belong to the <see cref="Diagram"/>.
    /// </value>
    public IEnumerable<Part> Parts {
      get { return (IEnumerable<Part>)GetValue(PartsProperty); }
      set { SetValue(PartsProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Bounds"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BoundsProperty;
    /// <summary>
    /// Gets or sets the area of the model to be printed, in model coordinates.
    /// </summary>
    /// <value>
    /// The default value is <c>Rect.Empty</c>.
    /// A value of <c>Rect.Empty</c> causes it to print either
    /// the whole <see cref="DiagramPanel.DiagramBounds"/> if <see cref="Parts"/> is null or empty,
    /// or else the union of the bounds of the given collection of <see cref="Parts"/>.
    /// </value>
    public Rect Bounds {
      get { return (Rect)GetValue(BoundsProperty); }
      set { SetValue(BoundsProperty, value); }
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
    /// A value of <c>Double.NaN</c> causes it to print at a scale at which the printed area of the diagram will fit on a single page.
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
    /// Identifies the <see cref="PageOptions"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PageOptionsProperty;
    /// <summary>
    /// Gets or sets how printing fills each page.
    /// </summary>
    /// <value>
    /// The default value is <see cref="PrintPageOptions.GridBackground"/>.
    /// </value>
    public PrintPageOptions PageOptions {
      get { return (PrintPageOptions)GetValue(PageOptionsProperty); }
      set { SetValue(PageOptionsProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="BackgroundTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BackgroundTemplateProperty;
    /// <summary>
    /// Gets or sets the <c>DataTemplate</c> printed as the background for each page.
    /// </summary>
    /// <value>
    /// The default value is null -- this prints no per-page decorations.
    /// </value>
    /// <remarks>
    /// <para>
    /// The background elements are printed behind any <see cref="GridPattern"/>.
    /// </para>
    /// <para>
    /// The background <c>DataTemplate</c> is data-bound to an instance of
    /// <see cref="PageInfo"/> that describes the current page.
    /// </para>
    /// </remarks>
    public DataTemplate BackgroundTemplate {
      get { return (DataTemplate)GetValue(BackgroundTemplateProperty); }
      set { SetValue(BackgroundTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ForegroundTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ForegroundTemplateProperty;
    /// <summary>
    /// Gets or sets the <c>DataTemplate</c> printed as the foreground for each page.
    /// </summary>
    /// <value>
    /// The default template prints cut marks and page numbers.
    /// You can see its definition in the Generic.XAML file in the docs subdirectory
    /// of the GoXam installation.
    /// </value>
    /// <remarks>
    /// <para>
    /// The foreground elements are printed in front of all <see cref="Part"/>s.
    /// </para>
    /// <para>
    /// The foreground <c>DataTemplate</c> is data-bound to an instance of
    /// <see cref="PageInfo"/> that describes the current page.
    /// </para>
    /// </remarks>
    public DataTemplate ForegroundTemplate {
      get { return (DataTemplate)GetValue(ForegroundTemplateProperty); }
      set { SetValue(ForegroundTemplateProperty, value); }
    }
    

    /// <summary>
    /// Gets the <see cref="Northwoods.GoXam.Diagram"/> for which this <see cref="PrintManager"/> performs printing.
    /// </summary>
    /// <value>
    /// This value is automatically set by the <see cref="Northwoods.GoXam.Diagram.PrintManager"/> setter.
    /// </value>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Diagram Diagram { get; internal set; }

    internal HashSet<Part> PartsToPrint { get; set; }


    /// <summary>
    /// This read-only class provides information about the current page while printing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each page constructs elements for the <see cref="BackgroundTemplate"/> and <see cref="ForegroundTemplate"/>
    /// that are data-bound to an instance of this class.
    /// </para>
    /// <para>
    /// For convenience, the integer index properties are one-based, not zero-based.
    /// This makes it easy to show something like "Page 4 of 4".
    /// </para>
    /// </remarks>
    public sealed class PageInfo {  // nested class
      /// <summary>
      /// Gets a reference to the <see cref="Northwoods.GoXam.Diagram"/> being printed.
      /// </summary>
      public Diagram Diagram { get; internal set; }

      /// <summary>
      /// Gets a one-based index of the current page in the total <see cref="Count"/> of pages to be printed.
      /// </summary>
      public int Index { get; internal set; }  // one-based

      /// <summary>
      /// Gets the total number of pages to be printed.
      /// </summary>
      public int Count { get; internal set; }

      /// <summary>
      /// Gets a one-based index of the current page in the total <see cref="ColumnCount"/> of pages to be printed for each row.
      /// </summary>
      public int Column { get; internal set; }  // one-based

      /// <summary>
      /// Gets the total number of pages to be printed in each row.
      /// </summary>
      public int ColumnCount { get; internal set; }

      /// <summary>
      /// Gets a one-based index of the current page in the total <see cref="RowCount"/> of pages to be printed for each column.
      /// </summary>
      public int Row { get; internal set; }  // one-based

      /// <summary>
      /// Gets the total number of pages to be printed in each column.
      /// </summary>
      public int RowCount { get; internal set; }

      /// <summary>
      /// Gets the bounds, in model coordinates, of the total area of the diagram being printed.
      /// </summary>
      public Rect TotalBounds { get; internal set; }  // model coordinates

      /// <summary>
      /// Gets the bounds, in model coordinates, of the area being printed for this page.
      /// </summary>
      public Rect ViewportBounds { get; internal set; }  // model coordinates

      /// <summary>
      /// Gets the actual scale at which the diagram is being printed.
      /// </summary>
      public double Scale { get; internal set; }

      /// <summary>
      /// Gets the size of the current page, in device-independent pixels.
      /// </summary>
      /// <remarks>
      /// Not all of the page might actually be printed, depending on the capabilities of the printer.
      /// </remarks>
      public Size Size { get; internal set; }  // device-independent pixels

      /// <summary>
      /// Gets the size and location of the diagram area (within the margins), in device-independent pixels.
      /// </summary>
      /// <remarks>
      /// A <see cref="SpotPanel"/> in the template will be measured and arranged to get these bounds.
      /// </remarks>
      public Rect Viewport { get; internal set; }  // device-independent pixels
    }




















































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
      this._wasVirtualizing = panel.IsVirtualizing;
      panel.UpdateAllVisuals();
      panel.IsVirtualizing = false;
      PrintDocument pdoc = new PrintDocument();
      pdoc.BeginPrint += pdoc_BeginPrint;
      pdoc.EndPrint += pdoc_EndPrint;
      pdoc.PrintPage += pdoc_PrintPage;
      String modelname = (model.Name != null ? model.Name : "Diagram");
      pdoc.Print(modelname);
    }

    private bool _wasVirtualizing = true;

    private void pdoc_BeginPrint(object sender, BeginPrintEventArgs e) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      this.EffectivePageCount = 0;  // uninitialized
      this.PageIndex = 0;
      if (this.Parts != null && this.Parts.FirstOrDefault() != null) {
        this.PartsToPrint = new HashSet<Part>(this.Parts);
      } else {
        this.PartsToPrint = null;
      }
      diagram.Cursor = Cursors.Wait;
    }

    private void pdoc_EndPrint(object sender, EndPrintEventArgs e) {
      this.PartsToPrint = null;
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return;
      panel.ReleasePrintingPanel();
      panel.IsVirtualizing = this._wasVirtualizing;
      // Silverlight adornments might have wrong offsets afterwards -- fix up:
      foreach (Part p in diagram.SelectedParts) p.RefreshAdornments();
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
        // the printable region is the same as the PrintManager.Bounds or the
        // DiagramBounds or the bounds of the Parts
        Rect b = this.Bounds;
        if (b.Width <= 0 || b.Height <= 0) {
          if (this.PartsToPrint != null) {
            b = panel.ComputeBounds(this.PartsToPrint);
            Thickness pad = panel.Padding;
            b = new Rect(Math.Floor(b.X - pad.Left), Math.Floor(b.Y - pad.Top),
                         Math.Ceiling(pad.Left + b.Width + pad.Right), Math.Ceiling(pad.Top + b.Height + pad.Bottom));
          } else {
            b = panel.DiagramBounds;
          }
        }
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
          if ((this.PageOptions & PrintPageOptions.Full) != 0) {
            this.EffectiveBounds = new Rect(this.EffectiveBounds.X, this.EffectiveBounds.Y,
                                            this.EffectivePageSize.Width * this.EffectiveColumns,
                                            this.EffectivePageSize.Height * this.EffectiveRows);
          }
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
          if ((this.PageOptions & PrintPageOptions.Full) != 0) {
            this.EffectiveBounds = new Rect(this.EffectiveBounds.X, this.EffectiveBounds.Y,
                                            this.EffectivePageSize.Width * this.EffectiveColumns,
                                            this.EffectivePageSize.Height * this.EffectiveRows);
          }
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
      Rect viewb = new Rect(b.X + x*sz.Width,
                            b.Y + y*sz.Height,
                            Math.Min(sz.Width, Math.Max(1, b.Width-x*sz.Width)),
                            Math.Min(sz.Height, Math.Max(1, b.Height-y*sz.Height)));

      double sc = this.EffectiveScale;
      Size pixsz = new Size(viewb.Width*sc, viewb.Height*sc);
      DiagramPanel ppanel = panel.GrabPrintingPanel(viewb, sc, this.PartsToPrint);

      Thickness th = this.Margin;
      if (Double.IsNaN(th.Left) || th.Left > pixsz.Width/2) th.Left = Math.Min(50, pixsz.Width/2);
      if (Double.IsNaN(th.Top) || th.Top > pixsz.Height/2) th.Top = Math.Min(50, pixsz.Height/2);

      PrintManager.PageInfo info = new PrintManager.PageInfo() {
        Diagram = this.Diagram,
        Index = pageNumber+1,
        Count = this.PageCount,
        Column = x+1,
        ColumnCount = this.EffectiveColumns,
        Row = y+1,
        RowCount = this.EffectiveRows,
        TotalBounds = this.EffectiveBounds,
        ViewportBounds = viewb,
        Scale = sc,
        Size = this.PageSize,
        Viewport = new Rect(th.Left, th.Top, pixsz.Width, pixsz.Height)
      };

      Canvas root = new Canvas();
      Diagram diagram = this.Diagram;
      if (diagram != null &&
          (this.PageOptions & PrintPageOptions.Background) != 0) {
        root.Background = diagram.Background;
      }

      DataTemplate backtemplate = this.BackgroundTemplate;
      if (backtemplate != null) {
        ContentPresenter back = new ContentPresenter();
        back.Content = info;
        back.ContentTemplate = backtemplate;
        Canvas.SetLeft(back, th.Left);
        Canvas.SetTop(back, th.Top);
        root.Children.Add(back);
      }

      if (diagram != null && diagram.GridVisible && diagram.GridPattern != null &&
          (this.PageOptions & PrintPageOptions.Grid) != 0) {
        GridPattern grid = diagram.GridPattern;
        grid.DoUpdateBackgroundGrid(panel, new Rect(viewb.X, viewb.Y, sz.Width, sz.Height), sc, false);
        grid.Width = viewb.Width;
        grid.Height = viewb.Height;
        Canvas.SetLeft(grid, th.Left);
        Canvas.SetTop(grid, th.Top);
        root.Children.Add(grid);
      }

      // instead of ppanel.UpdateScrollTransform(new Point(viewb.X, viewb.Y), sc, pixsz, false):
      var tg = new System.Windows.Media.TransformGroup();
      tg.Children.Add(new System.Windows.Media.TranslateTransform() { X = -viewb.X, Y = -viewb.Y });
      tg.Children.Add(new System.Windows.Media.ScaleTransform() { ScaleX = sc, ScaleY = sc });
      ppanel.RenderTransform = tg;

      // clip
      foreach (UIElement lay in ppanel.Children) {
        lay.Clip = new System.Windows.Media.RectangleGeometry() { Rect = new Rect(viewb.X, viewb.Y, pixsz.Width/sc, pixsz.Height/sc) };
      }

      Canvas.SetLeft(ppanel, th.Left);
      Canvas.SetTop(ppanel, th.Top);
      root.Children.Add(ppanel);

      DataTemplate foretemplate = this.ForegroundTemplate;
      if (foretemplate != null) {
        ContentPresenter fore = new ContentPresenter();
        fore.Content = info;
        fore.ContentTemplate = foretemplate;
        Canvas.SetLeft(fore, th.Left);
        Canvas.SetTop(fore, th.Top);
        root.Children.Add(fore);
      }

      root.Measure(this.PageSize);
      root.Arrange(new Rect(0, 0, this.PageSize.Width, this.PageSize.Height));
      return root;
    }







  }






























































































































































































  /// <summary>
  /// This enumeration provides options for controlling how printing occupies each page.
  /// </summary>
  [Flags]
  public enum PrintPageOptions {
    /// <summary>
    /// Only print the requested area of the diagram or the given parts.
    /// </summary>
    None = 0,
    /// <summary>
    /// Print the <see cref="Northwoods.GoXam.Diagram"/>'s <c>Background</c>.
    /// </summary>
    Background = 1,
    /// <summary>
    /// Print any background grid pattern that the diagram may have.
    /// (<see cref="Northwoods.GoXam.Diagram.GridVisible"/> must also be true.)
    /// </summary>
    Grid = 2,
    /// <summary>
    /// Print both the <see cref="Northwoods.GoXam.Diagram"/>'s <c>Background</c>
    /// and any visible <see cref="Northwoods.GoXam.Diagram.GridPattern"/>.
    /// </summary>
    GridBackground = Grid | Background,
    /// <summary>
    /// Print over the whole page, not just along the left or top sides for those
    /// pages that are in the last column or row.
    /// </summary>
    Full = 4,
    /// <summary>
    /// Print over the whole page and include the <see cref="Northwoods.GoXam.Diagram"/>'s <c>Background</c>.
    /// </summary>
    FullBackground = Full | Background,
    /// <summary>
    /// Print over the whole page and include the <see cref="Northwoods.GoXam.Diagram"/>'s
    /// <see cref="Northwoods.GoXam.Diagram.GridPattern"/>, if <see cref="Northwoods.GoXam.Diagram.GridVisible"/>.
    /// </summary>
    FullGrid = Full | Grid,
    /// <summary>
    /// Print over the whole page and include the <see cref="Northwoods.GoXam.Diagram"/>'s
    /// <see cref="Northwoods.GoXam.Diagram.GridPattern"/> and Background.
    /// </summary>
    FullGridBackground = Full | Grid | Background,
    //??? Center = 8,  // instead of Top-Left aligned
    //?? SkipEmpty = 16,  // don't print pages without diagram contents in them
  }
}
