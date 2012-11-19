// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISleepingBackOffPolicy.cs" company="The original author or authors.">
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

namespace Spring.Retry.Retry.Backoff
{
    /// <summary>
    /// A interface which can be mixed in by <see cref="IBackOffPolicy"/>s indicating that they sleep A interface which can be mixed in by <see cref="IBackOffPolicy"/>s indicating that they sleep.
    /// </summary>
    /// <typeparam name="T">Type T.</typeparam>
    /// <author>Joe Fitzgerald (.NET)</author>
    public interface ISleepingBackOffPolicy : IBackOffPolicy
    {
        /// <summary>Clone the policy and return a new policy which uses the passed sleeper.</summary>
        /// <param name="sleeper">Target to be invoked any time the backoff policy sleeps.</param>
        /// <returns>A clone of this policy which will have all of its backoff sleeps routed into the passed sleeper.</returns>
        ISleepingBackOffPolicy WithSleeper(ISleeper sleeper);
    }
}
