// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackOffInterruptedException.cs" company="The original author or authors.">
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
using System.Threading;
#endregion

namespace Spring.Retry.Retry.Backoff
{
    /// <summary>
    /// Exception class signifiying that an attempt to back off using a <see cref="IBackOffPolicy"/> was interrupted, most likely by an
    /// <see cref="ThreadInterruptedException"/> during a call to <see cref="Thread.Sleep(int)"/>.
    /// </summary>
    public class BackOffInterruptedException : RetryException
    {
        /// <summary>Initializes a new instance of the <see cref="BackOffInterruptedException"/> class.</summary>
        /// <param name="msg">The msg.</param>
        public BackOffInterruptedException(string msg) : base(msg) { }

        /// <summary>Initializes a new instance of the <see cref="BackOffInterruptedException"/> class.</summary>
        /// <param name="msg">The msg.</param>
        /// <param name="cause">The cause.</param>
        public BackOffInterruptedException(string msg, Exception cause) : base(msg, cause) { }
    }
}
