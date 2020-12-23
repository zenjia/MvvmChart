using MvvmChart.Common;

namespace MvvmCharting.Axis
{
    public class DataOffset : BindableBase
    {
        private double _data;
        public double Data
        {
            get { return this._data; }
            set { SetProperty(ref this._data, value); }
        }

        private double _offset;
        public double Offset
        {
            get { return this._offset; }
            set { SetProperty(ref this._offset, value); }
        }

        private string _labelText;
        public string LabelText
        {
            get { return this._labelText; }
            set { SetProperty(ref this._labelText, value); }
        }
    }
}