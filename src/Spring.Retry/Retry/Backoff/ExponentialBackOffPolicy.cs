// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExponentialBackOffPolicy.cs" company="The original author or authors.">
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
using System.Runtime.CompilerServices;
using System.Threading;
using Common.Logging;
#endregion

namespace Spring.Retry.Retry.Backoff
{
    /// <summary>
    /// Implementation of <see cref="IBackOffPolicy"/> that increases the back off period
    /// for each retry attempt in a given set using the {@link Math#exp(double)
    /// exponential} function.
    /// This implementation is thread-safe and suitable for concurrent access.
    /// Modifications to the configuration do not affect any retry sets that are
    /// already in progress.
    /// The <see cref="InitialInterval"/> property controls the initial value
    /// passed to {@link Math#exp(double)} and the <see cref="Multiplier"/>
    /// property controls by how much this value is increased for each subsequent
    /// attempt.
    /// </summary>
    /// <author>Rob Harrop</author>
    /// <author>Dave Syer</author>
    /// <author>Gary Russell</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public class ExponentialBackOffPolicy
    {
        protected static readonly ILog Logger = LogManager.GetCurrentClassLogger();

        // The default 'initialInterval' value - 100 millisecs. Coupled with the
        // default 'multiplier' value this gives a useful initial spread of pauses
        // for 1-5 retries.
        public static readonly long DEFAULT_INITIAL_INTERVAL = 100L;

        // The default maximum backoff time (30 seconds).
        public static readonly long DEFAULT_MAX_INTERVAL = 30000L;

        // The default 'multiplier' value - value 2 (100% increase per backoff).
        public static readonly double DEFAULT_MULTIPLIER = 2;

        // The initial sleep interval.
        private long initialInterval = DEFAULT_INITIAL_INTERVAL;

        // The maximum value of the backoff period in milliseconds.
        private long maxInterval = DEFAULT_MAX_INTERVAL;

        // The value to increment the exp seed with for each retry attempt.
        private double multiplier = DEFAULT_MULTIPLIER;

        private ISleeper sleeper = new ObjectWaitSleeper();

        /// <summary>Sets the sleeper.</summary>
        public ISleeper Sleeper { set { this.sleeper = value; } }

        /// <summary>Gets or sets the initial interval.</summary>
        public long InitialInterval { get { return this.initialInterval; } set { this.initialInterval = value > 1 ? value : 1; } }

        /// <summary>Gets or sets the multiplier.</summary>
        public double Multiplier { get { return this.multiplier; } set { this.multiplier = value > 1.0 ? value : 1.0; } }

        /// <summary>Gets or sets the max interval.</summary>
        public long MaxInterval { get { return this.maxInterval; } set { this.maxInterval = value > 0 ? value : 1; } }

        /// <summary>The with sleeper.</summary>
        /// <param name="sleeper">The sleeper.</param>
        /// <returns>The Spring.Retry.Retry.Backoff.ExponentialBackOffPolicy.</returns>
        public ExponentialBackOffPolicy WithSleeper(ISleeper sleeper)
        {
            var res = this.NewInstance();
            this.CloneValues(res);
            res.Sleeper = sleeper;
            return res;
        }

        /// <summary>The new instance.</summary>
        /// <returns>The Spring.Retry.Retry.Backoff.ExponentialBackOffPolicy.</returns>
        protected virtual ExponentialBackOffPolicy NewInstance() { return new ExponentialBackOffPolicy(); }

        /// <summary>The clone values.</summary>
        /// <param name="target">The target.</param>
        protected void CloneValues(ExponentialBackOffPolicy target)
        {
            target.InitialInterval = this.InitialInterval;
            target.MaxInterval = this.MaxInterval;
            target.Multiplier = this.Multiplier;
            target.Sleeper = this.sleeper;
        }

        /// <summary>Returns a new instance of <see cref="IBackOffContext"/> configured with the 'expSeed' and 'increment' values.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The Spring.Retry.Retry.Backoff.IBackOffContext.</returns>
        public virtual IBackOffContext Start(IRetryContext context) { return new ExponentialBackOffContext(this.initialInterval, this.multiplier, this.maxInterval); }
        
        /// <summary>Pause for a length of time equal to 'exp(backOffContext.expSeed)'</summary>
        /// <param name="backOffContext">The back off context.</param>
        public void BackOff(IBackOffContext backOffContext)
        {
            var context = (ExponentialBackOffContext)backOffContext;
            try
            {
                long sleepTime = context.GetSleepAndIncrement();
                Logger.Debug(m => m("Sleeping for {0}", sleepTime));
                this.sleeper.Sleep(sleepTime);
            }
            catch (ThreadInterruptedException e)
            {
                throw new BackOffInterruptedException("Thread interrupted while sleeping", e);
            }
        }

        /// <summary>The to string.</summary>
        /// <returns>The System.String.</returns>
        public override string ToString() { return string.Format("[initialInterval={0}, multiplier={1}, maxInterval={2}]", this.GetType().Name, this.initialInterval, this.multiplier, this.maxInterval); }
    }

    internal class ExponentialBackOffContext : IBackOffContext
    {
        private readonly double multiplier;

        private long interval;

        private readonly long maxInterval;

        /// <summary>Initializes a new instance of the <see cref="ExponentialBackOffContext"/> class.</summary>
        /// <param name="expSeed">The exp seed.</param>
        /// <param name="multiplier">The multiplier.</param>
        /// <param name="maxInterval">The max interval.</param>
        public ExponentialBackOffContext(long expSeed, double multiplier, long maxInterval)
        {
            this.interval = expSeed;
            this.multiplier = multiplier;
            this.maxInterval = maxInterval;
        }

        /// <summary>Gets the multiplier.</summary>
        public double Multiplier { get { return this.multiplier; } }

        /// <summary>Gets the interval.</summary>
        public long Interval { get { return this.interval; } }

        /// <summary>Gets the max interval.</summary>
        public long MaxInterval { get { return this.maxInterval; } }

        /// <summary>The get sleep and increment.</summary>
        /// <returns>The System.Int64.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public long GetSleepAndIncrement()
        {
            var sleep = this.Interval;
            if (sleep > this.MaxInterval)
            {
                sleep = this.MaxInterval;
            }
            else
            {
                this.interval = this.GetNextInterval();
            }

            return sleep;
        }

        /// <summary>The get next interval.</summary>
        /// <returns>The System.Int64.</returns>
        protected virtual long GetNextInterval() { return (long)(this.Interval * this.Multiplier); }
    }
}
