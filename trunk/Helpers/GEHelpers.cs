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
        /// This is a simple "ge.getFeatures().appendChild(kml)" wrapper
        /// </summary>
        /// <param name="ge">the plugin instance to add the features to</param>
        /// <param name="kml">the faetures to add</param>
        public static void AddFeaturesToPlugin(dynamic ge, dynamic kml)
        {
            if (!IsGe(ge))
            {
                throw new ApplicationException("ge is not of the type GEPlugin");
            }

            try
            {
                ge.getFeatures().appendChild(kml);
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("AddFeaturesToPlugin: " + rbex.ToString(), "GEHelpers");
            }
            catch (COMException cex)
            {
                Debug.WriteLine("AddFeaturesToPlugin: " + cex.ToString(), "GEHelpers");
            }
        }

        /// <summary>
        /// Creates a kml placemark
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="latitude">The placemark latitude in decimal degrees</param>
        /// <param name="longitude">The placemark longitude in decimal degrees</param>
        /// <param name="altitude">Optional placemark altitude in metres. Default is 0</param>
        /// <param name="altitudeMode">Optional altitudeMode. Default is AltitudeMode.RelativeToGround</param>
        /// <param name="id">Optional placemark Id. Default is empty</param>
        /// <param name="name">Optional name of the placemark. Default is empty</param>
        /// <param name="description">Optional placemark description text. Default is empty</param>
        /// <param name="addFeature">Optionally adds the placemark directly to the plugin. Default is true</param>
        /// <returns>A placemark (or an empty object)</returns>
        public static dynamic CreatePlacemark(
            dynamic ge,
            double latitude = 0,
            double longitude = 0,
            double altitude = 0,
            AltitudeMode altitudeMode = AltitudeMode.RelativeToGround,
            string id = "",
            string name = "",
            string description = "",
            bool addFeature = true)
        {
            if (!IsGe(ge))
            {
                throw new ApplicationException("ge is not of the type GEPlugin");
            }

            dynamic placemark = null;

            try
            {
                placemark = ge.createPlacemark(id);
                placemark.setName(name);
                placemark.setDescription(description);

                dynamic p = ge.createPoint(String.Empty);
                p.setLatitude(latitude);
                p.setLongitude(longitude);
                p.setAltitude(altitude);
                p.setAltitudeMode(altitudeMode);

                placemark.setGeometry(p);
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine("CreatePlacemark: " + ex.ToString(), "GEHelpers");
            }

            if (placemark == null)
            {
                return new object { };
            }

            if (addFeature)
            {
                GEHelpers.AddFeaturesToPlugin(ge, placemark);
            }

            return placemark;
        }

        /// <summary>
        /// Draws a line string between the given points
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <param name="id">Optional ID of the placemark. Default is empty</param>
        /// <param name="tessellate">Optionally sets tessellation for the linestring. Default is true</param>
        /// <param name="addFeature">Optionally adds the linestring directly to the plugin. Default is true</param>
        /// <returns>A linestring placemark (or an empty object)</returns>
        public static object CreateLineString(
            dynamic ge,
            dynamic p1,
            dynamic p2,
            string id = "",
            bool tessellate = true,
            bool addFeature = true)
        {
            if (!IsGe(ge))
            {
                throw new ApplicationException("ge is not of the type GEPlugin");
            }

            dynamic placemark = null;
            dynamic lineString = null;

            try
            {
                placemark = ge.createPlacemark(id);
                lineString = ge.createLineString(String.Empty);
                lineString.setTessellate(Convert.ToInt16(tessellate));
                lineString.getCoordinates().pushLatLngAlt(p1.getLatitude(), p1.getLongitude(), p1.getAltitude());
                lineString.getCoordinates().pushLatLngAlt(p2.getLatitude(), p2.getLongitude(), p2.getAltitude());
                placemark.setGeometry(lineString);
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine("CreateLineString: " + ex.ToString(), "GEHelpers");
            }

            if (placemark == null)
            {
                return new object { };
            }

            if (addFeature)
            {
                GEHelpers.AddFeaturesToPlugin(ge, placemark);
            }

            return placemark;
        }

        /// <summary>
        /// Enables or disables a plugin layer - wrapper for ge.getLayerRoot().enableLayerById()
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="id">The id of the layer to work with</param>
        /// <param name="value">True turns the layer on, false off</param>
        public static void EnableLayerById(dynamic ge, Layer id, bool value)
        {
            if (!IsGe(ge))
            {
                throw new ApplicationException("ge is not of the type GEPlugin");
            }

            ge.getLayerRoot().EnableLayerById(id.ToString(), value);
        }

        /// <summary>
        /// Gets the Kml of all the features in the plug-in
        /// </summary>
        /// <param name="ge">The plugin</param>
        /// <returns>String of all the Kml from the plugin - or an empty string</returns>
        public static string GetAllFeaturesKml(dynamic ge)
        {
            if (!IsGe(ge))
            {
                throw new ApplicationException("ge is not of the type GEPlugin");
            }

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
        /// <returns>Point set to the current view (or an empty object)</returns>
        public static dynamic GetCurrentViewAsPoint(dynamic ge)
        {
            if (!IsGe(ge))
            {
                throw new ApplicationException("ge is not of the type GEPlugin");
            }

            dynamic point = null;
            dynamic lookat = null;

            try
            {
                lookat = ge.getView().copyAsLookAt(ge.ALTITUDE_RELATIVE_TO_GROUND);
                point = ge.createPoint(String.Empty);

                // latitude, longitude, altitude, altitudeMode, extrude, tessellate
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
                Debug.WriteLine("GetCurrentViewAsPoint: " + ex.ToString(), "GEHelpers");
            }

            if (point == null)
            {
                return new object { };
            }

            return point;
        }

        /// <summary>
        /// Get the type of an active scripting object from a generic runtime callable wrapper.
        /// This method invokes the member getType on the com object.
        /// </summary>
        /// <param name="wrapper">The com object wrapper</param>
        /// <returns>The managed type, or an empty string on failure</returns>
        public static string GetTypeFromRcw(object wrapper)
        {
            string type = string.Empty;

            try
            {
                type = (string)wrapper.GetType().InvokeMember(
                   "getType",
                   System.Reflection.BindingFlags.InvokeMethod,
                   null,
                   wrapper,
                   null);
            }
            catch (System.Reflection.TargetInvocationException)
            {
            }

            return type;
        }

        /// <summary>
        /// Checks is an objects type name matches the given string
        /// </summary>
        /// <param name="wrapper">The object to check</param>
        /// <param name="type">The name to check for</param>
        /// <returns></returns>
        public static bool IsObjectType(object wrapper, string type)
        {
            if (GEHelpers.GetTypeFromRcw(wrapper) == type)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the given object in an RCW is of the type GEPlugin
        /// </summary>
        /// <param name="wrapper">The object to check</param>
        /// <returns>true if the object is of the type GEPlugin </returns>
        public static bool IsGe(object wrapper)
        {
            return GEHelpers.IsObjectType(wrapper, "GEPlugin");
        }

        /// <summary>
        /// Look at the given coordinates
        /// </summary>
        /// <param name="ge">the plugin</param>
        /// <param name="latitude">latitude in decimal degrees</param>
        /// <param name="longitude">longitude in decimal degrees</param>
        /// <param name="id">Optional LookAt Id. Default is empty</param>
        /// <param name="altitude">Optional altitude. Default is 0</param>
        /// <param name="altitudeMode">Optional altitudeMode. Default is AltitudeMode.RelativeToGround</param>
        /// <param name="heading">Optional heading in degrees. Default is 0 (north)</param>
        /// <param name="tilt">Optional tilt in degrees. Default is 0</param>
        /// <param name="range">Optional range in metres. Default is 1000</param>
        /// <returns>true on success</returns>
        public static bool LookAt(
            dynamic ge,
            double latitude,
            double longitude,
            string id = "",
            double altitude = 0,
            AltitudeMode altitudeMode = AltitudeMode.RelativeToGround,
            double heading = 0,
            double tilt = 0,
            double range = 1000
            )
        {
            if (!IsGe(ge))
            {
                throw new ApplicationException("ge is not of the type GEPlugin");
            }

            dynamic lookat = null;

            try
            {
                lookat = ge.createLookAt(id);

                // latitude, longitude, altitude, altitudeMode, heading, tilt, range
                lookat.set(
                    latitude,
                    longitude,
                    altitude,
                    altitudeMode,
                    heading,
                    tilt,
                    range);

                ge.getView().setAbstractView(lookat);
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine("LookAt: " + ex.ToString());
                return false;
            }

            if (lookat == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Look at the given geometry 
        /// </summary>
        /// <param name="feature">the geomerty to look at</param>
        /// <param name="browser">A instance of the GEWebBrowser object</param>
        /// <returns>true on success</returns>
        public static bool LookAt(dynamic feature, GEWebBrowser browser)
        {
            dynamic ge = browser.GetPlugin();

            if (!IsGe(ge))
            {
                throw new ApplicationException("ge is not of the type GEPlugin");
            }

            dynamic abstractView = null;
            string type = string.Empty;

            try
            {
                type = feature.getType();

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
                            return LookAt(feature.getGeometry(), browser);
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

        /// <summary>
        /// Opens the balloon for the given feature in the plugin
        /// </summary>
        /// <param name="ge">the plugin instance</param>
        /// <param name="feature">the feature to open a ballon for</param>
        /// <param name="setBalloon">Optionally opens the balloon in the plugin. Default is true</param>
        /// <returns>The balloon object (GEFeatureBalloon) or and empty object</returns>
        public static dynamic OpenFeatureBalloon(
            dynamic ge,
            dynamic feature,
            bool setBalloon = true)
        {
            if (!IsGe(ge))
            {
                throw new ApplicationException("ge is not of the type GEPlugin");
            }

            dynamic balloon = null;

            try
            {
                balloon = ge.getBalloon();

                if (balloon == null)
                {
                    balloon = ge.createFeatureBalloon(String.Empty);
                }

                balloon.setFeature(feature);

                if (setBalloon)
                {
                    ge.setBalloon(balloon);
                }
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine("OpenFeatureBalloon: " + ex.ToString(), "GEHelpers");
            }

            if (balloon == null)
            {
                return new object { };
            }

            return balloon;
        }

        /// <summary>
        /// Remove all features from the plugin 
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        public static void RemoveAllFeatures(dynamic ge)
        {
            if (!IsGe(ge))
            {
                throw new ApplicationException("ge is not of the type GEPlugin");
            }

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
            if (!IsGe(ge))
            {
                throw new ApplicationException("ge is not of the type GEPlugin");
            }

            dynamic lookat = null;
            double range = 0;
            double zoom = 0;
            StringBuilder url =
                new StringBuilder();

            try
            {
                // Get the current view 
                lookat = ge.getView().copyAsLookAt(ge.ALTITUDE_RELATIVE_TO_GROUND);
                range = lookat.getRange();

                // calculate the equivelent zoom level from the given range
                zoom = Math.Round(26 - (Math.Log(lookat.getRange()) / Math.Log(2)));

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
                url.Append("http://maps.google.co.uk/maps?ll=");
                url.Append(lookat.getLatitude());
                url.Append(",");
                url.Append(lookat.getLongitude());
                url.Append("&z=");
                url.Append(zoom);
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine("ShowCurrentViewInMaps: " + ex.ToString());
            }

            // launch the default browser with the url
            System.Diagnostics.Process.Start(url.ToString());
        }
    }
}