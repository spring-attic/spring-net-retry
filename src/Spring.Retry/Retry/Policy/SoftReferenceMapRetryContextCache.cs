// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SoftReferenceMapRetryContextCache.cs" company="The original author or authors.">
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

// NOTE: Excluded from the solution as .NET does not have SoftReferences.

#region Using Directives
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Spring.Retry.Support;
#endregion

namespace Spring.Retry.Retry.Policy
{
    /// <summary>
    /// Map-based implementation of <see cref="IRetryContextCache"/>. The map backing the
    /// cache of contexts is synchronized and its entries are soft-referenced, so may
    /// be garbage collected under pressure.
    /// <see cref="MapRetryContextCache"/> for non-soft referenced version.
    /// </summary>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public class SoftReferenceMapRetryContextCache : IRetryContextCache
    {
        /// <summary>
        /// Default value for maximum capacity of the cache. This is set to a
        /// reasonably low value (4096) to avoid users inadvertently filling the
        /// cache with item keys that are inconsistent.
        /// </summary>
        public static readonly int DEFAULT_CAPACITY = 4096;

        private IDictionary<object, SoftReference<IRetryContext>> map = new ConcurrentDictionary<object, SoftReference<IRetryContext>>();

        private int capacity;

        /// <summary>Initializes a new instance of the <see cref="SoftReferenceMapRetryContextCache"/> class with default capacity.</summary>
        public SoftReferenceMapRetryContextCache() : this(DEFAULT_CAPACITY) { }

        /// <summary>Initializes a new instance of the <see cref="SoftReferenceMapRetryContextCache"/> class.</summary>
        /// <param name="defaultCapacity">The default capacity.</param>
        public SoftReferenceMapRetryContextCache(int defaultCapacity) { this.Capacity = defaultCapacity; }

        /// <summary>
        /// Sets the capacity.
        /// Public setter for the capacity. Prevents the cache from growing
        /// unboundedly if items that fail are misidentified and two references to an
        /// identical item actually do not have the same key. This can happen when
        /// users implement equals and hashCode based on mutable fields, for
        /// instance.
        /// </summary>
        public int Capacity { set { this.capacity = value; } }

        /// <summary>The contains key.</summary>
        /// <param name="key">The key.</param>
        /// <returns>The System.Boolean.</returns>
        public bool ContainsKey(object key)
        {
            if (!map.ContainsKey(key))
            {
                return false;
            }

            if (map.Get(key).get() == null)
            {
                // our reference was garbage collected
                map.Remove(key);
            }

            return map.ContainsKey(key);
        }

        /// <summary>The get.</summary>
        /// <param name="key">The key.</param>
        /// <returns>The Spring.Retry.Retry.IRetryContext.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public IRetryContext Get(object key) { throw new NotImplementedException(); }

        /// <summary>The put.</summary>
        /// <param name="key">The key.</param>
        /// <param name="context">The context.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Put(object key, IRetryContext context) { throw new NotImplementedException(); }

        /// <summary>The remove.</summary>
        /// <param name="key">The key.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Remove(object key) { throw new NotImplementedException(); }

    }
}
