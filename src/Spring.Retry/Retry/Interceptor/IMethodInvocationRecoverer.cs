// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMethodInvocationRecoverer.cs" company="The original author or authors.">
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
#endregion

namespace Spring.Retry.Retry.Interceptor
{
    /// <summary>Strategy interface for recovery action when processing of an item fails.</summary>
    /// <typeparam name="T">Type T.</typeparam>
    /// <author>Dave Syer</author><author>Joe Fitzgerald (.NET)</author>
    public interface IMethodInvocationRecoverer
    {
        /// <summary>Recover gracefully from an error. Clients can call this if processing of
        /// the item throws an unexpected exception. Caller can use the return value
        /// to decide whether to try more corrective action or perhaps throw an
        /// exception.</summary>
        /// <param name="args">The arguments for the method invocation that failed.</param>
        /// <param name="cause">The cause of the failure that led to this recovery.</param>
        /// <returns>The T.</returns> 
        /// <typeparam name="T">Type T.</typeparam>
        T Recover<T>(object[] args, Exception cause);
    }
}
