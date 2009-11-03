// <copyright file="GEHelpers.cs" company="FC">
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
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Forms;
    using GEPlugin;

    /// <summary>
    /// This class provides a very basic Google Earth plugin helpers
    /// It is based on the GEHelpers javasctipt library at:
    /// http://earth-api-samples.googlecode.com/svn/trunk/lib/geplugin-helpers.js
    /// </summary>
    public static class GEHelpers
    {
        /// <summary>
        /// Creates a kml placemark
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="latLngAlt">The latitude and longitude in decimal degrees</param>
        /// <param name="data">Optional data (id, name, description)</param>
        /// <returns>The placemark object</returns>
        public static IKmlPlacemark CreatePlacemark(IGEPlugin ge, double[] latLngAlt, params string[] data)
        {
            int agrs1 = latLngAlt.Length;

            if (agrs1 < 2)
            {
                try
                {
                    IKmlLookAt la = ge.getView().copyAsLookAt(ge.ALTITUDE_CLAMP_TO_GROUND);
                    latLngAlt = new double[] { la.getLatitude(), la.getLongitude(), la.getAltitude() };
                }
                catch (COMException cex)
                {
                    Debug.WriteLine("CreatePlacemark: " + cex.ToString());
                }
            }
            else if (agrs1 == 2)
            {
                latLngAlt = new double[] { latLngAlt[0], latLngAlt[1], 0 };
            }

            int args2 = data.Length;

            switch (args2)
            {
                case 1:
                    return CreatePlacemark(ge, latLngAlt[0], latLngAlt[1], latLngAlt[2], data[0], string.Empty, string.Empty);
                case 2:
                    return CreatePlacemark(ge, latLngAlt[0], latLngAlt[1], latLngAlt[2], data[0], data[1], string.Empty);
                case 3:
                    return CreatePlacemark(ge, latLngAlt[0], latLngAlt[1], latLngAlt[2], data[0], data[1], data[2]);
                case 0:
                default:
                    return CreatePlacemark(ge, latLngAlt[0], latLngAlt[1], latLngAlt[2], string.Empty, string.Empty, string.Empty);
            }
        }

        /// <summary>
        /// Creates a kml placemark
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="latitude">The placemark latitude in decimal degrees</param>
        /// <param name="longitude">The placemark longitude in decimal degrees</param>
        /// <param name="altitude">The placemark altitude in metres</param>
        /// <param name="id">The placemark Id</param>
        /// <param name="name">The name of the placemark</param>
        /// <param name="description">The placemark description text</param>
        /// <returns>The placemark object</returns>
        public static IKmlPlacemark CreatePlacemark(
            IGEPlugin ge,
            double latitude,
            double longitude,
            double altitude,
            string id,
            string name,
            string description)
        {
            IKmlPlacemark placemark = new KmlPlacemarkCoClass();

            try
            {
                placemark = ge.createPlacemark(id);
                placemark.setName(name);
                placemark.setDescription(description);

                IKmlPoint p = ge.createPoint(String.Empty);
                p.setLatitude(latitude);
                p.setLongitude(longitude);
                p.setAltitude(altitude);
                p.setAltitudeMode(ge.ALTITUDE_RELATIVE_TO_GROUND);

                placemark.setGeometry(p);
            }
            catch (COMException cex)
            {
                Debug.WriteLine("CreatePlacemark: " + cex.ToString());
            }

            return placemark;
        }

        /// <summary>
        /// Draws a line string between the given points
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <returns>A linestring placemark</returns>
        public static IKmlPlacemark CreateLineString(IGEPlugin ge, IKmlPoint p1, IKmlPoint p2)
        {
            IKmlPlacemark placemark = new KmlPlacemarkCoClass();

            try
            {
                placemark = ge.createPlacemark(String.Empty);
                IKmlLineString lineString = ge.createLineString(String.Empty);
                placemark.setGeometry(lineString);
                lineString.setTessellate(1);
                lineString.getCoordinates().pushLatLngAlt(p1.getLatitude(), p1.getLongitude(), 0);
                lineString.getCoordinates().pushLatLngAlt(p2.getLatitude(), p2.getLongitude(), 0);
            }
            catch (COMException cex)
            {
                Debug.WriteLine("CreateLineString: " + cex.ToString());
            }

            return placemark;
        }

        /// <summary>
        /// Gets the Kml of all the features in the plug-in
        /// </summary>
        /// <param name="ge">The plugin</param>
        /// <returns>String of Kml</returns>
        public static string GetAllFeaturesKml(IGEPlugin ge)
        {
            StringBuilder kml = new StringBuilder();

            try
            {
                IKmlObjectList children = ge.getFeatures().getChildNodes();
                for (int i = 0; i < children.getLength(); i++)
                {
                    IKmlFeature child = children.item(i) as IKmlFeature;
                    if (child != null)
                    {
                        kml.Append(child.getKml());
                    }
                }
            }
            catch (COMException cex)
            {
                Debug.WriteLine("GetAllFeaturesKml: " + cex.ToString());
            }

            return kml.ToString();
        }

        /// <summary>
        /// Get the current pluin view as a point object
        /// </summary>
        /// <param name="ge">the plugin</param>
        /// <returns>Point set to the current view</returns>
        public static IKmlPoint GetCurrentViewAsPoint(IGEPlugin ge)
        {
            IKmlPoint point = new KmlPointCoClass();

            try
            {
                IKmlLookAt lookat = lookat = ge.getView().copyAsLookAt(ge.ALTITUDE_RELATIVE_TO_GROUND);
                point = ge.createPoint(String.Empty);
                point.set(
                    lookat.getLatitude(),
                    lookat.getLongitude(),
                    lookat.getAltitude(),
                    ge.ALTITUDE_RELATIVE_TO_GROUND,
                    0,
                    0);
            }
            catch (COMException cex)
            {
                Debug.WriteLine("GetCurrentViewAsPoint: " + cex.ToString());
            }

            return point;
        }

        /// <summary>
        /// Get the type of an active scripting object from a generic runtime callable wrapper.
        /// </summary>
        /// <param name="wrapper">The com object wrapper</param>
        /// <returns>The managed type</returns>
        public static string GetTypeFromRcw(object wrapper)
        {
            string type = (string)wrapper.GetType().InvokeMember(
                "getType",
                System.Reflection.BindingFlags.InvokeMethod,
                null,
                wrapper,
                null);
            return type;
        }

        /// <summary>
        /// Look at the given coordinates
        /// </summary>
        /// <param name="ge">the plugin</param>
        /// <param name="latitude">latitude in decimal degrees</param>
        /// <param name="longitude">longitude in decimal degrees</param>
        /// <returns>true on success</returns>
        public static bool LookAt(IGEPlugin ge, double latitude, double longitude)
        {
            try
            {
                IKmlLookAt lookat = ge.createLookAt(String.Empty);
                lookat.set(
                    latitude,
                    longitude,
                    0,
                    ge.ALTITUDE_RELATIVE_TO_GROUND,
                    0,
                    0,
                    1000);
                ge.getView().setAbstractView(lookat);
                return true;
            }
            catch (COMException cex)
            {
                Debug.WriteLine("LookAt: " + cex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Look at the given feature
        /// </summary>
        /// <param name="ge">the plugin</param>
        /// <param name="feature">the feature to look at</param>
        /// <param name="gewb">a browser object for to access plugin via conduit</param>
        /// <returns>true on success</returns>
        public static bool LookAt(IGEPlugin ge, IKmlFeature feature, GEWebBrowser gewb)
        {
            try
            {
                IKmlAbstractView abstractView = null;
                switch (feature.getType())
                {
                    case "KmlFolder":
                    case "KmlDocument":
                        if (null != feature.getAbstractView())
                        {
                            abstractView = feature.getAbstractView();
                        }

                        break;
                    case "KmlNetworkLink":
                        if (null != feature.getAbstractView())
                        {
                            abstractView = feature.getAbstractView();
                        }
                        else
                        {
                            if (null != gewb)
                            {
                                string linkUrl = ((IKmlNetworkLink)feature).getLink().getHref();
                                IKmlObject kmlObject = gewb.FetchKmlSynchronous(linkUrl);
                                if (null != kmlObject)
                                {
                                    abstractView = kmlObject.getOwnerDocument().getAbstractView();
                                }
                            }
                        }

                        break;

                    case "KmlPlacemark":
                        if (null != feature.getAbstractView())
                        {
                            abstractView = feature.getAbstractView();
                        }
                        else
                        {
                            IKmlPlacemark placemark = (IKmlPlacemark)feature;
                            return LookAt(ge, placemark.getGeometry());
                        }

                        break;
                }

                if (null != abstractView)
                {
                    ge.getView().setAbstractView(abstractView);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (COMException cex)
            {
                Debug.WriteLine("LookAt: " + cex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Look at the given geometry 
        /// </summary>
        /// <param name="ge">the plugin</param>
        /// <param name="geometry">the geomerty to look at</param>
        /// <returns>true on success</returns>
        public static bool LookAt(IGEPlugin ge, IKmlGeometry geometry)
        {
            if (null != ge && null != geometry)
            {
                try
                {
                    switch (geometry.getType())
                    {
                        case "KmlPoint":
                            return LookAt(ge, (IKmlPoint)geometry);
                        case "KmlPolygon":
                            IKmlPolygon polygon = (IKmlPolygon)geometry;
                            return LookAt(
                                ge,
                                polygon.getOuterBoundary().getCoordinates().get(0).getLatitude(),
                                polygon.getOuterBoundary().getCoordinates().get(0).getLongitude());
                        case "KmlLineString":
                            IKmlLineString lineString = (IKmlLineString)geometry;
                            return LookAt(
                                ge,
                                lineString.getCoordinates().get(0).getLatitude(),
                                lineString.getCoordinates().get(0).getLongitude());
                        case "KmlMultiGeometry":
                            ////IKmlMultiGeometry multiGeometry = (IKmlMultiGeometry)geometry;
                            ////multiGeometry.getGeometries().getFirstChild().getType();
                            return false;
                        default:
                            return false;
                    }
                }
                catch (COMException cex)
                {
                    Debug.WriteLine("LookAt: " + cex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Look at the given point
        /// </summary>
        /// <param name="ge">the plugin</param>
        /// <param name="point">the point to look at</param>
        /// <returns>true on success</returns>
        public static bool LookAt(IGEPlugin ge, IKmlPoint point)
        {
            return LookAt(ge, point.getLatitude(), point.getLongitude());
        }

        /// <summary>
        /// Opens a balloon for the given feature
        /// </summary>
        /// <param name="ge">the plugin</param>
        /// <param name="feature">the feature</param>
        public static void OpenFeatureBalloon(IGEPlugin ge, IKmlFeature feature)
        {
            try
            {
                IGEFeatureBalloon balloon = ge.getBalloon() as IGEFeatureBalloon;

                if (null != balloon)
                {
                    balloon = ge.createFeatureBalloon(String.Empty);
                    balloon.setFeature(feature);
                    ge.setBalloon(balloon);
                }
                else
                {
                    balloon.setFeature(feature);
                    ge.setBalloon(balloon);
                }
            }
            catch (COMException cex)
            {
                Debug.WriteLine("OpenFeatureBalloon: " + cex.ToString());
            }
        }

        /// <summary>
        /// Remove all features from the plugin 
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        public static void RemoveAllFeatures(IGEPlugin ge)
        {
            try
            {
                IGEFeatureContainer features = ge.getFeatures();
                while (features.getLastChild() != null)
                {
                    features.removeChild(features.getLastChild());
                }
            }
            catch (COMException cex)
            {
                Debug.WriteLine("RemoveAllFeatures: " + cex.ToString());
            }
        }

        /// <summary>
        /// Remove the feature with the given id from the plugin
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="id">The id of the element to remove</param>
        public static void RemoveFeatureById(IGEPlugin ge, string id)
        {
            try
            {
                while (Convert.ToBoolean(ge.getFeatures().hasChildNodes()))
                {
                    if (ge.getFeatures().getFirstChild().getId() == id)
                    {
                        ge.getFeatures().removeChild(ge.getFeatures().getFirstChild());
                    }
                }
            }
            catch (COMException cex)
            {
                Debug.WriteLine("RemoveFeatureById: " + cex.ToString());
            }
        }

        /// <summary>
        /// Displays the current plugin view in Google Maps using the default system browser
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        public static void ShowCurrentViewInMaps(IGEPlugin ge)
        {
            try
            {
                // Get the current view 
                IKmlLookAt lookat = ge.getView().copyAsLookAt(ge.ALTITUDE_RELATIVE_TO_GROUND);
                double range = lookat.getRange();

                // calculate the equivelent zoom level from the given range
                double zoom = Math.Round(26 - (Math.Log(lookat.getRange()) / Math.Log(2)));

                // Google Maps have an integer "zoom level" which defines the resolution of the current view.
                // Zoom levels between 0 (entire world on map) to 21+ (down to individual buildings) are possible.
                if (zoom < 1)
                {
                    zoom = 1;
                }
                else if (zoom > 21)
                {
                    zoom = 21;
                }

                // Build the maps url
                StringBuilder url =
                    new StringBuilder("http://maps.google.co.uk/maps?ll=");
                url.Append(lookat.getLatitude());
                url.Append(",");
                url.Append(lookat.getLongitude());
                url.Append("&z=");
                url.Append(zoom);

                // launch the default browser with the url
                System.Diagnostics.Process.Start(url.ToString());
            }
            catch (COMException cex)
            {
                Debug.WriteLine("ShowCurrentViewInMaps: " + cex.ToString());
            }
        }
    }
}