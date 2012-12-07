// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassifierAttribute.cs" company="The original author or authors.">
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
#endregion

namespace Spring.Retry.Classify.Attributes
{
    /// <summary>
    /// Mark a method as capable of classifying its input to an instance of its output. Should only be used on non-void methods with one parameter.
    /// </summary>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ClassifierAttribute : Attribute
    {
    }
}
