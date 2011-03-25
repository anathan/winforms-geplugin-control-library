// <copyright file="JSFunction.cs" company="FC">
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
    /// A collection the names of javascript functions that should be available in the active document.
    /// These are wrapper functions to allow access to objects outside the Google Earth Api.
    /// </summary>
    /// <remarks>
    /// This collection can be thought of as a simple kind of an interface for javascipt methods in an active document.
    /// That is, any function name listed here should be present in a document that is designed to work with the controls.
    /// </remarks>
    public struct JSFunction
    {
        /// <summary>
        /// The javascript method name of the google.earth.addEventListener wrapper.
        /// </summary>
        public const string AddEventListener = "jsAddEventListener";

        /// <summary>
        /// The javascript method name of the google.earth.createInstance wrapper.
        /// </summary>
        public const string CreateInstance = "jsCreateInstance";

        /// <summary>
        /// The javascript method name of the  GClientGeocoder.getLatLng wrapper.
        /// </summary>
        public const string DoGeocode = "jsDoGeocode";

        /// <summary>
        /// The javascript method name of the eval wrapper.
        /// </summary>
        public const string Evaluate = "jsEvaluate";

        /// <summary>
        /// The javascript method name of the  window.execScript wrapper.
        /// </summary>
        public const string Execute = "jsExecute";

        /// <summary>
        /// The javascript method name of the google.earth.fetchKml wrapper.
        /// </summary>
        public const string FetchKml = "jsFetchKml";

        /// <summary>
        /// The javascript method name of the google.earth.getLanguage wrapper.
        /// </summary>
        public const string GetLanguage = "jsGetLanguage";

        /// <summary>
        /// The javascript method name of the google.earth.isInstalled wrapper.
        /// </summary>
        public const string IsInstalled = "jsIsInstalled";

        /// <summary>
        /// The javascript method name of the google.earth.removeEventListener wrapper.
        /// </summary>
        public const string RemoveEventListener = "jsRemoveEventListener";

        /// <summary>
        /// The javascript method name of the google.earth.setLanguage wrapper.
        /// </summary>
        public const string SetLanguage = "jsSetLanguage";
    }
}
