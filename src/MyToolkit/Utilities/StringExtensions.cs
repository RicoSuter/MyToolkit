//-----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace MyToolkit.Utilities
{
    /// <summary>Provides methods to manipulate strings. </summary>
    public static class StringExtensions
    {
#if !LEGACY

        /// <summary>Converts a string to an enum value. </summary>
        /// <typeparam name="TEnum">The enum type. </typeparam>
        /// <param name="value">The value. </param>
        /// <param name="defaultValue">The default value which is returned when the value could not be parsed. </param>
        /// <returns>The enum value. </returns>
        /// <exception cref="ArgumentException">TEnum must be an enum. </exception>
        public static TEnum ToEnum<TEnum>(this string value, TEnum defaultValue)
            where TEnum : struct, IComparable, IFormattable
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
                throw new ArgumentException("TEnum must be an enum. ");

            TEnum result;
            if (Enum.TryParse(value, true, out result))
                return result;

            return defaultValue;
        }

        /// <summary>Converts a string to an enum value. </summary>
        /// <typeparam name="TEnum">The enum type. </typeparam>
        /// <param name="value">The value. </param>
        /// <returns>The enum value. </returns>
        /// <exception cref="ArgumentException">TEnum must be an enum. </exception>
        public static TEnum? ToEnum<TEnum>(this string value)
            where TEnum : struct, IComparable, IFormattable
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
                throw new ArgumentException("TEnum must be an enum. ");

            TEnum result;
            if (Enum.TryParse(value, true, out result))
                return result;

            return null;
        }

