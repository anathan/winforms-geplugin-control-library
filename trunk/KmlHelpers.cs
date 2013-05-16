// <copyright file="KmlHelpers.cs" company="FC">
// Copyright (c) 2008 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2009-10-04</date>
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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Xml;
    using FC.GEPluginCtrls.Geo;
    using Microsoft.CSharp.RuntimeBinder;

    /// <summary>
    /// This class provides basic Kml helper methods
    /// </summary>
    public static class KmlHelpers
    {
        /// <summary>
        /// Computes the bounding box for the given object.
        /// Note that this method walks the object's DOM, so may have poor performance for large objects.
        /// </summary>
        /// <param name="kmlFeature">{KmlFeature|KmlGeometry} object The feature or geometry whose bounds should be computed</param>
        /// <returns>A bounds object based on the <paramref name="kmlFeature"/> (or an empty bounds object)</returns>
        /// <remarks>
        /// Based on the methods at:
        /// http://code.google.com/p/earth-api-utility-library/source/browse/trunk/extensions/src/dom/utils.js
        /// </remarks>
        public static Bounds ComputeBounds(dynamic kmlFeature)
        {
            Bounds bounds = new Bounds();
            Action<dynamic> eachNode = (feature) =>
            {
                ApiType type = GEHelpers.GetApiType(feature);
                switch (type)
                {
                    case ApiType.KmlGroundOverlay:
                        dynamic llb = feature.getLatLonBox();

                        if (llb != null)
                        {
                            double alt = feature.getAltitude();
                            bounds.Extend(new Coordinate(llb.getNorth(), llb.getEast(), alt));
                            bounds.Extend(new Coordinate(llb.getNorth(), llb.getWest(), alt));
                            bounds.Extend(new Coordinate(llb.getSouth(), llb.getEast(), alt));
                            bounds.Extend(new Coordinate(llb.getSouth(), llb.getWest(), alt));
                        }

                        break;

                    case ApiType.KmlModel:
                        bounds.Extend(new Coordinate(feature.getLocation()));
                        break;

                    case ApiType.KmlLinearRing:
                    case ApiType.KmlLineString:
                        dynamic coords = feature.getCoordinates();

                        if (coords != null)
                        {
                            int count = coords.getLength();
                            for (int i = 0; i < count; i++)
                            {
                                bounds.Extend(new Coordinate(coords.get(i)));
                            }
                        }

                        break;

                    case ApiType.KmlCoord:
                    case ApiType.KmlLocation:
                    case ApiType.KmlPoint:
                        bounds.Extend(new Coordinate(feature));
                        break;

                    case ApiType.KmlPlacemark:
                        dynamic geometry = feature.getGeometry();
                        if (GEHelpers.IsApiType(geometry, ApiType.KmlPoint))
                        {
                            bounds.Extend(new Coordinate(geometry));
                        }

                        break;
                }
            };

            KmlHelpers.WalkKmlDom(kmlFeature, eachNode, true, true);

            return bounds;
        }

        /// <summary>
        /// Creates a kmlLookAt from a Bounds object.
        /// </summary>
        /// <param name="ge">the plug-in</param>
        /// <param name="bounds">the bounds object to create the view of</param>
        /// <param name="aspectRatio">Optional aspect ratio</param>
        /// <param name="defaultRange">Optional default range</param>
        /// <param name="scaleRange">Optional scale range</param>
        /// <returns>A KmlLookAt based on the <paramref name="bounds"/> (or null)</returns>
        public static dynamic CreateBoundsView(
            dynamic ge,
            Bounds bounds,
            double aspectRatio = 1.0,
            double defaultRange = 1000,
            double scaleRange = 1.5)
        {
            Coordinate center = bounds.Center();
            Coordinate boundsSpan = bounds.Span();
            double lookAtRange = defaultRange;
            dynamic lookat = null;

            if (Convert.ToBoolean(boundsSpan.Latitude) || Convert.ToBoolean(boundsSpan.Longitude))
            {
                // Distance - using law of cosines for speed...
                double distEW = new Coordinate(center.Latitude, bounds.East)
                   .Distance(new Coordinate(center.Latitude, bounds.West));

                double distNS = new Coordinate(bounds.North, center.Longitude)
                   .Distance(new Coordinate(bounds.South, center.Longitude));

                aspectRatio = Math.Min(Math.Max(aspectRatio, distEW / distNS), 1.0);

                // Create a LookAt using the experimentally derived distance formula.
                double alpha = Maths.ConvertDegreesToRadians(45.0 / (aspectRatio + 0.4) - 2.0);
                double expandToDistance = Math.Max(distNS, distEW);
                double beta = Math.Min(
                    Maths.ConvertDegreesToRadians(90),
                    alpha + expandToDistance / (2 * Maths.EarthMeanRadiusKilometres));

                lookAtRange = scaleRange * Maths.EarthMeanRadiusKilometres *
                    (Math.Sin(beta) * Math.Sqrt(1 + 1 / Math.Pow(Math.Tan(alpha), 2)) - 1);
            }

            try
            {
                lookat = ge.createLookAt(string.Empty);
                lookat.set(center.Latitude, center.Longitude, bounds.Top, bounds.Northeast.AltitudeMode, 0, 0, lookAtRange);
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("CreateBoundsView: " + rbex.Message, "KmlHelpers");
            }

            return lookat;
        }

        /// <summary>
        /// Creates a KML placemark
        /// </summary>
        /// <param name="ge">The plug-in instance</param>
        /// <param name="id">Optional placemark Id. Default is empty</param>
        /// <param name="latitude">The placemark latitude in decimal degrees</param>
        /// <param name="longitude">The placemark longitude in decimal degrees</param>
        /// <param name="altitude">Optional placemark altitude in metres. Default is 0</param>
        /// <param name="altitudeMode">Optional altitudeMode. Default is AltitudeMode.RelativeToGround</param>
        /// <param name="name">Optional name of the placemark. Default is empty</param>
        /// <param name="description">Optional placemark description text. Default is empty</param>
        /// <param name="addFeature">Optionally adds the placemark directly to the plug-in. Default is true</param>
        /// <returns>A placemark (or null)</returns>
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
            if (!GEHelpers.IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            dynamic placemark = null;

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
                Debug.WriteLine("CreatePlacemark: " + rbex.Message, "KmlHelpers");
            }
            catch (COMException cex)
            {
                Debug.WriteLine("CreatePlacemark: " + cex.Message, "KmlHelpers");
            }

            return placemark;
        }

        /// <summary>
        /// Creates a KML placemark
        /// </summary>
        /// <param name="ge">The plug-in instance</param>
        /// <param name="coordinate">A Coordinate to use as the place marks location</param>
        /// <param name="id">Optional placemark Id. Default is empty</param>
        /// <param name="name">Optional name of the placemark. Default is empty</param>
        /// <param name="description">Optional placemark description text. Default is empty</param>
        /// <param name="addFeature">Optionally adds the placemark directly to the plug-in. Default is true</param>
        /// <returns>A placemark (or null)</returns>
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
        /// Creates a KML point
        /// </summary>
        /// <param name="ge">The plug-in instance</param>
        /// <param name="id">Optional placemark Id. Default is empty</param>
        /// <param name="latitude">The placemark latitude in decimal degrees</param>
        /// <param name="longitude">The placemark longitude in decimal degrees</param>
        /// <param name="altitude">Optional placemark altitude in metres. Default is 0</param>
        /// <param name="altitudeMode">Optional altitudeMode. Default is AltitudeMode.RelativeToGround</param>
        /// <returns>A Kml point (or null)</returns>
        public static dynamic CreatePoint(
            dynamic ge,
            string id = "",
            double latitude = 0,
            double longitude = 0,
            double altitude = 0,
            AltitudeMode altitudeMode = AltitudeMode.RelativeToGround)
        {
            if (!GEHelpers.IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            dynamic point = null;

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
                Debug.WriteLine("CreatePoint: " + rbex.Message, "KmlHelpers");
            }

            return point;
        }

        /// <summary>
        /// Creates a KML point from a Coordinate
        /// </summary>
        /// <param name="ge">The plug-in instance</param>
        /// <param name="coordinate">The Coordinate to base the point on</param>
        /// <param name="id">Optional point Id. Default is empty</param>
        /// <returns>A KML point (or null)</returns>
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
        /// Creates an Html String Balloon
        /// </summary>
        /// <param name="ge">The plug-in instance</param>
        /// <param name="html">The balloon content html string</param>
        /// <param name="minWidth">Optional minimum balloon width, default is 100</param>
        /// <param name="minHeight">Optional minimum balloon height, default is 100</param>
        /// <param name="maxWidth">Optional maximum balloon width, default is 800</param>
        /// <param name="maxHeight">Optional maximum balloon height, default is 600</param>
        /// <param name="setBalloon">Optionally set the balloon to be the current in the plug-in</param>
        /// <param name="feature">Optionally associate the balloon with a feature</param>
        /// <param name="closeButtonEnabled">Optionally display a closed button on the balloon, default is true</param>
        /// <param name="backgroundColor">Optionally set the balloon backgroundColor in the #rrggbb format, default #FFFFFF</param>
        /// <param name="foregroundColor">Optionally set the balloon backgroundColor in the #rrggbb format, default #000000</param>
        /// <returns>A HtmlStringBalloon object (or null)</returns>
        public static dynamic CreateHtmlStringBalloon(
            dynamic ge,
            string html = "",
            int minWidth = 0,
            int minHeight = 0,
            int maxWidth = 800,
            int maxHeight = 600,
            bool setBalloon = true,
            dynamic feature = null,
            bool closeButtonEnabled = true,
            string backgroundColor = "#FFFFFF",
            string foregroundColor = "#000000")
        {
            dynamic balloon = null;

            try
            {
                balloon = ge.createHtmlStringBalloon(string.Empty);
                balloon.setMinHeight(minHeight);
                balloon.setMaxHeight(maxHeight);
                balloon.setMinWidth(minWidth);
                balloon.setMaxWidth(maxWidth);
                balloon.setCloseButtonEnabled(Convert.ToInt16(closeButtonEnabled));

                if (!string.IsNullOrEmpty(backgroundColor))
                {
                    balloon.setBackgroundColor(backgroundColor);
                }

                if (!string.IsNullOrEmpty(foregroundColor))
                {
                    balloon.setForegroundColor(foregroundColor);
                }

                balloon.setContentString(html);

                if (feature != null)
                {
                    balloon.setFeature(feature);
                }

                if (setBalloon)
                {
                    ge.setBalloon(balloon);
                }
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("OpenFeatureBalloon: " + rbex.Message, "KmlHelpers");
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            return balloon;
        }

        /// <summary>
        /// Draws a line string between the given place marks or points
        /// </summary>
        /// <param name="ge">The plug-in instance</param>
        /// <param name="start">The first placemark or point</param>
        /// <param name="end">The second placemark or point</param>
        /// <param name="id">Optional ID of the line string placemark. Default is empty</param>
        /// <param name="tessellate">Optionally sets tessellation for the line string. Default is true</param>
        /// <param name="addFeature">Optionally adds the line string directly to the plug-in. Default is true</param>
        /// <param name="width">Optional line string width, default is 1</param>
        /// <param name="color">Optional KmlColor, default is white/opaque</param>
        /// <returns>A line string placemark (or null)</returns>
        public static dynamic CreateLineString(
            dynamic ge,
            dynamic start,
            dynamic end,
            string id = "",
            bool tessellate = true,
            bool addFeature = true,
            int width = 1,
            KmlColor color = new KmlColor())
        {
            if (!GEHelpers.IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            if (GEHelpers.IsApiType(start, ApiType.KmlPlacemark))
            {
                start = start.getGeometry();
            }

            if (GEHelpers.IsApiType(end, ApiType.KmlPlacemark))
            {
                end = end.getGeometry();
            }

            dynamic placemark = null;

            try
            {
                placemark = CreatePlacemark(ge);

                dynamic lineString = ge.createLineString(id);
                lineString.setTessellate(Convert.ToUInt16(tessellate));
                lineString.getCoordinates().pushLatLngAlt(start.getLatitude(), start.getLongitude(), start.getAltitude());
                lineString.getCoordinates().pushLatLngAlt(end.getLatitude(), end.getLongitude(), end.getAltitude());

                if (placemark.getStyleSelector() == null)
                {
                    placemark.setStyleSelector(ge.createStyle(string.Empty));
                }

                dynamic lineStyle = placemark.getStyleSelector().getLineStyle();
                lineStyle.setWidth(width);
                lineStyle.getColor().set(color.ToString());

                placemark.setGeometry(lineString);

                if (addFeature)
                {
                    GEHelpers.AddFeaturesToPlugin(ge, placemark);
                }
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("CreateLineString: " + rbex.Message, "KmlHelpers");
            }

            return placemark;
        }

        /// <summary>
        /// Draws a line string between the given coordinates
        /// </summary>
        /// <param name="ge">The plug-in instance</param>
        /// <param name="coordinates">List of points</param>
        /// <param name="id">Optional ID of the line string placemark. Default is empty</param>
        /// <param name="tessellate">Optionally sets tessellation for the line string. Default is true</param>
        /// <param name="addFeature">Optionally adds the line string directly to the plug-in. Default is true</param>
        /// <param name="width">Optional line string-width, default is 1</param>
        /// <param name="color">Optional KmlColor, default is white/opaque</param>
        /// <returns>A line string placemark (or null)</returns>
        public static dynamic CreateLineString(
            dynamic ge,
            IList<Coordinate> coordinates,
            string id = "",
            bool tessellate = true,
            bool addFeature = true,
            int width = 1,
            KmlColor color = new KmlColor())
        {
            if (!GEHelpers.IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            dynamic placemark = null;

            try
            {
                placemark = CreatePlacemark(ge, addFeature: addFeature);
                dynamic lineString = ge.createLineString(id);
                lineString.setTessellate(Convert.ToUInt16(tessellate));

                foreach (Coordinate c in coordinates)
                {
                    lineString.getCoordinates().pushLatLngAlt(c.Latitude, c.Longitude, c.Altitude);
                }

                if (placemark.getStyleSelector() == null)
                {
                    placemark.setStyleSelector(ge.createStyle(string.Empty));
                }

                dynamic lineStyle = placemark.getStyleSelector().getLineStyle();
                lineStyle.setWidth(width);
                lineStyle.getColor().set(color.ToString());

                placemark.setGeometry(lineString);
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("CreateLineString: " + rbex.ToString(), "KmlHelpers");
            }

            return placemark;
        }

        /// <summary>
        /// Converts a System.Drawing.Color into a KmlColor
        /// </summary>
        /// <param name="color">A colour to base the KmlColor on</param>
        /// <param name="alpha">Optional alpha value in the range [0-1].
        /// Where 0 is fully transparent and 1 is fully opaque. Default value is 1</param>
        /// <returns>A Kml colour object</returns>
        public static KmlColor ToKmlColor(this Color color, double alpha = 1.0)
        {
            return new KmlColor(color, alpha);
        }

        /// <summary>
        /// Converts a System.Drawing.Color into a KmlColor string
        /// </summary>
        /// <param name="color">A colour to base the KmlColor on</param>
        /// <param name="alpha">Optional alpha value in the range [0-1].
        /// Where 0 is fully transparent and 1 is fully opaque. Default value is 1</param>
        /// <returns>A Kml colour string in the aabbggrr format</returns>
        public static string ToKmlColorString(this Color color, double alpha = 1.0)
        {
            return new KmlColor(color, alpha).ToString();
        }

        /// <summary>
        /// Look at the given coordinates
        /// </summary>
        /// <param name="ge">the plug-in</param>
        /// <param name="latitude">latitude in decimal degrees</param>
        /// <param name="longitude">longitude in decimal degrees</param>
        /// <param name="id">Optional LookAt Id. Default is empty</param>
        /// <param name="altitude">Optional altitude. Default is 0</param>
        /// <param name="altitudeMode">Optional altitudeMode. Default is AltitudeMode.RelativeToGround</param>
        /// <param name="heading">Optional heading in degrees. Default is 0 (north)</param>
        /// <param name="tilt">Optional tilt in degrees. Default is 0</param>
        /// <param name="range">Optional range in metres. Default is 1000</param>
        /// <param name="setView">Optional set the current view to the lookAt</param>
        /// <returns>a look at object (or null)</returns>
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
            if (!GEHelpers.IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            dynamic lookat = null;

            try
            {
                lookat = ge.createLookAt(id);
                lookat.set(latitude, longitude, altitude, altitudeMode, heading, tilt, range);

                if (setView)
                {
                    ge.getView().setAbstractView(lookat);
                }
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("CreateLookAt: " + rbex.Message, "KmlHelpers");
            }

            return lookat;
        }

        /// <summary>
        /// Creates a style that can be referenced by StyleMaps and features.
        /// </summary>
        /// <param name="ge">The plug-in instance</param>
        /// <param name="id">Optional style Id. Default is empty</param>
        /// <returns>A Google API KmlStyle object</returns>
        public static dynamic CreateStyle(dynamic ge, string id = "")
        {
            dynamic style = null;

            try
            {
                style = ge.createStyle(id);
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("CreateStyle: " + rbex.Message, "KmlHelpers");
            }

            return style;
        }

        /// <summary>
        /// Gives access to un-typed data/value pairs using the basic Data element
        /// See: http://code.google.com/apis/kml/documentation/kmlreference.html#extendeddata
        /// </summary>
        /// <param name="kmlFeature">feature to get data from</param>
        /// <returns>A list of key value pairs</returns>
        public static Dictionary<string, string> GetExtendedData(dynamic kmlFeature)
        {
            Dictionary<string, string> keyValues =
                new Dictionary<string, string>();

            XmlNodeList list = GetElementsByTagName(kmlFeature, "Data");
            int c = list.Count;

            for (int i = 0; i < c; i++)
            {
                XmlAttributeCollection xmlAttributeCollection = list[i].Attributes;
                if (xmlAttributeCollection != null)
                {
                    keyValues.Add(
                        xmlAttributeCollection["name"].InnerText,
                        list[i].ChildNodes[0].InnerText);
                }
            }

            return keyValues;
        }

        /// <summary>
        /// Returns an System.Xml.XmlNodeList containing a list of all descendant elements
        /// that match the specified <paramref name="tagName"/>.
        /// </summary>
        /// <param name="kmlFeature">
        /// The Kml feature on which to check for nodes matching the <paramref name="tagName"/>
        /// </param>
        /// <param name="tagName">
        /// The qualified name to match.
        /// It is matched against the Name property of the matching node.
        /// The special value "*" matches all tags
        /// </param>
        /// <returns>
        /// An System.Xml.XmlNodeList containing a list of all matching nodes.
        /// If no nodes match name, the returned collection will be empty.
        /// </returns>
        public static XmlNodeList GetElementsByTagName(dynamic kmlFeature, string tagName)
        {
            XmlDocument doc = new XmlDocument();
            string kml = string.Empty;

            try
            {
                kml = kmlFeature.getKml();
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("GetElementsByTagName: " + rbex.Message, "KmlHelplers");
                return doc.ChildNodes;
            }
            catch (COMException)
            {
                return doc.ChildNodes;
            }

            doc.InnerXml = kml;

            return doc.GetElementsByTagName(tagName);
        }

        /// <summary>
        ///  Gives access to the URL element in pre KML Release 2.1 documents
        ///  This allows the controls to work with legacy Kml formats
        /// </summary>
        /// <param name="kmlFeature">The network link to look for a URL in</param>
        /// <returns>A URL from the feature or null</returns>
        /// <remarks>This method is used by <see cref="KmlTreeView"/> for legacy KML support</remarks>
        /// <example>string URL = KmlHelpers.GetUrl(kmlObject);</example>
        public static Uri GetUrl(dynamic kmlFeature)
        {
            Uri uri = null;
            string link = string.Empty;

            XmlNodeList list = GetElementsByTagName(kmlFeature, "href");

            if (list.Count > 0)
            {
                link = list[0].InnerText;
            }

            if (string.IsNullOrEmpty(link))
            {
                try
                {
                    link = kmlFeature.getUrl();
                }
                catch (COMException)
                {
                }

                if (string.IsNullOrEmpty(link))
                {
                    try
                    {
                        link = kmlFeature.getLink().getHref();
                    }
                    catch (COMException)
                    {
                    }
                }
            }

            Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out uri);

            return uri;
        }

        /// <summary>
        /// Wrapper for getOwnerDocument().getComputedStyle().getListStyle().getListItemType()
        /// See: 
        /// </summary>
        /// <param name="kmlFeature">The feature to find the list item type of</param>
        /// <returns>The corresponding ListItem type <see cref="ListItemStyle"/></returns>
        /// <remarks>This method is used by <see cref="KmlTreeView"/> to build the nodes</remarks>
        /// <example>Example: KmlHelpers.GetListItemType(kmlFeature)</example>
        public static ListItemStyle GetListItemType(dynamic kmlFeature)
        {
            ListItemStyle listItem = ListItemStyle.Check;

            try
            {
                listItem = (ListItemStyle)kmlFeature.getComputedStyle().getListStyle().getListItemType();
            }
            catch (COMException cex)
            {
                Debug.WriteLine("GetListItemType: " + cex.Message, "KmlHelpers");
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("GetListItemType: " + rbex.Message, "KmlHelpers");
            }

            return listItem;
        }

        /// <summary>
        /// Gets the child nodes from a KML feature. 
        /// Basically a wrapper for feature.getFeatures().getChildNodes();
        /// </summary>
        /// <param name="feature">The feature to get the children from</param>
        /// <returns>A KML object containing the child nodes, or null if the feature has no children or on any error.</returns>
        public static dynamic GetChildNodes(dynamic feature)
        {
            try
            {
                if (IsKmlContainer(feature))
                {
                    feature = feature.getFeatures();
                }

                return feature.getChildNodes();
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("GetChildNodes: " + rbex.Message, "KmlHelplers");
            }
            catch (COMException)
            {
            }

            return null;
        }

        /// <summary>
        /// Tests if a given kml feature has child nodes
        /// </summary>
        /// <param name="feature">The feature to check</param>
        /// <returns>True if the feature has children</returns>
        public static bool HasChildNodes(dynamic feature)
        {
            try
            {
                if (IsKmlContainer(feature))
                {
                    feature = feature.getFeatures();
                }

                return Convert.ToBoolean(feature.hasChildNodes());
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("HasChildNodes: " + rbex.Message, "KmlHelplers");
            }
            catch (COMException)
            {
            }

            return false;
        }

        /// <summary>
        /// Tests if a given kml feature is a Kml container
        /// </summary>
        /// <param name="feature">The feature to check</param>
        /// <returns>True if the feature is a Kml container</returns>
        public static bool IsKmlContainer(dynamic feature)
        {
            ApiType type = GEHelpers.GetApiType(feature);

            switch (type)
            {
                case ApiType.KmlDocument:
                case ApiType.KmlFolder:
                case ApiType.KmlLayer:
                case ApiType.KmlLayerRoot:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Sets a Bounds object to the current plugin view
        /// </summary>
        /// <param name="ge">the plugin</param>
        /// <param name="bounds">the bounds object to create the view of</param>
        /// <param name="aspectRatio">Optional aspect ratio</param>
        /// <param name="defaultRange">Optional default range</param>
        /// <param name="scaleRange">Optional scale range</param>
        public static void SetBoundsView(
            dynamic ge,
            Bounds bounds,
            double aspectRatio = 1.0,
            double defaultRange = 1000,
            double scaleRange = 1.5)
        {
            try
            {
                ge.getView().setAbstractView(
                    KmlHelpers.CreateBoundsView(
                    ge,
                    bounds,
                    aspectRatio,
                    defaultRange,
                    scaleRange));
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("SetBoundsView: " + rbex.Message, "KmlHelpers");
            }
        }

        /// <summary>
        /// Based on kmldomwalk.js 
        /// see: http://code.google.com/p/earth-api-samples/source/browse/trunk/lib/kmldomwalk.js
        /// </summary>
        /// <param name="feature">The KML object to parse</param>
        /// <param name="callback">A delegate action, each node visited will be passed to this as the single parameter</param>
        /// <param name="walkFeatures">Optionally walk features, default is true</param>
        /// <param name="walkGeometries">Optionally walk geometries, default is false</param>
        /// <remarks>This method is used by <see cref="KmlTreeView"/> to build the nodes</remarks>
        public static void WalkKmlDom(
            dynamic feature,
            Action<dynamic> callback,
            bool walkFeatures = true,
            bool walkGeometries = false)
        {
            if (feature == null)
            {
                return;
            }

            dynamic objectContainer = null;
            ApiType type = GEHelpers.GetApiType(feature);

            switch (type)
            {
                // objects that support getFeatures
                case ApiType.KmlDocument:
                case ApiType.KmlFolder:
                case ApiType.KmlLayer:
                case ApiType.KmlLayerRoot:
                    {
                        if (walkFeatures)
                        {
                            objectContainer = feature.getFeatures();  // GESchemaObjectContainer
                        }
                    }

                    break;

                // objects that support getGeometry
                case ApiType.KmlPlacemark:
                    {
                        if (walkGeometries)
                        {
                            WalkKmlDom(feature.getGeometry(), callback, walkFeatures, true);
                        }
                    }

                    break;

                // object that support getInnerBoundaries
                case ApiType.KmlPolygon:
                    {
                        if (walkGeometries)
                        {
                            WalkKmlDom(feature.getOuterBoundary(), callback, walkFeatures, true);
                        }
                    }

                    break;

                // objects that support getGeometries
                case ApiType.KmlMultiGeometry:
                    {
                        if (walkGeometries)
                        {
                            objectContainer = feature.getGeometries();  // GESchemaObjectContainer
                            ////WalkKmlDom(feature.getOuterBoundary(), callback, walkFeatures, walkGeometries);
                        }
                    }

                    break;
            }

            callback(feature);

            if (objectContainer != null && HasChildNodes(objectContainer))
            {
                // 'GetChildNodes' returns null in some circumstances.
                // see: Issue 96 
                dynamic childNodes = KmlHelpers.GetChildNodes(objectContainer);
                int count = childNodes == null ? 0 : childNodes.getLength();
                for (int i = 0; i < count; i++)
                {
                    dynamic node = childNodes.item(i);
                    WalkKmlDom(node, callback, walkFeatures, walkGeometries);
                    callback(node);
                }
            }
        }
    }
}