// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RetrySynchronizationManager.cs" company="The original author or authors.">
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
using System.Threading;
#endregion

namespace Spring.Retry.Retry.Support
{
    /// <summary>
    /// Global variable support for retry clients. Normally it is not necessary for
    /// clients to be aware of the surrounding environment because a
    /// <see cref="IRetryCallback{T}"/> can always use the context it is passed by the
    /// enclosing <see cref="IRetryOperations{T}"/>. But occasionally it might be helpful to
    /// have lower level access to the ongoing <see cref="IRetryContext"/> so we provide a
    /// global accessor here. The mutator methods ({@link #clear()} and
    /// </summary>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public class RetrySynchronizationManager
    {
        private RetrySynchronizationManager() { }

        private static readonly ThreadLocal<IRetryContext> context = new ThreadLocal<IRetryContext>();
        
        /// <summary>Public accessor for the locally enclosing <see cref="IRetryContext"/>.</summary>
        /// <returns>The current retry context, or null if there isn't one.</returns>
        public static IRetryContext GetContext()
        {
            if (context.IsValueCreated)
            {
                var result = context.Value;
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Method for registering a context - should only be used by
        /// <see cref="IRetryOperations{T}"/> implementations to ensure that
        /// <see cref="GetContext"/> always returns the correct value.
        /// </summary>
        /// <param name="context">The new context to register.</param>
        /// <returns>The old context if there was one.</returns>
        public static IRetryContext Register(IRetryContext context)
        {
            var oldContext = GetContext();
            RetrySynchronizationManager.context.Value = context;
            return oldContext;
        }

        /// <summary>
        /// Clear the current context at the end of a batch - should only be used by
        /// <see cref="IRetryOperations{T}"/> implementations.
        /// </summary>
        /// <returns>The old value if there was one.</returns>
        public static IRetryContext Clear()
        {
            var value = GetContext();
            var parent = value == null ? null : value.GetParent();
            context.Value = parent;
            return value;
        }
    }
}
