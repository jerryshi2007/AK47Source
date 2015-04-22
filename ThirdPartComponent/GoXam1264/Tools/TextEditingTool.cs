
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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Northwoods.GoXam;

namespace Northwoods.GoXam.Tool {

  /// <summary>
  /// The <c>TextEditingTool</c> is used to let the user interactively edit text in place.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Typically this is used by setting the <c>go:Part.TextEditable</c> attached property to true
  /// on a particular <c>TextBlock</c> in a node.
  /// When the node is selected and the user clicks on the <c>TextBlock</c>
  /// or invokes the <see cref="CommandHandler.Edit"/> command,
  /// this tool is started and it uses an <see cref="ITextEditor"/> to perform in-place text editing.
  /// This text editor control is held in an <see cref="Adornment"/> so that it can be positioned
  /// in front of the <c>TextBlock</c>.
  /// (For more details see the description for <see cref="DoActivate"/>.)
  /// </para>
  /// <para>
  /// The <c>TextBlock</c> is accessible as the <see cref="TextBlock"/> property.
  /// The text editor is accessible as the <see cref="TextEditor"/> property;
  /// the adornment holding the editor is accessible as the <see cref="EditorAdornment"/> property.
  /// From the text editor control one can access the <c>TextBlock</c> being edited via the
  /// <see cref="ITextEditor.TextEditingTool"/> to get to this tool, from which one can use
  /// the <see cref="TextBlock"/> property.
  /// </para>
  /// <para>
  /// You can disable mouse clicking from starting this text editing tool
  /// by setting <see cref="DiagramTool.MouseEnabled"/> to false.
  /// You can disable the F2 key from starting this text editing tool
  /// by making sure <see cref="Part.CanEdit"/> returns false,
  /// by either setting <see cref="Northwoods.GoXam.Diagram.AllowEdit"/> to false
  /// or by setting <see cref="Part.Editable"/> to false.
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class TextEditingTool : DiagramTool {

    static TextEditingTool() {
      StartingProperty = DependencyProperty.Register("Starting", typeof(TextEditingStarting), typeof(TextEditingTool), new FrameworkPropertyMetadata(TextEditingStarting.SingleClickSelected));
    }

    private const String ToolCategory = "TextEdit";

    /// <summary>
    /// Identifies the <see cref="Starting"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StartingProperty;

    /// <summary>
    /// Gets or sets how user gestures can start in-place editing of text.
    /// </summary>
    /// <value>
    /// The default is <see cref="TextEditingStarting.SingleClickSelected"/>.
    /// </value>
    public TextEditingStarting Starting {
      get { return (TextEditingStarting)GetValue(StartingProperty); }
      set { SetValue(StartingProperty, value); }
    }

    /// <summary>
    /// The <see cref="TextEditingTool"/> may run when there is a mouse-click on a <c>TextBlock</c>
    /// for which the <c>go:Part.TextEditable</c> attached property is true in a <see cref="Part"/>
    /// that <see cref="Part.IsSelected"/>.
    /// </summary>
    /// <returns></returns>
    public override bool CanStart() {
      if (!base.CanStart()) return false;

      Diagram diagram = this.Diagram;

      // heed IsReadOnly
      if (diagram == null || diagram.IsReadOnly) return false;

      // only works with the left button
      if (!IsLeftButtonDown()) return false;

      // the mouse down point needs to be near the mouse up point
      if (IsBeyondDragSize()) return false;

      Point p = diagram.LastMousePointInModel;
      TextBlock tb = diagram.Panel.FindElementAt<TextBlock>(p, Diagram.FindAncestorOrSelf<TextBlock>, x => true, SearchLayers.Parts);
      if (tb == null) {







        return false;
      }
      if (!Part.GetTextEditable(tb)) return false;

      Part part = Diagram.FindAncestor<Part>(tb);
      if (part == null) return false;
      if (this.Starting == TextEditingStarting.SingleClickSelected && !part.IsSelected) return false;

      return true;
    }

    /// <summary>
    /// Gets or sets the <c>TextBlock</c> that is being edited.
    /// </summary>
    /// <value>
    /// This property is initially null and is set in <see cref="DoActivate"/>
    /// as the <c>TextBlock</c> at the mouse click point.
    /// However, if you set this property beforehand, <see cref="DoActivate"/>
    /// will not set it, and this tool will edit the given <c>TextBlock</c>.
    /// </value>
    public TextBlock TextBlock { get; set; }

    /// <summary>
    /// Gets the current <see cref="Part"/> that the <see cref="TextBlock"/> is in.
    /// </summary>
    protected Part AdornedPart {
      get { return Diagram.FindAncestor<Part>(this.TextBlock); }
    }

    /// <summary>
    /// Gets or sets the "editor" <see cref="Adornment"/> that contains the control
    /// used to edit the text.
    /// </summary>
    /// <value>
    /// This is set by <see cref="DoActivate"/>.
    /// </value>
    public Adornment EditorAdornment { get; set; }

    /// <summary>
    /// Gets or sets the "editing" code that manages the interactions of the <see cref="EditorAdornment"/>.
    /// </summary>
    /// <value>
    /// This is set by <see cref="DoActivate"/>.
    /// </value>
    public ITextEditor TextEditor { get; set; }

    /// <summary>
    /// When starting this tool, call <see cref="DoActivate"/> if there is a
    /// <see cref="TextBlock"/> supplied.
    /// </summary>
    public override void DoStart() {
      if (!this.Active && this.TextBlock != null) {
        DoActivate();
      }
    }

    /// <summary>
    /// Start editing the text for a <see cref="TextBlock"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If <see cref="TextBlock"/> is not already specified, this looks for one at the current
    /// mouse point.  If none is found, this method does nothing.
    /// </para>
    /// <para>
    /// This method then creates an <see cref="Adornment"/> using the <see cref="Part.GetTextEditAdornmentTemplate"/>,
    /// or a default data template that displays a <c>TextBox</c>.
    /// The adornment is remembered as the <see cref="EditorAdornment"/> property.
    /// </para>
    /// <para>
    /// Then this method finds an <see cref="ITextEditor"/> to manage the <see cref="EditorAdornment"/>.
    /// If the root visual element of the adornment template implements <see cref="ITextEditor"/>,
    /// it uses that.
    /// Otherwise it uses <see cref="Part.GetTextEditor"/> on the <c>TextBlock</c>.
    /// Finally, by default it uses one that understands <c>TextBox</c>es, which matches the
    /// implementation of the default TextEditAdornmentTemplate.
    /// </para>
    /// <para>
    /// This also calls <see cref="DiagramTool.StartTransaction"/>
    /// and sets <see cref="DiagramTool.Active"/> to true.
    /// You should call <see cref="AcceptText"/> if you want to finish the edit
    /// by modifying the <see cref="TextBlock"/> and committing the edit transaction.
    /// Or call <see cref="DiagramTool.DoCancel"/> if you want to abort the edit.
    /// </para>
    /// </remarks>
    public override void DoActivate() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      // there should be a TextBlock; find it if not given
      if (this.TextBlock == null) {
        this.TextBlock = diagram.Panel.FindElementAt<TextBlock>(diagram.LastMousePointInModel,
                            Diagram.FindAncestorOrSelf<TextBlock>, x => true, SearchLayers.Parts);
      }
      if (this.TextBlock == null) {








        return;
      }
      //if (!Part.GetTextEditable(this.TextBlock)) return;  //?? able to operate on all TextBlocks
      Part part = this.AdornedPart;
      if (part == null) return;  //?? don't check for IsSelected here, just in CanStart?

      // make an EditorAdornment
      Adornment adornment = part.GetAdornment(ToolCategory);
      if (adornment == null) {
        DataTemplate template = Part.GetTextEditAdornmentTemplate(this.TextBlock);
        if (template == null) {
          template = Diagram.FindDefault<DataTemplate>("DefaultTextEditAdornmentTemplate");
        }
        adornment = part.MakeAdornment(this.TextBlock, template);
        if (adornment != null) {
          adornment.Category = ToolCategory;
          adornment.LocationSpot = Spot.Center;
        }
      }
      this.EditorAdornment = adornment;
      part.SetAdornment(ToolCategory, adornment);

      // start editing
      if (adornment != null) {
        this.Active = true;
        this.State = TextEditingState.Active;
        StartTransaction(ToolCategory);

        // make sure there's a TextEditor
        // If the adornment's root element is a UserControl that implements ITextEditor, use it
        this.TextEditor = adornment.VisualElement as ITextEditor;
        // otherwise look on the part itself
        if (this.TextEditor == null) {
          this.TextEditor = Part.GetTextEditor(this.TextBlock);
        }
        if (this.TextEditor == null) {
          // this matches up with the default TextEditAdornmentTemplate, which creates a TextBox
          this.TextEditor = new TextBoxEditor();
        }
        this.TextEditor.GotFocus += HandleFocus;

        // default location
        Point loc = this.AdornedPart.GetElementPoint(this.TextBlock, Spot.Center);
        adornment.Location = loc;

        // the text editor is not rotated!

        // setting the TextEditingTool should initialize its Text value and assign focus
        this.TextEditor.TextEditingTool = this;
      }
    }





