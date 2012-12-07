// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeRetryPolicy.cs" company="The original author or authors.">
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
using Spring.Retry.Retry.Context;
#endregion

namespace Spring.Retry.Retry.Policy
{
    /// <summary>
    /// A <see cref="IRetryPolicy"/> that composes a list of other policies and delegates calls to them in order.
    /// </summary>
    /// <author>Dave Syer</author>
    /// <author>Michael Minella</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public class CompositeRetryPolicy : IRetryPolicy
    {
        internal IRetryPolicy[] policies = new IRetryPolicy[0];

        private bool optimistic;

        /// <summary>Sets the policies.</summary>
        public IRetryPolicy[] Policies { set { this.policies = value; } }

        /// <summary>Sets a value indicating whether optimistic.</summary>
        public bool Optimistic { set { this.optimistic = value; } }

        /// <summary>Delegate to the policies that were in operation when the context was
        /// created. If any of them cannot retry then return false, otherwise return true.<see cref="IRetryPolicy.CanRetry"/></summary>
        /// <param name="context">The context.</param>
        /// <returns>The System.Boolean.</returns>
        public bool CanRetry(IRetryContext context)
        {
            var contexts = ((CompositeRetryContext)context).contexts;
            var policies = ((CompositeRetryContext)context).policies;

            var retryable = true;

            if (this.optimistic)
            {
                retryable = false;
                for (var i = 0; i < contexts.Length; i++)
                {
                    if (policies[i].CanRetry(contexts[i]))
                    {
                        retryable = true;
                    }
                }
            }
            else
            {
                for (var i = 0; i < contexts.Length; i++)
                {
                    if (!policies[i].CanRetry(contexts[i]))
                    {
                        retryable = false;
                    }
                }
            }

            return retryable;
        }

        /// <summary>Delegate to the policies that were in operation when the context was
        /// created. If any of them fails to close the exception is propagated (and
        /// those later in the chain are closed before re-throwing).<see cref="IRetryPolicy.Close"/></summary>
        /// <param name="context">The context.</param>
        /// <exception cref="Exception"></exception>
        public void Close(IRetryContext context)
        {
            var contexts = ((CompositeRetryContext)context).contexts;
            var policies = ((CompositeRetryContext)context).policies;
            Exception exception = null;
            for (var i = 0; i < contexts.Length; i++)
            {
                try
                {
                    policies[i].Close(contexts[i]);
                }
                catch (Exception e)
                {
                    if (exception == null)
                    {
                        exception = e;
                    }
                }
            }

            if (exception != null)
            {
                throw exception;
            }
        }

        /// <summary>Creates a new context that copies the existing policies and keeps a list of the contexts from each one. <see cref="IRetryPolicy.Open"/></summary>
        /// <param name="parent">The parent.</param>
        /// <returns>The Spring.Retry.Retry.IRetryContext.</returns>
        public IRetryContext Open(IRetryContext parent)
        {
            var list = new List<IRetryContext>();
            for (var i = 0; i < this.policies.Length; i++)
            {
                list.Add(this.policies[i].Open(parent));
            }

            return new CompositeRetryContext(parent, list, this);
        }

        /// <summary>Delegate to the policies that were in operation when the context was created. <see cref="IRetryPolicy.Close"/></summary>
        /// <param name="context">The context.</param>
        /// <param name="exception">The exception.</param>
        public void RegisterException(IRetryContext context, Exception exception)
        {
            var contexts = ((CompositeRetryContext)context).contexts;
            var policies = ((CompositeRetryContext)context).policies;
            for (var i = 0; i < contexts.Length; i++)
            {
                policies[i].RegisterException(contexts[i], exception);
            }

            ((RetryContextSupport)context).RegisterException(exception);
        }
    }

    internal class CompositeRetryContext : RetryContextSupport
    {
        internal IRetryContext[] contexts;

        internal readonly IRetryPolicy[] policies;

        private readonly CompositeRetryPolicy outer;

        /// <summary>Initializes a new instance of the <see cref="CompositeRetryContext"/> class.</summary>
        /// <param name="parent">The parent.</param>
        /// <param name="contexts">The contexts.</param>
        /// <param name="outer">The outer.</param>
        public CompositeRetryContext(IRetryContext parent, List<IRetryContext> contexts, CompositeRetryPolicy outer) : base(parent)
        {
            this.contexts = contexts.ToArray();
            this.policies = this.outer.policies;
            this.outer = outer;
        }
    }
}
