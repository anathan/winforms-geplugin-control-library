// <copyright file="ApiType.cs" company="FC">
// Copyright (c) 2011 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2011-03-06</date>
// <summary>This file is part of FC.GEPluginCtrls
// FC.GEPluginCtrls is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.
// </summary>
namespace FC.GEPluginCtrls
{
    /// <summary>
    /// Enumeration of the available types in the Google Earth Api.
    /// </summary>
    /// <remarks>This is a compleate list as of v1.007 of the API</remarks>
    public enum ApiType
    {
        /// <summary>
        /// No type
        /// </summary>
        None = 0,

        /// <summary>
        /// The GEAbstractBalloon type
        /// </summary>
        GEAbstractBalloon = 1,

        /// <summary>
        /// The GEControl type
        /// </summary>
        GEControl = 2,

        /// <summary>
        /// The GEEventEmitter type
        /// </summary>
        GEEventEmitter = 3,

        /// <summary>
        /// The GEFeatureBalloon type
        /// </summary>
        GEFeatureBalloon = 4,

        /// <summary>
        /// The GEFeatureContainer type
        /// </summary>
        GEFeatureContainer = 5,

        /// <summary>
        /// The GEGeometryContainer type
        /// </summary>
        GEGeometryContainer = 6,

        /// <summary>
        /// The GEGlobe type
        /// </summary>
        GEGlobe = 7,

        /// <summary>
        /// The GEHitTestResult type
        /// </summary>
        GEHitTestResult = 8,

        /// <summary>
        /// The GEHtmlBalloon type
        /// </summary>
        GEHtmlBalloon = 9,

        /// <summary>
        /// The GEHtmlDivBalloon type
        /// </summary>
        GEHtmlDivBalloon = 10,

        /// <summary>
        /// The GEHtmlStringBalloon type
        /// </summary>
        GEHtmlStringBalloon = 11,

        /// <summary>
        /// The GELinearRingContainer type
        /// </summary>
        GELinearRingContainer = 12,

        /// <summary>
        /// The GENavigationControl type
        /// </summary>
        GENavigationControl = 13,

        /// <summary>
        /// The GEOptions type
        /// </summary>
        GEOptions = 14,

        /// <summary>
        /// The GEPhotoOverlayViewer type
        /// </summary>
        GEPhotoOverlayViewer = 15,

        /// <summary>
        /// The GEPlugin type
        /// </summary>
        GEPlugin = 16,

        /// <summary>
        /// The GESchemaObjectContainer type
        /// </summary>
        GESchemaObjectContainer = 17,

        /// <summary>
        /// The GEStyleSelectorContainer type
        /// </summary>
        GEStyleSelectorContainer = 18,

        /// <summary>
        /// The GESun type
        /// </summary>
        GESun = 19,

        /// <summary>
        /// The GETime type
        /// </summary>
        GETime = 20,

        /// <summary>
        /// The GETimeControl type
        /// </summary>
        GETimeControl = 21,

        /// <summary>
        /// The GETourPlayer type
        /// </summary>
        GETourPlayer = 22,

        /// <summary>
        /// The GEView type
        /// </summary>
        GEView = 23,

        /// <summary>
        /// The GEWindow type
        /// </summary>
        GEWindow = 24,

        /// <summary>
        /// The KmlAbstractView type
        /// </summary>
        KmlAbstractView = 100,

        /// <summary>
        /// The KmlAltitudeGeometry type
        /// </summary>
        KmlAltitudeGeometry = 101,

        /// <summary>
        /// The KmlBalloonOpeningEvent type
        /// </summary>
        KmlBalloonOpeningEvent = 102,

        /// <summary>
        /// The KmlBalloonStyle type
        /// </summary>
        KmlBalloonStyle = 103,

        /// <summary>
        /// The KmlCamera type
        /// </summary>
        KmlCamera = 104,

        /// <summary>
        /// The KmlColor type
        /// </summary>
        KmlColor = 105,

        /// <summary>
        /// The KmlColorStyle type
        /// </summary>
        KmlColorStyle = 106,

        /// <summary>
        /// The KmlContainer type
        /// </summary>
        KmlContainer = 107,

        /// <summary>
        /// The KmlCoord type
        /// </summary>
        KmlCoord = 108,

        /// <summary>
        /// The KmlCoordArray type
        /// </summary>
        KmlCoordArray = 109,

        /// <summary>
        /// The KmlDateTime type
        /// </summary>
        KmlDateTime = 110,

        /// <summary>
        /// A KmlDocument has containers that holds features and styles.
        /// </summary>
        KmlDocument = 111,

        /// <summary>
        /// The KmlEvent type
        /// </summary>
        KmlEvent = 112,

        /// <summary>
        /// The KmlExtrudableGeometry type
        /// </summary>
        KmlExtrudableGeometry = 113,

        /// <summary>
        /// The KmlFeature type
        /// </summary>
        KmlFeature = 114,

