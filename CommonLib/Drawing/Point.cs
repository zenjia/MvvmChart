using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MvvmChart.Common.Drawing
{
    /// <summary>
    /// Represents an x- and y-coordinate pair in two-dimensional space.
    /// </summary>
    [Serializable]
    public struct Point 
    {
        internal double _x;
        internal double _y;

        /// <summary>Compares two <see cref="T:System.Windows.Point" /> structures for equality. </summary>
        /// <param name="point1">The first <see cref="T:System.Windows.Point" /> structure to compare.</param>
        /// <param name="point2">The second <see cref="T:System.Windows.Point" /> structure to compare.</param>
        /// <returns>
        /// <see langword="true" /> if both the <see cref="P:System.Windows.Point.X" /> and <see cref="P:System.Windows.Point.Y" /> coordinates of <paramref name="point1" /> and <paramref name="point2" /> are equal; otherwise, <see langword="false" />.</returns>
        public static bool operator ==(Point point1, Point point2)
        {
            return point1.X == point2.X && point1.Y == point2.Y;
        }

        /// <summary>Compares two <see cref="T:System.Windows.Point" /> structures for inequality. </summary>
        /// <param name="point1">The first point to compare.</param>
        /// <param name="point2">The second point to compare.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="point1" /> and <paramref name="point2" /> have different <see cref="P:System.Windows.Point.X" /> or <see cref="P:System.Windows.Point.Y" /> coordinates; <see langword="false" /> if <paramref name="point1" /> and <paramref name="point2" /> have the same <see cref="P:System.Windows.Point.X" /> and <see cref="P:System.Windows.Point.Y" /> coordinates.</returns>
        public static bool operator !=(Point point1, Point point2)
        {
            return !(point1 == point2);
        }

        /// <summary>Compares two <see cref="T:System.Windows.Point" /> structures for equality. </summary>
        /// <param name="point1">The first point to compare.</param>
        /// <param name="point2">The second point to compare.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="point1" /> and <paramref name="point2" /> contain the same <see cref="P:System.Windows.Point.X" /> and <see cref="P:System.Windows.Point.Y" /> values; otherwise, <see langword="false" />.</returns>
        public static bool Equals(Point point1, Point point2)
        {
            return point1.X.Equals(point2.X) && point1.Y.Equals(point2.Y);
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is a <see cref="T:System.Windows.Point" /> and whether it contains the same coordinates as this <see cref="T:System.Windows.Point" />. </summary>
        /// <param name="o">The <see cref="T:System.Object" /> to compare.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="o" /> is a <see cref="T:System.Windows.Point" /> and contains the same <see cref="P:System.Windows.Point.X" /> and <see cref="P:System.Windows.Point.Y" /> values as this <see cref="T:System.Windows.Point" />; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object o)
        {
            return o != null && o is Point point2 && Point.Equals(this, point2);
        }

        /// <summary>Compares two <see cref="T:System.Windows.Point" /> structures for equality.</summary>
        /// <param name="value">The point to compare to this instance.</param>
        /// <returns>
        /// <see langword="true" /> if both <see cref="T:System.Windows.Point" /> structures contain the same <see cref="P:System.Windows.Point.X" /> and <see cref="P:System.Windows.Point.Y" /> values; otherwise, <see langword="false" />.</returns>
        public bool Equals(Point value)
        {
            return Point.Equals(this, value);
        }

        /// <summary>Returns the hash code for this <see cref="T:System.Windows.Point" />.</summary>
        /// <returns>The hash code for this <see cref="T:System.Windows.Point" /> structure.</returns>
        public override int GetHashCode()
        {
            double num = this.X;
            int hashCode1 = num.GetHashCode();
            num = this.Y;
            int hashCode2 = num.GetHashCode();
            return hashCode1 ^ hashCode2;
        }

 

        /// <summary>Gets or sets the <see cref="P:System.Windows.Point.X" />-coordinate value of this <see cref="T:System.Windows.Point" /> structure. </summary>
        /// <returns>The <see cref="P:System.Windows.Point.X" />-coordinate value of this <see cref="T:System.Windows.Point" /> structure.  The default value is 0.</returns>
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

        /// <summary>Gets or sets the <see cref="P:System.Windows.Point.Y" />-coordinate value of this <see cref="T:System.Windows.Point" />. </summary>
        /// <returns>The <see cref="P:System.Windows.Point.Y" />-coordinate value of this <see cref="T:System.Windows.Point" /> structure.  The default value is 0.</returns>
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

        /// <summary>Creates a <see cref="T:System.String" /> representation of this <see cref="T:System.Windows.Point" />. </summary>
        /// <returns>A <see cref="T:System.String" /> containing the <see cref="P:System.Windows.Point.X" /> and <see cref="P:System.Windows.Point.Y" /> values of this <see cref="T:System.Windows.Point" /> structure.</returns>
        public override string ToString()
        {
            return $"({X}, {Y})";
        }

       

        /// <summary>Creates a new <see cref="T:System.Windows.Point" /> structure that contains the specified coordinates. </summary>
        /// <param name="x">The x-coordinate of the new <see cref="T:System.Windows.Point" /> structure. </param>
        /// <param name="y">The y-coordinate of the new <see cref="T:System.Windows.Point" /> structure. </param>
        public Point(double x, double y)
        {
            this._x = x;
            this._y = y;
        }

        /// <summary>Offsets a point's <see cref="P:System.Windows.Point.X" /> and <see cref="P:System.Windows.Point.Y" /> coordinates by the specified amounts.</summary>
        /// <param name="offsetX">The amount to offset the point's
        /// <see cref="P:System.Windows.Point.X" /> coordinate. </param>
        /// <param name="offsetY">The amount to offset thepoint's <see cref="P:System.Windows.Point.Y" /> coordinate.</param>
        public void Offset(double offsetX, double offsetY)
        {
            this._x += offsetX;
            this._y += offsetY;
        }

        
    }
}
