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
    using System.Linq;
    using System.Reflection;
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
        /// <exception cref="System.ArgumentException" >Throws an exception if ge is not an instance of GEPlugin.</exception>
        public static void AddFeaturesToPlugin(dynamic ge, dynamic kml)
        {
            if (!IsGe(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
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
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            dynamic placemark = null;

            try
            {
                placemark = ge.createPlacemark(id);
                placemark.setName(name);
                placemark.setDescription(description);
                placemark.setGeometry(CreatePoint(ge, latitude, longitude, altitude, altitudeMode));
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("CreatePlacemark: " + rbex.ToString(), "GEHelpers");
            }

            if (null == placemark)
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
        /// Creates a kml placemark
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="point">A KmlPoint object to use for the placemark location.</param>
        /// <param name="id">Optional placemark Id. Default is empty</param>
        /// <param name="name">Optional name of the placemark. Default is empty</param>
        /// <param name="description">Optional placemark description text. Default is empty</param>
        /// <param name="addFeature">Optionally adds the placemark directly to the plugin. Default is true</param>
        /// <returns>A placemark (or an empty object)</returns>
        public static dynamic CreatePlacemark(
            dynamic ge,
            dynamic point,
            string id = "",
            string name = "",
            string description = "",
            bool addFeature = true)
        {
            if (!IsGe(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }
            else if (GetTypeFromRcw(point) != ApiType.KmlPoint)
            {
                throw new ArgumentException("point is not of the type KmlPoint");
            }

            dynamic placemark = CreatePlacemark(ge, name: name, id: id, description: description);

            try
            {
                placemark.setGeometry(point);
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("CreatePlacemark: " + rbex.ToString(), "GEHelpers");
            }

            if (null == placemark)
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
        /// Creates a kml point
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="latitude">The placemark latitude in decimal degrees</param>
        /// <param name="longitude">The placemark longitude in decimal degrees</param>
        /// <param name="altitude">Optional placemark altitude in metres. Default is 0</param>
        /// <param name="altitudeMode">Optional altitudeMode. Default is AltitudeMode.RelativeToGround</param>
        /// <param name="id">Optional placemark Id. Default is empty</param>
        /// <returns>A Kml point</returns>
        public static dynamic CreatePoint(
            dynamic ge,
            double latitude = 0,
            double longitude = 0,
            double altitude = 0,
            AltitudeMode altitudeMode = AltitudeMode.RelativeToGround,
            string id = "")
        {
            if (!IsGe(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            dynamic point = ge.createPoint(id);
            point.setLatitude(latitude);
            point.setLongitude(longitude);
            point.setAltitude(altitude);
            point.setAltitudeMode(altitudeMode);
            return point;
        }

        /// <summary>
        /// Draws a line string between the given points
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="start">The first point</param>
        /// <param name="end">The second point</param>
        /// <param name="id">Optional ID of the placemark. Default is empty</param>
        /// <param name="tessellate">Optionally sets tessellation for the linestring. Default is true</param>
        /// <param name="addFeature">Optionally adds the linestring directly to the plugin. Default is true</param>
        /// <returns>A linestring placemark (or an empty object)</returns>
        public static object CreateLineString(
            dynamic ge,
            dynamic start,
            dynamic end,
            string id = "",
            bool tessellate = true,
            bool addFeature = true)
        {
            if (!IsGe(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            dynamic placemark = null;
            dynamic lineString = null;

            try
            {
                placemark = ge.createPlacemark(id);
                lineString = ge.createLineString(String.Empty);
                lineString.setTessellate(Convert.ToInt16(tessellate));
                lineString.getCoordinates().pushLatLngAlt(start.getLatitude(), start.getLongitude(), start.getAltitude());
                lineString.getCoordinates().pushLatLngAlt(end.getLatitude(), end.getLongitude(), end.getAltitude());
                placemark.setGeometry(lineString);
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("CreateLineString: " + rbex.ToString(), "GEHelpers");
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
        public static void EnableLayerById(dynamic ge, string id, bool value)
        {
            if (!IsGe(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            try
            {
                ge.getLayerRoot().EnableLayerById(id.ToString(), value);
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("GetAllFeaturesKml: " + rbex.ToString(), "GEHelpers");
            }
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
                throw new ArgumentException("ge is not of the type GEPlugin");
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
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("GetAllFeaturesKml: " + rbex.ToString(), "GEHelpers");
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
                throw new ArgumentException("ge is not of the type GEPlugin");
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
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("GetCurrentViewAsPoint: " + rbex.ToString(), "GEHelpers");
            }

            if (null == point)
            {
                return new object { };
            }

            return point;
        }

        /// <summary>
        /// Depreciated: now the dynamic type is used and getType can be called directly.
        /// Get the type of an active scripting object from a generic runtime callable wrapper.
        /// This method attempts to invoke the member 'getType' on the given object.
        /// </summary>
        /// <param name="wrapper">The com object wrapper</param>
        /// <returns>The name of the type, or an empty string on failure</returns>
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
            catch (NullReferenceException)
            {
            }

            return type;
        }

        /// <summary>
        /// Checks if the given object in an RCW is of the type GEPlugin
        /// </summary>
        /// <param name="wrapper">The object to check</param>
        /// <returns>true if the object is of the type GEPlugin </returns>
        public static bool IsGe(dynamic wrapper)
        {
            try
            {
                return wrapper.getType() == ApiType.GEPlugin;
            }
            catch (RuntimeBinderException)
            {
                return false;
            }
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
            double range = 1000)
        {
            if (!IsGe(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            dynamic lookat = null;

            try
            {
                lookat = ge.createLookAt(id);
                lookat.set(latitude, longitude, altitude, altitudeMode, heading, tilt, range);
                ge.getView().setAbstractView(lookat);
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("LookAt: " + rbex.ToString(), "GEHelpers");
                return false;
            }

            if (null == lookat)
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
            dynamic ge = browser.Plugin;
            dynamic abstractView = null;
            string type = feature.getType();

            switch (type)
            {
                case ApiType.KmlFolder:
                case ApiType.KmlDocument:
                    {
                        if (feature.getAbstractView() != null)
                        {
                            abstractView = feature.getAbstractView();
                        }
                    }

                    break;

                case ApiType.KmlNetworkLink:
                    {
                        if (feature.getAbstractView() != null)
                        {
                            abstractView = feature.getAbstractView();
                        }
                        else
                        {
                            string linkUrl = KmlHelpers.GetUrl(feature);
                            dynamic kmlObject = browser.FetchKmlSynchronous(linkUrl);

                            if (kmlObject != null && kmlObject.getOwnerDocument() != null)
                            {
                                abstractView = kmlObject.getOwnerDocument().getAbstractView();
                            }
                        }
                    }

                    break;

                case ApiType.KmlPoint:
                    return LookAt(ge, feature.getLatitude(), feature.getLongitude());

                case ApiType.KmlPolygon:
                    {
                        // TODO - make this work better!
                        dynamic p = feature.getOuterBoundary().getCoordinates().get(0);
                        return LookAt(ge, p.getLatitude(), p.getLongitude(), p.getAltitude());
                    }

                case ApiType.KmlPlacemark:
                    {
                        if (feature.getAbstractView() != null)
                        {
                            abstractView = feature.getAbstractView();
                        }
                        else
                        {
                            if (feature.getGeometry() != null)
                            {
                                return LookAt(feature.getGeometry(), browser);
                            }
                            else
                            {
                                // nothing to look at!
                                return false;
                            }
                        }
                    }

                    break;

                case ApiType.KmlLineString:
                    {
                        // TODO - make this work better!
                        return LookAt(
                            ge,
                            feature.getCoordinates().get(0).getLatitude(),
                            feature.getCoordinates().get(0).getLongitude());
                    }

                case ApiType.KmlMultiGeometry:
                    {
                        return LookAt(feature.getGeometries().getFirstChild(), browser);
                    }

                default:
                    return false;
            }

            if (abstractView != null)
            {
                ge.getView().setAbstractView(abstractView);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Opens the balloon for the given feature in the plugin using OpenFeatureBalloon()
        /// </summary>
        /// <param name="ge">the plugin instance</param>
        /// <param name="feature">the feature to open a balloon for</param>
        /// <param name="minWidth">Optional minimum balloon width</param>
        /// <param name="minHeight">Optional minimum balloon height</param>
        /// <param name="maxWidth">Optional maximum balloon width</param>
        /// <param name="maxHeight">Optional maximum balloon height</param>
        /// <param name="setBalloon">Optionally set the balloon to be the current in the plugin</param>
        /// <returns>The feature balloon</returns>
        public static dynamic OpenFeatureBalloon(
            dynamic ge,
            dynamic feature,
            int minWidth = 0,
            int minHeight = 0,
            int maxWidth = 800,
            int maxHeight = 600,
            bool setBalloon = true)
        {
            dynamic balloon = null;

            try
            {
                string type = feature.getType();

                if (type == ApiType.KmlFolder || type == ApiType.KmlDocument || feature.getGeometry() == null)
                {
                    balloon = ge.createHtmlStringBalloon(String.Empty);
                    string content = feature.getBalloonHtml();

                    // Scrubbing string...
                    // see: http://code.google.com/apis/earth/documentation/balloons.html
                    if (setBalloon && content != "<!--\nContent-type: mhtml-die-die-die\n\n-->")
                    {
                        balloon.setContentString(content);
                        ge.setBalloon(balloon);
                    }

                    return balloon;
                }

                balloon = ge.createFeatureBalloon(String.Empty);
                balloon.setFeature(feature);
                balloon.setMinHeight(minHeight);
                balloon.setMaxHeight(maxHeight);
                balloon.setMinWidth(minWidth);
                balloon.setMaxWidth(maxWidth);

                if (setBalloon)
                {
                    ge.setBalloon(balloon);
                }
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("OpenFeatureBalloon: " + rbex.ToString(), "GEHelpers");
            }

            if (null == balloon)
            {
                return new object { };
            }

            return balloon;
        }

        /// <summary>
        /// Opens the balloon for the given feature in the plugin using getBalloonHtmlUnsafe()
        /// </summary>
        /// <param name="ge">the plugin instance</param>
        /// <param name="feature">the feature to open a balloon for</param>
        /// <param name="minWidth">Optional minimum balloon width</param>
        /// <param name="minHeight">Optional minimum balloon height</param>
        /// <param name="maxWidth">Optional maximum balloon width</param>
        /// <param name="maxHeight">Optional maximum balloon height</param>
        /// <param name="setBalloon">Optionally set the balloon to be the current in the plugin</param>
        /// <returns>The unsafe html balloon</returns>
        public static dynamic OpenBalloonHtmlUnsafe(
            dynamic ge,
            dynamic feature,
            int minWidth = 0,
            int minHeight = 0,
            int maxWidth = 800,
            int maxHeight = 600,
            bool setBalloon = true)
        {
            if (!IsGe(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            dynamic balloon = null;

            try
            {
                balloon = ge.createHtmlStringBalloon(string.Empty);
                balloon.setContentString(feature.getBalloonHtmlUnsafe());
                balloon.setMinHeight(minHeight);
                balloon.setMaxHeight(maxHeight);
                balloon.setMinWidth(minWidth);
                balloon.setMaxWidth(maxWidth);
                balloon.setFeature(feature); // optional?
                ge.setBalloon(balloon);
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("OpenBalloonHtmlUnsafe: " + rbex.ToString(), "GEHelpers");
            }

            if (null == balloon)
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
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            try
            {
                dynamic features = ge.getFeatures();
                while (features.getLastChild() != null)
                {
                    features.removeChild(features.getLastChild());
                }
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("RemoveAllFeatures: " + rbex.ToString(), "GEHelpers");
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
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            dynamic lookat = null;
            double range = 0;
            double zoom = 0;
            string url = string.Empty;

            try
            {
                // Get the current view 
                lookat = ge.getView().copyAsLookAt(ge.ALTITUDE_RELATIVE_TO_GROUND);
                range = lookat.getRange();

                // calculate the equivelent zoom level from the given range
                zoom = Math.Round(26 - (Math.Log(lookat.getRange()) / Math.Log(2)));

                // Google Maps have an integer "zoom level" which defines the resolution of the current view.
                // Zoom levels between 0 (entire world on map) to 21+ (down to individual buildings) are possible.
                zoom = Math.Min(Math.Max(zoom, 1), 21);

                // Build the maps url
                StringBuilder sb = new StringBuilder("http://maps.google.co.uk/maps?ll=");
                sb.Append(lookat.getLatitude());
                sb.Append(",");
                sb.Append(lookat.getLongitude());
                sb.Append("&z=");
                sb.Append(zoom);
                url = sb.ToString();
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("ShowCurrentViewInMaps: " + rbex.ToString(), "GEHelpers");
            }

            // launch the default browser with the url
            Process.Start(url);
        }
    }
}