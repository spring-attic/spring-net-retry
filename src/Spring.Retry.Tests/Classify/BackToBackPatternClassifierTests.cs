// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackToBackPatternClassifierTests.cs" company="The original author or authors.">
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
using Spring.Retry.Classify.Attributes;
#endregion

namespace Spring.Retry.Tests.Classify
{
    /// <summary>
    /// Back To Back Pattern Classifier Tests
    /// </summary>
    public class BackToBackPatternClassifierTests
    {
        private BackToBackPatternClassifier<string, string> classifier = new BackToBackPatternClassifier<string, string>();

        private IDictionary<string, string> map;

        /// <summary>The create map.</summary>
        [SetUp]
        public void CreateMap()
        {
            this.classifier  = new BackToBackPatternClassifier<string, string>();
            this.map = new Dictionary<string, string>();
            this.map.Add("foo", "bar");
            this.map.Add("*", "spam");
        }

        /// <summary>The test no classifiers.</summary>
        [Test]
        public void TestNoClassifiers()
        {
            try
            {
                this.classifier.Classify("foo");
            }
            catch (Exception ex)
            {
                Assert.IsAssignableFrom(typeof(NullReferenceException), ex);
            }
        }

        /// <summary>The test create from constructor.</summary>
        [Test]
        public void TestCreateFromConstructor()
        {
            this.classifier = new BackToBackPatternClassifier<string, string>(
                new PatternMatchingClassifier<string>(new Dictionary<string, string> { { "oof", "bucket" } }), 
                new PatternMatchingClassifier<string>(this.map));
            Assert.AreEqual("spam", this.classifier.Classify("oof"));
        }

        /// <summary>The test set router delegate.</summary>
        [Test]
        public void TestSetRouterDelegate()
        {
            this.classifier.SetRouterDelegate(new TestSetRouterDelegate());
            this.classifier.SetMatcherMap(this.map);
            Assert.AreEqual("spam", this.classifier.Classify("oof"));
        }
    }

    internal class TestSetRouterDelegate
    {
        /// <summary>The convert.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The System.String.</returns>
        [Classifier]
        public string Convert(string value) { return "bucket"; }
    }
}
