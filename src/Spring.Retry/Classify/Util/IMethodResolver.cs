// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMethodResolver.cs" company="The original author or authors.">
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
using System.Reflection;
#endregion

namespace Spring.Retry.Classify.Util
{
    /// <summary>
    /// Strategy interface for detecting a single Method on a Class.
    /// </summary>
    /// <author>Mark Fisher</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public interface IMethodResolver
    {
        /// <summary>Find a single Method on the provided Object that matches this resolver's criteria.</summary>
        /// <param name="candidate">The candidate object whose type should be searched for a method.</param>
        /// <returns>A single method or null if no method matching this resolver's criteria can be found.</returns>
        MethodInfo FindMethod(object candidate);
        
        /// <summary>Find a single method on the given type that matches this resolver's criteria.</summary>
        /// <param name="type">The type in which to search for a method.</param>
        /// <returns>A single method or null if no method matching this resolver's criteria can be found.</returns>
        MethodInfo FindMethod(Type type);
    }
}
