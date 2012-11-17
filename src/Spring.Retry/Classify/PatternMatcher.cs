// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatternMatcher.cs" company="The original author or authors.">
//   Copyright 2002-2012 the original author or authors.
//   
//   Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with
//   the License. You may obtain a copy of the License at
//   
//   http://www.apache.org/licenses/LICENSE-2.0
//   
//   Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on
//   an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the
//   specific language governing permissions and limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#region Using Directives
using System;
using System.Collections.Generic;
using Spring.Util;
#endregion

namespace Spring.Retry.Classify
{
    /// <summary>Pattern Matcher.</summary>
    /// <typeparam name="T">Type T.</typeparam>
    /// <author>Dave Syer</author><author>Dan Garrette</author><author>Joe Fitzgerald (.NET)</author>
    public class PatternMatcher<T>
    {
        private IDictionary<string, T> map = new Dictionary<string, T>();
        private readonly List<string> sorted = new List<string>();

        /// <summary>Initializes a new instance of the <see cref="PatternMatcher{T}"/> class.</summary>
        /// <param name="map">The map.</param>
        public PatternMatcher(IDictionary<string, T> map)
        {
            this.map = map;

            // Sort keys to start with the most specific
            this.sorted = new List<string>(map.Keys);
            this.sorted.Sort((s1, s2) => string.Compare(s1, s2, System.StringComparison.Ordinal));
        }

        public static bool Match(string pattern, string str)
        {
            var patArr = pattern.ToCharArray();
            var strArr = str.ToCharArray();
            var patIdxStart = 0;
            var patIdxEnd = patArr.Length - 1;
            var strIdxStart = 0;
            var strIdxEnd = strArr.Length - 1;
            char ch;

            var containsStar = pattern.Contains("*");

            if (!containsStar)
            {
                // No '*'s, so we make a shortcut
                if (patIdxEnd != strIdxEnd)
                {
                    return false; // Pattern and string do not have the same size
                }

                for (var i = 0; i <= patIdxEnd; i++)
                {
                    ch = patArr[i];
                    if (ch != '?')
                    {
                        if (ch != strArr[i])
                        {
                            return false; // Character mismatch
                        }
                    }
                }

                return true; // String matches against pattern
            }

            if (patIdxEnd == 0)
            {
                return true; // Pattern contains only '*', which matches anything
            }

            // Process characters before first star
            while ((ch = patArr[patIdxStart]) != '*' && strIdxStart <= strIdxEnd)
            {
                if (ch != '?')
                {
                    if (ch != strArr[strIdxStart])
                    {
                        return false; // Character mismatch
                    }
                }

                patIdxStart++;
                strIdxStart++;
            }

            if (strIdxStart > strIdxEnd)
            {
                // All characters in the string are used. Check if only '*'s are
                // left in the pattern. If so, we succeeded. Otherwise failure.
                for (var i = patIdxStart; i <= patIdxEnd; i++)
                {
                    if (patArr[i] != '*')
                    {
                        return false;
                    }
                }

                return true;
            }

            // Process characters after last star
            while ((ch = patArr[patIdxEnd]) != '*' && strIdxStart <= strIdxEnd)
            {
                if (ch != '?')
                {
                    if (ch != strArr[strIdxEnd])
                    {
                        return false; // Character mismatch
                    }
                }

                patIdxEnd--;
                strIdxEnd--;
            }

            if (strIdxStart > strIdxEnd)
            {
                // All characters in the string are used. Check if only '*'s are
                // left in the pattern. If so, we succeeded. Otherwise failure.
                for (var i = patIdxStart; i <= patIdxEnd; i++)
                {
                    if (patArr[i] != '*')
                    {
                        return false;
                    }
                }

                return true;
            }

            // process pattern between stars. padIdxStart and patIdxEnd point
            // always to a '*'.
            while (patIdxStart != patIdxEnd && strIdxStart <= strIdxEnd)
            {
                var patIdxTmp = -1;
                for (var i = patIdxStart + 1; i <= patIdxEnd; i++)
                {
                    if (patArr[i] == '*')
                    {
                        patIdxTmp = i;
                        break;
                    }
                }

                if (patIdxTmp == patIdxStart + 1)
                {
                    // Two stars next to each other, skip the first one.
                    patIdxStart++;
                    continue;
                }

                // Find the pattern between padIdxStart & padIdxTmp in str between
                // strIdxStart & strIdxEnd
                var patLength = (patIdxTmp - patIdxStart - 1);
                var strLength = (strIdxEnd - strIdxStart + 1);
                var foundIdx = -1;
                for (var i = 0; i <= strLength - patLength; i++)
                {
                    for (var j = 0; j < patLength; j++)
                    {
                        ch = patArr[patIdxStart + j + 1];
                        if (ch != '?')
                        {
                            if (ch != strArr[strIdxStart + i + j])
                            {
                                break;
                            }
                        }
                    }

                    foundIdx = strIdxStart + i;
                    break;
                }

                if (foundIdx == -1)
                {
                    return false;
                }

                patIdxStart = patIdxTmp;
                strIdxStart = foundIdx + patLength;
            }

            // All characters in the string are used. Check if only '*'s are left
            // in the pattern. If so, we succeeded. Otherwise failure.
            for (var i = patIdxStart; i <= patIdxEnd; i++)
            {
                if (patArr[i] != '*')
                {
                    return false;
                }
            }

            return true;
        }

        public T Match(string line)
        {
            var value = default(T);
            AssertUtils.ArgumentNotNull(line, "A non-null key must be provided to match against.");

            foreach (var key in sorted)
            {
                if (Match(key, line))
                {
                    value = map[key];
                    break;
                }
            }

            if (value == null)
            {
                throw new InvalidOperationException("Could not find a matching pattern for key=[" + line + "]");
            }

            return value;

        }
    }
}
