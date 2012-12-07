// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TerminatedRetryExceptionTests.cs" company="The original author or authors.">
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
using NUnit.Framework;
using Spring.Retry.Retry;
#endregion

namespace Spring.Retry.Tests.Retry
{
    /// <summary>
    /// TerminatedRetryException Tests
    /// </summary>
    [TestFixture]
    public class TerminatedRetryExceptionTests : AbstractExceptionTests
    {
        /// <summary>The get exception.</summary>
        /// <param name="msg">The msg.</param>
        /// <returns>The System.Exception.</returns>
        public override Exception GetException(string msg) { return new TerminatedRetryException(msg); }

        /// <summary>The get exception.</summary>
        /// <param name="msg">The msg.</param>
        /// <param name="t">The t.</param>
        /// <returns>The System.Exception.</returns>
        public override Exception GetException(string msg, Exception t) { return new TerminatedRetryException(msg, t); }
    }
}
