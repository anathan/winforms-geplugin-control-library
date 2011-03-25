// <copyright file="Bounds.cs" company="FC">
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
    /// Defines bounding objects
    /// </summary>
    public class Bounds : IEquatable<Bounds>
    {
        /// <summary>
        /// Initializes a new instance of the Bounds class.
        /// </summary>
        public Bounds()
        {
            this.NorthEast = new Coordinate();
            this.SouthWest = new Coordinate();
        }

        /// <summary>
        /// Initializes a new instance of the Bounds class.
        /// Single Coordinate constructor
        /// </summary>
        /// <param name="coord">the southwest/northEast Coordinate</param>
        public Bounds(Coordinate coord) :
            this()
        {
            this.SouthWest = this.NorthEast = coord;
        }

        /// <summary>
        /// Initializes a new instance of the Bounds class.
        /// Dual Coordinate constructor
        /// </summary>
        /// <param name="southwest">The southwest Coordinate</param>
        /// <param name="northEast">The northEast Coordinate</param>
        public Bounds(Coordinate southwest, Coordinate northEast)
            : this()
        {
            this.NorthEast = northEast;
            this.SouthWest = southwest;

            if (this.SouthWest.Latitude > this.NorthEast.Latitude)
            {
                throw new ArgumentException("Bounds southwest coordinate cannot be north of the northeast coordinate");
            }

            if (this.SouthWest.Altitude > this.NorthEast.Altitude)
            {
                throw new ArgumentException("Bounds southwest coordinate cannot be north of the northeast coordinate");
            }
        }

        /// <summary>
        /// Initializes a new instance of the Bounds class.
        /// Bounds clone constructor
        /// </summary>
        /// <param name="bounds">The bounds object to copy</param>
        public Bounds(Bounds bounds)
            : this()
        {
            this.NorthEast = bounds.NorthEast;
            this.SouthWest = bounds.SouthWest;
        }

        /// <summary>
        /// Initializes a new instance of the Bounds class.
        /// </summary>
        /// <param name="southWest">southeast coordinate [lat, lng]</param>
        /// <param name="northEast">northwest coordinate [lat, lng]</param>
        public Bounds(double[] southWest, double[] northEast) :
            this(new Coordinate(southWest), new Coordinate(northEast))
        {
        }

        #region Public Properties

        /// <summary>
        /// Gets the bounds' north coordinate.
        /// </summary>
        public double North
        {
            get { return !this.IsEmpty ? this.NorthEast.Latitude : 0; }
        }

        /// <summary>
        /// Gets the bounds' east coordinate.
        /// </summary>
        public double East
        {
            get { return !this.IsEmpty ? this.NorthEast.Longitude : 0; }
        }

        /// <summary>
        /// Gets the bounds' west coordinate.
        /// </summary>
        public double West
        {
            get { return !this.IsEmpty ? this.SouthWest.Longitude : 0; }
        }

        /// <summary>
        /// Gets the bounds' south coordinate.
        /// </summary>
        public double South
        {
            get { return !this.IsEmpty ? this.SouthWest.Latitude : 0; }
        }

        /// <summary>
        /// Gets the bounds' maximum altitude.
        /// </summary>
        public double Top
        {
            get { return !this.IsEmpty ? this.NorthEast.Altitude : 0; }
        }

        /// <summary>
        /// Gets the bounds' minimum altitude.
        /// </summary>
        public double Bottom
        {
            get { return !this.IsEmpty ? this.SouthWest.Altitude : 0; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the bounds have an altitude component.
        /// </summary>
        public bool Is3D
        {
            get
            {
                return !this.IsEmpty && (this.SouthWest.Is3D || this.NorthEast.Is3D);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the bounds is empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return (this.SouthWest.Latitude == 0 && this.SouthWest.Longitude == 0) &&
                    this.SouthWest.Equals(this.NorthEast);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the bounds occupy the entire latitudinal range.
        /// </summary>
        public bool IsFullLatitude
        {
            get { return this.South == -90 && this.North == 90; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the bounds occupy the entire longitudinal range.
        /// </summary>
        public bool IsFullLongitude
        {
            get { return this.West == -180 && this.East == 180; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the bounds intersect the antimeridian.
        /// </summary>
        public bool CrossesAntimeridian
        {
            get
            {
                return this.SouthWest.Longitude > this.NorthEast.Longitude;
            }
        }

        /// <summary>
        /// Gets or sets the Bounds NorthEast Coordinate
        /// </summary>
        public Coordinate NorthEast { get; set; }

        /// <summary>
        /// Gets or sets the Bounds SouthWest Coordinate
        /// </summary>
        public Coordinate SouthWest { get; set; }

        #endregion

        #region operators

        /// <summary>
        /// Determines whether the specified Bounds objects are equal.
        /// </summary>
        /// <param name="bounds1">The first Bounds object</param>
        /// <param name="bounds2">The second Bounds object</param>
        /// <returns>true if the specified Bounds objects are equal</returns>
        public static bool operator ==(Bounds bounds1, Bounds bounds2)
        {
            return bounds1.Equals(bounds2);
        }

        /// <summary>
        /// Determines whether the specified Bounds objects are unequal.
        /// </summary>
        /// <param name="bounds1">The first Bounds object</param>
        /// <param name="bounds2">The second Bounds object</param>
        /// <returns>true if the specified Bounds objects are unequal</returns>
        public static bool operator !=(Bounds bounds1, Bounds bounds2)
        {
            return !(bounds1 == bounds2);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns whether or not the given Coordinate is inside the Bounds.
        /// </summary>
        /// <param name="coord">The Coordinate to test</param>
        /// <returns>True if the Coordinate is inside the Bounds</returns>
        public bool ContainsCoordinate(Coordinate coord)
        {
            // check latitude
            if (!(this.South <= coord.Latitude && coord.Latitude <= this.North))
            {
                return false;
            }

            // check altitude
            if (this.Is3D &&
                !(this.Bottom <= coord.Altitude && coord.Altitude <= this.Top))
            {
                return false;
            }

            // check longitude
            return this.ContainsLongitude(coord.Longitude);
        }

        /// <summary>
        /// Gets the center of the Bounds.
        /// </summary>
        /// <returns>The center of the Bounds as a Coordinate</returns>
        public Coordinate Center()
        {
            if (this.IsEmpty)
            {
                return new Coordinate();
            }

            double latitude = (this.SouthWest.Latitude + this.NorthEast.Latitude) / 2;
            double longitude = this.CrossesAntimeridian ?
                    Maths.FixLongitude((this.SouthWest.Longitude + this.LongitudinalSpan(this.SouthWest.Longitude, this.NorthEast.Longitude)) / 2)
                        : (this.SouthWest.Longitude + this.NorthEast.Longitude) / 2;
            double altitude = (this.SouthWest.Altitude + this.NorthEast.Altitude) / 2;

            return new Coordinate(latitude, longitude, altitude);
        }

        /// <summary>
        /// Returns the Bounds' latitude, longitude, and altitude span as a Coordinate
        /// </summary>
        /// <returns>A Coordinate object containing the lat, lng and alt of the Bounds span</returns>
        public Coordinate Span()
        {
            if (this.IsEmpty)
            {
                return new Coordinate();
            }

            return new Coordinate(
                latitude: this.NorthEast.Latitude - this.SouthWest.Latitude,
                longitude: this.LongitudinalSpan(this.SouthWest.Longitude, this.NorthEast.Longitude),
                altitude: this.Is3D ? this.NorthEast.Altitude - this.SouthWest.Altitude : 0);
        }

        /// <summary>
        /// Extends the bounds object by the given Coordinate, 
        /// if the bounds don't already contain the Coordinate.
        /// Longitudinally, the bounds will be extended either east or west, 
        /// whichever results in a smaller longitudinal span.
        /// </summary>
        /// <param name="coord">The Coordinate to extend the bounds by.</param>
        public void Extend(Coordinate coord)
        {
            if (this.ContainsCoordinate(coord))
            {
                return;
            }

            if (this.IsEmpty)
            {
                this.SouthWest = this.NorthEast = coord;
                return;
            }

            // extend up or down
            double newBottom = this.Bottom;
            double newTop = this.Top;

            if (this.Is3D)
            {
                newBottom = Math.Min(newBottom, coord.Altitude);
                newTop = Math.Max(newTop, coord.Altitude);
            }

            // extend north or south
            double newSouth = Math.Min(this.South, coord.Latitude);
            double newNorth = Math.Max(this.North, coord.Latitude);

            double newWest = this.West;
            double newEast = this.East;

            if (!this.ContainsLongitude(coord.Longitude))
            {
                // try extending east and try extending west, and use the one that
                // has the smaller longitudinal span
                double extendEastLngSpan = this.LongitudinalSpan(newWest, coord.Longitude);
                double extendWestLngSpan = this.LongitudinalSpan(coord.Longitude, newEast);

                if (extendEastLngSpan <= extendWestLngSpan)
                {
                    newEast = coord.Longitude;
                }
                else
                {
                    newWest = coord.Longitude;
                }
            }

            // update the bounds' coordinates
            this.SouthWest = new Coordinate(newSouth, newWest, newBottom);
            this.NorthEast = new Coordinate(newNorth, newEast, newTop);
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current Bounds object.
        /// </summary>
        /// <param name="obj">The System.Object to compare with the current Bounds object.</param>
        /// <returns>true if the specified System.Object is equal to the current Bounds object</returns>
        public override bool Equals(object obj)
        {
            if (obj is Bounds)
            {
                return this.Equals((Bounds)obj);
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified Bounds objects are equal.
        /// </summary>
        /// <param name="bounds">The Bounds object to compare with the current Bounds object.</param>
        /// <returns>true if the specified Bounds object is equal to the current Bounds object</returns>
        public bool Equals(Bounds bounds)
        {
            return this.NorthEast == bounds.NorthEast && this.SouthWest == bounds.SouthWest;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code for this instance.</returns>
        public override int GetHashCode()
        {
            int hash = 23;

            hash = ((hash << 5) * 37) ^ this.SouthWest.GetHashCode();
            hash = ((hash << 5) * 37) ^ this.NorthEast.GetHashCode();

            return hash;
        }

        /// <summary>
        /// Returns a string that represents the current Bounds [(ne.lat, ne.lng, ne.alt), (sw.lat, sw.lng, sw.alt)]
        /// </summary>
        /// <returns>[(ne.lat, ne.lng, ne.alt), (sw.lat, sw.lng, sw.alt)]</returns>
        public override string ToString()
        {
            return string.Format("[{0}, {1}]", this.NorthEast.ToString(), this.SouthWest.ToString());
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns whether or not the given line of longitude is inside the Bounds.
        /// </summary>
        /// <param name="longitude">The longitude to test.</param>
        /// <returns>True if the longitude is within the bounds</returns>
        private bool ContainsLongitude(double longitude)
        {
            if (this.CrossesAntimeridian)
            {
                return longitude <= this.East || longitude >= this.West;
            }
            else
            {
                return this.West <= longitude && longitude <= this.East;
            }
        }

        /// <summary>
        /// Gets the longitudinal span of the given west and east decimal coordinates.
        /// </summary>
        /// <param name="west">the west coordinate</param>
        /// <param name="east">the east coordinate</param>
        /// <returns>the longitudinal span of the given west and east coordinates</returns>
        private double LongitudinalSpan(double west, double east)
        {
            return (west > east) ? (east + 360 - west) : (east - west);
        }

        #endregion
    }
}