// <copyright file="Coordinate.cs" company="FC">
// Copyright (c) 2011 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2011-03-25</date>
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
using System.Globalization;

#endregion

namespace FC.GEPluginCtrls.Geo
{
    /// <summary>
    /// Coordinate class
    /// </summary>
    /// <remarks>
    /// This class is based on the point object in the geojs library
    /// http://code.google.com/p/geojs/wiki/GeoMathReference
    /// </remarks>
    public class Coordinate : IEquatable<Coordinate>
    {
        /// <summary>
        /// Initializes a new instance of the Coordinate class.
        /// </summary>
        /// <param name="latitude">the Coordinate latitude</param>
        /// <param name="longitude">the Coordinate longitude</param>
        /// <param name="altitude">the Coordinate altitude</param>
        /// <param name="altitudeMode">the Coordinate altitudeMode</param>
        public Coordinate(
            double latitude = 0,
            double longitude = 0,
            double altitude = 0,
            AltitudeMode altitudeMode = AltitudeMode.RelativeToGround)
        {
            this.Latitude = Maths.FixLatitude(latitude);
            this.Longitude = Maths.FixLongitude(longitude);
            this.Altitude = altitude;
            this.AltitudeMode = altitudeMode;
        }

        /// <summary>
        /// Initializes a new instance of the Coordinate class.
        /// </summary>
        /// <param name="feature">the api object to base the coordinate on.
        /// This should be a KmlPoint, KmlCoord, KmlLocation, KmlLookAt or KmlCamera (...or a simple placemark containing a point)</param>
        public Coordinate(dynamic feature)
            : this()
        {
            switch ((ApiType)GEHelpers.GetApiType(feature))
            {
                case ApiType.KmlPlacemark:
                    {
                        dynamic geometry = feature.getGeometry();

                        if (GEHelpers.IsApiType(geometry, ApiType.KmlPoint))
                        {
                            feature = geometry;
                            goto case ApiType.KmlPoint;
                        }
                    }

                    return;

                case ApiType.KmlCoord:
                case ApiType.KmlLocation:
                    {
                        this.Latitude = feature.getLatitude();
                        this.Longitude = feature.getLongitude();
                        this.Altitude = feature.getAltitude();
                    }

                    return;
                case ApiType.KmlPoint:
                case ApiType.KmlLookAt:
                case ApiType.KmlCamera:
                    {
                        this.Latitude = feature.getLatitude();
                        this.Longitude = feature.getLongitude();
                        this.Altitude = feature.getAltitude();
                        this.AltitudeMode = (AltitudeMode)feature.getAltitudeMode();
                    }

                    return;
            }
        }

        /// <summary>
        /// Initializes a new instance of the Coordinate class.
        /// </summary>
        /// <param name="coordinate">expects 2, 3 or 4 values
        /// [lat, lng] or
        /// [lat, lng, alt] or 
        /// [lat, lng, alt, (int)altMode]</param>
        public Coordinate(double[] coordinate)
            : this()
        {
            if (coordinate == null)
            {
                return;
            }

            switch (coordinate.Length)
            {
                case 4:
                    {
                        this.AltitudeMode = (AltitudeMode)coordinate[3];
                        goto case 3; // really!?
                    }

                case 3:
                    {
                        this.Altitude = coordinate[2];
                        goto case 2; // yes, to get statement fallthrough...
                    }

                case 2:
                    {
                        this.Latitude = Maths.FixLatitude(coordinate[0]);
                        this.Longitude = Maths.FixLongitude(coordinate[1]);
                    }

                    return;

                default:
                    throw
                        new ArgumentException("Could not create coordinate. " +
                        "The coordinate parameter array must have a length of 2, 3 or 4");
            }
        }

        /// <summary>
        /// Initializes a new instance of the Coordinate class.
        /// </summary>
        /// <param name="coordinate">the Coordinate to clone</param>
        /// <remarks>clones a coordinate</remarks>
        public Coordinate(Coordinate coordinate)
            : this(coordinate.Latitude, coordinate.Longitude, coordinate.Altitude, coordinate.AltitudeMode)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the Coordinate class.
        /// </summary>
        /// <param name="coordinate">A Tuple to base the Coordinate on (lat, lng, alt, mode)</param>
        public Coordinate(Tuple<double, double, double, int> coordinate)
            : this(coordinate.Item1, coordinate.Item2, coordinate.Item3, (AltitudeMode)coordinate.Item4)
        { 
        }

        #region Public properties

        /// <summary>
        /// Gets or sets the Coordinate Latitude
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the Coordinate Longitude
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the Coordinate Altitude
        /// </summary>
        public double Altitude { get; set; }

        /// <summary>
        /// Gets or sets the Coordinate AltitudeMode
        /// </summary>
        public AltitudeMode AltitudeMode { get; set; }

