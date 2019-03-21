// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClassifier.cs" company="The original author or authors.">
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
    /// <summary>Interface for a classifier. At its simplest a <see cref="IClassifier{C,T}"/> is just a map from objects of one type to objects of another type.</summary>
    /// <typeparam name="C">Type C</typeparam>
    /// <typeparam name="T">Type T</typeparam>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public interface IClassifier<C, T>
    {
        /// <summary>The classify.</summary>
        /// <param name="classifiable">The classifiable.</param>
        /// <returns>The T.</returns>
        T Classify(C classifiable);
    }
}
