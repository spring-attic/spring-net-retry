// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapRetryContextCache.cs" company="The original author or authors.">
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
using System.Collections.Concurrent;
using System.Collections.Generic;
#endregion

namespace Spring.Retry.Retry.Policy
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MapRetryContextCache : IRetryContextCache
    {
        /**
	 * Default value for maximum capacity of the cache. This is set to a
	 * reasonably low value (4096) to avoid users inadvertently filling the
	 * cache with item keys that are inconsistent.
	 */
        public static readonly int DEFAULT_CAPACITY = 4096;

        private readonly IDictionary<object, IRetryContext> map = new ConcurrentDictionary<object, IRetryContext>();

        private int capacity;

        /**
	 * Create a {@link MapRetryContextCache} with default capacity.
	 */

        /// <summary>Initializes a new instance of the <see cref="MapRetryContextCache"/> class.</summary>
        public MapRetryContextCache() : this(DEFAULT_CAPACITY) { }

        /**
	 * @param defaultCapacity
	 */

        /// <summary>Initializes a new instance of the <see cref="MapRetryContextCache"/> class.</summary>
        /// <param name="defaultCapacity">The default capacity.</param>
        public MapRetryContextCache(int defaultCapacity) { this.Capacity = defaultCapacity; }

        /// <summary>Sets the capacity.</summary>
        public int Capacity { set { this.capacity = value; } }

        /// <summary>The contains key.</summary>
        /// <param name="key">The key.</param>
        /// <returns>The System.Boolean.</returns>
        public bool ContainsKey(object key) { return this.map.ContainsKey(key); }

        /// <summary>The get.</summary>
        /// <param name="key">The key.</param>
        /// <returns>The Spring.Retry.Retry.IRetryContext.</returns>
        public IRetryContext Get(object key)
        {
            if (this.map.ContainsKey(key))
            {
                return this.map[key];
            }
            else
            {
                return null;
            }
        }

        /// <summary>The put.</summary>
        /// <param name="key">The key.</param>
        /// <param name="context">The context.</param>
        /// <exception cref="RetryCacheCapacityExceededException"></exception>
        public void Put(object key, IRetryContext context)
        {
            if (this.map.Count >= this.capacity)
            {
                throw new RetryCacheCapacityExceededException(
                    "Retry cache capacity limit breached. "
                    + "Do you need to re-consider the implementation of the key generator, "
                    + "or the equals and hashCode of the items that failed?");
            }

            if (this.map.ContainsKey(key))
            {
                this.map[key] = context;
            }
            else
            {
                this.map.Add(key, context);
            }
        }

        /// <summary>The remove.</summary>
        /// <param name="key">The key.</param>
        public void Remove(object key) { this.map.Remove(key); }
    }
}
