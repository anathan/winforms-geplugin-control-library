// <copyright file="Maths.cs" company="FC">
// Copyright (c) 2011 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2009-03-02</date>
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
namespace FC.GEPluginCtrls.Maths
{
    using System;

    /// <summary>
    /// Various Geodesic methods to work with the plugin api
    /// This class is based on the javascript library geojs by Roman Nurik
    /// See http://code.google.com/p/geojs/
    /// </summary>
    public class Maths
    {
        /// <summary>
        /// Miles To Kilometres Conversion ratio.
        /// </summary>
        private const double MilesToKilometres = 0.621371192;

        /// <summary>
        /// Feet to Metres conversion ratio.
        /// </summary>
        private const double FeetToMetres = 0.3048;

        /// <summary>
        /// Converts decimal degrees to radians
        /// </summary>
        /// <param name="degrees">value in degrees</param>
        /// <returns>value in radians</returns>
        public static double ConvertDegreesToRadians(double degrees)
        {
            return degrees == 0 ? degrees : (degrees * Math.PI / 180.0);
        }

        /// <summary>
        /// Converts radians to decimal degrees
        /// </summary>
        /// <param name="radains">value in radians</param>
        /// <returns>value in degrees</returns>
        public static double ConvertRadiansToDegrees(double radains)
        {
            return radains == 0 ? radains : (radains / Math.PI * 180.0);
        }

        /// <summary>
        /// Converts a heading in the range [-180,180] to a bearing in the range [0,360] 
        /// </summary>
        /// <param name="heading">heading in the range [-180,180]</param>
        /// <returns>bearing in the range [0,360]</returns>
        public static double ConvertHeadingToBearing(double heading)
        {
            return heading <= 0 ? 360 - (-heading % 360) : heading % 360;
        }

        /// <summary>
        /// Convert Kilometres To Miles 
        /// </summary>
        /// <param name="kilometres">distance in kilometrees</param>
        /// <returns>distance in miles</returns>
        public static double ConvertKilometresToMiles(double kilometres)
        {
            return kilometres == 0 ? kilometres : kilometres * MilesToKilometres;
        }

        /// <summary>
        /// Convert Miles To Kilometres
        /// </summary>
        /// <param name="miles">distance in miles</param>
        /// <returns>distance in kilometrees</returns>
        public static double ConvertMilesToKilometres(double miles)
        {
            return miles == 0 ? miles : miles / MilesToKilometres;
        }

        /// <summary>
        /// Keep a number in the [0,PI] range
        /// </summary>
        /// <param name="radians">value in radians</param>
        /// <returns>normalised angle in radians</returns>
        public static double NormaliseAngle(double radians)
        {
            radians = radians % (2 * Math.PI);
            return radians >= 0 ? radians : radians + (2 * Math.PI);
        }

        /// <summary>
        ///  Normalises a latitude to the [-90,90] range. Latitudes above 90 or below -90 are capped, not wrapped.
        /// </summary>
        /// <param name="latitude">The latitude in degrees to normalize.</param>
        /// <returns>Latitude within the [-90,90] range</returns>
        public static double NormaliseLatitude(double latitude)
        {
            return Math.Max(-90, Math.Min(90, latitude));
        }

        /// <summary>
        /// Normalises a longitude to the [-180,180] range. Longitudes above 180 or below -180 are wrapped.
        /// </summary>
        /// <param name="longitude">The longitude in degrees to normalize</param>
        /// <returns>Longitude within the [-180,180] range</returns>
        public static double NormaliseLongitude(double longitude)
        {
            if (longitude % 360 == 180)
            {
                return 180;
            }

            longitude = longitude % 360;
            return longitude < -180 ? longitude + 360 : longitude > 180 ? longitude - 360 : longitude;
        }

        /// <summary>
        /// Reverses a number in the [0,PI] range
        /// </summary>
        /// <param name="radians">value in radians</param>
        /// <returns>The oposite angle</returns>
        public static double ReverseAngle(double radians)
        {
            return NormaliseAngle(radians + Math.PI);
        }

        /// <summary>
        /// Wraps the given number to the given range.
        /// If the wrapped value is exactly equal to min or max, favors max, unless favorMin is true.
        /// </summary>
        /// <param name="value">The value to wrap</param>
        /// <param name="minimum">The minimum bound of the range</param>
        /// <param name="maximum">The maximum bound of the range</param>
        /// <param name="favorMinimum">Whether or not to favor min over max in the case of ambiguity. Default is false</param>
        /// <returns>The value in the given range.</returns>
        public static double WrapValue(double value, double minimum, double maximum, bool favorMinimum = false)
        {
            // Don't wrap min as max.
            if (value == minimum)
            {
                return minimum;
            }

            // Normalize to min = 0.
            value -= minimum;

            value = value % (maximum - minimum);
            if (value < 0)
            {
                value += maximum - minimum;
            }

            // Reverse normalization.
            value += minimum;

            // When ambiguous (min or max), return max unless favorMin is true.
            return (value == minimum) ? (favorMinimum ? minimum : maximum) : value;
        }

        /// <summary>
        /// Constrains the given number to the given range.
        /// </summary>
        /// <param name="value">The value to wrap</param>
        /// <param name="minimum">The minimum bound of the range</param>
        /// <param name="maximum">The maximum bound of the range</param>
        /// <returns>The value constrained to the given range</returns>
        public static double ConstrainValue(double value, double minimum, double maximum)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }
    }
}
