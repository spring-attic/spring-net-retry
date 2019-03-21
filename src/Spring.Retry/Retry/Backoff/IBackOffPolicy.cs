// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBackOffPolicy.cs" company="The original author or authors.">
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

namespace Spring.Retry.Retry.Backoff
{
    /// <summary>
    /// Strategy interface to control back off between attempts in a single
    /// <see cref="RetryTemplate"/> retry operation.
    /// Implementations are expected to be thread-safe and should be designed
    /// for concurrent access. Configuration for each implementation is also expected
    /// to be thread-safe but need not be suitable for high load concurrent access.
    /// For each block of retry operations the <see cref="Start"/> method is called
    /// and implementations can return an implementation-specific
    /// <see cref="IBackOffContext"/> that can be used to track state through subsequent
    /// back off invocations. Each back off process is handled via a call to
    /// <see cref="BackOff"/>. The
    /// <see cref="RetryTemplate"/> will pass in
    /// the corresponding <see cref="IBackOffContext"/> object created by the call to
    /// <see cref="Start"/>.
    /// </summary>
    /// <author>Rob Harrop</author>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public interface IBackOffPolicy
    {
        /// <summary>Start a new block of back off operations. Implementations can choose to
        /// pause when this method is called, but normally it returns immediately.</summary>
        /// <param name="context">The current retry context, which might contain information that we can use to decide how to proceed.</param>
        /// <returns>The implementation-specific <see cref="IBackOffContext"/> or null.</returns>
        IBackOffContext Start(IRetryContext context);

        /// <summary>Back off/pause in an implementation-specific fashion. The passed in<see cref="IBackOffContext"/> corresponds to the one created by the call to<see cref="Start"/> for a given retry operation set.</summary>
        /// <param name="backOffContext">The back off context.</param>
        void BackOff(IBackOffContext backOffContext);
    }
}
