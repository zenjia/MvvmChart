using MvvmChart.Common;

namespace MvvmCharting.Axis
{

    public class AxisItemDrawingParam : BindableBase
    {
        private double _value;
        public double Value
        {
            get { return this._value; }
            set
            {
                SetProperty(ref this._value, value);
            }
        }

        private double _coordinate;
        public double Coordinate
        {
            get { return this._coordinate; }
            set { SetProperty(ref this._coordinate, value); }
        }

        private string _labelText;
        public string LabelText
        {
            get { return this._labelText; }
            set { SetProperty(ref this._labelText, value); }
        }
    }
}