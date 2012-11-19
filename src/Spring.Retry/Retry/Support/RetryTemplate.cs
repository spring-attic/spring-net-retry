// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RetryTemplate.cs" company="The original author or authors.">
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
using System.Collections.Generic;
using Common.Logging;
using Spring.Core;
using Spring.Retry.Retry.Backoff;
using Spring.Retry.Retry.Policy;
#endregion

namespace Spring.Retry.Retry.Support
{
    /// <summary>Template class that simplifies the execution of operations with retry
    /// semantics.
    /// Retryable operations are encapsulated in implementations of the<see cref="IRetryCallback{T}"/> interface and are executed using one of the supplied
    /// execute methods. 
    /// By default, an operation is retried if is throws any <see cref="Exception"/> or
    /// subclass of <see cref="Exception"/>. This behaviour can be changed by using the<see cref="IRetryPolicy"/> method. <br/>
    ///  Also by default, each operation is retried for a maximum of three attempts
    /// with no back off in between. This behaviour can be configured using the<see cref="SetRetryPolicy"/>{@link #setRetryPolicy(RetryPolicy)} and
    /// {@link #setBackOffPolicy(BackOffPolicy)} properties. The
    /// {@link org.springframework.retry.backoff.BackOffPolicy} controls how
    /// long the pause is between each individual retry attempt. <br/>
    ///  This class is thread-safe and suitable for concurrent access when executing
    /// operations and when performing configuration changes. As such, it is possible
    /// to change the number of retries on the fly, as well as the
    /// {@link BackOffPolicy} used and no in progress retryable operations will be
    /// affected.</summary>
    /// <typeparam name="T">Type T.</typeparam>
    public class RetryTemplate<T> : IRetryOperations<T>
    {
        protected static readonly ILog Logger = LogManager.GetCurrentClassLogger();
        private volatile IBackOffPolicy backOffPolicy = new NoBackOffPolicy();

        private volatile IRetryPolicy retryPolicy = new SimpleRetryPolicy(3, new Dictionary<Type, bool> { { typeof(Exception), true } });

        private volatile IRetryListener<T>[] listeners = new IRetryListener<T>[0];

        private IRetryContextCache retryContextCache = new MapRetryContextCache();

        /// <summary>Sets the retry context cache.</summary>
        public IRetryContextCache RetryContextCache { set { this.retryContextCache = value; } }

        /// <summary>Sets the listeners.</summary>
        public IRetryListener<T>[] Listeners { set { this.listeners = value; } }

        /// <summary>Sets the back off policy.</summary>
        public IBackOffPolicy BackOffPolicy { set { this.backOffPolicy = value; } }

        /// <summary>Sets the retry policy.</summary>
        public IRetryPolicy RetryPolicy { set { this.retryPolicy = value; } }

        /**
	 * Register an additional listener.
	 * 
	 * @param listener
	 * @see #setListeners(RetryListener[])
	 */

        /// <summary>The register listener.</summary>
        /// <param name="listener">The listener.</param>
        public void RegisterListener(IRetryListener<T> listener)
        {
            var list = new List<IRetryListener<T>>(this.listeners);
            list.Add(listener);
            this.listeners = list.ToArray();
        }

        /// <summary>The execute.</summary>
        /// <param name="retryCallback">The retry callback.</param>
        /// <returns>The T.</returns>
        public T Execute(IRetryCallback<T> retryCallback) { return this.DoExecute(retryCallback, null, null); }

        /// <summary>The execute.</summary>
        /// <param name="retryCallback">The retry callback.</param>
        /// <param name="recoveryCallback">The recovery callback.</param>
        /// <returns>The T.</returns>
        public T Execute(IRetryCallback<T> retryCallback, IRecoveryCallback<T> recoveryCallback) { return this.DoExecute(retryCallback, recoveryCallback, null); }

        /// <summary>The execute.</summary>
        /// <param name="retryCallback">The retry callback.</param>
        /// <param name="retryState">The retry state.</param>
        /// <returns>The T.</returns>
        public T Execute(IRetryCallback<T> retryCallback, IRetryState retryState) { return this.DoExecute(retryCallback, null, retryState); }

        /// <summary>The execute.</summary>
        /// <param name="retryCallback">The retry callback.</param>
        /// <param name="recoveryCallback">The recovery callback.</param>
        /// <param name="retryState">The retry state.</param>
        /// <returns>The T.</returns>
        public T Execute(IRetryCallback<T> retryCallback, IRecoveryCallback<T> recoveryCallback, IRetryState retryState) { return this.DoExecute(retryCallback, recoveryCallback, retryState); }

