// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatefulRetryOperationsInterceptor.cs" company="The original author or authors.">
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
using AopAlliance.Intercept;
using Common.Logging;
using Spring.Retry.Retry.Support;
using Spring.Util;
#endregion

namespace Spring.Retry.Retry.Interceptor
{
    /// <summary>
    /// A <see cref="IMethodInterceptor"/> that can be used to automatically retry calls to
    /// a method on a service if it fails. The argument to the service method is
    /// treated as an item to be remembered in case the call fails. So the retry
    /// operation is stateful, and the item that failed is tracked by its unique key
    /// (via <see cref="IMethodArgumentsKeyGenerator"/>) until the retry is exhausted, at
    /// which point the <see cref="IMethodInvocationRecoverer"/> is called.
    /// The main use case for this is where the service is transactional, via a
    /// transaction interceptor on the interceptor chain. In this case the retry (and
    /// recovery on exhausted) always happens in a new transaction.
    /// The injected <see cref="IRetryOperations"/> is used to control the number of
    /// retries. By default it will retry a fixed number of times, according to the
    /// defaults in <see cref="RetryTemplate"/>.
    /// </summary>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public class StatefulRetryOperationsInterceptor : IMethodInterceptor
    {
        [NonSerialized] private ILog logger = LogManager.GetCurrentClassLogger();

        private IMethodArgumentsKeyGenerator keyGenerator;

        private IMethodInvocationRecoverer recoverer;

        private INewMethodArgumentsIdentifier newMethodArgumentsIdentifier;

        private IRetryOperations retryOperations;

        public IRetryOperations RetryOperations
        {
            set
            {
                AssertUtils.ArgumentNotNull(value, "'retryOperations' cannot be null.");
                this.retryOperations = value;
            }
        }

        public StatefulRetryOperationsInterceptor() : base()
        {
            var retryTemplate = new RetryTemplate();
            retryTemplate.RetryPolicy = new NeverRetryPolicy();
            retryOperations = retryTemplate;
        }

        public IMethodInvocationRecoverer Recoverer { set { this.recoverer = value; } }

        public IMethodArgumentsKeyGenerator KeyGenerator { set { this.keyGenerator = value; } }

        public INewMethodArgumentsIdentifier NewMethodArgumentsIdentifier { set { this.newMethodArgumentsIdentifier = value; } }

        public object Invoke(IMethodInvocation invocation)
        {
            logger.Debug(m => m("Executing proxied method in stateful retry: {0}({1})", invocation.StaticPart, ObjectUtils.GetIdentityHexString(invocation)));

            var args = invocation.Arguments;
            AssertUtils.State(args.Length > 0, "Stateful retry applied to method that takes no arguments: " + invocation.StaticPart);
            object arg = args;
            if (args.Length == 1)
            {
                arg = args[0];
            }

            var item = arg;

            var retryState = new DefaultRetryState(
                keyGenerator != null ? keyGenerator.GetKey(args) : item,
                newMethodArgumentsIdentifier != null ? newMethodArgumentsIdentifier.IsNew(args) : false);

            var result = retryOperations.Execute(
                new MethodInvocationRetryCallback(invocation),
                new StatefulRetryOperationsItemRecovererCallback(args, recoverer),
                retryState);

            logger.Debug(m => m("Exiting proxied method in stateful retry with result: ({0})", result));

            return result;
        }
    }

    /// <summary>
    /// MethodInvocationRetryCallback for StatefulRetryOperationsInterceptor
    /// </summary>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    internal class MethodInvocationRetryCallback : IRetryCallback<object>
    {
        private readonly IMethodInvocation invocation;

        internal MethodInvocationRetryCallback(IMethodInvocation invocation) { this.invocation = invocation; }

        /// <summary>The do with retry.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The System.Object.</returns>
        public object DoWithRetry(IRetryContext context)
        {
            return this.invocation.Proceed();

            // try
            // {
            //     return this.invocation.Proceed();
            // }
            // catch (Exception e)
            // {
            //     throw e;
            // }
            // catch (Error e)
            // {
            //     throw e;
            // }
            // catch (Throwable e)
            // {
            //     throw new IllegalStateException(e);
            // }
        }
    }

    /// <summary>
    /// ItemRecovererCallback for StatefulRetryOperationsInterceptor
    /// </summary>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    internal class StatefulRetryOperationsItemRecovererCallback : IRecoveryCallback<object>
    {
        private readonly object[] args;

        private readonly IMethodInvocationRecoverer recoverer;

        internal StatefulRetryOperationsItemRecovererCallback(IEnumerable<object> args, IMethodInvocationRecoverer recoverer)
        {
            this.args = args == null ? new object[0] : new List<object>(args).ToArray();
            this.recoverer = recoverer;
        }

        /// <summary>The recover.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The System.Object.</returns>
        public object Recover(IRetryContext context)
        {
            if (this.recoverer != null)
            {
                return this.recoverer.Recover<object>(this.args, context.LastException);
            }

            throw new ExhaustedRetryException("Retry was exhausted but there was no recovery path.");
        }
    }
}
