﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMethodArgumentsKeyGenerator.cs" company="The original author or authors.">
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

namespace Spring.Retry.Retry.Interceptor
{
    /// <summary>
    /// Interface that allows method parameters to be identified and tagged by a
    /// unique key.
    /// </summary>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public interface IMethodArgumentsKeyGenerator
    {
        /// <summary>
        /// Get a unique identifier for the item that can be used to cache it between
        /// calls if necessary, and then identify it later.
        /// </summary>
        /// <param name="item">The current item.</param>
        /// <returns>A unique identifier.</returns>
        object GetKey(object[] item);
    }
}