        /// <summary>
        /// Execute the callback once if the policy dictates that we can, otherwise execute the recovery callback.
        /// </summary>
        /// <param name="retryCallback">The retry callback.</param>
        /// <param name="recoveryCallback">The recovery callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>The T.</returns>
        protected T DoExecute(IRetryCallback<T> retryCallback, IRecoveryCallback<T> recoveryCallback, IRetryState state)
        {
            var retryPolicy = this.retryPolicy;
            var backOffPolicy = this.backOffPolicy;

            // Allow the retry policy to initialise itself...
            var context = this.Open(retryPolicy, state);
            Logger.Trace(m => m("RetryContext retrieved: {0}", context));

            // Make sure the context is available globally for clients who need
            // it...
            RetrySynchronizationManager.Register(context);

            Exception lastException = null;

            try
            {
                // Give clients a chance to enhance the context...
                var running = this.DoOpenInterceptors(retryCallback, context);

                if (!running)
                {
                    throw new TerminatedRetryException("Retry terminated abnormally by interceptor before first attempt");
                }

                // Get or Start the backoff context...
                IBackOffContext backOffContext = null;
                IAttributeAccessor attributeAccessor = null;
                if (context is IAttributeAccessor)
                {
                    attributeAccessor = context;
                    var resource = attributeAccessor.GetAttribute("backOffContext");
                    if (resource is IBackOffContext)
                    {
                        backOffContext = (IBackOffContext)resource;
                    }
                }

                if (backOffContext == null)
                {
                    backOffContext = backOffPolicy.Start(context);
                    if (attributeAccessor != null && backOffContext != null)
                    {
                        attributeAccessor.SetAttribute("backOffContext", backOffContext);
                    }
                }

                // We allow the whole loop to be skipped if the policy or context
                // already forbid the first try. This is used in the case of
                // external retry to allow a recovery in handleRetryExhausted
                // without the callback processing (which would throw an exception).
                while (this.CanRetry(retryPolicy, context) && !context.ExhaustedOnly)
                {
                    try
                    {
                        Logger.Debug(m => m("Retry: count={0}", context.RetryCount));

                        // Reset the last exception, so if we are successful
                        // the close interceptors will not think we failed...
                        lastException = null;
                        return retryCallback.DoWithRetry(context);
                    }
                    catch (Exception e)
                    {
                        lastException = e;

                        this.DoOnErrorInterceptors(retryCallback, context, e);

                        try
                        {
                            this.RegisterThrowable(retryPolicy, state, context, e);
                        }
                        catch (Exception ex)
                        {
                            throw new TerminatedRetryException("Could not register throwable", ex);
                        }

                        if (this.CanRetry(retryPolicy, context) && !context.ExhaustedOnly)
                        {
                            try
                            {
                                backOffPolicy.BackOff(backOffContext);
                            }
                            catch (BackOffInterruptedException ex)
                            {
                                lastException = e;

                                // back off was prevented by another thread - fail
                                // the retry
                                Logger.Debug(m => m("Abort retry because interrupted: count={0}", context.RetryCount));
                                throw;
                            }
                        }

                        Logger.Debug(m => m("Checking for rethrow: count={0}", context.RetryCount));
                        if (this.ShouldRethrow(retryPolicy, context, state))
                        {
                            Logger.Debug(m => m("Rethrow in retry for policy: count={0}", context.RetryCount));
                            throw WrapIfNecessary(e);
                        }
                    }

                 // A stateful attempt that can retry should have rethrown the
				 // exception by now - i.e. we shouldn't get this far for a
				 // stateful attempt if it can retry.
                }

                Logger.Debug(m => m("Retry failed last attempt: count={0}" + context.RetryCount));

                if (context.ExhaustedOnly)
                {
                    throw new ExhaustedRetryException("Retry exhausted after last attempt with no recovery path.", context.LastThrowable);
                }

                return this.HandleRetryExhausted(recoveryCallback, context, state);
            }
            finally
            {
                this.Close(retryPolicy, context, state, lastException == null);
                this.DoCloseInterceptors(retryCallback, context, lastException);
                RetrySynchronizationManager.Clear();
            }
        }

        /**
 * Decide whether to proceed with the ongoing retry attempt. This method is
 * called before the {@link RetryCallback} is executed, but after the
 * backoff and open interceptors.
 * 
 * @param retryPolicy the policy to apply
 * @param context the current retry context
 * @return true if we can continue with the attempt
 */

        /// <summary>The can retry.</summary>
        /// <param name="retryPolicy">The retry policy.</param>
        /// <param name="context">The context.</param>
        /// <returns>The System.Boolean.</returns>
        protected bool CanRetry(IRetryPolicy retryPolicy, IRetryContext context) { return retryPolicy.CanRetry(context); }
        
        /// <summary>Clean up the cache if necessary and close the context provided (if the flag indicates that processing was successful).</summary>
        /// <param name="retryPolicy">The retry policy.</param>
        /// <param name="context">The context.</param>
        /// <param name="state">The state.</param>
        /// <param name="succeeded">The succeeded.</param>
        protected void Close(IRetryPolicy retryPolicy, IRetryContext context, IRetryState state, bool succeeded)
        {
            if (state != null)
            {
                if (succeeded)
                {
                    this.retryContextCache.Remove(state.GetKey());
                    retryPolicy.Close(context);
                }
            }
            else
            {
                retryPolicy.Close(context);
            }
        }

