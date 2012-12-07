// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassifierSupportTests.cs" company="The original author or authors.">
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
using Spring.Retry.Classify;
#endregion

namespace Spring.Retry.Tests.Classify
{
    /// <summary>
    /// Classifier Support Tests
    /// </summary>
    [TestFixture]
    public class ClassifierSupportTests
    {
        /// <summary>The test classify null is default.</summary>
        [Test]
        public void TestClassifyNullIsDefault()
        {
            var classifier = new ClassifierSupport<string, string>("foo");
            Assert.AreEqual(classifier.Classify(null), "foo");
        }

        /// <summary>The test classify random exception.</summary>
        [Test]
        public void TestClassifyRandomException()
        {
            var classifier = new ClassifierSupport<Exception, string>("foo");
            Assert.AreEqual(classifier.Classify(new InvalidOperationException("Foo")), classifier.Classify(null));
        }
    }
}
