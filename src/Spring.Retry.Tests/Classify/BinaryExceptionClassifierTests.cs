// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryExceptionClassifierTests.cs" company="The original author or authors.">
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
using System.Collections.Generic;
using NUnit.Framework;
using Spring.Retry.Classify;
#endregion

namespace Spring.Retry.Tests.Classify
{
    /// <summary>
    /// Binary Exception Classifier Tests
    /// </summary>
    [TestFixture]
    public class BinaryExceptionClassifierTests
    {
        private BinaryExceptionClassifier classifier = new BinaryExceptionClassifier(false);

        [SetUp]
        public void SetUp()
        {
            this.classifier = new BinaryExceptionClassifier(false);
        }

        /// <summary>The test classify null is default.</summary>
        [Test]
        public void TestClassifyNullIsDefault() { Assert.IsFalse(this.classifier.Classify(null)); }

        /// <summary>The test false is default.</summary>
        [Test]
        public void TestFalseIsDefault() { Assert.IsFalse(this.classifier.GetDefault()); }

        /// <summary>The test default provided.</summary>
        [Test]
        public void TestDefaultProvided()
        {
            this.classifier = new BinaryExceptionClassifier(true);
            Assert.IsTrue(this.classifier.GetDefault());
        }

        /// <summary>The test classify random exception.</summary>
        [Test]
        public void TestClassifyRandomException() { Assert.IsFalse(this.classifier.Classify(new InvalidOperationException("foo"))); }

        /// <summary>The test classify exact match.</summary>
        [Test]
        public void TestClassifyExactMatch()
        {
            var set = new List<Type> { typeof(InvalidOperationException) };
            Assert.IsTrue(new BinaryExceptionClassifier(set).Classify(new InvalidOperationException("Foo")));
        }

        /// <summary>The test types provided in constructor.</summary>
        [Test]
        public void TestTypesProvidedInConstructor()
        {
            this.classifier = new BinaryExceptionClassifier(new List<Type> { typeof(InvalidOperationException) });
            Assert.IsTrue(this.classifier.Classify(new InvalidOperationException("Foo")));
        }

        /// <summary>The test types provided in constructor with non default.</summary>
        [Test]
        public void TestTypesProvidedInConstructorWithNonDefault()
        {
            this.classifier = new BinaryExceptionClassifier(new List<Type> { typeof(InvalidOperationException) }, false);
            Assert.IsFalse(this.classifier.Classify(new InvalidOperationException("Foo")));
        }
    }
}