    /// <summary>
    /// This stops the current transaction and removes the editor adornment.
    /// </summary>
    /// <remarks>
    /// You should call <see cref="AcceptText"/> if you want to modify the
    /// <see cref="TextBlock"/> and commit the edit transaction.
    /// Or call <see cref="DiagramTool.DoCancel"/> if you want to abort the edit.
    /// </remarks>
    public override void DoDeactivate() {
      if (this.Active) StopTransaction();
      this.State = TextEditingState.None;







      this.TextBlock = null;
      if (this.EditorAdornment != null) {
        if (this.EditorAdornment.AdornedPart != null) {
          this.EditorAdornment.AdornedPart.SetAdornment(ToolCategory, null);
        }
        this.EditorAdornment = null;
      }
      if (this.TextEditor != null) {
        this.TextEditor.GotFocus -= HandleFocus;
        this.TextEditor.TextEditingTool = null;
        this.TextEditor = null;
      }
      this.Active = false;
    }

    /// <summary>
    /// A click (mouse up) calls <see cref="DoActivate"/> if this tool is not already active
    /// and if <see cref="CanStart"/> returns true.
    /// </summary>
    public override void DoMouseUp() {
      if (!this.Active && CanStart()) {
        DoActivate();
      }
    }

    /// <summary>
    /// If the user clicks elsewhere in the diagram, call <see cref="AcceptText"/>.
    /// </summary>
    public override void DoMouseDown() {
      if (this.Active) {
        AcceptText(TextEditingReason.MouseDown);
      }
    }

