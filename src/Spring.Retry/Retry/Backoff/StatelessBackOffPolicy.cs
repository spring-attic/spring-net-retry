// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatelessBackOffPolicy.cs" company="The original author or authors.">
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
    /// Simple base class for <see cref="IBackOffPolicy"/> implementations that maintain no state across invocations.
    /// </summary>
    /// <author>Rob Harrop</author>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public abstract class StatelessBackOffPolicy : IBackOffPolicy
    {
        /// <summary>Delegates directly to the <see cref="DoBackOff"/> method without passing on the <see cref="IBackOffContext"/> argument which is not needed for stateless implementations.</summary>
        /// <param name="backOffContext">The back off context.</param>
        public void BackOff(IBackOffContext backOffContext) { this.DoBackOff(); }

        /// <summary>Returns null. Subclasses can add behaviour, e.g. * initial sleep before first attempt.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The Spring.Retry.Retry.Backoff.IBackOffContext.</returns>
        public virtual IBackOffContext Start(IRetryContext context) { return null; }

        /// <summary>Sub-classes should implement this method to perform the actual back off.</summary>
        protected abstract void DoBackOff();
    }
}
