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
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using FC.GEPluginCtrls.Geo;
    using Microsoft.CSharp.RuntimeBinder;

    /// <summary>
    /// This class provides some basic Google Earth plug-in helpers functions.
    /// Interfaces whose names begin with GE allow for programmatic access to core plug-in functionality and other miscellaneous options.
    /// It is based on the "GEHelpers JavaScript library" by Roman Nurik
    /// </summary>
    public static class GEHelpers
    {
        // See: http://earth-api-samples.googlecode.com/svn/trunk/lib/geplugin-helpers.js

        /// <summary>
        /// This is a simple "ge.getFeatures().appendChild()" wrapper
        /// </summary>
        /// <param name="ge">the plug-in instance to add the features to</param>
        /// <param name="kml">the features to add</param>
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
                Debug.WriteLine("AddFeaturesToPlugin: " + rbex.Message, "GEHelpers");
            }
            catch (COMException cex)
            {
                Debug.WriteLine("AddFeaturesToPlugin: " + cex.Message, "GEHelpers");
            }
        }

        /// <summary>
        /// Enables or disables a plug-in layer - wrapper for ge.getLayerRoot().enableLayerById()
        /// </summary>
        /// <param name="ge">The plug-in instance</param>
        /// <param name="layer">The id of the layer to work with</param>
        /// <param name="value">True turns the layer on, false off</param>
        public static void EnableLayer(dynamic ge, Layer layer, bool value)
        {
            if (!IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            ////ge.getLayerRoot().enableLayerById(layer.GetId(), value);
            foreach (Layer eachLayer in Enum.GetValues(typeof(Layer)))
            {
                if (eachLayer.Equals(Layer.None) || !layer.HasFlag(eachLayer))
                {
                    continue;
                }

                try
                {
                    ge.getLayerRoot().enableLayerById(eachLayer.GetId(), value);
                }
                catch (RuntimeBinderException rbex)
                {
                    Debug.WriteLine("GetAllFeaturesKml: " + rbex, "GEHelpers");
                }
                catch (COMException)
                {
                }
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
                feature = ge.getElementById("#" + id);
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("GetElementById: " + rbex.Message, "GEHelpers");
            }

            return feature;
        }

        /// <summary>
        /// Attempts to set the view of the plug-in to the given API object 
        /// </summary>
        /// <param name="ge">the plug-in</param>
        /// <param name="feature">the API object</param>
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
            if (!IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            try
            {
                if (feature.getAbstractView() != null)
                {
                    ge.getView().setAbstractView(feature.getAbstractView());
                }
                else if (boundsFallback)
                {
                    Bounds bounds = KmlHelpers.ComputeBounds(feature);

                    if (!bounds.IsEmpty)
                    {
                        KmlHelpers.SetBoundsView(
                            ge,
                            bounds,
                            aspectRatio,
                            defaultRange,
                            scaleRange);
                    }
                }
            }
            catch (COMException)
            {
            }
        }

        /// <summary>
        /// Gets the Kml of all the features in the plug-in
        /// </summary>
        /// <param name="ge">The plug-in</param>
        /// <returns>String of all the Kml from the plug-in - or an empty string</returns>
        public static string GetAllFeaturesKml(dynamic ge)
        {
            if (!IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            StringBuilder kml = new StringBuilder();

            try
            {
                dynamic children = KmlHelpers.GetChildNodes(ge);
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
                Debug.WriteLine("GetAllFeaturesKml: " + rbex.Message, "GEHelpers");
            }

            return kml.ToString();
        }

        /// <summary>
        /// Get the current plug-in view as a point object
        /// </summary>
        /// <param name="ge">the plug-in</param>
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
                Debug.WriteLine("GetCurrentViewAsPoint: " + rbex.Message, "GEHelpers");
            }

            return false;
        }

        /// <summary>
        /// Depreciated: now the dynamic type is used and getType can be called directly.
        /// Gets the type of an active scripting object from a generic runtime callable wrapper.
        /// This method attempts to invoke the member 'getType' on the given object.
        /// </summary>
        /// <param name="wrapper">The com object wrapper</param>
        /// <returns>The name of the type, or an empty string on failure</returns>
        public static string GetTypeFromWrapper(object wrapper)
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
        /// Gets the Google API type of a given <paramref name="feature">feature</paramref>
        /// </summary>
        /// <param name="feature">The feature to get the type of</param>
        /// <returns>
        /// The type of the <paramref name="feature">feature</paramref> or 'ApiType.None'.
        /// </returns>
        public static ApiType GetApiType(dynamic feature)
        {
            if (feature != null)
            {
                try
                {
                    string type = feature.getType();

                    if (Enum.IsDefined(typeof(ApiType), type))
                    {
                        return (ApiType)Enum.Parse(typeof(ApiType), type);
                    }
                }
                catch (RuntimeBinderException rbex)
                {
                    Debug.WriteLine("GetApiType: " + rbex.Message, "GEHelpers");
                }
            }

            return ApiType.None;
        }

        /// <summary>
        /// Checks if the given object in an RCW is of the type GEPlugin
        /// </summary>
        /// <param name="feature">The object to check</param>
        /// <returns>true if the object is of the type GEPlugin </returns>
        public static bool IsGE(dynamic feature)
        {
            return IsApiType(feature, ApiType.GEPlugin);
        }

        /// <summary>
        /// Checks if a given <paramref name="feature">feature</paramref>
        /// is of a given type <paramref name="type">type</paramref>
        /// </summary>
        /// <param name="feature">The feature to test</param>
        /// <param name="type">The type to check</param>
        /// <returns>
        /// True if the <paramref name="feature">feature</paramref>
        /// is the given <paramref name="type">type</paramref>
        /// </returns>
        public static bool IsApiType(dynamic feature, ApiType type)
        {
            return GetApiType(feature) == type;
        }

        /// <summary>
        /// Checks if a given string <paramref name="input"/> is a valid URI
        /// of the given <paramref name="kind"/>
        /// </summary>
        /// <param name="input">the string to check</param>
        /// <param name="kind">the kind of URI to check for</param>
        /// <returns>true if the string is a URI</returns>
        public static bool IsUri(string input, UriKind kind)
        {
            Uri x;
            return Uri.TryCreate(input, kind, out x);
        }

        /// <summary>
        /// Opens the balloon for the given feature in the plug-in using OpenFeatureBalloon()
        /// </summary>
        /// <param name="ge">the plug-in instance</param>
        /// <param name="feature">the feature to open a balloon for</param>
        /// <param name="useUnsafeHtml">Optional setting to use getBalloonHtmlUnsafe, default is false</param>
        /// <param name="minWidth">Optional minimum balloon width, default is 100</param>
        /// <param name="minHeight">Optional minimum balloon height, default is 100</param>
        /// <param name="maxWidth">Optional maximum balloon width, default is 800</param>
        /// <param name="maxHeight">Optional maximum balloon height, default is 600</param>
        /// <param name="setBalloon">Optionally set the balloon to be the current in the plug-in</param>
        /// <returns>The feature balloon or null</returns>
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
            if (!IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            string content = string.Empty;

            try
            {
                ge.setBalloon(null);
                content = useUnsafeHtml ? feature.getBalloonHtmlUnsafe() : feature.getBalloonHtml();
            }
            catch (COMException cex)
            {
                Debug.WriteLine("OpenFeatureBalloon: " + cex.Message, "GEHelpers");
            }

            // Scrubbing string...
            // see: http://code.google.com/apis/earth/documentation/balloons.html
            if (string.IsNullOrEmpty(content) || content == "<!--\nContent-type: mhtml-die-die-die\n\n-->")
            {
                return null;
            }

            dynamic balloon = KmlHelpers.CreateHtmlStringBalloon(
                ge,
                content,
                minWidth,
                minHeight,
                maxWidth,
                maxHeight,
                setBalloon);

            balloon.setFeature(feature);

            return balloon;
        }

        /// <summary>
        /// Remove all features from the plug-in 
        /// </summary>
        /// <param name="ge">The plug-in instance</param>
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
                    features.getLastChild().release();
                }
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("RemoveAllFeatures: " + rbex.Message, "GEHelpers");
            }
        }

        /// <summary>
        /// Remove a feature from the plug-in based on the feature ID
        /// </summary>
        /// <param name="ge">The plug-in instance</param>
        /// <param name="id">The id of the feature to remove</param>
        public static void RemoveFeatureById(dynamic ge, string id)
        {
            if (!IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            dynamic feature = GEHelpers.GetElementById(ge, id);

            if (feature != null)
            {
                try
                {
                    ge.getFeatures().removeChild(feature);
                    feature.release();
                }
                catch (RuntimeBinderException rbex)
                {
                    Debug.WriteLine("RemoveFeatureById: " + rbex.Message, "GEHelpers");
                }
            }
        }

        /// <summary>
        /// Remove a features from the plug-in based on the feature IDs
        /// </summary>
        /// <param name="ge">The plug-in instance</param>
        /// <param name="ids">The ids of the features to remove</param>
        public static void RemoveFeatureById(dynamic ge, string[] ids)
        {
            if (!IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            foreach (string id in ids)
            {
                GEHelpers.RemoveFeatureById(ge, id);
            }
        }

        /// <summary>
        /// Displays the current plug-in view in Google Maps using the default system browser
        /// </summary>
        /// <param name="ge">The plug-in instance</param>
        [SecurityCritical]
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static void ShowCurrentViewInMaps(dynamic ge)
        {
            if (!IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            string url = "about:blank";

            try
            {
                // Get the current view 
                dynamic lookat = ge.getView().copyAsLookAt(ge.ALTITUDE_RELATIVE_TO_GROUND);
 
                // calculate the equivalent zoom level from the given range.
                double equivalent = Math.Round(26 - (Math.Log(lookat.getRange()) / Math.Log(2)));

                // Google Maps have an integer "zoom level" which defines the resolution of the current view.
                // Zoom levels between 0 (entire world on map) to 21+ (down to individual buildings) are possible.
                double zoom = Maths.ConstrainValue(equivalent, 1, 21);
                
                // Build the maps URL
                url = string.Format(
                    "https://maps.google.co.uk/maps?ll={0},{1}&z={2}",
                    lookat.getLatitude(),
                    lookat.getLongitude(),
                    zoom);
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("ShowCurrentViewInMaps: " + rbex.Message, "GEHelpers");
            }

            // launch the default browser with the URL
            Process.Start(url);
        }

        /// <summary>
        /// Toggles any 'media player' associated with a particular Kml type represented by a tree node.
        /// So far this includes KmlTours (GETourPlayer) and KmlPhotoOverlays (GEPhotoOverlayViewer)
        /// </summary>
        /// <param name="ge">The plug-in instance</param>
        /// <param name="feature">The feature to check</param>
        /// <param name="visible">Value indicating whether the player should be visible or not.</param>
        public static void ToggleMediaPlayer(dynamic ge, dynamic feature, bool visible = true)
        {
            if (!IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            ApiType type = GEHelpers.GetApiType(feature);

            if (visible)
            {
                if (type == ApiType.KmlTour)
                {
                    ge.setBalloon(null);
                    ge.getTourPlayer().setTour(feature);
                }
                else if (type == ApiType.KmlPhotoOverlay)
                {
                    ge.getPhotoOverlayViewer().setPhotoOverlay(feature);
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