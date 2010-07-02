// <copyright file="Maths.cs" company="FC">
// Copyright (c) 2008 Fraser Chapman
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
namespace FC.GEPluginCtrls
{
    using System;

    /// <summary>
    /// Various Geodesic methods to work with the plugin api
    /// This class is based on the javascript library geojs by Roman Nurik
    /// See http://code.google.com/p/geojs/
    /// </summary>
    public static class Maths
    {
        /// <summary>
        /// List of Cardinal comapass point names
        /// </summary>
        public static readonly string[] CardinalWords = new string[]
        { 
            "North", "North-East",
            "East", "South-East",
            "South", "South-West",
            "West", "North-West "
        };

        /// <summary>
        /// Earth's radius in metres
        /// </summary>
        private const int EarthRadius = 6378135;

        /// <summary>
        /// Smallest significant value 
        /// </summary>
        private const double Epslilon = 0.0000000000001;

        /// <summary>
        /// Miles To Kilometres Conversion Ratio
        /// </summary>
        private const double MilesToKilometres = 0.621371192;

        /// <summary>
        /// Calculates the angular distance between two points
        /// </summary>
        /// <param name="origin">The first point</param>
        /// <param name="destination">The second point</param>
        /// <returns>The distance between the points</returns>
        public static double AngularDistance(double[] origin, double[] destination)
        {
            double phi1 = ConvertDegreesToRadians(origin[0]);
            double phi2 = ConvertDegreesToRadians(destination[0]);
            double dphi = ConvertDegreesToRadians(destination[0] - origin[0]);
            double dlambda = ConvertDegreesToRadians(destination[1] - origin[1]);
            double a = Math.Pow(Math.Sin(dphi / 2), 2) +
                Math.Cos(phi1) * Math.Cos(phi2) *
                Math.Pow(Math.Sin(dlambda / 2), 2);

            return 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        /// <summary>
        /// Calculates the angular distance between two points
        /// </summary>
        /// <param name="origin">The first point</param>
        /// <param name="destination">The second point</param>
        /// <returns>The distance between the points</returns>
        public static double AngularDistance(object origin, object destination)
        {
            return AngularDistance(PointToDouble(origin), PointToDouble(destination));
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
        /// Converts a heading in the range [-180,180] to a bearing in the range [0,360] 
        /// </summary>
        /// <param name="heading">heading in the range [-180,180]</param>
        /// <returns>bearing in the range [0,360]</returns>
        public static double ConvertHeadingToBearing(double heading)
        {
            return heading <= 0 ? 360 - (-heading % 360) : heading % 360;
        }

        /// <summary>
        /// Converts a heading in the range [-180,180] to the corresponding cardinal direction
        /// </summary>
        /// <param name="heading">decimal heading</param>
        /// <returns>The cardinal point</returns>
        public static string ConvertHeadingToCardinal(double heading)
        {
            return ConvertHeadingToCardinal(heading, CardinalWords);
        }

        /// <summary>
        /// Converts a heading in the range [-180,180] to the corresponding cardinal direction
        /// </summary>
        /// <param name="heading">decimal heading</param>
        /// <param name="points">A list of cardinal names</param>
        /// <returns>The cardinal point</returns>
        public static string ConvertHeadingToCardinal(double heading, string[] points)
        {
            int c = points.Length;
            return points[(int)((Math.Round(Maths.ConvertHeadingToBearing(heading) / 360, 2) * c) + 0.5) % c];
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
        /// Calculates the destination point along a geodesic, given an initial point, heading and distance
        /// see http://www.movable-type.co.uk/scripts/latlong.html
        /// </summary>
        /// <param name="origin">The first point</param>
        /// <param name="heading">heading in degrees</param>
        /// <param name="distance">distance in metres</param>
        /// <returns>The point at the location along the geodesic</returns>
        public static double[] Destination(double[] origin, double heading, double distance)
        {
            double phi1 = ConvertDegreesToRadians(origin[0]);
            double lambda1 = ConvertDegreesToRadians(origin[1]);

            double sin_phi1 = Math.Sin(phi1);
            double angularDistance = distance / EarthRadius;
            double heading_rad = ConvertDegreesToRadians(heading);
            double sin_angularDistance = Math.Sin(angularDistance);
            double cos_angularDistance = Math.Cos(angularDistance);

            double phi2 =
                Math.Asin(sin_phi1 * cos_angularDistance + Math.Cos(phi1) *
                sin_angularDistance * Math.Cos(heading_rad));

            return new double[] 
            {
                ConvertRadiansToDegrees(phi2),
                ConvertRadiansToDegrees(Math.Atan2(Math.Sin(heading_rad) * sin_angularDistance * Math.Cos(phi2), cos_angularDistance - sin_phi1 * Math.Sin(phi2))) + origin[1]
            };
        }

        /// <summary>
        /// Calculates the destination point along a geodesic, given an initial point, heading and distance
        /// see http://www.movable-type.co.uk/scripts/latlong.html
        /// </summary>
        /// <param name="origin">The first point</param>
        /// <param name="heading">heading in degrees</param>
        /// <param name="distance">distance in metres</param>
        /// <returns>The point at the location along the geodesic</returns>
        public static double[] Destination(dynamic origin, double heading, double distance)
        {
            return Destination(origin.PointToDouble(), heading, distance);
        }

        /// <summary>
        /// Calculates the great circle distance between two points using the Haversine formula
        /// </summary>
        /// <param name="origin">The first point</param>
        /// <param name="destination">The second point</param>
        /// <returns>The distance between the given points in metres</returns>
        public static double DistanceHaversine(double[] origin, double[] destination)
        {
            return EarthRadius * AngularDistance(origin, destination);
        }

        /// <summary>
        /// Calculates the great circle distance between two points using the Haversine formula
        /// </summary>
        /// <param name="origin">The first point</param>
        /// <param name="destination">The second point</param>
        /// <returns>The distance between the given points in metres</returns>
        public static double DistanceHaversine(dynamic origin, dynamic destination)
        {
            return EarthRadius * AngularDistance(origin, destination);
        }

        /// <summary>
        /// Calculates the great circle distance between two points using the Vincenty formulae
        /// This function is based on the geodesy-library code by Mike Gavaghan 
        /// See http://www.gavaghan.org/blog/2007/08/06/c-gps-receivers-and-geocaching-vincentys-formula/
        /// </summary>
        /// <param name="origin">The first point</param>
        /// <param name="destination">The second point</param>
        /// <returns>The distance between the given points in metres</returns>
        public static double DistanceVincenty(double[] origin, double[] destination)
        {
            // All equation numbers refer back to Vincenty's publication:
            // See http://www.ngs.noaa.gov/PUBS_LIB/inverse.pdf

            // WGS84 Ellipsoid 
            // See http://en.wikipedia.org/wiki/WGS84
            double a = 6378137;
            double b = 6356752.3142;
            double f = 1 / 298.257223563;

            // get parametres as radians
            double phi1 = ConvertDegreesToRadians(origin[0]);
            double phi2 = ConvertDegreesToRadians(destination[0]);
            double lambda1 = ConvertDegreesToRadians(origin[1]);
            double lambda2 = ConvertDegreesToRadians(destination[1]);

            // calculations
            double a2 = a * a;
            double b2 = b * b;
            double aabbaa = (a2 - b2) / b2;

            double omega = lambda2 - lambda1;

            double tan_phi1 = Math.Tan(phi1);
            double tan_U1 = (1.0 - f) * tan_phi1;
            double u1 = Math.Atan(tan_U1);
            double sinU1 = Math.Sin(u1);
            double cosU1 = Math.Cos(u1);

            double tanPhi2 = Math.Tan(phi2);
            double tanU2 = (1.0 - f) * tanPhi2;
            double atanU2 = Math.Atan(tanU2);
            double sinU2 = Math.Sin(atanU2);
            double cosU2 = Math.Cos(atanU2);

            double sinU1sinU2 = sinU1 * sinU2;
            double cosU1sinU2 = cosU1 * sinU2;
            double sinU1cosU2 = sinU1 * cosU2;
            double cosU1cosU2 = cosU1 * cosU2;

            // eq. 13
            double lambda = omega;

            // intermediates we'll need to compute 's'
            double acs = 0.0;
            double bcs = 0.0;
            double sigma = 0.0;
            double dsigma = 0.0;
            double lambda0;

            for (int i = 0; i < 20; i++)
            {
                lambda0 = lambda;

                double sin_lambda = Math.Sin(lambda);
                double cos_lambda = Math.Cos(lambda);

                // eq. 14
                double sin2_sigma = (cosU2 * sin_lambda * cosU2 * sin_lambda) + Math.Pow(cosU1sinU2 - (sinU1cosU2 * cos_lambda), 2.0);
                double sin_sigma = Math.Sqrt(sin2_sigma);

                // eq. 15
                double cos_sigma = sinU1sinU2 + (cosU1cosU2 * cos_lambda);

                // eq. 16
                sigma = Math.Atan2(sin_sigma, cos_sigma);

                // eq. 17    Careful!  sin2sigma might be almost 0!
                double sin_alpha = (sin2_sigma == 0) ? 0.0 : cosU1cosU2 * sin_lambda / sin_sigma;
                double alpha = Math.Asin(sin_alpha);
                double cos_alpha = Math.Cos(alpha);
                double cos2_alpha = cos_alpha * cos_alpha;

                // eq. 18    Careful!  cos2alpha might be almost 0!
                double cos2_sigmam = cos2_alpha == 0.0 ? 0.0 : cos_sigma - (2 * (sinU1sinU2 / cos2_alpha));
                double u2 = cos2_alpha * aabbaa;

                double cos2_sigmam2 = cos2_sigmam * cos2_sigmam;

                // eq. 3
                acs = 1.0 + u2 / 16384 * (4096 + u2 * (-768 + u2 * (320 - 175 * u2)));

                // eq. 4
                bcs = u2 / 1024 * (256 + u2 * (-128 + u2 * (74 - 47 * u2)));

                // eq. 6
                dsigma = bcs * sin_sigma *
                    (cos2_sigmam + bcs / 4 *
                    (cos_sigma * (-1 + 2 * cos2_sigmam2) - bcs / 6 *
                    cos2_sigmam * (-3 + 4 * sin2_sigma) *
                    (-3 + 4 * cos2_sigmam2)));

                // eq. 10
                double c = f / 16 *
                    cos2_alpha *
                    (4 + f * (4 - 3 * cos2_alpha));

                // eq. 11 (modified)
                lambda = omega + (1 - c) *
                    f * sin_alpha *
                    (sigma + c * sin_sigma * (cos2_sigmam + c * cos_sigma * (-1 + 2 * cos2_sigmam2)));

                // see how much improvement there is
                double change = Math.Abs((lambda - lambda0) / lambda);

                if ((i > 1) && (change < Epslilon))
                {
                    break;
                }
            }

            // eq. 19
            return b * acs * (sigma - dsigma);
        }

        /// <summary>
        /// Calculates the great circle distance between two points using the Vincenty formulae
        /// This function is based on the geodesy-library code by Mike Gavaghan 
        /// See http://www.gavaghan.org/blog/2007/08/06/c-gps-receivers-and-geocaching-vincentys-formula/
        /// </summary>
        /// <param name="origin">The first point</param>
        /// <param name="destination">The second point</param>
        /// <returns>The distance between the given points in metres</returns>
        public static double DistanceVincenty(dynamic origin, dynamic destination)
        {
            return DistanceVincenty(
                new double[] { origin.getLatitude(), origin.getLongitude() },
                new double[] { destination.getLatitude(), destination.getLongitude() });
        }

        /// <summary>
        /// Keep a Longitudinal angle in the [-180, 180] range
        /// </summary>
        /// <param name="angle">Longitude to fix</param>
        /// <returns>Longitude in range</returns>
        public static double FixLongitudinalAngle(double angle)
        {
            if (angle % 360 == 180)
            {
                return 180;
            }

            angle = angle % 360;

            return angle < -180 ? angle + 360 : angle > 180 ? angle - 360 : angle;
        }

        /// <summary>
        /// Keep a Longitudinal angle in the [-90, 90] range
        /// </summary>
        /// <param name="angle">Longitude to fix</param>
        /// <returns>The angle in range</returns>
        public static double FixLatitudinalAngle(double angle)
        {
            return Math.Max(-90, Math.Min(90, angle));
        }

        /// <summary>
        /// Calculates the initial heading/bearing at which an object at the start
        /// point will need to travel to get to the destination point.
        /// </summary>
        /// <param name="origin">The first point</param>
        /// <param name="destination">The second point</param>
        /// <returns>The initial heading required ibn degrees</returns>
        public static double Heading(double[] origin, double[] destination)
        {
            double phi1 = ConvertDegreesToRadians(origin[0]);
            double phi2 = ConvertDegreesToRadians(destination[0]);
            double cos_phi2 = Math.Cos(phi2);
            double dlambda = ConvertDegreesToRadians(destination[1] - origin[1]);

            return NormaliseAngle(
                Math.Atan2(
                Math.Sin(dlambda) * cos_phi2,
                ConvertRadiansToDegrees(Math.Cos(phi1) * Math.Sin(phi2) - Math.Sin(phi1) * cos_phi2 * Math.Cos(dlambda))));
        }

        /// <summary>
        /// Calculates the initial heading/bearing at which an object at the start
        /// point will need to travel to get to the destination point.
        /// </summary>
        /// <param name="origin">The first point</param>
        /// <param name="destination">The second point</param>
        /// <returns>The initial heading required ibn degrees</returns>
        public static double Heading(dynamic origin, dynamic destination)
        {
            return Heading(origin.PointToDouble(), destination.PointToDouble());
        }

        /// <summary>
        /// Calculates an intermediate point on the geodesic between the two given points 
        /// See: http://williams.best.vwh.net/avform.htm#Intermediate
        /// </summary>
        /// <param name="origin">The first point</param>
        /// <param name="destination">The second point</param>
        /// <param name="fraction">Intermediate location as a decimal fraction (T value)</param>
        /// <returns>The point at the specified fraction along the geodesic</returns>
        public static double[] IntermediatePoint(double[] origin, double[] destination, double fraction)
        {
            if (fraction > 1 || fraction < 0)
            {
                throw new ArgumentOutOfRangeException("fraction must be between 0 and 1");
            }

            // TODO: check for antipodality and fail w/ exception in that case 
            double phi1 = ConvertDegreesToRadians(origin[0]);
            double phi2 = ConvertDegreesToRadians(destination[0]);
            double lambda1 = ConvertDegreesToRadians(origin[1]);
            double lambda2 = ConvertDegreesToRadians(destination[1]);

            double cos_phi1 = Math.Cos(phi1);
            double cos_phi2 = Math.Cos(phi2);
            double angularDistance = AngularDistance(origin, destination);
            double sin_angularDistance = Math.Sin(angularDistance);

            double a = Math.Sin((1 - fraction) * angularDistance) / sin_angularDistance;
            double b = Math.Sin(fraction * angularDistance) / sin_angularDistance;
            double x = a * cos_phi1 * Math.Cos(lambda1) + b * cos_phi2 * Math.Cos(lambda2);
            double y = a * cos_phi1 * Math.Sin(lambda1) + b * cos_phi2 * Math.Sin(lambda2);
            double z = a * Math.Sin(phi1) + b * Math.Sin(phi2);

            return new double[] 
            { 
                ConvertRadiansToDegrees(Math.Atan2(z, Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)))),
                ConvertRadiansToDegrees(Math.Atan2(y, x))
            };
        }

        /// <summary>
        /// Calculates an intermediate point on the geodesic between the two given points 
        /// See: http://williams.best.vwh.net/avform.htm#Intermediate
        /// </summary>
        /// <param name="origin">The first point</param>
        /// <param name="destination">The second point</param>
        /// <param name="fraction">Intermediate location as a decimal fraction (T value)</param>
        /// <returns>The point at the specified fraction along the geodesic</returns>
        public static double[] IntermediatePoint(dynamic origin, dynamic destination, double fraction)
        {
            return IntermediatePoint(origin.PointToDouble(), destination.PointToDouble(), fraction);
        }

        /// <summary>
        /// Extension method to convert an IkmlPoint object to a System.Double in lat,lng,alt format
        /// </summary>
        /// <param name="point">The point to convert</param>
        /// <returns>The point as an array of doubles</returns>
        public static double[] PointToDouble(dynamic point)
        {
            return new double[]
            { 
                point.getLatitude(),
                point.getLongitude(),
                point.getAltitude()
            };
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
        /// Reverses a number in the [0,PI] range
        /// </summary>
        /// <param name="radians">value in radians</param>
        /// <returns>The oposite angle</returns>
        public static double ReverseAngle(double radians)
        {
            return NormaliseAngle(radians + Math.PI);
        }
    }
}