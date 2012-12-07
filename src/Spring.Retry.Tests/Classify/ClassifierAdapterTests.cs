// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassifierAdapterTests.cs" company="The original author or authors.">
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
using Moq;
using NUnit.Framework;
using Spring.Retry.Classify;
using Spring.Retry.Classify.Attributes;
#endregion

namespace Spring.Retry.Tests.Classify
{
    /// <summary>
    /// Classifier Adapter Tests
    /// </summary>
    public class ClassifierAdapterTests
    {
        private ClassifierAdapter<string, int> adapter = new ClassifierAdapter<string, int>();

        /// <summary>The test classifier adapter object.</summary>
        [Test]
        public void TestClassifierAdapterObject()
        {
            this.adapter = new ClassifierAdapter<string, int>(new TestClassifierAdapterObject());
            Assert.AreEqual(23, this.adapter.Classify("23"));
        }


        [Test]
        public void TestClassifierAdapterObjectWithNoAnnotation()
        {
            try
            {
                adapter = new ClassifierAdapter<string, int>(new TestClassifierAdapterObjectWithNoAnnotation());
                Assert.AreEqual(23, adapter.Classify("23"));
                Assert.Fail("Expected an InvalidOperationException");
            }
            catch (InvalidOperationException ex)
            {
                Assert.True(ex is InvalidOperationException);
            }
        }

        [Test]
        public void TestClassifierAdapterObjectSingleMethodWithNoAnnotation()
        {
            adapter = new ClassifierAdapter<string, int>(new TestClassifierAdapterObjectSingleMethodWithNoAnnotation());
            Assert.AreEqual(23, adapter.Classify("23"));
        }

        [Test]
        public void TestClassifierAdapterClassifier()
        {
            var mockClassifier = new Mock<IClassifier<string, int>>();
            mockClassifier.Setup(m => m.Classify(It.IsAny<string>())).Returns<string>(int.Parse);

            adapter = new ClassifierAdapter<string, int>(mockClassifier.Object);
            Assert.AreEqual(23, adapter.Classify("23"));
        }

        [Test]
        public void TestClassifyWithSetter()
        {
            adapter.SetDelegate(new TestClassifyWithSetter());
            Assert.AreEqual(23, adapter.Classify("23"));
        }

        [Test]
        public void TestClassifyWithWrongType()
        {
            try
            {
                adapter.SetDelegate(new TestClassifyWithWrongType());
                Assert.AreEqual(23, adapter.Classify("23"));
            }
            catch (InvalidOperationException ex)
            {
                Assert.True(ex is InvalidOperationException);
            }
        }

        [Test]
        public void TestClassifyWithClassifier()
        {
            var mockClassifier = new Mock<IClassifier<string, int>>();
            mockClassifier.Setup(m => m.Classify(It.IsAny<string>())).Returns<string>(int.Parse);

            adapter.SetDelegate(mockClassifier.Object);
            Assert.AreEqual(23, adapter.Classify("23"));
        }
    }

    internal class TestClassifierAdapterObject
    {
        /// <summary>The get value.</summary>
        /// <param name="key">The key.</param>
        /// <returns>The System.Int32.</returns>
        [Classifier]
        public int GetValue(string key) { return int.Parse(key); }

        /// <summary>The get another.</summary>
        /// <param name="key">The key.</param>
        /// <returns>The System.Int32.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public int GetAnother(string key) { throw new InvalidOperationException("Not allowed"); }
    }

    internal class TestClassifierAdapterObjectWithNoAnnotation
    {
        public int GetValue(string key) { return int.Parse(key); }

        public int GetAnother(string key) { throw new InvalidOperationException("Not allowed"); }
    }

    internal class TestClassifierAdapterObjectSingleMethodWithNoAnnotation
    {
        public int GetValue(string key) { return int.Parse(key); }

        public void DoNothing(string key) { }

        public string DoNothing(string key, int value) { return "foo"; }
    }

    internal class TestClassifyWithSetter
    {
        [Classifier]
        public int GetValue(string key) { return int.Parse(key); }
    }

    internal class TestClassifyWithWrongType
    {
        [Classifier]
        public string GetValue(int key) { return key.ToString(); }
    }
}
