// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeoutRetryPolicy.cs" company="The original author or authors.">
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
using System;
using Spring.Retry.Retry.Context;
#endregion

namespace Spring.Retry.Retry.Policy
{
    /// <summary>
    /// A <see cref="IRetryPolicy"/> that allows a retry only if it hasn't timed out. The
    /// clock is started on a call to <see cref="Open"/>.
    /// </summary>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald</author>
    public class TimeoutRetryPolicy : IRetryPolicy
    {
        // Default value for timeout (milliseconds).
        public static readonly long DEFAULT_TIMEOUT = 1000;

        private long timeout = DEFAULT_TIMEOUT;

        /// <summary>Gets or sets the timeout.</summary>
        public long Timeout { get { return this.timeout; } set { this.timeout = value; } }

        /// <summary>Only permits a retry if the timeout has not expired. Does not check the exception at all.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The System.Boolean.</returns>
        public bool CanRetry(IRetryContext context) { return ((TimeoutRetryContext)context).IsAlive(); }

        /// <summary>The close.</summary>
        /// <param name="context">The context.</param>
        public void Close(IRetryContext context) { }

        /// <summary>The open.</summary>
        /// <param name="parent">The parent.</param>
        /// <returns>The Spring.Retry.Retry.IRetryContext.</returns>
        public IRetryContext Open(IRetryContext parent) { return new TimeoutRetryContext(parent, this.timeout); }

        /// <summary>The register exception.</summary>
        /// <param name="context">The context.</param>
        /// <param name="exception">The exception.</param>
        public void RegisterException(IRetryContext context, Exception exception)
        {
            ((RetryContextSupport)context).RegisterException(exception);

            // otherwise no-op - we only time out, otherwise retry everything...
        }
    }

    internal class TimeoutRetryContext : RetryContextSupport
    {
        private readonly long timeout;

        private readonly DateTime start;

        /// <summary>Initializes a new instance of the <see cref="TimeoutRetryContext"/> class.</summary>
        /// <param name="parent">The parent.</param>
        /// <param name="timeout">The timeout.</param>
        public TimeoutRetryContext(IRetryContext parent, long timeout)
            : base(parent)
        {
            this.start = DateTime.UtcNow;
            this.timeout = timeout;
        }

        /// <summary>The is alive.</summary>
        /// <returns>The System.Boolean.</returns>
        public bool IsAlive() { return DateTime.UtcNow.Subtract(this.start).Milliseconds <= this.timeout; }
    }
}
