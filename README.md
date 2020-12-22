# MvvmChart
MvvmChart is a simple, MVVM support and highly customizable chart control for WPF(and UWP soon).</br>
![](https://img.shields.io/badge/license-MIT-green)![](https://img.shields.io/badge/support-WPF-brightgreen)

## Features
* Very **lightweighted**;
* Highly **MVVM** support(probably the most important feature of MvvmChart);
* Currently support line series(PolylineSeries, StepLineSeres, SplineSeries) and area series(PolylineAreaSeries, StepLineArea) with or without item points;
* Highly customizable;
* Currently only suport **WPF**(**UWP** support will be added soon).

## Screenshots
![PolyLineSeries, StepLineSeries& SplineSeries without item point](https://github.com/zenjia/MvvmChart/blob/master/Demo/Images/withoutdot2.PNG)
![PolyLineSeries, StepLineSeries& SplineSeries with item point](https://github.com/zenjia/MvvmChart/blob/master/Demo/Images/withdot2.PNG)
![PolyLineAreaSeries without item point](https://github.com/zenjia/MvvmChart/blob/master/Demo/Images/areaWithoutDot.PNG)
![PolyLineAreaSeries with item point](https://github.com/zenjia/MvvmChart/blob/master/Demo/Images/areaWithDot.PNG)

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

        <mvvmCharting:SeriesChart.XAxis>
            <axis:XAxis />
        </mvvmCharting:SeriesChart.XAxis>

        <mvvmCharting:SeriesChart.YAxis>
            <axis:YAxis />
        </mvvmCharting:SeriesChart.YAxis>

    </mvvmCharting:SeriesChart>
```
### Advance usages:
MvvmChart support lots of advanced feature, such as:
* **SeriesDataTemplateSelector** support;
* Series style is highly customizable; 
* **ItemPoint** Style customization support;
* Axis: label text **ValueConverter** & explicit axis ticks;
* Besides the several default series types, users can create almost any series thay want just by implementing **IGeometryBuilder** interface and pass it to the **GeometryBuilder** property of **PathSeries**.

To see the samples, please run the **demo** app(more samples will be added soon). 
