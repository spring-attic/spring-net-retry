// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRetryContextCache.cs" company="The original author or authors.">
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

namespace Spring.Retry.Retry.Policy
{
    /// <summary>
    /// Simple map-like abstraction for stateful retry policies to use when storing
    /// and retrieving <see cref="IRetryContext"/> instances.
    /// </summary>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public interface IRetryContextCache
    {
        /// <summary>The get.</summary>
        /// <param name="key">The key.</param>
        /// <returns>The Spring.Retry.Retry.IRetryContext.</returns>
        IRetryContext Get(object key);

        /// <summary>The put.</summary>
        /// <param name="key">The key.</param>
        /// <param name="context">The context.</param>
        void Put(object key, IRetryContext context);

        /// <summary>The remove.</summary>
        /// <param name="key">The key.</param>
        void Remove(object key);

        /// <summary>The contains key.</summary>
        /// <param name="key">The key.</param>
        /// <returns>The System.Boolean.</returns>
        bool ContainsKey(object key);
    }
}
