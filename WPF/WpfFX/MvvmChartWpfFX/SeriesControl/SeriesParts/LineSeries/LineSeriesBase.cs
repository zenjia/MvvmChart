using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using MvvmCharting.Common; 

namespace MvvmCharting.WpfFX.Series
{
    /// <summary>
    /// Represents a line or area series which can be displayed.
    /// If the Fill property of the path is null, then it will
    /// draw a line series(<see cref="LineSeries"/>), otherwise, area series(<see cref="AreaSeries"/>).
    /// This class does not provide a default Template or any plotting logic,
    /// it's the descendant class's responsibility to override <see cref="OnCoordinatesChanged"/> to
    /// provide their own plotting logic.
    /// </summary>
    public abstract class LineSeriesBase : InteractiveControl
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

        /// <summary>
        /// Update the Shape/Geometry of a Line(for <see cref="LineSeries"/>) or
        /// a Area(for <see cref="AreaSeries"/>)
        /// </summary>
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

            if (!CanUpdateShape())
            {
                return;
            }

            var coordinates = this.Owner.GetCoordinates();
 
            Point[] previous = this.Owner.GetPreviousCoordinates(this.IsAreaMode);

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
        /// <param name="previousCoordinates">previous series coordinates(Used to plot <see cref="AreaSeries"/>
        /// or other series types when in <see cref="T:StackMode.Stacked"/> or <see cref="T:StackMode.Stacked100"/>)</param>
        protected abstract void OnCoordinatesChanged(Point[] coordinates, Point[] previousCoordinates);

    }
}