// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRecoveryCallback.cs" company="The original author or authors.">
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

namespace Spring.Retry.Retry
{
    /// <summary>Callback for stateful retry after all tries are exhausted.</summary>
    /// <typeparam name="T">Type T.</typeparam>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald</author>
    public interface IRecoveryCallback<T>
    {
        /// <summary>Recover with the supplied context.</summary>
        /// <param name="context">The current retry context.</param>
        /// <returns>An object that can be used to replace the callback result that failed.</returns>
        T Recover(IRetryContext context);
    }
}
