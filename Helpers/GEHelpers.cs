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
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using FC.GEPluginCtrls.Geo;
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
            if (!IsGE(ge))
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
        /// <param name="id">Optional placemark Id. Default is empty</param>
        /// <param name="latitude">The placemark latitude in decimal degrees</param>
        /// <param name="longitude">The placemark longitude in decimal degrees</param>
        /// <param name="altitude">Optional placemark altitude in metres. Default is 0</param>
        /// <param name="altitudeMode">Optional altitudeMode. Default is AltitudeMode.RelativeToGround</param>
        /// <param name="name">Optional name of the placemark. Default is empty</param>
        /// <param name="description">Optional placemark description text. Default is empty</param>
        /// <param name="addFeature">Optionally adds the placemark directly to the plugin. Default is true</param>
        /// <returns>A placemark (or an empty object)</returns>
        public static dynamic CreatePlacemark(
            dynamic ge,
            string id = "",
            double latitude = 0,
            double longitude = 0,
            double altitude = 0,
            AltitudeMode altitudeMode = AltitudeMode.RelativeToGround,
            string name = "",
            string description = "",
            bool addFeature = true)
        {
            if (!IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            dynamic placemark = new object();

            try
            {
                dynamic point = CreatePoint(
                    ge,
                    string.Empty,
                    Maths.FixLatitude(latitude), 
                    Maths.FixLongitude(longitude), 
                    altitude,
                    altitudeMode);

                placemark = ge.createPlacemark(id);
                placemark.setGeometry(point);
                placemark.setName(name);
                placemark.setDescription(description);

                if (addFeature)
                {
                    GEHelpers.AddFeaturesToPlugin(ge, placemark);
                }
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("CreatePlacemark: " + rbex.ToString(), "GEHelpers");
            }

            return placemark;
        }

        /// <summary>
        /// Creates a kml placemark
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="kmlPoint">A KmlPoint object to use for the placemark location.</param>
        /// <param name="id">Optional placemark Id. Default is empty</param>
        /// <param name="name">Optional name of the placemark. Default is empty</param>
        /// <param name="description">Optional placemark description text. Default is empty</param>
        /// <param name="addFeature">Optionally adds the placemark directly to the plugin. Default is true</param>
        /// <returns>A placemark (or false)</returns>
        public static dynamic CreatePlacemark(
            dynamic ge,
            dynamic kmlPoint,
            string id = "",
            string name = "",
            string description = "",
            bool addFeature = true)
        {
            dynamic placemark = new object();

            if (kmlPoint.getType() != ApiType.KmlPoint)
            {
                throw new ArgumentException("the kmlPoint parameter must be of the type KmlPoint");
            }

            placemark = CreatePlacemark(
                ge,
                name: name,
                id: id, 
                description: description,
                addFeature: addFeature);
            
            placemark.setGeometry(kmlPoint);

            return placemark;
        }

        /// <summary>
        /// Creates a kml placemark
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="coordinate">A Coordinate to use as the placemarks location</param>
        /// <param name="id">Optional placemark Id. Default is empty</param>
        /// <param name="name">Optional name of the placemark. Default is empty</param>
        /// <param name="description">Optional placemark description text. Default is empty</param>
        /// <param name="addFeature">Optionally adds the placemark directly to the plugin. Default is true</param>
        /// <returns>A placemark (or false)</returns>
        public static dynamic CreatePlacemark(
            dynamic ge,
            Coordinate coordinate,
            string id = "",
            string name = "",
            string description = "",
            bool addFeature = true)
        {
            return CreatePlacemark(
            ge: ge,
            latitude: coordinate.Latitude,
            longitude: coordinate.Longitude,
            altitude: coordinate.Altitude,
            altitudeMode: coordinate.AltitudeMode,
            name: name,
            id: id,
            description: description,
            addFeature: addFeature);
        }

        /// <summary>
        /// Creates a kml point
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="id">Optional placemark Id. Default is empty</param>
        /// <param name="latitude">The placemark latitude in decimal degrees</param>
        /// <param name="longitude">The placemark longitude in decimal degrees</param>
        /// <param name="altitude">Optional placemark altitude in metres. Default is 0</param>
        /// <param name="altitudeMode">Optional altitudeMode. Default is AltitudeMode.RelativeToGround</param>
        /// <returns>A Kml point (or false)</returns>
        public static dynamic CreatePoint(
            dynamic ge,
            string id = "",
            double latitude = 0,
            double longitude = 0,
            double altitude = 0,
            AltitudeMode altitudeMode = AltitudeMode.RelativeToGround)
        {
            if (!IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            dynamic point = new object();

            try
            {
                point = ge.createPoint(id);
                point.setLatitude(latitude);
                point.setLongitude(longitude);
                point.setAltitude(altitude);
                point.setAltitudeMode(altitudeMode);
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("CreatePoint: " + rbex.ToString(), "GEHelpers");
            }

            return point;
        }

        /// <summary>
        /// Creates a kml point from a Coordinate
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="coordinate">The Coordinate to base the point on</param>
        /// <param name="id">Optional point Id. Default is empty</param>
        /// <returns>a kml point</returns>
        public static dynamic CreatePoint(
            dynamic ge,
            Coordinate coordinate,
            string id = "")
        {
            return CreatePoint(
            ge: ge,
            id: id,
            latitude: coordinate.Latitude,
            longitude: coordinate.Longitude,
            altitude: coordinate.Altitude,
            altitudeMode: coordinate.AltitudeMode);
        }

        /// <summary>
        /// Draws a line string between the given placemarks or points
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="start">The first placemark or point</param>
        /// <param name="end">The second placemark or point</param>
        /// <param name="id">Optional ID of the linestring placemark. Default is empty</param>
        /// <param name="tessellate">Optionally sets tessellation for the linestring. Default is true</param>
        /// <param name="addFeature">Optionally adds the linestring directly to the plugin. Default is true</param>
        /// <returns>A linestring placemark (or false)</returns>
        public static object CreateLineString(
            dynamic ge,
            dynamic start,
            dynamic end,
            string id = "",
            bool tessellate = true,
            bool addFeature = true)
        {
            if (!IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            if (start.getType() == ApiType.KmlPlacemark)
            {
                start = start.getGeometry();
            }

            if (end.getType() == ApiType.KmlPlacemark)
            {
                end = end.getGeometry();
            }

            try
            {
                dynamic placemark = CreatePlacemark(ge, addFeature: addFeature);
                dynamic lineString = ge.createLineString(id);
                lineString.setTessellate(Convert.ToUInt16(tessellate));
                lineString.getCoordinates().pushLatLngAlt(start.getLatitude(), start.getLongitude(), start.getAltitude());
                lineString.getCoordinates().pushLatLngAlt(end.getLatitude(), end.getLongitude(), end.getAltitude());
                placemark.setGeometry(lineString);

                if (addFeature)
                {
                    GEHelpers.AddFeaturesToPlugin(ge, placemark);
                }

                return placemark;
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("CreateLineString: " + rbex.ToString(), "GEHelpers");
            }

            return false;
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
        /// <param name="setView">Optional set the current view to the lookAt</param>
        /// <returns>true on success</returns>
        public static dynamic CreateLookAt(
            dynamic ge,
            double latitude,
            double longitude,
            string id = "",
            double altitude = 0,
            AltitudeMode altitudeMode = AltitudeMode.RelativeToGround,
            double heading = 0,
            double tilt = 0,
            double range = 1000,
            bool setView = true)
        {
            if (!IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            try
            {
                dynamic lookat = ge.createLookAt(id);
                lookat.set(latitude, longitude, altitude, altitudeMode, heading, tilt, range);

                if (setView)
                {
                    ge.getView().setAbstractView(lookat);
                }

                return lookat;
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("CreateLookAt: " + rbex.ToString(), "GEHelpers");
            }

            return false;
        }

        /// <summary>
        /// Creates an Html String Balloon
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="html">The balloon content html string</param>
        /// <param name="minWidth">Optional minimum balloon width, default is 100</param>
        /// <param name="minHeight">Optional minimum balloon height, default is 100</param>
        /// <param name="maxWidth">Optional maximum balloon width, default is 800</param>
        /// <param name="maxHeight">Optional maximum balloon height, default is 600</param>
        /// <param name="setBalloon">Optionally set the balloon to be the current in the plugin</param>
        /// <returns>The feature balloon (or false)</returns>
        public static dynamic CreateHtmlStringBalloon(
            dynamic ge,
            string html = "",
            int minWidth = 0,
            int minHeight = 0,
            int maxWidth = 800,
            int maxHeight = 600,
            bool setBalloon = true)
        {
            try
            {
                dynamic balloon = ge.createHtmlStringBalloon(string.Empty);
                balloon.setContentString(html);
                balloon.setMinHeight(minHeight);
                balloon.setMaxHeight(maxHeight);
                balloon.setMinWidth(minWidth);
                balloon.setMaxWidth(maxWidth);

                if (setBalloon)
                {
                    ge.setBalloon(balloon);
                }

                return balloon;
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("OpenFeatureBalloon: " + rbex.ToString(), "GEHelpers");
            }

            return false;
        }

        /// <summary>
        /// Enables or disables a plugin layer - wrapper for ge.getLayerRoot().enableLayerById()
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="id">The id of the layer to work with</param>
        /// <param name="value">True turns the layer on, false off</param>
        public static void EnableLayerById(dynamic ge, Layer id, bool value)
        {
            if (!IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            try
            {
                ge.getLayerRoot().EnableLayerById(StringEnum.GetStringValue(id), value);
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("GetAllFeaturesKml: " + rbex.ToString(), "GEHelpers");
            }
        }
        
        /// <summary>
        /// Get an element by ID - wrapper for ge.getElementById()
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="id">The id feature to find</param>
        /// <returns>The feature specified by the ID parameter if it exists</returns>
        public static dynamic GetElementById(dynamic ge, string id)
        {
            dynamic feature = null;

            if (!IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            try
            {
                feature = ge.getElementById(id);
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("GetElementById: " + rbex.ToString(), "GEHelpers");
            }

            return feature;
        }

        /// <summary>
        /// Attempts to set the view of the plugin to the given api object 
        /// </summary>
        /// <param name="ge">the plugin</param>
        /// <param name="feature">the api object</param>
        /// <param name="boundsFallback">Optionally set whether to fallback to the bounds method</param>
        /// <param name="aspectRatio">Optional aspect ratio</param>
        /// <param name="defaultRange">Optional default range</param>
        /// <param name="scaleRange">Optional scale range</param>
        public static void FlyToObject(
            dynamic ge,
            dynamic feature,
            bool boundsFallback = true,
            double aspectRatio = 1.0,
            double defaultRange = 1000,
            double scaleRange = 1.5)
        {
            if (feature.getAbstractView() != null)
            {
                ge.getView().setAbstractView(feature.getAbstractView());
            }
            else if (boundsFallback)
            {
                KmlHelpers.SetBoundsView(
                    ge,
                    KmlHelpers.ComputeBounds(feature),
                    aspectRatio,
                    defaultRange,
                    scaleRange);
            }
        }

        /// <summary>
        /// Gets the Kml of all the features in the plug-in
        /// </summary>
        /// <param name="ge">The plugin</param>
        /// <returns>String of all the Kml from the plugin - or an empty string</returns>
        public static string GetAllFeaturesKml(dynamic ge)
        {
            if (!IsGE(ge))
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
        /// <returns>Point set to the current view (or false)</returns>
        public static dynamic GetCurrentViewAsPoint(dynamic ge)
        {
            if (!IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            try
            {
                dynamic lookat = ge.getView().copyAsLookAt(ge.ALTITUDE_RELATIVE_TO_GROUND);
                dynamic point = ge.createPoint(string.Empty);

                // latitude, longitude, altitude, altitudeMode, extrude, tessellate
                point.set(
                    lookat.getLatitude(),
                    lookat.getLongitude(),
                    lookat.getAltitude(),
                    ge.ALTITUDE_RELATIVE_TO_GROUND,
                    0,
                    0);

                return point;
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("GetCurrentViewAsPoint: " + rbex.ToString(), "GEHelpers");
            }

            return false;
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
                   BindingFlags.InvokeMethod,
                   null,
                   wrapper,
                   null, 
                   CultureInfo.InvariantCulture);
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
        public static bool IsGE(dynamic wrapper)
        {
            try
            {
                return wrapper.getType() == ApiType.GEPlugin;
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("IsGe: " + rbex.ToString(), "GEHelpers");
            }

            return false;
        }

        /// <summary>
        /// Opens the balloon for the given feature in the plugin using OpenFeatureBalloon()
        /// </summary>
        /// <param name="ge">the plugin instance</param>
        /// <param name="feature">the feature to open a balloon for</param>
        /// <param name="useUnsafeHtml">Optional setting to use getBalloonHtmlUnsafe, default is false</param>
        /// <param name="minWidth">Optional minimum balloon width, default is 100</param>
        /// <param name="minHeight">Optional minimum balloon height, default is 100</param>
        /// <param name="maxWidth">Optional maximum balloon width, default is 800</param>
        /// <param name="maxHeight">Optional maximum balloon height, default is 600</param>
        /// <param name="setBalloon">Optionally set the balloon to be the current in the plugin</param>
        /// <returns>The feature balloon (or false)</returns>
        public static dynamic OpenFeatureBalloon(
            dynamic ge,
            dynamic feature,
            bool useUnsafeHtml = false,
            int minWidth = 100,
            int minHeight = 100,
            int maxWidth = 800,
            int maxHeight = 600,
            bool setBalloon = true)
        {
            dynamic balloon = null;

            try
            {
                string type = feature.getType();
                string content = string.Empty;

                if (useUnsafeHtml)
                {
                    content = feature.getBalloonHtmlUnsafe();
                }
                else
                {
                    content = feature.getBalloonHtml();
                }

                // Scrubbing string...
                // see: http://code.google.com/apis/earth/documentation/balloons.html
                if (string.IsNullOrEmpty(content) || content == "<!--\nContent-type: mhtml-die-die-die\n\n-->")
                {
                    // no content...
                    return false;
                }

                balloon = CreateHtmlStringBalloon(
                    ge,
                    content,
                    minWidth,
                    minHeight,
                    maxWidth,
                    maxHeight,
                    setBalloon);

                if (type != ApiType.KmlFolder && type != ApiType.KmlDocument && feature.getGeometry() != null)
                {
                    balloon.setFeature(feature);
                }

                return balloon;
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("OpenFeatureBalloon: " + rbex.ToString(), "GEHelpers");
            }

            return false;
        }

        /// <summary>
        /// Remove all features from the plugin 
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        public static void RemoveAllFeatures(dynamic ge)
        {
            if (!IsGE(ge))
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
        /// Remove a feature from the plug-in based on the feature ID
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="id">The id of the feature to remove</param>
        public static void RemoveFeatureById(dynamic ge, string id)
        {
            dynamic feature = GEHelpers.GetElementById(ge, id);
            if (feature != null)
            {
                try
                {
                    ge.getFeatures().removeChild(feature);
                }
                catch (RuntimeBinderException rbex)
                {
                    Debug.WriteLine("RemoveFeatureById: " + rbex.ToString(), "GEHelpers");
                }
            }
        }

        /// <summary>
        /// Remove a features from the plug-in based on the feature IDs
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="ids">The ids of the features to remove</param>
        public static void RemoveFeatureById(dynamic ge, string[] ids)
        {
            foreach (string id in ids)
            {
                GEHelpers.RemoveFeatureById(ge, id);
            }
        }

        /// <summary>
        /// Displays the current plugin view in Google Maps using the default system browser
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        public static void ShowCurrentViewInMaps(dynamic ge)
        {
            if (!IsGE(ge))
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

        /// <summary>
        /// Toggles any 'media player' associated with a particular Kml type represented by a treenode.
        /// So far this includes KmlTours (GETourPlayer) and KmlPhotoOverlays (GEPhotoOverlayViewer)
        /// </summary>
        /// <param name="ge">The plugin instance</param>
        /// <param name="node">The KmlTreeViewNode to check</param>
        /// <param name="visible">Vaule indicating whether the player should be visible or not.</param>
        public static void ToggleMediaPlayer(dynamic ge, dynamic node, bool visible = true)
        {
            string type = string.Empty;

            try
            {
                type = node.getType();
            }
            catch (COMException)
            {
                return;
            }

            if (visible)
            {
                if (type == ApiType.KmlTour)
                {
                    ge.getTourPlayer().setTour(node);
                }
                else if (type == ApiType.KmlPhotoOverlay)
                {
                    ge.getPhotoOverlayViewer().setPhotoOverlay(node);
                }
            }
            else
            {
                if (type == ApiType.KmlTour)
                {
                    ge.getTourPlayer().setTour(null);
                }
                else if (type == ApiType.KmlPhotoOverlay)
                {
                    ge.getPhotoOverlayViewer().setPhotoOverlay(null);
                }
            }
        }
    }
}