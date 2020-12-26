 
using MvvmChart.Common;

namespace MvvmCharting.Axis
{
    public interface IAxisItem: IPlottable_1D
    {
        /// <summary>
        /// Accept change of Axis placement
        /// </summary>
        /// <param name="newValue"></param>
        void SetAxisPlacement(AxisPlacement newValue);

        /// <summary>
        /// Accept change of Axis <see cref="LabelTextConverter"/>
        /// </summary>
        /// <param name="newValue"></param>
        void SetLabelTextConverter(IValueConverterNS newValue);
    }
}