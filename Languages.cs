// <copyright file="Languages.cs" company="FC">
// Copyright (c) 2010 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2010-05-</date>
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

#region

using System.Collections.Generic;

#endregion

namespace FC.GEPluginCtrls
{
    /// <summary>
    /// A class to provide the supported language codes
    /// Supported language codes are listed in the Google Maps API Coverage document
    /// see: http://spreadsheets.google.com/pub?key=p9pdwsai2hDMsLkXsoM05KQ
    /// </summary>
    public static class Languages
    {
        /// <summary>
        /// Provides a Dictionary of language codes and native names 
        /// </summary>
        /// <returns>A Dictionary of language codes and native names</returns>
        public static Dictionary<string, string> Codes()
        {
            Dictionary<string, string> list = 
                new Dictionary<string, string>
                    {
                        {"ar", "العربية"},
                        {"eu", "Euskara"},
                        {"bg", "български език"},
                        {"bn", "বাংলা"},
                        {"ca", "Català"},
                        {"cs", "ceština"},
                        {"da", "Dansk"},
                        {"de", "Deutsch"},
                        {"el", "ελληνικά"},
                        {"en", "English"},
                        {"en-AU", "English (AU)"},
                        {"en-GB", "English (GB)"},
                        {"es", "Español"},
                        {"fa", "فارسی"},
                        {"fi", "suomi"},
                        {"fil", "Filipino"},
                        {"fr", "français"},
                        {"gl", "Galego"},
                        {"gu", "ગુજરાતી"},
                        {"hi", "हिन्दी"},
                        {"hr", "hrvatski"},
                        {"hu", "magyar nyelv"},
                        {"id", "Indonesia"},
                        {"it", "italiano"},
                        {"iw", "עִבְרִית"},
                        {"ja", "日本語"},
                        {"kn", "ಕನ್ನಡ"},
                        {"ko", "한국어"},
                        {"lt", "lietuvių"},
                        {"lv", "latviešu"},
                        {"ml", "മലയാളം"},
                        {"mr", "मराठी"},
                        {"nl", "Nederlands"},
                        {"nn", "Nynorsk"},
                        {"no", "norsk"},
                        {"or", "ଓଡିଆ"},
                        {"pl", "polski"},
                        {"pt", "português"},
                        {"pt-BR", "português brasileiro"},
                        {"rm", "Romansch"},
                        {"ro", "română"},
                        {"ru", "русский язык"},
                        {"sk", "slovenčina"},
                        {"sl", "slovenščina"},
                        {"sr", "Српски"},
                        {"sv", "nysvenska"},
                        {"tl", "Tagalog "},
                        {"ta", "தமிழ்"},
                        {"te", "తెలుగు"},
                        {"th", "ภาษาไทย"},
                        {"tr", "Türkçe"},
                        {"uk", "украї́нська мо́ва"},
                        {"vi", "tiếng Việt"},
                        {"zh-CN", "简体字"},
                        {"zh-TW", "中文"}
                    };

            return list;
        }
    }
}