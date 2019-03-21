// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackToBackPatternClassifier.cs" company="The original author or authors.">
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
using System.Collections.Generic;
using Spring.Retry.Classify.Attributes;
#endregion

namespace Spring.Retry.Classify
{
    /// <summary>A special purpose <see cref="IClassifier{C,T}"/> with easy configuration options for mapping from one arbitrary type of object to another via a pattern matcher.</summary>
    /// <typeparam name="C">Type C.</typeparam>
    /// <typeparam name="T">Type T.</typeparam>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public class BackToBackPatternClassifier<C, T> : IClassifier<C, T>
    {
        private IClassifier<C, string> router;

        private IClassifier<string, T> matcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackToBackPatternClassifier{C,T}"/> class.
        /// Default constructor, provided as a convenience for people using setter injection.
        /// </summary>
        public BackToBackPatternClassifier() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackToBackPatternClassifier{C,T}"/> class.
        /// Set up a classifier with input to the router and output from the matcher.
        /// </summary>
        /// <param name="router">The router.</param>
        /// <param name="matcher">The matcher.</param>
        public BackToBackPatternClassifier(IClassifier<C, string> router, IClassifier<string, T> matcher)
        {
            this.router = router;
            this.matcher = matcher;
        }

        /// <summary>A convenience method for creating a pattern matching classifier for the matcher component.</summary>
        /// <param name="map">Maps pattern keys with wildcards to output values.</param>
        public void SetMatcherMap(IDictionary<string, T> map) { this.matcher = new PatternMatchingClassifier<T>(map); }

        /// <summary>
        /// A convenience method of creating a router classifier based on a plain old
        /// .NET Object. The object provided must have precisely one public method
        /// that either has the <see cref="ClassifierAttribute"/> attribute or accepts a single argument
        /// and outputs a string. This will be used to create an input classifier for the router component.
        /// </summary>
        /// <param name="routerDelegate">The router delegate.</param>
        public void SetRouterDelegate(object routerDelegate) { this.router = new ClassifierAdapter<C, string>(routerDelegate); }

        /// <summary>The classify.</summary>
        /// <param name="classifiable">The classifiable.</param>
        /// <returns>The T.</returns>
        public T Classify(C classifiable) { return this.matcher.Classify(this.router.Classify(classifiable)); }
    }
}
