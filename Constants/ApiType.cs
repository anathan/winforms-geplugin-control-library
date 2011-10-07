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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.
// </summary>
namespace FC.GEPluginCtrls
{
    /// <summary>
    /// A collection of the names of all the objects in the Google Earth API.
    /// The constants here are usefull when checking the type of any object 
    /// that comes from outside managed code
    /// </summary>
    /// <remarks>This is a compleate list as of v1.007 of the API</remarks>
    public static class ApiType
    {
        #region GE

        /// <summary>
        /// GEAbstractBalloon Api name
        /// </summary>
        public const string GEAbstractBalloon = "GEAbstractBalloon";

        /// <summary>
        /// GEControl Api name
        /// </summary>
        public const string GEControl = "GEControl";

        /// <summary>
        /// GEEventEmitter Api name
        /// </summary>
        public const string GEEventEmitter = "GEEventEmitter";

        /// <summary>
        /// GEFeatureBalloon Api name
        /// </summary>
        public const string GEFeatureBalloon = "GEFeatureBalloon";

        /// <summary>
        /// GEFeatureContainer Api name
        /// </summary>
        public const string GEFeatureContainer = "GEFeatureContainer";

        /// <summary>
        /// GEometryContainer Api name
        /// </summary>
        public const string GEGeometryContainer = "GEGeometryContainer";

        /// <summary>
        /// GEGlobe Api name
        /// </summary>
        public const string GEGlobe = "GEGlobe";

        /// <summary>
        /// GEHitTestResult Api name
        /// </summary>
        public const string GEHitTestResult = "GEHitTestResult";

        /// <summary>
        /// GEHtmlBalloon Api name
        /// </summary>
        public const string GEHtmlBalloon = "GEHtmlBalloon";

        /// <summary>
        /// GEHtmlDivBalloon Api name
        /// </summary>
        public const string GEHtmlDivBalloon = "GEHtmlDivBalloon";

        /// <summary>
        /// GEHtmlStringBalloon Api name
        /// </summary>
        public const string GEHtmlStringBalloon = "GEHtmlStringBalloon";

        /// <summary>
        /// GELinearRingContainer Api name
        /// </summary>
        public const string GELinearRingContainer = "GELinearRingContainer";

        /// <summary>
        /// GENavigationControl Api name
        /// </summary>
        public const string GENavigationControl = "GENavigationControl";

        /// <summary>
        /// GEOptions Api name
        /// </summary>
        public const string GEOptions = "GEOptions";

        /// <summary>
        /// GEPhotoOverlayViewer Api name
        /// </summary>
        public const string GEPhotoOverlayViewer = "GEPhotoOverlayViewer";

        /// <summary>
        /// GEPlugin Api name
        /// </summary>
        public const string GEPlugin = "GEPlugin";

        /// <summary>
        /// GESchemaObjectContainer Api name
        /// </summary>
        public const string GESchemaObjectContainer = "GESchemaObjectContainer";

        /// <summary>
        /// GEStyleSelectorContainer Api name
        /// </summary>
        public const string GEStyleSelectorContainer = "GEStyleSelectorContainer";

        /// <summary>
        /// GESun Api name
        /// </summary>
        public const string GESun = "GESun";

        /// <summary>
        /// GETime Api name
        /// </summary>
        public const string GETime = "GETime";

        /// <summary>
        /// GETimeControl Api name
        /// </summary>
        public const string GETimeControl = "GETimeControl";

        /// <summary>
        /// GETourPlayer Api name
        /// </summary>
        public const string GETourPlayer = "GETourPlayer";

        /// <summary>
        /// GEView Api name
        /// </summary>
        public const string GEView = "GEView";

        /// <summary>
        /// GEWindow Api name
        /// </summary>
        public const string GEWindow = "GEWindow";

        #endregion

        #region Kml

        /// <summary>
        /// KmlAbstractView Api name
        /// </summary>
        public const string KmlAbstractView = "KmlAbstractView";

