// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRetryCallback.cs" company="The original author or authors.">
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

namespace Spring.Retry.Retry
{
    /// <summary>Callback interface for an operation that can be retried using a <see cref="IRetryOperations"/></summary>
    /// <typeparam name="T">Type T.</typeparam>
    /// <author>Rob Harrop</author>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public interface IRetryCallback<T>
    {
        /// <summary>
        /// Execute an operation with retry semantics. Operations should generally be
        /// idempotent, but implementations may choose to implement compensation
        /// semantics when an operation is retried.
        /// </summary>
        /// <param name="context">The current retry context.</param>
        /// <returns>The result of the successful operation.</returns>
        T DoWithRetry(IRetryContext context);
    }
}
