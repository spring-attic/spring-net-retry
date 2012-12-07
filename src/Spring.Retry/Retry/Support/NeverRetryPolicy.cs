// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NeverRetryPolicy.cs" company="The original author or authors.">
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
using Spring.Retry.Retry.Context;
#endregion

namespace Spring.Retry.Retry.Support
{
    /// <summary>
    /// A <see cref="IRetryPolicy"/> that allows the first attempt but never permits a
    /// retry. Also be used as a base class for other policies, e.g. for test
    /// purposes as a stub.
    /// </summary>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public class NeverRetryPolicy : IRetryPolicy
    {
        /// <summary>Returns false after the first exception. So there is always one try, and then the retry is prevented. <see cref="IRetryContext"/></summary>
        /// <param name="context">The context.</param>
        /// <returns>The System.Boolean.</returns>
        public virtual bool CanRetry(IRetryContext context) { return !((NeverRetryContext)context).IsFinished; }

        /// <summary>Do nothing. <see cref="IRetryContext"/></summary>
        /// <param name="context">The context.</param>
        public virtual void Close(IRetryContext context)
        {
            // no-op
        }

        /// <summary>Return a context that can respond to early termination requests, but does nothing else. <see cref="IRetryPolicy.Open"/></summary>
        /// <param name="parent">The parent.</param>
        /// <returns>The Spring.Retry.Retry.IRetryContext.</returns>
        public virtual IRetryContext Open(IRetryContext parent) { return new NeverRetryContext(parent); }

        /// <summary>Make the exception available for downstream use through the context. <see cref="IRetryPolicy.RegisterException"/></summary>
        /// <param name="context">The context.</param>
        /// <param name="exception">The exception.</param>
        public virtual void RegisterException(IRetryContext context, Exception exception)
        {
            ((NeverRetryContext)context).SetFinished();

            ((RetryContextSupport)context).RegisterException(exception);
        }
    }

    /// <summary>
    /// Special context object for <see cref="NeverRetryPolicy"/>. Implements a flag
    /// with a similar function to <see cref="IRetryContext.ExhaustedOnly"/>, but
    /// kept separate so that if subclasses of <see cref="NeverRetryPolicy"/> need to
    /// they can modify the behaviour of
    /// <see cref="NeverRetryPolicy.CanRetry"/> without affecting
    /// <see cref="IRetryContext.ExhaustedOnly"/>.
    /// </summary>
    internal class NeverRetryContext : RetryContextSupport
    {
        private bool finished;

        /// <summary>Initializes a new instance of the <see cref="NeverRetryContext"/> class.</summary>
        /// <param name="parent">The parent.</param>
        public NeverRetryContext(IRetryContext parent) : base(parent) { }

        /// <summary>Gets or sets a value indicating whether is finished.</summary>
        public bool IsFinished { get { return this.finished; } }

        /// <summary>The set finished.</summary>
        public void SetFinished() { this.finished = true; }
    }
}
