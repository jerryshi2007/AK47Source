
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
using System.Windows;

namespace Northwoods.GoXam {

  /// <summary>
  /// This dictionary holds a number of <c>DataTemplates</c> indexed by category names.
  /// </summary>
  /// <remarks>
  /// <para>
  /// There are three standard template collections:
  /// <see cref="Diagram.NodeTemplateDictionary"/>,
  /// <see cref="Diagram.GroupTemplateDictionary"/>, and
  /// <see cref="Diagram.LinkTemplateDictionary"/>.
  /// You can swap whole sets of data templates at once by setting those three properties.
  /// </para>
  /// <para>
  /// However, the <see cref="Diagram"/> properties:
  /// <see cref="Diagram.NodeTemplate"/>,
  /// <see cref="Diagram.GroupTemplate"/>, and
  /// <see cref="Diagram.LinkTemplate"/>
  /// take precedence over the <see cref="Default"/> value in this dictionary.
  /// </para>
  /// <para>
  /// Caution: if you create a <c>DataTemplateDictionary</c> in XAML as the value of a
  /// Style Setter, that dictionary will be shared by all diagrams affected by that style.
  /// The same is true if you create the <c>DataTemplateDictionary</c> as a resource
  /// and refer to it in more than one diagram.
  /// </para>
  /// <para>
  /// An instance of this dictionary may be shared by multiple <see cref="Diagram"/>s.
  /// However, if you modify this dictionary, no <see cref="Diagram"/> will be notified
  /// of such a change until you explicitly call <see cref="Diagram.RaiseTemplatesChanged"/>.
  /// </para>
  /// </remarks>



  public class DataTemplateDictionary : Dictionary<String, DataTemplate> {
    /// <summary>
    /// Create an empty dictionary.
    /// </summary>
    public DataTemplateDictionary() { }










    /// <summary>
    /// Gets or sets the default data template, whose name is an empty string, "".
    /// </summary>
    /// <remarks>
    /// Depending on the dictionary, this value corresponds to either:
    /// <see cref="Diagram.NodeTemplate"/>,
    /// <see cref="Diagram.GroupTemplate"/>, or
    /// <see cref="Diagram.LinkTemplate"/>.
    /// However the value of those properties takes precedence over
    /// the dictionary's value of this Default property.
    /// </remarks>
    public DataTemplate Default {
      get {
        DataTemplate t;
        TryGetValue("", out t);
        return t;
      }
      set {
        this[""] = value;
      }
    }

    /// <summary>
    /// Gets or sets the data template named "Comment".
    /// </summary>
    public DataTemplate Comment {
      get {
        DataTemplate t;
        TryGetValue("Comment", out t);
        return t;
      }
      set {
        this["Comment"] = value;
      }
    }

    /// <summary>
    /// Gets or sets the data template named "LinkLabel".
    /// </summary>
    /// <remarks>
    /// This data template is used for nodes that are link labels,
    /// when the model supports separate link data.
    /// </remarks>
    public DataTemplate LinkLabel {
      get {
        DataTemplate t;
        TryGetValue("LinkLabel", out t);
        return t;
      }
      set {
        this["LinkLabel"] = value;
      }
    }
  }

}
