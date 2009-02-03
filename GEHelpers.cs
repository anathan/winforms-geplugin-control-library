// <copyright file="GEHelpers.cs" company="FC">
// Copyright (c) 2008 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2008-12-22</date>
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
    /// This class provides a very basic Google Earth plugin helpers
    /// It is based on the GEHelpers javasctipt library at:
    /// http://earth-api-samples.googlecode.com/svn/trunk/lib/geplugin-helpers.js
    /// </summary>
    public static class GEHelpers
    {
        /// <summary>
        /// Earth's radius in km 
        /// </summary>
        private const int Radius = 6371;

        /// <summary>
        /// Miles To Kilometers Conversion Ratio
        /// </summary>
        private const double MilesToKilometersRatio = 0.621371192;

        /// <summary>
        /// Remove all features from the plugin 
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        public static void RemoveAllFeatures(IGEPlugin ge)
        {
            IGEFeatureContainer features = ge.getFeatures();
            IKmlObject feature = features.getFirstChild();
            while (feature != null)
            {
                features.removeChild(feature);
            }
        }

        /// <summary>
        /// Remove the feature with the given id from the plugin
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="id">The id of the element to remove</param>
        public static void RemoveFeatureById(IGEPlugin ge, string id)
        {
            IGEFeatureContainer features = ge.getFeatures();
            IKmlObject c = features.getFirstChild();
            while (c != null)
            {
                if (c.getId() == id)
                {
                    features.removeChild(c);
                    break;
                }
            }
        }

        /// <summary>
        /// Convert degrees to radians
        /// </summary>
        /// <param name="degrees">Decimal degrees</param>
        /// <returns>Number of radians</returns>
        public static double ConvertDegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }

        /// <summary>
        /// Convert radians to degrees
        /// </summary>
        /// <param name="radians">Number of radians</param>
        /// <returns>Decimal degrees</returns>
        public static double ConvertRadiansToDegrees(double radians)
        {
            return radians * (180.0 / Math.PI);
        }

        /// <summary>
        /// Convert Kilometers To Miles 
        /// </summary>
        /// <param name="kilometers">distance in kilometeres</param>
        /// <returns>distance in miles</returns>
        public static double ConvertKilometersToMiles(double kilometers)
        {
            return kilometers * MilesToKilometersRatio;
        }

        /// <summary>
        /// Convert Miles To Kilometers
        /// </summary>
        /// <param name="miles">distance in miles</param>
        /// <returns>distance in kilometeres</returns>
        public static double ConvertMilesToKilometers(double miles)
        {
            return miles / MilesToKilometersRatio;
        }

        /// <summary>
        /// Find the intermediate point at a given factor between two points
        /// </summary>
        /// <param name="p1">first point</param>
        /// <param name="p2">second point</param>
        /// <param name="f">intermediate factor</param>
        /// <returns>Ikmlpoint intermediate point</returns>
        public static IKmlPoint InterpolateLocation(IKmlPoint p1, IKmlPoint p2, double f)
        {
            IKmlPoint p = p1;
            p.setLatitude(p1.getLatitude() + (f * (p2.getLatitude() - p1.getLatitude())));
            p.setLongitude(p1.getLongitude() + (f * (p2.getLongitude() - p1.getLongitude())));
            return p;
        }

        /// <summary>
        /// Find the intermediate point at a given factor between two points
        /// </summary>
        /// <param name="loc1">first point</param>
        /// <param name="loc2">second point</param>
        /// <param name="f">intermediate factor</param>
        /// <returns>double intermediate point</returns>
        public static double[] InterpolateLocation(double[] loc1, double[] loc2, double f)
        {
            return new double[]
            {
                loc1[0] + (f * (loc2[0] - loc1[0])),
                loc1[1] + (f * (loc2[1] - loc1[1])) 
            };
        }

        /// <summary>
        /// Get the current pluin view as a point object
        /// </summary>
        /// <param name="ge">the plugin</param>
        /// <returns>view as point</returns>
        public static IKmlPoint GetCurrentViewAsPoint(IGEPlugin ge)
        {
            IKmlLookAt lookat = ge.getView().copyAsLookAt(ge.ALTITUDE_RELATIVE_TO_GROUND);
            IKmlPoint point = ge.createPoint(String.Empty);
            point.set(lookat.getLatitude(), lookat.getLongitude(), lookat.getAltitude(), ge.ALTITUDE_RELATIVE_TO_GROUND, 0, 0);
            return point;
        }
    }
}