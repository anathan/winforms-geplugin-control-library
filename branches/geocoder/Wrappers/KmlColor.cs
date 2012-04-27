// <copyright file="KmlColor.cs" company="FC">
// Copyright (c) 2008 - 2012 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2012-02-29</date>
// <summary>This file is part of FC.GEPluginCtrls
// FC.GEPluginCtrls is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.
// </summary>

namespace FC.GEPlugin
{
    using System;
    using System.Drawing;
    using System.Globalization;

    using FC.GEPlugin.Helpers;

    /// <summary>
    /// Wrapper for the KmlColor object, maps all the getter and setter methods to managed properties. 
    /// <para>
    /// The range of values for any one colour component is 0 to 255 (0x00 to 0xff).
    /// For alpha, 0x00 is fully transparent and 0xff is fully opaque.
    /// </para>
    /// </summary>
    public struct KmlColor : IEquatable<KmlColor>
    {
        /// <summary>
        /// Alpha (opacity) value
        /// </summary>
        private readonly byte alpha;

        /// <summary>
        /// Blue value
        /// </summary>
        private readonly byte blue;

        /// <summary>
        /// Green value
        /// </summary>
        private readonly byte green;

        /// <summary>
        /// Red value
        /// </summary>
        private readonly byte red;

        /// <summary>
        /// Initializes a new instance of the KmlColor struct.
        /// </summary>
        /// <param name="alpha"> the alpha value, Default 255 </param>
        /// <param name="blue"> the blue value, Default 255 </param>
        /// <param name="green"> the green value, Default 255 </param>
        /// <param name="red"> the red value, Default 255 </param>
        public KmlColor(byte alpha = 255, byte blue = 255, byte green = 255, byte red = 255)
            : this()
        {
            this.alpha = alpha;
            this.blue = blue;
            this.green = green;
            this.red = red;
        }

        /// <summary>
        /// Initializes a new instance of the KmlColor struct from a system colour and alpha value.
        /// </summary>
        /// <param name="color"> the colour to base the new kml colour on </param>
        /// <param name="alpha"> Optional alpha value in the range [0-1].  <para>
        /// Where 0 is fully transparent and 1 is fully opaque. Default value is 1
        /// </para>
        /// </param>
        public KmlColor(Color color, double alpha = 1.0)
            : this()
        {
            this.alpha = AlphaRangeFix(alpha);
            this.blue = color.B;
            this.green = color.G;
            this.red = color.R;
        }

        /// <summary>
        /// Initializes a new instance of the KmlColor struct from a colour name and alpha value. 
        /// <para>
        /// Named colours are listed here:
        /// http://msdn.microsoft.com/en-us/library/system.drawing.knowncolor.aspx
        /// If the name parameter is not the valid name of a predefined colour, 
        /// the KmlColour defaults to black (0x000000)
        /// </para>
        /// </summary>
        /// <param name="name"> The name of the colour </param>
        /// <param name="alpha"> Optional alpha value in the range [0-1].  <para>
        /// Where 0 is fully transparent and 1 is fully opaque. Default value is 1
        /// </para>
        /// </param>
        public KmlColor(string name, double alpha = 1.0)
            : this(Color.FromName(name), alpha)
        {
        }

        /// <summary>
        /// Initializes a new instance of the KmlColor struct from an Api KmlColor object.
        /// </summary>
        /// <param name="color"> An Api object to base the new colour on </param>
        public KmlColor(dynamic color)
            : this()
        {
            if (!GEHelpers.IsApiType(color, ApiType.KmlColor))
            {
                throw new ArgumentException("feature is not of the type KmlColor");
            }

            this.alpha = Convert.ToByte(color.getA());
            this.blue = Convert.ToByte(color.getB());
            this.green = Convert.ToByte(color.getG());
            this.red = Convert.ToByte(color.getR());
        }

        /// <summary>
        /// Gets the Alpha (opacity) value
        /// </summary>
        public byte Alpha
        {
            get
            {
                return this.alpha;
            }
        }

        /// <summary>
        /// Gets the Blue value
        /// </summary>
        public byte Blue
        {
            get
            {
                return this.blue;
            }
        }

        /// <summary>
        /// Gets the Green value
        /// </summary>
        public byte Green
        {
            get
            {
                return this.green;
            }
        }

        /// <summary>
        /// Gets the Red value
        /// </summary>
        public byte Red
        {
            get
            {
                return this.red;
            }
        }

        /// <summary>
        /// KmlColor equality operator (aabbggrr)
        /// </summary>
        /// <param name="color1"> The first KmlColor </param>
        /// <param name="color2"> The Second KmlColor </param>
        /// <returns> True if the two KmlColors are equal </returns>
        public static bool operator ==(KmlColor color1, KmlColor color2)
        {
            return color1.Equals(color2);
        }

        /// <summary>
        /// KmlColor inequality operator (aabbggrr)
        /// </summary>
        /// <param name="color1"> The first KmlColor </param>
        /// <param name="color2"> The Second KmlColor </param>
        /// <returns> True if the two KmlColors are unequal </returns>
        public static bool operator !=(KmlColor color1, KmlColor color2)
        {
            return !color1.Equals(color2);
        }

        /// <summary>
        /// KmlColor equality (aabbggrr)
        /// </summary>
        /// <param name="obj"> the object to check against </param>
        /// <returns> True if the KmlColor are equal </returns>
        public override bool Equals(object obj)
        {
            return (obj is KmlColor) && this.Equals((KmlColor)obj);
        }

        /// <summary>
        /// KmlColor equality (argb)
        /// </summary>
        /// <param name="other"> the KmlColor to check against </param>
        /// <returns> True if the KmlColor are equal </returns>
        public bool Equals(KmlColor other)
        {
            return this.Alpha == other.Alpha && this.Red == other.Red && this.Blue == other.Blue
                   && this.Green == other.Green;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns> the hash code for this instance. </returns>
        public override int GetHashCode()
        {
            int hash = 23;

            hash = ((hash << 5) * 37) ^ this.Alpha.GetHashCode();
            hash = ((hash << 5) * 37) ^ this.Red.GetHashCode();
            hash = ((hash << 5) * 37) ^ this.Blue.GetHashCode();
            hash = ((hash << 5) * 37) ^ this.Green.GetHashCode();

            return hash;
        }

        /// <summary>
        /// Overrides the ToString method
        /// </summary>
        /// <returns> The KmlColor object in the aabbggrr format </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture, 
                "{0}{1}{2}{3}", 
                this.Alpha.ToString("X2", CultureInfo.InvariantCulture), 
                this.Blue.ToString("X2", CultureInfo.InvariantCulture), 
                this.Green.ToString("X2", CultureInfo.InvariantCulture), 
                this.Red.ToString("X2", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Converts a System.double in the range [0.0-1.0] to a System.byte [0-255]
        /// </summary>
        /// <param name="input"> The alpha value as a double </param>
        /// <returns> The alpha value as a byte </returns>
        private static byte AlphaRangeFix(double input)
        {
            return (byte)(Math.Max(0, Math.Min(input, 1)) * 255);
        }
    }
}