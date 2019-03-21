// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RetryContext.cs" company="The original author or authors.">
//   Copyright 2002-2012 the original author or authors.
//   
//   Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with
//   the License. You may obtain a copy of the License at
//   
//   https://www.apache.org/licenses/LICENSE-2.0
//   
//   Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on
//   an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the
//   specific language governing permissions and limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#region Using Directives
using System;
using Spring.Core;
#endregion

namespace Spring.Retry.Retry
{
    /// <summary>
    /// Retry Context Interface.
    /// </summary>
    public interface IRetryContext : IAttributeAccessor
    {
        /// <summary>
        /// Gets or sets a value indicating whether exhausted only.
        /// Signal to the framework that no more attempts should be made to try or retry the current <see cref="IRetryCallback{T}"/>.
        /// </summary>
        /// <author>Dave Syer</author>
        /// <author>Joe Fitzgerald (.NET)</author>
        bool ExhaustedOnly { get; set; }
        
        /// <summary>Accessor for the parent context if retry blocks are nested.</summary>
        /// <returns>The parent Spring.Retry.Retry.IRetryContext or null if there is none.</returns>
        IRetryContext GetParent();

        /// <summary>
        /// Gets the retry count.
        /// Counts the number of retry attempts. Before the first attempt this
        /// counter is zero, and before the first and subsequent attempts it should
        /// increment accordingly.
        /// </summary>
        int RetryCount { get; }
        
        /// <summary>
        /// Gets the last Exception. Accessor for the exception object that caused the current retry.
        /// </summary>
        Exception LastException { get; }
    }
}
