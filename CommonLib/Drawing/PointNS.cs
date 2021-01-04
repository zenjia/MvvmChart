namespace MvvmCharting.Drawing
{
    /// <summary>
    /// Represents an x- and y-coordinate pair in two-dimensional space.
    /// This is the .NET Standard version of WPF/UWP Point type.
    /// </summary>
    public struct PointNS 
    {
        internal double _x;
        internal double _y;

        public static readonly PointNS Empty = new PointNS(double.NaN, double.NaN);
        
        public static bool operator ==(PointNS point1, PointNS point2)
        {
            return point1.X == point2.X && point1.Y == point2.Y;
        }

        public static bool operator !=(PointNS point1, PointNS point2)
        {
            return !(point1 == point2);
        }

        public static bool Equals(PointNS point1, PointNS point2)
        {
            return point1.X.Equals(point2.X) && point1.Y.Equals(point2.Y);
        }

        public override bool Equals(object o)
        {
            return o != null && o is PointNS point2 && PointNS.Equals(this, point2);
        }

        public bool Equals(PointNS value)
        {
            return PointNS.Equals(this, value);
        }

        public override int GetHashCode()
        {
            double num = this.X;
            int hashCode1 = num.GetHashCode();
            num = this.Y;
            int hashCode2 = num.GetHashCode();
            return hashCode1 ^ hashCode2;
        }

        public double X
        {
            get
            {
                return this._x;
            }
            set
            {
                this._x = value;
            }
        }

        public double Y
        {
            get
            {
                return this._y;
            }
            set
            {
                this._y = value;
            }
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public PointNS(double x, double y)
        {
            this._x = x;
            this._y = y;
        }

        public void Offset(double offsetX, double offsetY)
        {
            this._x += offsetX;
            this._y += offsetY;
        }

        
    }
}
