﻿using System;

namespace MvvmCharting.Axis
{
    public interface IYAxisOwner : IAxisOwner
    {


        event Action<PlottingSettingsBase> VerticalSettingChanged;
    }
}