        /// <summary>The register throwable.</summary>
        /// <param name="retryPolicy">The retry policy.</param>
        /// <param name="state">The state.</param>
        /// <param name="context">The context.</param>
        /// <param name="e">The e.</param>
        /// <exception cref="RetryException"></exception>
        protected void RegisterThrowable(IRetryPolicy retryPolicy, IRetryState state, IRetryContext context, Exception e)
        {
            if (state != null)
            {
                var key = state.GetKey();
                if (context.RetryCount > 0 && !this.retryContextCache.ContainsKey(key))
                {
                    throw new RetryException(
                        "Inconsistent state for failed item key: cache key has changed. "
                        + "Consider whether equals() or hashCode() for the key might be inconsistent, "
                        + "or if you need to supply a better key");
                }

                this.retryContextCache.Put(key, context);
            }

            retryPolicy.RegisterThrowable(context, e);
        }

        /// <summary>
        /// Delegate to the <see cref="IRetryPolicy"/> having checked in the cache for an existing value if the state is not null.
        /// </summary>
        /// <param name="retryPolicy">The retry policy.</param>
        /// <param name="state">The state.</param>
        /// <returns>The Spring.Retry.Retry.IRetryContext, either a new one or the one used last time the same state was encountered.</returns>
        protected IRetryContext Open(IRetryPolicy retryPolicy, IRetryState state)
        {
            if (state == null)
            {
                return this.DoOpenInternal(retryPolicy);
            }

            var key = state.GetKey();
            if (state.IsForceRefresh())
            {
                return this.DoOpenInternal(retryPolicy);
            }

            // If there is no cache hit we can avoid the possible expense of the
            // cache re-hydration.
            if (!this.retryContextCache.ContainsKey(key))
            {
                // The cache is only used if there is a failure.
                return this.DoOpenInternal(retryPolicy);
            }

            var context = this.retryContextCache.Get(key);
            if (context == null)
            {
                if (this.retryContextCache.ContainsKey(key))
                {
                    throw new RetryException(
                        "Inconsistent state for failed item: no history found. "
                        + "Consider whether equals() or hashCode() for the item might be inconsistent, "
                        + "or if you need to supply a better ItemKeyGenerator");
                }

                // The cache could have been expired in between calls to
                // containsKey(), so we have to live with this:
                return this.DoOpenInternal(retryPolicy);
            }

            return context;
        }

        private IRetryContext DoOpenInternal(IRetryPolicy retryPolicy) { return retryPolicy.Open(RetrySynchronizationManager.GetContext()); }

        /// <summary>
        /// Actions to take after final attempt has failed. If there is state clean 
        /// up the cache. If there is a recovery callback, execute that and return
        /// its result. Otherwise throw an exception.
        /// </summary>
        /// <param name="recoveryCallback">The callback for recovery (might be null).</param>
        /// <param name="context">The current retry context.</param>
        /// <param name="state">The state.</param>
        /// <returns>The T.</returns>
        protected T HandleRetryExhausted(IRecoveryCallback<T> recoveryCallback, IRetryContext context, IRetryState state)
        {
            if (state != null)
            {
                this.retryContextCache.Remove(state.GetKey());
            }

            if (recoveryCallback != null)
            {
                return recoveryCallback.Recover(context);
            }

            if (state != null)
            {
                Logger.Debug(m => m("Retry exhausted after last attempt with no recovery path."));
                throw new ExhaustedRetryException("Retry exhausted after last attempt with no recovery path", context.LastThrowable);
            }

            throw WrapIfNecessary(context.LastThrowable);
        }

        /// <summary>
        /// Extension point for subclasses to decide on behaviour after catching an
        /// exception in a <see cref="IRetryCallback{T}"/>. Normal stateless behaviour is not
        /// to rethrow, and if there is state we rethrow.
        /// </summary>
        /// <param name="retryPolicy">The retry policy.</param>
        /// <param name="context">The context.</param>
        /// <param name="state">The state.</param>
        /// <returns>The System.Boolean.</returns>
        protected bool ShouldRethrow(IRetryPolicy retryPolicy, IRetryContext context, IRetryState state)
        {
            if (state == null)
            {
                return false;
            }
            else
            {
                return state.RollbackFor(context.LastThrowable);
            }
        }

        private bool DoOpenInterceptors(IRetryCallback<T> callback, IRetryContext context)
        {
            var result = true;

            for (var i = 0; i < this.listeners.Length; i++)
            {
                result = result && this.listeners[i].Open(context, callback);
            }

            return result;
        }

        private void DoCloseInterceptors(IRetryCallback<T> callback, IRetryContext context, Exception lastException)
        {
            for (var i = this.listeners.Length; i-- > 0;)
            {
                this.listeners[i].Close(context, callback, lastException);
            }
        }

        private void DoOnErrorInterceptors(IRetryCallback<T> callback, IRetryContext context, Exception throwable)
        {
            for (var i = this.listeners.Length; i-- > 0;)
            {
                this.listeners[i].OnError(context, callback, throwable);
            }
        }
        
        private static Exception WrapIfNecessary(Exception throwable)
        {
            // if (throwable is Error) {
            //  throw (Error) throwable;
            // }
            // else 
            if (throwable is Exception)
            {
                return throwable;
            }
            else
            {
                return new RetryException("Exception in batch process", throwable);
            }
        }
    }
}
