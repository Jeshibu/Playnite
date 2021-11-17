﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace System
{
    public static class StringExtensions
    {
        private static readonly CultureInfo enUSCultInfo = new CultureInfo("en-US", false);

        public static string MD5(this string s)
        {
            var builder = new StringBuilder();
            foreach (byte b in MD5Bytes(s))
            {
                builder.Append(b.ToString("x2").ToLower());
            }

            return builder.ToString();
        }

        public static byte[] MD5Bytes(this string s)
        {
            using (var provider = System.Security.Cryptography.MD5.Create())
            {
                return provider.ComputeHash(Encoding.UTF8.GetBytes(s));
            }
        }

        public static string ConvertToSortableName(this string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }

            var converter = new SortableNameConverter(new[] { "The", "A", "An" });
            return converter.Convert(name);
        }

        public static string RemoveTrademarks(this string str, string remplacement = "")
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }

            return Regex.Replace(str, @"[™©®]", remplacement);
        }

        public static bool IsNullOrEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        public static bool IsNullOrWhiteSpace(this string source)
        {
            return string.IsNullOrWhiteSpace(source);
        }

        public static string Format(this string source, params object[] args)
        {
            return string.Format(source, args);
        }

        public static string TrimEndString(this string source, string value, StringComparison comp = StringComparison.Ordinal)
        {
            if (!source.EndsWith(value, comp))
            {
                return source;
            }

            return source.Remove(source.LastIndexOf(value, comp));
        }

        public static string ToTileCase(this string source, CultureInfo culture = null)
        {
            if (source.IsNullOrEmpty())
            {
                return source;
            }

            if (culture != null)
            {
                return culture.TextInfo.ToTitleCase(source);
            }
            else
            {
                return enUSCultInfo.TextInfo.ToTitleCase(source);
            }
        }

        private static string RemoveUnlessThatEmptiesTheString(string input, string pattern)
        {
            string output = Regex.Replace(input, pattern, string.Empty);

            if (string.IsNullOrWhiteSpace(output))
            {
                return input;
            }
            return output;
        }

        public static string NormalizeGameName(this string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }

            var newName = name;
            newName = newName.RemoveTrademarks();
            newName = newName.Replace("_", " ");
            newName = newName.Replace(".", " ");
            newName = RemoveTrademarks(newName);
            newName = newName.Replace('’', '\'');
            newName = RemoveUnlessThatEmptiesTheString(newName, @"\[.*?\]");
            newName = RemoveUnlessThatEmptiesTheString(newName, @"\(.*?\)");
            newName = Regex.Replace(newName, @"\s*:\s*", ": ");
            newName = Regex.Replace(newName, @"\s+", " ");
            if (Regex.IsMatch(newName, @",\s*The$"))
            {
                newName = "The " + Regex.Replace(newName, @",\s*The$", "", RegexOptions.IgnoreCase);
            }

            return newName.Trim();
        }

        public static string GetSHA256Hash(this string input)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }

        public static string GetPathWithoutAllExtensions(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            return Regex.Replace(path, @"(\.[A-Za-z0-9]+)+$", "");
        }

        public static bool Contains(this string str, string value, StringComparison comparisonType)
        {
            return str.IndexOf(value, 0, comparisonType) != -1;
        }

        public static bool ContainsAny(this string str, char[] chars)
        {
            return str.IndexOfAny(chars) >= 0;
        }

        public static bool IsHttpUrl(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            return Regex.IsMatch(str, @"^https?:\/\/", RegexOptions.IgnoreCase);
        }

        public static bool IsUri(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            return Uri.IsWellFormedUriString(str, UriKind.Absolute);
        }

        public static string UrlEncode(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return HttpUtility.UrlPathEncode(str);
        }

        public static string UrlDecode(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return HttpUtility.UrlDecode(str);
        }

        // Courtesy of https://stackoverflow.com/questions/6275980/string-replace-ignoring-case
        public static string Replace(this string str, string oldValue, string @newValue, StringComparison comparisonType)
        {
            // Check inputs.
            if (str == null)
            {
                // Same as original .NET C# string.Replace behavior.
                throw new ArgumentNullException(nameof(str));
            }
            if (str.Length == 0)
            {
                // Same as original .NET C# string.Replace behavior.
                return str;
            }
            if (oldValue == null)
            {
                // Same as original .NET C# string.Replace behavior.
                throw new ArgumentNullException(nameof(oldValue));
            }
            if (oldValue.Length == 0)
            {
                // Same as original .NET C# string.Replace behavior.
                throw new ArgumentException("String cannot be of zero length.");
            }

            // Prepare string builder for storing the processed string.
            // Note: StringBuilder has a better performance than String by 30-40%.
            StringBuilder resultStringBuilder = new StringBuilder(str.Length);

            // Analyze the replacement: replace or remove.
            bool isReplacementNullOrEmpty = string.IsNullOrEmpty(@newValue);

            // Replace all values.
            const int valueNotFound = -1;
            int foundAt;
            int startSearchFromIndex = 0;
            while ((foundAt = str.IndexOf(oldValue, startSearchFromIndex, comparisonType)) != valueNotFound)
            {
                // Append all characters until the found replacement.
                int @charsUntilReplacment = foundAt - startSearchFromIndex;
                bool isNothingToAppend = @charsUntilReplacment == 0;
                if (!isNothingToAppend)
                {
                    resultStringBuilder.Append(str, startSearchFromIndex, @charsUntilReplacment);
                }

                // Process the replacement.
                if (!isReplacementNullOrEmpty)
                {
                    resultStringBuilder.Append(@newValue);
                }

                // Prepare start index for the next search.
                // This needed to prevent infinite loop, otherwise method always start search
                // from the start of the string. For example: if an oldValue == "EXAMPLE", newValue == "example"
                // and comparisonType == "any ignore case" will conquer to replacing:
                // "EXAMPLE" to "example" to "example" to "example" … infinite loop.
                startSearchFromIndex = foundAt + oldValue.Length;
                if (startSearchFromIndex == str.Length)
                {
                    // It is end of the input string: no more space for the next search.
                    // The input string ends with a value that has already been replaced.
                    // Therefore, the string builder with the result is complete and no further action is required.
                    return resultStringBuilder.ToString();
                }
            }

            // Append the last part to the result.
            int @charsUntilStringEnd = str.Length - startSearchFromIndex;
            resultStringBuilder.Append(str, startSearchFromIndex, @charsUntilStringEnd);
            return resultStringBuilder.ToString();
        }
    }

    public class SortableNameConverter
    {
        public IEnumerable<string> Articles { get; }

        private Regex _regex;

        /// <summary>
        /// The minimum string length of numbers. If 4, XXIII or 23 will turn into 0023.
        /// </summary>
        private static int NumberLength = 2;

        public SortableNameConverter(IEnumerable<string> articles, bool batchOperation = false)
        {
            Articles = articles ?? throw new ArgumentNullException(nameof(articles));
            string articlesPattern = string.Join("|", articles.Select(Regex.Escape));
            var options = RegexOptions.ExplicitCapture;
            if (batchOperation)
                options |= RegexOptions.Compiled;

            //(?!^) prevents the numerical matches from happening at the start of the string (for example for X-COM or XIII)
            //using [0-9] here instead of \d because \d also matches ٠١٢٣٤٥٦٧٨٩ and I don't know what to do with those           
            //the (?i) is a modifier that makes the rest of the regex (to the right of it) case insensitive
            //see https://www.regular-expressions.info/modifiers.html
            _regex = new Regex($@"(?!^)\b((?<roman>[MDCLXVI\u2160-\u217F]+)|(?<arabic>[0-9]+))\b|(?i)^(?<article>{articlesPattern})\s+", options);
        }

        public string Convert(string input)
        {
            return _regex.Replace(input, MatchEvaluator);
        }

        private string MatchEvaluator(Match match)
        {
            Group matchedGroup = match.Groups.Cast<Group>().Skip(1).SingleOrDefault(g => g.Success);
            if (match.Groups["roman"].Success)
            {
                return ConvertRomanNumeralToInt(match.Value).ToString(new string('0', NumberLength));
            }
            else if (match.Groups["arabic"].Success)
            {
                return match.Value.PadLeft(NumberLength, '0');
            }
            else if (match.Groups["article"].Success)
            {
                return string.Empty;
            }
            return match.Value;
        }

        //There's unicode forms of roman numerals but I haven't seen them in the wild in use in game titles, so I'm ignoring them for now
        private static Dictionary<char, int> RomanNumeralValues = new Dictionary<char, int>
        {
            { 'I', 1 }, { 'V', 5 }, { 'X', 10 }, { 'L', 50 }, { 'C', 100 }, { 'D', 500 }, { 'M', 1000 },
            //unicode uppercase
            {'Ⅰ', 1}, {'Ⅱ', 2}, {'Ⅲ', 3}, {'Ⅳ', 4}, {'Ⅴ', 5}, {'Ⅵ', 6}, {'Ⅶ', 7}, {'Ⅷ', 8}, {'Ⅸ', 9}, {'Ⅹ', 10}, {'Ⅺ', 11}, {'Ⅻ', 12}, {'Ⅼ', 50}, {'Ⅽ', 100}, {'Ⅾ', 500}, {'Ⅿ', 1000},
            //unicode lowercase
            {'ⅰ', 1}, {'ⅱ', 2}, {'ⅲ', 3}, {'ⅳ', 4}, {'ⅴ', 5}, {'ⅵ', 6}, {'ⅶ', 7}, {'ⅷ', 8}, {'ⅸ', 9}, {'ⅹ', 10}, {'ⅺ', 11}, {'ⅻ', 12}, {'ⅼ', 50}, {'ⅽ', 100}, {'ⅾ', 500}, {'ⅿ', 1000},
        };
        //TODO: figure out if a number IS a roman numeral or if roman numerals are its components
        public static int ConvertRomanNumeralToInt(string input)
        {
            char? prevChar = null;
            var numericalValues = new Stack<int>();
            foreach (char c in input)
            {
                int value = RomanNumeralValues[c];

                //group by character, eg III=3, XX=20
                if (prevChar == c)
                {
                    value += numericalValues.Pop();
                }

                numericalValues.Push(value);

                prevChar = c;
            }

            int output = 0;
            int? numberToTheRight = null;
            //Since this is a stack, this will go through grouped number values of the roman numerals right to left
            foreach (int v in numericalValues)
            {
                if (v < numberToTheRight)
                {
                    output -= v;
                }
                else if (v > numberToTheRight || numberToTheRight == null)
                {
                    output += v;
                }
                numberToTheRight = v;
            }
            return output;
        }
    }
}
