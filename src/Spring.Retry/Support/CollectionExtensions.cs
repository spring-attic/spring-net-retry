// -----------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Common.Logging;

namespace Spring.Retry.Support
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Collection Extensions
    /// </summary>
    /// <author>Joe Fitzgerald (.NET)</author>
    public static class CollectionExtensions
    {
        private static readonly ILog Logger = LogManager.GetCurrentClassLogger();

        public static T2 Get<T1, T2>(this IDictionary<T1, T2> dictionary, T1 key)
        {
            try
            {
                T2 result;
                dictionary.TryGetValue(key, out result);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Debug(ex);
            }

            return default(T2);
        }
    }
}
