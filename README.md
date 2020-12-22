# MvvmChart
MvvMChart is a simple, MvvM support and highly customizable chart control for WPF(and UWP soon).
![](https://img.shields.io/badge/license-MIT-green)![](https://img.shields.io/badge/support-WPF-brightgreen)

## Features
* Very lightweighted;
* Highly MvvM support;
* Currently support line series( PolylineSeries, StepLineSeres, SplineSeries) and area series(PolylineAreaSeries, StepLineArea) with or without item point;
* Highly customizable;
* Currently only suport WPF(UWP support will be added soon).

## Screenshots
![PolyLineSeries, StepLineSeries& SplineSeries without item point](https://github.com/zenjia/MvvmChart/blob/master/Demo/Images/withoutdot2.PNG)
![PolyLineSeries, StepLineSeries& SplineSeries with item point](https://github.com/zenjia/MvvmChart/blob/master/Demo/Images/withdot2.PNG)
![PolyLineAreaSeries without item point](https://github.com/zenjia/MvvmChart/blob/master/Demo/Images/areaWithoutDot.PNG)
![PolyLineAreaSeries with item point](https://github.com/zenjia/MvvmChart/blob/master/Demo/Images/areaWithDot.PNG)

## Quick start
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