#endif

        /// <summary>Correctly URI escapes the given string. </summary>
        /// <param name="value">The string to escape. </param>
        /// <returns>The escaped string</returns>
        public static string EscapeUriString(this string value)
        {
            const int limit = 32768;
            var sb = new StringBuilder();
            var loops = value.Length / limit;
            for (int i = 0; i <= loops; i++)
            {
                if (i < loops)
                    sb.Append(Uri.EscapeDataString(value.Substring(limit * i, limit)));
                else
                    sb.Append(Uri.EscapeDataString(value.Substring(limit * i)));
            }
            return sb.ToString();
        }

        /// <summary>Splits a string into key-value pairs (format: 'key1:value2,key2:value2'). </summary>
        /// <param name="text">The parameter string. </param>
        /// <returns>The parsed dictionary. </returns>
        public static Dictionary<string, string> GetConverterParameters(this string text)
        {
            var output = new Dictionary<string, string>();
            foreach (var item in text.Split(','))
            {
                var arr = item.Split(':');
                if (arr.Length == 2)
                    output[arr[0].ToLower()] = arr[1];
                else
                    output[arr[0].ToLower()] = "";
            }
            return output;
        }

        /// <summary>Removes all HTML tags from the given string. </summary>
        /// <param name="html">The HTML string to remove the HTML tags from. </param>
        /// <returns>The cleaned string. </returns>
        public static string RemoveHtmlTags(this string html)
        {
            return Regex.Replace(html, "<[^>]*>", string.Empty);
        }

        /// <summary>Removes all HTML links from the given string. </summary>
        /// <param name="html">The HTML string to remove the links from. </param>
        /// <returns>The cleaned string. </returns>
        public static string RemoveHtmlLinks(this string html)
        {
            return Regex.Replace(html, "<a[^>]*>[^<]*</a>", string.Empty);
        }

        /// <summary>Converts all contained UTF-8 characters in the string to their correct internal representation. </summary>
        /// <param name="text">The string to convert. </param>
        /// <returns>The converted string. </returns>
        public static string ConvertUtf8Characters(this string text)
        {
            return text.
                Replace("\u008B", "‹").
                Replace("\u009B", "›");
        }

        /// <summary>Converts all HTML entities to their correct character representation. </summary>
        /// <param name="html">The string to convert. </param>
        /// <returns>The converted string. </returns>
        public static string ConvertHtmlCharacters(this string html)
        {
            if (html == null)
                return null;

            if (html.IndexOf('&') < 0)
                return html;

            var sb = new StringBuilder();
            var writer = new StringWriter(sb, CultureInfo.InvariantCulture);
            var length = html.Length;
            for (var i = 0; i < length; i++)
            {
                var ch = html[i];
                if (ch == '&')
                {
                    var num3 = html.IndexOfAny(new char[] { ';', '&' }, i + 1);
                    if ((num3 > 0) && (html[num3] == ';'))
                    {
                        var entity = html.Substring(i + 1, (num3 - i) - 1);
                        if ((entity.Length > 1) && (entity[0] == '#'))
                        {
                            try
                            {
                                if ((entity[1] == 'x') || (entity[1] == 'X'))
                                    ch = (char)int.Parse(entity.Substring(2), NumberStyles.AllowHexSpecifier,
                                        CultureInfo.InvariantCulture);
                                else
                                    ch = (char)int.Parse(entity.Substring(1), CultureInfo.InvariantCulture);
                                i = num3;
                            }
                            catch (FormatException)
                            {
                                i++;
                            }
                            catch (ArgumentException)
                            {
                                i++;
                            }
                        }
                        else
                        {
                            i = num3;
                            var ch2 = HtmlEntities.Lookup(entity);
                            if (ch2 != '\0')
                                ch = ch2;
                            else
                            {
                                writer.Write('&');
                                writer.Write(entity);
                                writer.Write(';');
                                continue;
                            }
                        }
                    }
                }
                writer.Write(ch);
            }

            return sb.ToString();
        }

        /// <summary>Removes unneeded (hidden) HTML whitespaces. </summary>
        /// <param name="html">The HTML string to convert. </param>
        /// <returns>The transformed string. </returns>
        public static string RemoveHtmlWhitespaces(this string html)
        {
            html = new Regex(@"\n\s+").Replace(html, " ");
            html = new Regex(@"<br />\s+").Replace(html, "<br />");
            html = html.Replace("\n", " ").Replace("\r", "");
            html = new Regex(@">(\s|\t)+<").Replace(html, "><");
            return html;
        }

        /// <summary>Removes the HTML comments from the given HTML.</summary>
        /// <param name="html">The HTML.</param>
        /// <returns>The HTML without comments.</returns>
        public static string RemoveHtmlComments(this string html)
        {
            return new Regex(@"<!--(.*?)-->").Replace(html, string.Empty);
        }

        /// <summary>Truncates a string without chopping whole words. </summary>
        /// <param name="text">The string to truncate. </param>
        /// <param name="length">The maximum string length of the result. </param>
        /// <returns>The truncated string. </returns>
        public static string TruncateWithoutChopping(this string text, int length)
        {
            if (text == null || text.Length < length)
                return text;

            var index = text.LastIndexOf(" ", length, StringComparison.Ordinal);
            return string.Format("{0}...", text.Substring(0, (index > 0) ? index : text.Length).Trim());
        }

        /// <summary>Trims a string from the start of the input string. </summary>
        /// <param name="input">The input string. </param>
        /// <param name="trimString">The string to trim. </param>
        /// <returns>The trimmed string. </returns>
        public static string TrimStart(this string input, string trimString)
        {
            var result = input;
            while (result.StartsWith(trimString))
                result = result.Substring(trimString.Length);
            return result;
        }

        /// <summary>Trims a string from the end of the input string. </summary>
        /// <param name="input">The input string. </param>
        /// <param name="trimString">The string to trim. </param>
        /// <returns>The trimmed string. </returns>
        public static string TrimEnd(this string input, string trimString)
        {
            var result = input;
            while (result.EndsWith(trimString))
                result = result.Substring(0, result.Length - trimString.Length);
            return result;
        }

        /// <summary>Trims a string from the start and end of the input string. </summary>
        /// <param name="input">The input string. </param>
        /// <param name="trimString">The string to trim. </param>
        /// <returns>The trimmed string. </returns>
        public static string Trim(this string input, string trimString)
        {
            return input.TrimStart(trimString).TrimEnd(trimString);
        }

        /// <summary>Extracts a language string from the given input string (format: 'en:Hello;de:Hallo;fr:"semi;colon"'). </summary>
        /// <param name="input">The input string. </param>
        /// <param name="language">The language. </param>
        /// <returns>The extracted string. </returns>
        public static string ExtractLocalizedString(this string input, string language = null)
        {
            if (input == null)
                return null;

            if (language == null)
                language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            var mapping = new Dictionary<string, string>();
            var position = 0;
            string key = null;
            while (true)
            {
                if (key == null) // find language
                {
                    var index = input.IndexOf(':', position);
                    if (index == -1)
                        break;

                    key = input.Substring(position, index - position).Trim();
                    position = index + 1;
                }
                else
                {
                    var semiIndex = input.IndexOf(';', position);
                    var startQuoteIndex = input.IndexOf('"', position);

                    if (startQuoteIndex != -1 && (semiIndex == -1 || startQuoteIndex < semiIndex))
                    {
                        position = startQuoteIndex + 1;
                        while (true)
                        {
                            position = input.IndexOf('"', position);
                            if (position == -1)
                                return "wrong_quoting";

                            position++;
                            if (input[position - 2] != '\\')
                                break;
                        }

                        if (position == -1)
                            break;

                        mapping.Add(key, input.Substring(startQuoteIndex + 1, position - startQuoteIndex - 2).Replace("\\\"", "\"").Trim());
                        position = input.IndexOf(';', position);

                        if (position == -1)
                            break;
                        position++;
                    }
                    else
                    {
                        if (semiIndex == -1)
                        {
                            mapping.Add(key, input.Substring(position).Trim());
                            break;
                        }
                        mapping.Add(key, input.Substring(position, semiIndex - position).Trim());
                        position = semiIndex + 1;
                    }

                    key = null;
                }
            }

            return mapping.Count > 0 ? (mapping.ContainsKey(language) ? mapping[language] : mapping.First().Value) : input;
        }

        internal static class HtmlEntities
        {
            static HtmlEntities()
            {
                _lookupLockObject = new object();
                _entitiesList = new[] { 
                    "\"-quot", "&-amp", "<-lt", ">-gt", "\x00a0-nbsp", "\x00a1-iexcl", "\x00a2-cent", "\x00a3-pound", "\x00a4-curren", "\x00a5-yen", "\x00a6-brvbar", "\x00a7-sect", "\x00a8-uml", "\x00a9-copy", "\x00aa-ordf", "\x00ab-laquo", 
                    "\x00ac-not", "\x00ad-shy", "\x00ae-reg", "\x00af-macr", "\x00b0-deg", "\x00b1-plusmn", "\x00b2-sup2", "\x00b3-sup3", "\x00b4-acute", "\x00b5-micro", "\x00b6-para", "\x00b7-middot", "\x00b8-cedil", "\x00b9-sup1", "\x00ba-ordm", "\x00bb-raquo", 
                    "\x00bc-frac14", "\x00bd-frac12", "\x00be-frac34", "\x00bf-iquest", "\x00c0-Agrave", "\x00c1-Aacute", "\x00c2-Acirc", "\x00c3-Atilde", "\x00c4-Auml", "\x00c5-Aring", "\x00c6-AElig", "\x00c7-Ccedil", "\x00c8-Egrave", "\x00c9-Eacute", "\x00ca-Ecirc", "\x00cb-Euml", 
                    "\x00cc-Igrave", "\x00cd-Iacute", "\x00ce-Icirc", "\x00cf-Iuml", "\x00d0-ETH", "\x00d1-Ntilde", "\x00d2-Ograve", "\x00d3-Oacute", "\x00d4-Ocirc", "\x00d5-Otilde", "\x00d6-Ouml", "\x00d7-times", "\x00d8-Oslash", "\x00d9-Ugrave", "\x00da-Uacute", "\x00db-Ucirc", 
                    "\x00dc-Uuml", "\x00dd-Yacute", "\x00de-THORN", "\x00df-szlig", "\x00e0-agrave", "\x00e1-aacute", "\x00e2-acirc", "\x00e3-atilde", "\x00e4-auml", "\x00e5-aring", "\x00e6-aelig", "\x00e7-ccedil", "\x00e8-egrave", "\x00e9-eacute", "\x00ea-ecirc", "\x00eb-euml", 
                    "\x00ec-igrave", "\x00ed-iacute", "\x00ee-icirc", "\x00ef-iuml", "\x00f0-eth", "\x00f1-ntilde", "\x00f2-ograve", "\x00f3-oacute", "\x00f4-ocirc", "\x00f5-otilde", "\x00f6-ouml", "\x00f7-divide", "\x00f8-oslash", "\x00f9-ugrave", "\x00fa-uacute", "\x00fb-ucirc", 
                    "\x00fc-uuml", "\x00fd-yacute", "\x00fe-thorn", "\x00ff-yuml", "\u0152-OElig", "\u0153-oelig", "\u0160-Scaron", "\u0161-scaron", "\u0178-Yuml", "\u0192-fnof", "\u02c6-circ", "\u02dc-tilde", "\u0391-Alpha", "\u0392-Beta", "\u0393-Gamma", "\u0394-Delta", 
                    "\u0395-Epsilon", "\u0396-Zeta", "\u0397-Eta", "\u0398-Theta", "\u0399-Iota", "\u039a-Kappa", "\u039b-Lambda", "\u039c-Mu", "\u039d-Nu", "\u039e-Xi", "\u039f-Omicron", "\u03a0-Pi", "\u03a1-Rho", "\u03a3-Sigma", "\u03a4-Tau", "\u03a5-Upsilon", 
                    "\u03a6-Phi", "\u03a7-Chi", "\u03a8-Psi", "\u03a9-Omega", "\u03b1-alpha", "\u03b2-beta", "\u03b3-gamma", "\u03b4-delta", "\u03b5-epsilon", "\u03b6-zeta", "\u03b7-eta", "\u03b8-theta", "\u03b9-iota", "\u03ba-kappa", "\u03bb-lambda", "\u03bc-mu", 
                    "\u03bd-nu", "\u03be-xi", "\u03bf-omicron", "\u03c0-pi", "\u03c1-rho", "\u03c2-sigmaf", "\u03c3-sigma", "\u03c4-tau", "\u03c5-upsilon", "\u03c6-phi", "\u03c7-chi", "\u03c8-psi", "\u03c9-omega", "\u03d1-thetasym", "\u03d2-upsih", "\u03d6-piv", 
                    "\u2002-ensp", "\u2003-emsp", "\u2009-thinsp", "\u200c-zwnj", "\u200d-zwj", "\u200e-lrm", "\u200f-rlm", "\u2013-ndash", "\u2014-mdash", "\u2018-lsquo", "\u2019-rsquo", "\u201a-sbquo", "\u201c-ldquo", "\u201d-rdquo", "\u201e-bdquo", "\u2020-dagger", 
                    "\u2021-Dagger", "\u2022-bull", "\u2026-hellip", "\u2030-permil", "\u2032-prime", "\u2033-Prime", "\u2039-lsaquo", "\u203a-rsaquo", "\u203e-oline", "\u2044-frasl", "\u20ac-euro", "\u2111-image", "\u2118-weierp", "\u211c-real", "\u2122-trade", "\u2135-alefsym", 
                    "\u2190-larr", "\u2191-uarr", "\u2192-rarr", "\u2193-darr", "\u2194-harr", "\u21b5-crarr", "\u21d0-lArr", "\u21d1-uArr", "\u21d2-rArr", "\u21d3-dArr", "\u21d4-hArr", "\u2200-forall", "\u2202-part", "\u2203-exist", "\u2205-empty", "\u2207-nabla", 
                    "\u2208-isin", "\u2209-notin", "\u220b-ni", "\u220f-prod", "\u2211-sum", "\u2212-minus", "\u2217-lowast", "\u221a-radic", "\u221d-prop", "\u221e-infin", "\u2220-ang", "\u2227-and", "\u2228-or", "\u2229-cap", "\u222a-cup", "\u222b-int", 
                    "\u2234-there4", "\u223c-sim", "\u2245-cong", "\u2248-asymp", "\u2260-ne", "\u2261-equiv", "\u2264-le", "\u2265-ge", "\u2282-sub", "\u2283-sup", "\u2284-nsub", "\u2286-sube", "\u2287-supe", "\u2295-oplus", "\u2297-otimes", "\u22a5-perp", 
                    "\u22c5-sdot", "\u2308-lceil", "\u2309-rceil", "\u230a-lfloor", "\u230b-rfloor", "\u2329-lang", "\u232a-rang", "\u25ca-loz", "\u2660-spades", "\u2663-clubs", "\u2665-hearts", "\u2666-diams"
               };
            }

            internal static char Lookup(string entity)
            {
                if (_entitiesLookupTable == null)
                {
                    lock (_lookupLockObject)
                    {
                        if (_entitiesLookupTable == null)
                        {
                            var dictionary = new Dictionary<string, char>();
                            foreach (string text1 in _entitiesList)
                                dictionary[text1.Substring(2)] = text1[0];
                            _entitiesLookupTable = dictionary;
                        }
                    }
                }

                char character;
                if (_entitiesLookupTable.TryGetValue(entity, out character))
                    return character;
                return '\0';
            }

            private static readonly string[] _entitiesList;
            private static Dictionary<string, char> _entitiesLookupTable;
            private static readonly object _lookupLockObject;
        }
    }
}
