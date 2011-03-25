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
namespace FC.GEPluginCtrls
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Resources;

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
        public static Dictionary<string, string> GetList()
        {
            Dictionary<string, string> list = 
                new Dictionary<string, string>();

            list.Add("ar", "العربية"); // Arabic
            list.Add("eu", "Euskara"); // Basque 
            list.Add("bg", "български език"); // Bulgarian
            list.Add("bn", "বাংলা"); // Bengai
            list.Add("ca", "Català"); // Catalan
            list.Add("cs", "ceština"); // Czech
            list.Add("da", "Dansk"); // Danish
            list.Add("de", "Deutsch"); // German
            list.Add("el", "ελληνικά"); // Greek
            list.Add("en", "English"); // English
            list.Add("en-AU", "English (AU)"); // Australian English
            list.Add("en-GB", "English (GB)"); // British English
            list.Add("es", "Español"); // Spanish
            list.Add("fa", "فارسی"); // Farsi
            list.Add("fi", "suomi"); // Finish
            list.Add("fil", "Filipino"); // Filipino
            list.Add("fr", "français"); // French
            list.Add("gl", "Galego"); // Galician
            list.Add("gu", "ગુજરાતી"); // Gujarati
            list.Add("hi", "हिन्दी"); // Hindi
            list.Add("hr", "hrvatski"); // Croatian
            list.Add("hu", "magyar nyelv"); // Hungarian
            list.Add("id", "Indonesia"); // Indonesian
            list.Add("it", "italiano"); // Italian
            list.Add("iw", "עִבְרִית"); // Hebrew
            list.Add("ja", "日本語"); // Japanese
            list.Add("kn", "ಕನ್ನಡ"); // Kannada
            list.Add("ko", "한국어"); // Korean
            list.Add("lt", "lietuvių"); // Lithuanian
            list.Add("lv", "latviešu"); // Latvian
            list.Add("ml", "മലയാളം"); // Malayalam
            list.Add("mr", "मराठी"); // Marathi
            list.Add("nl", "Nederlands"); // Dutch
            list.Add("nn", "Nynorsk"); // Norwegian (NN)
            list.Add("no", "norsk"); // Nowwegian (NO)
            list.Add("or", "ଓଡିଆ"); // Oriya
            list.Add("pl", "polski"); // Polish
            list.Add("pt", "português"); // Portuguese
            list.Add("pt-BR", "português brasileiro"); // Portuguese (BR)
            list.Add("rm", "Romansch"); // Romansch
            list.Add("ro", "română"); // Romanian
            list.Add("ru", "русский язык"); // Russian
            list.Add("sk", "slovenčina"); // Slovak
            list.Add("sl", "slovenščina"); // Slovenian
            list.Add("sr", "Српски"); // Serbian
            list.Add("sv", "nysvenska"); // Swedish
            list.Add("tl", "Tagalog "); // Tagalog
            list.Add("ta", "தமிழ்"); // Tamil
            list.Add("te", "తెలుగు"); // Telugu
            list.Add("th", "ภาษาไทย"); // Thai
            list.Add("tr", "Türkçe"); // Turkish
            list.Add("uk", "украї́нська мо́ва"); // Ukrainian
            list.Add("vi", "tiếng Việt"); // Vietnamese
            list.Add("zh-CN", "简体字"); // Chinese (CN)
            list.Add("zh-TW", "中文"); // Chinese (TW)

            return list;
        }
    }
}