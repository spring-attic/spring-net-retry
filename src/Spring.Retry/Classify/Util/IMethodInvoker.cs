﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMethodInvoker.cs" company="The original author or authors.">
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

namespace Spring.Retry.Classify.Util
{
    /// <summary>
    /// A strategy interface for invoking a method. Typically used by adapters.
    /// </summary>
    /// <author>Mark Fisher</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public interface IMethodInvoker
    {
        /// <summary>The invoke method.</summary>
        /// <param name="args">The args.</param>
        /// <returns>The System.Object.</returns>
        object InvokeMethod(params object[] args);
    }
}
