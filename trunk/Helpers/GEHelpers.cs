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
    using Microsoft.CSharp.RuntimeBinder;

    /// <summary>
    /// This class provides some basic Google Earth plugin helpers functions.
    /// It is based on the "GEHelpers javasctipt library" by Roman Nurik
    /// See: http://earth-api-samples.googlecode.com/svn/trunk/lib/geplugin-helpers.js
    /// </summary>
    public static class GEHelpers
    {
        /// <summary>
        /// Creates a kml placemark
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="latLngAlt">The latitude and longitude in decimal degrees</param>
        /// <param name="data">Optional string data (id, name, description)</param>
        /// <returns>The placemark object</returns>
        public static dynamic CreatePlacemark(
            dynamic ge,
            double[] latLngAlt,
            params string[] data)
        {
            int agrs1 = latLngAlt.Length;

            if (agrs1 < 2)
            {
                try
                {
                    dynamic la = ge.getView().copyAsLookAt(ge.ALTITUDE_CLAMP_TO_GROUND);
                    latLngAlt = new double[] { la.getLatitude(), la.getLongitude(), la.getAltitude() };
                }
                catch (RuntimeBinderException ex)
                {
                    Debug.WriteLine("CreatePlacemark: " + ex.ToString());
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
        public static dynamic CreatePlacemark(
            dynamic ge,
            double latitude,
            double longitude,
            double altitude,
            string id,
            string name,
            string description)
        {
            dynamic placemark = ge.createPlacemark(String.Empty);

            try
            {
                placemark = ge.createPlacemark(id);
                placemark.setName(name);
                placemark.setDescription(description);

                dynamic p = ge.createPoint(String.Empty);
                p.setLatitude(latitude);
                p.setLongitude(longitude);
                p.setAltitude(altitude);
                p.setAltitudeMode(ge.ALTITUDE_RELATIVE_TO_GROUND);

                placemark.setGeometry(p);
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine("CreatePlacemark: " + ex.ToString());
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
        public static object CreateLineString(dynamic ge, dynamic p1, dynamic p2)
        {
            dynamic placemark = ge.createPlacemark(String.Empty);

            try
            {
                placemark = ge.createPlacemark(String.Empty);
                dynamic lineString = ge.createLineString(String.Empty);
                placemark.setGeometry(lineString);
                lineString.setTessellate(1);
                lineString.getCoordinates().pushLatLngAlt(p1.getLatitude(), p1.getLongitude(), 0);
                lineString.getCoordinates().pushLatLngAlt(p2.getLatitude(), p2.getLongitude(), 0);
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine("CreateLineString: " + ex.ToString());
            }

            return placemark;
        }

        /// <summary>
        /// Gets the Kml of all the features in the plug-in
        /// </summary>
        /// <param name="ge">The plugin</param>
        /// <returns>String of Kml</returns>
        public static string GetAllFeaturesKml(dynamic ge)
        {
            StringBuilder kml = new StringBuilder();

            try
            {
                dynamic children = ge.getFeatures().getChildNodes();
                for (int i = 0; i < children.getLength(); i++)
                {
                    dynamic child = children.item(i);

                    if (child != null)
                    {
                        kml.Append(child.getKml());
                    }
                }
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine("GetAllFeaturesKml: " + ex.ToString());
            }

            return kml.ToString();
        }

        /// <summary>
        /// Get the current pluin view as a point object
        /// </summary>
        /// <param name="ge">the plugin</param>
        /// <returns>Point set to the current view</returns>
        public static dynamic GetCurrentViewAsPoint(dynamic ge)
        {
            dynamic point = ge.createPoint(string.Empty);

            try
            {
                dynamic lookat = lookat = ge.getView().copyAsLookAt(ge.ALTITUDE_RELATIVE_TO_GROUND);
                point = ge.createPoint(String.Empty);
                point.set(
                    lookat.getLatitude(),
                    lookat.getLongitude(),
                    lookat.getAltitude(),
                    ge.ALTITUDE_RELATIVE_TO_GROUND,
                    0,
                    0);
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine("GetCurrentViewAsPoint: " + ex.ToString());
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
        public static bool LookAt(dynamic ge, double latitude, double longitude)
        {
            try
            {
                dynamic lookat = ge.createLookAt(String.Empty);
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
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine("LookAt: " + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Look at the given geometry 
        /// </summary>
        /// <param name="ge">the plugin</param>
        /// <param name="feature">the geomerty to look at</param>
        /// <param name="browser">A instance of the GEWebBrowser object</param>
        /// <returns>true on success</returns>
        public static bool LookAt(dynamic ge, dynamic feature, GEWebBrowser browser)
        {
            dynamic abstractView = null;

            if (null != ge && null != feature)
            {
                string type = feature.getType();

                try
                {
                    switch (type)
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
                                if (null != browser)
                                {
                                    string linkUrl = string.Empty;

                                    // Kml documents using the pre 2.1 spec may contain the <Url> element 
                                    // in these cases the getHref call will return null
                                    try
                                    {
                                        linkUrl = feature.getLink().getHref();
                                    }
                                    catch (NullReferenceException)
                                    {
                                        linkUrl = feature.GetUrl();
                                    }

                                    dynamic kmlObject = browser.FetchKmlSynchronous(linkUrl);

                                    if (null != kmlObject)
                                    {
                                        if (kmlObject.getOwnerDocument() != null)
                                        {
                                            abstractView = kmlObject.getOwnerDocument().getAbstractView();
                                        }
                                    }
                                }
                            }

                            break;
                        case "KmlPoint":
                            return LookAt(ge, feature.getLatitude(), feature.getLongitude());
                        case "KmlPolygon":
                            return LookAt(
                                ge,
                                feature.getOuterBoundary().getCoordinates().get(0).getLatitude(),
                                feature.getOuterBoundary().getCoordinates().get(0).getLongitude());
                        case "KmlPlacemark":
                            if (null != feature.getAbstractView())
                            {
                                abstractView = feature.getAbstractView();
                            }
                            else
                            {
                                return LookAt(ge, feature.getGeometry(), browser);
                            }

                            break;
                        case "KmlLineString":
                            return LookAt(
                                ge,
                                feature.getCoordinates().get(0).getLatitude(),
                                feature.getCoordinates().get(0).getLongitude());
                        case "KmlMultiGeometry":
                            ////IKmlMultiGeometry multiGeometry = (IKmlMultiGeometry)geometry;
                            ////multiGeometry.getGeometries().getFirstChild().getType();
                            return false;
                        default:
                            return false;
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
                catch (RuntimeBinderException ex)
                {
                    Debug.WriteLine("LookAt: " + ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Opens a balloon for the given feature
        /// </summary>
        /// <param name="ge">the plugin</param>
        /// <param name="feature">the feature</param>
        public static void OpenFeatureBalloon(dynamic ge, dynamic feature)
        {
            try
            {
                dynamic balloon = ge.getBalloon();

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
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine("OpenFeatureBalloon: " + ex.ToString());
            }
        }

        /// <summary>
        /// Remove all features from the plugin 
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        public static void RemoveAllFeatures(dynamic ge)
        {
            try
            {
                dynamic features = ge.getFeatures();
                while (features.getLastChild() != null)
                {
                    features.removeChild(features.getLastChild());
                }
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine("RemoveAllFeatures: " + ex.ToString());
            }
        }

        /// <summary>
        /// Displays the current plugin view in Google Maps using the default system browser
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        public static void ShowCurrentViewInMaps(dynamic ge)
        {
            try
            {
                // Get the current view 
                dynamic lookat = ge.getView().copyAsLookAt(ge.ALTITUDE_RELATIVE_TO_GROUND);
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
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine("ShowCurrentViewInMaps: " + ex.ToString());
            }
        }
    }
}