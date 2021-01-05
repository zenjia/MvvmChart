using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using MvvmCharting.Common;
using MvvmCharting.Drawing;

namespace MvvmCharting.WpfFX.Series
{
    public class LineSeriesBase : InteractiveControl
    {
        protected bool IsAreaMode { get; set; }

        internal ILineSeriesOwner Owner { get; set; }

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }
        public static readonly DependencyProperty StrokeProperty =
            Shape.StrokeProperty.AddOwner(typeof(LineSeriesBase));

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            Shape.StrokeThicknessProperty.AddOwner(typeof(LineSeriesBase), new PropertyMetadata(1.0));

        public Style LineStyle
        {
            get { return (Style)GetValue(LineStyleProperty); }
            set { SetValue(LineStyleProperty, value); }
        }
        public static readonly DependencyProperty LineStyleProperty =
            DependencyProperty.Register("LineStyle", typeof(Style), typeof(LineSeriesBase), new PropertyMetadata(null));

        internal void UpdateShape()
        {
            if (this.Owner?.SeriesControlOwner == null)
            {
                return;
            }

            if (this.Owner.SeriesControlOwner.IsSeriesCollectionChanging)
            {
                return;
            }

            if (!this.CanUpdateShape())
            {
                return;
            }

            var coordinates = this.Owner.GetCoordinates();
 
            PointNS[] previous = this.Owner.GetPreviousSeriesCoordinates(this.IsAreaMode);

            OnCoordinatesChanged(coordinates, previous);

        }

        protected virtual bool CanUpdateShape()
        {
            return !this.Owner.XPixelPerUnit.IsInvalid() && 
                   !this.Owner.YPixelPerUnit.IsInvalid() && 
                   this.IsLoaded;
        }

        /// <summary>
        /// If a user wants to customize the Line/Area plotting logic, they can
        /// override this method. 
        /// </summary>
        /// <param name="coordinates">current series coordinates</param>
        /// <param name="previousCoordinates">previous series coordinates</param>
        protected virtual void OnCoordinatesChanged(PointNS[] coordinates, PointNS[] previousCoordinates)
        {

        }
    }
}