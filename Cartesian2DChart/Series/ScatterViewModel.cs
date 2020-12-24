using System.Windows;
using MvvmChart.Common;

namespace MvvmCharting
{
    public class ScatterViewModel: BindableBase
    {
        public ScatterViewModel(object item)
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