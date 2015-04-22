
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("GoSilverlight: GoXam for Silverlight")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("GoXam for Silverlight")]
[assembly: AssemblyCopyright("Copyright © Northwoods Software 1998-2010")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]











[assembly:System.CLSCompliant(true)]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]













[assembly: AssemblyVersion("1.1.9.4")]      //??? also update Diagram.VersionName and
[assembly: AssemblyFileVersion("1.1.9.4")]  //???   kit\LicenseManager\AssemblyInfo.cs









//?? this only works in WPF, but maybe it will work in Silverlight some day...
[assembly: XmlnsDefinition("http://schemas.nwoods.com/GoXam", "Northwoods.GoXam")]
[assembly: XmlnsDefinition("http://schemas.nwoods.com/GoXam", "Northwoods.GoXam.Model")]
[assembly: XmlnsDefinition("http://schemas.nwoods.com/GoXam", "Northwoods.GoXam.Tool")]
[assembly: XmlnsDefinition("http://schemas.nwoods.com/GoXam", "Northwoods.GoXam.Layout")]
[assembly:     XmlnsPrefix("http://schemas.nwoods.com/GoXam", "go")]


[assembly: XmlnsDefinition("clr-namespace:Northwoods.GoXam;assembly=Northwoods.GoSilverlight", "Northwoods.GoXam")]
[assembly:     XmlnsPrefix("clr-namespace:Northwoods.GoXam;assembly=Northwoods.GoSilverlight", "go")]
[assembly: XmlnsDefinition("clr-namespace:Northwoods.GoXam.Model;assembly=Northwoods.GoSilverlight", "Northwoods.GoXam.Model")]
[assembly:     XmlnsPrefix("clr-namespace:Northwoods.GoXam.Model;assembly=Northwoods.GoSilverlight", "gomodel")]
[assembly: XmlnsDefinition("clr-namespace:Northwoods.GoXam.Layout;assembly=Northwoods.GoSilverlight", "Northwoods.GoXam.Layout")]
[assembly:     XmlnsPrefix("clr-namespace:Northwoods.GoXam.Layout;assembly=Northwoods.GoSilverlight", "golayout")]
[assembly: XmlnsDefinition("clr-namespace:Northwoods.GoXam.Tool;assembly=Northwoods.GoSilverlight", "Northwoods.GoXam.Tool")]
[assembly:     XmlnsPrefix("clr-namespace:Northwoods.GoXam.Tool;assembly=Northwoods.GoSilverlight", "gotool")]