        /// <summary>
        /// A Folder is used to arrange other features hierarchically (Folders, Placemarks, NetworkLinks, or Overlays).
        /// </summary>
        KmlFolder = 115,

        /// <summary>
        /// The KmlGeometry type
        /// </summary>
        KmlGeometry = 116,

        /// <summary>
        /// The KmlGroundOverlay type
        /// </summary>
        KmlGroundOverlay = 117,

        /// <summary>
        /// The KmlIcon type
        /// </summary>
        KmlIcon = 118,

        /// <summary>
        /// The KmlIconStyle type
        /// </summary>
        KmlIconStyle = 119,

        /// <summary>
        /// The KmlLabelStyle type
        /// </summary>
        KmlLabelStyle = 120,

        /// <summary>
        /// The KmlLatLonAltBox type
        /// </summary>
        KmlLatLonAltBox = 121,

        /// <summary>
        /// The KmlLatLonBox type
        /// </summary>
        KmlLatLonBox = 122,

        /// <summary>
        /// The KmlLayer type
        /// </summary>
        KmlLayer = 123,

        /// <summary>
        /// The KmlLayerRoot type
        /// </summary>
        KmlLayerRoot = 124,

        /// <summary>
        /// The KmlLinearRing type
        /// </summary>
        KmlLinearRing = 125,

        /// <summary>
        /// The KmlLineString type
        /// </summary>
        KmlLineString = 126,

        /// <summary>
        /// The KmlLineStyle type
        /// </summary>
        KmlLineStyle = 127,

        /// <summary>
        /// The KmlLink type
        /// </summary>
        KmlLink = 128,

        /// <summary>
        /// The KmlListStyle type
        /// </summary>
        KmlListStyle = 129,

        /// <summary>
        /// The KmlLocation type
        /// </summary>
        KmlLocation = 130,

        /// <summary>
        /// The KmlLod type
        /// </summary>
        KmlLod = 131,

        /// <summary>
        /// The KmlLookAt type
        /// </summary>
        KmlLookAt = 132,

        /// <summary>
        /// The KmlModel type
        /// </summary>
        KmlModel = 133,

        /// <summary>
        /// The KmlMouseEvent type
        /// </summary>
        KmlMouseEvent = 134,

        /// <summary>
        /// The KmlMultiGeometry type
        /// </summary>
        KmlMultiGeometry = 135,

        /// <summary>
        /// The KmlNetworkLink type
        /// </summary>
        KmlNetworkLink = 136,

        /// <summary>
        /// The KmlObject type
        /// </summary>
        KmlObject = 137,

        /// <summary>
        /// The KmlObjectList type
        /// </summary>
        KmlObjectList = 138,

        /// <summary>
        /// The KmlOrientation type
        /// </summary>
        KmlOrientation = 139,

        /// <summary>
        /// The KmlOverlay type
        /// </summary>
        KmlOverlay = 140,

        /// <summary>
        /// The KmlPhotoOverlay type
        /// </summary>
        KmlPhotoOverlay = 141,

        /// <summary>
        /// The KmlPlacemark is a feature with associated Geometry.
        /// </summary>
        KmlPlacemark = 142,

        /// <summary>
        /// A geographic location defined by longitude, latitude, and (optional) altitude.
        /// </summary>
        KmlPoint = 143,

        /// <summary>
        /// The KmlPolygon type
        /// </summary>
        KmlPolygon = 144,

        /// <summary>
        /// The KmlPolyStyle type
        /// </summary>
        KmlPolyStyle = 145,

        /// <summary>
        /// The KmlRegion type
        /// </summary>
        KmlRegion = 146,

        /// <summary>
        /// The KmlScale type
        /// </summary>
        KmlScale = 147,

        /// <summary>
        /// The KmlScreenOverlay type
        /// </summary>
        KmlScreenOverlay = 148,

        /// <summary>
        /// The KmlStyle type
        /// </summary>
        KmlStyle = 149,

        /// <summary>
        /// The KmlStyleMap type
        /// </summary>
        KmlStyleMap = 150,

        /// <summary>
        /// The KmlStyleSelector type
        /// </summary>
        KmlStyleSelector = 151,

        /// <summary>
        /// The KmlTimePrimitive type
        /// </summary>
        KmlTimePrimitive = 152,

        /// <summary>
        /// The KmlTimeSpan type
        /// </summary>
        KmlTimeSpan = 153,

        /// <summary>
        /// The KmlTimeStamp type
        /// </summary>
        KmlTimeStamp = 154,

        /// <summary>
        /// The KmlTour type
        /// </summary>
        KmlTour = 155,

        /// <summary>
        /// The KmlVec2 type
        /// </summary>
        KmlVec2 = 156,

        /// <summary>
        /// The KmlViewerOptions type
        /// </summary>
        KmlViewerOptions = 157
    }
}