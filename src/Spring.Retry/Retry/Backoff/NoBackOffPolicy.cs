// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoBackOffPolicy.cs" company="The original author or authors.">
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
    /// Implementation of <see cref="IBackOffPolicy"/> that performs a no-op and as such all retry operation in a given set proceed one after the other with no pause.
    /// </summary>
    /// <author>Rob Harrop</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public class NoBackOffPolicy : StatelessBackOffPolicy
    {
        /// <summary>The do back off.</summary>
        protected override void DoBackOff() { }
    }
}
