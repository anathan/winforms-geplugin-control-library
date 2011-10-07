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
    using System.Xml;
    using FC.GEPluginCtrls.Geo;
    using Microsoft.CSharp.RuntimeBinder;

    /// <summary>
    /// This class provides basic Kml helper methods
    /// </summary>
    public static class KmlHelpers
    {
        /// <summary>
        /// Based on kmldomwalk.js 
        /// see: http://code.google.com/p/earth-api-samples/source/browse/trunk/lib/kmldomwalk.js
        /// </summary>
        /// <param name="feature">The kml object to parse</param>
        /// <param name="callback">A delegate action, each node visited will be passed to this as the single parameter</param>
        /// <param name="walkFeatures">Optionally walk features, defualt is true</param>
        /// <param name="walkGeometries">Optionally walk geometries, default is false</param>
        /// <remarks>This method is used by <see cref="KmlTreeView"/> to build the nodes</remarks>
        /// <example>KmlHelpers.WalkKmlDom(kml, (Action dynamic)(x => { /* each x in the dom */}));</example>
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

            string type = feature.getType();
            dynamic objectContainer = null; // GESchemaObjectContainer

            switch (type)
            {
                // objects that support getFeatures (GEFeatureContainer)
                case ApiType.KmlDocument:
                case ApiType.KmlFolder:
                case ApiType.KmlLayer:
                case ApiType.KmlLayerRoot:
                    {
                        if (walkFeatures)
                        {
                            objectContainer = feature.getFeatures();
                        }
                    }

                    break;

                // objects that support getGeometry
                case ApiType.KmlAltitudeGeometry:
                case ApiType.KmlExtrudableGeometry:
                case ApiType.KmlModel:
                case ApiType.KmlPlacemark:
                    {
                        if (walkGeometries)
                        {
                            WalkKmlDom(feature.getGeometry(), callback, walkFeatures, walkGeometries);
                        }
                    }

                    break;

                // KmlPolygon (object supports getOuterBoundary)
                case ApiType.KmlPolygon:
                    {
                        if (walkGeometries)
                        {
                            WalkKmlDom(feature.getOuterBoundary(), callback, walkFeatures, walkGeometries);
                            objectContainer = feature.getInnerBoundaries(); // GELinearRingContainer
                        }
                    }

                    break;

                // KmlMultiGeometry (object supports getGeometries)
                case ApiType.KmlMultiGeometry:
                    {
                        if (walkGeometries)
                        {
                            objectContainer = feature.getGeometries();
                        }
                    }
                    
                    break;

                ////case ApiType.KmlLineString:
                ////case ApiType.KmlLinearRing:

                default:
                    break;
            }

            callback(feature);

            if (objectContainer != null && Convert.ToBoolean(objectContainer.hasChildNodes()))
            {
                dynamic childNodes = objectContainer.getChildNodes();
                int count = childNodes.getLength();
                for (int i = 0; i < count; i++)
                {
                    dynamic node = childNodes.item(i);
                    WalkKmlDom(node, callback);
                    callback(node);
                }
                ////Parallel.For(0, count, i => { });
            }
        }

        /// <summary>
        ///  Gives access to Url element in pre KML Release 2.1 documents
        ///  This allows the controls to work with legacy Kml formats
        /// </summary>
        /// <param name="kmlFeature">The network link to look for a url in</param>
        /// <returns>The url value or an empty string</returns>
        /// <remarks>This method is used by <see cref="KmlTreeView"/> for legacy kml support</remarks>
        /// <example>string url = KmlHelpers.GetUrl(kmlObject);</example>
        public static string GetUrl(dynamic kmlFeature)
        {
            string kml = string.Empty;
            string url = string.Empty;

            try
            {
                kml = kmlFeature.getKml();
            }
            catch (RuntimeBinderException)
            {
                return string.Empty;
            }

            XmlDocument doc = new System.Xml.XmlDocument();
            doc.InnerXml = kml;

            XmlNodeList list = doc.GetElementsByTagName("href");

            if (list.Count > 0)
            {
                url = list[0].InnerText;
            }

            if (string.IsNullOrEmpty(url))
            {
                try
                {
                    url = kmlFeature.getUrl();
                }
                catch (RuntimeBinderException)
                {
                }

                if (string.IsNullOrEmpty(url))
                {
                    try
                    {
                        url = kmlFeature.getLink().getHref();
                    }
                    catch (RuntimeBinderException)
                    {
                    }
                }
            }

            return url;
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
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("GetListItemType: " + rbex.ToString(), "KmlHelpers");
            }

            return listItem;
        }

        /// <summary>
        /// Gives access to untyped data/value pairs using the basic Data element
        /// See: http://code.google.com/apis/kml/documentation/kmlreference.html#extendeddata
        /// </summary>
        /// <param name="kmlFeature">feature to get data from</param>
        /// <returns>A list of key value pairs</returns>
        public static Dictionary<string, string> GetExtendedData(dynamic kmlFeature)
        {
            Dictionary<string, string> keyValues =
                new Dictionary<string, string>();

            XmlDocument doc = new XmlDocument();
            doc.InnerXml = kmlFeature.getKml();

            XmlNodeList list = doc.GetElementsByTagName("Data");
            int c = list.Count;

            for (int i = 0; i < c; i++)
            {
                keyValues.Add(
                    list[i].Attributes["name"].InnerText,
                    list[i].ChildNodes[0].InnerText);
            }

            return keyValues;
        }

        /// <summary>
        /// Computes the bounding box for the given object.
        /// Note that this method walks the object's DOM, so may have poor performance for large objects.
        /// In that case the use parallel option can speed the operation on some machines. 
        /// </summary>
        /// <param name="kmlFeature">{KmlFeature|KmlGeometry} object The feature or geometry whose bounds should be computed</param>
        /// <returns>The bounds object for the feature</returns>
        /// <remarks>
        /// Based on the methods at:
        /// http://code.google.com/p/earth-api-utility-library/source/browse/trunk/extensions/src/dom/utils.js
        /// </remarks>
        public static Bounds ComputeBounds(dynamic kmlFeature)
        {
            Bounds bounds = new Bounds();

            KmlHelpers.WalkKmlDom(
                kmlFeature,
                (Action<dynamic>)(feature =>
            {
                string type = feature.getType();

                switch (type)
                {
                    case ApiType.KmlGroundOverlay:
                        {
                            dynamic llb = feature.getLatLonBox();

                            if (llb != null)
                            {
                                double alt = feature.getAltitude();
                                bounds.Extend(new Coordinate(llb.getNorth(), llb.getEast(), alt));
                                bounds.Extend(new Coordinate(llb.getNorth(), llb.getWest(), alt));
                                bounds.Extend(new Coordinate(llb.getSouth(), llb.getEast(), alt));
                                bounds.Extend(new Coordinate(llb.getSouth(), llb.getWest(), alt));
                            }
                        }

                        break;

                    case ApiType.KmlModel:
                        bounds.Extend(new Coordinate(feature.getLocation()));
                        break;

                    case ApiType.KmlLinearRing:
                    case ApiType.KmlLineString:
                        {
                            dynamic coords = feature.getCoordinates();

                            if (coords != null)
                            {
                                int count = coords.getLength();

                                ////Parallel.For(0, count, i => { });
                                for (int i = 0; i < count; i++)
                                {
                                    bounds.Extend(new Coordinate(coords.get(i)));
                                }
                            }
                        }

                        break;

                    case ApiType.KmlCoord: // coordinates
                    case ApiType.KmlLocation: // models
                    case ApiType.KmlPoint: // points
                        bounds.Extend(new Coordinate(feature));
                        break;
                }
            }),
            walkFeatures: true,
            walkGeometries: true);

            return bounds;
        }

        /// <summary>
        /// Creates a kmlLookAt from a Bounds object.
        /// </summary>
        /// <param name="ge">the plugin</param>
        /// <param name="bounds">the bounds object to create the view of</param>
        /// <param name="aspectRatio">Optional aspect ratio</param>
        /// <param name="defaultRange">Optional default range</param>
        /// <param name="scaleRange">Optional scale range</param>
        /// <returns>The KmlLookAt for the bounds (or false)</returns>
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

            if (Convert.ToBoolean(boundsSpan.Latitude) || Convert.ToBoolean(boundsSpan.Longitude))
            {
                // Distance - using law of cosines for speed...
                double distEW = new Geo.Coordinate(center.Latitude, bounds.East)
                   .Distance(new Geo.Coordinate(center.Latitude, bounds.West));

                double distNS = new Geo.Coordinate(bounds.North, center.Longitude)
                   .Distance(new Geo.Coordinate(bounds.South, center.Longitude));

                aspectRatio = Math.Min(Math.Max(aspectRatio, distEW / distNS), 1.0);

                // Create a LookAt using the experimentally derived distance formula.
                double alpha = Maths.ConvertDegreesToRadians((45.0 / (aspectRatio + 0.4)) - 2.0);
                double expandToDistance = Math.Max(distNS, distEW);
                double beta = Math.Min(
                    Maths.ConvertDegreesToRadians(90),
                    (alpha + expandToDistance) / (2 * Maths.EarthMeanRadiusKilometres));

                lookAtRange = scaleRange * Maths.EarthMeanRadiusKilometres * ((Math.Sin(beta) * Math.Sqrt(1 + (1 / Math.Pow(Math.Tan(alpha), 2)))) - 1);
            }

            try
            {
                dynamic lookat = ge.createLookAt(string.Empty);
                lookat.set(center.Latitude, center.Longitude, bounds.Top, bounds.Northeast.AltitudeMode, 0, 0, lookAtRange);
                return lookat;
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("CreateBoundsView: " + rbex.ToString(), "KmlHelpers");
            }

            return false;
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
                Debug.WriteLine("SetBoundsView: " + rbex.ToString(), "KmlHelpers");
            }
        }
    }
}