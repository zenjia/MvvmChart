# MvvmChart
MvvmChart is an extremely lightweight, MVVM support and highly customizable chart control for WPF(including DotNet.Core).</br>
[![Gitter](https://badges.gitter.im/MvvmChart/community.svg)](https://gitter.im/MvvmChart/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)
![](https://img.shields.io/badge/license-MIT-green)![](https://img.shields.io/badge/support-WPF-brightgreen)


## Features
* Extremely **lightweighted**: The whole codebase is just about 6000 lines;
* Perfect **MVVM** support. Series, LegendItem, Scatter, BarItem, all can be created with **DataTemplate** through data binding. 
* Support four types of series: **LineSeries**, **AreaSeries**, **ScatterSeries** and **BarSerie**s. The four type of series in a **SeriesControl** can be overlapped or removed dynamically. 
* And all four series types support 3 stack mode: **Normal**(None), **Stacked**, **Stacked 100%**;
* LineSeries and AreaSeries support three types of geometry: **Polyline**, **Stepline** and **Spinlin**e. And the geometry of a LineSeries/AreaSeries can be changed throught data binding easily.
* support **DateTime**&**DateTimeOffset** data and **category** data.
* Axis support various axis placement(for x-axis: Top&Bottom, for y-axis: Left&Right). Support **GridLine**, **CrossHair**, **Legend**, **BackgroundElement**...
* Highly customizable: Almost everything can be customized through **Binding** or by dynamically changing **Styles** or **Template**. And one of the most highlighted features: user can completely change the shape of **Series** just by implementing the **ISeriesGeometryBuilder**and binding them to PathSeries.**GeometryBuilder**;
* Good performance: User will not lose too much performance for true MVVM support and great customizability.



## Quick start:
   1. Create a new WPF app.
   2. Install from NuGet: **Install-Package MvvmChartWpfFX** 
   3. Define the view models:
```c#
    public class SomePoint
    {
        public double t { get; }
        public double Y { get; }
    }
    
    public class SomePointList: BindableBase
    {

        public int Index { get;  }

        public ObservableCollection<SomePoint> DataList { get; }

        public SomePointList(int index)
        {
            this.Index = index;
            this.DataList = new ObservableCollection<SomePoint>();

        }
    }
    
    public class DemoDataViewModel 
    {
        public List<List<SomePoint>> ItemsSourceList { get; }
    }
```
  4. Define a DataTemplate:
```Xaml
        <DataTemplate x:Key="MySeriesTemplate" DataType="local:SomePointList">
            <chart:SeriesControl IndependentValueProperty="t"
                                         DependentValueProperty="Y"
                                         ItemsSource="{Binding DataList}"
                                         >
                <chart:SeriesControl.LineSeries>
                    <chart:PolyLineSeries Stroke="{Binding Index, Converter={StaticResource IndexToStrokeConverter}}"
                                           >
                    </chart:PolyLineSeries>

                </chart:SeriesControl.LineSeries>

                <chart:SeriesControl.AreaSeries>
                    <chart:PolyLineAreaSeries Stroke="{Binding Index, Converter={StaticResource IndexToAreaSeriesFillConverter}}">
                    </chart:PolyLineAreaSeries>

                </chart:SeriesControl.AreaSeries>

                <chart:SeriesControl.ScatterSeries>
                    <chart:ScatterSeries>
                    </chart:ScatterSeries>
                </chart:SeriesControl.ScatterSeries>
            </chart:SeriesControl>
        </DataTemplate>
 ```
  5. Finally create a Chart and reference the DataTemplate:
 ```Xaml    
        <chart:Chart Background="Bisque"
                            SeriesTemplate="{StaticResource MySeriesTemplate}"
                            SeriesItemsSource="{Binding ItemsSourceList, Source={StaticResource DemoDataViewModel}}">
            <chart:Chart.XAxis>
                <chart:NumericAxis Title="t values"/>
            </chart:Chart.XAxis>
            <chart:Chart.YAxis>
                <chart:NumericAxis Title="Y values"/>
            </chart:Chart.YAxis>
        </chart:Chart>
```
Then everything should be ready.
</br>

![LineSeries+ScatterSeries](https://github.com/zenjia/MvvmChart/blob/master/Images/Line%2BScatter.PNG)
![LineSeries+ScatterSeries+AreaSeries](https://github.com/zenjia/MvvmChart/blob/master/Images/Line%2BScatter%2BArea.PNG)
![BarSeries](https://github.com/zenjia/MvvmChart/blob/master/Images/Line%2BScatter.PNG)

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
    <chart:Chart SeriesDataTemplate="{StaticResource SeriesTemplate}"
                 SeriesItemsSource="{Binding ItemsSourceList, Source={StaticResource TimeSeriesViewModel}}">

        <chart:SeriesChart.XAxis>
            <chart:NumericAxis LabelTextConverter="{StaticResource DoubleToDateTimeStringConverter}"/>
        </chart:SeriesChart.XAxis>


    </chart:SeriesChart>
```
Here is a screenshort from the DateTimeOffset XAxis demo:

![DateTimeOffset XAxis demo](https://github.com/zenjia/MvvmChart/blob/master/Images/DateTimeDemo.PNG)

 
To see more samples, just download the source code, and run the Demo app(more samples will be added continually). Enjoy!


