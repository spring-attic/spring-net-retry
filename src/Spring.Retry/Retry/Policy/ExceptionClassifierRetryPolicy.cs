// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionClassifierRetryPolicy.cs" company="The original author or authors.">
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
using Spring.Retry.Classify;
using Spring.Retry.Retry.Context;
using Spring.Retry.Retry.Support;
using Spring.Util;
#endregion

namespace Spring.Retry.Retry.Policy
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ExceptionClassifierRetryPolicy : IRetryPolicy
    {
        private IClassifier<Exception, IRetryPolicy> exceptionClassifier = new ClassifierSupport<Exception, IRetryPolicy>(new NeverRetryPolicy());

        /// <summary>
        /// Setter for an exception classifier. The classifier is responsible for
        /// translating exceptions to concrete retry policies. Either this property
        /// or the policy map should be used, but not both.
        /// </summary>
        public IClassifier<Exception, IRetryPolicy> ExceptionClassifier { set { this.exceptionClassifier = value; } }

        /// <summary>Setter for policy map used to create a classifier. Either this property
        /// or the exception classifier directly should be set, but not both.</summary>
        /// <param name="policyMap">A map of Exception type to <see cref="IRetryPolicy"/> that will be used to create a <see cref="IClassifier{C,T}"/> to locate a policy.</param>
        public void SetPolicyMap(IDictionary<Type, IRetryPolicy> policyMap)
        {
            var subclassClassifier = new SubclassClassifier<Exception, IRetryPolicy>(policyMap, new NeverRetryPolicy());
            this.ExceptionClassifier = subclassClassifier;
        }

        /// <summary>Delegate to the policy currently activated in the context. <see cref="IRetryPolicy.CanRetry"/></summary>
        /// <param name="context">The context.</param>
        /// <returns>The System.Boolean.</returns>
        public bool CanRetry(IRetryContext context)
        {
            var policy = (IRetryPolicy)context;
            return policy.CanRetry(context);
        }

        /// <summary>Delegate to the policy currently activated in the context. <see cref="IRetryPolicy.Close"/></summary>
        /// <param name="context">The context.</param>
        public void Close(IRetryContext context)
        {
            var policy = (IRetryPolicy)context;
            policy.Close(context);
        }

        /// <summary>Create an active context that proxies a retry policy by chosing a target from the policy map. <see cref="IRetryPolicy.Open"/></summary>
        /// <param name="parent">The parent.</param>
        /// <returns>The Spring.Retry.Retry.IRetryContext.</returns>
        public IRetryContext Open(IRetryContext parent) { return new ExceptionClassifierRetryContext(parent, this.exceptionClassifier).Open(parent); }

        /// <summary>The register exception.</summary>
        /// <param name="context">The context.</param>
        /// <param name="exception">The exception.</param>
        public void RegisterException(IRetryContext context, Exception exception)
        {
            var policy = (IRetryPolicy)context;
            policy.RegisterException(context, exception);
            ((RetryContextSupport)context).RegisterException(exception);
        }
    }

    internal class ExceptionClassifierRetryContext : RetryContextSupport, IRetryPolicy
    {
        private readonly IClassifier<Exception, IRetryPolicy> exceptionClassifier;

        // Dynamic: depends on the latest exception:
        private IRetryPolicy policy;

        // Dynamic: depends on the policy:
        private IRetryContext context;

        private readonly IDictionary<IRetryPolicy, IRetryContext> contexts = new Dictionary<IRetryPolicy, IRetryContext>();

        /// <summary>Initializes a new instance of the <see cref="ExceptionClassifierRetryContext"/> class.</summary>
        /// <param name="parent">The parent.</param>
        /// <param name="exceptionClassifier">The exception classifier.</param>
        public ExceptionClassifierRetryContext(IRetryContext parent, IClassifier<Exception, IRetryPolicy> exceptionClassifier) : base(parent) { this.exceptionClassifier = exceptionClassifier; }

        /// <summary>The can retry.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The System.Boolean.</returns>
        public bool CanRetry(IRetryContext context)
        {
            if (this.context == null)
            {
                // there was no error yet
                return true;
            }

            return this.policy.CanRetry(this.context);
        }

        /// <summary>The close.</summary>
        /// <param name="context">The context.</param>
        public void Close(IRetryContext context)
        {
            // Only close those policies that have been used (opened):
            foreach (var policy in this.contexts.Keys)
            {
                policy.Close(this.GetContext(policy, context.GetParent()));
            }
        }

        /// <summary>The open.</summary>
        /// <param name="parent">The parent.</param>
        /// <returns>The Spring.Retry.Retry.IRetryContext.</returns>
        public IRetryContext Open(IRetryContext parent) { return this; }

        /// <summary>The register exception.</summary>
        /// <param name="context">The context.</param>
        /// <param name="exception">The exception.</param>
        public void RegisterException(IRetryContext context, Exception exception)
        {
            this.policy = this.exceptionClassifier.Classify(exception);
            AssertUtils.ArgumentNotNull(this.policy, "Could not locate policy for exception=[" + exception + "].");
            this.context = this.GetContext(this.policy, context.GetParent());
            this.policy.RegisterException(this.context, exception);
        }

        private IRetryContext GetContext(IRetryPolicy policy, IRetryContext parent)
        {
            IRetryContext context;
            this.contexts.TryGetValue(policy, out context);
            if (context == null)
            {
                context = policy.Open(parent);
                this.contexts.Add(policy, context);
            }

            return context;
        }
    }
}
