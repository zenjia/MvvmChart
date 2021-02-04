using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

[assembly: ComVisible(false)]
[assembly:ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                             //(used if a resource is not found in the page,
                             // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                      //(used if a resource is not found in the page,
                                      // app, or any theme specific resource dictionaries)
)]

[assembly: XmlnsDefinition("http://schemas.mvvmchart.com/2020", "MvvmCharting.WpfFX", AssemblyName = "MvvmChartWpfFX")]
[assembly: XmlnsDefinition("http://schemas.mvvmchart.com/2020", "MvvmCharting.WpfFX.Series", AssemblyName = "MvvmChartWpfFX")]
[assembly: XmlnsDefinition("http://schemas.mvvmchart.com/2020", "MvvmCharting.WpfFX.Axis", AssemblyName = "MvvmChartWpfFX")]

[assembly: XmlnsPrefix("http://schemas.mvvmchart.com/2020", "charting")]
[assembly: NeutralResourcesLanguage("en-US")]

