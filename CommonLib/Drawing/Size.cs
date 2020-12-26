using System;

namespace MvvmChart.Common.Drawing
{
    /// <summary>
    /// Implements a structure that is used to describe the <see cref="T:System.Windows.Size" /> of an object.
    /// </summary>
    public struct Size 
    {
        private static readonly Size s_empty = Size.CreateEmptySize();
        internal double _width;
        internal double _height;

        /// <summary>Compares two instances of <see cref="T:System.Windows.Size" /> for equality. </summary>
        /// <param name="size1">The first instance of <see cref="T:System.Windows.Size" /> to compare.</param>
        /// <param name="size2">The second instance of <see cref="T:System.Windows.Size" /> to compare.</param>
        /// <returns>true if the two instances of <see cref="T:System.Windows.Size" /> are equal; otherwise <see langword="false" />.</returns>
        public static bool operator ==(Size size1, Size size2)
        {
            return size1.Width == size2.Width && size1.Height == size2.Height;
        }

        /// <summary>Compares two instances of <see cref="T:System.Windows.Size" /> for inequality. </summary>
        /// <param name="size1">The first instance of <see cref="T:System.Windows.Size" /> to compare.</param>
        /// <param name="size2">The second instance of <see cref="T:System.Windows.Size" /> to compare.</param>
        /// <returns>
        /// <see langword="true" /> if the instances of <see cref="T:System.Windows.Size" /> are not equal; otherwise <see langword="false" />.</returns>
        public static bool operator !=(Size size1, Size size2)
        {
            return !(size1 == size2);
        }

        /// <summary>Compares two instances of <see cref="T:System.Windows.Size" /> for equality. </summary>
        /// <param name="size1">The first instance of <see cref="T:System.Windows.Size" /> to compare.</param>
        /// <param name="size2">The second instance of <see cref="T:System.Windows.Size" /> to compare.</param>
        /// <returns>
        /// <see langword="true" /> if the instances of <see cref="T:System.Windows.Size" /> are equal; otherwise, <see langword="false" />.</returns>
        public static bool Equals(Size size1, Size size2)
        {
            if (size1.IsEmpty)
                return size2.IsEmpty;
            return size1.Width.Equals(size2.Width) && size1.Height.Equals(size2.Height);
        }

        /// <summary>Compares an object to an instance of <see cref="T:System.Windows.Size" /> for equality. </summary>
        /// <param name="o">The <see cref="T:System.Object" /> to compare.</param>
        /// <returns>
        /// <see langword="true" /> if the sizes are equal; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object o)
        {
            return o != null && o is Size size2 && Size.Equals(this, size2);
        }

        /// <summary>Compares a value to an instance of <see cref="T:System.Windows.Size" /> for equality. </summary>
        /// <param name="value">The size to compare to this current instance of <see cref="T:System.Windows.Size" />.</param>
        /// <returns>
        /// <see langword="true" /> if the instances of <see cref="T:System.Windows.Size" /> are equal; otherwise, <see langword="false" />.</returns>
        public bool Equals(Size value)
        {
            return Size.Equals(this, value);
        }

        /// <summary>Gets the hash code for this instance of <see cref="T:System.Windows.Size" />. </summary>
        /// <returns>The hash code for this instance of <see cref="T:System.Windows.Size" />.</returns>
        public override int GetHashCode()
        {
            return this.IsEmpty ? 0 : this.Width.GetHashCode() ^ this.Height.GetHashCode();
        }

 

        /// <summary>Returns a <see cref="T:System.String" /> that represents this <see cref="T:System.Windows.Size" /> object. </summary>
        /// <returns>A <see cref="T:System.String" /> that specifies the width followed by the height.</returns>
        public override string ToString()
        {
            return $"({this.Width}, {this.Height})";
        }

 

 

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Size" /> structure and assigns it an initial <paramref name="width" /> and <paramref name="height" />.</summary>
        /// <param name="width">The initial width of the instance of <see cref="T:System.Windows.Size" />.</param>
        /// <param name="height">The initial height of the instance of <see cref="T:System.Windows.Size" />.</param>
        public Size(double width, double height)
        {
            if (width < 0.0 || height < 0.0)
                throw new ArgumentException(("Size_WidthAndHeightCannotBeNegative"));
            this._width = width;
            this._height = height;
        }

        /// <summary>Gets a value that represents a static empty <see cref="T:System.Windows.Size" />. </summary>
        /// <returns>An empty instance of <see cref="T:System.Windows.Size" />.</returns>
        public static Size Empty
        {
            get
            {
                return Size.s_empty;
            }
        }

        /// <summary>Gets a value that indicates whether this instance of <see cref="T:System.Windows.Size" /> is <see cref="P:System.Windows.Size.Empty" />. </summary>
        /// <returns>
        /// <see langword="true" /> if this instance of size is <see cref="P:System.Windows.Size.Empty" />; otherwise <see langword="false" />.</returns>
        public bool IsEmpty
        {
            get
            {
                return this._width < 0.0;
            }
        }

        /// <summary>Gets or sets the <see cref="P:System.Windows.Size.Width" /> of this instance of <see cref="T:System.Windows.Size" />. </summary>
        /// <returns>The <see cref="P:System.Windows.Size.Width" /> of this instance of <see cref="T:System.Windows.Size" />. The default value is 0. The value cannot be negative.</returns>
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

        /// <summary>Gets or sets the <see cref="P:System.Windows.Size.Height" /> of this instance of <see cref="T:System.Windows.Size" />. </summary>
        /// <returns>The <see cref="P:System.Windows.Size.Height" /> of this instance of <see cref="T:System.Windows.Size" />. The default is 0. The value cannot be negative.</returns>
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

 

        /// <summary>Explicitly converts an instance of <see cref="T:System.Windows.Size" /> to an instance of <see cref="T:System.Windows.Point" />. </summary>
        /// <param name="size">The <see cref="T:System.Windows.Size" /> value to be converted.</param>
        /// <returns>A <see cref="T:System.Windows.Point" /> equal in value to this instance of <see cref="T:System.Windows.Size" />.</returns>
        public static explicit operator Point(Size size)
        {
            return new Point(size._width, size._height);
        }

        private static Size CreateEmptySize()
        {
            return new Size()
            {
                _width = double.NegativeInfinity,
                _height = double.NegativeInfinity
            };
        }
    }
}