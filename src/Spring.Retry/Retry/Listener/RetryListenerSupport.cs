// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RetryListenerSupport.cs" company="The original author or authors.">
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

namespace Spring.Retry.Retry.Listener
{
    /// <summary>Empty method implementation of <see cref="IRetryListener"/>.</summary>
    /// <typeparam name="T"></typeparam>
    /// <author>Dave Syer</author><author>Joe Fitzgerald (.NET)</author>
    public class RetryListenerSupport : IRetryListener
    {
        /// <summary>The close.</summary>
        /// <param name="context">The context.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="throwable">The throwable.</param>
        /// <typeparam name="T">Type T.</typeparam>
        public virtual void Close<T>(IRetryContext context, IRetryCallback<T> callback, Exception throwable) { }

        /// <summary>The close.</summary>
        /// <param name="context">The context.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="throwable">The throwable.</param>
        /// <typeparam name="T">Type T.</typeparam>
        public virtual void Close<T>(IRetryContext context, Func<IRetryContext, T> callback, Exception throwable) { }

        /// <summary>The on error.</summary>
        /// <param name="context">The context.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="throwable">The throwable.</param>
        /// <typeparam name="T">Type T.</typeparam>
        public virtual void OnError<T>(IRetryContext context, IRetryCallback<T> callback, Exception throwable) { }

        /// <summary>The on error.</summary>
        /// <param name="context">The context.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="throwable">The throwable.</param>
        /// <typeparam name="T">Type T.</typeparam>
        public virtual void OnError<T>(IRetryContext context, Func<IRetryContext, T> callback, Exception throwable) { }

        /// <summary>The open.</summary>
        /// <param name="context">The context.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>The System.Boolean.</returns>
        /// <typeparam name="T">Type T.</typeparam>
        public virtual bool Open<T>(IRetryContext context, IRetryCallback<T> callback) { return true; }

        /// <summary>The open.</summary>
        /// <param name="context">The context.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>The System.Boolean.</returns>
        /// <typeparam name="T">Type T.</typeparam>
        public virtual bool Open<T>(IRetryContext context, Func<IRetryContext, T> callback) { return true; }
    }
}
