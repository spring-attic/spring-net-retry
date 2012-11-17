// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubclassExceptionClassifierTests.cs" company="The original author or authors.">
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
using System.Collections.Generic;
using NUnit.Framework;
using Spring.Retry.Classify;
#endregion

namespace Spring.Retry.Tests.Classify
{
    /// <summary>
    /// Subclass Exception Classifier Tests
    /// </summary>
    [TestFixture]
    public class SubclassExceptionClassifierTests
    {
        private SubclassClassifier<Exception, string> classifier = new SubclassClassifier<Exception, string>();
        
        [SetUp]
        public void Setup()
        {
            this.classifier = new SubclassClassifier<Exception, string>();
        }

        [Test]
        public void TestClassifyNullIsDefault() { Assert.AreEqual(classifier.Classify(null), classifier.GetDefault()); }

        [Test]
        public void TestClassifyNull() { Assert.IsNull(classifier.Classify(null)); }

        [Test]
        public void TestClassifyNullNonDefault()
        {
            classifier = new SubclassClassifier<Exception, string>("foo");
            Assert.AreEqual("foo", classifier.Classify(null));
        }

        [Test]
        public void TestClassifyRandomException()
        {
            Assert.IsNull(classifier.Classify(new InvalidOperationException("Foo")));
        }

        [Test]
        public void TestClassifyExactMatch()
        {
            classifier.SetTypeMap(new Dictionary<Type, string>() { { typeof(InvalidOperationException), "foo" } });
            Assert.AreEqual("foo", classifier.Classify(new InvalidOperationException("Foo")));
        }

        [Test]
        public void TestClassifySubclassMatch()
        {
            classifier.SetTypeMap(new Dictionary<Type, string>() { { typeof(Exception), "foo" } });
            Assert.AreEqual("foo", classifier.Classify(new InvalidOperationException("Foo")));
        }

        [Test]
        public void TestClassifySuperclassDoesNotMatch()
        {
            classifier.SetTypeMap(new Dictionary<Type, string>() { { typeof(InvalidOperationException), "foo" } });
            Assert.AreEqual(classifier.GetDefault(), classifier.Classify(new Exception("Foo")));
        }

        [Test]
        public void TestClassifyAncestorMatch()
        {
            classifier.SetTypeMap(new Dictionary<Type, string>() { { typeof(Exception), "foo" }, { typeof(ArgumentException), "bar" }, { typeof(InvalidOperationException), "spam" } });
            Assert.AreEqual("spam", classifier.Classify(new InvalidOperationException("Foo")));
        }
    }
}
