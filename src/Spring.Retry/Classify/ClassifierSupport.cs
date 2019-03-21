// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassifierSupport.cs" company="The original author or authors.">
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

namespace Spring.Retry.Classify
{
    /// <summary>Base class for <see cref="IClassifier{C,T}"/> implementations. Provides default behavior and some convenience members, like constants.</summary>
    /// <typeparam name="C">Type C.</typeparam>
    /// <typeparam name="T">Type T.</typeparam>
    public class ClassifierSupport<C, T> : IClassifier<C, T>
    {
        private readonly T defaultValue;

        /// <summary>Initializes a new instance of the <see cref="ClassifierSupport{C,T}"/> class.</summary>
        /// <param name="defaultValue">The default value.</param>
        public ClassifierSupport(T defaultValue) { this.defaultValue = defaultValue; }

        /// <summary>Always returns the default value. This is the main extension point for subclasses, so it must be able to classify null.</summary>
        /// <param name="classifiable">The classifiable.</param>
        /// <returns>The T.</returns>
        public T Classify(C classifiable) { return this.defaultValue; }
    }
}
