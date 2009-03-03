// <copyright file="Maths.cs" company="FC">
// Copyright (c) 2008 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2009-03-02</date>
// <summary>This program is part of FC.GEPluginCtrls
// FC.GEPluginCtrls is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </summary>
namespace FC.GEPluginCtrls
{
    using System;
    using GEPlugin;

    /// <summary>
    /// Various Geodesic methods to work with the plugin api
    /// </summary>
    public static class Maths
    {

        /// <summary>
        /// Earth's radius in metres
        /// </summary>
        private const int EARTH_RADIUS = 6378135;

        /// <summary>
        /// Smallest significant value 
        /// </summary>
        private const double EPSILON = 0.0000000000001;

        /// <summary>
        /// Miles To Kilometers Conversion Ratio
        /// </summary>
        private const double MILES_TO_KILOMETRES = 0.621371192;

        /// <summary>
        /// Keep a Longitudinal angle in the [-180, 180] range
        /// </summary>
        /// <param name="angle">Longitude to fix</param>
        /// <returns>Longitude in range</returns>
        public static double FixLongitudinalAngle(this double angle)
        {
            while (angle < -180)
            {
                angle += 360;
            }

            while (angle > 180)
            {
                angle -= 360;
            }

            return angle;
        }

        /// <summary>
        /// Keep a Longitudinal angle in the [-90, 90] range
        /// </summary>
        /// <param name="angle">Longitude to fix</param>
        /// <returns>Longitude to fix</returns>
        public static double FixLatitudinalAngle(this double angle)
        {
            while (angle < -90)
            {
                angle += 90;
            }

            while (angle > 90)
            {
                angle -= 90;
            }

            return angle;
        }

        /// <summary>
        /// Converts decimal degrees to radians
        /// </summary>
        /// <param name="degrees">value in degrees</param>
        /// <returns>value in radians</returns>
        public static double DegreesToRadians(this double degrees)
        {
            return degrees == 0 ? degrees : (degrees * Math.PI / 180.0);
        }

        /// <summary>
        /// Converts radians to decimal degrees
        /// </summary>
        /// <param name="radains">value in radians</param>
        /// <returns>value in degrees</returns>
        public static double RadiansToDegrees(this double radains)
        {
            return radains == 0 ? radains : (radains / Math.PI * 180.0);
        }

        /// <summary>
        /// Convert Kilometers To Miles 
        /// </summary>
        /// <param name="kilometers">distance in kilometeres</param>
        /// <returns>distance in miles</returns>
        public static double KilometersToMiles(double kilometers)
        {
            return kilometers * MILES_TO_KILOMETRES;
        }

        /// <summary>
        /// Convert Miles To Kilometers
        /// </summary>
        /// <param name="miles">distance in miles</param>
        /// <returns>distance in kilometeres</returns>
        public static double MilesToKilometers(double miles)
        {
            return miles / MILES_TO_KILOMETRES;
        }

        /// <summary>
        /// Keep a number in the [0,PI] range
        /// </summary>
        /// <param name="radians">value in radians</param>
        /// <returns>normalised angle in radians</returns>
        public static double NormaliseAngle(this double radians)
        {
            radians = radians % (2 * Math.PI);
            return radians >= 0 ? radians : radians + 2 * Math.PI;
        }

        /// <summary>
        /// Reverses a number in the [0,PI] range
        /// </summary>
        /// <param name="radians">value in radians</param>
        /// <returns>The oposite angle</returns>
        public static double ReverseAngle(this double radians)
        {
            return NormaliseAngle(radians + Math.PI);
        }

        /// <summary>
        /// Computes the distance between two points using the Haversine formula
        /// </summary>
        /// <param name="origin">The first point</param>
        /// <param name="destination">The second point</param>
        /// <returns>The distance between the given points in metres</returns>
        public static double DistanceHaversine(IKmlPoint origin, IKmlPoint destination)
        {
            return EARTH_RADIUS * AngularDistance(origin, destination);
        }

