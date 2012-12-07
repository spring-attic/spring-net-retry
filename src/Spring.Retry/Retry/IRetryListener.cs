// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRetryListener.cs" company="The original author or authors.">
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

namespace Spring.Retry.Retry
{
    /// <summary>Interface for listener that can be used to add behavior to a retry.
    /// Implementations of <see cref="IRetryOperations{T}"/> can chose to issue callbacks to an
    /// interceptor during the retry lifecycle.</summary>
    /// <typeparam name="T">Type T.</typeparam>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public interface IRetryListener
    {
        /// <summary>Called before the first attempt in a retry. For instance, implementers
        /// can set up state that is needed by the policies in the<see cref="IRetryOperations{T}"/>. The whole retry can be vetoed by returning
        /// false from this method, in which case a <see cref="TerminatedRetryException"/>
        /// will be thrown.</summary>
        /// <param name="context">The current <see cref="IRetryContext"/>.</param>
        /// <param name="callback">The current <see cref="IRecoveryCallback{T}"/>.</param>
        /// <returns>True if the retry should proceed.</returns>
        bool Open<T>(IRetryContext context, IRetryCallback<T> callback);

        /// <summary>Called before the first attempt in a retry. For instance, implementers
        /// can set up state that is needed by the policies in the<see cref="IRetryOperations{T}"/>. The whole retry can be vetoed by returning
        /// false from this method, in which case a <see cref="TerminatedRetryException"/>
        /// will be thrown.</summary>
        /// <param name="context">The current <see cref="IRetryContext"/>.</param>
        /// <param name="callback">The current <see cref="IRecoveryCallback{T}"/>.</param>
        /// <returns>True if the retry should proceed.</returns>
        bool Open<T>(IRetryContext context, Func<IRetryContext, T> callback);

        /// <summary>Called after the final attempt (successful or not). Allow the interceptor
        /// to clean up any resource it is holding before control returns to the
        /// retry caller.</summary>
        /// <param name="context">The current <see cref="IRetryContext"/>.</param>
        /// <param name="callback">The current <see cref="IRecoveryCallback{T}"/>.</param>
        /// <param name="throwable">The last exception that was thrown by the callback.</param>
        void Close<T>(IRetryContext context, IRetryCallback<T> callback, Exception throwable);

        /// <summary>Called after the final attempt (successful or not). Allow the interceptor
        /// to clean up any resource it is holding before control returns to the
        /// retry caller.</summary>
        /// <param name="context">The current <see cref="IRetryContext"/>.</param>
        /// <param name="callback">The current <see cref="IRecoveryCallback{T}"/>.</param>
        /// <param name="throwable">The last exception that was thrown by the callback.</param>
        void Close<T>(IRetryContext context, Func<IRetryContext, T> callback, Exception throwable);

        /// <summary>Called after every unsuccessful attempt at a retry.</summary>
        /// <param name="context">The current <see cref="IRetryContext"/>.</param>
        /// <param name="callback">The current <see cref="IRecoveryCallback{T}"/>.</param>
        /// <param name="throwable">The last exception that was thrown by the callback.</param>
        void OnError<T>(IRetryContext context, IRetryCallback<T> callback, Exception throwable);

        /// <summary>Called after every unsuccessful attempt at a retry.</summary>
        /// <param name="context">The current <see cref="IRetryContext"/>.</param>
        /// <param name="callback">The current <see cref="IRecoveryCallback{T}"/>.</param>
        /// <param name="throwable">The last exception that was thrown by the callback.</param>
        void OnError<T>(IRetryContext context, Func<IRetryContext, T> callback, Exception throwable);
    }
}
