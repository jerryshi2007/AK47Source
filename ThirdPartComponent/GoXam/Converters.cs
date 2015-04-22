
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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Northwoods.GoXam {

  /// <summary>
  /// This is a base class for all of the converters.
  /// </summary>
  public abstract class Converter : IValueConverter {
    /// <summary>
    /// By default this just throws a <c>NotImplementedException</c>.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public virtual Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }

    /// <summary>
    /// By default this just throws a <c>NotImplementedException</c>.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public virtual Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }
  }


  /// <summary>
  /// Convert the name of a color into a <c>Color</c>.
  /// </summary>
  [ValueConversion(typeof(String), typeof(Color))]
  public class StringColorConverter : Converter {
    /// <summary>
    /// Convert a color name or RGB value into a <c>Color</c>.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public override Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
      if (value == null) return null;
      String str = value.ToString();

      if (str != null) {
        // first try looking up in our table
        Color c;
        if (ConvertToColor(str, out c)) return c;

        // otherwise try using the XAML reader
        String xaml = "<Border xmlns=\"http://schemas.microsoft.com/client/2007\" Background=\"" + str + "\"/>";
        try {
          Border elt = System.Windows.Markup.XamlReader.Load(xaml) as Border;
          if (elt != null) {
            SolidColorBrush scb = elt.Background as SolidColorBrush;
            if (scb != null) return scb.Color;
          }
        } catch (System.Windows.Markup.XamlParseException) {
        }
      }
      return Colors.Transparent;




    }

    /// <summary>
    /// This converts a Color to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public override Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
      Color c = (Color)value;

      return c.ToString();



    }



    internal static bool ConvertToColor(String name, out Color color) {
      color = Colors.Transparent;
      if (String.IsNullOrEmpty(name)) return false;
      if (name[0] == '#') {
        try {
          if (name.Length == 9) {
            byte a = System.Convert.ToByte(name.Substring(1, 2), 16);
            byte r = System.Convert.ToByte(name.Substring(3, 2), 16);
            byte g = System.Convert.ToByte(name.Substring(5, 2), 16);
            byte b = System.Convert.ToByte(name.Substring(7, 2), 16);
            color = Color.FromArgb(a, r, g, b);
            return true;
          } else if (name.Length == 7) {
            byte r = System.Convert.ToByte(name.Substring(1, 2), 16);
            byte g = System.Convert.ToByte(name.Substring(3, 2), 16);
            byte b = System.Convert.ToByte(name.Substring(5, 2), 16);
            color = Color.FromArgb(255, r, g, b);
            return true;
          }
        } catch (Exception) {
          // continue, by looking up name in table
        }
      }
      if (_ColorNames == null) {
        _ColorNames = new Dictionary<String, Color>();
        _ColorNames["ALICEBLUE"] = Color.FromArgb(255, 240, 248, 255);
        _ColorNames["ANTIQUEWHITE"] = Color.FromArgb(255, 250, 235, 215);
        _ColorNames["AQUA"] = Color.FromArgb(255, 0, 255, 255);
        _ColorNames["AQUAMARINE"] = Color.FromArgb(255, 127, 255, 212);
        _ColorNames["AZURE"] = Color.FromArgb(255, 240, 255, 255);
        _ColorNames["BEIGE"] = Color.FromArgb(255, 245, 245, 220);
        _ColorNames["BISQUE"] = Color.FromArgb(255, 255, 228, 196);
        _ColorNames["BLACK"] = Color.FromArgb(255, 0, 0, 0);
        _ColorNames["BLANCHEDALMOND"] = Color.FromArgb(255, 255, 235, 205);
        _ColorNames["BLUE"] = Color.FromArgb(255, 0, 0, 255);
        _ColorNames["BLUEVIOLET"] = Color.FromArgb(255, 138, 43, 226);
        _ColorNames["BROWN"] = Color.FromArgb(255, 165, 42, 42);
        _ColorNames["BURLYWOOD"] = Color.FromArgb(255, 222, 184, 135);
        _ColorNames["CADETBLUE"] = Color.FromArgb(255, 95, 158, 160);
        _ColorNames["CHARTREUSE"] = Color.FromArgb(255, 127, 255, 0);
        _ColorNames["CHOCOLATE"] = Color.FromArgb(255, 210, 105, 30);
        _ColorNames["CORAL"] = Color.FromArgb(255, 255, 127, 80);
        _ColorNames["CORNFLOWERBLUE"] = Color.FromArgb(255, 100, 149, 237);
        _ColorNames["CORNSILK"] = Color.FromArgb(255, 255, 248, 220);
        _ColorNames["CRIMSON"] = Color.FromArgb(255, 220, 20, 60);
        _ColorNames["CYAN"] = Color.FromArgb(255, 0, 255, 255);
        _ColorNames["DARKBLUE"] = Color.FromArgb(255, 0, 0, 139);
        _ColorNames["DARKCYAN"] = Color.FromArgb(255, 0, 139, 139);
        _ColorNames["DARKGOLDENROD"] = Color.FromArgb(255, 184, 134, 11);
        _ColorNames["DARKGRAY"] = Color.FromArgb(255, 169, 169, 169);
        _ColorNames["DARKGREEN"] = Color.FromArgb(255, 0, 100, 0);
        _ColorNames["DARKKHAKI"] = Color.FromArgb(255, 189, 183, 107);
        _ColorNames["DARKMAGENTA"] = Color.FromArgb(255, 139, 0, 139);
        _ColorNames["DARKOLIVEGREEN"] = Color.FromArgb(255, 85, 107, 47);
        _ColorNames["DARKORANGE"] = Color.FromArgb(255, 255, 140, 0);
        _ColorNames["DARKORCHID"] = Color.FromArgb(255, 153, 50, 204);
        _ColorNames["DARKRED"] = Color.FromArgb(255, 139, 0, 0);
        _ColorNames["DARKSALMON"] = Color.FromArgb(255, 233, 150, 122);
        _ColorNames["DARKSEAGREEN"] = Color.FromArgb(255, 143, 188, 143);
        _ColorNames["DARKSLATEBLUE"] = Color.FromArgb(255, 72, 61, 139);
        _ColorNames["DARKSLATEGRAY"] = Color.FromArgb(255, 47, 79, 79);
        _ColorNames["DARKTURQUOISE"] = Color.FromArgb(255, 0, 206, 209);
        _ColorNames["DARKVIOLET"] = Color.FromArgb(255, 148, 0, 211);
        _ColorNames["DEEPPINK"] = Color.FromArgb(255, 255, 20, 147);
        _ColorNames["DEEPSKYBLUE"] = Color.FromArgb(255, 0, 191, 255);
        _ColorNames["DIMGRAY"] = Color.FromArgb(255, 105, 105, 105);
        _ColorNames["DODGERBLUE"] = Color.FromArgb(255, 30, 144, 255);
        _ColorNames["FIREBRICK"] = Color.FromArgb(255, 178, 34, 34);
        _ColorNames["FLORALWHITE"] = Color.FromArgb(255, 255, 250, 240);
        _ColorNames["FORESTGREEN"] = Color.FromArgb(255, 34, 139, 34);
        _ColorNames["FUCHSIA"] = Color.FromArgb(255, 255, 0, 255);
        _ColorNames["GAINSBORO"] = Color.FromArgb(255, 220, 220, 220);
        _ColorNames["GHOSTWHITE"] = Color.FromArgb(255, 248, 248, 255);
        _ColorNames["GOLD"] = Color.FromArgb(255, 255, 215, 0);
        _ColorNames["GOLDENROD"] = Color.FromArgb(255, 218, 165, 32);
        _ColorNames["GRAY"] = Color.FromArgb(255, 128, 128, 128);
        _ColorNames["GREEN"] = Color.FromArgb(255, 0, 128, 0);
        _ColorNames["GREENYELLOW"] = Color.FromArgb(255, 173, 255, 47);
        _ColorNames["HONEYDEW"] = Color.FromArgb(255, 240, 255, 240);
        _ColorNames["HOTPINK"] = Color.FromArgb(255, 255, 105, 180);
        _ColorNames["INDIANRED"] = Color.FromArgb(255, 205, 92, 92);
        _ColorNames["INDIGO"] = Color.FromArgb(255, 75, 0, 130);
        _ColorNames["IVORY"] = Color.FromArgb(255, 255, 255, 240);
        _ColorNames["KHAKI"] = Color.FromArgb(255, 240, 230, 140);
        _ColorNames["LAVENDER"] = Color.FromArgb(255, 230, 230, 250);
        _ColorNames["LAVENDERBLUSH"] = Color.FromArgb(255, 255, 240, 245);
        _ColorNames["LAWNGREEN"] = Color.FromArgb(255, 124, 252, 0);
        _ColorNames["LEMONCHIFFON"] = Color.FromArgb(255, 255, 250, 205);
        _ColorNames["LIGHTBLUE"] = Color.FromArgb(255, 173, 216, 230);
        _ColorNames["LIGHTCORAL"] = Color.FromArgb(255, 240, 128, 128);
        _ColorNames["LIGHTCYAN"] = Color.FromArgb(255, 224, 255, 255);
        _ColorNames["LIGHTGOLDENRODYELLOW"] = Color.FromArgb(255, 250, 250, 210);
        _ColorNames["LIGHTGRAY"] = Color.FromArgb(255, 211, 211, 211);
        _ColorNames["LIGHTGREEN"] = Color.FromArgb(255, 144, 238, 144);
        _ColorNames["LIGHTPINK"] = Color.FromArgb(255, 255, 182, 193);
        _ColorNames["LIGHTSALMON"] = Color.FromArgb(255, 255, 160, 122);
        _ColorNames["LIGHTSEAGREEN"] = Color.FromArgb(255, 32, 178, 170);
        _ColorNames["LIGHTSKYBLUE"] = Color.FromArgb(255, 135, 206, 250);
        _ColorNames["LIGHTSLATEGRAY"] = Color.FromArgb(255, 119, 136, 153);
        _ColorNames["LIGHTSTEELBLUE"] = Color.FromArgb(255, 176, 196, 222);
        _ColorNames["LIGHTYELLOW"] = Color.FromArgb(255, 255, 255, 224);
        _ColorNames["LIME"] = Color.FromArgb(255, 0, 255, 0);
        _ColorNames["LIMEGREEN"] = Color.FromArgb(255, 50, 205, 50);
        _ColorNames["LINEN"] = Color.FromArgb(255, 250, 240, 230);
        _ColorNames["MAGENTA"] = Color.FromArgb(255, 255, 0, 255);
        _ColorNames["MAROON"] = Color.FromArgb(255, 128, 0, 0);
        _ColorNames["MEDIUMAQUAMARINE"] = Color.FromArgb(255, 102, 205, 170);
        _ColorNames["MEDIUMBLUE"] = Color.FromArgb(255, 0, 0, 205);
        _ColorNames["MEDIUMORCHID"] = Color.FromArgb(255, 186, 85, 211);
        _ColorNames["MEDIUMPURPLE"] = Color.FromArgb(255, 147, 112, 219);
        _ColorNames["MEDIUMSEAGREEN"] = Color.FromArgb(255, 60, 179, 113);
        _ColorNames["MEDIUMSLATEBLUE"] = Color.FromArgb(255, 123, 104, 238);
        _ColorNames["MEDIUMSPRINGGREEN"] = Color.FromArgb(255, 0, 250, 154);
        _ColorNames["MEDIUMTURQUOISE"] = Color.FromArgb(255, 72, 209, 204);
        _ColorNames["MEDIUMVIOLETRED"] = Color.FromArgb(255, 199, 21, 133);
        _ColorNames["MIDNIGHTBLUE"] = Color.FromArgb(255, 25, 25, 112);
        _ColorNames["MINTCREAM"] = Color.FromArgb(255, 245, 255, 250);
        _ColorNames["MISTYROSE"] = Color.FromArgb(255, 255, 228, 225);
        _ColorNames["MOCCASIN"] = Color.FromArgb(255, 255, 228, 181);
        _ColorNames["NAVAJOWHITE"] = Color.FromArgb(255, 255, 222, 173);
        _ColorNames["NAVY"] = Color.FromArgb(255, 0, 0, 128);
        _ColorNames["OLDLACE"] = Color.FromArgb(255, 253, 245, 230);
        _ColorNames["OLIVE"] = Color.FromArgb(255, 128, 128, 0);
        _ColorNames["OLIVEDRAB"] = Color.FromArgb(255, 107, 142, 35);
        _ColorNames["ORANGE"] = Color.FromArgb(255, 255, 165, 0);
        _ColorNames["ORANGERED"] = Color.FromArgb(255, 255, 69, 0);
        _ColorNames["ORCHID"] = Color.FromArgb(255, 218, 112, 214);
        _ColorNames["PALEGOLDENROD"] = Color.FromArgb(255, 238, 232, 170);
        _ColorNames["PALEGREEN"] = Color.FromArgb(255, 152, 251, 152);
        _ColorNames["PALETURQUOISE"] = Color.FromArgb(255, 175, 238, 238);
        _ColorNames["PALEVIOLETRED"] = Color.FromArgb(255, 219, 112, 147);
        _ColorNames["PAPAYAWHIP"] = Color.FromArgb(255, 255, 239, 213);
        _ColorNames["PEACHPUFF"] = Color.FromArgb(255, 255, 218, 185);
        _ColorNames["PERU"] = Color.FromArgb(255, 205, 133, 63);
        _ColorNames["PINK"] = Color.FromArgb(255, 255, 192, 203);
        _ColorNames["PLUM"] = Color.FromArgb(255, 221, 160, 221);
        _ColorNames["POWDERBLUE"] = Color.FromArgb(255, 176, 224, 230);
        _ColorNames["PURPLE"] = Color.FromArgb(255, 128, 0, 128);
        _ColorNames["RED"] = Color.FromArgb(255, 255, 0, 0);
        _ColorNames["ROSYBROWN"] = Color.FromArgb(255, 188, 143, 143);
        _ColorNames["ROYALBLUE"] = Color.FromArgb(255, 65, 105, 225);
        _ColorNames["SADDLEBROWN"] = Color.FromArgb(255, 139, 69, 19);
        _ColorNames["SALMON"] = Color.FromArgb(255, 250, 128, 114);
        _ColorNames["SANDYBROWN"] = Color.FromArgb(255, 244, 164, 96);
        _ColorNames["SEAGREEN"] = Color.FromArgb(255, 46, 139, 87);
        _ColorNames["SEASHELL"] = Color.FromArgb(255, 255, 245, 238);
        _ColorNames["SIENNA"] = Color.FromArgb(255, 160, 82, 45);
        _ColorNames["SILVER"] = Color.FromArgb(255, 192, 192, 192);
        _ColorNames["SKYBLUE"] = Color.FromArgb(255, 135, 206, 235);
        _ColorNames["SLATEBLUE"] = Color.FromArgb(255, 106, 90, 205);
        _ColorNames["SLATEGRAY"] = Color.FromArgb(255, 112, 128, 144);
        _ColorNames["SNOW"] = Color.FromArgb(255, 255, 250, 250);
        _ColorNames["SPRINGGREEN"] = Color.FromArgb(255, 0, 255, 127);
        _ColorNames["STEELBLUE"] = Color.FromArgb(255, 70, 130, 180);
        _ColorNames["TAN"] = Color.FromArgb(255, 210, 180, 140);
        _ColorNames["TEAL"] = Color.FromArgb(255, 0, 128, 128);
        _ColorNames["THISTLE"] = Color.FromArgb(255, 216, 191, 216);
        _ColorNames["TOMATO"] = Color.FromArgb(255, 255, 99, 71);
        _ColorNames["TRANSPARENT"] = Color.FromArgb(0, 255, 255, 255);
        _ColorNames["TURQUOISE"] = Color.FromArgb(255, 64, 224, 208);
        _ColorNames["VIOLET"] = Color.FromArgb(255, 238, 130, 238);
        _ColorNames["WHEAT"] = Color.FromArgb(255, 245, 222, 179);
        _ColorNames["WHITE"] = Color.FromArgb(255, 255, 255, 255);
        _ColorNames["WHITESMOKE"] = Color.FromArgb(255, 245, 245, 245);
        _ColorNames["YELLOW"] = Color.FromArgb(255, 255, 255, 0);
        _ColorNames["YELLOWGREEN"] = Color.FromArgb(255, 154, 205, 50);
      }
      if (_ColorNames.TryGetValue(name.ToUpper(System.Globalization.CultureInfo.InvariantCulture), out color)) {
        return true;
      }
      return false;
    }

    private static Dictionary<String, Color> _ColorNames;



  }


  /// <summary>
  /// Convert the name of a color into a <c>Brush</c>.
  /// </summary>
  [ValueConversion(typeof(String), typeof(Brush))]
  public class StringBrushConverter : Converter {
    /// <summary>
    /// Convert a color name or RGB value into a <c>SolidColorBrush</c>.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public override Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
      if (value == null) return null;
      String str = value.ToString();

      if (str != null) {
        // first try looking up in our table
        Color c;
        if (StringColorConverter.ConvertToColor(str, out c)) {
          return new SolidColorBrush(c);
        }

        //?? otherwise try using the XAML reader
        //String xaml = "<Border xmlns=\"http://schemas.microsoft.com/client/2007\" Background=\"" + str + "\"/>";
        //try {
        //  Border elt = System.Windows.Markup.XamlReader.Load(xaml) as Border;
        //  if (elt != null) return elt.Background;
        //} catch (System.Windows.Markup.XamlParseException) {
        //}
      }
      return new SolidColorBrush(Colors.Transparent);




    }

    /// <summary>
    /// This works to retrieve the color as a string from a <c>SolidColorBrush</c>.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public override Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
      Brush b = (Brush)value;

      SolidColorBrush scb = b as SolidColorBrush;
      if (scb != null) scb.Color.ToString();
      return "Transparent";



    }




  }


  /// <summary>
  /// Convert a positive integer to the value <c>Visibility.Visible</c>;
  /// zero or a negative value converts to <c>Visibility.Collapsed</c>.
  /// </summary>
  /// <remarks>
  /// This can be useful when you want the something to be visible only
  /// when some value, such as the Count of a list, is positive.
  /// </remarks>
  [ValueConversion(typeof(int), typeof(Visibility))]
  public class CountVisibilityConverter : Converter {
    /// <summary>
    /// This returns <c>Visibility.Visible</c> when the value is positive,
    /// or <c>Visibility.Collapsed</c> otherwise.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
      if (value == null) return Visibility.Collapsed;
      int i = (int)value;
      if (this.Inverted)
        return (i > 0) ? Visibility.Collapsed : Visibility.Visible;
      else
        return (i > 0) ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    /// Gets or sets whether a positive value should convert to
    /// <c>Visibility.Collapsed</c> instead of <c>Visibility.Visible</c>.
    /// </summary>
    /// <value>
    /// The default value is false, e.g. a zero value converts to <c>Collapsed</c>.
    /// </value>
    public bool Inverted { get; set; }
  }


  /// <summary>
  /// Convert a boolean to one of two <c>Brush</c>es.
  /// </summary>
  /// <remarks>
  /// <para>
  /// You can specify a brush for when the boolean is True and
  /// another one for when the boolean is False,
  /// either by setting the <see cref="TrueBrush"/> or <see cref="FalseBrush"/> properties,
  /// or by setting the <see cref="TrueColor"/> or <see cref="FalseColor"/> properties
  /// which will also set the corresponding brush property to a <c>SolidColorBrush</c> of that color.
  /// </para>
  /// <para>
  /// Typically you will create an instance (or two) of this converter class as resources.
  /// For example:
  /// <code>
  /// &lt;go:BooleanBrushConverter x:Key="theBooleanBrushConverter" TrueColor="DodgerBlue"&gt;
  ///   &lt;go:BooleanBrushConverter.FalseBrush&gt;
  ///     &lt;LinearGradientBrush . . .&gt;
  ///        . . .
  ///     &lt;/LinearGradientBrush&gt;
  ///   &lt;/go:BooleanBrushConverter.FalseBrush&gt;
  /// &lt;/go:BooleanBrushConverter&gt;
  /// </code>
  /// </para>
  /// <para>
  /// One possible use for this converter is to indicate selection:
  /// <code>
  /// &lt;DataTemplate x:Key="NodeTemplate2"&gt;
  ///   &lt;Border BorderBrush="Gray" BorderThickness="1"
  ///           Background="{Binding Path=Part.IsSelected, Converter={StaticResource theBooleanBrushConverter}}"
  ///           go:Node.Location="{Binding Path=Data.Location, Mode=TwoWay}"&gt;
  ///     &lt;StackPanel Orientation="Vertical"&gt;
  ///       &lt;Ellipse Fill="Blue" Width="20" Height="20" HorizontalAlignment="Center"
  ///                go:Node.LinkableFrom="True" go:Node.LinkableTo="True" Cursor="Arrow"
  ///                go:Node.FromSpot="AllSides" go:Node.ToSpot="AllSides"/&gt;
  ///       &lt;TextBlock x:Name="Text" Text="{Binding Path=Data.Key}" HorizontalAlignment="Center" /&gt;
  ///     &lt;/StackPanel&gt;
  ///   &lt;/Border&gt;
  /// &lt;/DataTemplate&gt;
  /// </code>
  /// When the node is not selected, the background is a linear gradient brush.
  /// When the node is selected, the background is a solid DodgerBlue.
  /// </para>
  /// <para>
  /// By default true converts to the system highlight color/brush (typically bluish)
  /// and false converts to the system highlight text color/brush (typically white).
  /// </para>
  /// </remarks>
  [ValueConversion(typeof(bool), typeof(Brush))]
  public class BooleanBrushConverter : Converter {
    /// <summary>
    /// Create a boolean to Brush converter using the system highlight color
    /// as the <see cref="TrueColor"/> and the system highlight text color
    /// as the <see cref="FalseColor"/>.
    /// </summary>
    public BooleanBrushConverter() {

      this.TrueColor = SystemColors.HighlightColor;
      this.FalseColor = SystemColors.HighlightTextColor;




    }

#pragma warning disable 1591
    [EditorBrowsableAttribute(EditorBrowsableState.Never)]
    public String Init {
      get { return ""; }
      set {
        switch (value) {
          case "SelectedTextForeground":
            this.TrueColor = SystemColors.HighlightTextColor;
            this.FalseColor = SystemColors.WindowTextColor;
            break;
          case "SelectedTextBackground":
            this.TrueColor = SystemColors.HighlightColor;
            this.FalseColor = Colors.Transparent;
            break;
          case "SelectedLineBrush":
            this.TrueColor = SystemColors.HighlightColor;
            this.FalseColor = SystemColors.WindowTextColor;
            break;
          case "SelectedShapeBrush":
            this.TrueColor = SystemColors.HighlightColor;
            this.FalseColor = SystemColors.HighlightTextColor;
            break;
        }
      }
    }


    /// <summary>
    /// This returns <see cref="FalseBrush"/> when the <paramref name="value"/> is false;
    /// otherwise this returns <see cref="TrueBrush"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
      if (value == null || (bool)value == false)
        return _FalseBrush;
      else
        return _TrueBrush;
    }

    /// <summary>
    /// Gets or sets the color of <see cref="TrueBrush"/> when it is a <c>SolidColorBrush</c>.
    /// </summary>
    /// <value>
    /// Changing this property will also modify the <see cref="TrueBrush"/> property.
    /// </value>
    /// <remarks>
    /// This is not a dependency property, so one cannot bind this property value.
    /// </remarks>
    public Color TrueColor {
      get { return _TrueColor; }
      set {
        _TrueColor = value;
        _TrueBrush = new SolidColorBrush(_TrueColor);
      }
    }
    private Color _TrueColor;

    /// <summary>
    /// Gets or sets the brush to be returned by the conversion when the input value is true.
    /// </summary>
    /// <value>
    /// Changing this property will also modify the <see cref="TrueColor"/> property
    /// if the new brush is a <c>SolidColorBrush</c>.
    /// </value>
    /// <remarks>
    /// This is not a dependency property, so one cannot bind this property value.
    /// </remarks>
    public Brush TrueBrush {
      get { return _TrueBrush; }
      set {
        _TrueBrush = value;
        SolidColorBrush b = value as SolidColorBrush;
        if (b != null) _TrueColor = b.Color;
      }
    }
    private Brush _TrueBrush;

    /// <summary>
    /// Gets or sets the color of <see cref="FalseBrush"/> when it is a <c>SolidColorBrush</c>.
    /// </summary>
    /// <value>
    /// Changing this property will also modify the <see cref="FalseBrush"/> property.
    /// </value>
    /// <remarks>
    /// This is not a dependency property, so one cannot bind this property value.
    /// </remarks>
    public Color FalseColor {
      get { return _FalseColor; }
      set {
        _FalseColor = value;
        _FalseBrush = new SolidColorBrush(_FalseColor);
      }
    }
    private Color _FalseColor;

    /// <summary>
    /// Gets or sets the brush to be returned by the conversion when the input value is false.
    /// </summary>
    /// <value>
    /// Changing this property will also modify the <see cref="FalseColor"/> property
    /// if the new brush is a <c>SolidColorBrush</c>.
    /// </value>
    /// <remarks>
    /// This is not a dependency property, so one cannot bind this property value.
    /// </remarks>
    public Brush FalseBrush {
      get { return _FalseBrush; }
      set {
        _FalseBrush = value;
        SolidColorBrush b = value as SolidColorBrush;
        if (b != null) _FalseColor = b.Color;
      }
    }
    private Brush _FalseBrush;
  }


  /// <summary>
  /// Convert a boolean to one of two strings.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Typically you will create an instance of this as a resource.
  /// For example, one possible use of this converter:
  /// <code>
  /// &lt;go:BooleanStringConverter x:Key="theBooleanLayerConverter" TrueString="Foreground" FalseString="" /&gt;
  /// </code>
  /// <code>
  /// &lt;DataTemplate&gt;
  ///   &lt;Border . . .
  ///           go:Part.LayerName="{Binding Path=Part.IsSelected, Converter={StaticResource theBooleanLayerConverter}}"&gt;
  ///     . . .
  ///   &lt;/Border&gt;
  /// &lt;/DataTemplate&gt;
  /// </code>
  /// When the node is selected, the node is moved to the <see cref="NodeLayer"/> named "Foreground".
  /// When the node is no longer selected, the node is moved to the default layer, named with the empty string.
  /// </para>
  /// </remarks>
  [ValueConversion(typeof(bool), typeof(String))]
  public class BooleanStringConverter : Converter {
    /// <summary>
    /// This returns <see cref="FalseString"/> when the <paramref name="value"/> is false;
    /// otherwise this returns <see cref="TrueString"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (value == null || (bool)value == false)
        return this.FalseString;
      else
        return this.TrueString;
    }

    /// <summary>
    /// Gets or sets the string that is returned by the conversion when the value is true.
    /// </summary>
    /// <remarks>
    /// This is not a dependency property, so one cannot bind this property value.
    /// </remarks>
    public String TrueString { get; set; }

    /// <summary>
    /// Gets or sets the string that is returned by the conversion when the value is false.
    /// </summary>
    /// <remarks>
    /// This is not a dependency property, so one cannot bind this property value.
    /// </remarks>
    public String FalseString { get; set; }
  }


  /// <summary>
  /// Convert a boolean to one of two <c>Thickness</c> values.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Typically you will create an instance of this as a resource.
  /// For example, one possible use of this converter:
  /// <code>
  /// &lt;go:BooleanThicknessConverter x:Key="theBooleanThicknessConverter" TrueThickness="3" FalseThickness="2" /&gt;
  /// </code>
  /// <code>
  /// &lt;DataTemplate&gt;
  ///   &lt;Border . . .
  ///       BorderThickness="{Binding Path=Part.IsSelected, Converter={StaticResource theBooleanThicknessConverter}}"&gt;
  ///     . . .
  ///   &lt;/Border&gt;
  /// &lt;/DataTemplate&gt;
  /// </code>
  /// When the node is selected, the node's border is thicker than normal.
  /// </para>
  /// </remarks>
  [ValueConversion(typeof(bool), typeof(Thickness))]
  public class BooleanThicknessConverter : Converter {
    /// <summary>
    /// This returns the <see cref="FalseThickness"/> value when the <paramref name="value"/> is false;
    /// otherwise this returns the value of <see cref="TrueThickness"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (targetType == typeof(Double)) {
        if (value == null || (bool)value == false)
          return this.FalseThickness.Left;
        else
          return this.TrueThickness.Left;
      } else {
        if (value == null || (bool)value == false)
          return this.FalseThickness;
        else
          return this.TrueThickness;
      }
    }

    /// <summary>
    /// Gets or sets the double-precision floating point value that is returned by the conversion when the value is true.
    /// </summary>
    /// <remarks>
    /// This is not a dependency property, so one cannot bind this property value.
    /// </remarks>
    public Thickness TrueThickness { get; set; }

    /// <summary>
    /// Gets or sets the double-precision floating point value that is returned by the conversion when the value is false.
    /// </summary>
    /// <remarks>
    /// This is not a dependency property, so one cannot bind this property value.
    /// </remarks>
    public Thickness FalseThickness { get; set; }
  }

  /// <summary>
  /// Convert a reference value to false if the value is null or to true otherwise.
  /// When <see cref="Inverted"/> is true the sense is reversed.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Typically you will create an instance of this as a resource.
  /// For example, one possible use of this converter:
  /// <code>
  /// &lt;go:NullBooleanConverter x:Key="theNullBooleanConverter" /&gt;
  /// </code>
  /// <code>
  ///   &lt;ComboBox IsEnabled="{Binding Path=SelectedNode, ElementName=myDiagram, Converter={StaticResource theNullBooleanConverter}}"&gt;
  /// </code>
  /// When a node is selected, the <c>ComboBox</c> is enabled.
  /// </para>
  /// </remarks>
  public class NullBooleanConverter : Converter {
    /// <summary>
    /// This returns false or true depending on whether the value is null or not
    /// and on the value of <see cref="Inverted"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (this.Inverted)
        return (value == null);
      else
        return (value != null);
    }

    /// <summary>
    /// Gets or sets whether a null value should convert to true or false.
    /// </summary>
    /// <value>
    /// The default value is false, which converts a null value to false.
    /// </value>
    public bool Inverted { get; set; }
  }
}
