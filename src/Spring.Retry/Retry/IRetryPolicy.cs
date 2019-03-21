// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRetryPolicy.cs" company="The original author or authors.">
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
    /// A <see cref="IRetryPolicy"/> is responsible for allocating and managing resources
    /// needed by <see cref="IRetryOperations{T}"/>. The <see cref="IRetryPolicy"/> allows retry
    /// operations to be aware of their context. Context can be internal to the retry
    /// framework, e.g. to support nested retries. Context can also be external, and
    /// the <see cref="IRetryPolicy"/> provides a uniform API for a range of different
    /// platforms for the external context.
    /// </summary>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public interface IRetryPolicy
    {
        /// <summary>Indicates whether the operation can proceed.</summary>
        /// <param name="context">The current retry status.</param>
        /// <returns>True if the operation can proceed.</returns>
        bool CanRetry(IRetryContext context);

        /// <summary>Acquire resources needed for the retry operation. The callback is passed
        /// in so that marker interfaces can be used and a manager can collaborate
        /// with the callback to set up some state in the status token.</summary>
        /// <param name="parent">The parent context if we are in a nested retry.</param>
        /// <returns>A <see cref="IRetryContext"/> object specific to this manager.</returns>
        IRetryContext Open(IRetryContext parent);

        /// <summary>Close the retry operation.</summary>
        /// <param name="context">A retry status created by the <see cref="IRetryContext"/> method of this manager.</param>
        void Close(IRetryContext context);

        /// <summary>Called once per retry attempt, after the callback fails.</summary>
        /// <param name="context">The current status object.</param>
        /// <param name="exception">The exception.</param>
        void RegisterException(IRetryContext context, Exception exception);
    }
}
