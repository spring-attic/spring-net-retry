// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRetryStatistics.cs" company="The original author or authors.">
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

namespace Spring.Retry.Retry
{
    /// <summary>
    /// Interface for statistics reporting of retry attempts. Counts the number of
    /// retry attempts, successes, errors (includiBng retries), and aborts.
    /// </summary>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public interface IRetryStatistics
    {
        /// <summary>Get the number of completed retry attempts (successful or not).</summary>
        int CompleteCount { get; }

        /// <summary>Get the number of times a retry block has been entered, irrespective of how many times the operation was retried.</summary>
        int StartedCount { get; }

        /// <summary>Get the number of errors detected, whether or not they resulted in a retry.</summary>
        int ErrorCount { get; }

        /// <summary>Get the number of times a block failed to complete successfully, even after retry.</summary>
        int AbortCount { get; }

        /// <summary>Get an identifier for the retry block for reporting purposes.</summary>
        string Name { get; }
    }
}