    /// <summary>
    /// Finish editing by trying to accept the new text.
    /// </summary>
    /// <remarks>
    /// Basically this just calls <see cref="DoAcceptText"/>
    /// </remarks>
    public void AcceptText(TextEditingReason reason) {
      switch (reason) {
        case TextEditingReason.MouseDown:
          if (this.State == TextEditingState.Validated ||
              this.State == TextEditingState.Editing2) {
            this.TextEditor.Focus();
          } else if (this.State == TextEditingState.Active ||
                     this.State == TextEditingState.Editing) {
                     this.State = TextEditingState.Validating;
            DoAcceptText();
          }
          break;
        case TextEditingReason.LostFocus:
        case TextEditingReason.Enter:
        case TextEditingReason.Tab:
          if (this.State == TextEditingState.Active ||
              this.State == TextEditingState.Editing) {
            this.State = TextEditingState.Validating;
            DoAcceptText();
          }
          break;
      }
    }

    /// <summary>
    /// Modify the <see cref="TextBlock"/>'s <c>Text</c> property to
    /// the new text string value if it <see cref="IsValidText"/>.
    /// </summary>
    /// <returns>
    /// True if it succeeds, false if <see cref="IsValidText"/> was false.
    /// </returns>
    /// <remarks>
    /// If <see cref="IsValidText"/> is true,
    /// this sets the <see cref="TextBlock"/>'s <c>Text</c> property,
    /// raises the <see cref="Diagram"/>'s <c>TextEditedEvent</c>,
    /// gives focus to the diagram,
    /// sets the <see cref="DiagramTool.TransactionResult"/>,
    /// and stops this tool.
    /// If <see cref="IsValidText"/> is false,
    /// this method does nothing and editing continues.
    /// </remarks>
    protected virtual bool DoAcceptText() {  //??? validation
      if (this.TextBlock != null && this.TextEditor != null) {
        String oldstring = this.TextBlock.Text;
        String newstring = this.TextEditor.Text;
        bool valid = IsValidText(oldstring, newstring);
        this.State = TextEditingState.Validated;
        if (!valid) {
          return false;
        }
        if (oldstring != newstring) {
          this.TextBlock.Text = newstring;
        }
        if (this.AdornedPart != null) this.AdornedPart.Remeasure();
        this.TransactionResult = ToolCategory;
        RaiseEvent(Diagram.TextEditedEvent, new DiagramEventArgs(this.AdornedPart, this.TextBlock));
        StopTool();
        if (this.Diagram != null) this.Diagram.Focus();
      }
      return true;
    }

