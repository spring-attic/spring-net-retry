// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlwaysRetryPolicy.cs" company="The original author or authors.">
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
using Spring.Retry.Retry.Support;
#endregion

namespace Spring.Retry.Retry.Policy
{
    /// <summary>
    /// A <see cref="IRetryPolicy"/> that always permits a retry. Can also be used as a base class for other policies, e.g. for test purposes as a stub.
    /// </summary>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public class AlwaysRetryPolicy : NeverRetryPolicy
    {
        /// <summary>Always return true. <see cref="IRetryPolicy.CanRetry"/></summary>
        /// <param name="context">The context.</param>
        /// <returns>The System.Boolean.</returns>
        public override bool CanRetry(IRetryContext context) { return true; }
    }
}
