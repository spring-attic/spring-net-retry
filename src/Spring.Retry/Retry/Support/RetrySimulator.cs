// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RetrySimulator.cs" company="The original author or authors.">
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
using System.Collections.Generic;
using Spring.Retry.Retry.Backoff;
#endregion

namespace Spring.Retry.Retry.Support
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class RetrySimulator
    {
        private readonly ISleepingBackOffPolicy backOffPolicy;
        private readonly IRetryPolicy retryPolicy;

        /// <summary>Initializes a new instance of the <see cref="RetrySimulator"/> class.</summary>
        /// <param name="backOffPolicy">The back off policy.</param>
        /// <param name="retryPolicy">The retry policy.</param>
        public RetrySimulator(ISleepingBackOffPolicy backOffPolicy, IRetryPolicy retryPolicy)
        {
            this.backOffPolicy = backOffPolicy;
            this.retryPolicy = retryPolicy;
        }

        /**
     * Execute the simulator for a give # of iterations.
     *
     * @param numSimulations  Number of simulations to run
     * @return the outcome of all simulations
     */

        /// <summary>The execute simulation.</summary>
        /// <param name="numSimulations">The num simulations.</param>
        /// <returns>The Spring.Retry.Retry.Support.RetrySimulation.</returns>
        public RetrySimulation ExecuteSimulation(int numSimulations)
        {
            var simulation = new RetrySimulation();

            for (int i = 0; i < numSimulations; i++)
            {
                simulation.AddSequence(this.ExecuteSingleSimulation());
            }

            return simulation;
        }

        /**
     * Execute a single simulation
     * @return The sleeps which occurred within the single simulation.
     */

        /// <summary>The execute single simulation.</summary>
        /// <returns>The System.Collections.Generic.IList`1[T -&gt; System.Int64].</returns>
        public IList<long> ExecuteSingleSimulation()
        {
            var stealingSleeper = new StealingSleeper();
            var stealingBackoff = this.backOffPolicy.WithSleeper(stealingSleeper);

            var template = new RetryTemplate();
            template.BackOffPolicy = stealingBackoff;
            template.RetryPolicy = this.retryPolicy;

            try
            {
                template.Execute(new FailingRetryCallback<object>());
            }
            catch (FailingRetryException e)
            {
            }
            catch (Exception e)
            {
                throw new Exception("Unexpected exception", e);
            }

            return stealingSleeper.GetSleeps();
        }
    }

    internal class FailingRetryCallback<T> : IRetryCallback<T>
    {
        /// <summary>The do with retry.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The T.</returns>
        /// <exception cref="FailingRetryException"></exception>
        public T DoWithRetry(IRetryContext context) { throw new FailingRetryException(); }
    }

    internal class FailingRetryException : Exception
    {
    }

    internal class StealingSleeper : ISleeper
    {
        private readonly IList<long> sleeps = new List<long>();

        /// <summary>The sleep.</summary>
        /// <param name="backOffPeriod">The back off period.</param>
        public void Sleep(long backOffPeriod) { this.sleeps.Add(backOffPeriod); }

        /// <summary>The get sleeps.</summary>
        /// <returns>The System.Collections.Generic.IList`1[T -&gt; System.Int64].</returns>
        public IList<long> GetSleeps() { return this.sleeps; }
    }
}
