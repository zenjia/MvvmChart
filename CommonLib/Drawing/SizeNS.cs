using System;

namespace MvvmChart.Common.Drawing
{
    /// <summary>
    /// SizeNS is the .Net Standard version of WPF or UWP Point type
    /// </summary>
    public struct SizeNS 
    {
        private static readonly SizeNS s_empty = SizeNS.CreateEmptySize();
        internal double _width;
        internal double _height;

        public static bool operator ==(SizeNS size1, SizeNS size2)
        {
            return size1.Width == size2.Width && size1.Height == size2.Height;
        }

        public static bool operator !=(SizeNS size1, SizeNS size2)
        {
            return !(size1 == size2);
        }

        public static bool Equals(SizeNS size1, SizeNS size2)
        {
            if (size1.IsEmpty)
                return size2.IsEmpty;
            return size1.Width.Equals(size2.Width) && size1.Height.Equals(size2.Height);
        }

        public override bool Equals(object o)
        {
            return o != null && o is SizeNS size2 && SizeNS.Equals(this, size2);
        }

        public bool Equals(SizeNS value)
        {
            return SizeNS.Equals(this, value);
        }

        /// <summary>Gets the hash code for this instance of <see cref="T:System.Windows.SizeNS" />. </summary>
        /// <returns>The hash code for this instance of <see cref="T:System.Windows.SizeNS" />.</returns>
        public override int GetHashCode()
        {
            return this.IsEmpty ? 0 : this.Width.GetHashCode() ^ this.Height.GetHashCode();
        }

 

        public override string ToString()
        {
            return $"({this.Width}, {this.Height})";
        }

 

 

        public SizeNS(double width, double height)
        {
            if (width < 0.0 || height < 0.0)
                throw new ArgumentException(("Size_WidthAndHeightCannotBeNegative"));
            this._width = width;
            this._height = height;
        }

        public static SizeNS Empty
        {
            get
            {
                return SizeNS.s_empty;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return this._width < 0.0;
            }
        }

        public double Width
        {
            get
            {
                return this._width;
            }
            set
            {
                if (this.IsEmpty)
                    throw new InvalidOperationException("Size_CannotModifyEmptySize");
                if (value < 0.0)
                    throw new ArgumentException("Size_WidthCannotBeNegative");
                this._width = value;
            }
        }

        public double Height
        {
            get
            {
                return this._height;
            }
            set
            {
                if (this.IsEmpty)
                    throw new InvalidOperationException("Size_CannotModifyEmptySize");
                if (value < 0.0)
                    throw new ArgumentException("Size_HeightCannotBeNegative");
                this._height = value;
            }
        }

 



        private static SizeNS CreateEmptySize()
        {
            return new SizeNS()
            {
                _width = double.NegativeInfinity,
                _height = double.NegativeInfinity
            };
        }
    }
}