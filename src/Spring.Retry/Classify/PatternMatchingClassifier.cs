// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatternMatchingClassifier.cs" company="The original author or authors.">
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
#endregion

namespace Spring.Retry.Classify
{
    /// <summary>A <see cref="IClassifier{C,T}"/> that maps from String patterns with wildcards to a set
    /// of values of a given type. An input String is matched with the most specific
    /// pattern possible to the corresponding value in an input map. A default value
    /// should be specified with a pattern key of "*".</summary>
    /// <typeparam name="T">Type T.</typeparam>
    public class PatternMatchingClassifier<T> : IClassifier<string, T>
    {
        private PatternMatcher<T> values;

        /// <summary>Initializes a new instance of the <see cref="PatternMatchingClassifier{T}"/> class.</summary>
        public PatternMatchingClassifier() : this(new Dictionary<string, T>()) { }

        /// <summary>Initializes a new instance of the <see cref="PatternMatchingClassifier{T}"/> class.</summary>
        /// <param name="values">The values.</param>
        public PatternMatchingClassifier(IDictionary<string, T> values) { this.values = new PatternMatcher<T>(values); }

        /// <summary>The set pattern map.</summary>
        /// <param name="values">The values.</param>
        public void SetPatternMap(IDictionary<string, T> values) { this.values = new PatternMatcher<T>(values); }

        /// <summary>
        /// Classify the input by matching it against the patterns provided in <see cref="SetPatternMap"/>.
        /// The most specific pattern that matches will be used to locate a value.
        /// </summary>
        /// <param name="classifiable">The classifiable.</param>
        /// <returns>The T.</returns>
        public T Classify(string classifiable)
        {
            var value = this.values.Match(classifiable);
            return value;
        }
    }
}