        /// <summary>
        /// KmlAltitudeGeometry Api name
        /// </summary>
        public const string KmlAltitudeGeometry = "KmlAltitudeGeometry";

        /// <summary>
        /// KmlBalloonOpeningEvent Api name
        /// </summary>
        public const string KmlBalloonOpeningEvent = "KmlBalloonOpeningEvent";

        /// <summary>
        /// KmlBalloonStyle Api name
        /// </summary>
        public const string KmlBalloonStyle = "KmlBalloonStyle";

        /// <summary>
        /// KmlCamera Api name
        /// </summary>
        public const string KmlCamera = "KmlCamera";

        /// <summary>
        /// KmlColor Api name
        /// </summary>
        public const string KmlColor = "KmlColor";

        /// <summary>
        /// KmlColorStyle Api name
        /// </summary>
        public const string KmlColorStyle = "KmlColorStyle";

        /// <summary>
        /// KmlContainer Api name
        /// </summary>
        public const string KmlContainer = "KmlContainer";

        /// <summary>
        /// KmlCoord Api name
        /// </summary>
        public const string KmlCoord = "KmlCoord";

        /// <summary>
        /// KmlCoordArray Api name
        /// </summary>
        public const string KmlCoordArray = "KmlCoordArray";

        /// <summary>
        /// KmlDateTime Api name
        /// </summary>
        public const string KmlDateTime = "KmlDateTime";

        /// <summary>
        /// KmlDocument Api name
        /// </summary>
        public const string KmlDocument = "KmlDocument";

        /// <summary>
        /// KmlEvent Api name
        /// </summary>
        public const string KmlEvent = "KmlEvent";

        /// <summary>
        /// KmlExtrudableGeometry Api name
        /// </summary>
        public const string KmlExtrudableGeometry = "KmlExtrudableGeometry";

        /// <summary>
        /// KmlFeature Api name
        /// </summary>
        public const string KmlFeature = "KmlFeature";

        /// <summary>
        /// KmlFolder Api name
        /// </summary>
        public const string KmlFolder = "KmlFolder";

        /// <summary>
        /// KmlGeometry Api name
        /// </summary>
        public const string KmlGeometry = "KmlGeometry";

        /// <summary>
        /// KmlGroundOverlay Api name
        /// </summary>
        public const string KmlGroundOverlay = "KmlGroundOverlay";

        /// <summary>
        /// KmlIcon Api name
        /// </summary>
        public const string KmlIcon = "KmlIcon";

        /// <summary>
        /// KmlIconStyle Api name
        /// </summary>
        public const string KmlIconStyle = "KmlIconStyle";

        /// <summary>
        /// KmlLabelStyle Api name
        /// </summary>
        public const string KmlLabelStyle = "KmlLabelStyle";

        /// <summary>
        /// KmlLatLonAltBox Api name
        /// </summary>
        public const string KmlLatLonAltBox = "KmlLatLonAltBox";

        /// <summary>
        /// KmlLatLonBox Api name
        /// </summary>
        public const string KmlLatLonBox = "KmlLatLonBox";

        /// <summary>
        /// KmlLayer Api name
        /// </summary>
        public const string KmlLayer = "KmlLayer";

        /// <summary>
        /// KmlLayerRoot Api name
        /// </summary>
        public const string KmlLayerRoot = "KmlLayerRoot";

        /// <summary>
        /// KmlLinearRing Api name
        /// </summary>
        public const string KmlLinearRing = "KmlLinearRing";

        /// <summary>
        /// KmlLineString Api name
        /// </summary>
        public const string KmlLineString = "KmlLineString";

        /// <summary>
        /// KmlLineStyle Api name
        /// </summary>
        public const string KmlLineStyle = "KmlLineStyle";

        /// <summary>
        /// KmlLink Api name
        /// </summary>
        public const string KmlLink = "KmlLink";

