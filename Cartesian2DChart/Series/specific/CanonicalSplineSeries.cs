using System.Windows;

namespace MvvmCharting
{
    public class CanonicalSplineSeries : LineSeries
    {


        public double SplineTension
        {
            get { return (double)GetValue(SplineTensionProperty); }
            set { SetValue(SplineTensionProperty, value); }
        }
        public static readonly DependencyProperty SplineTensionProperty =
            DependencyProperty.Register("SplineTension", typeof(double), typeof(CanonicalSplineSeries), new PropertyMetadata(0.4));



        static CanonicalSplineSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CanonicalSplineSeries), new FrameworkPropertyMetadata(typeof(CanonicalSplineSeries)));
        }
    }
}