// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RetryOperationsInterceptor.cs" company="The original author or authors.">
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
using Spring.Aop;
using Spring.Retry.Retry.Support;
using Spring.Util;
#endregion

namespace Spring.Retry.Retry.Interceptor
{
    /// <summary>
    /// A <see cref="IMethodInterceptor"/> that can be used to automatically retry calls to a method on a service if it fails. The
    /// injected <see cref="IRetryOperations{T}"/> is used to control the number of retries. By default it will retry a fixed number of
    /// times, according to the defaults in <see cref="RetryTemplate{T}"/>. 
    /// Hint about transaction boundaries. If you want to retry a failed transaction you need to make sure that the
    /// transaction boundary is inside the retry, otherwise the successful attempt will roll back with the whole transaction.
    /// If the method being intercepted is also transactional, then use the ordering hints in the advice declarations to
    /// ensure that this one is before the transaction interceptor in the advice chain.
    /// </summary>
    /// <author>Rob Harrop</author>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public class RetryOperationsInterceptor : IMethodInterceptor
    {
        private IRetryOperations retryOperations = new RetryTemplate();
        private IMethodInvocationRecoverer recoverer;

        public IRetryOperations RetryOperations
        {
            set
            {
                AssertUtils.ArgumentNotNull(value, "'retryOperations' cannot be null.");
                this.retryOperations = value;
            }
        }

        public IMethodInvocationRecoverer Recoverer { set { this.recoverer = value; } }

        public object Invoke(IMethodInvocation invocation)
        {
            var retryCallback = new Func<IRetryContext, object>(
                (context) =>
                {
                    /*
				 * If we don't copy the invocation carefully it won't keep a reference to the other interceptors in the
				 * chain. We don't have a choice here but to specialise to ReflectiveMethodInvocation (but how often
				 * would another implementation come along?).
				 */
                    if (invocation is IProxyMethodInvocation)
                    {
                        try
                        {
                            return ((IProxyMethodInvocation)invocation).InvocableClone().Proceed();
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        /*catch (Error e)
                    {
                        throw e;
                    }
                    catch (Throwable e)
                    {
                        throw new IllegalStateException(e);
                    }*/
                    }
                    else
                    {
                        throw new InvalidOperationException("MethodInvocation of the wrong type detected - this should not happen with Spring AOP for .NET, so please raise an issue if you see this exception");
                    }
                });

            if (recoverer != null)
            {
                var recoveryCallback = new ItemRecovererCallback(invocation.Arguments, recoverer);
                return this.retryOperations.Execute(retryCallback, (IRecoveryCallback<object>)recoveryCallback);
            }

            return this.retryOperations.Execute(retryCallback);
        }
    }

    internal class ItemRecovererCallback : IRecoveryCallback<object>
    {
        private readonly object[] args;

        private readonly IMethodInvocationRecoverer recoverer;

        internal ItemRecovererCallback(object[] args, IMethodInvocationRecoverer recoverer)
        {
            this.args = args == null ? new object[0] : new List<object>(args).ToArray();
            this.recoverer = recoverer;
        }

        public object Recover(IRetryContext context)
        {
            if (recoverer != null)
            {
                return recoverer.Recover<object>(args, context.LastException);
            }
            throw new ExhaustedRetryException("Retry was exhausted but there was no recovery path.");
        }
    }
}
