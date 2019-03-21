// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRetryOperations.cs" company="The original author or authors.">
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
using Spring.Retry.Retry.Support;
#endregion

namespace Spring.Retry.Retry
{
    /// <summary>
    /// Defines the basic set of operations implemented by <see cref="IRetryOperations"/> to execute operations with configurable retry behaviour.
    /// </summary>
    /// <author>Rob Harrop</author>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public interface IRetryOperations
    {
        /// <summary>Execute the supplied <see cref="IRetryCallback{T}"/> with the configured retry
        /// semantics. See implementations for configuration details.</summary>
        /// <param name="retryCallback">The retry callback.</param>
        /// <typeparam name="T">Type T.</typeparam>
        /// <returns>The value returned by the {@link RetryCallback} upon successful invocation.</returns>
        T Execute<T>(IRetryCallback<T> retryCallback);

        /// <summary>Execute the supplied <see cref="IRetryCallback{T}"/> with the configured retry
        /// semantics. See implementations for configuration details.</summary>
        /// <param name="retryCallback">The retry callback.</param>
        /// <typeparam name="T">Type T.</typeparam>
        /// <returns>The value returned by the {@link RetryCallback} upon successful invocation.</returns>
        T Execute<T>(Func<IRetryContext, T> retryCallback);

        /// <summary>Execute the supplied <see cref="IRetryCallback{T}"/> with a fallback on exhausted
        /// retry to the <see cref="IRecoveryCallback{T}"/>. See implementations for
        /// configuration details.</summary>
        /// <param name="retryCallback">The retry callback.</param>
        /// <param name="recoveryCallback">The recovery callback.</param>
        /// <typeparam name="T">Type T.</typeparam>
        /// <returns>The value returned by the <see cref="IRetryCallback{T}"/> upon successful invocation, and that returned by the <see cref="IRecoveryCallback{T}"/> otherwise.</returns>
        T Execute<T>(IRetryCallback<T> retryCallback, IRecoveryCallback<T> recoveryCallback);

        /// <summary>Execute the supplied <see cref="IRetryCallback{T}"/> with a fallback on exhausted
        /// retry to the <see cref="IRecoveryCallback{T}"/>. See implementations for
        /// configuration details.</summary>
        /// <param name="retryCallback">The retry callback.</param>
        /// <param name="recoveryCallback">The recovery callback.</param>
        /// <typeparam name="T">Type T.</typeparam>
        /// <returns>The value returned by the <see cref="IRetryCallback{T}"/> upon successful invocation, and that returned by the <see cref="IRecoveryCallback{T}"/> otherwise.</returns>
        T Execute<T>(Func<IRetryContext, T> retryCallback, Func<IRetryContext, T> recoveryCallback);

        /// <summary>Execute the supplied <see cref="IRetryCallback{T}"/> with a fallback on exhausted
        /// retry to the <see cref="IRecoveryCallback{T}"/>. See implementations for
        /// configuration details.</summary>
        /// <param name="retryCallback">The retry callback.</param>
        /// <param name="recoveryCallback">The recovery callback.</param>
        /// <typeparam name="T">Type T.</typeparam>
        /// <returns>The value returned by the <see cref="IRetryCallback{T}"/> upon successful invocation, and that returned by the <see cref="IRecoveryCallback{T}"/> otherwise.</returns>
        T Execute<T>(IRetryCallback<T> retryCallback, Func<IRetryContext, T> recoveryCallback);

        /// <summary>Execute the supplied <see cref="IRetryCallback{T}"/> with a fallback on exhausted
        /// retry to the <see cref="IRecoveryCallback{T}"/>. See implementations for
        /// configuration details.</summary>
        /// <param name="retryCallback">The retry callback.</param>
        /// <param name="recoveryCallback">The recovery callback.</param>
        /// <typeparam name="T">Type T.</typeparam>
        /// <returns>The value returned by the <see cref="IRetryCallback{T}"/> upon successful invocation, and that returned by the <see cref="IRecoveryCallback{T}"/> otherwise.</returns>
        T Execute<T>(Func<IRetryContext, T> retryCallback, IRecoveryCallback<T> recoveryCallback);

        /// <summary>A simple stateful retry. Execute the supplied {@link RetryCallback} with
        /// a target object for the attempt identified by the {@link DefaultRetryState}.
        /// Exceptions thrown by the callback are always propagated immediately so
        /// the state is required to be able to identify the previous attempt, if
        /// there is one - hence the state is required. Normal patterns would see
        /// this method being used inside a transaction, where the callback might
        /// invalidate the transaction if it fails.
        /// See implementations for configuration details.</summary>
        /// <param name="retryCallback">The retry callback.</param>
        /// <param name="retryState">The retry state.</param>
        /// <typeparam name="T">Type T.</typeparam>
        /// <returns>The value returned by the <see cref="IRetryCallback{T}"/> upon successful invocation, and that returned by the <see cref="IRecoveryCallback{T}"/> otherwise.</returns>
        T Execute<T>(IRetryCallback<T> retryCallback, IRetryState retryState);

        /// <summary>A simple stateful retry. Execute the supplied {@link RetryCallback} with
        /// a target object for the attempt identified by the {@link DefaultRetryState}.
        /// Exceptions thrown by the callback are always propagated immediately so
        /// the state is required to be able to identify the previous attempt, if
        /// there is one - hence the state is required. Normal patterns would see
        /// this method being used inside a transaction, where the callback might
        /// invalidate the transaction if it fails.
        /// See implementations for configuration details.</summary>
        /// <param name="retryCallback">The retry callback.</param>
        /// <param name="retryState">The retry state.</param>
        /// <typeparam name="T">Type T.</typeparam>
        /// <returns>The value returned by the <see cref="IRetryCallback{T}"/> upon successful invocation, and that returned by the <see cref="IRecoveryCallback{T}"/> otherwise.</returns>
        T Execute<T>(Func<IRetryContext, T> retryCallback, IRetryState retryState);

        /// <summary>A stateful retry with a recovery path. Execute the supplied<see cref="IRetryCallback{T}"/> with a fallback on exhausted retry to the<see cref="IRecoveryCallback{T}"/> and a target object for the retry attempt
        /// identified by the <see cref="DefaultRetryState"/>.<see cref="Execute{T}(Spring.Retry.Retry.IRetryCallback{T})"/></summary>
        /// <param name="retryCallback">The retry callback.</param>
        /// <param name="recoveryCallback">The recovery callback.</param>
        /// <param name="retryState">The retry state.</param>
        /// <typeparam name="T">Type T.</typeparam>
        /// <returns>The value returned by the <see cref="IRetryCallback{T}"/> upon successful invocation, and that returned by the <see cref="IRecoveryCallback{T}"/> otherwise.</returns>
        T Execute<T>(IRetryCallback<T> retryCallback, IRecoveryCallback<T> recoveryCallback, IRetryState retryState);

        /// <summary>A stateful retry with a recovery path. Execute the supplied<see cref="IRetryCallback{T}"/> with a fallback on exhausted retry to the<see cref="IRecoveryCallback{T}"/> and a target object for the retry attempt
        /// identified by the <see cref="DefaultRetryState"/>.<see cref="Execute{T}(Spring.Retry.Retry.IRetryCallback{T})"/></summary>
        /// <param name="retryCallback">The retry callback.</param>
        /// <param name="recoveryCallback">The recovery callback.</param>
        /// <param name="retryState">The retry state.</param>
        /// <typeparam name="T">Type T.</typeparam>
        /// <returns>The value returned by the <see cref="IRetryCallback{T}"/> upon successful invocation, and that returned by the <see cref="IRecoveryCallback{T}"/> otherwise.</returns>
        T Execute<T>(Func<IRetryContext, T> retryCallback, Func<IRetryContext, T> recoveryCallback, IRetryState retryState);

        /// <summary>A stateful retry with a recovery path. Execute the supplied<see cref="IRetryCallback{T}"/> with a fallback on exhausted retry to the<see cref="IRecoveryCallback{T}"/> and a target object for the retry attempt
        /// identified by the <see cref="DefaultRetryState"/>.<see cref="Execute{T}(Spring.Retry.Retry.IRetryCallback{T})"/></summary>
        /// <param name="retryCallback">The retry callback.</param>
        /// <param name="recoveryCallback">The recovery callback.</param>
        /// <param name="retryState">The retry state.</param>
        /// <typeparam name="T">Type T.</typeparam>
        /// <returns>The value returned by the <see cref="IRetryCallback{T}"/> upon successful invocation, and that returned by the <see cref="IRecoveryCallback{T}"/> otherwise.</returns>
        T Execute<T>(IRetryCallback<T> retryCallback, Func<IRetryContext, T> recoveryCallback, IRetryState retryState);

        /// <summary>A stateful retry with a recovery path. Execute the supplied<see cref="IRetryCallback{T}"/> with a fallback on exhausted retry to the<see cref="IRecoveryCallback{T}"/> and a target object for the retry attempt
        /// identified by the <see cref="DefaultRetryState"/>.<see cref="Execute{T}(Spring.Retry.Retry.IRetryCallback{T})"/></summary>
        /// <param name="retryCallback">The retry callback.</param>
        /// <param name="recoveryCallback">The recovery callback.</param>
        /// <param name="retryState">The retry state.</param>
        /// <typeparam name="T">Type T.</typeparam>
        /// <returns>The value returned by the <see cref="IRetryCallback{T}"/> upon successful invocation, and that returned by the <see cref="IRecoveryCallback{T}"/> otherwise.</returns>
        T Execute<T>(Func<IRetryContext, T> retryCallback, IRecoveryCallback<T> recoveryCallback, IRetryState retryState);
    }
}
