using MvvmCharting.Common;

namespace MvvmCharting.Axis
{

    public interface IAxisItemDrawingBaseParams
    {
        double Coordinate { get; set; }
        string LabelText { get; set; }

        object Value { get; set; }
    }

 

    public class AxisItemDrawingParam : BindableBase, IAxisItemDrawingBaseParams
    {
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

        private object _value;
        public object Value
        {
            get { return this._value; }
            set
            {
                SetProperty(ref this._value, value);
            }
        }
    }

 
}