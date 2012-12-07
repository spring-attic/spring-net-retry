// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleRetryPolicy.cs" company="The original author or authors.">
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
#endregion

namespace Spring.Retry.Retry.Policy
{
    /// <summary>
    /// Simple retry policy that retries a fixed number of times for a set of named
    /// exceptions (and subclasses). The number of attempts includes the initial try,
    /// so e.g.
    /// <code>
    /// retryTemplate = new RetryTemplate(new SimpleRetryPolicy(3));
    /// retryTemplate.Execute(callback);
    /// </code>
    /// will execute the callback at least once, and as many as 3 times.
    /// </summary>
    /// <author>Dave Syer</author>
    /// <author>Rob Harrop</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public class SimpleRetryPolicy : IRetryPolicy
    {
        // The default limit to the number of attempts for a new policy.
        public static readonly int DEFAULT_MAX_ATTEMPTS = 3;

        private volatile int maxAttempts;

        private readonly BinaryExceptionClassifier retryableClassifier = new BinaryExceptionClassifier(false);

        /// <summary>Initializes a new instance of the <see cref="SimpleRetryPolicy"/> class.</summary>
        public SimpleRetryPolicy() : this(DEFAULT_MAX_ATTEMPTS, new Dictionary<Type, bool> { { typeof(Exception), true } }) { }

        /// <summary>Initializes a new instance of the <see cref="SimpleRetryPolicy"/> class.</summary>
        /// <param name="maxAttempts">The max attempts.</param>
        /// <param name="retryableExceptions">The retryable exceptions.</param>
        public SimpleRetryPolicy(int maxAttempts, IDictionary<Type, bool> retryableExceptions)
        {
            this.maxAttempts = maxAttempts;
            this.retryableClassifier = new BinaryExceptionClassifier(retryableExceptions);
        }

        /// <summary>Gets or sets the maximum number of retry attempts before failure.</summary>
        public int MaxAttempts { get { return this.maxAttempts; } set { this.maxAttempts = value; } }

        /// <summary>Test for retryable operation based on the status.</summary>
        /// <param name="context">The context.</param>
        /// <returns>True if the last exception was retryable and the number of attempts so far is less than the limit.</returns>
        public bool CanRetry(IRetryContext context)
        {
            var t = context.LastException;
            return (t == null || this.RetryForException(t)) && context.RetryCount < this.maxAttempts;
        }

        /// <summary>The close.</summary>
        /// <param name="context">The context.</param>
        public void Close(IRetryContext context) { }

        /// <summary>Update the status with another attempted retry and the latest exception.</summary>
        /// <param name="context">The context.</param>
        /// <param name="exception">The throwable.</param>
        public void RegisterException(IRetryContext context, Exception exception)
        {
            var simpleContext = (SimpleRetryContext)context;
            simpleContext.RegisterException(exception);
        }

        /// <summary>Get a status object that can be used to track the current operation
        /// according to this policy. Has to be aware of the latest exception and the
        /// number of attempts.</summary>
        /// <param name="parent">The parent.</param>
        /// <returns>The Spring.Retry.Retry.IRetryContext.</returns>
        public IRetryContext Open(IRetryContext parent) { return new SimpleRetryContext(parent); }

        /// <summary>Delegates to an exception classifier.</summary>
        /// <param name="ex">The Exception that causes retry.</param>
        /// <returns>True if this exception or its ancestors have been registered as retryable.</returns>
        private bool RetryForException(Exception ex) { return this.retryableClassifier.Classify(ex); }

        /// <summary>The to string.</summary>
        /// <returns>The System.String.</returns>
        public override string ToString() { return this.GetType().Name + "[maxAttempts=" + this.maxAttempts + "]"; }
    }

    internal class SimpleRetryContext : RetryContextSupport
    {
        /// <summary>Initializes a new instance of the <see cref="SimpleRetryContext"/> class.</summary>
        /// <param name="parent">The parent.</param>
        public SimpleRetryContext(IRetryContext parent) : base(parent) { }
    }
}
