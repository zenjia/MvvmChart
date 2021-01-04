using System;

namespace MvvmCharting.Axis
{
    public interface IXAxisOwner : IAxisOwner
    {


        event Action<PlottingRangeBase> HorizontalSettingChanged;


    }
}