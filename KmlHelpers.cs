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
    using System.Runtime.InteropServices;
    using System.IO;
    using System.IO.Compression;
    using GEPlugin;

    /// <summary>
    /// This class provides basic Kml helper methods
    /// </summary>
    public class KmlHelpers
    {
        /// <summary>
        /// A callback delegate for all items iterated over in WalkKmlDom
        /// </summary>
        /// <param name="kmlObject">The current KmlObject</param>
        public delegate void CallBack(IKmlObject kmlObject);

        /// <summary>
        /// Based on kmldomwalk.js
        /// see: http://code.google.com/p/earth-api-samples/source/browse/trunk/lib/kmldomwalk.js
        /// </summary>
        /// <param name="kmlObject">The kml object to parse</param>
        /// <param name="callBack">The funciton to call on each node</param>
        public static void WalkKmlDom(IKmlObject kmlObject, CallBack callBack)
        {
            string type = kmlObject.getType();

            switch (type)
            {
                case "KmlDocument":
                case "KmlFolder":
                    IKmlContainer container = kmlObject as IKmlContainer;
                    if (Convert.ToBoolean(container.getFeatures().hasChildNodes()))
                    {
                        IKmlObjectList subNodes = container.getFeatures().getChildNodes();

                        for (int i = 0; i < subNodes.getLength(); i++)
                        {
                            IKmlObject subNode = subNodes.item(i);
                            WalkKmlDom(subNode, callBack);
                            callBack(subNode);
                        }
                    }

                    break;
                default:
                    callBack(kmlObject);
                    break;
            }
        }
    }
}
