# MvvmChart
MvvmChart is an extremely lightweighted, MVVM support and highly customizable chart control for WPF(including DotNet.Core, and UWP).</br>
[![Gitter](https://badges.gitter.im/MvvmChart/community.svg)](https://gitter.im/MvvmChart/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)
![](https://img.shields.io/badge/license-MIT-green)![](https://img.shields.io/badge/support-WPF-brightgreen)


## Features
* Extremely **lightweighted**(The whole codebase is only 4000 lines);
* Perfect **MVVM** support(probably the most important feature of MvvmChart);
* Suppert DateTime&DateTimeOffset. By default providing 3 line series types(PolylineSeries, StepLineSeres, SplineSeries) and 3 area series types(PolylineAreaSeries, StepLineAreaSeries, SplineAreaSeries), both with or without item points(Scatters);
* Suppert various axis placement(for x-axis: Top&Bottom, for y-axis: Left&Right), GridLine, CrossHair...（No support for Legends currrently）
* Highly customizable: Almost everything can be customized through **Binding** or by dynamically changing **Styles** or **Template**. And one of the most highlighted features: user can completely change the shape of **Series**(or **Scatters**) just by implementing the **ISeriesGeometryBuilder**(or **IScatterGeometryBuilder** for Scatter) and binding them to PathSeries.**GeometryBuilder**(or Scatter.**GeometryBuilder** for Scatter);
* Suport **WPF**(and .NET Core, **UWP** support will be added soon).

## Screenshots
![PolyLineSeries, StepLineSeries& SplineSeries without item point](https://github.com/zenjia/MvvmChart/blob/master/Images/withoutdot2.PNG)
![PolyLineSeries, StepLineSeries& SplineSeries with item point](https://github.com/zenjia/MvvmChart/blob/master/Images/withdot2.PNG)
![PolyLineAreaSeries without item point](https://github.com/zenjia/MvvmChart/blob/master/Images/areaWithoutDot.PNG)
![PolyLineAreaSeries with item point](https://github.com/zenjia/MvvmChart/blob/master/Images/areaWithDot.PNG)

## How to use
### Quick start:
    First define some view models:
```c#
    public class SomePoint
    {
        public double t { get; }
        public double Y { get; }
    }
    
    public class DemoDataViewModel 
    {
        public List<List<SomePoint>> ItemsSourceList { get; }
    }
```
  Then in Resouces define a DataTemplate:
```Xaml
        <DataTemplate x:Key="SeriesTemplate1">
            <mvvmCharting:PolyLineSeries IndependentValueProperty="t"
                                         DependentValueProperty="Y"
                                         Stroke="Red"
                                         StrokeThickness="1.5"
                                         ItemsSource="{Binding}">
            </mvvmCharting:PolyLineSeries>
        </DataTemplate>
 ```
  Finally create a SeriesChart and reference the DataTemplate:
 ```Xaml    
    <mvvmCharting:SeriesChart Background="Bisque"
                              SeriesDataTemplate="{StaticResource SeriesTemplate1}"
                              SeriesItemsSource="{Binding ItemsSourceList, Source={StaticResource GlobalDemoDataViewModel}}">
    </mvvmCharting:SeriesChart>
```
</br>
### Use with DateTime/DateTimeOffset data:
MvvmChart supports DateTime/DateTimeOffset type data. When it sees the type of data is the DateTime/DateTimeOffset, it will automatically convert it to double using **DoubleValueConverter.ObjectToDouble()** method. But when displaying the axis label text, it will be the user's responsibility to write a converter to convert it back and format it to a string. In order to convert the value back correctly, the user can use  **DoubleValueConverter.DoubleToDateTime()**(for DateTime type) or **DoubleValueConverter.DoubleToDateTimeOffset()** (for DateTimeOffset type) method. </br>
For example:</br>

```c#
    public class DoubleToDateTimeStringConverter : IValueConverterNS
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var q = (double) value;

            var t = DoubleValueConverter.DoubleToDateTimeOffset(q);

            return t.ToString("yyyy MMMM dd");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
```
and set the converter to the LabelTextConverter property of Axis:
```xaml
    <mvvmCharting:SeriesChart SeriesDataTemplate="{StaticResource SeriesTemplate}"
                              SeriesItemsSource="{Binding ItemsSourceList, Source={StaticResource TimeSeriesViewModel}}">

        <mvvmCharting:SeriesChart.XAxis>
            <axis:XAxis LabelTextConverter="{StaticResource DoubleToDateTimeStringConverter}"/>
        </mvvmCharting:SeriesChart.XAxis>


    </mvvmCharting:SeriesChart>
```
Here is a screenshort from the DateTimeOffset XAxis demo:

![DateTimeOffset XAxis demo](https://github.com/zenjia/MvvmChart/blob/master/Images/DateTimeDemo.PNG)

### Advance usages:
#### Change the default ScatterTemplate
There are two ways to achieve this:
1. Implementing IScatterGeometryBuilder:
```c#
    public class RectangleBuilder : IScatterGeometryBuilder
    {
        public Geometry GetGeometry()
        {
            return new RectangleGeometry(new Rect(new Size(10,10)));
        }
    }
```
Define your new ScatterTemplate:
```xaml
        <mvvmCharting:RectangleBuilder x:Key="RectangleBuilder"/>

        <DataTemplate x:Key="MyScatterTemplate">
            <mvvmCharting:Scatter GeometryBuilder="{StaticResource RectangleBuilder}"
                                  Fill="Red"/>
        </DataTemplate>
                
```
And reference the new DataTemplate in SeriesBase.ScatterTemplate property:

```xaml
        <DataTemplate x:Key="SeriesTemplate1" DataType="local:SomePointList">
            <mvvmCharting:PolyLineSeries IndependentValueProperty="t"
                                         DependentValueProperty="Y"
                                         Stroke="CadetBlue"
                                         StrokeThickness="1.5"
                                         ItemsSource="{Binding DataList}"
                                         ScatterTemplate="{StaticResource MyScatterTemplate}">
  
            </mvvmCharting:PolyLineSeries>

        </DataTemplate>
```

or</br>
2.Set the Data propery of Scatter to a new Geometry in xaml directly.

```xaml
        <DataTemplate x:Key="SeriesTemplate0" DataType="local:SomePointList">
            <mvvmCharting:PolyLineAreaSeries IndependentValueProperty="t"
                                             DependentValueProperty="Y"
                                             Fill="Blue"
                                             ItemsSource="{Binding DataList}">
                <mvvmCharting:PolyLineAreaSeries.ScatterTemplate>
                    <DataTemplate>
                        <mvvmCharting:Scatter Fill="Red">
                            <RectangleGeometry Rect="0,0,50,50"/>
                        </mvvmCharting:Scatter>
                    </DataTemplate>
                </mvvmCharting:PolyLineAreaSeries.ScatterTemplate>
            </mvvmCharting:PolyLineAreaSeries>

        </DataTemplate>
```

![Scatter customization demo screen shot](https://img2020.cnblogs.com/blog/2238515/202012/2238515-20201223140833298-1559993859.png)

To see more samples, just download the source code, and run the Demo app(more samples will be added continually). Enjoy!

