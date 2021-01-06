using System.Diagnostics;
using System.Windows;
using Windows.Foundation;
using MvvmCharting.Common;

namespace Demo
{
    public class ChartValuePaddingViewModel : BindableBase
    {
        private double _xMinValuePadding;
        public double XMinValuePadding
        {
            get { return this._xMinValuePadding; }
            set
            {
                if (SetProperty(ref this._xMinValuePadding, value))
                {
                    UpdateXValuePadding();
                }

            }
        }

        private double _yMinValuePadding;
        public double YMinValuePadding
        {
            get { return this._yMinValuePadding; }
            set
            {
                if (SetProperty(ref this._yMinValuePadding, value))
                {
                    UpdateYValuePadding();
                }

            }
        }

        private double _xMaxValuePadding;
        public double XMaxValuePadding
        {
            get { return this._xMaxValuePadding; }
            set
            {
                if (SetProperty(ref this._xMaxValuePadding, value))
                {
                    UpdateXValuePadding();
                }

            }
        }

        private double _yMaxValuePadding;
        public double YMaxValuePadding
        {
            get { return this._yMaxValuePadding; }
            set
            {
                if (SetProperty(ref this._yMaxValuePadding, value))
                {
                    UpdateYValuePadding();
                }

            }
        }

        private void UpdateXValuePadding()
        {
            this.XValuePadding = new Range(this.XMinValuePadding, this.XMaxValuePadding);
        }

        private void UpdateYValuePadding()
        {
            this.YValuePadding = new Range(this.YMinValuePadding, this.YMaxValuePadding);
        }

        private Range _yValuePadding;
        public Range YValuePadding
        {
            get { return this._yValuePadding; }
            set
            {
                if (SetProperty(ref this._yValuePadding, value))
                {
                    
                }

            }
        }

        private Range _xValuePadding;
        public Range XValuePadding
        {
            get { return this._xValuePadding; }
            set
            {
                if (SetProperty(ref this._xValuePadding, value))
                {
                    
                }

            }
        }
    }
}