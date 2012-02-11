// <copyright file="KmlColor.cs" company="FC">
// Copyright (c) 2012 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2012-03-10</date>
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
namespace FC.GEPluginCtrls
{
    using System;
    using System.Drawing;

    /// <summary>
    /// Wrapper for the KmlColor com object, maps all the getter and setter methods to managed properties.
    /// The range of values for any one color component is 0 to 255 (0x00 to 0xff).
    /// For alpha, 0x00 is fully transparent and 0xff is fully opaque.
    /// </summary>
    public struct KmlColor
    {
        /// <summary>
        /// Gets or sets the Alpha (opacity) value 
        /// </summary>
        public byte Alpha { get; set; }

        /// <summary>
        /// Gets or sets the Blue value 
        /// </summary>
        public byte Blue { get; set; }

        /// <summary>
        /// Gets or sets the Green value 
        /// </summary>
        public byte Green { get; set; }

        /// <summary>
        /// Gets or sets the Red value 
        /// </summary>
        public byte Red { get; set; }

        /// <summary>
        /// Initializes a new instance of the KmlColor struct.
        /// </summary>
        /// <param name="alpha">the alpha value</param>
        /// <param name="blue">the blue value</param>
        /// <param name="green">the green value</param>
        /// <param name="red">the red value</param>
        public KmlColor(byte alpha = 255, byte blue = 255, byte green = 255, byte red = 255)
            : this()
        {
            this.Alpha = alpha;
            this.Blue = blue;
            this.Green = green;
            this.Red = red;
        }

        /// <summary>
        /// Initializes a new instance of the KmlColor struct.
        /// </summary>
        /// <param name="aabbggrr">Tuple in the squence: alpha, blue, green, red</param>
        public KmlColor(Tuple<byte, byte, byte, byte> aabbggrr)
            : this()
        {
            this.Alpha = aabbggrr.Item1;
            this.Blue = aabbggrr.Item2;
            this.Green = aabbggrr.Item3;
            this.Red = aabbggrr.Item4;
        }

        /// <summary>
        /// Initializes a new instance of the KmlColor struct.
        /// </summary>
        /// <param name="color">the color to base the new kml color on</param>
        /// <param name="alpha">the alpha value</param>
        public KmlColor(Color color, double alpha = 1.0)
            : this()
        {
            this.Alpha = alphaRangeFix(alpha);
            this.Blue = color.B;
            this.Green = color.G;
            this.Red = color.R;
        }

        /// <summary>
        /// Initializes a new instance of the KmlColor struct from an Api KmlColor object.
        /// </summary>
        /// <param name="colorObject">the api object to base the new color on</param>
        /// <param name="alpha">the alpha value</param>
        public KmlColor(dynamic colorObject)
            : this()
        {
            if (GEHelpers.IsApiType(colorObject, ApiType.KmlColor))
            {

                this.Alpha = Convert.ToByte(colorObject.getA());
                this.Blue = Convert.ToByte(colorObject.getB());
                this.Green = Convert.ToByte(colorObject.getG());
                this.Red = Convert.ToByte(colorObject.getR());
            }
            else
            {
                throw new ArgumentException("feature is not of the type KmlColor");
            }
        }

        public static dynamic ToApiObject(dynamic ge)
        {
            if (!GEHelpers.IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            return ge;
        }

        /// <summary>
        /// Overrides the ToString method
        /// </summary>
        /// <returns>The KmlColor object in the aabbggrr format</returns>
        public override string ToString()
        {
            return string.Format(
                "{0}{1}{2}{3}",
                this.Alpha.ToString("X2"),
                this.Blue.ToString("X2"),
                this.Green.ToString("X2"),
                this.Red.ToString("X2"));
        }

        /// <summary>
        /// Converts a System.double in the range [0.0-1.0] to a System.byte [0-255]
        /// </summary>
        /// <param name="input">The alpha value as a double</param>
        /// <returns>The alpha value as a byte</returns>
        private byte alphaRangeFix(double input)
        {
            return (byte)(Math.Max(0, Math.Min(input, 1)) * 255);
        }
    }
}