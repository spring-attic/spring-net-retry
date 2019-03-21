// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRetryState.cs" company="The original author or authors.">
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
#endregion

namespace Spring.Retry.Retry
{
    /// <summary>
    /// Stateful retry is characterized by having to recognize the items that are
    /// being processed, so this interface is used primarily to provide a cache key in
    /// between failed attempts. It also provides a hints to the <see cref="IRetryOperations{T}"/>
    /// for optimizations to do with avoidable cache hits and switching to stateless retry if a rollback is not needed.
    /// </summary>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public interface IRetryState
    {
        /// <summary>
        /// Key representing the state for a retry attempt. Stateful retry is
        /// characterised by having to recognise the items that are being processed,
        /// so this value is used as a cache key in between failed attempts.
        /// </summary>
        /// <returns>The key that this state represents.</returns>
        object GetKey();

        /// <summary>
        /// Indicate whether a cache lookup can be avoided. If the key is known ahead
        /// of the retry attempt to be fresh (i.e. has never been seen before) then a
        /// cache lookup can be avoided if this flag is true.
        /// </summary>
        /// <returns>True if the state does not require an explicit check for the key.</returns>
        bool IsForceRefresh();

        /// <summary>Check whether this exception requires a rollback. The default is always
        /// true, which is conservative, so this method provides an optimisation for
        /// switching to stateless retry if there is an exception for which rollback
        /// is unnecessary. Example usage would be for a stateful retry to specify a
        /// validation exception as not for rollback.</summary>
        /// <param name="exception">The exception that caused a retry attempt to fail.</param>
        /// <returns>True if this exception should cause a rollback.</returns>
        bool RollbackFor(Exception exception);
    }
}