        /// <summary>
        /// This function is based on the geodesy-library code by Mike Gavaghan 
        /// See http://www.gavaghan.org/blog/2007/08/06/c-gps-receivers-and-geocaching-vincentys-formula/
        /// </summary>
        /// <param name="origin">The first point</param>
        /// <param name="destination">The second point</param>
        /// <returns>The distance between the given points in metres</returns>
        public static double DistanceVincenty(IKmlPoint origin, IKmlPoint destination)
        {
            //
            // All equation numbers refer back to Vincenty's publication:
            // See http://www.ngs.noaa.gov/PUBS_LIB/inverse.pdf
            //

            // WGS84 Ellipsoid 
            double a = 6378137;
            double b = 6356752.3142;
            double f = 1 / 298.257223563;

            // get parameters as radians
            double phi1 = origin.getLatitude().DegreesToRadians();
            double phi2 = destination.getLatitude().DegreesToRadians();
            double lambda1 = origin.getLongitude().DegreesToRadians();
            double lambda2 = destination.getLongitude().DegreesToRadians();

            // calculations
            double a2 = a * a;
            double b2 = b * b;
            double a2b2b2 = (a2 - b2) / b2;

            double omega = lambda2 - lambda1;

            double tanphi1 = Math.Tan(phi1);
            double tanU1 = (1.0 - f) * tanphi1;
            double U1 = Math.Atan(tanU1);
            double sinU1 = Math.Sin(U1);
            double cosU1 = Math.Cos(U1);

            double tanphi2 = Math.Tan(phi2);
            double tanU2 = (1.0 - f) * tanphi2;
            double U2 = Math.Atan(tanU2);
            double sinU2 = Math.Sin(U2);
            double cosU2 = Math.Cos(U2);

            double sinU1sinU2 = sinU1 * sinU2;
            double cosU1sinU2 = cosU1 * sinU2;
            double sinU1cosU2 = sinU1 * cosU2;
            double cosU1cosU2 = cosU1 * cosU2;

            // eq. 13
            double lambda = omega;

            // intermediates we'll need to compute 's'
            double A = 0.0;
            double B = 0.0;
            double sigma = 0.0;
            double deltasigma = 0.0;
            double lambda0;

            for (int i = 0; i < 20; i++)
            {
                lambda0 = lambda;

                double sinlambda = Math.Sin(lambda);
                double coslambda = Math.Cos(lambda);

                // eq. 14
                double sin2sigma = (cosU2 * sinlambda * cosU2 * sinlambda) + Math.Pow(cosU1sinU2 - sinU1cosU2 * coslambda, 2.0);
                double sinsigma = Math.Sqrt(sin2sigma);

                // eq. 15
                double cossigma = sinU1sinU2 + (cosU1cosU2 * coslambda);

                // eq. 16
                sigma = Math.Atan2(sinsigma, cossigma);

                // eq. 17    Careful!  sin2sigma might be almost 0!
                double sinalpha = (sin2sigma == 0) ? 0.0 : cosU1cosU2 * sinlambda / sinsigma;
                double alpha = Math.Asin(sinalpha);
                double cosalpha = Math.Cos(alpha);
                double cos2alpha = cosalpha * cosalpha;

                // eq. 18    Careful!  cos2alpha might be almost 0!
                double cos2sigmam = cos2alpha == 0.0 ? 0.0 : cossigma - 2 * sinU1sinU2 / cos2alpha;
                double u2 = cos2alpha * a2b2b2;

                double cos2sigmam2 = cos2sigmam * cos2sigmam;

                // eq. 3
                A = 1.0 + u2 / 16384 * (4096 + u2 * (-768 + u2 * (320 - 175 * u2)));

                // eq. 4
                B = u2 / 1024 * (256 + u2 * (-128 + u2 * (74 - 47 * u2)));

                // eq. 6
                deltasigma = B * sinsigma * (cos2sigmam + B / 4 * (cossigma * (-1 + 2 * cos2sigmam2) - B / 6 * cos2sigmam * (-3 + 4 * sin2sigma) * (-3 + 4 * cos2sigmam2)));

                // eq. 10
                double C = f / 16 * cos2alpha * (4 + f * (4 - 3 * cos2alpha));

                // eq. 11 (modified)
                lambda = omega + (1 - C) * f * sinalpha * (sigma + C * sinsigma * (cos2sigmam + C * cossigma * (-1 + 2 * cos2sigmam2)));

                // see how much improvement we got
                double change = Math.Abs((lambda - lambda0) / lambda);

                if ((i > 1) && (change < EPSILON))
                {
                    break;
                }
            }

            // eq. 19
            double s = b * A * (sigma - deltasigma);

            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static double AngularDistance(IKmlPoint point1, IKmlPoint point2)
        {
            double phi1 = point1.getLatitude().DegreesToRadians();
            double phi2 = point2.getLatitude().DegreesToRadians();
            double d_phi = (point2.getLatitude() - point1.getLatitude()).DegreesToRadians();
            double d_lmd = (point2.getLongitude() - point1.getLongitude()).DegreesToRadians();
            double A = Math.Pow(Math.Sin(d_phi / 2), 2) +
                Math.Cos(phi1) * Math.Cos(phi2) *
                Math.Pow(Math.Sin(d_lmd / 2), 2);

            return 2 * Math.Atan2(Math.Sqrt(A), Math.Sqrt(1 - A));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static double Heading(IKmlPoint origin, IKmlPoint destination)
        {
            double phi1 = origin.getLatitude().DegreesToRadians();
            double phi2 = destination.getLatitude().DegreesToRadians();
            double cos_phi2 = Math.Cos(phi2);
            double d_lmd = (destination.getLongitude() - origin.getLongitude()).DegreesToRadians();

            return NormaliseAngle(
                Math.Atan2(Math.Sin(d_lmd) * cos_phi2,
                Math.Cos(phi1) * Math.Sin(phi2) - Math.Sin(phi1) *
                cos_phi2 * Math.Cos(d_lmd))).RadiansToDegrees();
        }

        /// <summary>
        /// Calculates an intermediate point on the geodesic between the two given points 
        /// See: http://williams.best.vwh.net/avform.htm#Intermediate
        /// </summary>
        /// <param name="origin">The first point</param>
        /// <param name="destination">The second point</param>
        /// <param name="fraction">Intermediate location as a decimal fraction (T value)</param>
        /// <returns></returns>
        public static IKmlPoint IntermediatePoint(IKmlPoint origin, IKmlPoint destination, double fraction)
        {
            if (fraction > 1 || fraction < 0)
            {
                throw new ArgumentOutOfRangeException("fraction must be between 0 and 1");
            }

            // TODO: check for antipodality and fail w/ exception in that case 
            double phi1 = origin.getLatitude().DegreesToRadians();
            double phi2 = destination.getLatitude().DegreesToRadians();
            double lmd1 = origin.getLongitude().DegreesToRadians();
            double lmd2 = destination.getLongitude().DegreesToRadians();

            double cos_phi1 = Math.Cos(phi1);
            double cos_phi2 = Math.Cos(phi2);
            double angularDistance = AngularDistance(origin, destination);
            double sin_angularDistance = Math.Sin(angularDistance);

            double A = Math.Sin((1 - fraction) * angularDistance) / sin_angularDistance;
            double B = Math.Sin(fraction * angularDistance) / sin_angularDistance;
            double x = A * cos_phi1 * Math.Cos(lmd1) + B * cos_phi2 * Math.Cos(lmd2);
            double y = A * cos_phi1 * Math.Sin(lmd1) + B * cos_phi2 * Math.Sin(lmd2);
            double z = A * Math.Sin(phi1) + B * Math.Sin(phi2);

            IKmlPoint result = origin;
            result.set(0, 0, 0, 0, 0, 0);
            result.setLatLng(Math.Atan2(z, Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2))).RadiansToDegrees(),
                Math.Atan2(y, x).RadiansToDegrees());

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin">The first point</param>
        /// <param name="heading"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static IKmlPoint Destination(IKmlPoint origin, double heading, double distance)
        {
            double phi1 = origin.getLatitude().DegreesToRadians();
            double lmd1 = origin.getLongitude().DegreesToRadians();

            double sin_phi1 = Math.Sin(phi1);
            double angularDistance = distance / EARTH_RADIUS;
            double heading_rad = heading.DegreesToRadians();
            double sin_angularDistance = Math.Sin(angularDistance);
            double cos_angularDistance = Math.Cos(angularDistance);

            double phi2 =
                Math.Asin(sin_phi1 * cos_angularDistance + Math.Cos(phi1) *
                sin_angularDistance * Math.Cos(heading_rad));

            IKmlPoint result = origin;
            result.set(0, 0, 0, 0, 0, 0);
            result.setLatLng(phi2.RadiansToDegrees(),
                Math.Atan2(Math.Sin(heading_rad) * sin_angularDistance * Math.Cos(phi2),
                cos_angularDistance - sin_phi1 * Math.Sin(phi2)).RadiansToDegrees() + origin.getLongitude());

            return result;
        }

    }
}

