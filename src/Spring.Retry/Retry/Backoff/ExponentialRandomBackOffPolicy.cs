// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExponentialRandomBackOffPolicy.cs" company="The original author or authors.">
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
using System.Runtime.CompilerServices;
#endregion

namespace Spring.Retry.Retry.Backoff
{
    /// <summary>
    /// Implementation of {@link org.springframework.retry.backoff.ExponentialBackOffPolicy} that
    /// chooses a random multiple of the interval.  The random multiple is selected based on
    /// how many iterations have occurred.
    /// This has shown to at least be useful in testing scenarios where excessive contention is generated
    /// by the test needing many retries.  In test, usually threads are started at the same time, and thus
    /// stomp together onto the next interval.  Using this {@link BackOffPolicy} can help avoid that scenario.
    /// Example:
    ///   initialInterval = 50
    ///   multiplier      = 2.0
    ///   maxInterval     = 3000
    ///   numRetries      = 5
    /// <see cref="ExponentialBackOffPolicy"/>         yields:   [50, 100, 200, 400, 800]
    /// <see cref="ExponentialRandomBackOffPolicy"/> may yield   [50, 100, 100, 100, 600]
    ///                                                     or   [50, 100, 150, 400, 800]
    /// </summary>
    /// <author>Jon Travis</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public class ExponentialRandomBackOffPolicy : ExponentialBackOffPolicy
    {
        /// <summary>Returns a new instance of <see cref="IBackOffContext"/>, seeded with this policy's settings.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The Spring.Retry.Retry.Backoff.IBackOffContext.</returns>
        public override IBackOffContext Start(IRetryContext context) { return new ExponentialRandomBackOffContext(this.InitialInterval, this.Multiplier, this.MaxInterval); }

        /// <summary>The new instance.</summary>
        /// <returns>The Spring.Retry.Retry.Backoff.ExponentialBackOffPolicy.</returns>
        protected override ExponentialBackOffPolicy NewInstance() { return new ExponentialRandomBackOffPolicy(); }
    }

    internal class ExponentialRandomBackOffContext : ExponentialBackOffContext
    {
        private readonly Random r = new Random();
        private readonly long initialInterval;
        private long intervalIdx;

        /// <summary>Initializes a new instance of the <see cref="ExponentialRandomBackOffContext"/> class.</summary>
        /// <param name="expSeed">The exp seed.</param>
        /// <param name="multiplier">The multiplier.</param>
        /// <param name="maxInterval">The max interval.</param>
        public ExponentialRandomBackOffContext(long expSeed, double multiplier, long maxInterval)
            : base(expSeed, multiplier, maxInterval)
        {
            this.initialInterval = expSeed;
            this.intervalIdx = 0;
        }

        /// <summary>The get next interval.</summary>
        /// <returns>The System.Int64.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override long GetNextInterval()
        {
            this.intervalIdx++;
            return this.initialInterval + (this.initialInterval * Math.Max(1, this.r.Next((int)Math.Pow(this.Multiplier, this.intervalIdx))));
        }
    }
}
