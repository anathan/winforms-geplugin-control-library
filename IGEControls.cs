// <copyright file="IGEControls.cs" company="FC">
// Copyright (c) 2008 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2008-12-22</date>
// <summary>This program is part of FC.GEPluginCtrls
// FC.GEPluginCtrls is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </summary>
namespace FC.GEPluginCtrls
{
    /// <summary>
    /// This interface should be inherited by all the controls
    /// It allows the control access to both the plugin and htmlDoument
    /// </summary>
    public interface IGEControls
    {
        /// <summary>
        /// Tell the control the instance of GEWebBrowser to work with
        /// </summary>
        /// <param name="instance">The GEWebBrowser instance</param>
        void SetBrowserInstance(GEWebBrowser instance);
    }
}

