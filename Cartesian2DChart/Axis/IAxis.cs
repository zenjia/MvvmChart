using System;
using System.Windows;


namespace Cartesian2DChart.Axis
{
    public interface IAxis
    {
        double Minimum { get; }
        double Maximum { get; }
        Thickness Padding { get; }

        event Action<double> MinimumChanged;
        event Action<double> MaximumChanged;
        event Action<Thickness> PaddingChanged;

        void OnDataRangeChanged(Range range);
        double ActualLength { get; }
    }
}