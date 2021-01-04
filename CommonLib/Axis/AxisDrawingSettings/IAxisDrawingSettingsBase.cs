using System.Collections.Generic;

namespace MvvmCharting.Axis
{
    public interface IAxisDrawingSettingsBase
    {
 
        IEnumerable<object> GetPlottingValues();

        bool CanUpdateAxisItems();

        bool CanUpdateAxisItemsCoordinate();

        double CalculateCoordinate(double value, AxisType axisType);
    }
}