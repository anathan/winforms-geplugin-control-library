// <copyright file="ViewerOptions.cs" company="FC">
//   Copyright (c) 2008 - 2012 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2012-11-20</date>
// <summary>
//   This file is part of FC.GEPlugin
//   FC.GEPlugin is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License
//   along with this program. If not, see http://www.gnu.org/licenses/.
// </summary>

#region

using System;
using System.Runtime.InteropServices;

#endregion

namespace FC.GEPluginCtrls
{
    /// <summary>
    /// Enumeration of options for use with <see cref="ViewerOptions"/>
    /// </summary>
    public enum ViewerOption
    {
        /// <summary>
        /// No option
        /// </summary>
        None = 0,

        /// <summary>
        /// Passed to the ViewerOptions.SetOption method, along with a OptionsValue, to specify whether the Sun option should be visible.
        /// </summary>
        Sunlight = 1,

        /// <summary>
        /// Passed to the ViewerOptions.SetOption method, along with a OptionsValue, to specify whether historical imagery should be enabled.
        /// </summary>
        HistoricalImagery = 2,

        /// <summary>
        /// Passed to the ViewerOptions.SetOption method, along with a OptionsValue, to specify whether Street View should be enabled when the view reaches ground level. Note that this applies only to programmatic movement, such as fly-to;
        /// </summary>
        StreetView = 3
    }

    /// <summary>
    /// Enumeration of states to use with <see cref="ViewerOptions"/>
    /// </summary>
    public enum OptionState
    {
        /// <summary>
        /// Sets the render state to its default value.
        /// </summary>
        /// <remarks>
        /// Currently, sunlight, Street View, and historical imagery all default to a disabled state.
        /// </remarks>
        Default = 0,

        /// <summary>
        /// Set the render state to on. Passed to the KmlViewerOptions.setOption method.
        /// </summary>
        Enabled = 1,

        /// <summary>
        /// Set the render state to off. Passed to the KmlViewerOptions.setOption method.
        /// </summary>
        Disabled = 2
    }

    /// <summary>
    /// Controls the display of sunlight, historical imagery, and Street View panoramas in the plug-in.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Unlike kmlViewerOptions in the api this class automatically applies any changes when SetOption is called.
    /// This negates the user having to create a camera, set its options, then apply its view to the current abstract view to see the change.
    /// </para>
    /// </remarks>
    public sealed class ViewerOptions
    {
        /// <summary>
        /// The ViewerOptions object
        /// </summary>
        private readonly dynamic viewerOptions;

        private readonly dynamic plugin;

        private readonly dynamic camera;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewerOptions"/> class. 
        /// Initializes a new instance of the ViewerOptions class.
        /// </summary>
        /// <param name="ge">
        /// The plug-in object 
        /// </param>
        public ViewerOptions(dynamic ge)
        {
            if (!GEHelpers.IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            this.plugin = ge;
            this.viewerOptions = plugin.createViewerOptions(string.Empty);
            this.camera = plugin.createCamera(string.Empty);

        }

        /// <summary>
        /// Returns the current state of the specified option.
        /// </summary>
        /// <param name="option">
        /// The ViewerOption. 
        /// </param>
        /// <returns>
        /// The current ViewerOptionsValue for the specified ViewerOption 
        /// </returns>
        public OptionState GetOption(ViewerOption option)
        {
            try
            {
                int i = this.viewerOptions.getOption((int) option);
                if (Enum.IsDefined(typeof (OptionState), option))
                {
                    return (OptionState) i;
                }
            }
            catch (COMException)
            {
            }

            return OptionState.Default;
        }

        /// <summary>
        /// Set the state of viewer options, including sunlight, Street View, and historical imagery.
        /// </summary>
        /// <param name="option">
        /// The ViewerOption. 
        /// </param>
        /// <param name="value">
        /// The ViewerOptionsValue. 
        /// </param>
        public void SetOption(ViewerOption option, OptionState value)
        {
            try
            {
                this.viewerOptions.setOption((int) option, (int) value);
                ApplyOptions();
            }
            catch (COMException)
            {
            }
        }

        /// <summary>
        /// Applies the ViewerOptions to the current view.
        /// </summary>
        private void ApplyOptions()
        {
            camera.setViewerOptions(viewerOptions);
            plugin.getView().setAbstractView(camera);
        }
    }
}