        /// <summary>
        /// KmlListStyle Api name
        /// </summary>
        public const string KmlListStyle = "KmlListStyle";

        /// <summary>
        /// KmlLocation Api name
        /// </summary>
        public const string KmlLocation = "KmlLocation";

        /// <summary>
        /// KmlLod Api name
        /// </summary>
        public const string KmlLod = "KmlLod";

        /// <summary>
        /// KmlLookA Api name
        /// </summary>
        public const string KmlLookAt = "KmlLookAt";

        /// <summary>
        /// KmlModel  Api name
        /// </summary>
        public const string KmlModel = "KmlModel";

        /// <summary>
        /// KmlMouseEvent Api name
        /// </summary>
        public const string KmlMouseEvent = "KmlMouseEvent";

        /// <summary>
        /// KmlMultiGeometry Api name
        /// </summary>
        public const string KmlMultiGeometry = "KmlMultiGeometry";

        /// <summary>
        /// KmlNetworkLink Api name
        /// </summary>
        public const string KmlNetworkLink = "KmlNetworkLink";

        /// <summary>
        /// KmlObject Api name
        /// </summary>
        public const string KmlObject = "KmlObject";

        /// <summary>
        /// KmlObjectList Api name
        /// </summary>
        public const string KmlObjectList = "KmlObjectList";

        /// <summary>
        /// KmlOrientation Api name
        /// </summary>
        public const string KmlOrientation = "KmlOrientation";

        /// <summary>
        /// KmlOverlay Api name
        /// </summary>
        public const string KmlOverlay = "KmlOverlay";

        /// <summary>
        /// KmlPhotoOverlay Api name
        /// </summary>
        public const string KmlPhotoOverlay = "KmlPhotoOverlay";

        /// <summary>
        /// KmlPlacemark Api name
        /// </summary>
        public const string KmlPlacemark = "KmlPlacemark";

        /// <summary>
        /// KmlPoint Api name
        /// </summary>
        public const string KmlPoint = "KmlPoint";

        /// <summary>
        /// KmlPolygon Api name
        /// </summary>
        public const string KmlPolygon = "KmlPolygon";

        /// <summary>
        /// KmlPolyStyle Api name
        /// </summary>
        public const string KmlPolyStyle = "KmlPolyStyle";

        /// <summary>
        /// KmlRegion Api name
        /// </summary>
        public const string KmlRegion = "KmlRegion";

        /// <summary>
        /// KmlScale Api name
        /// </summary>
        public const string KmlScale = "KmlScale";

        /// <summary>
        /// KmlScreenOverlay Api name
        /// </summary>
        public const string KmlScreenOverlay = "KmlScreenOverlay";

        /// <summary>
        /// KmlStyle Api name
        /// </summary>
        public const string KmlStyle = "KmlStyle";

        /// <summary>
        /// KmlStyleMap Api name
        /// </summary>
        public const string KmlStyleMap = "KmlStyleMap";

        /// <summary>
        /// KmlStyleSelector Api name
        /// </summary>
        public const string KmlStyleSelector = "KmlStyleSelector";

        /// <summary>
        /// KmlTimePrimitive Api name
        /// </summary>
        public const string KmlTimePrimitive = "KmlTimePrimitive";

        /// <summary>
        /// KmlTimeSpan Api name
        /// </summary>
        public const string KmlTimeSpan = "KmlTimeSpan";

        /// <summary>
        /// KmlTimeStamp Api name
        /// </summary>
        public const string KmlTimeStamp = "KmlTimeStamp";

        /// <summary>
        /// KmlTour Api name
        /// </summary>
        public const string KmlTour = "KmlTour";

        /// <summary>
        /// KmlVec2 Api name
        /// </summary>
        public const string KmlVec2 = "KmlVec2";

        /// <summary>
        /// KmlViewerOptions Api name
        /// </summary>
        public const string KmlViewerOptions = "KmlViewerOptions";

        #endregion
    }
}