    internal /*?? public */ TextEditingState State { get; set; }

    private void HandleFocus(Object sender, EventArgs e) {
      if (this.State == TextEditingState.Active) {
        this.State = TextEditingState.Editing;
      } else if (this.State == TextEditingState.Validated) {

        this.State = TextEditingState.Editing2;



      } else if (this.State == TextEditingState.Editing2) {
        this.State = TextEditingState.Editing;
      }
    }

    /// <summary>
    /// Decide whether the proposed new text string is valid.
    /// </summary>
    /// <param name="oldstring"></param>
    /// <param name="newstring"></param>
    /// <returns>By default this returns true</returns>
    /// <remarks>
    /// You might want to override this method to provide custom validation.
    /// </remarks>
    protected virtual bool IsValidText(String oldstring, String newstring) {
      if (this.TextEditor != null)
        return this.TextEditor.IsValidText(oldstring, newstring);
      else
        return true;
    }
  }

  /// <summary>
  /// Enumerate reasons for calling <see cref="TextEditingTool.AcceptText"/>.
  /// </summary>
  public enum TextEditingReason {
    /// <summary>
    /// The text editing control has lost focus.
    /// </summary>
    LostFocus,
    /// <summary>
    /// The user has clicked somewhere else in the diagram.
    /// </summary>
    MouseDown,
    /// <summary>
    /// The user has typed TAB.
    /// </summary>
    Tab,
    /// <summary>
    /// The user has typed ENTER.
    /// </summary>
    Enter
  }

  internal /*?? public */ enum TextEditingState {
    None = 0,
    Active,
    Editing,
    Editing2,
    Validating,
    Validated
  }


  /// <summary>
  /// This interface manages the text editing interaction performed by the 
  /// <see cref="TextEditingTool"/>'s <see cref="Northwoods.GoXam.Tool.TextEditingTool.EditorAdornment"/>.
  /// </summary>
  public interface ITextEditor {
    /// <summary>
    /// Gets or sets the <see cref="TextEditingTool"/>.
    /// </summary>
    /// <remarks>
    /// You will want to have the setter perform the editing control's initialization
    /// when the new value is not null.
    /// </remarks>
    TextEditingTool TextEditingTool { get; set; }

    /// <summary>
    /// Give focus to the editing control.
    /// </summary>
    void Focus();

    /// <summary>
    /// An event indicating that the editing control has gotten focus.
    /// </summary>
    event EventHandler GotFocus;

    /// <summary>
    /// Gets or sets the current text value being edited.
    /// </summary>
    String Text { get; set; }

    /// <summary>
    /// This predicate should be true if the <paramref name="newstring"/> is a valid value.
    /// </summary>
    /// <param name="oldstring"></param>
    /// <param name="newstring"></param>
    /// <returns></returns>
    bool IsValidText(String oldstring, String newstring);
  }


  internal class TextBoxEditor : ITextEditor {
    public TextEditingTool TextEditingTool {
      get { return _TextEditingTool; }
      set {
        _TextEditingTool = value;
        if (value != null) {
          TextBox box = this.TextBox;
          if (box != null) {
            this.Text = value.TextBlock.Text;
            box.SelectAll();
            box.KeyDown += (s, e) => HandleKey(e);
            box.TextChanged += (s, e) => LayoutBox(box);
            box.GotFocus += (s, e) => RaiseFocus(s, e);
            box.LostFocus += (s, e) => LostFocus();
            LayoutBox(box);  // initial positioning
            Focus();
            //?? support real-time editing
          }
        }
      }
    }
    private TextEditingTool _TextEditingTool;

    public void Focus() {
      TextBox box = this.TextBox;
      if (box != null) box.Focus();
    }

    public event EventHandler GotFocus;

    public String Text {
      get {
        TextBox box = this.TextBox;
        if (box != null) return box.Text;
        return "";
      }
      set {
        TextBox box = this.TextBox;
        if (box != null) box.Text = value;
      }
    }

    public bool IsValidText(String oldstring, String newstring) {
      return true;
    }

    public TextBox TextBox {
      get {
        if (this.TextEditingTool == null) return null;
        if (this.TextEditingTool.EditorAdornment == null) return null;
        return this.TextEditingTool.EditorAdornment.FindDescendant(e => e is TextBox) as TextBox;
      }
    }

    private void RaiseFocus(Object s, EventArgs e) {
      if (this.GotFocus != null) this.GotFocus(s, e);
    }

    private void LostFocus() {
      TextEditingTool tool = this.TextEditingTool;
      if (tool != null) tool.AcceptText(TextEditingReason.LostFocus);
    }

    private void HandleKey(KeyEventArgs e) {
      if (e.Key == Key.Enter) {
        TextBox box = this.TextBox;
        if (box != null && !box.AcceptsReturn) {
          TextEditingTool tool = this.TextEditingTool;
          if (tool != null) tool.AcceptText(TextEditingReason.Enter);
          return;
        }
      } else if (e.Key == Key.Tab) {
        TextBox box = this.TextBox;
        if (box != null



          ) {
          TextEditingTool tool = this.TextEditingTool;
          if (tool != null) tool.AcceptText(TextEditingReason.Tab);
          return;
        }
      }
    }

    private void LayoutBox(TextBox box) {
      if (box == null) return;
      TextEditingTool tool = this.TextEditingTool;
      if (tool == null) return;
      TextBlock block = tool.TextBlock;
      if (block == null) return;
      Adornment edad = tool.EditorAdornment;
      if (edad == null) return;
      Thickness thick = box.BorderThickness;
      Thickness pad = box.Padding;
      if (block.TextWrapping != TextWrapping.NoWrap) {
        double w = MeasureWrappedTextWidth(box.Text, box, block);
        Size es = edad.GetEffectiveSize(block);
        double ww = Math.Max(w, es.Width);  // at least existing width
        box.MaxWidth = ww + thick.Left + thick.Right + pad.Left + pad.Right + 6;  //??? hard-coded sizes
        //Diagram.Debug(Diagram.Str(ds) + Diagram.Str(es) + " ww: " + Diagram.Str(ww) + " maxw: " + Diagram.Str(box.MaxWidth));
      }
      edad.Remeasure();
    }

    private double MeasureWrappedTextWidth(String s, TextBox box, TextBlock block) {
      if (s == null) return 0;
      TextBlock tb = this.MTB;
      if (tb == null) {
        tb = new TextBlock();
        this.MTB = tb;
      }
      // make this TextBlock look like the TextBox
      tb.FontFamily = box.FontFamily;
      tb.FontStretch = box.FontStretch;
      tb.FontStyle = box.FontStyle;
      tb.FontWeight = box.FontWeight;
      tb.TextAlignment = box.TextAlignment;
      //tb.TextDecorations = box.TextDecorations;
      tb.TextWrapping = box.TextWrapping;
      // or like the TextBlock
      tb.MinWidth = block.MinWidth;
      tb.MaxWidth = block.MaxWidth;
      Part.SetTextAspectRatio(tb, Part.GetTextAspectRatio(block));
      // initial text
      tb.Text = s;
      return NodePanel.MeasureWrappedTextWidth(tb, new Size(tb.MaxWidth, tb.MaxHeight));
    }
    private TextBlock MTB { get; set; }
  }

  /// <summary>
  /// This enum specifies the different ways users can start the <see cref="TextEditingTool"/>.
  /// </summary>
  public enum TextEditingStarting {
    /// <summary>
    /// A single click on a <c>TextBlock</c> with <c>go:Part.TextEditable</c> attached property set to true
    /// will start in-place editing.
    /// </summary>
    SingleClick,

    /// <summary>
    /// A single click on a <c>TextBlock</c> with <c>go:Part.TextEditable</c> attached property set to true
    /// will start in-place editing, but only if the <see cref="Part"/> that the <c>TextBlock</c> is in is already selected.
    /// </summary>
    SingleClickSelected,

    //DoubleClick
    //DoubleClickSelected,
  }
}
