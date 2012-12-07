// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RetryContextSupport.cs" company="The original author or authors.">
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
using Spring.Core;
#endregion

namespace Spring.Retry.Retry.Context
{
    /// <summary>
    /// Retry Context Support
    /// </summary>
    /// <author>Joe Fitzgerald (.NET)</author>
    public class RetryContextSupport : AttributeAccessorSupport, IRetryContext
    {
        private bool terminate;

        private int count;

        private Exception lastException;

        private readonly IRetryContext parent;

        /// <summary>Initializes a new instance of the <see cref="RetryContextSupport"/> class.</summary>
        /// <param name="parent">The parent.</param>
        public RetryContextSupport(IRetryContext parent) { this.parent = parent; }

        /// <summary>The get parent.</summary>
        /// <returns>The Spring.Retry.Retry.IRetryContext.</returns>
        public IRetryContext GetParent() { return this.parent; }

        /// <summary>Gets or sets a value indicating whether exhausted only.</summary>
        public bool ExhaustedOnly { get { return this.terminate; } set { this.terminate = value; } }

        /// <summary>Gets the retry count.</summary>
        public int RetryCount { get { return this.count; } }

        /// <summary>Gets the last exception.</summary>
        public Exception LastException { get { return this.lastException; } }

        /// <summary>Set the exception for the public interface <see cref="IRetryContext"/>, and
        /// also increment the retry count if the throwable is non-null.
        /// All <see cref="IRetryPolicy"/> implementations should use this method when they
        /// register the exception. It should only be called once per retry attempt
        /// because it increments a counter.
        /// Use of this method is not enforced by the framework - it is a service provider contract for authors of policies.</summary>
        /// <param name="exception">The exception that caused the current retry attempt to fail.</param>
        public void RegisterException(Exception exception)
        {
            this.lastException = exception;
            if (exception != null)
            {
                this.count++;
            }
        }

        /// <summary>The to string.</summary>
        /// <returns>The System.String.</returns>
        public override string ToString() { return string.Format("[RetryContext: count={0}, lastException={1}, exhausted={2}]", this.count, this.lastException, this.terminate); }
    }
}
