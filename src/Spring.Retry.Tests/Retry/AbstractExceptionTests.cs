// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractExceptionTests.cs" company="The original author or authors.">
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
using NUnit.Framework;
#endregion

namespace Spring.Retry.Tests.Retry
{
    /// <summary>
    /// Abstract Exception Test Class.
    /// </summary>
    /// <author>Joe Fitzgerald (.NET)</author>
    public abstract class AbstractExceptionTests
    {
        /// <summary>The test exception string.</summary>
        [Test]
        public void TestExceptionString()
        {
            var exception = this.GetException("foo");
            Assert.AreEqual("foo", exception.Message);
        }

        /// <summary>The test exception string exception.</summary>
        [Test]
        public void TestExceptionStringException()
        {
            var exception = this.GetException("foo", new InvalidOperationException());
            Assert.AreEqual("foo", exception.Message.Substring(0, 3));
        }

        /// <summary>The get exception.</summary>
        /// <param name="msg">The msg.</param>
        /// <returns>The System.Exception.</returns>
        public abstract Exception GetException(string msg);

        /// <summary>The get exception.</summary>
        /// <param name="msg">The msg.</param>
        /// <param name="t">The t.</param>
        /// <returns>The System.Exception.</returns>
        public abstract Exception GetException(string msg, Exception t);
    }
}