        /// <summary>
        /// Gets a value indicating whether the Coordinate is Three Dimensional
        /// </summary>
        public bool Is3D
        { 
            get { return this.Altitude != 0; } 
        }

        #endregion

        #region Operators

        /// <summary>
        /// Coordinate equality operator 
        /// </summary>
        /// <param name="coordinate1">The first Coordinate</param>
        /// <param name="coordinate2">The Second Coordinate</param>
        /// <returns>True if the two coordinates are equal</returns>
        public static bool operator ==(Coordinate coordinate1, Coordinate coordinate2)
        {
            return coordinate1.Equals(coordinate2);
        }

        /// <summary>
        /// Coordinate inequality operator 
        /// </summary>
        /// <param name="coordinate1">The first Coordinate</param>
        /// <param name="coordinate2">The Second Coordinate</param>
        /// <returns>True if the two coordinates are unequal</returns>
        public static bool operator !=(Coordinate coordinate1, Coordinate coordinate2)
        {
            return !(coordinate1 == coordinate2);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Find the destination point given distance and bearing from start point
        /// </summary>
        /// <param name="distance">the given distance in km or m</param>
        /// <param name="bearing">the bearing in radians, clockwise from north</param>
        /// <param name="units">The unit system to use, default is metric</param>
        /// <returns>The destination location as a Coordinate</returns>
        public Coordinate Destination(double distance, double bearing, UnitSystem units = UnitSystem.Metric)
        {
            return new Coordinate(Maths.Destination(this, distance, bearing, units));
        }

        /// <summary>
        /// Gets the distance in km from one Coordinate to another
        /// </summary>
        /// <param name="destination">The end Coordinate</param>
        /// <param name="haversine">Optionally use the haversine formula, default is false</param>
        /// <param name="units">The unit system to use, default is metric</param>
        /// <remarks>by defaut simple spherical trig (law of cosines) is used</remarks>
        /// <returns>The distance between the two Coordinates in km</returns>
        public double Distance(Coordinate destination, bool haversine = false, UnitSystem units = UnitSystem.Metric)
        {
            if (haversine)
            {
                return Maths.DistanceHaversine(this, destination, units);
            }
            
            return Maths.DistanceCosine(this, destination, units);
        }

        /// <summary>
        /// Coordinate equality
        /// </summary>
        /// <param name="obj">the object to check against</param>
        /// <returns>True if the objects are equal</returns>
        public override bool Equals(object obj)
        {
            Coordinate other = obj as Coordinate;

            if (other != null)
            {
                return this.Equals(other);
            }

            return false;
        }

        /// <summary>
        /// Coordinate equality
        /// </summary>
        /// <param name="other">the Coordinate to check against</param>
        /// <returns>True if the Coordinates are equal</returns>
        public bool Equals(Coordinate other)
        {
            return this.Latitude == other.Latitude &&
                this.Longitude == other.Longitude &&
                this.Altitude == other.Altitude &&
                this.AltitudeMode == other.AltitudeMode;
        }

        /// <summary>
        /// Coordinate 2D equality
        /// </summary>
        /// <param name="coordinate">the Coordinate to check latitude and longitude against</param>
        /// <returns>True if the Coordinates are equal in latitude and longitude</returns>
        public bool Equals2D(Coordinate coordinate)
        {
            return this.Latitude == coordinate.Latitude && this.Longitude == coordinate.Longitude;
        }

        /// <summary>
        /// Removes any altitude data from a Coordinate
        /// </summary>
        /// <returns>a Coordinate with no altitude data</returns>
        public Coordinate Flatten()
        {
            return new Coordinate(this.Latitude, this.Longitude);
        }

        /// <summary>
        /// Computes the final bearing to a Coordinate
        /// </summary>
        /// <param name="destination">the destination Coordinate</param>
        /// <returns>The final bearing to the destination Coordinate</returns>
        public double BearingFinal(Coordinate destination)
        {
            return Maths.BearingFinal(this, destination);
        }

        /// <summary>
        /// Computes the inital bearing to a Coordinate
        /// </summary>
        /// <param name="destination">the destination Coordinate</param>
        /// <returns>The inital bearing to the destination Coordinate</returns>
        public double BearingInitial(Coordinate destination)
        {
            return Maths.BearingInitial(this, destination);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            int hash = 23;

            hash = ((hash << 5) * 37) ^ this.Latitude.GetHashCode();
            hash = ((hash << 5) * 37) ^ this.Longitude.GetHashCode();
            hash = ((hash << 5) * 37) ^ this.Altitude.GetHashCode();
            hash = ((hash << 5) * 37) ^ this.AltitudeMode.GetHashCode();

            return hash;
        }

        /// <summary>
        /// Returns a string that represents the current Coordinate (lat, lng, alt)
        /// </summary>
        /// <returns> (lat, lng, alt)</returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "({0}, {1}, {2})",
                this.Latitude,
                this.Longitude,
                this.Altitude);
        }

        #endregion
    }
}