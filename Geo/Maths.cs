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
namespace FC.GEPluginCtrls.Geo
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
        ///  Earth’s mean radius in Kilometres
        /// </summary>
        public const double EarthMeanRadiusKilometres = 6371;

        /// <summary>
        ///  Earth’s mean radius in miles
        /// </summary>
        public const double EarthMeanRadiusMiles = 3959;

        /// <summary>
        /// Miles To Kilometres Conversion ratio.
        /// </summary>
        /// <remarks>divide by to get miles to km, multiply to get km to miles</remarks>
        public const double MilesToKilometresRatio = 0.621371192;

        /// <summary>
        /// Feet to Metres conversion ratio.
        /// </summary>
        public const double FeetToMetresRatio = 0.3048;

        /// <summary>
        /// Get the final bearing from one Coordinate to another
        /// </summary>
        /// <param name="start">the start Coordinate</param>
        /// <param name="destination">the destination Coordinate</param>
        /// <returns>The final bearing from start to destination</returns>
        /// <remarks>http://williams.best.vwh.net/avform.htm</remarks>
        public static double BearingFinal(Coordinate start, Coordinate destination)
        {
            return (BearingInitial(destination, start) + 180) % 360;
        }

        /// <summary>
        /// Get the inital bearing from one Coordinate to another
        /// </summary>
        /// <param name="start">the starting Coordinate</param>
        /// <param name="destination">the destination Coordinate</param>
        /// <remarks>http://williams.best.vwh.net/avform.htm</remarks>
        /// <returns>The inital bearing from start to destination</returns>
        public static double BearingInitial(Coordinate start, Coordinate destination)
        {
            double lat1 = ConvertDegreesToRadians(destination.Latitude);
            double lat2 = ConvertDegreesToRadians(start.Latitude);
            double dLon = ConvertDegreesToRadians(destination.Longitude - start.Longitude);
            double y = Math.Sin(dLon) * Math.Cos(lat2);
            double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
            double bearing = Math.Atan2(y, x);

            return (ConvertRadiansToDegrees(bearing) + 360) % 360;
        }

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
            return kilometres == 0 ? kilometres : kilometres * MilesToKilometresRatio;
        }

        /// <summary>
        /// Convert Miles To Kilometres
        /// </summary>
        /// <param name="miles">distance in miles</param>
        /// <returns>distance in kilometrees</returns>
        public static double ConvertMilesToKilometres(double miles)
        {
            return miles == 0 ? miles : miles / MilesToKilometresRatio;
        }

        /// <summary>
        /// Returns the distance in miles or kilometers of any two latitude / longitude points.
        /// </summary>
        /// <param name="origin">The start api object </param>
        /// <param name="destination">The destination api object</param>
        /// <returns>Distance in kilometers</returns>
        public static double DistanceCosine(dynamic origin, dynamic destination)
        {
            return DistanceCosine(new Coordinate(origin), new Coordinate(destination));
        }

        /// <summary>
        /// Returns the distance in miles or kilometers of any two latitude / longitude points.
        /// </summary>
        /// <param name="origin">The start latitude and longitude </param>
        /// <param name="destination">The destination latitude and longitude </param>
        /// <returns>Distance in kilometers</returns>
        public static double DistanceCosine(double[] origin, double[] destination)
        {
            return DistanceCosine(new Coordinate(origin), new Coordinate(destination));
        }

        /// <summary>
        /// Returns the distance in miles or kilometers of any two latitude / longitude points.
        /// </summary>
        /// <param name="origin">The start Coordinate</param>
        /// <param name="destination">The destination Coordinate</param>
        /// <returns>Distance in kilometers</returns>
        public static double DistanceCosine(Coordinate origin, Coordinate destination)
        {
            double lat1 = ConvertDegreesToRadians(destination.Latitude);
            double lat2 = ConvertDegreesToRadians(origin.Latitude);
            double dLon = ConvertDegreesToRadians(destination.Longitude - origin.Longitude);

            return Math.Acos(Math.Sin(lat1) * Math.Sin(lat2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Cos(dLon)) * EarthMeanRadiusKilometres;
        }

        /// <summary>
        /// Returns the distance in miles or kilometers of any two latitude / longitude points.
        /// </summary>
        /// <param name="origin">The start api object </param>
        /// <param name="destination">The destination api object</param>
        /// <returns>Distance in kilometers</returns>
        public static double DistanceHaversine(dynamic origin, dynamic destination)
        {
            return DistanceHaversine(new Coordinate(origin), new Coordinate(destination));
        }

        /// <summary>
        /// Returns the distance in miles or kilometers of any two latitude / longitude points.
        /// </summary>
        /// <param name="origin">The start latitude and longitude </param>
        /// <param name="destination">The destination latitude and longitude </param>
        /// <returns>Distance in kilometers</returns>
        public static double DistanceHaversine(double[] origin, double[] destination)
        {
            return DistanceHaversine(new Coordinate(origin), new Coordinate(destination));
        }

        /// <summary>
        /// Returns the distance in miles or kilometers of any two latitude / longitude points.
        /// </summary>
        /// <param name="origin">The start Coordinate</param>
        /// <param name="destination">The destination Coordinate</param>
        /// <returns>Distance in kilometers</returns>
        public static double DistanceHaversine(Coordinate origin, Coordinate destination)
        {
            var lat = ConvertDegreesToRadians(destination.Latitude - origin.Latitude);
            var lng = ConvertDegreesToRadians(destination.Longitude - origin.Longitude);
            var h1 = Math.Sin(lat / 2) * Math.Sin(lat / 2) +
                          Math.Cos(ConvertDegreesToRadians(origin.Latitude)) * Math.Cos(ConvertDegreesToRadians(destination.Latitude)) *
                          Math.Sin(lng / 2) * Math.Sin(lng / 2);
            var h2 = 2 * Math.Asin(Math.Min(1, Math.Sqrt(h1)));

            return EarthMeanRadiusKilometres * h2;
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
        ///  Normalises a latitude to the [-90,90] range.
        ///  Latitudes above 90 or below -90 are constrained rather than wrapped.
        /// </summary>
        /// <param name="latitude">The latitude in degrees to normalize.</param>
        /// <returns>Latitude within the [-90,90] range</returns>
        public static double FixLatitude(double latitude)
        {
            return Math.Max(-90, Math.Min(90, latitude));
        }

        /// <summary>
        /// Normalises a longitude to the [-180,180] range.
        /// Longitudes above 180 or below -180 are wrapped.
        /// If the wrapped value is exactly equal to min or max, favors max, unless favorMin is true.
        /// </summary>
        /// <param name="longitude">The longitude in degrees to normalise</param>
        /// <returns>Longitude within the [-180,180] range</returns>
        public static double FixLongitude(double longitude)
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
        /// <param name="min">The minimum bound of the range</param>
        /// <param name="max">The maximum bound of the range</param>
        /// <param name="favorMin">Whether or not to favor min over max in the case of ambiguity. Default is false</param>
        /// <returns>The value in the given range.</returns>
        public static double WrapValue(double value, double min, double max, bool favorMin = false)
        {
            // Don't wrap min as max.
            if (value == min)
            {
                return min;
            }

            // Normalize to min = 0.
            value -= min;

            value = value % (max - min);
            if (value < 0)
            {
                value += max - min;
            }

            // Reverse normalization.
            value += min;

            // When ambiguous (min or max), return max unless favorMin is true.
            return (value == min) ? (favorMin ? min : max) : value;
        }

        /// <summary>
        /// Constrains the given number to the given range.
        /// </summary>
        /// <param name="value">The value to wrap</param>
        /// <param name="min">The minimum bound of the range</param>
        /// <param name="max">The maximum bound of the range</param>
        /// <returns>The value constrained to the given range</returns>
        public static double ConstrainValue(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }
}
