// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RetryCacheCapacityExceededException.cs" company="The original author or authors.">
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
using System;
#endregion

namespace Spring.Retry.Retry.Policy
{
    /// <summary>
    /// Exception that indicates that a cache limit was exceeded. This is often a
    /// sign of badly or inconsistently implemented GetHashCode, Equals in failed items.
    /// Items can then fail repeatedly and appear different to the cache, so they get
    /// added over and over again until a limit is reached and this exception is
    /// thrown. Consult the documentation of the <see cref="IRetryContextCache"/> in use to
    /// determine how to increase the limit if appropriate.
    /// </summary>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public class RetryCacheCapacityExceededException : RetryException
    {
        /// <summary>Initializes a new instance of the <see cref="RetryCacheCapacityExceededException"/> class.</summary>
        /// <param name="message">The message.</param>
        public RetryCacheCapacityExceededException(string message) : base(message) { }

        /// <summary>Initializes a new instance of the <see cref="RetryCacheCapacityExceededException"/> class.</summary>
        /// <param name="msg">The msg.</param>
        /// <param name="nested">The nested.</param>
        public RetryCacheCapacityExceededException(string msg, Exception nested) : base(msg, nested) { }
    }
}
