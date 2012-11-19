// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RetrySimulation.cs" company="The original author or authors.">
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
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Spring.Retry.Retry.Support
{
    /// <summary>
    /// The results of a simulation.
    /// </summary>
    /// <author>Joe Fitzgerald (.NET)</author>
    public class RetrySimulation
    {
        private readonly IList<SleepSequence> sleepSequences = new List<SleepSequence>();
        private readonly IDictionary<long, long> sleepHistogram = new Dictionary<long, long>();
        
        /// <summary>Add a sequence of sleeps to the simulation.</summary>
        /// <param name="sleeps">The sleeps.</param>
        public void AddSequence(IList<long> sleeps)
        {
            foreach (var sleep in sleeps)
            {
                var existingHisto = default(long);
                this.sleepHistogram.TryGetValue(sleep, out existingHisto);
                if (existingHisto == default(long))
                {
                    this.sleepHistogram.Add(sleep, 1);
                }
                else
                {
                    this.sleepHistogram.Add(sleep, existingHisto + 1);
                }
            }

            this.sleepSequences.Add(new SleepSequence(sleeps));
        }

        /// <summary>Returns a list of all the unique sleep values which were executed within all simulations.</summary>
        /// <returns>The System.Collections.Generic.IList`1[T -&gt; System.Int64].</returns>
        public IList<long> GetUniqueSleeps()
        {
            var res = new List<long>(this.sleepHistogram.Keys);
            res.Sort();
            return res;
        }

        /// <summary>The count of each sleep which was seen throughout all sleeps.</summary>
        /// <returns>The System.Collections.Generic.IList`1[T -&gt; System.Int64].</returns>
        public IList<long> GetUniqueSleepsHistogram()
        {
            var res = new List<long>(this.sleepHistogram.Count);
            foreach (var sleep in this.GetUniqueSleeps())
            {
                res.Add(this.sleepHistogram[sleep]);
            }

            return res;
        }

        /// <summary>The get longest total sleep sequence.</summary>
        /// <returns>The longest total time slept by a retry sequence.</returns>
        public SleepSequence GetLongestTotalSleepSequence()
        {
            SleepSequence longest = null;
            foreach (var sequence in this.sleepSequences)
            {
                if (longest == null || sequence.GetTotalSleep() > longest.GetTotalSleep())
                {
                    longest = sequence;
                }
            }

            return longest;
        }
    }

    /// <summary>The sleep sequence.</summary>
    public class SleepSequence
    {
        private readonly IList<long> sleeps;
        private readonly long longestSleep;
        private readonly long totalSleep;

        /// <summary>Initializes a new instance of the <see cref="SleepSequence"/> class.</summary>
        /// <param name="sleeps">The sleeps.</param>
        public SleepSequence(IList<long> sleeps)
        {
            this.sleeps = sleeps;
            this.longestSleep = sleeps.Max();
            long totalSleep = 0;
            foreach (var sleep in sleeps)
            {
                totalSleep += sleep;
            }

            this.totalSleep = totalSleep;
        }

        /// <summary>The get sleeps.</summary>
        /// <returns>The System.Collections.Generic.IList`1[T -&gt; System.Int64].</returns>
        public IList<long> GetSleeps() { return this.sleeps; }

        /**
         * Returns the longest individual sleep within this sequence.
         */

        /// <summary>The get longest sleep.</summary>
        /// <returns>The System.Int64.</returns>
        public long GetLongestSleep() { return this.longestSleep; }

        /// <summary>The get total sleep.</summary>
        /// <returns>The System.Int64.</returns>
        public long GetTotalSleep() { return this.totalSleep; }

        /// <summary>The to string.</summary>
        /// <returns>The System.String.</returns>
        public override string ToString() { return "totalSleep=" + this.totalSleep + ": " + this.sleeps; }
    }
}
