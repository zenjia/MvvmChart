using System.Collections.Generic;
using System.Windows;
using MvvmCharting.Axis;

namespace MvvmCharting.WpfFX.Axis
{    
    /// <summary>
    /// Represents a discrete axis for category data type.
    /// </summary>
    public class CategoryAxis : AxisBase, ICategoryAxis
    {
        static CategoryAxis()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CategoryAxis), new FrameworkPropertyMetadata(typeof(CategoryAxis)));
        }

        protected override void UpdateAxisDrawingSettings()
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<double> GetAxisItemCoordinates()
        {
            throw new System.NotImplementedException();
        }

 

        protected override bool DoLoadAxisItemDrawingParams()
        {
            throw new System.NotImplementedException();
        }

        protected override void DoUpdateAxisItemsCoordinate()
        {
            throw new System.NotImplementedException();
        }
    }
}