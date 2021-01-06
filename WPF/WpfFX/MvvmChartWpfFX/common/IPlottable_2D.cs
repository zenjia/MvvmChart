using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Security;
using System.Text;
using System.Windows;

namespace MvvmCharting.WpfFX
{


    /// <summary>
    /// Represents a UIElement that can be plotted(positioned) in a
    /// 2-dimension space
    /// </summary>
    public interface IPlottable_2D
    {
        Point Coordinate { get; set; }
    }
}