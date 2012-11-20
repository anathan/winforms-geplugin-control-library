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

#region

using System;

#endregion

namespace FC.GEPluginCtrls.Geo
{
    ////using LatLng = System.Tuple<double, double>;

    /// <summary>
    /// Various Geodesic methods to work with the plugin api
    /// This class is based on the javascript library geojs by Roman Nurik
    /// See http://code.google.com/p/geojs/
    /// </summary>
    public static class Maths
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
        /// <param name="origin">the start Coordinate</param>
        /// <param name="destination">the destination Coordinate</param>
        /// <returns>The final bearing from start to destination</returns>
        /// <remarks>See: http://williams.best.vwh.net/avform.htm for the original function </remarks>
        public static double BearingFinal(Coordinate origin, Coordinate destination)
        {
            return (BearingInitial(destination, origin) + 180) % 360;
        }

        /// <summary>
        /// Get the inital bearing from one location to another
        /// </summary>
        /// <param name="origin">the starting location</param>
        /// <param name="destination">the destination location</param>>
        /// <remarks>See: http://williams.best.vwh.net/avform.htm for the original function </remarks>
        /// <returns>The inital bearing from origin to destination</returns>
        public static double BearingInitial(Coordinate origin, Coordinate destination)
        {
            double phi1 = ConvertDegreesToRadians(origin.Latitude);
            double phi2 = ConvertDegreesToRadians(destination.Latitude);
            double deltaLambda = ConvertDegreesToRadians(destination.Longitude - origin.Longitude);
            double y = Math.Sin(deltaLambda) * Math.Cos(phi2);
            double x = (Math.Cos(phi1) * Math.Sin(phi2)) -
                (Math.Sin(phi1) * Math.Cos(phi2) * Math.Cos(deltaLambda));
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
        /// <param name="radians">value in radians</param>
        /// <returns>value in degrees</returns>
        public static double ConvertRadiansToDegrees(double radians)
        {
            return radians == 0 ? radians : (radians / Math.PI * 180.0);
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
        /// Destination point given distance and bearing from start point
        /// </summary>
        /// <param name="origin">the start point</param>
        /// <param name="distance">the given distance in km or m</param>
        /// <param name="bearing">the bearing in radians, clockwise from north</param>
        /// <param name="units">The unit system to use, default is metric</param>
        /// <returns>destination location as a Tuple(double lat, double lng)</returns>
        public static Coordinate Destination(Coordinate origin, double distance, double bearing, UnitSystem units = UnitSystem.Metric)
        {
            double phi1 = ConvertDegreesToRadians(origin.Latitude);
            double lambda1 = ConvertDegreesToRadians(origin.Longitude);
            bearing = ConvertDegreesToRadians(bearing);

            double angularDistance;

            if (units == UnitSystem.Metric)
            {
                angularDistance = distance / EarthMeanRadiusKilometres;
            }
            else
            {
                distance = ConvertKilometresToMiles(distance);
                angularDistance = distance / EarthMeanRadiusMiles;
            }

            double phi2 = Math.Asin(Math.Sin(phi1) * Math.Cos(angularDistance) +
                Math.Cos(phi1) * Math.Sin(angularDistance) * Math.Cos(bearing));

            double lambda2 = lambda1 +
                Math.Atan2(
                Math.Sin(bearing) * Math.Sin(angularDistance) * Math.Cos(phi1),
                Math.Cos(angularDistance) - Math.Sin(phi1) * Math.Sin(phi2));

            return new Coordinate(ConvertRadiansToDegrees(phi2), ConvertRadiansToDegrees(lambda2));
        }

        /// <summary>
        /// Returns the distance in miles or kilometres of any two latitude / longitude points.
        /// </summary>
        /// <param name="origin">The start api object </param>
        /// <param name="destination">The destination api object</param>
        /// <param name="units">The unit system to use, default is metric</param>
        /// <returns>Distance in kilometres</returns>
        public static double DistanceCosine(dynamic origin, dynamic destination, UnitSystem units = UnitSystem.Metric)
        {
            return DistanceCosine(new Coordinate(origin), new Coordinate(destination), units);
        }

        /// <summary>
        /// Returns the distance in miles or kilometres of any two latitude / longitude points.
        /// </summary>
        /// <param name="origin">The start latitude and longitude </param>
        /// <param name="destination">The destination latitude and longitude </param>
        /// <param name="units">The unit system to use, default is metric</param>
        /// <returns>Distance in kilometres</returns>
        public static double DistanceCosine(Coordinate origin, Coordinate destination, UnitSystem units = UnitSystem.Metric)
        {
            double phi1 = ConvertDegreesToRadians(origin.Latitude);
            double phi2 = ConvertDegreesToRadians(destination.Latitude);
            double deltaLamdba = ConvertDegreesToRadians(destination.Longitude - origin.Longitude);
            double d = Math.Acos((Math.Sin(phi1) * Math.Sin(phi2)) + (Math.Cos(phi1) * Math.Cos(phi2) * Math.Cos(deltaLamdba)));

            if (units == UnitSystem.Metric)
            {
                return d * EarthMeanRadiusKilometres;
            }

            return d * EarthMeanRadiusMiles;
        }

        /// <summary>
        /// Returns the distance in miles or kilometres of any two latitude / longitude points.
        /// </summary>
        /// <param name="origin">The start latitude and longitude </param>
        /// <param name="destination">The destination latitude and longitude </param>
        /// <param name="units">The unit system to use, default is metric</param>
        /// <returns>Distance in kilometres</returns>
        public static double DistanceHaversine(Coordinate origin, Coordinate destination, UnitSystem units = UnitSystem.Metric)
        {
            double phi1 = ConvertDegreesToRadians(origin.Latitude);
            double phi2 = ConvertDegreesToRadians(destination.Latitude);
            double deltaPhi = ConvertDegreesToRadians(destination.Latitude - origin.Latitude); 
            double deltaLambda = ConvertDegreesToRadians(destination.Longitude - origin.Longitude);
            double h1 = (Math.Sin(deltaPhi / 2) * Math.Sin(deltaPhi / 2)) +
                (Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2) * Math.Cos(phi1) * Math.Cos(phi2));
            double d = 2 * Math.Asin(Math.Min(1, Math.Sqrt(h1)));

            if (units == UnitSystem.Metric)
            {
                return d * EarthMeanRadiusKilometres;
            }

            return d * EarthMeanRadiusMiles;
        }

        /// <summary>
        /// Returns the distance in miles or kilometres of any two latitude / longitude points.
        /// </summary>
        /// <param name="origin">The start latitude and longitude </param>
        /// <param name="destination">The destination latitude and longitude </param>
        /// <param name="units">The unit system to use, default is metric</param>
        /// <returns>Distance in kilometres</returns>
        public static double DistanceHaversine(dynamic origin, dynamic destination, UnitSystem units = UnitSystem.Metric)
        {
            return DistanceHaversine(new Coordinate(origin), new Coordinate(destination), units);
        }

        /// <summary>
        /// Keep a number in the [0,PI] range
        /// </summary>
        /// <param name="radians">value in radians</param>
        /// <returns>normalised angle in radians</returns>
        public static double NormalizeAngle(double radians)
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
            return NormalizeAngle(radians + Math.PI);
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