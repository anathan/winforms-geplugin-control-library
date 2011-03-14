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
    using System.IO;
    using System.IO.Compression;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.CSharp.RuntimeBinder;

    /// <summary>
    /// This class provides basic Kml helper methods
    /// </summary>
    public static class KmlHelpers
    {
        /// <summary>
        /// Based on kmldomwalk.js Parallel
        /// see: http://code.google.com/p/earth-api-samples/source/browse/trunk/lib/kmldomwalk.js
        /// </summary>
        /// <param name="kmlObject">The kml object to parse</param>
        /// <param name="callBack">A delegate action, each node visited will be passed to this as the single parameter</param>
        public static void WalkKmlDom(dynamic kmlObject, Action<dynamic> callBack)
        {
            string type = kmlObject.getType();

            switch (type)
            {
                case ApiType.KmlDocument:
                case ApiType.KmlFolder:

                    if (Convert.ToBoolean(kmlObject.getFeatures().hasChildNodes()))
                    {
                        dynamic childNodes = kmlObject.getFeatures().getChildNodes();
                        int count = childNodes.getLength();

                        Parallel.For(
                            0,
                            count,
                            i =>
                        {
                            dynamic node = childNodes.item(i);
                            WalkKmlDom(node, callBack);
                            callBack(node);
                        });
                    }

                    break;

                default:
                    callBack(kmlObject);
                    break;
            }
        }

        /// <summary>
        ///  Gives access to Url element in pre KML Release 2.1 documents
        ///  This allows the controls to work with legacy Kml formats
        /// </summary>
        /// <param name="networklink">The network link to look for a url in</param>
        /// <returns>The url value or an empty string</returns>
        public static string GetUrl(dynamic networklink)
        {
            string kml = string.Empty;
            string url = string.Empty;

            try
            {
                kml = networklink.getKml();
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

            if (url == string.Empty)
            {
                try
                {
                    url = networklink.getUrl();
                }
                catch (RuntimeBinderException)
                {
                }

                if (url == string.Empty)
                {
                    try
                    {
                        url = networklink.getLink().getHref();
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
        /// <param name="feature">The feature to find the list item type of</param>
        /// <returns>The corresponding ListItem type <see cref="ListItemStyle"/></returns>
        public static ListItemStyle GetListItemType(dynamic feature)
        {
            ListItemStyle listItem = ListItemStyle.Check;

            try
            {
                listItem = (ListItemStyle)feature.getComputedStyle().getListStyle().getListItemType();
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
        /// <param name="feature">feature to get data from</param>
        /// <returns>A list of key value pairs</returns>
        public static List<KeyValuePair<string, string>> GetExtendedData(dynamic feature)
        {
            List<KeyValuePair<string, string>> keyValues = 
                new List<KeyValuePair<string, string>>();

            XmlDocument doc = new XmlDocument();
            doc.InnerXml = feature.getKml();
            XmlNodeList list = doc.GetElementsByTagName("Data");
            int c = list.Count;

            for (int i = 0; i < c; i++)
            {
                keyValues.Add(new KeyValuePair<string, string>(
                    list[i].Attributes["name"].InnerText,
                    list[i].ChildNodes[0].InnerText));
            }

            return keyValues;
        }
    }
}
