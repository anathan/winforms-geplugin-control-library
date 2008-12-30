// <copyright file="IHTMLScriptElement.cs" company="FC">
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
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// A COM interface is needed because .NET does not provide a way
    /// to set the properties of a HTML script element.
    /// This negates the need to refrence mshtml
    /// </summary>
    [ComImport, Guid("3050F536-98B5-11CF-BB82-00AA00BDCE0B"),
    InterfaceType((short)2),
    TypeLibType((short)0x4112)]
    public interface IHTMLScriptElement
    {
        /// <summary>
        /// Sets the text property
        /// </summary>
        [DispId(1006)]
        string Text
        {
            [param: MarshalAs(UnmanagedType.BStr)]
            [PreserveSig,
            MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime),
            DispId(-2147417085)]
            set;
        }

        /// <summary>
        /// Sets the src property
        /// </summary>
        [DispId(1001)]
        string Src
        {
            [param: MarshalAs(UnmanagedType.BStr)]
            [PreserveSig,
            MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime),
            DispId(-1001)]
            set;
        }
    }
}