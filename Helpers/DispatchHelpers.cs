// <copyright file="DispatchHelpers.cs" company="FC">
// Copyright (c) 2008 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2008-12-22</date>
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
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using Microsoft.CSharp.RuntimeBinder;

    /// <summary>
    /// Helper methods to invoke properties on unamanged types
    /// Thanks to Bill Jeam from
    /// http://blogs.msdn.com/ericlippert/archive/2003/09/22/jscript-and-vbscript-arrays.aspx
    /// </summary>
    /// <remarks>See http://code.google.com/p/winforms-geplugin-control-library/issues/detail?id=14</remarks>
    internal class DispatchHelpers
    {
        /// <summary>
        /// Converts a javascript array to a managed array object
        /// </summary>
        /// <param name="comObject">A javascript COM object</param>
        /// <returns>An array of objects</returns>
        internal static object[] GetObjectArrayFrom__COMObjectArray(object comObject)
        {
            int length = Convert.ToInt32(GetPropertyValue(comObject, "length"));
            object[] objArr = new object[length];

            for (int idx = 0; idx < length; idx++)
            {
                objArr[idx] = GetPropertyValue(comObject, idx.ToString());
            }

            return objArr;
        }

        /// <summary>
        /// Gets the value of an internal property on a target object
        /// </summary>
        /// <param name="dispObject">An unamaged object</param>
        /// <param name="property">The name of the property</param>
        /// <returns>The value of the property </returns>
        internal static object GetPropertyValue(object dispObject, string property)
        {
            object propValueRef = null;

            try
            {
                Type type = dispObject.GetType();
                propValueRef = type.InvokeMember(
                    property,
                    BindingFlags.GetProperty,
                    null,
                    dispObject,
                    null);
            }
            catch (RuntimeBinderException ex) 
            {
                Debug.WriteLine(ex.ToString());
                ////throw;
            }

            return propValueRef;
        }
    }
}