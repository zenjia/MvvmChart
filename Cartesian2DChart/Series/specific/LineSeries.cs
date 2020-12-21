using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MvvmCharting
{


  
    public class LineSeries : SeriesBase
    {
 
        static LineSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LineSeries), new FrameworkPropertyMetadata(typeof(LineSeries)));
        }

        public PointCollection PointCollection
        {
            get { return (PointCollection)GetValue(PointCollectionProperty); }
            set { SetValue(PointCollectionProperty, value); }
        }
        public static readonly DependencyProperty PointCollectionProperty =
            DependencyProperty.Register("PointCollection", typeof(PointCollection), typeof(LineSeries), new PropertyMetadata(null));



        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }
        public static readonly DependencyProperty StrokeProperty =
            Shape.StrokeProperty.AddOwner(typeof(LineSeries));



        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            Shape.StrokeThicknessProperty.AddOwner(typeof(LineSeries), new PropertyMetadata(1.0));

        //public override void OnApplyTemplate()
        //{
        //    base.OnApplyTemplate();

        //    UpdateShape();
        //}







        protected virtual PointCollection GetPointCollection()
        {
            PointCollection pc = new PointCollection(this.DataPointViewModels.Select(x => x.Position).OrderBy(x=>x.X));
            return pc;
        }

        private void UpdateShape()
        {

            if (this.DataPointViewModels == null)
            {
                return;
            }

            this.PointCollection = GetPointCollection();

            //this.PART_PolyLine.Points = GetPointCollection();

            //Debug.WriteLine(this.GetHashCode() + $" UpdateShape:  " + this.PART_PolyLine.Points.Count);
        }

        protected override void OnItemPointViewModelsChanged()
        {
            base.OnItemPointViewModelsChanged();
            UpdateShape();
        }

        protected override void OnPointsPositionUpdated()
        {
            UpdateShape();
        }



    }
}
