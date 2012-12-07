// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FixedBackOffPolicy.cs" company="The original author or authors.">
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
using System.Threading;
#endregion

namespace Spring.Retry.Retry.Backoff
{
    /// <summary>
    /// Implementation of <see cref="IBackOffPolicy"/> that pauses for a fixed period of
    /// time before continuing. A pause is implemented using <see cref="Thread.Sleep(int)"/>.
    /// <see cref="BackOffPeriod"/> is thread-safe and it is safe to set
    /// <see cref="BackOffPeriod"/> during execution from multiple threads, however
    /// this may cause a single retry operation to have pauses of different
    /// intervals.
    /// </summary>
    public class FixedBackOffPolicy : StatelessBackOffPolicy, ISleepingBackOffPolicy
    {
        // Default back off period - 1000ms.
        private static readonly long DEFAULT_BACK_OFF_PERIOD = 1000L;

        // The back off period in milliseconds. Defaults to 1000ms.
        private long backOffPeriod = DEFAULT_BACK_OFF_PERIOD;

        private ISleeper sleeper = new ObjectWaitSleeper();

        /// <summary>The with sleeper.</summary>
        /// <param name="sleeper">The sleeper.</param>
        /// <returns>The Spring.Retry.Retry.Backoff.FixedBackOffPolicy.</returns>
        public ISleepingBackOffPolicy WithSleeper(ISleeper sleeper)
        {
            var res = new FixedBackOffPolicy();
            res.BackOffPeriod = this.BackOffPeriod;
            res.Sleeper = sleeper;
            return res;
        }

        /// <summary>Sets the sleeper.</summary>
        public ISleeper Sleeper { set { this.sleeper = value; } }

        /// <summary>Gets or sets the back off period.</summary>
        public long BackOffPeriod { get { return this.backOffPeriod; } set { this.backOffPeriod = value > 0 ? value : 1; } }

        /// <summary>Pause for the <see cref="BackOffPeriod"/>.</summary>
        protected override void DoBackOff()
        {
            try
            {
                this.sleeper.Sleep(this.backOffPeriod);
            }
            catch (ThreadInterruptedException e)
            {
                throw new BackOffInterruptedException("Thread interrupted while sleeping", e);
            }
        }

        /// <summary>The to string.</summary>
        /// <returns>The System.String.</returns>
        public override string ToString() { return "FixedBackOffPolicy[backOffPeriod=" + this.backOffPeriod + "]"; }
    }
}
