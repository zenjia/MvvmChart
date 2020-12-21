using System.Windows;

namespace MvvmCharting
{
    public class DataPointViewModel: BindableBase
    {
        public DataPointViewModel(object item)
        {
            this.Item = item;
        }

        public object Item { get; }

        private Point _position;
        public Point Position
        {
            get { return this._position; }
            set { SetProperty(ref this._position, value); }
        }
    }
}