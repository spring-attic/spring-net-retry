// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INewMethodArgumentsIdentifier.cs" company="The original author or authors.">
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

namespace Spring.Retry.Retry.Interceptor
{
    /// <summary>
    /// Strategy interface to distinguish new arguments from ones that have been
    /// processed before, e.g. by examining a message flag.
    /// </summary>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public interface INewMethodArgumentsIdentifier
    {
        /// <summary>
        /// Inspect the arguments and determine if they have never been processed
        /// before. The safest choice when the answer is indeterminate is 'false'.
        /// </summary>
        /// <param name="args">The current method arguments.</param>
        /// <returns>True if the item is known to have never been processed before.</returns>
        bool IsNew(object[] args);
    }
}
