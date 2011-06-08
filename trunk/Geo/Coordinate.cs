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
namespace FC.GEPluginCtrls.Geo
{
    using System;

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
        /// <param name="apiObject">the api object to base the coordinate from</param>
        public Coordinate(dynamic apiObject)
            : this()
        {
            string type = string.Empty;

            try
            {
                type = apiObject.getType();
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                return;
            }

            // no need to normalise as the Coordinate is 
            // constructed from an existing api type (I think!)
            if (type == ApiType.KmlPlacemark)
            {
                apiObject = apiObject.getGeometry();
                this.Latitude = apiObject.getLatitude();
                this.Longitude = apiObject.getLongitude();
                this.Altitude = apiObject.getAltitude();
                this.AltitudeMode = (AltitudeMode)apiObject.getAltitudeMode();
                return;
            }
            else if (type == ApiType.KmlCoord || type == ApiType.KmlLocation)
            {
                this.Latitude = apiObject.getLatitude();
                this.Longitude = apiObject.getLongitude();
                this.Altitude = apiObject.getAltitude();
                return;
            }
            else if (type == ApiType.KmlPoint || type == ApiType.KmlLookAt)
            {
                this.Latitude = apiObject.getLatitude();
                this.Longitude = apiObject.getLongitude();
                this.Altitude = apiObject.getAltitude();
                this.AltitudeMode = (AltitudeMode)apiObject.getAltitudeMode();
                return;
            }

            throw new ArgumentException("Could not create a point from the given api object: " + type);
        }

        /// <summary>
        /// Initializes a new instance of the Coordinate class.
        /// </summary>
        /// <param name="coord">expects 2, 3 or 4 values
        /// [lat, lng] or
        /// [lat, lng, alt] or 
        /// [lat, lng, alt, (int)altMode]</param>
        public Coordinate(double[] coord)
            : this()
        {
            if (coord == null) 
            {
                return; 
            }

            switch (coord.Length)
            {
                case 2:
                    new Coordinate(Maths.FixLatitude(coord[0]), Maths.FixLongitude(coord[1]));
                    break;
                case 3:
                    new Coordinate(Maths.FixLatitude(coord[0]), Maths.FixLongitude(coord[1]), coord[2]);
                    break;
                case 4:
                    new Coordinate(Maths.FixLatitude(coord[0]), Maths.FixLongitude(coord[1]), coord[2], (AltitudeMode)coord[3]);
                    return;
                default:
                    throw new ArgumentException("Could not create a point from the parameter");
            }
        }

        /// <summary>
        /// Initializes a new instance of the Coordinate class.
        /// </summary>
        /// <param name="coord">the Coordinate to clone</param>
        public Coordinate(Coordinate coord)
            : this(coord.Latitude, coord.Longitude, coord.Altitude, coord.AltitudeMode)
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
        /// <param name="coord1">The first Coordinate</param>
        /// <param name="coord2">The Second Coordinate</param>
        /// <returns>True if the two coordinates are equal</returns>
        public static bool operator ==(Coordinate coord1, Coordinate coord2)
        {
            return coord1.Equals(coord2);
        }

        /// <summary>
        /// Coordinate inequality operator 
        /// </summary>
        /// <param name="coord1">The first Coordinate</param>
        /// <param name="coord2">The Second Coordinate</param>
        /// <returns>True if the two coordinates are unequal</returns>
        public static bool operator !=(Coordinate coord1, Coordinate coord2)
        {
            return !(coord1 == coord2);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the distance in km from one Coordinate to another
        /// </summary>
        /// <param name="destination">The end Coordinate</param>
        /// <param name="haversine">Optionally use the haversine formula, default is false</param>
        /// <remarks>by defaut simple spherical trig (law of cosines) is used</remarks>
        /// <returns>The distance between the two Coordinates in km</returns>
        public double Distance(Coordinate destination, bool haversine = false)
        {
            if (haversine)
            {
                return Maths.DistanceHaversine(this, destination);
            }
            else
            {
                return Maths.DistanceCosine(this, destination);
            }
        }

        /// <summary>
        /// Coordinate equality
        /// </summary>
        /// <param name="obj">the object to check against</param>
        /// <returns>true if the objects are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is Coordinate)
            {
                return this.Equals((Coordinate)obj);
            }

            return false;
        }

        /// <summary>
        /// Coordinate equality
        /// </summary>
        /// <param name="coord">the Coordinate to check against</param>
        /// <returns>true if the Coordinates are equal</returns>
        public bool Equals(Coordinate coord)
        {
            return this.Latitude == coord.Latitude &&
                this.Longitude == coord.Longitude &&
                this.Altitude == coord.Altitude &&
                this.AltitudeMode == coord.AltitudeMode;
        }

        /// <summary>
        /// Coordinate 2D equality
        /// </summary>
        /// <param name="coord">the Coordinate to check latitude and longitude against</param>
        /// <returns>True if the Coordinates are equal in latitude and longitude</returns>
        public bool Equals2D(Coordinate coord)
        {
            return this.Latitude == coord.Latitude && this.Longitude == coord.Longitude;
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
            return string.Format("({0}, {1}, {2})", this.Latitude, this.Longitude, this.Altitude);
        }

        #endregion
    }
}